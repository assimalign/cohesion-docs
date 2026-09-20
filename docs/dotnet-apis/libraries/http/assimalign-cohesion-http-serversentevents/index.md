# Assimalign.Cohesion.Http.ServerSentEvents

Formats Server-Sent Events and writes them through HTTP response streaming.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Http](../index.md)

## Scope

Event formatting writes UTF-8 into an `IBufferWriter<byte>` independently of a transport. Streaming
extension members bridge events and keep-alives onto `IHttpResponseStreamingFeature`. Applications
still own the long-lived response and event production.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Http`](../../http/assimalign-cohesion-http/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.Http.Streaming`](../../http/assimalign-cohesion-http-streaming/index.md) | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `ServerSentEvent` | `src/ServerSentEvent.cs` |
| `ServerSentEventFormatter` | `src/ServerSentEventFormatter.cs` |
| `ServerSentEventStreamingExtensions` | `src/Extensions/ServerSentEventStreamingExtensions.cs` |

## Sources

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ServerSentEvents/src/Assimalign.Cohesion.Http.ServerSentEvents.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ServerSentEvents/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ServerSentEvents/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Http/README.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ServerSentEvents/src`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ServerSentEvents/src/ServerSentEvent.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ServerSentEvents/src/ServerSentEventFormatter.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ServerSentEvents/src/Extensions/ServerSentEventStreamingExtensions.cs`.
