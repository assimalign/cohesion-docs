# CREATE VIEW (Cohesion SQL)

Persists a read-only query with frozen output columns in the session database.

> **Status:** Implemented. Supports the documented subset.

## Syntax

```syntaxsql
CREATE [ OR REPLACE ] VIEW [ schema_name . ] view_name
[ ( column_name [ , ...n ] ) ] AS query_body [ ; ]
```

## Remarks

The body is a SELECT or [set operation](../clauses/set-operations.md), optionally with its own
WITH, joins, subqueries, grouping, ordering and paging. A recursive query body can bind and freeze
anchor metadata at creation; querying that view executes on demand, including after reopen.
It follows the ordinary SELECT subset. Parameters are refused. An explicit column list must name every output exactly;
output names must be unique.

The catalog stores canonical SQL with bound input qualifiers and expanded stars at every query
level. Later ADD COLUMN does not change the view's output. Rebinding refuses a changed input
column identity with CS03033. Stored function identities keep later overloads from changing
a call. Unqualified view names use dbo; tables and views share a relation namespace.

OR REPLACE must retain every existing column's name, position, type, length, precision, scale,
domain and collation. It may append columns. Incompatible replacement and dependency cycles
fail atomically. The complete encoded definition, columns and dependencies must fit 8,092 bytes.

Views work in FROM, JOIN, subqueries, INSERT SELECT and
[CREATE TABLE AS](create-table.md#create-table-as). Each nonrecursive reference materializes its body
once under the caller's statement snapshot, using memory proportional to the body rows.
Recursive bodies use [shared demand-driven readers](../clauses/with.md#on-demand-evaluation-and-memory),
so an outer LIMIT can stop an infinite producer.
Outer filtering, grouping, ordering and paging execute over those rows. View queries have
ordinary SQL NULL behavior and no MISSING value. INSERT, UPDATE, DELETE, ALTER TABLE,
CREATE INDEX and DROP TABLE targeting a view fail with CS04021 (25006).

[INFORMATION_SCHEMA](../system-views/information-schema.md) lists views in TABLES, COLUMNS and
VIEWS. Column names and type facets are frozen; every catalog view column reports
IS_NULLABLE = YES. CASE, COALESCE and set outputs retain facets only when all result alternatives
agree in type, domain and modifiers; NULLIF retains its first argument's facets. USING merges
with mismatched facets produce the unconstrained common type.

A recoverable body bind failure affects reads of that view; it does not block database open,
unrelated reads, metadata enumeration or DROP VIEW. The read retains the underlying diagnostic
when available and otherwise uses CS03033. Cancellation, insufficient stack, out-of-memory,
storage and I/O failures propagate, including through nested exceptions. Read-time stack
exhaustion reports CS05004 (54001); malformed persisted canonical text can still fail open.

DDL requires auto-commit. Session-created views have Adhoc ownership; declared scripts publish
Schema-owned definitions and V: receipts atomically. Format 23 introduced view records of kind 17;
format 24 introduced explicit NULL placement. Current format 31 also admits distinct predicates,
Boolean tests and LIKE ESCAPE in stored bodies. Older formats are refused.

## Delimited identifiers

View, column and alias names accept doubled quotes, as in `"a""b"` for `a"b`.
Canonical rendering escapes embedded quotes and binding replaces the complete quoted source token.
The catalog encoding and storage format are unchanged.

An earlier build could persist adjacent quoted tokens without a space: `"a""x"` meant column
`a` with alias `x`. It now means the single identifier `a"x`. An affected stored view can bind
differently or fail after reopen. Recreate it using `"a" "x"` or `"a" AS "x"` to retain the
column/alias interpretation; there is no stored-text migration.

For example, after the quoted table example on [Literals and identifiers](../expressions/literals.md):

```sql
CREATE VIEW "v""x" AS SELECT "a""b" AS "c""d" FROM "t""x";
SELECT "c""d" FROM "v""x";
```

The query returns 7 under the name `c"d`.

## Diagnostics

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CS03033 | Invalid view declaration, duplicate outputs, parameters, relation collision, incompatible replacement, dependency cycle/change, oversized record or changed bound column identity | — |
| CS05013 | Explicit output column-list width differs from the body | — |
| CS01028 | Target namespace does not exist | 3F000 |
| CK00001 | MATERIALIZED, RECURSIVE, TEMP/TEMPORARY, IF NOT EXISTS, WITH options or WITH [LOCAL\|CASCADED] CHECK OPTION | — |
| CS04003 | View DDL inside an explicit transaction | — |

Ordinary query binding and function diagnostics also apply. Dependency protection uses
CS03302: DROP TABLE/FUNCTION/DOMAIN, table rename, and drop/rename or type/length/precision/scale
changes to a referenced column are refused. Default and nullability changes retain their usual rules.

## Example

Execute each statement separately:

```sql
CREATE TABLE source_rows (id INT NOT NULL, name VARCHAR(10));
INSERT INTO source_rows VALUES (2, 'Bea'), (1, 'Ada');
CREATE VIEW names AS SELECT * FROM source_rows;
ALTER TABLE source_rows ADD COLUMN extra INT;
SELECT * FROM names ORDER BY id;
CREATE OR REPLACE VIEW names AS SELECT id, name, extra FROM source_rows;
SELECT id, name, extra FROM names ORDER BY id;
```

The first query returns (1, 'Ada'), (2, 'Bea') with two columns. The second returns
(1, 'Ada', NULL), (2, 'Bea', NULL) with the appended column.

## See also

[DROP VIEW](drop-view.md) · [FROM](../clauses/from.md) · [ALTER TABLE](alter-table.md) · [Statements](index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlViewTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlViewTests.TypeFacets.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlViewTests.BindingFailures.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Internal/Planning/SqlPlanner.Views.cs`.
