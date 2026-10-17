# Clauses

GQL clauses filter matched bindings and project graph bindings or scalar expressions.

> **Status:** Implemented.

- **[WHERE](where.md)** — Boolean expressions, arithmetic and exact numeric comparisons.
- **[RETURN](return.md)** — expressions, aliases, DISTINCT and named-binding star.

`WHERE` follows the matching patterns. `RETURN` finishes a read query or follows insertion.
RETURN can also stand alone to evaluate one row of expressions.
Catalog `SHOW` statements cannot carry either clause.

## See also

- **[Language (GQL)](../index.md)** — reference hub.
- **[Statements](../statements/index.md)** — complete query forms.

## Sources

- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/docs/DESIGN.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlQueryParserTests.cs`.

