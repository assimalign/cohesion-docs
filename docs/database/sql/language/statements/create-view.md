# CREATE VIEW (Cohesion SQL)

Persists a read-only query with frozen output columns in the session database.

> **Status:** Implemented for the documented subset.

## Syntax

```syntaxsql
CREATE [ OR REPLACE ] VIEW [ schema_name . ] view_name
[ ( column_name [ , ...n ] ) ] AS query_body [ ; ]
```

## Remarks

The body is a SELECT or [set operation](../clauses/set-operations.md), optionally with its own
non-recursive WITH, joins, subqueries, grouping, ordering and paging. It follows the ordinary
SELECT subset. Parameters are refused. An explicit column list must name every output exactly;
output names must be unique.

The catalog stores canonical SQL with bound input qualifiers and expanded stars at every query
level. Later ADD COLUMN does not change the view's output. Rebinding refuses a changed input
column identity with COHSQLE033. Stored function identities keep later overloads from changing
a call. Unqualified view names use dbo; tables and views share a relation namespace.

OR REPLACE must retain every existing column's name, position, type, length, precision, scale,
domain and collation. It may append columns. Incompatible replacement and dependency cycles
fail atomically. The complete encoded definition, columns and dependencies must fit 8,092 bytes.

Views work in FROM, JOIN, subqueries and INSERT SELECT. Each reference materializes its body
once under the caller's statement snapshot, using memory proportional to the body rows.
Outer filtering, grouping, ordering and paging execute over those rows. View queries have
ordinary SQL NULL behavior and no MISSING value. INSERT, UPDATE, DELETE, ALTER TABLE,
CREATE INDEX and DROP TABLE targeting a view fail with COHSQLE021 (25006).

[INFORMATION_SCHEMA](../system-views/information-schema.md) lists views in TABLES, COLUMNS and
VIEWS. Column names and type facets are frozen; every catalog view column reports
IS_NULLABLE = YES. CASE, COALESCE and set outputs retain facets only when all result alternatives
agree in type, domain and modifiers; NULLIF retains its first argument's facets. USING merges
with mismatched facets produce the unconstrained common type.

A recoverable body bind failure affects reads of that view; it does not block database open,
unrelated reads, metadata enumeration or DROP VIEW. The read retains the underlying diagnostic
when available and otherwise uses COHSQLE033. Cancellation, insufficient stack, out-of-memory,
storage and I/O failures propagate, including through nested exceptions. Read-time stack
exhaustion reports COHSQLE004 (54001); malformed persisted canonical text can still fail open.

DDL requires auto-commit. Session-created views have Adhoc ownership; declared scripts publish
Schema-owned definitions and V: receipts atomically. Format 23 introduced view records of kind 17;
current SQL storage format 24 also persists explicit NULL placement. Older formats are refused.

## Diagnostics

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| COHSQLE033 | Invalid view declaration, duplicate outputs, parameters, relation collision, incompatible replacement, dependency cycle/change, oversized record or changed bound column identity | — |
| COHSQLE013 | Explicit output column-list width differs from the body | — |
| COHSQLE028 | Target namespace does not exist | 3F000 |
| COHDBL001 | MATERIALIZED, RECURSIVE, TEMP/TEMPORARY, IF NOT EXISTS, WITH options or WITH [LOCAL\|CASCADED] CHECK OPTION | — |
| COHSQLT003 | View DDL inside an explicit transaction | — |

Ordinary query binding and function diagnostics also apply. Dependency protection uses
COHSQLD002: DROP TABLE/FUNCTION/DOMAIN, table rename, and drop/rename or type/length/precision/scale
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
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Internal/SqlPlanner.Views.cs`.
