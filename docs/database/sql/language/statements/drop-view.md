# DROP VIEW (Cohesion SQL)

Removes a persisted view, optionally removing views that depend on it.

> **Status:** Implemented.

## Syntax

```syntaxsql
DROP VIEW [ IF EXISTS ] [ schema_name . ] view_name [ RESTRICT | CASCADE ] [ ; ]
```

## Remarks

RESTRICT is the default. It refuses a view with dependent views. CASCADE removes those
dependent views in the same atomic catalog publication. It does not delete the underlying
tables or rows. IF EXISTS accepts an absent view, but still refuses a stored table.

DROP does not bind the body, so a view whose body no longer binds can be removed.
DDL requires auto-commit; ownership checks still apply. [DROP SCHEMA](drop-schema.md)
RESTRICT counts views; CASCADE removes contained views and external views depending on
its tables or views.

## Diagnostics

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| COHSQLD002 | RESTRICT finds a dependent view; message names it | 2BP01 |
| COHSQLE033 | Target is absent without IF EXISTS, or is a table even with IF EXISTS | — |
| COHSQLT003 | DDL inside an explicit transaction | — |

## Example

```sql
CREATE TABLE t (id INT);
INSERT INTO t VALUES (7);
CREATE VIEW v AS SELECT id FROM t;
CREATE VIEW w AS SELECT id FROM v;
DROP VIEW v CASCADE;
SELECT id FROM t;
DROP VIEW IF EXISTS w;
```

SELECT still returns 7. Both views are absent; the final DROP succeeds without a change.
Replacing CASCADE with RESTRICT reports COHSQLD002 and preserves both views.

## See also

[CREATE VIEW](create-view.md) · [DROP SCHEMA](drop-schema.md) · [Statements](index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlViewTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Catalog/SqlCatalog.Views.cs`.
