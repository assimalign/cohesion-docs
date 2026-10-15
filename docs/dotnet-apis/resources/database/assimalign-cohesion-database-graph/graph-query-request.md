# GraphQueryRequest

The `GraphQueryRequest` type is part of the documented `Assimalign.Cohesion.Database.Graph` API.

> **Status:** Partial.

A parsed, database-scoped GQL statement.

Namespace: `Assimalign.Cohesion.Database.Graph`.

## Documented behavior

- **`GraphQueryRequest.FromGql(text)`** — parses a request and throws `DatabaseParseException` for an
  error diagnostic. `IDatabaseSession.ExecuteAsync(string)` uses the same parser.

## Related reference

- **Assembly** — [`Assimalign.Cohesion.Database.Graph`](index.md).
- **Design** — [Lifetime and implementation decisions](design.md).
- **Examples** — [Source-backed usage](examples/index.md).

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/docs/Assembly/Assimalign.Cohesion.Database.Graph/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/GraphQueryRequest.cs`.
