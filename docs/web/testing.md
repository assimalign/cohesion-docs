# Testing

Web test factories exercise manually composed pipelines or an enabled executable's real entry point.

> **Status:** Implemented. The factory supports HTTP/1 and prior-knowledge HTTP/2; HTTP/3 is outside its scope.

## Manual composition

`Assimalign.Cohesion.Web.Testing` provides `WebApplicationTestFactory` and its interfaces.
The manual factory uses in-memory connections and starts its server when the first client is
created. It supplies hosting and transport, leaving assertions and test-framework integration
to the caller.

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
configuring `factory.Application`. Factory disposal owns application shutdown.

## Real Program execution

`WebApplicationTestFactory.FromProgram<Program>()` invokes an enabled resource executable under an
invocation-local `ResourceContext`. It waits for that invocation's registered control plane and
uses the ambient loopback `http` endpoint. The returned `IWebApplicationProgramTestFactory` exposes
the invocation context.

This mode tests the same composition root used by the executable, including its resource
registration and lifecycle. It is distinct from constructing a second application solely inside
the test. `WebApplicationProgramTestFactoryOptions` configures context, arguments, and lifecycle
budgets; declare the executable's `Program` marker public and partial so the test project can name it.

## Protocol scope

| Mode | Supported transport |
|---|---|
| Default manual factory | Hypertext Transfer Protocol (HTTP) 1 over in-memory connections. |
| `WebApplicationTestProtocol.Http2` | Prior-knowledge HTTP/2 over the same in-memory connection pair. |
| Program-backed factory | The enabled executable's ambient loopback endpoint. |

HTTP/3 requires its separate real transport integration coverage. The factory's in-memory tests
do not establish QUIC platform availability or certificate behavior.

Return to [Web](index.md).

## Sources

- **Factory scope and examples** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Testing/docs/OVERVIEW.md`.
- **Lifecycle and isolation** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Testing/docs/DESIGN.md`.
- **Program marker contract** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Testing/src/WebApplicationTestFactory.cs`.
