# Cache

Cache is a deferred database model whose current packages contain scaffolding rather than an engine.

> **Status:** Not yet implemented. Cache behavior is outside the database minimum viable product.

The package design explicitly states that Cache has no implemented engine, persistence, or
lifecycle and is deferred behind Key-Value Pair. Its source contains an empty `Class1` in
`Assimalign.Cohesion.Database.CacheDb`; the test is a placeholder. These stubs do not define
the future Cache architecture.

## Current package surface

| Package | Present source |
|---|---|
| `Assimalign.Cohesion.Database.Cache` | Empty class and placeholder test |
| `Assimalign.Cohesion.Database.Cache.Language` | Project files, without a parser or grammar |
| `Assimalign.Cohesion.Database.Cache.Catalog` | Project files, without catalog implementation |
| `Assimalign.Cohesion.Database.Cache.Client` | Project files, without client implementation |
| `Assimalign.Cohesion.Database.Cache.Storage` | Project files, without storage implementation |

No Cache commands, operand types, wire operations, or diagnostic identifiers are defined by
these projects. The design requires a future implementation to compose the shared Database
kernel, but does not establish a command contract.

## Reference

- **[Unsupported surface](unsupported.md)** — exact boundaries of the scaffold.
- **[Key-Value Pair](../key-value-pair/index.md)** — the implemented ordered byte-key engine.
- **[Database](../index.md)** — model navigation.
- **[Database overview](../overview.md)** — shared kernel context.
- **[Database API reference](../../dotnet-apis/resources/database/index.md)** — resource packages.

## Sources

- **Scope** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Cache/docs/OVERVIEW.md`.
- **Design** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Cache/docs/DESIGN.md`.
- **Source stub** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Cache/src/Class1.cs`.
- **Test stub** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Cache/tests/UnitTest1.cs`.
- **Language scaffold** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Cache.Language/src/Assimalign.Cohesion.Database.Cache.Language.csproj`.
- **Catalog scaffold** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Cache.Catalog/src/Assimalign.Cohesion.Database.Cache.Catalog.csproj`.
- **Client scaffold** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Cache.Client/src/Assimalign.Cohesion.Database.Cache.Client.csproj`.
- **Storage scaffold** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Cache.Storage/src/Assimalign.Cohesion.Database.Cache.Storage.csproj`.
