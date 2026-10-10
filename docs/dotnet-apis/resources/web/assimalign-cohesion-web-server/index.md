# Assimalign.Cohesion.Web.Server

The contracts the Web server publishes on each exchange.

> **Status:** Partial.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.

The contracts the Web server publishes on each exchange. Contracts only: the default server in
`Assimalign.Cohesion.Web.Hosting` installs them before the pipeline runs, and middleware and
handlers read them from `context.Features`. The package has no tests of its own; the
[Web.Hosting examples](../assimalign-cohesion-web-hosting/examples/index.md) exercise the server
that installs the features.

## Scope

- **Request id** — `IWebRequestIdFeature.RequestId` is the request's W3C trace id: the server
  span's trace id when the server traces the request, otherwise the trace id of a valid
  `traceparent`, otherwise a random id generated on first read (#1064).
- **Response completion** — `IWebResponseCompletionFeature.Register` queues callbacks that run in
  order after the response is written to the transport. Registration after completion throws.
  `Web.Hosting.Resources` defers a control-plane stop until its `202` is written this way.
- **Drain signal** — `IWebServerDrainFeature.Draining` is cancelled when the server begins its
  lame-duck drain, so a long-lived exchange can end itself within the stop's budget. It cancels
  nothing. `Web.WebSockets` closes open sockets with `1001 Going Away` on it.
- **Client fault** — `IWebClientFaultFeature.StatusCode` is the `4xx` the transport answers the
  exchange with because the request body broke its framing or a configured limit while it was
  read, or the client cut it short by closing the connection (`400`, `413`, `408` or `431`), or
  `null`. The read still throws; this tells the code that sees the exception it was the client's
  fault, not an application defect (#1340). The default server installs it on an HTTP/1.1 request
  that declares a body. `Web.Diagnostics`, `Web.ErrorHandling` and `Web.Compression` read it.

A custom `IWebApplicationServer` may omit any of the four, so every reader handles an absent
feature.

## Namespace

The contracts declare `Assimalign.Cohesion.Web`, the namespace they shipped in when the Web root
held them. Moving them here (owner decision 33, #1379) changed their assembly, not their name, so
call sites compile unchanged.

## Dependencies

`Assimalign.Cohesion.Web` (the root, for `IWebApplicationServer` in the contracts' documentation)
and `Assimalign.Cohesion.Http` (`IHttpFeature`). No Hosting, DI or configuration reference.

## Usage

```csharp
// The 32-character lower-case trace id, or null under a server that does not publish it.
string? requestId = context.Features.Get<IWebRequestIdFeature>()?.RequestId.ToHexString();

// Defer work until the response has reached the transport.
context.Features.Get<IWebResponseCompletionFeature>()?.Register(() => ValueTask.CompletedTask);

// End a long-lived exchange cleanly when the server starts draining.
CancellationToken draining = context.Features.Get<IWebServerDrainFeature>()?.Draining ?? CancellationToken.None;

// After a body read threw: was it the client's fault, and which status does the transport send?
HttpStatusCode? clientFault = context.Features.Get<IWebClientFaultFeature>()?.StatusCode;
```

`Sdk.Web` applications get the package through the `App.Web` shared framework. A library that
reads one of these features references the package directly. Design detail:
[design](design.md).

## Principal public types

| Type | Member | Source file |
|---|---|---|
| `IWebRequestIdFeature` | `ActivityTraceId RequestId` | `src/Abstractions/IWebRequestIdFeature.cs` |
| `IWebResponseCompletionFeature` | `void Register(Func<ValueTask> callback)` | `src/Abstractions/IWebResponseCompletionFeature.cs` |
| `IWebServerDrainFeature` | `CancellationToken Draining` | `src/Abstractions/IWebServerDrainFeature.cs` |
| `IWebClientFaultFeature` | `HttpStatusCode? StatusCode` | `src/Abstractions/IWebClientFaultFeature.cs` |

Each contract extends `IHttpFeature`.

## Project references

| Reference | Kind |
|---|---|
| [`Assimalign.Cohesion.Web`](../assimalign-cohesion-web/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.Http`](../../../libraries/http/assimalign-cohesion-http/index.md) | `CohesionProjectReference` |

[Parent: Web](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Server/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Server/src/Assimalign.Cohesion.Web.Server.csproj`.
- **Contracts** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Server/src/Abstractions`.
- **Framework membership** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Runtime/Directory.Build.props`.
