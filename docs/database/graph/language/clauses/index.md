# Clauses

GQL clauses filter matched bindings and project graph elements or scalar properties.

> **Status:** Implemented.

- **[WHERE](where.md)** — scalar comparisons and conjunction.
- **[RETURN](return.md)** — variables, properties, and aliases.

`WHERE` follows the matching patterns. `RETURN` finishes a read query or follows insertion.
Catalog `SHOW` statements cannot carry either clause.

## See also

- **[Language (GQL)](../index.md)** — reference hub.
- **[Statements](../statements/index.md)** — complete query forms.

## Sources

- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/docs/DESIGN.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlQueryParserTests.cs`.

