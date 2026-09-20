# Parameters (Cohesion SQL)

Binds runtime values to named or numbered placeholders in a SQL statement.

> **Status:** Partial. This page describes the executable subset and its boundaries.

## Syntax

```syntaxsql
@parameter_name
$parameter_number
```

## Arguments

- **`parameter_name`** — A value name supplied with the command.
- **`parameter_number`** — The numeric name in the dollar-prefixed form, such as `$1`.

## Remarks

The SQL client normalizes a leading `@` or `$` away from parameter names; bare names travel on
the wire. Parameters carry values, not identifiers or syntax. Their runtime types matter for
conversion and comparison, unlike integer and fractional literals' fixed evaluator types.

A parameter in an ordering expression is a value, not a select-list ordinal. Parameters are excluded
from literal defaults and deterministic row-local check constraints.

Use `SqlCommand.WithParameter` or the client parameter collection. Parsing and execution occur
at the server; the typed client does not parse SQL.

## Examples

Use the [conformance fixture](../statements/select.md#a-create-the-conformance-fixture).

```sql
SELECT id FROM t WHERE age > @minimum ORDER BY id;
```

Bind the bare name `minimum` to `40` to return `2` and `3`, as in the SELECT client example.

## See also

[Expressions](index.md) · [SELECT](../statements/select.md) · [Data types](../data-types/index.md)

## Sources

- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`
- **Implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Client/src/SqlCommand.cs`
- **Implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Client/docs/DESIGN.md`

