# RETURNING (Cohesion SQL)

Returns a projection for each directly inserted, updated or deleted row.

> **Status:** Execution for INSERT, UPDATE, DELETE and MERGE.

## Syntax

```syntaxsql
{ insert_statement | update_statement | delete_statement | merge_statement }
RETURNING { * | expression [ [ AS ] column_alias ] } [ , ...n ] [ ; ]
```

## Remarks

[MERGE](../statements/merge.md) RETURNING binds target/source columns and the contextual
[merge_action()](../functions/merge-action.md) TEXT expression, yielding INSERT, UPDATE or DELETE.
It emits one row per direct action: new target values for INSERT/UPDATE, old values for DELETE,
and NULL source values for BY SOURCE. These are direct values before referential effects; an
updated/inserted row later removed by a cascade still returns and counts. DO NOTHING and cascades
emit no rows of their own. Output order is unspecified. The atomic execution/result-delivery
rules below also apply to MERGE.

The final list uses SELECT projection rules: star expands in table-column order, pass-through
columns keep their names, and computed outputs without aliases use columnN names. Expressions
can use target columns, parameters and scalar calls, plus source columns for joined UPDATE/DELETE. The target name or alias qualifies columns;
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

Ordinary projection runs per row inside the write after that row's mutation and index checks.
A list containing [sequence function calls](../functions/sequences.md) evaluates before the global
apply gate so durable allocation/setval commits occur outside it. NEXT VALUE FOR is refused in
every RETURNING list with CS03076 (0A000), before allocation or row mutation. Sequence effects
remain nontransactional if the statement later fails. Any constraint or projection error rolls the entire statement back, including
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
| CS03076 | NEXT VALUE FOR in RETURNING | 0A000 |
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

## Joined mutations

[UPDATE ... FROM](../statements/update.md#from-sources) and
[DELETE ... USING](../statements/delete.md#using-sources) execute with source columns in RETURNING.
Bare RETURNING * expands target columns first, then visible sources in written FROM/USING order.
JOIN USING contributes merged keys, then remaining left/right columns; hidden duplicates stay
hidden. Duplicate output names are allowed; use ordinal access to distinguish them (client name
lookup returns the first). Type modifiers, collation and LEFT nullability are preserved.

UPDATE returns new target values and collected source values. More than one source match for a
target fails CS05068 (21000) before SET evaluation or writes. DELETE deletes each physical target
once and returns its old values with one unspecified source association. Output order is
unspecified; cascade/child-action rows remain excluded. Constraints and projection errors roll
back the entire statement. EXPLAIN describes the association and expanded output metadata.

With t(id,v) containing (1,0) and s(id,v) containing (1,7):

```sql
UPDATE t SET v = s.v FROM s WHERE t.id = s.id RETURNING *;
```

The result is (1,7,1,7), columns id, v, id, v and affected count 1.

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlReturningExecutionTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Client/tests/SqlReturningWireTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlQueryParser.Returning.cs`.
