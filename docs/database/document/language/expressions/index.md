# Expressions

OQL expressions combine document paths, scalar values, parameters, operators, and aggregate calls.

## Expression reference

- **Document access** — [Paths and arrays](paths-and-arrays.md) addresses nested properties and elements.
- **Evaluation** — [Operators](operators.md) defines arithmetic, comparisons, null tests, and precedence.
- **Constant values** — [Literals](literals.md) covers strings, decimal numbers, Boolean values, and null.
- **Bound values** — [Parameters](parameters.md) explains named and numbered markers.

Parentheses group expressions. Aggregate calls are documented under
[Aggregate functions](../functions/aggregate-functions.md). Nested queries and collection
constructors are reserved but unsupported. Recursive expression parsing is capped at 128 levels.

## See also

[Language (OQL)](../index.md) and [SELECT](../statements/select.md).

## Sources

- **Expressions** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/OqlQueryParser.Expressions.cs`.
- **Scope** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/docs/DESIGN.md`.
