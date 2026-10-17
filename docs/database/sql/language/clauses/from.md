# FROM (Cohesion SQL)

Identifies stored tables and system relations, with a join tree for each comma-separated item.

> **Status:** Partial. The documented subset executes; restrictions are listed below.

## Syntax

```syntaxsql
FROM <table_source> [ <join> ...n ] [ , <table_source> [ <join> ...n ] ...n ]

<table_source> ::= [ schema_name . ] table_name [ [ AS ] table_alias ]
```

## Arguments

- **`table_name`** — A stored table or one of the named virtual system relations.
- **`schema_name`** — A SQL namespace inside the session's database.
- **`table_alias`** — An optional name used to qualify column references.

## Remarks

Every executable `SELECT` requires `FROM`. A session cannot select another database through
qualification or switch databases through SQL. System relations require the `INFORMATION_SCHEMA`
or `COHESION_SCHEMA` qualifier.

Each item can contain any number of [INNER, LEFT or CROSS joins](join.md). Comma-separated
items produce a Cartesian product; derived tables report `COHDBL001`. JOIN binds more tightly
than comma: in `FROM a, b JOIN c ON ...`, ON sees only b and c. It cannot see later inputs.
An explicit alias hides the base name in SELECT,
UPDATE and DELETE: `SELECT t.id FROM t AS x` reports located `COHSQLE015`; use `x.id`.
Unknown or ambiguous columns fail during planning, including on empty inputs.
Invalid qualifiers/columns use `COHSQLE015`; duplicate table bindings use `COHSQLE019`.

## Examples

These examples use the [conformance fixture](../statements/select.md#a-create-the-conformance-fixture).

```sql
SELECT u.name FROM dbo.t AS u WHERE id = 1;
```

This conformance query returns `Ada`.

```sql
SELECT COUNT(*) FROM t AS a, t AS b CROSS JOIN t AS c;
```

This returns `27`: each of the three inputs contains three rows. An empty input makes a product
empty. See [JOIN](join.md) for LEFT's NULL extension and USING's merged columns.

## See also

[Clauses](index.md) · [SELECT](../statements/select.md) · [Unsupported syntax](../unsupported.md)

## Sources

- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`
