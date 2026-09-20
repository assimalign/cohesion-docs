# GqlQueryStatement

The `GqlQueryStatement` type is part of the documented `Assimalign.Cohesion.Database.Graph.Language` API.

> **Status:** Partial.

A GQL statement and its parse diagnostics.

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Documented behavior

`GqlQueryParser` derives from the shared `QueryParser` and returns `GqlQueryStatement`, whose
diagnostics describe malformed or unsupported input. `GqlLanguageProfile.Instance` advertises only
executable clauses. `GqlClauses` names lexical capability keys, including deferred clauses.

## Related reference

- **Assembly** — [`Assimalign.Cohesion.Database.Graph.Language`](index.md).
- **Design** — [Lifetime and implementation decisions](design.md).
- **Examples** — [Source-backed usage](examples/index.md).

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/docs/Assembly/Assimalign.Cohesion.Database.Graph.Language/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlQueryStatement.cs`.
