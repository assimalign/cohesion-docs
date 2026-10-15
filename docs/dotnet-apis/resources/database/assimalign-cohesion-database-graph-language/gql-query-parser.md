# GqlQueryParser

The `GqlQueryParser` type is part of the documented `Assimalign.Cohesion.Database.Graph.Language` API.

> **Status:** Partial.

Parses the executable GQL subset using the shared lexer and analyzer pipeline.

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
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlQueryParser.cs`.
