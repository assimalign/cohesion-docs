# EXPLAIN

Returns the chosen OQL plan without executing its SELECT, INSERT, UPSERT, UPDATE or DELETE.

## Syntax

```syntaxsql
EXPLAIN { select_statement | insert_statement | upsert_statement | update_statement | delete_statement } [ ; ]
```

## Remarks

Normal binding, parameter validation and provisioning admission apply to the wrapped statement. No
document is read by its data executor or inserted, even if the surrounding transaction commits. DDL,
nested EXPLAIN and ANALYZE are excluded with `CK00001`. Planning failures keep their codes.

The [shared eight-column shape](../../../sql/language/statements/explain.md) is node_id, parent_id,
operator, target, index, detail, estimated_rows and actual_rows. Nodes appear in depth-first
pre-order; available index-entry estimates are populated and actual counts remain NULL.
Operators include CollectionScan, IndexSeek, IndexIntersection,
SystemCollection, Insert, InsertSelect, Upsert, UpsertSelect, Update and Delete. IndexSeek reports its selected index and bounds.
Documents sends one JSON Document per row, including explicit null members. A test guard checks
that every plan record has a description mapping; this does not admit DDL under EXPLAIN.

VALUE/RAW uses ProjectValue for an ungrouped projection. DISTINCT adds a parent Distinct above
the projection or Group stage, without changing the shared result columns.

EXPLAIN DELETE emits a Delete root with a CollectionScan or IndexSeek child and the chosen filter/access
detail. With the by_score index, `EXPLAIN DELETE FROM items WHERE score >= 2` describes Delete
with detail `filter=True; access=IndexSeek` and its by_score seek, without deleting documents.

EXPLAIN UPDATE emits an Update root and its CollectionScan/IndexSeek child. For example,
`EXPLAIN UPDATE items SET score = score + 1 WHERE score >= 2` describes the filter and chosen
access without changing scores.

EXPLAIN UPSERT returns an Upsert root for VALUES or UpsertSelect above its source SELECT plan.
`EXPLAIN UPSERT INTO items (KEY, VALUE) VALUES ('a', 1);` describes Upsert without replacing a.

LIMIT/OFFSET counts are validated without reads and appear as `limit=...; offset=...` in Select
detail. Composite IndexSeek detail prints every equality-prefix and next-path range bound, such as
`a = 1 AND b = 2 AND c <= 3`.

[UNNEST](../clauses/unnest.md) adds Unnest/LeftUnnest nodes between projection/grouping and access,
with each variable in Detail. Virtual-source expansion uses SystemCollectionScan. For example,
`EXPLAIN SELECT t FROM items d LEFT UNNEST d.tags t` describes LeftUnnest without expansion.

Array IndexSeek detail renders normalized DISTINCT ARRAY keys and
`distinct_documents=True; array_residual=True`, with posting-count estimates. Candidate
identities deduplicate before fetching, and the complete predicate remains a residual.
Array/scalar accesses can participate in IndexIntersection. See
[array access and fetch bounds](create-index.md#array-access-and-fetch-bounds).

## Examples

For items containing one document and an index by_score on score:

```sql
EXPLAIN SELECT name FROM items WHERE score >= 2 AND score < 10;
EXPLAIN INSERT INTO items (KEY, VALUE) VALUES ('fresh', 1);
```

The first description includes IndexSeek, target items, index by_score and detail `score >= 2 AND
score < 10`. The second includes Insert; no fresh document is created.

## See also

[Statements](index.md) · [SELECT](select.md) · [INSERT](insert.md) · [DatabasePlanDescription](../../../../dotnet-apis/resources/database/assimalign-cohesion-database-execution/database-plan-description.md)

## Probe costs and indexed intersection

Bounded snapshot-visible index probes choose among eligible accesses; equal costs retain equality-prefix, range, UNIQUE and ordinal-name ties. Comparable complete indexed predicates may intersect ordered document identities before JSON reads, then apply the full predicate. Incomplete inputs keep a single driver. No persisted statistics or ANALYZE command exists.

EXPLAIN can read index metadata for costing but executes no document statement. IndexSeek estimates identify exact visible index-entry counts or lower bounds; IndexIntersection reports the minimum input count as an upper bound. actual_rows stays NULL. See [access design](../../../../dotnet-apis/resources/database/assimalign-cohesion-database-documents/design.md#bounded-access-probes-and-intersection).

## Transaction controls

EXPLAIN of BEGIN/START, COMMIT or ROLLBACK reports CK00001. Text transaction control itself
executes in process and through the Documents client; CD07002 requires negotiated transaction-status.
See [transactions](transactions.md).

## Sources

- **Description and no execution** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentExplainTests.cs`.
- **JSON rows** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Client/tests/DocumentExplainWireTests.cs`.
