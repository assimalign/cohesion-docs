using System;
using System.Net.Http;
using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;

using Assimalign.Cohesion.Docs;
using Assimalign.Cohesion.Viu.Hosting;
using Assimalign.Cohesion.Viu.Hosting.Browser;
using Assimalign.Cohesion.Viu.Routing;
using Assimalign.Frontend;
using Assimalign.Viu.Components;
using Assimalign.Viu.Router;

ViuApplicationBuilder builder = ViuApplication.CreateBuilder();
using HttpClient httpClient = new() { BaseAddress = ResolveBaseAddress() };

builder.Services.AddComponents();
builder.Services.AddFrontendComponents();
builder.Services.AddRouting(routing => routing.Routes.Add(new RouteRecord(
    "/",
    "preview-landing-layout",
    children:
    [
        new RouteRecord(
            string.Empty,
            "preview-landing",
            component: new ComponentNode(ComponentReference.ForName("LandingPage"))),
    ],
    component: new ComponentNode(ComponentReference.ForName("AppShell")))));
builder.Services.AddMicroFrontends(microFrontends => microFrontends
    .UseShell(ComponentReference.ForName("AppShell"))
    .Add(new CohesionDocsMicroFrontend(httpClient)));
builder.AddClientConfiguration(httpClient);
builder.ConfigureApplication(options =>
    options.RootComponent = new ComponentNode(RouterView.Registration.Reference));
builder.ConfigureBrowser(options => options.MountTargetSelector = "#app");

await using ViuApplication application = builder.Build();
await application.UseRouter().RunAsync();

static Uri ResolveBaseAddress()
{
    JSObject? document = JSHost.GlobalThis.GetPropertyAsJSObject("document");
    if (document is null)
    {
        throw new InvalidOperationException("The browser document is unavailable.");
    }

    try
    {
        string? baseUri = document.GetPropertyAsString("baseURI");
        if (string.IsNullOrWhiteSpace(baseUri))
        {
            throw new InvalidOperationException("The browser document has no base URI.");
        }

        return new Uri(baseUri, UriKind.Absolute);
    }
    finally
    {
        document.Dispose();
    }
}
