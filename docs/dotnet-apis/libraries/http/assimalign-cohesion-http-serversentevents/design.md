# Assimalign.Cohesion.Http.ServerSentEvents design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Http.ServerSentEvents`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

Event formatting writes UTF-8 into an `IBufferWriter<byte>` independently of a transport. Streaming
extension members bridge events and keep-alives onto `IHttpResponseStreamingFeature`. Applications
still own the long-lived response and event production.

Transport integration tests consume duplex byte pipes through `IDuplexConnection` and HTTP
connections through `IHttpConnection`. Their listeners use `AcceptAsync`; HTTP/3 stream fixtures
use `IMultiplexedConnectionStream`. These contract names do not alter this package's
request/response behavior.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Http`, `Assimalign.Cohesion.Http.Streaming`. The
[overview](index.md#dependencies) distinguishes project references, package references, shared
source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ServerSentEvents/src/Assimalign.Cohesion.Http.ServerSentEvents.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ServerSentEvents/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ServerSentEvents/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Http/README.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ServerSentEvents/src`.
