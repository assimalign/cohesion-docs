# Statements

Graph statements match data, mutate bound graphs, or inspect the session database catalog.

> **Status:** Implemented.

## Queries and mutations

- **[MATCH and OPTIONAL MATCH](match.md)** — bind nodes and relationships before projection or mutation.
- **[RETURN](return.md)** — query composition and result projection.
- **[Linear composition](../clauses/next.md)** — LET, FILTER, FOR/UNWIND, RETURN … NEXT and Cypher WITH.
- **[INSERT](insert.md)** — insert nodes and relationships.
- **[CREATE](create.md)** — compatibility spelling for insertion.
- **[DELETE](delete.md)** — restricted deletion of matched elements.
- **[DETACH DELETE](detach-delete.md)** — deletion with incident-relationship cascading.
- **[SHOW](show.md)** — read-only catalog metadata.

A query reading graph elements binds variables through MATCH; LET and FOR/UNWIND add value or
proven entity bindings. Insertion, RETURN and value pipelines may stand alone. `SHOW` is a separate
statement and cannot be combined with graph matching, projection, or mutation.

- **[EXPLAIN](explain.md)** — Return a chosen plan without executing it.

## See also

- **[Language (GQL)](../index.md)** — reference hub.

## Sources

- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/docs/DESIGN.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlQueryParserTests.cs`.
