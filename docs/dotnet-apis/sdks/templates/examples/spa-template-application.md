# SPA template application

The shipped `cohesion-spa` template demonstrates the standalone single-page application entry point.

> **Status:** Implemented.

This is the `Program.cs` from `cohesion-spa`. Use the matching `Assimalign.Cohesion.Sdk.Web`
project from that template. Static files are served from `wwwroot`. GET and HEAD requests for
client-side routes such as `/settings` fall back to `wwwroot/index.html`. A missing asset whose
last path segment names a file, such as `/assets/missing.js`, still returns 404.

```csharp
using Assimalign.Cohesion.Web.Hosting;
using Assimalign.Cohesion.Web.Routing;
using Assimalign.Cohesion.Web.StaticFiles;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddRouting();
await using WebApplication application = builder.Build();

// Serves wwwroot: index.html for "/", plus every asset beside it with ETags, ranges and
// precompressed .br/.gz siblings.
application.UseStaticFiles();
application.UseRouting();

// Client-side routes serve index.html; missing paths naming a file still answer 404.
application.MapFallbackToFile("index.html");

await application.RunAsync();
```

[All examples](index.md) · [Package overview](../index.md)

## Sources

- **Primary source** — `cohesion/tooling/templates/Assimalign.Cohesion.Templates/src/content/cohesion-spa/Program.cs`.
- **Source** — `cohesion/tooling/templates/Assimalign.Cohesion.Templates/src/content/cohesion-spa/README.md`.
