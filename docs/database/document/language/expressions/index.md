# Expressions

OQL expressions combine document paths, scalar values, parameters, operators, and aggregate calls.

## Expression reference

- **Document access** — [Paths and arrays](paths-and-arrays.md) addresses nested properties and elements.
- **Evaluation** — [Operators](operators.md) defines arithmetic, comparisons, null tests, and precedence.
- **Presence** — [MISSING and presence tests](missing.md) separates absent fields from NULL.
- **Conditional values** — [CASE](case.md) chooses a branch.
- **JSON objects** — [Object constructors](object-constructors.md) builds keyed values.
- **JSON arrays** — [Array constructors](array-constructors.md) builds ordered values with NULL for MISSING elements.
- **Array mappings** — [ARRAY and FIRST comprehensions](comprehensions.md) map and filter scoped elements in input order.
- **Constant values** — [Literals](literals.md) covers strings, decimal numbers, Boolean values, and null.
- **Nested queries** — [Expression subqueries](subqueries.md) materialize SELECT results; [EXISTS](exists.md) tests nonempty arrays.
- **Bound values** — [Parameters](parameters.md) explains named and numbered markers.

- **Array predicates** — [ANY/SOME/EVERY](quantified-predicates.md) test scoped elements; [IN](operators.md#in-and-between) also accepts an array operand.

Parentheses group expressions. Aggregate calls are documented under
[Aggregate functions](../functions/aggregate-functions.md). [Expression subqueries](subqueries.md) return arrays; [EXISTS](exists.md) tests array presence. Correlated and DML subqueries execute with shared work/memory limits. [Derived and expression FROM](../clauses/from.md) execute. Recursive expression parsing is capped at 128 levels.

## See also

[Language (OQL)](../index.md) and [SELECT](../statements/select.md).

## Sources

- **Expressions** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/OqlQueryParser.Expressions.cs`.
- **Scope** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/docs/DESIGN.md`.
