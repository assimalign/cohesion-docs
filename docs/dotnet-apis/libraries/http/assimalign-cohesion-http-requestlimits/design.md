# Assimalign.Cohesion.Http.RequestLimits design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Http.RequestLimits`.

> **Status:** Partial.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

The package owns three per-request limit features: the typed max-request-body-size feature, the
typed minimum request-body data-rate feature (#1383), and the typed minimum response data-rate
feature (#1384). It owns the feature contracts and their implementation and references only core
`Assimalign.Cohesion.Http`; the transport never references this package. The enforcement itself
stays transport-owned in `Http.Connections` (`HttpConnectionListenerLimits`), because the security
guarantee must hold with zero optional packages installed.

The body-size feature is attached on all three protocol parse paths and writes through to the
transport's limit state. It becomes read-only when body reading starts. All three protocols enforce
the wire-level cap (`413`); HTTP/2 freezes it when the request is dispatched, so only head hooks can
adjust it there.

## The minimum request-body data rate

`IHttpMinRequestBodyDataRateFeature` (#1383) is the same kind of view over the parse context's
`MinRequestBodyDataRate`: `MinDataRate` (an `HttpMinDataRate`, `null` = no rate) reads and writes
the knob the transport enforces, and `IsReadOnly` delegates to `IsMinRequestBodyDataRateReadOnly`.
Every transport seeds the knob from its listener's `MinRequestBodyDataRate` and freezes it at the
first read of the request body, on HTTP/2 too, where the body-size knob froze at dispatch. An
endpoint whose request body legitimately idles (a client-streaming or duplex call, a telemetry
upload that sends a message now and then) sets it to `null` before it reads; an endpoint that needs
a stricter rate raises it. Every other request keeps the listener's rate, so the slow-body defence
stays on for them. Kestrel's `IHttpMinRequestBodyDataRateFeature`, which gRPC disables on streaming
calls, is the model.

## The minimum response data rate

`IHttpMinResponseDataRateFeature` (#1384) views the context's `MinResponseDataRate`, the slow-read
defence's knob: a peer that withholds flow-control credit or reads below it is reclaimed and the
exchange cancelled. The transport freezes it at the response's first write, independently of the
request-body knobs, so an endpoint whose reader legitimately pauses for long stretches sets it to
`null` (or a lower rate) before it starts its response. Kestrel's `IHttpMinResponseDataRateFeature`
is the model.

## One object for every limit

`HttpRequestLimitsInterceptor.Create()` attaches every contract on **one** object
(`HttpRequestLimitsFeature`): the feature collection is keyed by name, and a typed lookup returns
the first feature implementing the contract asked for, so `Get<IHttpMaxRequestBodySizeFeature>()`,
`Get<IHttpMinRequestBodyDataRateFeature>()` and `Get<IHttpMinResponseDataRateFeature>()` all find
it. An exchange pays one allocation and one slot for every limit, as it did for the body-size
feature alone, so the web host's presized feature collection (#1381) does not grow.
`HttpMaxRequestBodySizeInterceptor.Create()` still attaches the body-size feature alone; the two register
under the same name (`"Assimalign.Cohesion.Http.MaxRequestBodySize"`, the value the transport
historically used), so a host registers one or the other. `Web.Hosting` registers
`HttpRequestLimitsInterceptor.Create()` ahead of all user configuration, so it holds interceptor slot 0.

## Data rates are transport limits

The minimum data-rate limits (`MinRequestBodyDataRate` and `MinResponseDataRate`) landed with #810
as transport-owned limits (`HttpConnectionListenerLimits`). `MinRequestBodyDataRate` is enforced
on HTTP/1.1, HTTP/2 and HTTP/3 (#1085 added the last two); `MinResponseDataRate` on every response
since #1384, which ended the HTTP/1.1-streaming-only coverage. Since #1383 the transport seeds the
request-body rate into the parse context too and freezes it at the first body read on all three
versions, and `IHttpMinRequestBodyDataRateFeature` is the typed view over it, with the
middleware-visible pre-read window on HTTP/2 as well. #1384 does the same for the response rate,
frozen at the response's first write. The
[`Assimalign.Cohesion.Http.Connections` design](../assimalign-cohesion-http-connections/design.md)
records how each transport enforces the rates.

## Public interceptors and breaking changes since 10.0.0-preview.1 (#1465)

`HttpRequestLimitsInterceptor` and `HttpMaxRequestBodySizeInterceptor` are public sealed
implementations of the existing `IHttpExchangeInterceptor` contract through
`HttpExchangeInterceptor`. Each has a private constructor and its own `Create()` returning its
concrete type. The interface remains the listener registration contract; the per-exchange feature
implementations remain internal. This is the factory-to-implementation convention approved by
owner decision 59. Register one interceptor or the other, first in the listener's list.

The `v10.0.0-preview.1` tag shipped `HttpRequestLimits.CreateMaxRequestBodySizeInterceptor()`;
consumers replace it with `HttpMaxRequestBodySizeInterceptor.Create()` and rebuild. Removing the
old static class and factory is a **source- and binary-breaking change**, accepted during previews
on decision 15's terms. The combined `HttpRequestLimits.CreateInterceptor()` was added after that
tag by #1383/#1384; its replacement, `HttpRequestLimitsInterceptor.Create()`, is likewise source-
and binary-breaking for consumers of the intervening branch. Both new factories remain assignable
to `IHttpExchangeInterceptor`. Their scope, registration order, limits, freeze rules, and feature
identity are unchanged.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Http`. The [overview](index.md#dependencies)
distinguishes project references, package references, shared source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.RequestLimits/src/Assimalign.Cohesion.Http.RequestLimits.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.RequestLimits/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.RequestLimits/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Http/README.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.RequestLimits/src`.
