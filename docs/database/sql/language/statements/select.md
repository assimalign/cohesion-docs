# SELECT (Cohesion SQL)

Retrieves rows from stored tables and system relations, including N-way joins and products.

> **Status:** Partial. The documented subset is measured against the live SQL engine.

## Syntax

```syntaxsql
[ WITH common_table_expression [ , ...n ] ]
SELECT [ DISTINCT ] [ TOP ( count_expression ) ] <select_list>
[ FROM <table_source> [ <join> ...n ] [ , <table_source> [ <join> ...n ] ...n ] ]
[ WHERE search_condition ]
[ GROUP BY expression [ , ...n ] ]
[ HAVING search_condition ]
[ ORDER BY <order_by_item> [ , ...n ] ]
[ LIMIT count_expression ] [ OFFSET count_expression [ ROW | ROWS ] ]
-- Or OFFSET then LIMIT; or FETCH FIRST/NEXT [count] ROW/ROWS ONLY with OFFSET in either order.
[ ; ]

<select_list> ::= <select_item> [ , ...n ]
<select_item> ::= * | expression [ [ AS ] column_alias ]
<table_source> ::= [ schema_name . ] table_name [ [ AS ] table_alias ]
                 | ( select_statement ) [ AS ] table_alias [ ( column_name [ , ...n ] ) ]
<join> ::= { [ INNER ] JOIN | LEFT [ OUTER ] JOIN } <table_source>
           { ON search_condition | USING ( column_name [ , ...n ] ) }
         | CROSS JOIN <table_source>
<order_by_item> ::= { expression | column_alias | select_list_ordinal } [ ASC | DESC ]
```

## Arguments

- **`select_list`** — Expressions produce output columns; unqualified `*` expands the input columns.
- **`table_source`** — A stored table, schema-qualified system relation or [aliased derived query](../clauses/from.md#derived-tables).
- **`search_condition`** — A predicate; only `TRUE` retains a row, pair, or group.
- **`select_list_ordinal`** — A one-based output position after wildcard expansion.
- **`nonnegative_integer`** — The row count for `LIMIT` or skipped count for `OFFSET`.

## Supported clauses

| Form | Support | Boundary |
| --- | --- | --- |
| Projections, aliases, `DISTINCT`, `WHERE` | Supported (measured) | Executable scalar expressions only |
| INNER/LEFT JOIN ... ON or USING | Supported (measured) | N-way stored/system joins; LEFT supplies NULL for unmatched right columns |
| CROSS JOIN, comma sources | Supported (measured) | Cartesian products; JOIN binds more tightly than comma |
| `GROUP BY`, `HAVING`, [aggregates](../functions/aggregate-functions.md) | Supported (measured) | DISTINCT/ALL and input ORDER BY; STRING_AGG and LISTAGG concatenate text |
| `ORDER BY` | Supported (measured) | Source expressions, output aliases, and output ordinals |
| [LIMIT, OFFSET, FETCH and TOP](../clauses/limit-offset.md) | Supported (measured) | One limit family; TOP is a lexical alias; nonnegative Int64 counts |
| Correlated and uncorrelated subqueries | Supported (measured) | Scalar, membership, and existence forms |
| `UNION`, `INTERSECT`, `EXCEPT` [ALL / DISTINCT] | Supported | [Set operations](../clauses/set-operations.md) in every query position; combined ORDER BY uses output names/ordinals |
| SELECT ALL | Recognized, not supported (`COHDBL001`) | ALL as a set quantifier executes |
| Derived tables | Supported (measured) | FROM, JOIN and comma inputs with required alias; outer ordering over derived outputs accepts per-key NULLS FIRST/LAST |
| [WITH](../clauses/with.md) | Supported (measured) | Ordered non-recursive SELECT definitions |
| Windows | Recognized, not supported (`COHDBL001`) | No window clauses |

## Remarks

[WITH](../clauses/with.md) binds non-recursive query definitions and materializes reachable
definitions once under the statement snapshot, before any write. Targets remain stored tables.

The conformance contract measures these subsets through the live engine, including wire execution.
A SELECT without FROM reads one implicit row, at the top level and in nested queries, views,
derived tables, CTEs and set operands. Scalar expressions and function calls therefore return one
row. WHERE FALSE or NULL, LIMIT 0 or OFFSET 1 can remove it. Aggregation, DISTINCT and ordering
follow ordinary semantics; COUNT(*) over the unfiltered implicit row is 1, and over WHERE FALSE
is 0. A wildcard without FROM fails planning with COHSQLE013.
Qualification identifies a schema within the current database, never another database.

`WHERE` filters input before grouping, and `HAVING` filters completed groups. Ordering precedes
pagination. `NULL` sorts first ascending and last descending. Equal ordering keys have no promised
relative order; supply a tie-breaking key for stable paging. See the individual clause pages for
alias precedence, grouping validity, collation, and subquery cardinality.

[USING](../clauses/join.md#using-columns) merges names and chooses a common type. Unqualified `*`
expands visible columns after merging; aliases and ordinals in ORDER BY count those columns.
Qualified `t.*` remains `COHDBL001`. The [JOIN examples](../clauses/join.md#examples) cover
INNER, LEFT, USING, CROSS and comma forms with results.

## Index access

Index ranges are read in bounded cancellable batches. Cancellation reaches cursor open, each
move, latch waits and page/entry reads; pages and the tree latch are released before expression
evaluation. This bounds index-range buffering; SELECT retains its result-materialization rules.

SELECT, [UPDATE](update.md) and [DELETE](delete.md) share rule-based index selection. Mandatory
AND terms can supply `=`, `<`, `<=`, `>` or `>=` comparisons between a direct indexed column and
a non-NULL, nonvolatile value evaluable during planning, such as a literal or bound parameter.
The column may be on either side; non-negated BETWEEN supplies inclusive bounds. Target aliases
and qualified references resolve normally. Conversion to the column's storage type must preserve
the comparison value.

An index can use equality on a leading key prefix, optionally followed by a range on the next key;
a range on its first key also qualifies. Selection prefers the longest equality prefix, then a usable
range, uniqueness and index name. Ranges support integer, Decimal, Boolean and temporal keys,
and string/character keys under the index's matching byte-expressible collation. Guid, binary and
JSON keys support equality only. Floating-point ranges scan, and floating signed-zero comparisons
do not supply a seek bound. A mismatched string collation also supplies no bound.

Computed key expressions, column-to-column comparisons, scalar-subquery comparands, NULL
comparands, IS NULL, NOT BETWEEN and a top-level OR do not supply bounds. Without another usable
AND term, execution scans. The full WHERE, including subqueries, still filters every candidate.
Only TRUE qualifies; NULL comparisons yield UNKNOWN and select no row. SQL has no MISSING value.

Execution falls back to a residual-filtered scan when the bound index was dropped or replaced,
including reuse of its name for different keys. [EXPLAIN](explain.md) shows the planned IndexSeek
and bounds or TableScan. Seek selection changes access, not predicate results.

For [JOIN](../clauses/join.md#predicate-safety), proven non-throwing single-input restrictions can
filter both inputs early and choose equality/range seeks. Compatible equijoin keys propagate exact
constants; a full unique seek can drive the first INNER pair even when written second. LEFT keeps
nullable-side WHERE predicates after NULL extension. Casts, arithmetic and calls that may throw
remain at their original evaluation level; Immutable or Stable metadata alone is insufficient.
Both constant input seeks and inner join probes fall back to scans if their planned index is
dropped or replaced. Complete ON/WHERE predicates preserve NULL and duplicate-row semantics.

## Examples

### A. Create the conformance fixture

Execute each statement separately. Later examples referring to `t` use this fixture.

```sql
CREATE TABLE t (id INT PRIMARY KEY, name TEXT, age INT);
INSERT INTO t VALUES (1, 'Ada', 36), (2, 'Grace', 45), (3, 'Alan', 41);
```

### B. Project and filter

```sql
SELECT id, age + 1 AS next_age FROM t ORDER BY id;
SELECT id FROM t WHERE age > 40 AND name LIKE 'G%' ORDER BY id;
```

The first query returns `(1, 37)`, `(2, 46)`, and `(3, 42)`; the second returns `2`.

### C. Select without FROM

Execute each query separately:

```sql
SELECT 1;
SELECT (SELECT 42) AS answer WHERE 1 IN (SELECT 1) AND EXISTS (SELECT NULL);
SELECT 1 WHERE NULL;
SELECT COUNT(*) WHERE FALSE;
SELECT answer FROM (SELECT 1 AS answer) d;
```

The results are one row containing 1, one row containing 42, no rows, one row containing 0,
and one row containing 1. EXISTS counts its projected NULL as a row. SELECT * without FROM
fails COHSQLE013. A non-Boolean WHERE or HAVING fails COHSQLE003 (42804); see
[search conditions](../clauses/where.md#boolean-search-conditions).

### D. Run a query from .NET

This complete client program assumes the fixture exists in database `audit` on the configured
Transmission Control Protocol (TCP) endpoint. Reference `Assimalign.Cohesion.Database.Sql.Client`
and `Assimalign.Cohesion.Connections.Tcp`.

```csharp
using System;

using Assimalign.Cohesion.Connections.Tcp;
using Assimalign.Cohesion.Database.Client;
using Assimalign.Cohesion.Database.Sql.Client;

await using var client = SqlClient.Create(new SqlClientOptions
{
    Settings = DatabaseConnectionSettings.Parse(
        "Database=audit;Endpoint=127.0.0.1:5439"),
    ConnectionFactory = new TcpConnectionFactory()
});
await using var connection = await client.ConnectAsync();
var command = new SqlCommand(
    "SELECT id FROM t WHERE age > @minimum ORDER BY id;")
    .WithParameter("minimum", 40);
SqlResultSet rows = await connection.QueryAsync(command);
foreach (SqlRow row in rows)
{
    Console.WriteLine(row.GetInt32(0));
}
```

The client materializes the complete result. One connection executes one command at a time.

## See also

[FROM](../clauses/from.md) · [JOIN](../clauses/join.md) · [WHERE](../clauses/where.md) · [ORDER BY](../clauses/order-by.md) · [Subqueries](../expressions/subqueries.md)

[Statements](index.md) · [Language (SQL)](../index.md)

## Sources

- **Implicit row and composition** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlConstantSelectTests.cs`.
- **Wire results** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Client/tests/SqlSubqueryWireTests.cs`.

- **Access selection** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Internal/Planning/SqlPlanner.cs`.
- **DML equivalence** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlDmlSeekTests.cs`.
- **Join restrictions and driving input** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlJoinExecutionTests.cs`.
- **Join error safety** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlJoinPredicateSafetyTests.cs`.

- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`
- **Implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Client/docs/OVERVIEW.md`
- **Implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Client/src/SqlCommand.cs`
- **Implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Client/src/SqlRow.cs`
- **TCP client transport** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Tcp/src/TcpConnectionFactory.cs`
