# Endpoints and responses

Web endpoints bind request inputs and write responses through the middleware pipeline and serialization registry.

> **Status:** Implemented. A handler writes the response or returns a plain value; there are no result types.

## Endpoint mapping

`Assimalign.Cohesion.Web.Api` supplies `Map`, `MapGet`, and the other endpoint mappings over
[routing](routing.md), plus `MapGroup` and `MapFallback`. Typed delegate handlers use
`Assimalign.Cohesion.SourceGeneration.Web` to generate parameter binding compatible with Native
ahead-of-time compilation (NativeAOT). A handler can be a lambda or a method group
(`app.MapGet("/users/{id}", GetUser)`), and every call form reaches the generator: `app.MapGet(...)`,
a conditional access `app?.MapGet(...)`, the static form of the extension member, and a call over a
type parameter in a reusable endpoint module.

Every `Map*` returns the mapped route's `IRouterRouteBuilder`, so endpoint policies attach where
the endpoint is mapped:

```csharp
using Assimalign.Cohesion.Http;
using Assimalign.Cohesion.Web;
using Assimalign.Cohesion.Web.RateLimiting;
using Assimalign.Cohesion.Web.Routing;
using Assimalign.Cohesion.Web.Serialization;

// app is the built WebApplication; Order is in the application's JsonSerializerContext.
app.MapGet("/orders/{id:int}", async (int id, IHttpContext context) =>
{
    context.Response.StatusCode = HttpStatusCode.Ok;
    await context.Response.WriteContentAsync(new Order(id), context.RequestCancelled);
})
    .WithName("order")
    .RequireRateLimiting("api");
```

Because a mapping returns the route builder, `Map*` calls no longer chain into one another
(`app.MapGet(...).MapGet(...)`); write one statement per endpoint. Route groups take the same
typed handlers: `app.MapGroup("/api").MapGet("orders", handler)` binds exactly like an
application endpoint, and the group's prefix, metadata and policies apply to it.

| Input | Selection | Behavior |
|---|---|---|
| Route value | Matching parameter name or `[FromRoute]` | Bind from the matched route token. |
| Route or query value | A scalar the call site cannot place: on a group endpoint, or with a non-literal pattern | Bind from the route value when the matched route captured one, else from the query string. |
| Query value | Scalar default or `[FromQuery]` | Parse the query-string value. |
| Header | `[FromHeader]` | Explicit header binding. |
| Request body | Complex default or `[FromBody]` | One body parameter, read through the serialization registry. |
| Form field | `[FromForm]` | Bind individual scalar fields; the endpoint requires antiforgery validation. |
| Uploaded file | `IHttpFormFile`, a file sequence, or `IHttpFormFileCollection`, with or without `[FromForm]` | Bind from the `multipart/form-data` body; the endpoint requires antiforgery validation. See [File uploads](#file-uploads). |
| `IHttpContext` | Parameter type | Inject the current exchange. |
| `IHttpRequest` / `IHttpResponse` | Parameter type | Inject `context.Request` / `context.Response`. |
| `CancellationToken` | Parameter type | Supply `RequestCancelled`. |
| `IHttpFeature` implementation | Parameter type | Resolve the typed context feature. |

Missing required or invalid scalar inputs, and a missing required file, produce Hypertext Transfer
Protocol (HTTP) 400 problem responses with an `errors` extension naming the parameter. A form over an
Http.Forms size limit produces 413 and a malformed form 400; with `UseForms()` in the pipeline the
middleware parses the form first and answers it with the same payload, and the endpoint does not
run. A request whose Content-Type is missing or has no registered reader produces 415; a malformed
body, its message framing included, produces 400. A body type the registered resolver has no
contract for, or an application with no serialization registry, is the server's fault, not the
client's: `HttpContentSerializationException` reaches the exception boundary (a 500) instead of a
415.

An endpoint with a `[FromForm]` or uploaded-file parameter requires antiforgery when the application
references `Assimalign.Cohesion.Web.Antiforgery`, which every `Sdk.Web` application does through
the shared framework. Register `builder.Services.AddAntiforgery(...)` and `UseAntiforgery()` after
`UseRouting()`, or opt the endpoint out with `.DisableAntiforgery()`; without the middleware the
endpoint fails at dispatch instead of running unprotected. The generator attaches the requirement
where the endpoint is mapped, so a group's `DisableAntiforgery()` does not reach it. See
[Web.Antiforgery](../dotnet-apis/resources/web/assimalign-cohesion-web-antiforgery/index.md).

## Return values

A handler that returns a value — directly, or through `Task<T>` or `ValueTask<T>` — has it written as
the response by the generated thunk, so `app.MapGet("/orders/{id}", (long id) => orders.Get(id))`
needs no `IHttpContext`:

| The handler returns | The response |
|---|---|
| Nothing (`void`, `Task`, `ValueTask`) | Whatever the handler wrote |
| A `string` | The text as UTF-8, `text/plain; charset=utf-8` unless the handler set a `Content-Type` |
| `null` | No body; `204 No Content` unless the handler set another status |
| Any other value | Serialized through the serialization registry for the request's `Accept`, with `Vary: Accept`; a bodyless `406` when nothing registered is acceptable |

- **Status** — 200 unless the handler set one: a handler that sets
  `context.Response.StatusCode = HttpStatusCode.Created` and returns the order answers 201 with the
  order as its body.
- **Contracts** — a serialized type needs a contract in the registered resolver
  (`[JsonSerializable(typeof(Order))]` on the application's `JsonSerializerContext`). A missing
  contract or registry throws `HttpContentSerializationException` to the exception boundary (a 500);
  there is no reflection fallback.
- **Streams** — returning a `Stream` is a compile error. Copy the stream to the response body and
  return `Task`, or send it with `Web.StaticFiles`' `WriteStreamAsync` (see
  [Writing responses](#writing-responses)). A `byte[]` is an ordinary value, serialized as base64
  JSON.

## Validation

`Assimalign.Cohesion.Web.Validation` validates the request-body model a typed endpoint binds, after
every parameter is bound and before the handler runs. Validators are ObjectValidation profiles,
registered per model type:

```csharp
using Assimalign.Cohesion.ObjectValidation;
using Assimalign.Cohesion.Web;
using Assimalign.Cohesion.Web.Validation;

// builder is the WebApplicationBuilder and app the built WebApplication; Customer is in the
// application's JsonSerializerContext.
builder.Services.AddValidation(validation => validation.AddProfile(new CustomerProfile()));

// An invalid customer is answered 400 before the handler runs.
app.MapPost("/customers", (Customer customer) => customer);

// Drafts are saved as they are.
app.MapPost("/drafts", (Customer customer) => customer).DisableValidation();

internal sealed record Address(string City);

internal sealed record Customer(string Name, int Age, Address Address);

internal sealed class CustomerProfile : ValidationProfile<Customer>
{
    public override void Configure(IValidationRuleDescriptor<Customer> descriptor)
    {
        descriptor.RuleFor(customer => customer.Name).NotEmpty();
        descriptor.RuleFor(customer => customer.Age).GreaterThanOrEqualTo(18);
        descriptor.RuleFor(customer => customer.Address).ChildRules(address => address.RuleFor(a => a.City).NotEmpty());
    }
}
```

An invalid model is answered `400` `application/problem+json` with the `detail` "One or more
validation errors occurred." and an `errors` map from member path to messages, the shape binding
failures use: `Name`, and `Address.City` for the nested profile's rule. The map lists members in
the order the profile declares them, and each member's messages in the order its rules are chained.
A validator that throws, such as a nested profile's rule or a custom rule, is a fault rather than a
verdict: the request reaches the exception boundary and the handler does not run. A binding failure
is answered first. A body type with no registered validator, and a `null` body, are not validated;
until `AddValidation` is called nothing is. `options.Enabled = false` turns validation off by
default, and `RequireValidation()` / `DisableValidation()` turn it on or off for a route or a group,
the most specific declaration winning. A handler that binds a value itself validates it with
`if (!await context.ValidateAsync(order, context.RequestCancelled)) return;`. See
[Web.Validation](../dotnet-apis/resources/web/assimalign-cohesion-web-validation/index.md).

## File uploads

A typed handler binds uploaded files from a `multipart/form-data` body by their type, with or
without `[FromForm]` (whose `Name` sets the field name):

```csharp
using System.Collections.Generic;

using Assimalign.Cohesion.Http;
using Assimalign.Cohesion.Web;

// app is the built WebApplication; builder.Services.AddAntiforgery(...) registered antiforgery, and
// UseAntiforgery follows UseRouting.
app.MapPost("/profiles/{id:long}/media", (long id, IHttpFormFile avatar, IReadOnlyList<IHttpFormFile> photos) =>
    $"{avatar.FileName}: {avatar.Length} bytes, {photos.Count} photos");
```

- **`IHttpFormFile`** is the first file uploaded under its field name, required unless the parameter
  is nullable; a missing required file is a `400`.
- **A file sequence** (`IHttpFormFile[]`, `IReadOnlyList<IHttpFormFile>`,
  `IReadOnlyCollection<IHttpFormFile>`, `IEnumerable<IHttpFormFile>`) is every file uploaded under the
  name, possibly none.
- **`IHttpFormFileCollection`** is every uploaded file, whatever its field name.
- **Limits** — the form is read once, for every field and file, under the Http.Forms limits of the
  exchange's form feature (`HttpFormOptions`). A form over a limit is answered
  `413 Content Too Large`, and any other unreadable form `400`. Install
  `new HttpFormFeature(context.Request, options)` in a middleware ahead of the endpoint to change the
  limits.
- **Antiforgery** — a file endpoint requires antiforgery, like a form-field endpoint.

Another attribute on a file parameter, or another file shape (`List<IHttpFormFile>`, the concrete
`HttpFormFile`), is a compile error rather than a JSON body.

## Describing endpoints

Every typed endpoint describes itself in route metadata for documentation adapters: an
`EndpointParameterMetadata` per request-bound parameter (name, source, CLR type, required) and
`EndpointResponseMetadata` for its responses (a `200` with the written type, `text/plain` for a
string, and a `204` when the result may be `null`). Four convention verbs curate the description on
a route or a group without depending on a documentation format: `WithTags` (tags compose, group
first), `WithSummary` and `WithDescription` (the most specific wins), and `ExcludeFromDescription`.
A response the handler answers on its own is described with
`.WithMetadata(new EndpointResponseMetadata(HttpStatusCode.NotFound))`. The [OpenAPI](openapi.md)
guide shows how `Web.OpenApi` turns this metadata into a document.

## Compile-time diagnostics

A handler the generator cannot bind fails the build with a `COHWEB` error that names the endpoint and
says what to write instead, rather than throwing when the endpoint is mapped:

| ID | Reported when |
|---|---|
| COHWEB0001 | The handler is a delegate instance rather than a lambda or a method group |
| COHWEB0002 | The return type cannot be written, such as `async void` or a stream |
| COHWEB0003 | A parameter cannot be bound |
| COHWEB0004 | More than one parameter binds from the request body |
| COHWEB0005 | The handler binds a request body and form fields or uploaded files |
| COHWEB0006 | Generated code cannot name the handler's delegate type |
| COHWEB0007 | The endpoint reads a body or returns a negotiated value without `Web.Serialization` referenced |

The full conditions are in the
[Web.Api design](../dotnet-apis/resources/web/assimalign-cohesion-web-api/design.md#compile-time-diagnostics-1059).

## Generator setup

The Web SDK adds the generator automatically. For explicit project wiring, the documented build
items are:

```xml
<ItemGroup>
  <CohesionAnalyzerReference Include="Assimalign.Cohesion.SourceGeneration.Web" />
</ItemGroup>
<PropertyGroup>
  <InterceptorsNamespaces>$(InterceptorsNamespaces);Assimalign.Cohesion.Web.Api.Generated</InterceptorsNamespaces>
</PropertyGroup>
```

Body binding and serialized return values also need `Web.Serialization` registration; form and file
binding consume `Http.Forms`. Those libraries are supplied through the Web shared framework, but the
application still composes the features it uses.

## Writing responses

Handlers can also set response status, headers, and body directly; the template on the
[Web page](index.md) shows a complete executable writing response bytes. There is no shipped
`IResult`, `Results`, or `TypedResults` programming model: those proposed abstractions were
withdrawn before merge, and a returned value is plain data. Controller and function packages were
also removed from that direction.

`Web.Serialization` supplies independent `IHttpContentReader` and `IHttpContentWriter` contracts
keyed by media type. `builder.Services.AddJsonSerialization(...)` uses a source-generated resolver
for JavaScript Object Notation (JSON). `ReadContentAsync` and `WriteContentAsync` provide the request/response call sites,
and `WriteNegotiatedContentAsync` selects the writer from the request's `Accept`.

`Web.StaticFiles` adds two response helpers that answer the way the static-files middleware does —
validators, conditional requests, single byte ranges, and `HEAD`:
`context.Response.SendFileAsync(fileSystem, path)` sends a path inside an `IFileSystem` mount (an
unsafe, missing, or directory path is answered `404`) or `SendFileAsync(file)` a file the handler
resolved, and `context.Response.WriteStreamAsync(stream, contentType, entityTag, lastModified)` sends
a stream with the validators the caller supplies. See
[Web.StaticFiles](../dotnet-apis/resources/web/assimalign-cohesion-web-staticfiles/index.md).

## Faults and problem payloads

`Web.ProblemDetails` owns the `ProblemDetails` payload, its `application/problem+json` writer,
and `WriteProblemDetailsAsync`. The writer is reflection-free. This library defines the payload;
it does not decide which application outcomes are errors.

`Web.ErrorHandling` supplies `builder.Services.AddErrorHandling(errors => errors.OnError(...))` and
`UseErrorHandling`.
The middleware captures faults in `IHttpExceptionFeature`, dispatches an ordered handler chain,
and provides a terminal problem response. It avoids clobbering an already committed response.
`UseStatusCodePages` can upgrade an otherwise bodyless 404. A request body the client broke, or
cut short, while a handler read it is not treated as a fault: on an HTTP/1.1 request the boundary
skips `OnException` and the `OnError` chain and stages the `400`, `413`, `408` or `431` the
transport answers with (see [Client faults](server.md#client-faults-and-refused-responses)).

Return to [Web](index.md).

## Sources

- **Mapping and binding** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Api/docs/OVERVIEW.md` and `cohesion/resources/Web/Assimalign.Cohesion.Web.Api/docs/DESIGN.md`.
- **Diagnostics** — `cohesion/analyzers/Assimalign.Cohesion.SourceGeneration.Web/src/AnalyzerReleases.Unshipped.md`.
- **Antiforgery on form-bound endpoints** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Antiforgery/docs/DESIGN.md`.
- **Validation** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Validation/docs/OVERVIEW.md`.
- **File and stream helpers** — `cohesion/resources/Web/Assimalign.Cohesion.Web.StaticFiles/docs/OVERVIEW.md`.
- **Form limits** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Forms/docs/DESIGN.md` and `cohesion/resources/Web/Assimalign.Cohesion.Web.Forms/docs/DESIGN.md`.
- **Serialization and withdrawn results** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Serialization/docs/DESIGN.md`.
- **Problem payload** — `cohesion/resources/Web/Assimalign.Cohesion.Web.ProblemDetails/docs/DESIGN.md`.
- **Error-handling surface** — `cohesion/resources/Web/README.md` and `cohesion/resources/Web/Assimalign.Cohesion.Web.ErrorHandling/docs/OVERVIEW.md`.
- **Programming-model direction** — `cohesion/docs/programs/HTTP_WEB_PROGRAM_PLAN.md`.
