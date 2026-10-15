# SELECT (Cohesion SQL)

Retrieves rows from one stored table, one system relation, or a two-table inner join.

> **Status:** Partial. The documented subset is measured against the live SQL engine.

## Syntax

```syntaxsql
SELECT [ DISTINCT ] <select_list>
FROM <table_source>
[ [ INNER ] JOIN <table_source> ON search_condition ]
[ WHERE search_condition ]
[ GROUP BY expression [ , ...n ] ]
[ HAVING search_condition ]
[ ORDER BY <order_by_item> [ , ...n ] ]
[ LIMIT nonnegative_integer ] [ OFFSET nonnegative_integer ]
[ ; ]

<select_list> ::= { * | expression [ [ AS ] column_alias ] [ , ...n ] }
<table_source> ::= [ schema_name . ] table_name [ [ AS ] table_alias ]
<order_by_item> ::= { expression | column_alias | select_list_ordinal } [ ASC | DESC ]
```

## Arguments

- **`select_list`** — Expressions produce output columns; unqualified `*` expands the input columns.
- **`table_source`** — A stored table or schema-qualified system relation in the session database.
- **`search_condition`** — A predicate; only `TRUE` retains a row, pair, or group.
- **`select_list_ordinal`** — A one-based output position after wildcard expansion.
- **`nonnegative_integer`** — The row count for `LIMIT` or skipped count for `OFFSET`.

## Supported clauses

| Form | Support | Boundary |
| --- | --- | --- |
| Projections, aliases, `DISTINCT`, `WHERE` | Supported (measured) | Executable scalar expressions only |
| `JOIN ... ON` | Supported (measured) | Exactly two stored tables; inner join only |
| `GROUP BY`, `HAVING`, aggregates | Supported (measured) | Grouping validity is checked before execution |
| `ORDER BY` | Supported (measured) | Source expressions, output aliases, and output ordinals |
| `LIMIT`, `OFFSET` | Supported (measured) | Nonnegative integer values |
| Uncorrelated subqueries | Supported (measured) | Scalar, membership, and existence forms |
| `TOP`, `ALL`, `FETCH`, `UNION`, `INTERSECT`, `EXCEPT` | Recognized, not supported (`COHDBL001`) | Excluded from the executable surface |
| `WITH`, windows, derived tables | Recognized, not supported (`COHDBL001`) | No common table expressions or derived relations |

## Remarks

The Phase 22 contract measures these subsets through the live engine, including wire execution.
Every `SELECT` requires `FROM`; a constant-only query without a relation fails planning.
Qualification identifies a schema within the current database, never another database.

`WHERE` filters input before grouping, and `HAVING` filters completed groups. Ordering precedes
pagination. `NULL` sorts first ascending and last descending. Equal ordering keys have no promised
relative order; supply a tie-breaking key for stable paging. See the individual clause pages for
alias precedence, grouping validity, collation, and subquery cardinality.

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

### C. Run a query from .NET

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

- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`
- **Implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Client/docs/OVERVIEW.md`
- **Implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Client/src/SqlCommand.cs`
- **Implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Client/src/SqlRow.cs`
- **TCP client transport** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Tcp/src/TcpConnectionFactory.cs`
