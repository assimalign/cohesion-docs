# Assimalign.Cohesion.Http.Streaming

Incremental HTTP response streaming as an opt-in feature package that plugs into
the transport via the response-interceptor seam.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Http](../index.md)

## Purpose

Let a handler start a response and write its body incrementally — flushing bytes to
the client as they are produced — over any transport that exposes the
`IHttpExchangeInterceptor` seam, without the transport (or the protocol core)
depending on this package.

## Scope

- `IHttpResponseStreamingFeature` — the typed streaming API (`StartAsync`,
  `WriteAsync`, `FlushAsync`, `CompleteAsync`, `HasStarted`).
- `HttpResponseStreamingInterceptor.Create()` — the `IHttpExchangeInterceptor` a
  host registers to make streaming available on every exchange.
- `Response.Streaming` / `Response.SupportsStreaming` — ergonomic accessors on
  `IHttpResponse`.

## Usage

```csharp
// Host / composition root — opt into streaming:
options.Interceptors.Add(HttpResponseStreamingInterceptor.Create());

// Handler:
context.Response.Headers[HttpHeaderKey.ContentType] = "text/plain";
IHttpResponseStreamingFeature stream = context.Response.Streaming;
await stream.WriteAsync(chunk1);
await stream.FlushAsync();      // the client observes chunk1 now
await stream.WriteAsync(chunk2);
```

## Recovering a refused head

If a streaming call throws `HttpException` with `Code == HttpErrorCode.InvalidResponseField`,
the transport refused the head before writing bytes. `HasStarted` stays false. Replace the invalid
fields, then retry `StartAsync` or `CompleteAsync`, or write a buffered replacement. Completion
becomes final only after its flush succeeds.

When the transport supplies exchange control, `HasStarted` follows its existing commitment state;
a later I/O failure after commitment still reports true and cannot be answered with another head.
Hand-built interceptor contexts without control use a fallback that advances only after a sink
call succeeds. Response-starting callbacks remain once per exchange even when a refused head is
retried. See [Design](design.md) for the preview.1 behavior change and fallback limitation.

## Layering

L1 foundation library (HTTP feature layer). See [Design](design.md) for why streaming is a
separate package and how it plugs into the transport. Server-Sent Events
(`Assimalign.Cohesion.Http.ServerSentEvents`) builds on this package.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Http`](../../http/assimalign-cohesion-http/index.md) | `CohesionProjectReference` |

- `Assimalign.Cohesion.Http` (the response-interceptor seam + feature collection).
- **Not** `Assimalign.Cohesion.Http.Connections` — streaming is transport-agnostic.

## Principal public types

| Type | Source file |
|---|---|
| `HttpResponseStreamingInterceptor` | `src/HttpResponseStreamingInterceptor.cs` |
| `IHttpResponseStreamingFeature` | `src/Abstractions/IHttpResponseStreamingFeature.cs` |
| `HttpResponseStreamingExtensions` | `src/Extensions/HttpResponseStreamingExtensions.cs` |

## Sources

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Streaming/src/Assimalign.Cohesion.Http.Streaming.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Streaming/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Streaming/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Http/README.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Streaming/src`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Streaming/src/HttpResponseStreamingInterceptor.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Streaming/src/Abstractions/IHttpResponseStreamingFeature.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Streaming/src/Extensions/HttpResponseStreamingExtensions.cs`.
