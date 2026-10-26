# CREATE TABLE (Cohesion SQL)

Creates a stored table with typed columns and supported constraints.

> **Status:** Partial. The documented subset is measured against the live SQL engine.

## Syntax

```syntaxsql
CREATE TABLE [ IF NOT EXISTS ] [ schema_name . ] table_name
( <table_element> [ , ...n ] )
[ ; ]

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

CREATE TABLE ... AS SELECT remains refused with CS00003. Declare the table, then use
[INSERT SELECT](insert.md). [TRUNCATE](truncate.md) empties existing table sets with identity and
foreign-key policies while preserving definitions.

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

## See also

[Data types](../data-types/index.md) · [Constraints](../constraints.md) · [COLLATE](../clauses/collate.md) · [ALTER TABLE](alter-table.md)

[Statements](index.md) · [Language (SQL)](../index.md)

## Sources

- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`
- **Implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlQueryParser.Ddl.cs`
- **Implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlQueryParser.Constraints.cs`

