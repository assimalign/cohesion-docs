# Template application

The shipped `cohesion-web` template demonstrates the Web executable entry point.

> **Status:** Partial.

This is the `Program.cs` from `cohesion-web`. `Use` the matching `Assimalign.Cohesion.Sdk.Web`
project from that template.

```csharp
using System.Text;
using Assimalign.Cohesion.Http;
using Assimalign.Cohesion.Web.Hosting;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
await using WebApplication application = builder.Build();

application.Use(async (context, next) =>
{
    if (context.Request.Path.Value != "/")
    {
        await next.Invoke(context).ConfigureAwait(false);
        return;
    }

    context.Response.StatusCode = HttpStatusCode.Ok;
    byte[] payload = Encoding.UTF8.GetBytes("Hello from CohesionProject");
    await context.Response.Body.WriteAsync(payload, context.RequestCancelled).ConfigureAwait(false);
});

await application.RunAsync();
```

[All examples](index.md) · [Assembly overview](../index.md)

## Sources

- **Primary source** — `cohesion/tooling/templates/Assimalign.Cohesion.Templates/src/content/cohesion-web/Program.cs`.
