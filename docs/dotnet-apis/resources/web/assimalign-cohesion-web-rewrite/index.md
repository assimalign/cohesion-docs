# Assimalign.Cohesion.Web.Rewrite

URL rewriting and redirect rules for the Cohesion Web pipeline.

> **Status:** Partial.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.

URL rewriting and redirect rules for the Cohesion Web pipeline (#782). A Cohesion application serves
its own traffic, with no IIS or nginx rewrite tier assumed in front of it, so canonical URLs,
legacy-path migration and internal rewrites have to be expressible in the application.
`UseRewrite(rules => ...)` registers code-first rules that run, in order, ahead of static files and
routing.

## Scope

- **Internal rewrites.** A rewrite changes the path and query the rest of the pipeline sees, without
  telling the client. The request itself is never mutated: the rest of the pipeline receives a
  request view whose `Path` and `Query` are the rewritten values, and `IWebRewriteFeature` keeps the
  originals readable (Web ADR 1). Routing, static files and the generated endpoint binders follow
  the rewrite with no change.
- **Redirects.** A redirect answers `301`, `302`, `307` or `308` with a `Location` and ends the
  pipeline.
- **Rule kinds.** Regular-expression rules over the path, or the path and query, with `$1`/`${name}`
  substitutions; predicate rules; delegate rules; and custom `IRewriteRule` implementations. String
  patterns run interpreted with a match timeout; a caller-supplied `Regex` can be `NonBacktracking`
  or a source-generated `[GeneratedRegex]`. `RegexOptions.Compiled` is never used.
- **Canonicalization helpers.** Redirects to HTTPS, to `www.` or away from it, to a trailing slash
  or away from it, and to a lowercase path. They read the effective scheme and host, so they work
  behind a TLS-terminating proxy that `UseForwardedHeaders` trusts, and they are idempotent.
- **Flow and loop protection.** After a rewrite, evaluation continues with the next rule, skips the
  remaining rules, or restarts from the first rule. Restarts are bounded by `MaxPasses`.
- **Path branches.** Inside `Map(path)`, the rules see and rewrite the path below the branch's
  prefix, and redirect targets are relative to it.

## Dependencies

- **`Assimalign.Cohesion.Web`** — the pipeline builder, the middleware abstraction, and the
  path-branch view (`IWebPathBaseFeature`).
- **`Assimalign.Cohesion.Http`** — the HTTP context, the `HttpPath`, `HttpHost` and query value
  objects, and the parsing the transports use for a request target.
- **`Assimalign.Cohesion.Http.Forwarded`** — the effective scheme and host the canonicalization
  helpers read.

It never references `Assimalign.Cohesion.Web.Hosting` or any `Assimalign.Cohesion.Hosting*` library
(`COHRES001`, `COHRES004`), nor Web.Routing: it runs ahead of routing and needs none of its types.
The package is a member of the `App.Web` shared framework.

## Usage

See the [source-backed usage examples](examples/index.md) and the
[rewrite guide](../../../../web/rewrite.md).

```csharp
using Assimalign.Cohesion.Http;
using Assimalign.Cohesion.Web.Rewrite;

// app is the built WebApplication. Behind a proxy, UseForwardedHeaders and UseHostFiltering come
// first, so the helpers read the client's scheme and a validated host.
// Ahead of UseStaticFiles and UseRouting.
app.UseRewrite(rules => rules
    // Redirects first: a redirect built after a rewrite would send the client to the rewritten URL.
    .AddRedirectToHttps()
    .AddRedirectToNonWww()
    .AddRedirect("^/blog/(\\d+)/(.*)$", "/posts/$2", HttpStatusCode.MovedPermanently)
    .AddRedirect("^/item\\.php\\?id=(\\d+)$", "/items/$1?", HttpStatusCode.MovedPermanently, RewriteMatchTarget.PathAndQuery)

    // Internal rewrites: the client keeps its URL.
    .AddRewrite("^/assets/v\\d+/(.*)$", "/$1")
    .AddRewrite("^/products/(\\d+)$", "/product?id=$1")
    .AddRewrite(context => context.HttpContext.Request.Headers.ContainsKey("X-Mobile"), "/mobile"));
```

Inside an endpoint, `context.Request.Path` is the rewritten path, and the URL the client sent is
`context.Features.Get<IWebRewriteFeature>()?.OriginalPath`.

See the [design](design.md) for the request view and the pitfall it carries, path branches, rule
evaluation and loop protection, the target syntax and its encoding model, redirects, the
canonicalization helpers, ordering, telemetry, the error model and the non-goals.

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Web` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Http` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Http.Forwarded` | `CohesionProjectReference` |

[Parent: Web](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Rewrite/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Rewrite/src/Assimalign.Cohesion.Web.Rewrite.csproj`.
- **Verbs and options** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Rewrite/src/Extensions/RewriteExtensions.cs` and `cohesion/resources/Web/Assimalign.Cohesion.Web.Rewrite/src/RewriteOptions.cs`.
- **Framework membership** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Runtime/Directory.Build.props`.
