# Assimalign.Cohesion.Web.Sessions

Per-request HTTP session support for the Cohesion Web pipeline.

> **Status:** Partial.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.

Per-request HTTP session support for the Cohesion Web pipeline. The package wires the async session
store seam from `Assimalign.Cohesion.Http.Sessions` into the Web middleware model: `UseSessions`
establishes a hardened session-id cookie, installs a store-backed session on the exchange lazily on
first access, and commits it after the pipeline unwinds. Concrete distributed backends are deferred
— the middleware runs unchanged over any `IHttpSessionStore`, defaulting to the in-process store.

## Scope

- **`UseSessions(...)`** — the pipeline verb. With no argument it uses an in-process
  `InMemoryHttpSessionStore`; a store overload accepts any `IHttpSessionStore` (a distributed adapter)
  so a multi-instance deployment gets sticky-session-free affinity with no other pipeline change.
- **Session-id cookie establishment** through the hardened `Http.Cookies` model — `HttpOnly` (from
  options), `SameSite=Lax`, `Secure` whenever the effective scheme is HTTPS (the transport-derived typed
  scheme, or a trusted proxy's forwarded scheme when `UseForwardedHeaders` runs first), session-scoped
  (no `Max-Age`/`Expires`). Cookie name/path/HttpOnly come from `HttpSessionOptions`.
- **Lazy lifecycle** — no store I/O and no cookie until the application first touches the session.
  `context.LoadSessionAsync()` materializes, establishes the cookie for a new id, and loads; the
  middleware commits (persist-if-modified, else slide) after `next`. An untouched session performs no
  I/O and mints no cookie.
- **Cryptographically random session ids** (128-bit, URL-safe base64url) plus
  **`context.RegenerateSessionIdAsync()`** — the post-authentication fixation defense (new id, same
  state, old id removed from the store, cookie replaced).
- **Sliding idle expiration** — renew-on-access, driven by `HttpSessionOptions.IdleTimeout` at the
  store level.

## Dependencies

- **`Assimalign.Cohesion.Web.Server`** — request-id correlation for orphan diagnostics.
- **`Assimalign.Cohesion.Web`** — the pipeline builder and middleware abstractions the verb and middleware build on.
- **`Assimalign.Cohesion.Http`** — the HTTP context, feature collection, and transport-derived scheme.
- **`Assimalign.Cohesion.Http.Forwarded`** — the `EffectiveScheme` read behind the `Secure` decision, which
  falls back to the transport-derived scheme when no forwarded-headers middleware ran.
- **`Assimalign.Cohesion.Http.Sessions`** — the `IHttpSession` surface, the `IHttpSessionStore` seam, the
  in-memory store, and the frame serializer this middleware composes.
- **`Assimalign.Cohesion.Http.Cookies`** — the hardened cookie model used to establish the session-id cookie.
- **`Assimalign.Cohesion.Http.Streaming`** — the response-streaming feature the establishment path checks
  before touching headers (head-committed guard).

It never references `Assimalign.Cohesion.Web.Hosting` (the resource hosting-isolation rule,
`COHRES001`).

## Usage

See the [source-backed usage examples](examples/index.md).

`For` an out-of-process deployment, pass a distributed store adapter — everything else is identical:

See the [source-backed usage examples](examples/index.md).

## Stored-session modification tracking

`IHttpStoredSession.IsModified` reports whether the built-in session needs a payload write.
It returns `true` by default for custom implementations that do not track changes, so existing
stores keep committing. Read-only built-in sessions still refresh their idle timeout.
Store failures can be observed through the `Assimalign.Cohesion.Http.Sessions` EventSource;
see the [design](design.md#store-failure-ownership-1436) for payload and ownership details.

Establish a new session before starting the response so its cookie can be delivered. If first
access occurs after the head starts, the session is discarded after the request. The
`Assimalign.Cohesion.Web.Sessions` EventSource reports a modified orphan with the throttled
Warning `SessionStateLost` and the exact `lost-sessions` counter; a read-only orphan emits only
Verbose `SessionOrphaned`. See [diagnostics](design.md#diagnostics) for event payloads and counters.

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Web` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Web.Server` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Http` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Http.Forwarded` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Http.Sessions` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Http.Cookies` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Http.Streaming` | `CohesionProjectReference` |

[Parent: Web](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Sessions/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Sessions/src/Assimalign.Cohesion.Web.Sessions.csproj`.
