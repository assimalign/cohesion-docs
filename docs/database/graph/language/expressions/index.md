# Expressions

GQL expressions provide scalar literals, property references, comparisons, and conjunction.

> **Status:** Implemented.

- **[Operators](operators.md)** — Boolean, arithmetic, null/truth, membership and string predicates.
- **[Parameters](parameters.md)** — runtime scalar bindings in maps and WHERE.
- **[Literals](literals.md)** — null, Boolean, integer, floating-point, and string values.

WHERE accepts literals, parameters and one-level properties in scalar expressions. RETURN still
projects bound variables/properties only. Lists are restricted to IN's right operand. The builtin
function table is empty; function calls and nested property access remain unsupported.

## See also

- **[Language (GQL)](../index.md)** — reference hub.
- **[WHERE](../clauses/where.md)** — predicate composition.
- **[RETURN](../clauses/return.md)** — projection grammar.

## Sources

- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/docs/DESIGN.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlQueryParserTests.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlQueryParser.Expressions.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlLanguageProfile.cs`.
