# Assimalign.Cohesion.Http.RequestLimits

Exposes per-request views of the request body-size cap and the minimum request-body and response
data rates.

> **Status:** Partial.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Http](../index.md)

## Scope

The package ships three typed features, `IHttpMaxRequestBodySizeFeature`,
`IHttpMinRequestBodyDataRateFeature` (#1383) and `IHttpMinResponseDataRateFeature` (#1384): views
over the request body-size cap and the minimum request-body and response data rates the server
transport enforces. Each writes through to the transport's limit state, so there is no second source
of truth.

`HttpRequestLimits.CreateInterceptor()` attaches all three on one object, on all three protocol
parse paths; `CreateMaxRequestBodySizeInterceptor()` attaches the body-size feature alone. The two
register under the same name, so a host registers one or the other. The Web host
(`Web.Hosting`) installs `CreateInterceptor()` by default, ahead of all user configuration.

- **Body-size cap.** Read the effective cap any time through `context.MaxRequestBodySize`; adjust
  it until the transport starts reading the body. All three protocols enforce the wire-level cap
  (`413`); HTTP/2 freezes it when the request is dispatched, so only head hooks can adjust it there.
- **Request-body rate.** An endpoint whose body legitimately idles (a client-streaming call) sets
  `context.MinRequestBodyDataRate!.MinDataRate = null` before it reads the body; every other request
  keeps the listener's rate. The transport freezes the rate at the first read of the body on every
  version, HTTP/2 included.
- **Response rate.** An endpoint whose reader pauses for long stretches sets
  `context.MinResponseDataRate!.MinDataRate = null` (or a lower rate) before its response starts.
  The transport freezes it at the response's first write.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Http`](../../http/assimalign-cohesion-http/index.md) | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `HttpRequestLimits` | `src/HttpRequestLimits.cs` |
| `IHttpMaxRequestBodySizeFeature` | `src/Abstractions/IHttpMaxRequestBodySizeFeature.cs` |
| `IHttpMinRequestBodyDataRateFeature` | `src/Abstractions/IHttpMinRequestBodyDataRateFeature.cs` |
| `IHttpMinResponseDataRateFeature` | `src/Abstractions/IHttpMinResponseDataRateFeature.cs` |
| `HttpRequestLimitsExtensions` | `src/Extensions/HttpRequestLimitsExtensions.cs` |

## Sources

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.RequestLimits/src/Assimalign.Cohesion.Http.RequestLimits.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.RequestLimits/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.RequestLimits/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Http/README.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.RequestLimits/src`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.RequestLimits/src/HttpRequestLimits.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.RequestLimits/src/Abstractions/IHttpMaxRequestBodySizeFeature.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.RequestLimits/src/Abstractions/IHttpMinRequestBodyDataRateFeature.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.RequestLimits/src/Abstractions/IHttpMinResponseDataRateFeature.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.RequestLimits/src/Extensions/HttpRequestLimitsExtensions.cs`.
