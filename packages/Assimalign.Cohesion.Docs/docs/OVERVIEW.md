# Assimalign.Cohesion.Docs

`Assimalign.Cohesion.Docs` is the host-neutral Viu micro-frontend for the Cohesion documentation.
It compiles the repository's `docs/**/*.md` tree into an immutable catalog and carries the Markdown
as package static content so a browser host fetches pages only when they are opened.

The module type is `Assimalign.Cohesion.Docs.CohesionDocsMicroFrontend`. It contributes Cohesion
branding, navigation, breadcrumbs, previous/next links, and routes rooted at `/cohesion`. It owns its
catalog, HTTP content source, and renderer; a consuming host must not call `AddMarkdownContent`.

## Consuming the package

Reference `Assimalign.Cohesion.Docs` version `1.0.0-preview.2`, create the module with the browser
host's base-addressed `HttpClient`, and add it in the application's services:

```csharp
builder.Services.AddFrontendComponents();
builder.Services.AddMicroFrontends(microFrontends => microFrontends
    .UseShell(ComponentReference.ForName("AppShell"))
    .Add(new CohesionDocsMicroFrontend(httpClient)));

await using ViuApplication application = builder.Build();
await application.UseRouter().RunAsync();
```

`AddMicroFrontends` contributes the module's components and routes and registers the router; the
host adds any landing route with `builder.Services.AddRouting`. The host document links the shared
components utility stylesheet and imports the shared frontend JavaScript helper.

Markdown assets are available at
`/_content/Assimalign.Cohesion.Docs/docs/<repository-relative-path>`. Package consumers receive the
files through the generated transitive content registration.
