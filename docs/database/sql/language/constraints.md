# Constraints (Cohesion SQL)

Declares primary keys, unique keys, foreign keys, checks, and column nullability.

> **Status:** Partial. The listed declarations execute; deferred enforcement and ON UPDATE actions are excluded.

## Syntax

```syntaxsql
[ CONSTRAINT constraint_name ] PRIMARY KEY ( column_name [ , ...n ] )
[ CONSTRAINT constraint_name ] UNIQUE ( column_name [ , ...n ] )
[ CONSTRAINT constraint_name ] CHECK ( search_condition )
[ CONSTRAINT constraint_name ] FOREIGN KEY ( column_name [ , ...n ] )
    REFERENCES [ schema_name . ] table_name ( column_name [ , ...n ] )
    [ ON DELETE { CASCADE | RESTRICT } ]
```

These are table constraints inside `CREATE TABLE` or after `ALTER TABLE ... ADD`.
Column forms omit the local column list and use `REFERENCES` directly; see
[CREATE TABLE](statements/create-table.md#syntax) for the complete column grammar.
Column nullability is declared with `NULL` or `NOT NULL`.

## Arguments

- **`constraint_name`** — An optional name retained in catalog metadata.
- **`column_name`** — A local or referenced key column; key-list order is significant.
- **`search_condition`** — A deterministic Boolean expression over the current row.
- **`CASCADE` or `RESTRICT`** — The action on parent deletion; omission means `RESTRICT`.

## Remarks

| Constraint | Enforcement |
| --- | --- |
| `PRIMARY KEY` | Identifies the primary key and its enforcing index |
| `UNIQUE` | Lowers to a unique catalog index; a second null violates a single-column unique key |
| `FOREIGN KEY` / `REFERENCES` | Validates a matching referenced key; null foreign-key values are allowed |
| `CHECK` | Rejects false, accepts true or unknown |
| `NOT NULL` | Rejects an explicit null and an omitted value without a non-null default |

Foreign and referenced lists must have the same number of columns. String foreign keys require
matching effective collations. Parent-key updates that would invalidate references are rejected;
`ON UPDATE` is recognized but unsupported with `COHDBL001`.
`ON DELETE CASCADE` does not imply `DROP TABLE ... CASCADE` support.

Checks are deterministic, Boolean, and row-local. The supported scalar functions can participate;
parameters, aggregates, subqueries, and CAST are excluded. Null propagation matters: a check such as
`code > 0` accepts a null code as unknown. Use `NOT NULL` separately when null must be forbidden.

The catalog persists normalized declarations and exact check-expression text. Unique declarations
share the same implementation as `CREATE UNIQUE INDEX`. Index-backed constraints require an
index-backed collation and reject compatibility `invariant`, even on empty tables.
System-view deferral flags are always `NO`.

## Examples

This conformance example accepts one positive and one null value:

```sql
CREATE TABLE c (code INT CHECK(code > 0));
INSERT INTO c VALUES (1), (NULL);
```

A later `INSERT INTO c VALUES (0);` fails the check.

Starting from a separate database with the
[conformance fixture](statements/select.md#a-create-the-conformance-fixture):

```sql
CREATE TABLE c (pid INT REFERENCES t(id) ON DELETE CASCADE);
INSERT INTO c VALUES (1), (NULL);
DELETE FROM t WHERE id = 1;
SELECT pid FROM c;
```

Only the null child remains.

## See also

[Language (SQL)](index.md) · [CREATE TABLE](statements/create-table.md) · [ALTER TABLE](statements/alter-table.md) · [System views](system-views/index.md)

## Sources

- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`
- **Constraint grammar** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlQueryParser.Constraints.cs`
- **Persistence** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Catalog/docs/DESIGN.md`
- **Collation compatibility** — `cohesion/docs/programs/COLLATION_DESIGN.md`

