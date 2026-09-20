# Expressions

GQL expressions provide scalar literals, property references, comparisons, and conjunction.

> **Status:** Implemented.

- **[Operators](operators.md)** — comparison operators and `AND`.
- **[Literals](literals.md)** — null, Boolean, integer, floating-point, and string values.

The scalar operand grammar permits a literal or exactly one `variable.property` reference.
Projection additionally permits whole bound variables. The builtin-function table is empty.
Parameters, function calls, collection literals, and nested property access are outside the
executable expression surface.

## See also

- **[Language (GQL)](../index.md)** — reference hub.
- **[WHERE](../clauses/where.md)** — predicate composition.
- **[RETURN](../clauses/return.md)** — projection grammar.

## Sources

- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/docs/DESIGN.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlQueryParserTests.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlQueryParser.Expressions.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlLanguageProfile.cs`.

