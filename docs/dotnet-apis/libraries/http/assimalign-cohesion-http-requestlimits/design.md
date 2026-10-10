# Assimalign.Cohesion.Http.RequestLimits design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Http.RequestLimits`.

> **Status:** Partial.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

The feature is attached on all three protocol parse paths and writes through to the transport's
limit state. It becomes read-only when body reading starts. All three protocols enforce the
wire-level cap (`413`); HTTP/2 freezes it when the request is dispatched, so only head hooks can
adjust it there.

## Data rates are transport limits

The minimum data-rate limits (`MinRequestBodyDataRate` and `MinResponseDataRate`) landed with #810,
but as transport-owned limits (`HttpConnectionListenerLimits`), not features surfaced by this
package. `MinRequestBodyDataRate` is enforced on HTTP/1.1, HTTP/2 and HTTP/3 (#1085 added the last
two); `MinResponseDataRate` on the HTTP/1.1 streaming response only. Neither can be changed for one
request: the transport seeds only `MaxRequestBodySize` into the parse context. A typed per-request
body-rate feature here, over a value the transport seeds and freezes at the first body read as it
does the cap, is the intended remedy for bodies that legitimately idle. The
[`Assimalign.Cohesion.Http.Connections` design](../assimalign-cohesion-http-connections/design.md)
records that gap under its HTTP/2 and HTTP/3 timeouts and data rates.

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
