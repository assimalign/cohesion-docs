# EXPLAIN (Cohesion SQL)

Returns the chosen plan without executing the wrapped statement.

> **Status:** Implemented for data statements.

## Syntax

```syntaxsql
EXPLAIN { select_statement | insert_statement | update_statement | delete_statement } [ ; ]
```

## Remarks

EXPLAIN performs the statement's normal parameter validation, name binding, planning and
provisioning admission. It waits for the same reserved tables as that statement. It never calls its
data executor, even inside a transaction that later commits. DDL, transaction control, nested
EXPLAIN and EXPLAIN ANALYZE report `COHDBL001`. Ordinary planning errors retain their codes.

All three query languages use the [DatabasePlanDescription](../../../../dotnet-apis/resources/database/assimalign-cohesion-database-execution/database-plan-description.md)
shape, in depth-first pre-order:

| Column | Type | Nullable |
| --- | --- | --- |
| node_id | Int32 | No |
| parent_id | Int32 | Yes; root has NULL |
| operator | String | No |
| target | String | Yes |
| index | String | Yes |
| detail | String | Yes |
| estimated_rows | Int64 | Yes; always NULL |
| actual_rows | Int64 | Yes; always NULL |

SQL operators include TableScan, IndexSeek, SystemView, NestedLoopJoin, JoinStep, Group, Insert, InsertSelect,
Update and Delete. IndexSeek includes the chosen index and bounds. These are model descriptions, not
an execution API. SQL carries the rows through ordinary result frames. A source test guard requires
every concrete plan record to have a description mapping.
N-way joins expose one JoinStep child per join with its kind and scan/seek access, including
steps whose inputs will be empty. Plan output uses the same eight columns.
Derived inputs add a DerivedTable node with their child plan. UPDATE/DELETE predicate subqueries
are described beneath Subqueries or CorrelatedSubqueries; EXPLAIN does not materialize their data or mutate the target.

[WITH](../clauses/with.md) descriptions add With, Materialize and CteScan operators. Constant
marks a FROM-less CTE body. `EXPLAIN WITH a AS (SELECT 1 AS id) SELECT * FROM a` yields
With → Materialize → Constant, followed by Select → CteScan, without evaluating the body.
CorrelatedSubqueries retains the complete child plans evaluated for each outer row.

[Set-operation](../clauses/set-operations.md) bodies use Union, Intersect and Except nodes with
operand plans beneath them. Compatible same-quantifier UNION chains may have N children.
Combined ordering/paging and DISTINCT state appear in detail. For example, after creating
`a(id INT)` and `b(id INT)`, `EXPLAIN SELECT id FROM a UNION ALL SELECT id FROM b;`
returns a Union node with two Select/TableScan operand plans, without reading their rows.

## Examples

On the [fixture](select.md#a-create-the-conformance-fixture):

```sql
EXPLAIN DELETE FROM t WHERE id = 1;
SELECT id FROM t ORDER BY id;
```

EXPLAIN returns eight-column plan rows including a Delete operator. The SELECT still returns `1`,
`2`, `3`. An indexed `EXPLAIN SELECT id FROM t WHERE age >= 20 AND age < 40` reports IndexSeek with
that bound when an eligible age index exists.

With the [three-input JOIN fixture](../clauses/join.md#left-using-and-computed-output):

```sql
EXPLAIN SELECT * FROM a JOIN b ON a.id = b.id LEFT JOIN c ON b.id = c.id;
```

The description contains two JoinStep children, one Inner and one LeftOuter, with table scans
because this fixture defines no indexes. It returns plan rows without reading or changing data rows.

## See also

[Statements](index.md) · [SELECT](select.md) · [Diagnostics](../diagnostics.md)

## Sources

- **Plans and no execution** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlExplainTests.cs`.
- **Shape** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Execution/src/DatabasePlanDescription.cs`.
- **Join steps** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlNWayJoinTests.cs`.
