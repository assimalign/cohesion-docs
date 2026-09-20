# Assimalign.Cohesion.Http.Sessions

Defines per-exchange binary session state and typed convenience access.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Http](../index.md)

## Scope

`IHttpSession` exposes byte-array values, keys, availability, and explicit load/commit operations.
Typed string and integer helpers are extension members over that binary contract. An attached
feature supplies the session without adding application state to the protocol core.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Http`](../../http/assimalign-cohesion-http/index.md) | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `HttpContextSessionExtensions` | `src/Extensions/HttpContextSessionExtensions.cs` |
| `HttpSession` | `src/HttpSession.cs` |
| `HttpSessionExtensions` | `src/Extensions/HttpSessionExtensions.cs` |
| `HttpSessionOptions` | `src/HttpSessionOptions.cs` |
| `HttpSessionSerializer` | `src/HttpSessionSerializer.cs` |
| `HttpSessionStoreExtensions` | `src/Extensions/HttpSessionStoreExtensions.cs` |
| `IHttpSession` | `src/Abstractions/IHttpSession.cs` |
| `IHttpSessionFeature` | `src/Abstractions/IHttpSessionFeature.cs` |
| `IHttpSessionStore` | `src/Abstractions/IHttpSessionStore.cs` |
| `IHttpStoredSession` | `src/Abstractions/IHttpStoredSession.cs` |
| `InMemoryHttpSessionStore` | `src/InMemoryHttpSessionStore.cs` |

## Sources

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Sessions/src/Assimalign.Cohesion.Http.Sessions.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Sessions/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Http/README.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Sessions/src`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Sessions/src/Extensions/HttpContextSessionExtensions.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Sessions/src/HttpSession.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Sessions/src/Extensions/HttpSessionExtensions.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Sessions/src/HttpSessionOptions.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Sessions/src/HttpSessionSerializer.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Sessions/src/Extensions/HttpSessionStoreExtensions.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Sessions/src/Abstractions/IHttpSession.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Sessions/src/Abstractions/IHttpSessionFeature.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Sessions/src/Abstractions/IHttpSessionStore.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Sessions/src/Abstractions/IHttpStoredSession.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Sessions/src/InMemoryHttpSessionStore.cs`.
