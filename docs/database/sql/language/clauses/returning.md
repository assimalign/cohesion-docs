# RETURNING (Cohesion SQL)

Returns a projection for each directly inserted, updated or deleted row.

> **Status:** Implemented. Supports INSERT, UPDATE and DELETE.

## Syntax

```syntaxsql
{ insert_statement | update_statement | delete_statement }
RETURNING { * | expression [ [ AS ] column_alias ] } [ , ...n ] [ ; ]
```

## Remarks

The final list uses SELECT projection rules: star expands in table-column order, pass-through
columns keep their names, and computed outputs without aliases use columnN names. Expressions
can use target columns, parameters and scalar calls. The target name or alias qualifies columns;
an alias hides the original name. INSERT SELECT exposes only the destination row to RETURNING.

INSERT returns stored values after coercion, generated identity allocation and per-row defaults.
VALUES and INSERT SELECT preserve source order. UPDATE returns new values, while SET expressions
read the original row. DELETE returns old values for directly targeted rows; cascade deletions and
SET NULL/DEFAULT child updates do not contribute rows. UPDATE/DELETE output order is unspecified.
NULL follows the selected expressions' ordinary rules; SQL has no MISSING value.

[ON CONFLICT](on-conflict.md) returns inserted and post-update rows in proposal source order.
DO NOTHING and FALSE/UNKNOWN update filters return no row and contribute no affected count.
The EXCLUDED pseudo-relation is outside RETURNING scope (CK00001); a real target named or
aliased excluded can still qualify its own returned columns.

Projection runs per row inside the write after that row's mutation and index checks. Even immutable
calls execute there. Any constraint or projection error rolls the entire statement back, including
inside BEGIN; earlier successful statements remain. Results buffer until apply completes and
auto-commit succeeds. A slow application function in RETURNING holds up other database writers.
Materialized results remain readable after a later ROLLBACK.

Over the wire, every header and row is encoded and size-checked inside the statement's undo
bracket before commit or result delivery. Encoding, enumeration or disposal failure rolls back
only this statement; earlier transaction work and the connection remain usable. Each header/row
must fit 16 MiB, with no aggregate result cap. Writes retain these prepared payloads until delivery.
See [result delivery](../diagnostics.md#result-encoding-and-delivery).

In process, DatabaseResultSet.AffectedCount is the direct write count. Over the wire,
[SqlResultSet.AffectedCount](../../../../dotnet-apis/resources/database/assimalign-cohesion-database-sql-client/sql-result-set.md)
provides that count alongside rows. SELECT uses -1. A zero-row write returns typed columns,
no rows and count zero. QueryAsync returns rows and count; ExecuteAsync transfers and buffers
the same rows, discards them and returns the count. ExecuteScalarAsync reads the first cell.

[EXPLAIN](../statements/explain.md) includes the expanded projection count and names without
executing the write or projection.

## Diagnostics

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CK00001 | OUTPUT, RETURNING INTO, OLD/NEW pseudo-relations, RETURNING WITH (...), subqueries or windows | — |
| CS05017 | Aggregate in RETURNING, rejected during planning even for empty input | — |
| CS01015 | Unknown/hidden target qualifier or an INSERT source reference | — |
| CS00003 | Empty RETURNING list or SELECT ... RETURNING | — |
| CS02001 | Projection divides or takes a remainder by zero; rolls back the statement | 22012 |
| CS05045 | Uncoded result preparation failure or encoded header/row above 16 MiB; rolls back the statement | 54000 for size refusals |

A target actually named or aliased old/new masks that pseudo-relation name. Ordinary output/old/new
identifiers retain their quoting rules. UPDATE accepts bare output as a target alias; DELETE needs
AS output because its bare spelling is reserved for OUTPUT detection. Other expression and write
diagnostics retain their codes.

## Examples

```sql
CREATE TABLE t (id INT GENERATED ALWAYS AS IDENTITY, name TEXT);
INSERT INTO t (name) VALUES ('Ada'), ('Bea') RETURNING id, name;
UPDATE t AS x SET name = UPPER(name) WHERE id = 1 RETURNING x.id, name;
DELETE FROM t WHERE id = 2 RETURNING *;
```

Results are (1, 'Ada'), (2, 'Bea') with count 2; (1, 'ADA') with count 1; and
(2, 'Bea') with count 1.

```sql
CREATE TABLE rollback_rows (id INT, divisor INT);
INSERT INTO rollback_rows VALUES (1, 1), (2, 0) RETURNING 10 / divisor;
SELECT COUNT(*) FROM rollback_rows;
```

INSERT reports CS02001 and SELECT returns 0: the earlier row was rolled back too.

## See also

[INSERT](../statements/insert.md) · [UPDATE](../statements/update.md) · [DELETE](../statements/delete.md) · [Clauses](index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlReturningExecutionTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Client/tests/SqlReturningWireTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlQueryParser.Returning.cs`.
