# FROM (Cohesion SQL)

Identifies the stored table or system relation read by a query.

> **Status:** Partial. The documented subset executes; restrictions are listed below.

## Syntax

```syntaxsql
FROM [ schema_name . ] table_name [ [ AS ] table_alias ]
```

## Arguments

- **`table_name`** — A stored table or one of the named virtual system relations.
- **`schema_name`** — A SQL namespace inside the session's database.
- **`table_alias`** — An optional name used to qualify column references.

## Remarks

Every executable `SELECT` requires `FROM`. A session cannot select another database through
qualification or switch databases through SQL. System relations require the `INFORMATION_SCHEMA`
or `COHESION_SCHEMA` qualifier.

An optional supported [JOIN](join.md) adds exactly one stored or system relation. Comma-separated
sources and derived tables report `COHDBL001`. An explicit alias hides the base name in SELECT,
UPDATE and DELETE: `SELECT t.id FROM t AS x` reports located `COHSQLE015`; use `x.id`.
Unknown or ambiguous columns fail during planning, including on empty inputs.

## Examples

These examples use the [conformance fixture](../statements/select.md#a-create-the-conformance-fixture).

```sql
SELECT u.name FROM dbo.t AS u WHERE id = 1;
```

This conformance query returns `Ada`.

## See also

[Clauses](index.md) · [SELECT](../statements/select.md) · [Unsupported syntax](../unsupported.md)

## Sources

- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`
