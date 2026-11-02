# DELETE

Deletes matching documents from one collection while retaining its definition and indexes.

## Syntax

```syntaxsql
DELETE FROM collection_name [ [ AS ] iteration_variable ]
[ WHERE predicate ] [ RETURNING projection ] [ ; ]
```

## Arguments

- **`collection_name`** — One unqualified stored collection in the session database.
- **`iteration_variable`** — An optional source alias for paths and META(alias).
- **`predicate`** — An ordinary OQL filter; omission selects every document.

## Remarks

WHERE uses [SQL++ truth conversion](../expressions/missing.md): NULL, MISSING and absent fields
select no documents; nonzero numbers and nonempty values convert to TRUE. Scalar comparisons,
structural equality, CASE, parameters and META execute as in SELECT filters. Subqueries execute;
aggregates in the outer predicate are excluded. All parameters and metadata contexts are validated
even on an empty
collection or in a branch that would not run.

The planner uses the existing path equality/range index seek when applicable, otherwise a collection
scan. The complete predicate is reapplied to candidates, so a residual condition still controls
deletion. META().id/META(alias).id addresses the external document key but remains a residual scan;
an index on a content field named id does not turn it into a key lookup.

Matching identities are materialized from the statement snapshot before the first write, in ordinal
identity order. Deletions update content and indexes in one transaction, so changing index entries
cannot change the candidate set. Rollback restores documents and index visibility; an older snapshot
can still read deleted documents. A later conflict or predicate failure rolls back earlier deletions.
A failure in an explicit transaction aborts all its work. Cancellation while waiting for the writer
rolls back the statement. Re-inserting a deleted key receives a higher version.

Without WHERE, every document is removed; the collection and its index definitions remain. A
schema-owned stored collection permits content deletion. System collections are read-only.
Success returns the deleted-document count, including zero. The wire sends that affected count in
Complete with no Document frames when RETURNING is omitted. Client QueryAsync then returns empty Columns, Documents
and rows with AffectedCount set; ExecuteAsync returns the count directly.

[RETURNING](../clauses/returning.md) projects each pre-image and its old metadata in ordinal
document-ID order, with the deleted count. Projection failure rolls back the deletions.

Pending writes share [StatementMemoryBudget](../functions/aggregate-functions.md#statement-memory)
with SELECT staging and RETURNING. The complete pending buffer is charged before mutation;
exhaustion reports CD05018 (no assigned SQLSTATE) before any write. Reservations release on
success, failure and cancellation.

[Subqueries](../expressions/subqueries.md#mutation-subqueries) execute in this statement,
including RETURNING. Nested reads see statement-start data and earlier transaction statements,
never this statement's writes.
The shared work limit reports CD05020 (no assigned SQLSTATE) on exhaustion.

## Diagnostics

| Code | Meaning and trigger |
| --- | --- |
| `CD00002` | Missing FROM, database-qualified target, malformed predicate or leftover syntax |
| `CK00001` | USE KEYS or DELETE LIMIT; reported at the clause token |
| `CD05001` | Invalid META alias/context or aggregate in the predicate |
| `CD01002` | Target collection does not exist |
| `CD04004` | Target is a read-only system collection |
| `CD01005` | Referenced parameter is unbound, including on empty input |
| `CD02006` (22012) | Division/remainder by zero while evaluating the predicate |

Other expression diagnostics retain their [normal meanings](../diagnostics.md). A concurrent
matched-key write can raise DatabaseTransactionAbortedException. No new diagnostic code, catalog
format or protocol version is introduced.

## Examples

Execute each statement separately:

```sql
CREATE COLLECTION deletions;
INSERT INTO deletions (KEY, VALUE) VALUES
  ('a', {amount: 2, category: 'a'}), ('b', {amount: 6, category: 'a'}),
  ('c', {amount: 4, category: 'b'}), ('d', {amount: NULL});
CREATE INDEX by_amount ON deletions (amount);
DELETE FROM deletions d WHERE d.amount >= 4 AND d.category = 'a';
SELECT META().id AS id FROM deletions ORDER BY id;
DELETE FROM deletions WHERE MISSING;
DELETE FROM deletions;
```

The first DELETE affects one document, b. SELECT returns a, c and d. The next DELETE affects
zero, and the final DELETE affects three; the collection and by_amount index remain. The same
range deletion produces the same results without by_amount.

## See also

[Statements](index.md) · [UPDATE](update.md) · [WHERE](../clauses/where.md) · [META](../functions/meta.md) · [EXPLAIN](explain.md) · [DROP COLLECTION](drop-collection.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentDeleteTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlDmlParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Client/tests/DocumentClientWireTests.cs`.
