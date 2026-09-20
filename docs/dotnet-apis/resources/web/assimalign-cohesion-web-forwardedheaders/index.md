# Assimalign.Cohesion.Web.ForwardedHeaders

Forwarded-headers resolution for Cohesion web applications: the first-position middleware that resolves the effective client address/scheme/host behind proxies under an explicit trust model, and the builder verb that composes it.

> **Status:** Partial.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.

Forwarded-headers resolution for Cohesion web applications: the first-position middleware that
resolves the effective client address/scheme/host behind proxies under an explicit trust model, and
the builder verb that composes it.

## Scope

- **`UseForwardedHeaders`** — the pipeline verb (on `IWebApplicationPipelineBuilder`)
  that validates and snapshots the trust model at composition time and registers the
  resolving middleware.
- **`ForwardedHeadersOptions`** — the trust model: `Headers`
  (`ForwardedHeaderNames` header selection), `KnownProxies` / `KnownNetworks` (CIDR),
  `ForwardLimit`, and `TrustLocalTransports` for non-IP (Unix domain socket /
  named-pipe / in-memory) peers.
- **The rightmost-first trust walk** — consumes the RFC 7239 `Forwarded` and
  `X-Forwarded-For/Proto/Host` parsing primitives from `Assimalign.Cohesion.Http`
  (never re-parses header text) and publishes the outcome as the
  `IHttpForwardedFeature` defined in `Assimalign.Cohesion.Http.Forwarded`.

## Dependencies

`Assimalign.Cohesion.Web` (pipeline seams) and `Assimalign.Cohesion.Http.Forwarded` (the output
contract), which brings the core `Assimalign.Cohesion.Http` primitives. No DI, configuration, or
logging — registration is dependency-free per the Web-area rules.

## Usage

See the [source-backed usage examples](examples/index.md).

`UseForwardedHeaders` must be the **first** middleware registered — see the ordering contract in
[DESIGN.md](design.md) . Downstream code reads the resolved identity through
`context.EffectiveScheme` / `context.EffectiveHost` / `context.EffectiveRemoteIp` /
`context.EffectiveRemoteEndPoint` (from `Assimalign.Cohesion.Http.Forwarded`) or the
`IHttpForwardedFeature` directly; the raw request headers and wire-level scheme/host/connection
surfaces are never mutated.

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Web` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Http.Forwarded` | `CohesionProjectReference` |

[Parent: Web](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.ForwardedHeaders/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.ForwardedHeaders/src/Assimalign.Cohesion.Web.ForwardedHeaders.csproj`.
