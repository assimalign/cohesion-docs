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

Multi-row UPDATE validates outgoing and incoming NO ACTION/RESTRICT references against the
statement's final rows. Old replaced versions are excluded and every final replacement contributes
its keys. A self-reference can move with its parent key, and mutually referenced keys can swap
when the final state is consistent. A removed key cannot authorize a dangling child; another
replacement retaining that key can satisfy the reference. Snapshot and ReadCommitted use the
same pending-row rules. A failure rolls back the statement and leaves an explicit transaction
usable. [ON CONFLICT DO UPDATE](clauses/on-conflict.md) shares this final-reference view.

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

Constraint violations throw [SqlConstraintViolationException](../../../dotnet-apis/resources/database/assimalign-cohesion-database-sql/sql-constraint-violation-exception.md)
with exactly one Error diagnostic. Messages name the constraint and table without key values;
domain CAST and function checks name the domain without a table. Available statement spans reach
clients with negotiated diagnostics. The rejected statement rolls back; the session and an explicit
transaction remain usable.

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| `COHSQLE037` | Duplicate PRIMARY KEY, UNIQUE constraint or unique-index key | 23505 |
| `COHSQLE038` | CHECK or domain check rejects a value, including CAST to a domain and function domain arguments/results | 23514 |
| `COHSQLE039` | Missing referenced parent or removal/change of a referenced key forbidden by NO ACTION/RESTRICT | 23503 |
| `COHSQLE024` | NOT NULL column receives NULL, including SET DEFAULT without a usable default | 23502 |

Default evaluation retains its normal diagnostic, and an ALWAYS identity SET DEFAULT assignment
uses `COHSQLE025`. See [Diagnostics](diagnostics.md) for location and wire behavior.

A non-Boolean CHECK or domain CHECK is rejected with COHSQLE003 (42804), at the predicate
span when available, before publishing the definition. This includes non-Boolean operands
inside logical expressions and mixed CASE/COALESCE alternatives in Boolean positions; see
[search conditions](clauses/where.md#boolean-search-conditions). Untyped NULL is accepted as
UNKNOWN. COHSQLE038 (23514) instead means a valid Boolean check evaluated to FALSE.

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

`CREATE TABLE bad_check (a INT CHECK (a));` and
`CREATE DOMAIN bad_domain AS INT CHECK (VALUE);` each fail with COHSQLE003 (42804).
Neither definition is created.

A later `INSERT INTO c VALUES (0);` fails with COHSQLE038 (23514); the two earlier rows remain.

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

## UPDATE final-row examples

Execute separately in a fresh database:

```sql
CREATE TABLE self_rows (id INT PRIMARY KEY, parent INT REFERENCES self_rows(id));
INSERT INTO self_rows VALUES (1, 1);
UPDATE self_rows SET id = 10, parent = 10;
SELECT id, parent FROM self_rows;
```

UPDATE affects one row; SELECT returns `(10, 10)`. With initial rows `(1, 2)`, `(2, 1)`,
`UPDATE self_rows SET id = 3 - id, parent = 3 - parent` affects two rows and preserves the
final set `(1, 2)`, `(2, 1)`, including unique-key enforcement.

```sql
CREATE TABLE moved_rows (id INT PRIMARY KEY, parent INT REFERENCES moved_rows(id));
INSERT INTO moved_rows VALUES (1, NULL), (2, NULL);
UPDATE moved_rows SET id = CASE WHEN id = 1 THEN 10 ELSE id END,
    parent = CASE WHEN id = 2 THEN 1 ELSE parent END;
SELECT id, parent FROM moved_rows ORDER BY id;
```

UPDATE reports COHSQLE039 (23503) because final key 1 is absent. SELECT still returns
`(1, NULL)`, `(2, NULL)`. Both examples behave the same with explicit ON DELETE RESTRICT.

## See also

[Language (SQL)](index.md) · [CREATE TABLE](statements/create-table.md) · [ALTER TABLE](statements/alter-table.md) · [System views](system-views/index.md)

## Sources

- **Boolean conditions** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlBooleanPredicateTests.cs`.


- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`
- **Referential action execution** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlReferentialActionTests.cs`
- **Final UPDATE rows** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlUpdateForeignKeyTests.cs`.
- **Wire enforcement** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Client/tests/SqlUpdateForeignKeyWireTests.cs`.
- **Coded violations** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlConstraintDiagnosticTests.cs` and `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Client/tests/SqlConstraintDiagnosticWireTests.cs`.
- **Constraint grammar** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlQueryParser.Constraints.cs`
- **Persistence** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/docs/DESIGN.md`
- **Collation compatibility** — `cohesion/docs/programs/COLLATION_DESIGN.md`

