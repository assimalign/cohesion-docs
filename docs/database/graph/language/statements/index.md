# Statements

Graph statements match data, mutate bound graphs, or inspect the session database catalog.

> **Status:** Implemented.

## Queries and mutations

- **[MATCH and OPTIONAL MATCH](match.md)** — bind nodes and relationships before projection or mutation.
- **[RETURN](return.md)** — query composition and result projection.
- **[INSERT](insert.md)** — insert nodes and relationships.
- **[CREATE](create.md)** — compatibility spelling for insertion.
- **[DELETE](delete.md)** — restricted deletion of matched elements.
- **[DETACH DELETE](detach-delete.md)** — deletion with incident-relationship cascading.
- **[SHOW](show.md)** — read-only catalog metadata.

A query reading graph elements or a deletion binds variables through `MATCH`. Insertion and
RETURN may stand alone; standalone RETURN evaluates one row. `SHOW` is a separate
statement and cannot be combined with graph matching, projection, or mutation.

- **[EXPLAIN](explain.md)** — Return a chosen plan without executing it.

## See also

- **[Language (GQL)](../index.md)** — reference hub.

## Sources

- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/docs/DESIGN.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlQueryParserTests.cs`.
