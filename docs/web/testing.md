# Testing

Web test factories exercise composed pipelines over in-memory connections.

> **Status:** Implemented. The factory supports HTTP/1 and prior-knowledge HTTP/2; HTTP/3 is outside its scope.

## Manual composition

`Assimalign.Cohesion.Web.Testing` provides `WebApplicationTestFactory` and its interfaces.
The manual factory uses in-memory connections and starts its server when the first client is
created. It supplies hosting and transport, leaving assertions and test-framework integration
to the caller.

Its socketless listener implements `IDuplexConnectionListener`, and the client uses
`IDuplexConnection` for the input/output pipe. Identity and lifetime come from the pipe-less
`IConnection` root. HTTP listeners accept through `AcceptAsync` and retain accepted connections
in their live `Connections` collection until `ConnectionClosed` fires.

This complete program follows the package's manual-factory example:

```csharp
using System;
using System.Net.Http;
using System.Text;

using Assimalign.Cohesion.Http;
using Assimalign.Cohesion.Web.Testing;

await using WebApplicationTestFactory factory = new();

factory.Application.Use(async (context, next) =>
{
    context.Response.StatusCode = HttpStatusCode.Ok;
    await context.Response.Body.WriteAsync(
        Encoding.UTF8.GetBytes("hello"), context.RequestCancelled);
});

using HttpClient client = factory.CreateClient();
string payload = await client.GetStringAsync("/");
Console.WriteLine(payload);
```

Perform builder-time feature registration through `factory.Builder` before obtaining and
configuring `factory.Application`. Factory disposal owns application shutdown: it stops the server
with a shutdown budget that has already run out, so requests still in flight are cancelled rather
than waited for. A test whose requests must finish calls `StopAsync` first, which lets them finish
within the token it is given and, with none, waits for them (see
[Graceful shutdown](server.md#graceful-shutdown)).

To test static files, point the application at a content root on disk. Its `wwwroot` becomes the
web root that `UseStaticFiles()` serves; the default content root is the test assembly's base
directory:

```csharp
await using WebApplicationTestFactory factory = new(new WebApplicationTestFactoryOptions
{
    ContentRootPath = FileSystemPath.Parse(contentRootDirectory),
});
```

## Managed resources in tests

The factory composes a plain Web application; it hosts no resource. To test a program as a managed
resource, run it under an in-process gateway: `InProcessInfrastructure` from
[`Gateway.InProcess`](../dotnet-apis/libraries/application-model/assimalign-cohesion-applicationmodel-gateway-inprocess/index.md)
calls the program's entry point on its own thread and environment, and the gateway's
`ExecuteAsync` runs its actions. The
[Web resource tests](../dotnet-apis/resources/web/assimalign-cohesion-web-hosting-applicationmodel/examples/index.md)
use that pattern.

## Protocol scope

| Mode | Supported transport |
|---|---|
| Default manual factory | Hypertext Transfer Protocol (HTTP) 1 over in-memory connections. |
| `WebApplicationTestProtocol.Http2` | Prior-knowledge HTTP/2 over the same in-memory connection pair. |

HTTP/3 requires its separate real transport integration coverage. The factory's in-memory tests
do not establish QUIC platform availability or certificate behavior.

## Diagnostic coverage

The source repository's event tests link the shared test-only recorder from
`build/TestSupport/EventSourceRecorder.cs`. It records one source's events separately from
`EventCounters` payloads and can await a fresh counter sample; it is not a shipped Web.Testing API.
Because event listeners observe the process, event suites isolate their listeners and use the
server's request ids and `serverId` to correlate concurrent work.

`WebEventSourceConformanceTests` discovers every public and private `App.Web` assembly, checks
that each has at most one internal sealed source named for its assembly, validates strict
manifests, and compares the Web sources with the event-source catalog and deliberate no-source
list. The Web.Server tests cover direct engine use, dispatch, drain, exchange features and server
diagnostics. Web.Hosting tests cover the host-service wrapper, HTTP composition, dependency
injection, and default forwarding. NativeAOT
delivery is exercised separately by the hosting AOT guard; a normal in-memory factory test does
not establish that a published binary retains event support.

`Sdk.Web` enables `EventSourceSupport` when unset. An explicit
`<EventSourceSupport>false</EventSourceSupport>` disables delivery, including to an in-process
listener. See [Observability](observability.md) for source selection and forwarding behavior.

Return to [Web](index.md).

## Sources

- **Factory scope and examples** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Testing/docs/OVERVIEW.md`.
- **Lifecycle and isolation** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Testing/docs/DESIGN.md`.
- **Factory disposal** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Testing/src/WebApplicationTestFactory.cs`.
- **In-process resource tests** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Hosting.ApplicationModel/tests/WebResourceInProcessTests.cs`.
- **Diagnostic recorder and conformance** — `cohesion/build/TestSupport/EventSourceRecorder.cs` and `cohesion/libraries/App/Assimalign.Cohesion.App.Runtime/tests/WebEventSourceConformanceTests.cs`.
- **Server engine and host service** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Server/tests/WebApplicationServerDirectUseTests.cs` and `cohesion/resources/Web/Assimalign.Cohesion.Web.Hosting/tests/WebApplicationServerHostServiceTests.cs`.
- **Server diagnostics and AOT delivery** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Server/tests/WebServerEventSourceAcceptanceTests.cs`, `cohesion/resources/Web/Assimalign.Cohesion.Web.Hosting/tests/WebDefaultEventSourceForwardingTests.cs` and `cohesion/resources/Web/Assimalign.Cohesion.Web.Hosting/samples/Assimalign.Cohesion.Web.AotGuard/Program.cs`.
