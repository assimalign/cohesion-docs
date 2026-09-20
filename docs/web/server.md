# Server and TLS

The Web host composes listeners and drives each accepted connection through the application pipeline.

> **Status:** Implemented. HTTP/3 requires a supported QUIC platform; configuration-based listener binding remains opt-in.

## Host and server ownership

`WebApplication` is a `Host<WebApplicationContext>`. The concrete builder materializes explicit
`AddService` factories once at `Build()`, preserves their order, and starts those services before
any server. Reverse shutdown drains servers before stopping application services.

`WebApplicationServer` binds its listeners before reporting successful startup. A bind failure
becomes a `HostStartupException` retaining the transport cause. Accepted connections have separate
tracked tasks; middleware faults are contained rather than escaping the accept loop.

The server owns accepted connections, exchanges, and contexts. It tracks in-flight work for
shutdown. A protocol-aware two-phase drain that lets a keep-alive connection finish its current
exchange while refusing the next remains outside the documented server iteration.

## Application configuration

`WebApplication.CreateBuilder(args)` loads configuration in increasing precedence:

1. Optional `appsettings.json`.
2. Optional `appsettings.{Environment}.json`.
3. Deployment settings, including `COHESION_CONFIG__` variables or their ambient equivalents.
4. Command-line arguments.

Enabled resources resolve files from `ResourceContext.ContentRootPath`; ordinary applications
use `AppContext.BaseDirectory`. The variable prefix is removed, and double underscores become
configuration path separators. In-process settings arrive through the ambient resource context
without modifying process-wide environment variables.

`Local` selects `appsettings.Local.json`; `Development` selects `appsettings.Development.json`.
They are distinct environments. The default remains `Production`.

## Listeners and certificates

Transport Layer Security (TLS) is composed beneath Hypertext Transfer Protocol (HTTP).
`HttpConnectionListenerOptions.UseHttp1s` and `UseHttp2s` accept a transport configuration callback
and `TlsServerOptions`. They compose the secure transport before registering the HTTP listener.
The connection's security capability determines the request's HTTPS scheme.

| Registration | Transport and certificate contract |
|---|---|
| `UseHttp1` / `UseHttp2` | Plain stream listeners. |
| `UseHttp1s` / `UseHttp2s` | Secure stream listeners using explicit `TlsServerOptions`. |
| `UseHttp3` | QUIC listener with mandatory TLS and a caller-provided certificate configuration. |

Application-Layer Protocol Negotiation (ALPN) defaults to `h2` for secure HTTP/2 and `h3` for
HTTP/3 when the caller leaves it unset. HTTP/3 defaults to TLS 1.3. QUIC registration defers
binding until the asynchronous listener lifecycle; the platform must provide a usable QUIC stack.

In an enabled resource, endpoint metadata names an ordinary Secret mount, defaulting to `tls`.
The delivered PEM document contains a leaf certificate, exactly one private key, and optional
issuer chain. Empty content means absent; malformed or multi-key bundles fail. Gateway-materialized
trust anchors travel separately from this certificate mount.

## Configuration-bound endpoints

`builder.Server.UseConfiguration(configuration)` explicitly binds the `Http` section's endpoints
and server limits. It is not wired by default. The binder is explicit and reflection-free.

Endpoint `Protocol` values include `Http1`, `Http2`, `Https`, `Http1s`, and `Http2s`.
`Certificate` names a Secret mount for a secure endpoint. HTTP/3 registration uses its separate
programmatic surface. Invalid configuration fails instead of silently selecting a transport.

## Enabled-resource management

The host discovers its entry assembly's generated `ResourceRuntime` registration. Ambient
binding tries endpoint name `http`, then `https`, and accepts the corresponding URI schemes.
The shared `Web.Hosting.Resources` terminal authenticates namespaced management requests.

Graceful stop returns HTTP 202 before invoking the shutdown signal through a response-completion
hook. This lets the acknowledgement reach the caller before server cancellation begins.

Return to [Web](index.md).

## Sources

- **Runtime** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Hosting/docs/OVERVIEW.md` and `cohesion/resources/Web/Assimalign.Cohesion.Web.Hosting/docs/DESIGN.md`.
- **Control-plane ownership** — `cohesion/docs/resources/Web/DESIGN.md`.
- **Certificate carrier** — `cohesion/docs/RUNTIME_CONTRACT.md`.
