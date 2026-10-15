# Antiforgery composition

The overview demonstrates antiforgery registration with a shared data-protection key ring.

> **Status:** Partial.

This example reproduces the overview's composition sample, including middleware placement and
endpoint conventions. Supply the application handlers referenced by the sample.

```csharp
using Assimalign.Cohesion.Http;
using Assimalign.Cohesion.Security.DataProtection;
using Assimalign.Cohesion.Web;
using Assimalign.Cohesion.Web.Antiforgery;
using Assimalign.Cohesion.Web.Hosting;
using Assimalign.Cohesion.Web.Routing;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// One key ring for every protected artifact; point every instance at the same repository.
IDataProtectionProvider dataProtection = DataProtectionProvider.Create(
    FileSystemKeyRepository.Create("/var/lib/app/keys"));

builder.Services.AddRouting();
builder.Services.AddAntiforgery(antiforgery => antiforgery.UseDataProtection(dataProtection));
// builder.Services.AddAntiforgery();   // development only: a per-process random key
// Token names and cookie attributes are antiforgery.Options; the cookie token is Secure on HTTPS.

WebApplication app = builder.Build();

app.UseRouting();
app.UseAntiforgery();          // after UseRouting

// The render path mints the pair: the cookie token is set on the response, and the request token
// goes into a hidden field (tokens.FormFieldName) or to a script that sends it in tokens.HeaderName.
app.MapGet("/contact", async context =>
{
    HttpAntiforgeryTokenSet tokens = context.RequireAntiforgery.GetAndStoreTokens(context);
    await RenderContactFormAsync(context, tokens);
});

// A typed form-bound endpoint requires antiforgery without a convention call.
app.MapPost("/contact", async ([FromForm] string email, IHttpContext context) => { /* ... */ });

// Raw endpoints and groups declare it; a route opts out of its group's requirement.
app.Map(HttpMethod.Post, "/settings", SaveSettingsAsync).RequireAntiforgery();
IRouterGroupBuilder account = app.MapGroup("/account").RequireAntiforgery();
account.Map(HttpMethod.Post, "webhook", ReceiveWebhookAsync).DisableAntiforgery();
```

[All examples](index.md) · [Assembly overview](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Antiforgery/docs/OVERVIEW.md`.
