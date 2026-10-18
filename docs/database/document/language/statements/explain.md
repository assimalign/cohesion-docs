# EXPLAIN

Returns the chosen OQL plan without executing its SELECT, INSERT or DELETE.

## Syntax

```syntaxsql
EXPLAIN { select_statement | insert_statement | delete_statement } [ ; ]
```

## Remarks

Normal binding, parameter validation and provisioning admission apply to the wrapped statement. No
document is read by its data executor or inserted, even if the surrounding transaction commits. DDL,
nested EXPLAIN and ANALYZE are excluded with `COHDBL001`. Planning failures keep their codes.

The [shared eight-column shape](../../../sql/language/statements/explain.md) is node_id, parent_id,
operator, target, index, detail, estimated_rows and actual_rows. Nodes appear in depth-first
pre-order; estimates and actual counts are NULL. Operators include CollectionScan, IndexSeek,
SystemCollection, Insert and InsertSelect. IndexSeek reports its selected index and bounds.
Documents sends one JSON Document per row, including explicit null members. A test guard checks
that every plan record has a description mapping; this does not admit DDL under EXPLAIN.

VALUE/RAW uses ProjectValue for an ungrouped projection. DISTINCT adds a parent Distinct above
the projection or Group stage, without changing the shared result columns.

EXPLAIN DELETE emits a Delete root with a CollectionScan or IndexSeek child and the chosen filter/access
detail. With the by_score index, `EXPLAIN DELETE FROM items WHERE score >= 2` describes Delete
with detail `filter=True; access=IndexSeek` and its by_score seek, without deleting documents.

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

## Sources

- **Description and no execution** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentExplainTests.cs`.
- **JSON rows** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Client/tests/DocumentExplainWireTests.cs`.
