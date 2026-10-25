# INSERT (Cohesion SQL)

Adds rows from scalar values or a query to a stored table.

> **Status:** Partial. The documented subset is measured against the live SQL engine.

## Syntax

```syntaxsql
[ WITH common_table_expression [ , ...n ] ]
INSERT INTO [ schema_name . ] table_name [ [ AS ] table_alias ]
[ ( column_name [ , ...n ] ) ]
{ VALUES ( { expression | DEFAULT } [ , ...n ] ) [ , ...n ]
| DEFAULT VALUES | select_statement }
[ ON CONFLICT [ ( column_name [ , ...n ] ) | ON CONSTRAINT constraint_name ]
    { DO NOTHING | DO UPDATE SET column_name = { expression | DEFAULT } [ , ...n ]
        [ WHERE search_condition ] } ]
[ RETURNING { * | expression [ [ AS ] column_alias ] } [ , ...n ] ]
[ ; ]
```

## Arguments

The query source may be a [set-operation body](../clauses/set-operations.md), including a
parenthesized body. All source rows stage before atomic destination writes; common-type and
width validation happens before reads/writes.

- **`table_name`** — The destination stored table in the current database.
- **`column_name`** — Destination columns in the order of the supplied values or query outputs.
- **`expression`** — A literal, parameter, or executable scalar expression without a subquery.
- **`select_statement`** — A query within the supported [SELECT](select.md) subset.

## Remarks

[WITH](../clauses/with.md) binds non-recursive query definitions and materializes reachable
definitions once under the statement snapshot, before any write. Targets remain stored tables.

A whole-item [DEFAULT](../clauses/default.md) requests the destination default;
`DEFAULT VALUES` inserts one row of defaults. A missing default on a NOT NULL column is
`CS03024` (SQLSTATE 23502). Without a column list, values map to all destination columns. Omitted columns use their declared
defaults or nullable `NULL`; an explicit `NULL` does not request the default. Destination coercion,
nullability, checks, primary and unique keys, and foreign keys apply to written rows.
[ON CONFLICT](../clauses/on-conflict.md) skips or updates unique-key conflicts for every source
form. Proposed NOT NULL/domain/CHECK validation still precedes arbitration; foreign keys validate
only rows actually written. DO UPDATE requires an arbiter and exposes target plus EXCLUDED values.
Each DO UPDATE SET target may appear only once, including assignments using DEFAULT;
case-insensitive duplicates report `CS01019` before writes. See [SET](../clauses/set.md).
RETURNING includes inserted/updated rows and their combined count, excluding skips. See the clause
for CS03035 (42P10), CS05036 (21000), isolation and transaction retry rules.

For an [identity column](../clauses/identity.md), omission or DEFAULT allocates one value per row,
including DEFAULT VALUES and omitted INSERT SELECT destinations. ALWAYS rejects explicit values
(`CS03025`); BY DEFAULT accepts non-null explicit values without advancing the counter.
Explicit NULL never requests generation. Failed statements can consume values without inserting rows.
[RETURNING](../clauses/returning.md) retrieves allocated identities and stored values.
OVERRIDING remains unsupported (`CK00001`).

`INSERT ... SELECT` validates output width and declared type compatibility even on empty input.
It materializes the entire source under the statement snapshot before writing. Reading from the
destination is therefore supported without feeding newly inserted rows back into the source.
Any failure leaves none of the query-source statement's rows inserted.

Constraint failures carry one Error diagnostic: `CS03037` (23505) for a duplicate primary,
unique or unique-index key, `CS03038` (23514) for a CHECK/domain check, `CS03039` (23503)
for a missing foreign-key parent, or `CS03024` (23502) for NOT NULL. Messages name the
constraint and table without key values. The statement rolls back and an explicit transaction
remains usable. See [Diagnostics](../diagnostics.md) for wire delivery and source spans.

Subqueries inside VALUES or RETURNING report CK00001.
Use `INSERT ... SELECT` when input comes from a query. The source may also start with WITH:
`INSERT INTO target WITH c AS (SELECT ...) SELECT ... FROM c`.

## Size limits

Rows and every write index key must fit the [encoded size limits](../diagnostics.md#row-and-index-key-limits).
An oversized value reports `CS05042` / SQLSTATE 54000 before any row in the statement is
inserted. Earlier statements in an explicit transaction remain intact and the transaction stays active.

Execute separately in an empty database, binding `value` to `new string('x', 9000)`:

```sql
CREATE TABLE large_rows (id INT PRIMARY KEY, value TEXT);
INSERT INTO large_rows VALUES (2, 'ok'), (3, @value);
SELECT id FROM large_rows;
```

INSERT fails with CS05042; SELECT returns no rows. This follows SqlProgramLimitConformanceTests.
ON CONFLICT checks only proposals that become writes; see [discarded proposals](../clauses/on-conflict.md#size-limits).

## Examples

Using the [conformance fixture](select.md#a-create-the-conformance-fixture), this inserts two rows:

```sql
INSERT INTO t VALUES (4, 'Barbara', 33), (5, 'Edsger', 42);
```

This minimally adapted query-source example copies the fixture's older people into a separate table:

```sql
CREATE TABLE selected_people (id INT, name TEXT);
INSERT INTO selected_people SELECT id, name FROM t WHERE age > 40;
```

Execute each statement separately; the insert reports the number of inserted rows.

## See also

[VALUES](../clauses/values.md) · [ON CONFLICT](../clauses/on-conflict.md) · [SELECT](select.md) · [Constraints](../constraints.md)

[Statements](index.md) · [Language (SQL)](../index.md)

## Sources

- **Conformance evidence** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlProgramLimitConformanceTests.cs`.
- **Conformance evidence** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlProgramLimitReviewTests.cs`.

- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`
- **Implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlSubqueryExecutionTests.cs`
- **Constraint diagnostics** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Client/tests/SqlConstraintDiagnosticWireTests.cs`.
