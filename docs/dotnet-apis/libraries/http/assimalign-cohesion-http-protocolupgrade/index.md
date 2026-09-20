# Assimalign.Cohesion.Http.ProtocolUpgrade

Models HTTP/1.1 upgrades and CONNECT tunnels.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Http](../index.md)

## Scope

The interceptor detects eligible requests and coordinates the transition response. Acceptance writes
the switching or tunnel response and transfers the raw duplex stream to the caller. The inner
protocol's lifetime then belongs to the accepting application.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Http`](../../http/assimalign-cohesion-http/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.Http.Cookies`](../../http/assimalign-cohesion-http-cookies/index.md) | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `HttpProtocolUpgrade` | `src/HttpProtocolUpgrade.cs` |
| `HttpProtocolUpgradeKind` | `src/HttpProtocolUpgradeKind.cs` |
| `HttpContextProtocolUpgradeExtensions` | `src/Extensions/HttpContextProtocolUpgradeExtensions.cs` |
| `IHttpProtocolUpgrade` | `src/Abstractions/IHttpProtocolUpgrade.cs` |
| `IHttpProtocolUpgradeFeature` | `src/Abstractions/IHttpProtocolUpgradeFeature.cs` |

## Sources

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ProtocolUpgrade/src/Assimalign.Cohesion.Http.ProtocolUpgrade.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ProtocolUpgrade/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ProtocolUpgrade/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Http/README.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ProtocolUpgrade/src`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ProtocolUpgrade/src/HttpProtocolUpgrade.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ProtocolUpgrade/src/HttpProtocolUpgradeKind.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ProtocolUpgrade/src/Extensions/HttpContextProtocolUpgradeExtensions.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ProtocolUpgrade/src/Abstractions/IHttpProtocolUpgrade.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ProtocolUpgrade/src/Abstractions/IHttpProtocolUpgradeFeature.cs`.
