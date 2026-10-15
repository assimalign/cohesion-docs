# Assimalign.Cohesion.Http.ClientFactory design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Http.ClientFactory`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

Client disposal does not immediately dispose a shared handler. Expired handlers are retired after
references drain, allowing rotation to refresh network state. Named options configure headers,
timeouts, handlers, and the factory-owned redirect policy.

## Redirect policy is factory-owned (RFC 10008 § 2.5)

Automatic redirect following lives in the factory's own `RedirectHttpMessageHandler`
(a `DelegatingHandler` wrapped around the pooled inner handler), not in
`SocketsHttpHandler.AllowAutoRedirect` — which the factory always forces **off** so exactly one
layer acts.

**Why own it rather than delegate to the BCL:**

- **The method semantics are load-bearing.** RFC 10008 § 2.5 requires that a redirected QUERY
  be re-issued as a QUERY with its original content on `301`/`302`/`307`/`308` — never silently
  rewritten to GET — while § 2.5.3 sanctions exactly one method switch: `303 See Other` is
  fulfilled with a GET on the Location URI. Owning the layer makes those semantics an explicit,
  tested contract of this library rather than an implementation detail of whatever inner
  handler happens to be pooled.
- **Uniformity across inner handlers.** `HandlerFactory`-injected handlers (tests, custom
  stacks) get the same redirect behavior as the default `SocketsHttpHandler` path, and the
  policy is unit-testable with a scripted terminal handler — no wire, no sockets.
- **One switch.** `NamedHttpClientOptions.AllowAutoRedirect` / `MaxAutomaticRedirections` are
  the only redirect knobs. Configuring redirects through `ConfigureHandler` is documented as
  ineffective (the inner value is forced off after the callback runs) — the alternative, honoring
  the inner setting, would mean two layers that can silently double-follow.

**The rules the handler implements** (mirroring RFC 9110 § 15.4 and the BCL's safety posture):

| Status | Behavior |
|---|---|
| `301` / `302` | Re-issue with method + content preserved; the historical POST→GET rewrite is preserved for POST only |
| `303` | Re-issue as GET, content dropped (the one sanctioned switch — RFC 10008 § 2.5.3) |
| `307` / `308` | Re-issue with method + content preserved (the statuses forbid rewriting) |
| `300` | Never followed — choosing among alternatives is the caller's decision |

Plus: relative `Location` values resolve against the current request URI; an `https`→`http`
downgrade is never followed (the raw `3xx` surfaces); the `Authorization` field is dropped on
every hop; exceeding `MaxAutomaticRedirections` stops following and returns the last `3xx`;
intermediate responses are disposed.

**Accepted constraint:** a followed redirect re-serializes the same `HttpContent` instance, so
buffered contents (`ByteArrayContent`, `StringContent`, `JsonContent`) ride redirects while
one-shot contents (`StreamContent` over a non-seekable stream) cannot — the same constraint the
BCL's built-in redirect handling carries. The factory does not buffer request bodies on the
caller's behalf.

## Reference-free composition integration

This library still has no dependency on, or directly authored surface for, Cohesion dependency
injection. Its assembly-level `ComponentIntegrationAttribute` names the
`IServiceProviderBuilder` seam as a string and names the already-public
`HttpClientFactoryBuilder.Build()` instance method. No public components class exists solely for
integration, so the declaration contributes the verb with zero added public surface while the
source assembly continues to reference only its own HTTP family.

When a consuming compilation references both libraries, `ComponentIntegrationGenerator` from
`Assimalign.Cohesion.SourceGeneration` recognizes the instance method as a builder
template and projects `AddHttpClientFactory(Action<HttpClientFactoryBuilder>? configure = null)` into
that consumer. The callback is optional for every builder template (owner decision 39, #1395), but a
bare `AddHttpClientFactory()` still fails at registration: `Build()` rejects a factory with no named
client with `InvalidOperationException`, so the requirement stays where it was, in the builder.
The generated adapter eagerly constructs the builder, invokes the configuration action when one is given, calls
`Build()` so its validation runs at registration time, and registers the result through a producer
lambda so the container captures it for disposal. Without the dependency-injection assembly the
generator emits no verb, preserving the library boundary while still providing an ergonomic
composition path where both sides are intentionally present.

## Exception message resources (#1467)

Resource keys name the failure with a short PascalCase noun phrase, at most 40 characters
(#1471). These internal names are independent of the preserved message text and placeholders.

Exception diagnostics live in `src/Properties/ErrorMessages.resx`. The shared build targets
use MSBuild strongly typed resource generation to produce an internal `ErrorMessages` class
in this assembly's `RootNamespace`, under the intermediate output directory. No designer source
is checked in, and a command-line or NativeAOT build uses the same embedded resources.
Formatted diagnostics use `string.Format(CultureInfo.CurrentCulture, ErrorMessages.<Name>, ...)`.
The invariant messages, exception types, error codes and existing input-redaction rules stay
unchanged; this introduces no public API or breaking behavior against 10.0.0-preview.1.
EventSource templates, log messages and HTTP response bodies keep their existing owners.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Http`. The [overview](index.md#dependencies)
distinguishes project references, package references, shared source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ClientFactory/src/Assimalign.Cohesion.Http.ClientFactory.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ClientFactory/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ClientFactory/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Http/README.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ClientFactory/src`.
