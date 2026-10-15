# Assimalign.Cohesion.Http.ClientFactory

Creates named HTTP clients while pooling and rotating their message handlers.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Http](../index.md)

## Scope

Client disposal does not immediately dispose a shared handler. Expired handlers are retired after
references drain, allowing rotation to refresh network state. Named options configure headers,
timeouts, handlers, and the factory-owned redirect policy.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Http`](../../http/assimalign-cohesion-http/index.md) | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `HttpClientFactoryBuilder` | `src/HttpClientFactoryBuilder.cs` |
| `IHttpClientFactory` | `src/Abstractions/IHttpClientFactory.cs` |
| `NamedHttpClientOptions` | `src/NamedHttpClientOptions.cs` |
| `HttpClientFactoryOptions` | `src/HttpClientFactoryOptions.cs` |

## Sources

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ClientFactory/src/Assimalign.Cohesion.Http.ClientFactory.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ClientFactory/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ClientFactory/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Http/README.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ClientFactory/src`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ClientFactory/src/HttpClientFactoryBuilder.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ClientFactory/src/Abstractions/IHttpClientFactory.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ClientFactory/src/NamedHttpClientOptions.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ClientFactory/src/HttpClientFactoryOptions.cs`.
