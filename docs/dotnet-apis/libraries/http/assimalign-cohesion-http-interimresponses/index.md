# Assimalign.Cohesion.Http.InterimResponses

Sends interim HTTP responses through the exchange interceptor seam.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Http](../index.md)

## Scope

- `IHttpInterimResponseFeature` — the typed interim-response API
  (`IsInterimResponseSupported`, `SendInterimResponseAsync`).
- `HttpInterimResponseInterceptor.Create()` — the `IHttpExchangeInterceptor` a
  host registers to make interim responses available on every exchange.
- `context.InterimResponse` / `context.SendEarlyHintsAsync(...)` /
  `context.SendContinueAsync()` — ergonomic accessors on `IHttpContext`.

A host registers the interceptor to attach the feature to exchanges. Interim responses precede the
final response and use the core response seam, keeping the transport independent of this package.
Convenience members cover Continue and Early Hints.

The transport emits `100 Continue` automatically, on HTTP/1.1, HTTP/2 and HTTP/3 (#1385), for a
request that carries `Expect: 100-continue` and still has content to send, whether or not this
package is registered: a large-upload client withholds its body until it is solicited. This package
covers the application-initiated interim responses, `103 Early Hints` and an explicit,
handler-driven `100 Continue` (`SendContinueAsync`).

## Usage

```csharp
// Host / composition root — opt into interim responses:
options.Interceptors.Add(HttpInterimResponseInterceptor.Create());

// Handler — send 103 Early Hints before doing the slow work:
await context.SendEarlyHintsAsync(["</style.css>; rel=preload; as=style"]);
// ... compute the real response ...
context.Response.StatusCode = HttpStatusCode.Ok;
```

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Http`](../../http/assimalign-cohesion-http/index.md) | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `HttpInterimResponseInterceptor` | `src/HttpInterimResponseInterceptor.cs` |
| `IHttpInterimResponseFeature` | `src/Abstractions/IHttpInterimResponseFeature.cs` |
| `HttpInterimResponseExtensions` | `src/Extensions/HttpInterimResponseExtensions.cs` |

## Sources

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.InterimResponses/src/Assimalign.Cohesion.Http.InterimResponses.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.InterimResponses/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.InterimResponses/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Http/README.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.InterimResponses/src`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.InterimResponses/src/HttpInterimResponseInterceptor.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.InterimResponses/src/Abstractions/IHttpInterimResponseFeature.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.InterimResponses/src/Extensions/HttpInterimResponseExtensions.cs`.
