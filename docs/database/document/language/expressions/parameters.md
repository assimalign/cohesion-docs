# Parameters

OQL parameters carry named or numbered values into expressions without embedding them in query text.

## Syntax

```syntaxsql
<parameter> ::= { $parameter_name | $parameter_number | @parameter_name }
```

## Arguments

- **`parameter_name`** — the nonempty name following `$` or `@`.
- **`parameter_number`** — a numbered marker such as `$1`.

## Remarks

`OqlParameterExpression.Name` retains the marker text without its leading character. Parameters
can appear in projections and predicates, and the physical planner can use parameter values in
indexed equality or range bounds. The parser validates marker syntax; it does not supply values.
An empty marker and the `?` marker produce `OQL0002`.

## Examples

The parser corpus accepts both named marker forms:

```sql
SELECT p.name FROM people AS p WHERE p.age >= $age
```

```sql
SELECT p.name FROM people p WHERE p.age >= @age
```

## See also

[Expressions](index.md), [Syntax conventions](../syntax-conventions.md), and [WHERE](../clauses/where.md).

## Sources

- **Marker forms** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/docs/DESIGN.md`.
- **Parameter nodes** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/OqlQueryParser.Expressions.cs`.
- **Index parameter binding** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/docs/DESIGN.md`.
- **Examples** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlQueryParserTests.cs`.
