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
    [ DEFAULT literal ]
    [ <column_constraint> ...n ]
<column_constraint> ::= [ CONSTRAINT constraint_name ]
    { PRIMARY KEY | UNIQUE | CHECK ( search_condition )
    | REFERENCES [ schema_name . ] referenced_table ( column_name [ , ...n ] )
      [ ON DELETE { CASCADE | RESTRICT } ] }
<table_constraint> ::= [ CONSTRAINT constraint_name ]
    { PRIMARY KEY ( column_name [ , ...n ] )
    | UNIQUE ( column_name [ , ...n ] )
    | CHECK ( search_condition )
    | FOREIGN KEY ( column_name [ , ...n ] )
      REFERENCES [ schema_name . ] referenced_table ( column_name [ , ...n ] )
      [ ON DELETE { CASCADE | RESTRICT } ] }
```

## Arguments

- **`table_name`** — A table in the current database; qualification selects its SQL namespace.
- **`data_type`** — A declared [type name](../data-types/index.md), with permitted parameters.
- **`literal`** — A default convertible to the column's storage type and within its bounds.
- **`collation_name`** — A supported string comparison rule.
- **`constraint_name`** — An optional retained name for a column or table constraint.

## Remarks

`IF NOT EXISTS` permits an already existing table. It does not make system relations writable.
Normal session-created objects have `DatabaseObjectOwner.Adhoc` ownership; compiled-schema
provisioning creates `DatabaseObjectOwner.Schema` objects.

Columns are nullable unless constrained otherwise. Defaults are literal-only: functions, parameters,
`CAST`, and other expressions are rejected before mutation. Strings are not truncated and decimal
defaults are not rounded. `UNIQUE` is enforced by a unique catalog index and treats `NULL` as an
equal key. `CHECK` rejects `FALSE` but accepts `UNKNOWN`.

Data definition language (DDL) is self-committing. It is rejected inside an explicit transaction
with `COHSQLT003`. See [Constraints](../constraints.md) for foreign-key and check restrictions.

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

## See also

[Data types](../data-types/index.md) · [Constraints](../constraints.md) · [COLLATE](../clauses/collate.md) · [ALTER TABLE](alter-table.md)

[Statements](index.md) · [Language (SQL)](../index.md)

## Sources

- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`
- **Implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlQueryParser.Ddl.cs`
- **Implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlQueryParser.Constraints.cs`

