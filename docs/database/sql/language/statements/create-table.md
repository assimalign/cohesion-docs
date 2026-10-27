# CREATE TABLE (Cohesion SQL)

Creates a stored table with typed columns and supported constraints.

> **Status:** Partial. The documented subset is measured against the live SQL engine.

## Syntax

```syntaxsql
CREATE TABLE [ IF NOT EXISTS ] [ schema_name . ] table_name
( <table_element> [ , ...n ] )
[ ; ]

CREATE TABLE [ IF NOT EXISTS ] [ schema_name . ] table_name
[ ( column_name [ , ...n ] ) ] AS query_body [ WITH [ NO ] DATA ] [ ; ]

<table_element> ::= { <column_definition> | <table_constraint> }
<column_definition> ::= column_name data_type
    [ COLLATE collation_name ]
    [ NULL | NOT NULL ]
    [ { DEFAULT default_expression | identity_clause } ]
    [ <column_constraint> ...n ]
<column_constraint> ::= [ CONSTRAINT constraint_name ]
    { PRIMARY KEY | UNIQUE | CHECK ( search_condition )
    | REFERENCES [ schema_name . ] referenced_table ( column_name [ , ...n ] )
      [ ON DELETE { CASCADE | RESTRICT | NO ACTION | SET NULL | SET DEFAULT } ] }
<table_constraint> ::= [ CONSTRAINT constraint_name ]
    { PRIMARY KEY ( column_name [ , ...n ] )
    | UNIQUE ( column_name [ , ...n ] )
    | CHECK ( search_condition )
    | FOREIGN KEY ( column_name [ , ...n ] )
      REFERENCES [ schema_name . ] referenced_table ( column_name [ , ...n ] )
      [ ON DELETE { CASCADE | RESTRICT | NO ACTION | SET NULL | SET DEFAULT } ] }
```

## Arguments

- **`table_name`** — A table in the current database; qualification selects its SQL namespace.
- **`data_type`** — A declared [type name](../data-types/index.md), with permitted parameters.
- **`default_expression`** — A literal or column-free scalar function default convertible to the storage type and within its bounds.
- **`collation_name`** — A supported string comparison rule.
- **`constraint_name`** — An optional retained name for a column or table constraint.
- **`identity_clause`** — A supported [GENERATED or SQL Server IDENTITY declaration](../clauses/identity.md#syntax).

## Remarks

[CREATE TABLE AS](#create-table-as) infers a new table from a query.
[TRUNCATE](truncate.md) empties existing table sets with identity and foreign-key policies
while preserving definitions.

The namespace must already exist; use [CREATE SCHEMA](create-schema.md). Unqualified names use dbo,
and code-first provisioning remains pinned to dbo. Missing namespaces use CS01028 (3F000)
for DDL; ordinary reads/DML retain CS01014 (42P01).

`IF NOT EXISTS` on an existing table returns success with zero affected rows and leaves the
definition unchanged, skipping key declaration validation. It does not make system relations writable.
Normal session-created objects have `DatabaseObjectOwner.Adhoc` ownership; compiled-schema
provisioning creates `DatabaseObjectOwner.Schema` objects.

An existing table without IF NOT EXISTS reports CS03049 (42P07). Duplicate constraint names
use CS03050 (42710) and duplicate columns CS03061 (42701). Execution/DDL refusals carry a
statement span when source text is available; message text has no embedded code prefix.
See [constraint declaration diagnostics](../constraints.md) and [text DEFAULT conversion](../clauses/default.md).

Columns are nullable unless constrained otherwise. [IDENTITY](../clauses/identity.md) permits one
built-in integer generator per table, implies NOT NULL and excludes a DEFAULT declaration.
Explicit NULL combined with NOT NULL, PRIMARY KEY (including table keys) or IDENTITY in either
order reports `CS00003`, naming the column, before any catalog change. This rule also applies to
ADD and ALTER column definitions. [Defaults](../clauses/default.md) admit column-free scalar
expressions, including arithmetic, CAST and Immutable/Stable/Volatile calls. Immutable expressions
fold at declaration; Stable/Volatile calls run per assigned row on the transaction-start clock.
Function result types must satisfy INSERT assignment compatibility (`CS05013`); columns,
parameters and aggregates are invalid defaults, and subqueries receive `CK00001`. Strings are
not truncated and decimal defaults are not rounded. Omitted ON DELETE means NO ACTION.

`UNIQUE` is enforced by a unique catalog index and treats `NULL` as an
equal key. `CHECK` rejects `FALSE` but accepts `UNKNOWN`.

PRIMARY KEY, UNIQUE and FOREIGN KEY lists must not repeat a column, including the referenced
FOREIGN KEY list; comparison is case-insensitive. A table may have one PRIMARY KEY declaration,
whether inline or table-level. Repeated clauses on the same column also count as multiple
declarations. These failures report `CS01019` before publication, naming the constraint and
duplicate column or table. Repeated FOREIGN KEY child columns are refused more strictly than
PostgreSQL. Composite keys with distinct columns remain supported.

A new CHECK may use bare columns, `table.column` or `schema.table.column` for this table.
A different table or schema qualifier reports located `CS01015` before publication.
See [CHECK binding](../constraints.md#check-binding) for existing stored and declared checks.

Data definition language (DDL) is self-committing. It is rejected inside an explicit transaction
with `CS04003`. See [Constraints](../constraints.md) for foreign-key and check restrictions.

## CREATE TABLE AS

`AS query_body` accepts the ordinary SELECT/set-operation subset, including a non-recursive WITH
inside the body and parameters. WITH DATA is the default and copies the query's rows, including
duplicates. WITH NO DATA binds and validates the query, then creates an empty table without
evaluating rows, predicates, functions or row-limit expressions.

Types, length/precision/scale facets, collations and domain identities follow
[view output inference](create-view.md#remarks). An untyped NULL becomes TEXT. Every destination
column is nullable; source NOT NULL, keys, CHECKs, column defaults, identities and indexes are not
copied. Inferred domain constraints and domain defaults still apply.

The optional name list must name **every** output column. This matches CREATE VIEW and differs
from PostgreSQL's partial CTAS rename list. Without it, every output needs a name; blank or
case-insensitively duplicate names are refused with CS03068 (42601). Supply aliases or a complete
name list for unnamed expressions.

IF NOT EXISTS skips binding and execution when the table already exists. It also succeeds with
zero affected rows if another session wins the name while the query builds; the winner's table
is kept. Without the guard, a name collision reports CS03049 (42P07). A successful WITH DATA
reports the number of rows written; WITH NO DATA, empty input and a skip report 0.

CTAS is self-committing and is refused inside an explicit transaction with CS04003 (25001).
A failed build leaves no table, including after crash recovery. Publication exposes the complete
table atomically. Once it is published, a snapshot taken before the builder transaction committed
sees the new table empty; a snapshot taken after that commit sees every row. No snapshot sees a
partial build. Source reads use one statement snapshot.

The statement memory budget covers retained source results and copy buffers as well as live hash
builds. Exhaustion reports CS05067 (53200) before publication; there is no spill. A running CTAS
holds the same metadata lease as CREATE INDEX: other sessions' definition DDL for views, functions,
domains and schemas fails immediately while it runs. Unrelated table DDL can proceed.

CTAS reuses TRUNCATE's durable prepared-owner reclamation machinery. Abandoned build storage is
reclaimed after statement/snapshot pins clear. SQL storage remains **format 26**.

### CTAS diagnostics

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CS03068 | Missing, blank, duplicate or wrong-width output names | 42601 (`syntax_error`) |
| CS03049 | Destination exists, including a name lost during the build, without IF NOT EXISTS | 42P07 (`duplicate_table`) |
| CS04003 | CTAS inside an explicit transaction | 25001 (`active_sql_transaction`) |
| CS05067 | Retained source, copy buffers or hash builds exhaust StatementMemoryBudget | 53200 (`out_of_memory`) |

Source binding, conversion and domain failures retain their ordinary codes.

## Examples

```sql
CREATE TABLE created (id INT PRIMARY KEY, label TEXT DEFAULT 'new');
INSERT INTO created (id) VALUES (1);
SELECT label FROM created;
```

The inserted row reads `new` for `label`.

```sql
CREATE TABLE c (code INT, CONSTRAINT uq_code UNIQUE(code));
```

This creates a unique constraint on code. In a separate empty database,
`CREATE TABLE bad (a INT, CONSTRAINT pk_bad PRIMARY KEY(a, A));` reports CS01019
and creates nothing. After `CREATE TABLE existing (a INT);`,
`CREATE TABLE IF NOT EXISTS existing (a INT PRIMARY KEY, b INT PRIMARY KEY);`
succeeds without adding a primary key or column b.

### Copy rows or only the definition

```sql
CREATE TABLE source_rows (id INT PRIMARY KEY, label VARCHAR(12) NOT NULL);
INSERT INTO source_rows VALUES (1,'one'),(2,'two');
CREATE TABLE copied (n,word) AS SELECT id,label FROM source_rows;
SELECT * FROM copied ORDER BY n;
CREATE TABLE empty_copy AS SELECT 1/0 AS n WITH NO DATA;
SELECT COUNT(*) AS n FROM empty_copy;
CREATE TABLE null_copy AS SELECT NULL AS value;
```

The first CTAS reports 2 affected rows; SELECT returns `(1,'one')`, `(2,'two')`.
The copied columns are nullable and have no primary key. The second CTAS reports 0, does not
evaluate `1/0`, and its COUNT returns 0. The last table has one nullable TEXT column and one NULL
row. `CREATE TABLE bad (n) AS SELECT 1,2;` instead reports CS03068 and creates no table.

## See also

[Data types](../data-types/index.md) · [Constraints](../constraints.md) · [COLLATE](../clauses/collate.md) · [ALTER TABLE](alter-table.md)

[Statements](index.md) · [Language (SQL)](../index.md)

## Sources

- **CTAS implementation and tests** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Internal/Planning/SqlPlanner.TableAs.cs`, `src/Internal/Execution/SqlPlanExecutor.TableAs.cs` and `tests/SqlCreateTableAsTests*.cs` in the same project.
- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`
- **Implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlQueryParser.Ddl.cs`
- **Implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlQueryParser.Constraints.cs`

