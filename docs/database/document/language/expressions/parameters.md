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
An empty or malformed marker, `?` and `:name` produce `OQL0002`.

Keys bind case-sensitively by bare name, ignoring one leading sigil even if the caller dictionary
uses a case-insensitive comparer. `$01` is name 01 rather than position 1. Numeric names contain
ASCII digits only; named forms start with a letter or underscore, then letters/digits/underscores.
Supplied NULL is bound; unused keys are ignored, but normalized collisions, including unused keys,
report `COHDOC009`. Missing references report `COHDOC005` before rows are read or written.
Unsupported markers use the same message as SQL/GQL:

```text
Unsupported parameter form; use $name, @name, or $1. Names must be an identifier (a letter or _, then letters, digits or _) or digits only.
```

## Examples

The parser corpus accepts both named marker forms:

```sql
SELECT p.name FROM people AS p WHERE p.age >= $age
```

```sql
SELECT p.name FROM people p WHERE p.age >= @age
```

For people with age 20, binding bare `age` to 18 retains that row in both examples.
`age` and `@age` together fail with COHDOC009.

## See also

[Expressions](index.md), [Syntax conventions](../syntax-conventions.md), and [WHERE](../clauses/where.md).

## Sources

- **Marker forms** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/docs/DESIGN.md`.
- **Parameter nodes** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/OqlQueryParser.Expressions.cs`.
- **Index parameter binding** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/docs/DESIGN.md`.
- **Examples** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlQueryParserTests.cs`.
