# Clauses

GQL clauses filter matched bindings and project graph bindings or scalar expressions.

> **Status:** Implemented.

- **[LET](let.md)** — add value bindings in assignment order.
- **[FILTER](filter.md)** — filter the current binding table.
- **[FOR and UNWIND](for.md)** — expand lists, retaining proven entity element kinds.
- **[WITH](with.md)** — intermediate projection with scoped aliases and filtering after paging.
- **[NEXT](next.md)** — compose source-ordered segments in one statement.

- **[WHERE](where.md)** — Boolean expressions, arithmetic and exact numeric comparisons.
- **[RETURN](return.md)** — expressions, aliases, DISTINCT and named-binding star.
- **[GROUP BY](group-by.md)** — Implicit or ISO grouping, RETURN aliases and the empty grouping set.
- **[ORDER BY](order-by.md)** — expression keys, direction and explicit NULL placement.
- **[OFFSET, SKIP and LIMIT](limit-offset.md)** — output paging after ordering.

- **[SET](set.md)** — assign scalar properties, replace/merge maps and add node labels.
- **[REMOVE](remove.md)** — remove properties or node labels.

`WHERE` follows matching patterns or a WITH tail. `RETURN` finishes a read query or precedes NEXT.
RETURN can also stand alone to evaluate one row of expressions. Its optional tail is GROUP BY, then ORDER BY,
then OFFSET/SKIP, then LIMIT; OFFSET/SKIP after LIMIT is invalid.
Catalog SHOW statements cannot carry these clauses.

## See also

- **[Language (GQL)](../index.md)** — reference hub.
- **[Statements](../statements/index.md)** — complete query forms.

## Sources

- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/docs/DESIGN.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlQueryParserTests.cs`.

