# Expressions

OQL expressions combine document paths, scalar values, parameters, operators, and aggregate calls.

## Expression reference

- **Document access** — [Paths and arrays](paths-and-arrays.md) addresses nested properties and elements.
- **Evaluation** — [Operators](operators.md) defines arithmetic, comparisons, null tests, and precedence.
- **Presence** — [MISSING and presence tests](missing.md) separates absent fields from NULL.
- **Conditional values** — [CASE](case.md) chooses a branch.
- **JSON objects** — [Object constructors](object-constructors.md) builds keyed values.
- **JSON arrays** — [Array constructors](array-constructors.md) builds ordered values with NULL for MISSING elements.
- **Constant values** — [Literals](literals.md) covers strings, decimal numbers, Boolean values, and null.
- **Bound values** — [Parameters](parameters.md) explains named and numbered markers.

Parentheses group expressions. Aggregate calls are documented under
[Aggregate functions](../functions/aggregate-functions.md). Nested queries remain unsupported. Recursive expression parsing is capped at 128 levels.

## See also

[Language (OQL)](../index.md) and [SELECT](../statements/select.md).

## Sources

- **Expressions** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/OqlQueryParser.Expressions.cs`.
- **Scope** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/docs/DESIGN.md`.
