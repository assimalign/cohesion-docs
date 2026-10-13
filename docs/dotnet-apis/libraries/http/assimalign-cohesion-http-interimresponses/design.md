# Assimalign.Cohesion.Http.InterimResponses design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Http.InterimResponses`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

A host registers the interceptor to attach the feature to exchanges. Interim responses precede the
final response and use the core response seam, keeping the transport independent of this package.
Convenience members cover Continue and Early Hints.

The automatic `Expect: 100-continue` handshake is not in scope here. That is a wire-level interop
concern the transport handles unconditionally on HTTP/1.1, HTTP/2 and HTTP/3 (it solicits the body
with `100 Continue` at its first read, so a withholding client does not deadlock; HTTP/2 and HTTP/3
since #1385), independent of this package. See the
[`Assimalign.Cohesion.Http.Connections` design](../assimalign-cohesion-http-connections/design.md).

## Contract and boundaries

- **`IsInterimResponseSupported`** reflects the exchange control's
  `CanWriteInterimResponse`: `true` while the final response has not started, then
  `false` once the head is committed (a streamed body started, or an HTTP/1.1
  connection was taken over). It is the report-don't-throw discoverability path —
  a caller checks it and never provokes an exception for an unsupported state.
- **`SendInterimResponseAsync`** forwards to the control's
  `WriteInterimResponseAsync`, which validates the status (`1xx`, not `101` — `101 Switching Protocols` belongs to
  `Http.ProtocolUpgrade`) and rejects an emission after the final response has
  started. The convenience `SendEarlyHintsAsync` / `SendContinueAsync` extensions
  no-op (return `false`) when the feature is absent or unsupported, so a handler
  can call them unconditionally.
- Interim responses carry no body; the transport writes no `Content-Length`. HTTP/2
  and HTTP/3 peers may receive several before the final response.

## Public interceptor and breaking changes since 10.0.0-preview.1 (#1465)

`HttpInterimResponseInterceptor` is the public sealed implementation of the existing
`IHttpExchangeInterceptor` contract through `HttpExchangeInterceptor`. Its constructor is private;
`HttpInterimResponseInterceptor.Create()` creates a new stateless instance and returns its concrete type.
The interface remains the contract for listener registration and custom interceptors. Feature
implementations remain internal. This is the factory-to-implementation convention approved by
owner decision 59.

The `v10.0.0-preview.1` tag shipped `HttpInterimResponses.CreateInterceptor()` returning
`IHttpExchangeInterceptor`. That static factory class is removed. Consumers must replace the call
with `HttpInterimResponseInterceptor.Create()` and rebuild. Removing the old type and method is a
**source- and binary-breaking change**, accepted during previews on decision 15's terms.
Assignments to `IHttpExchangeInterceptor` continue to work after the call is renamed. The
interceptor's registration order, scopes, feature installation, and request/response behavior
are unchanged.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Http`. The [overview](index.md#dependencies)
distinguishes project references, package references, shared source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.InterimResponses/src/Assimalign.Cohesion.Http.InterimResponses.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.InterimResponses/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.InterimResponses/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Http/README.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.InterimResponses/src`.
