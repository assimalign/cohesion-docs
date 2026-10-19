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
    [ ON DELETE { CASCADE | RESTRICT | NO ACTION | SET NULL | SET DEFAULT } ]
```

These are table constraints inside `CREATE TABLE` or after `ALTER TABLE ... ADD`.
Column forms omit the local column list and use `REFERENCES` directly; see
[CREATE TABLE](statements/create-table.md#syntax) for the complete column grammar.
Column nullability is declared with `NULL` or `NOT NULL`.

## Arguments

- **`constraint_name`** — An optional name retained in catalog metadata.
- **`column_name`** — A local or referenced key column; key-list order is significant.
- **`search_condition`** — A deterministic Boolean expression over the current row.
- **`ON DELETE`** — The action on parent deletion; omission means `NO ACTION`.

## Remarks

| Constraint | Enforcement |
| --- | --- |
| `PRIMARY KEY` | Identifies the primary key and its enforcing index |
| `UNIQUE` | Lowers to a unique catalog index; a second null violates a single-column unique key |
| `FOREIGN KEY` / `REFERENCES` | Validates a matching referenced key; null foreign-key values are allowed |
| `CHECK` | Rejects false, accepts true or unknown |
| `NOT NULL` | Rejects an explicit null and an omitted value without a non-null default |

Explicit NULL cannot be combined with NOT NULL, PRIMARY KEY or [IDENTITY](clauses/identity.md),
in either order, including table primary keys and CREATE/ADD/ALTER column definitions. This
reports `SQL0003`: `Conflicting NULL/NOT NULL declarations for column 'id'.` for a column id.
An ALTER of an existing primary key to explicit NULL is likewise refused before publication.

Foreign and referenced lists must have the same number of columns. String foreign keys require
matching effective collations. Parent-key updates that would invalidate references are rejected;
`ON UPDATE` is recognized but unsupported with `COHDBL001`.
`ON DELETE CASCADE` does not imply `DROP TABLE ... CASCADE` support.

| Delete action | Behavior |
| --- | --- |
| CASCADE | Deletes referencing rows, following cascade chains |
| RESTRICT | Refuses deletion while references remain |
| NO ACTION | Checks references against the statement's final row set; this is the SQL default |
| SET NULL | Assigns NULL to every referencing column in surviving child rows |
| SET DEFAULT | Assigns each referencing column's current default, or NULL when none exists |

SET actions also run for children of cascaded deletions. Rows already in the deletion closure
are deleted once and do not receive SET assignments. Only referencing columns change. Child
NOT NULL, CHECK, UNIQUE, domain and foreign-key rules still apply; a default that points to a
missing or deleted parent fails. Parent and child mutations and index maintenance roll back
together on failure. Constraints are immediate, with no deferred enforcement. NULL in any
foreign-key component exempts that row from requiring a parent; SQL has no MISSING key value.

Constraint violations throw SqlConstraintViolationException; an unavailable SET DEFAULT on a NOT NULL column uses
`COHSQLE024` (23502). Default evaluation retains its normal diagnostic, and an ALWAYS identity
SET DEFAULT assignment uses `COHSQLE025`. See [Diagnostics](diagnostics.md).

Checks are deterministic, Boolean, and row-local. The supported scalar functions can participate;
parameters, aggregates, subqueries, and CAST are excluded. Null propagation matters: a check such as
`code > 0` accepts a null code as unknown. Use `NOT NULL` separately when null must be forbidden.
Alternatively, `CHECK (flag IS TRUE)` rejects a NULL flag because the
[truth test](expressions/operators.md#boolean-tests) returns FALSE rather than UNKNOWN.

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

Execute separately in a fresh database:

```sql
CREATE TABLE parent (id INT PRIMARY KEY);
INSERT INTO parent VALUES (0), (1);
CREATE TABLE nullable_child (pid INT REFERENCES parent(id) ON DELETE SET NULL);
CREATE TABLE default_child (pid INT DEFAULT 0 REFERENCES parent(id) ON DELETE SET DEFAULT);
INSERT INTO nullable_child VALUES (1);
INSERT INTO default_child VALUES (1);
DELETE FROM parent WHERE id = 1;
SELECT pid FROM nullable_child;
SELECT pid FROM default_child;
```

The queries return NULL and `0`, respectively. An omitted action or explicit NO ACTION instead
refuses the deletion while either child still references `1`; a self-referencing deletion may
remove both parent and child in the same statement.

## See also

[Language (SQL)](index.md) · [CREATE TABLE](statements/create-table.md) · [ALTER TABLE](statements/alter-table.md) · [System views](system-views/index.md)

## Sources

- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`
- **Referential action execution** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlReferentialActionTests.cs`
- **Constraint grammar** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlQueryParser.Constraints.cs`
- **Persistence** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Catalog/docs/DESIGN.md`
- **Collation compatibility** — `cohesion/docs/programs/COLLATION_DESIGN.md`

