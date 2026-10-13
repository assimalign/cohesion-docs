# Assimalign.Cohesion.Http.ServerSentEvents design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Http.ServerSentEvents`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

Event formatting writes UTF-8 into an `IBufferWriter<byte>` independently of a transport. Streaming
extension members bridge events and keep-alives onto `IHttpResponseStreamingFeature`. Applications
still own the long-lived response and event production.

`EventType`, `Id`, and `Comment` MUST be single-line (the wire format terminates a
field at the first newline); only `Data` may contain line breaks. This is a
documented caller contract, not enforced, to keep the formatter allocation-light.

Transport integration tests consume duplex byte pipes through `IDuplexConnection` and HTTP
connections through `IHttpConnection`. Their listeners use `AcceptAsync`; HTTP/3 stream fixtures
use `IMultiplexedConnectionStream`. These contract names do not alter this package's
request/response behavior.

## The bridge onto the streaming seam

`ServerSentEventStreamingExtensions` adds `WriteEventAsync` and
`WriteKeepAliveAsync` to `IHttpResponseStreamingFeature` (from
`Assimalign.Cohesion.Http.Streaming`). Each serializes the event and does
`WriteAsync` + `FlushAsync` so the peer observes it immediately. This is where SSE
meets the streaming write path — no transport dependency, just the feature contract.
A host enables streaming by registering `HttpResponseStreamingInterceptor.Create()`
on the transport; a handler then resolves the feature via `context.Response.Streaming`.

### Header-commit and content type

Headers lock once a streaming response starts (the streaming feature's contract),
so the caller sets `Content-Type: text/event-stream`
(`ServerSentEvent.MediaType`) — and any other SSE conventions such as
`Cache-Control: no-cache` — **before** the first `WriteEventAsync`. The
`Last-Event-ID` request header a reconnecting client replays is
`HttpHeaderKey.LastEventId` in the core registry (alongside the other well-known
header-name constants); a handler reads it to resume a stream from the last
delivered id.

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
