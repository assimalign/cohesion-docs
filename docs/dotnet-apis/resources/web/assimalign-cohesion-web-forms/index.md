# Assimalign.Cohesion.Web.Forms

A single-purpose bridge that plugs HTTP form parsing (`Assimalign.Cohesion.Http.Forms`) into the Web application middleware pipeline (`Assimalign.Cohesion.Web`).

> **Status:** Implemented.

## Reference pages

- **[Design](design.md)** — Form parsing boundaries.
- **[Examples](examples/index.md)** — Middleware behavior exercised by tests.

The package owns `UseForms()`, a pipeline-builder extension in the `Assimalign.Cohesion.Web`
namespace. Parsing remains in `Assimalign.Cohesion.Http.Forms`. The middleware obtains
`IHttpFormFeature` from the request features, creates `HttpFormFeature` if absent, awaits
`ReadFormAsync(context.RequestCancelled)`, and invokes the next middleware.

Parsing is eager for every request; bodies with a non-form media type produce an empty collection.
Handlers needing form data only on selected routes can call `ReadFormAsync` lazily instead. The
source currently exposes no options overload.

No project `OVERVIEW.md` is present; this reference uses the extension source and the project’s
`DESIGN.md`.

[Parent: Web](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Forms/src/Extensions/WebApplicationExtensions.cs`.
- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Forms/docs/DESIGN.md`.
- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Forms/src/Assimalign.Cohesion.Web.Forms.csproj`.
