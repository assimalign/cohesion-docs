# OpenAPI

Web.OpenApi serves an OpenAPI description of a Web application, generated from the metadata its endpoints already carry rather than from runtime reflection.

> **Status:** Implemented. One document per application; OAuth2 flows, examples, and `x-` extensions need a document transformer.

`Assimalign.Cohesion.Web.OpenApi` reads the application's route table and describes each endpoint
from metadata produced at build time: the parameter and response descriptions the endpoint-binding
generator attaches to typed endpoints, the description verbs, and the System.Text.Json contracts the
application registered. Nothing is discovered by reflection, so the document is the same under
NativeAOT. The document can target OpenAPI 3.0, 3.1 or 3.2 and is served as JSON or YAML. The
reference pages are
[Web.OpenApi](../dotnet-apis/resources/web/assimalign-cohesion-web-openapi/index.md) and its
[design](../dotnet-apis/resources/web/assimalign-cohesion-web-openapi/design.md).

## Add the package

Web.OpenApi is a NuGet package, not a member of the `App.Web` shared framework: an application that
does not document its API carries none of the OpenApi family. Reference it from the application
project at the Cohesion version the SDK uses:

```xml
<ItemGroup>
  <PackageReference Include="Assimalign.Cohesion.Web.OpenApi" Version="$(CohesionVersion)" />
</ItemGroup>
```

The description verbs (`WithTags`, `WithSummary`, `WithDescription`, `ExcludeFromDescription`) and the
endpoint descriptions live in `Web.Api`, which is a framework member, so a library that maps
endpoints can describe them without referencing this package.

## Serve the document

`builder.Services.AddOpenApi` registers the document's options, and `MapOpenApi` maps a `GET` route
that serves it. Every type a typed endpoint reads or returns must be in the `JsonSerializerContext`
passed to `AddJsonSerialization`, as it must for the endpoint to serialize it.

```csharp
using System.Text.Json.Serialization;

using Assimalign.Cohesion.OpenApi;
using Assimalign.Cohesion.Web;
using Assimalign.Cohesion.Web.Hosting;
using Assimalign.Cohesion.Web.OpenApi;
using Assimalign.Cohesion.Web.Routing;
using Assimalign.Cohesion.Web.Serialization;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddRouting();
builder.Services.AddJsonSerialization(AppJsonContext.Default);
builder.Services.AddOpenApi(options =>
{
    options.Title = "Orders API";
    options.ApiVersion = "1.0.0";
});

await using WebApplication app = builder.Build();
app.UseRouting();

IRouterGroupBuilder orders = app.MapGroup("orders").WithTags("orders");

orders.MapGet("{id:long}", (long id) => new Order(id, "pending"))
    .WithName("getOrder")
    .WithSummary("Gets an order");

orders.MapPost("", (CreateOrder order) => new Order(1, order.Item))
    .WithName("createOrder")
    .WithDescription("Creates an order from the posted item.");

app.MapGet("/internal/cache", () => "cleared").ExcludeFromDescription();

app.MapOpenApi();                                                 // GET /openapi/v1.json, OpenAPI 3.1
app.MapOpenApi("/openapi/v1.yaml");                               // the same document as YAML
app.MapOpenApi("/openapi/v1-3.0.json", OpenApiSpecVersion.V3_0);  // a 3.0 rendition

await app.RunAsync();

internal sealed record Order(long Id, string Status);

internal sealed record CreateOrder(string Item);

[JsonSerializable(typeof(Order))]
[JsonSerializable(typeof(CreateOrder))]
internal sealed partial class AppJsonContext : JsonSerializerContext
{
}
```

- **The route** — `MapOpenApi()` serves `/openapi/v1.json`. A pattern ending in `.yaml` or `.yml`
  serves YAML as `application/yaml`; any other pattern serves JSON as
  `application/json; charset=utf-8`. Requests reach the route through `UseRouting`, like every
  route, and the route itself is left out of the document.
- **The line** — `OpenApiOptions.SpecVersion` (3.1 by default), or the line
  `MapOpenApi(pattern, specVersion)` names, so a 3.0 rendition can sit beside the default for tools
  that do not read 3.1.
- **Built once** — the document is built and serialized on the route's first request, after the
  route table is closed, and those bytes are served from then on with a strong `ETag`, the SHA-256 of
  the bytes. A matching `If-None-Match` is answered `304` with no body, and `HEAD` gets the `GET`
  header section.
- **Policies** — `MapOpenApi` returns the route's `IRouterRouteBuilder`, so the document route can
  carry `RequireAuthorization` or `RequireCors` like any other.
- **Options** — `Title` (the running application's name by default), `ApiVersion` (`1.0.0` by
  default), `Description`, and `SpecVersion`. The options are read-only once the `AddOpenApi`
  callback returns.

A type a typed endpoint reads or returns that no registered reader or writer covers fails the
document request with an `InvalidOperationException` that names the endpoint (`GET /orders/{id}`)
and the type and says to add it to the application's `JsonSerializerContext`. The exception reaches
the pipeline's exception boundary, and the next request tries again: a failure that does not recur,
such as a document transformer that throws once, does not leave the route failing until a restart.
A composition error recurs on every attempt until the application is fixed.

## What the document describes

Typed endpoints are described automatically, because the generator records each one's request
inputs and responses as route metadata. A raw middleware endpoint
(`MapGet(pattern, WebApplicationMiddleware)`) is described only when the application describes it
with one of the verbs below; nothing records its inputs or outputs otherwise.

| Element | Comes from |
|---|---|
| Path | The route template, each parameter as `{name}`, constraints and defaults dropped |
| `operationId` | The route name (`WithName`), suffixed `_{method}` when a named route maps several methods |
| `summary`, `description`, `tags` | `WithSummary`, `WithDescription` and `WithTags`; the document's tags are the declared ones followed by the names endpoints use |
| Path, query and header parameters | The handler's bound parameters; a path parameter's schema comes from its declared type or its route constraint |
| Request body | A body parameter, under the media type of the first registered reader that can read it, with the schema of its JSON contract |
| Form body | Form fields and files: `multipart/form-data` when the endpoint uploads a file, otherwise `application/x-www-form-urlencoded` |
| Responses | A `200` with the returned type's schema (`text/plain` for a string), a `204` when the result may be `null`, and any `EndpointResponseMetadata` the application adds |
| Binding outcomes | `400` problem+json when the endpoint binds any input, `413` when it reads a form field or file, `415` when it reads a body, a bodyless `406` when it writes a negotiated value |
| Security | Each endpoint's effective authorization policy; see "Security requirements" |

Schemas come from the very contracts the JSON writer serializes with, through
`TryGetJsonTypeInfo` and System.Text.Json's `JsonSchemaExporter`: property names, nullability,
required members, enum converters and number handling are exactly what goes on the wire. Object
types become named components (`Page<Order>` as `PageOfOrder`, `Order[]` as `OrderArray`), a
recursive type references its own component, and the RFC 9457 `ProblemDetails` type maps to a fixed
component with the five standard members.

`QUERY` endpoints become the 3.2 `query` operation and are left out of earlier lines. A route that
accepts any method, and `CONNECT` and extension methods, have no operation to describe.

### Uploaded files

An endpoint that binds an uploaded file is described with a `multipart/form-data` body, its form
fields as sibling parts. A file part is `type: string` with format `binary` on every line. An
`IHttpFormFile` is one part, required unless the parameter is nullable; a file sequence
(`IHttpFormFile[]`, `IReadOnlyList<IHttpFormFile>`, …) is an optional array of parts under one field
name; and an `IHttpFormFileCollection` is an optional array part named for the handler parameter.
The endpoint also lists `413`, the answer to a form over an Http.Forms limit.

## Describe endpoints

The description verbs work on a route and on a group alike:

| Verb | Effect | Composition |
|---|---|---|
| `WithTags(params string[])` | Operation tags | Every item applies, outer group first; each name listed once |
| `WithSummary(string)` | Operation summary | The most specific wins: a route's replaces its group's |
| `WithDescription(string)` | Operation description | The most specific wins |
| `ExcludeFromDescription()` | Leaves the endpoint out | Present anywhere, the endpoint is not described |

A response the handler answers on its own is described with
`.WithMetadata(new EndpointResponseMetadata(HttpStatusCode.NotFound))` on the endpoint or its group;
it composes with the generated ones, and for a status described twice the endpoint's own item wins.
XML comments are not read: they are not in the compiled metadata, so summaries and descriptions are
carried explicitly.

## Security requirements

Declare each authentication scheme an endpoint's authorization uses as a security scheme under the
authentication scheme's own name:

```csharp
using Assimalign.Cohesion.OpenApi;
using Assimalign.Cohesion.OpenApi.Attributes;
using Assimalign.Cohesion.Web.OpenApi;

// builder is the application's WebApplicationBuilder. The application also registers
// authentication with a "Bearer" scheme and AddAuthorization, and its pipeline runs
// UseAuthentication and UseAuthorization.
builder.Services.AddOpenApi(options =>
{
    options.Title = "Orders API";
    options.AddSecurityScheme(new OpenApiSecuritySchemeMetadata
    {
        Name = "Bearer",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
});
```

The adapter does not interpret authorization metadata itself: for each endpoint it asks
Web.Authorization for the effective policy, the one `UseAuthorization` applies (`GetEffectivePolicy`
over the registered options), so the default, fallback and named policies are all taken into
account. The policy's `AuthenticationSchemes` become the requirement; a policy that names none uses
the default authenticate scheme. Each scheme the document declares becomes one requirement with no
scopes, the requirements being alternatives.

With `Bearer` as the default authenticate scheme and a `partners` policy that selects `ApiKey`:

| Route | Described |
|---|---|
| No authorization metadata, no fallback policy | Open |
| No authorization metadata, `FallbackPolicy = DefaultPolicy` | `Bearer` |
| `RequireAuthorization("partners")` | `ApiKey` only |
| Group `RequireAuthorization("partners")`, route `AllowAnonymous()` | Open |
| Group `AllowAnonymous()`, route `RequireAuthorization("partners")` | `ApiKey` |

An application without `AddAuthorization` is described with the defaults `AddAuthorization()` would
register, so a route that declares a requirement is never described as open. A policy name that is
not registered fails the document with an `InvalidOperationException` naming the endpoint and the
policy. The document describes the registered policies, not the pipeline: an application that sets
a fallback policy but never registers `UseAuthorization` lists the requirement while serving its
unannotated endpoints unauthorized. `OpenApiSecuritySchemeMetadata` has no OAuth2 flows, so an OAuth2
scheme needs a document transformer; OpenID Connect, HTTP and API-key schemes are declared directly.

## Customize the document

```csharp
using Assimalign.Cohesion.OpenApi;
using Assimalign.Cohesion.OpenApi.Attributes;
using Assimalign.Cohesion.Web.OpenApi;

// builder is the application's WebApplicationBuilder.
builder.Services.AddOpenApi(options => options
    .AddTag(new OpenApiTagMetadata { Name = "orders", Description = "Order operations" })
    .AddDocumentTransformer(document => document.Servers.Add(new OpenApiServer { Url = "https://api.example.com" }))
    .AddDocumentTransformer(document => document.Info.Contact = new OpenApiContact { Name = "Orders team" }));
```

- **`AddTag`** declares a tag with its description; declared tags come first in the document, then
  the names endpoints use.
- **`AddDocumentTransformer`** edits each built document for what endpoint metadata does not carry:
  servers, contact and license information, external documentation, or extensions. A document
  endpoint builds its document once, so a transformer runs once per endpoint and must not depend on
  the request.
- **`AddEndpointSource`** composes another `IOpenApiEndpointSource` after the routes, for example the
  registry the OpenApi attribute source generator emits. An operation on a path and method the
  routes already describe replaces the route's.
- **`GetOpenApiDescriptionProvider()`** on the application returns the same document as a model, for
  tools and tests. Each call builds a new document; call it after the application has mapped its
  endpoints, because building reads the router and closes the route table.

## Limits

- **One document per application**, rendered per line and format.
- **One media type per body and per status.** A negotiated response lists the default writer's media
  type, and a form of fields alone lists `application/x-www-form-urlencoded`.
- **No antiforgery or CORS** in the description, and no discriminator objects, examples or `x-`
  extensions from contracts; a document transformer can add them.

For binding, return values and the description metadata itself, see
[Endpoints and responses](endpoints.md). Return to [Web](index.md).

## Sources

- **Package overview and design** — `cohesion/resources/Web/Assimalign.Cohesion.Web.OpenApi/docs/OVERVIEW.md` and `cohesion/resources/Web/Assimalign.Cohesion.Web.OpenApi/docs/DESIGN.md`.
- **Registration and options** — `cohesion/resources/Web/Assimalign.Cohesion.Web.OpenApi/src/ComponentModel/OpenApiComponents.cs`, `cohesion/resources/Web/Assimalign.Cohesion.Web.OpenApi/src/Extensions/OpenApiWebApplicationExtensions.cs`, and `cohesion/resources/Web/Assimalign.Cohesion.Web.OpenApi/src/OpenApiOptions.cs`.
- **Document endpoint** — `cohesion/resources/Web/Assimalign.Cohesion.Web.OpenApi/src/Internal/OpenApiDocumentEndpoint.cs`.
- **Test composition** — `cohesion/resources/Web/Assimalign.Cohesion.Web.OpenApi/tests/TestObjects/OpenApiTestApplication.cs` and `cohesion/resources/Web/Assimalign.Cohesion.Web.OpenApi/tests/OpenApiOptionsTests.cs`.
- **Description metadata and verbs** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Api/docs/DESIGN.md`.
- **NativeAOT guard** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Hosting/samples/Assimalign.Cohesion.Web.AotGuard/Program.cs`.
