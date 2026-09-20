# Assimalign.Cohesion.Http.ExtendedConnect

Exposes extended CONNECT exchanges through an optional HTTP feature.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Http](../index.md)

## Scope

The feature identifies a CONNECT exchange carrying the `:protocol` pseudo-header and exposes the
requested protocol. It describes the transition request; application code chooses the response and
inner protocol. It does not itself implement WebSocket behavior.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Http`](../../http/assimalign-cohesion-http/index.md) | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `HttpExtendedConnectExtensions` | `src/Extensions/HttpExtendedConnectExtensions.cs` |
| `IHttpExtendedConnectFeature` | `src/IHttpExtendedConnectFeature.cs` |

## Sources

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ExtendedConnect/src/Assimalign.Cohesion.Http.ExtendedConnect.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ExtendedConnect/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ExtendedConnect/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Http/README.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ExtendedConnect/src`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ExtendedConnect/src/Extensions/HttpExtendedConnectExtensions.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ExtendedConnect/src/IHttpExtendedConnectFeature.cs`.
