# Assimalign.Cohesion.Web.Forms

A single-purpose bridge that plugs HTTP form parsing (`Assimalign.Cohesion.Http.Forms`) into the Web application middleware pipeline (`Assimalign.Cohesion.Web`).

> **Status:** Implemented.

## Reference pages

- **[Design](design.md)** — Form parsing boundaries.
- **[Examples](examples/index.md)** — Middleware behavior exercised by tests.

The package owns `UseForms()`, a pipeline-builder extension in the `Assimalign.Cohesion.Web`
namespace. Parsing remains in `Assimalign.Cohesion.Http.Forms`. The middleware obtains
`IHttpFormFeature` from the request features, creates `HttpFormFeature` if absent, awaits
`ReadFormAsync(context.RequestCancelled)`, and invokes the next middleware. A form the parse rejects
is answered there and the rest of the pipeline does not run (#1210): `413 Content Too Large` when the
body exceeds a configured `HttpFormOptions` limit, and `400 Bad Request` when it is malformed, both as
`application/problem+json` with the payload a form-bound endpoint writes for the same failure.

The two form-error classifications apply only when the transport has reported no
client-fault status. An absent feature or a feature whose `StatusCode` is `null`
still allows normal form rejection handling.

`ValueCountLimit` also bounds every multipart section (#1435), including files, repeated fields and
unnamed parts that the parser skips. Exceeding the default 1,024 sections now answers `413`; this is a
behavior break from 10.0.0-preview.1, which did not enforce a section count. Diagnostics can read the
cause’s `HttpFormLimitExceededException.Limit` enum instead of parsing or logging an exception message
that may echo input.

Parsing is eager for every request; bodies with a non-form media type produce an empty collection.
Handlers needing form data only on selected routes can call `ReadFormAsync` lazily instead. The
source currently exposes no options overload.

No project `OVERVIEW.md` is present; this reference uses the extension source and the project’s
`DESIGN.md`.

[Parent: Web](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Forms/src/Extensions/WebApplicationExtensions.cs`.
- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Forms/src/Internal/FormsMiddleware.cs`.
- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Forms/docs/DESIGN.md`.
- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Forms/src/Assimalign.Cohesion.Web.Forms.csproj`.
