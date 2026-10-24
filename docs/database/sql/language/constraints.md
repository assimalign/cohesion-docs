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
reports `CS00003`: `Conflicting NULL/NOT NULL declarations for column 'id'.` for a column id.
An ALTER of an existing primary key to explicit NULL is likewise refused before publication.

Foreign and referenced lists must have the same number of columns. String foreign keys require
matching effective collations. Parent-key updates that would invalidate references are rejected;
`ON UPDATE` is recognized but unsupported with `CK00001`.
`ON DELETE CASCADE` does not imply `DROP TABLE ... CASCADE` support.

PRIMARY KEY, UNIQUE and FOREIGN KEY column lists, including referenced columns, must contain
distinct columns under case-insensitive binding. Repetitions report `CS01019`, naming the
column and constraint. A second PRIMARY KEY declaration also reports CS01019, naming the
constraint and table: inline, table-level, repeated clauses on one column, ADD CONSTRAINT and
ADD COLUMN forms all qualify. These are declaration errors before publication, distinct from
duplicate stored key values (CS03037). Refusing repeated FOREIGN KEY child columns is stricter
than PostgreSQL. CREATE TABLE IF NOT EXISTS on an existing table skips these declaration checks.

Self-referencing batch validation hashes candidate parent keys once per constraint. Regression
tests cover INSERT SELECT, ADD CONSTRAINT and ADD COLUMN REFERENCES at 50,000 rows. DDL streams
one current row at a time and uses indexed parent seeks where an index is available. A parent
key introduced by the same statement can require a scan instead; this bounds retained memory,
not execution time or the table's row count.

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
| `CS03037` | Duplicate PRIMARY KEY, UNIQUE constraint or unique-index key | 23505 |
| `CS05042` | Final referential-action row or write/build index key exceeds its encoded size limit | 54000 |
| `CS03038` | CHECK or domain check rejects a value, including CAST to a domain and function domain arguments/results | 23514 |
| `CS03039` | Missing referenced parent or removal/change of a referenced key forbidden by NO ACTION/RESTRICT | 23503 |
| `CS03024` | NOT NULL column receives NULL, including SET DEFAULT without a usable default | 23502 |
| `CS03050` | Constraint name collides with an existing constraint/index | 42710 |
| `CS03053` | Invalid table definition during execution; conflicting explicit NULL declarations still use CS00003 | 42P16 |
| `CS03054` | FOREIGN KEY column lists do not match or its referenced key is not unique | 42830 |
| `CS02055` | FOREIGN KEY types, CHECK function result or compared values are incompatible | 42804 |
| `CS03056` | Constraints/indexes depend on an object DDL would remove | 2BP01 |
| `CS05057` | Unsupported execution/DDL operation, including an index-backed constraint under invariant collation | 0A000 |
| `CS03059` | CHECK cannot be stored as a deterministic row definition | 42P17 |
| `CS02063` | FOREIGN KEY columns have incompatible collations | 42P21 |
| `CS05043` | Statement deadline expires during constraint/cascade execution; statement rolls back | 57014 |
| `CS04044` | A contended row, object or unique-key lock wait expires | 55P03 |
| `CS04006` | Target definition changed during lock preparation; retry in a fresh transaction | — |
| `CS01019` | Duplicate key-list column or multiple PRIMARY KEY declarations | — |
| `CS01015` | New CHECK uses a qualifier other than the constrained table/schema | — |

Default evaluation retains its normal diagnostic, and an ALWAYS identity SET DEFAULT assignment
uses `CS03025`. See [Diagnostics](diagnostics.md) for location and wire behavior.

Referential SET NULL/SET DEFAULT actions collect all assignments to each final child row, then
check the [row and index-key limits](diagnostics.md#row-and-index-key-limits) before apply.
CS05042 leaves both parent and child rows unchanged and an explicit transaction active.
An existing large logical row can still be updated to a fitting row or deleted; old values used
for locks and tombstones do not undergo new-write size validation.

ADD CONSTRAINT PRIMARY KEY/UNIQUE and ADD COLUMN with an index-backed constraint also enforce
the 1,024-byte encoded key limit. A failed build leaves the old schema, constraints and index
directory intact. See [CREATE INDEX](statements/create-index.md).

Adding a constraint validates existing live rows under Exclusive table locks without retaining
the whole table. Provisioning progress can add a count pass. PRIMARY KEY/UNIQUE backing builds
then check adjacent live keys in bounded batches over the completed index before publication;
deleted history is retained for old snapshots and excluded from uniqueness checks. NULL key
components still compare equal. An oversized key during construction (CS05042, 54000) can
precede duplicate detection (CS03037, 23505). Failure or cancellation before publication
leaves the ALTER unapplied and removes unpublished indexes. See the
[constraint retry example](statements/alter-table.md#retry-a-refused-constraint).

A non-Boolean CHECK or domain CHECK is rejected with CS02003 (42804), at the predicate
span when available, before publishing the definition. This includes non-Boolean operands
inside logical expressions and mixed CASE/COALESCE alternatives in Boolean positions; see
[search conditions](clauses/where.md#boolean-search-conditions). Untyped NULL is accepted as
UNKNOWN. CS03038 (23514) instead means a valid Boolean check evaluated to FALSE.

## Foreign-key preparation and concurrent DDL

DELETE cascade/restrict checks share incoming references and adjacent-table lock preparation once
per reached table within the statement. UPDATE and referential SET actions reuse the same statement
state. Key ordinals and effective collations are bound to each immutable table version; changing
an endpoint definition invalidates its projection. The next statement discovers a fresh graph,
including inside an explicit transaction. Snapshot metadata views retain their catalog snapshot;
integrity checks use live definitions protected by object locks and latest-state row checks.

After a contended object-lock wait, adjacent ADD COLUMN, child DROP and foreign-key removal rebuild
before row reads. A changed, dropped or replaced target instead reports retryable CS04006
(no assigned SQLSTATE); retry in a fresh transaction. A newly added incoming reference is checked
before mutation. Existing NULL/MATCH SIMPLE and final-row rules are unchanged.


## CHECK binding

New CHECKs accept bare columns, `table.column` and `schema.table.column` bound to the constrained
table, using case-insensitive identity. Another table or schema qualifier reports located
`CS01015` before CREATE TABLE, ADD CONSTRAINT or ADD COLUMN publishes anything.
Previously stored CHECKs retain their earlier qualifier binding on reopen. An already-applied
declared or imperative schema hash also retains it; a new or changed hash must pass the strict
qualifier validation before migration steps run.

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
`CREATE DOMAIN bad_domain AS INT CHECK (VALUE);` each fail with CS02003 (42804).
Neither definition is created.

A later `INSERT INTO c VALUES (0);` fails with CS03038 (23514); the two earlier rows remain.

```sql
CREATE TABLE qualified (a INT CHECK(qualified.a > 0 AND dbo.qualified.a < 100));
INSERT INTO qualified VALUES (1), (NULL);
ALTER TABLE qualified ADD CONSTRAINT bad_check CHECK(other.a > 0);
```

Execute separately: CREATE and INSERT succeed; ALTER reports CS01015 and publishes no
constraint. SELECT a FROM qualified returns 1 and NULL in unspecified order.

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

UPDATE reports CS03039 (23503) because final key 1 is absent. SELECT still returns
`(1, NULL)`, `(2, NULL)`. Both examples behave the same with explicit ON DELETE RESTRICT.

## See also

[Language (SQL)](index.md) · [CREATE TABLE](statements/create-table.md) · [ALTER TABLE](statements/alter-table.md) · [System views](system-views/index.md)

## Sources

- **Bounded DDL validation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlDdlMemoryTests.cs` and `SqlUniqueIndexBuildTests.cs` in the same directory.
- **Timeout and FK conformance** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlTimeoutConformanceTests.cs`, `SqlTimeoutRegressionTests.cs`, `SqlCascadeCatalogTests.cs`, `SqlForeignKeyBindingTests.cs` and `SqlReferenceLockTests.cs` in the same directory.

- **Conformance evidence** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlProgramLimitReviewTests.cs`.

- **Boolean conditions** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlBooleanPredicateTests.cs`.
- **Declaration validation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlInvalidStatementConformanceTests.cs`, `SqlQualifierConformanceTests.cs` and `SqlConstraintTimingTests.cs`.


- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`
- **Referential action execution** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlReferentialActionTests.cs`
- **Final UPDATE rows** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlUpdateForeignKeyTests.cs`.
- **Wire enforcement** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Client/tests/SqlUpdateForeignKeyWireTests.cs`.
- **Coded violations** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlConstraintDiagnosticTests.cs` and `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Client/tests/SqlConstraintDiagnosticWireTests.cs`.
- **Constraint grammar** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlQueryParser.Constraints.cs`
- **Persistence** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/docs/DESIGN.md`
- **Collation compatibility** — `cohesion/docs/programs/COLLATION_DESIGN.md`

