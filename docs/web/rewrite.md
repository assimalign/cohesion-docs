# URL rewriting

Web.Rewrite rewrites request URLs inside the application and answers redirects, ahead of static files and routing.

> **Status:** Implemented. Rules are code: no rule-file import, asynchronous rules, or query-merging flag.

A Cohesion application serves its own traffic, with no IIS or nginx rewrite tier assumed in front of
it, so canonical URLs, the migration of legacy paths and internal rewrites have to be expressible in
the application (#782).
[`Web.Rewrite`](../dotnet-apis/resources/web/assimalign-cohesion-web-rewrite/index.md), a member of
the `App.Web` shared framework, is that work as one pipeline verb, `UseRewrite(rules => ...)`, over
code-first rules registered at composition time.

## Register the rules

`UseRewrite` goes ahead of `UseStaticFiles` and `UseRouting`, so both see the rewritten URL. This
is a complete `Program.cs`:

```csharp
using Assimalign.Cohesion.Http;
using Assimalign.Cohesion.Web;
using Assimalign.Cohesion.Web.Hosting;
using Assimalign.Cohesion.Web.Rewrite;
using Assimalign.Cohesion.Web.Routing;
using Assimalign.Cohesion.Web.StaticFiles;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddRouting();

await using WebApplication app = builder.Build();

app.UseRewrite(rules => rules
    // Redirects first: a redirect evaluated after a rewrite would send the client to the rewritten URL.
    .AddRedirect("^/blog/(\\d+)/(.*)$", "/posts/$2", HttpStatusCode.MovedPermanently)
    .AddRedirect("^/item\\.php\\?id=(\\d+)$", "/items/$1?", HttpStatusCode.MovedPermanently, RewriteMatchTarget.PathAndQuery)

    // Internal rewrites: the client keeps its URL.
    .AddRewrite("^/assets/v\\d+/(.*)$", "/$1")
    .AddRewrite("^/products/(\\d+)$", "/product?id=$1"));

app.UseStaticFiles();
app.UseRouting();

// Binds the rewritten query: /products/7 is served as /product?id=7.
app.MapGet("/product", (int id) => $"product {id}");

await app.RunAsync();
```

The rules run in registration order, each once, in a *pass*, and each sees the URL as the rules
before it left it. A rule acts in one of these ways:

| Action | Rule evaluation | The pipeline |
| --- | --- | --- |
| None | Next rule | — |
| Rewrite, `RewriteFlow.Continue` (default) | Next rule, on the rewritten URL | Continues with the rewritten URL |
| Rewrite, `RewriteFlow.SkipRemainingRules` | Ends | Continues with the rewritten URL |
| Rewrite, `RewriteFlow.Restart` | A new pass from the first rule, on the rewritten URL | — |
| `SkipRemainingRules()` without a rewrite | Ends | Continues |
| Redirect | Ends | Ends with the redirect |
| `EndResponse()` | Ends | Ends with the rule's own response |

`RewriteOptions.MaxPasses` (default `10`) bounds the passes a request may take. Restart is the only
way a rule set can loop, so a request that would need more fails with an
`InvalidOperationException`, which the exception boundary answers `500`: a rule set that keeps
restarting has a bug, and a `500` surfaces it.

## Patterns and targets

`AddRewrite` and `AddRedirect` take a regular expression and a target:

- **A string pattern** is compiled once, at registration, as an interpreted, culture-invariant,
  case-sensitive expression with a one-second match timeout. For linear-time matching pass a `Regex`
  built with `RegexOptions.NonBacktracking`; for compiled-speed matching under NativeAOT pass a
  source-generated `[GeneratedRegex]`. The package never uses `RegexOptions.Compiled`.
- **What it matches.** By default the path as routing sees it: percent-decoded, starting with `/`.
  `RewriteMatchTarget.PathAndQuery` matches the path, `?`, and the query rebuilt from the parsed
  collection, each key and value percent-encoded.
- **Substitutions.** `$n` and `${n}` substitute a capture group (`$0` is the whole match), `${name}`
  a named group, and `$$` is a literal `$`. A reference to a group the pattern does not define is an
  `ArgumentException` at registration, so a typo fails at startup.
- **The query.** A target without `?` keeps the request's query, a target with one replaces it, and
  a target that ends in `?` removes it.
- **Encoding.** The request path is decoded exactly once, by the transport. Each capture is
  percent-encoded for the part of the URL it lands in, and the result is parsed back the way a
  transport parses a request target, so a capture can never add a query parameter, a fragment, or a
  host. When a capture makes the target undecodable, the rule answers `400 Bad Request`, which is
  what a transport answers for the same request target sent directly.

A rewrite target is a path, with an optional query; a redirect may also carry a `#fragment` or be an
absolute `http://` or `https://` URL. A predicate rule (`AddRewrite(predicate, target)`,
`AddRedirect(predicate, target)`) has a fixed target, and a delegate rule acts through
`IRewriteContext`. This guard keeps `/api/` out of a catch-all rewrite below it:

```csharp
using Assimalign.Cohesion.Http;
using Assimalign.Cohesion.Web.Rewrite;

// app is the built WebApplication.
app.UseRewrite(rules => rules
    .Add(context =>
    {
        if (context.Path.StartsWith(new HttpPath("/api/")))
        {
            context.SkipRemainingRules();
        }
    })
    .AddRewrite("^/(.*)$", "/spa/index.html"));
```

A rule reads `IRewriteContext.Path` and `Query`, never `HttpContext.Request.Path`, which keeps the
value the middleware received. A rule the built-in kinds cannot express implements `IRewriteRule` and
registers with `RewriteOptions.Add`; one instance serves every request, so it keeps no per-request
state. Rules are synchronous: a rule that needs I/O is better written as its own middleware.

## Redirects

A redirect answers with its status and `Location` and ends the pipeline. `AddRedirect` defaults to
`302`, because browsers cache a permanent redirect and a mistaken one is hard to take back; the
statuses are `301` and `308` for a permanent move and `302` and `307` for a temporary one, and `307`
and `308` keep the request method and body. Any other status fails at registration.

The canonicalization helpers are redirect rules that answer only when the URL is not already in its
canonical form, and default to `308`:

| Helper | Redirects when | To |
| --- | --- | --- |
| `AddRedirectToHttps` | the effective scheme is not `https` | `https://` + host + the HTTPS port (`443` omitted) |
| `AddRedirectToWww` | the host has no `www.` prefix | `www.` + host, scheme and port kept |
| `AddRedirectToNonWww` | the host has the `www.` prefix | the host without it |
| `AddRedirectToTrailingSlash` | the path has no trailing slash, is not the root, and its last segment has no `.` | the path + `/` |
| `AddRedirectToNoTrailingSlash` | the path ends in `/` and is not the root | the path without every trailing `/` |
| `AddRedirectToLowercase` | the path has an uppercase letter | the path in lowercase, the query untouched |

- **Effective values.** The helpers read the effective scheme and host, so behind a TLS-terminating
  proxy that `UseForwardedHeaders` trusts, a forwarded `https` counts as secure and the HTTPS rule
  never redirects a proxied request in a loop. A client's own `X-Forwarded-Proto` changes nothing
  without that middleware.
- **Safe hosts.** A host that is not a DNS name or IP literal is never echoed into a `Location`. The
  `www` helpers skip `localhost`, `*.localhost` and IP literals, and take an optional list of the
  hosts they apply to. `UseHostFiltering` ahead of `UseRewrite` is still the allowlist.
- **One redirect per helper.** Each helper is idempotent, and the first that applies answers, so a
  URL that differs in scheme and case takes two hops.

Register redirects, the helpers included, ahead of internal rewrites: a redirect evaluated after a
rewrite sends the client to the rewritten URL, exposing an internal path.

## The request view and `IWebRewriteFeature`

A rewrite never changes the request: `IHttpRequest` is immutable. The rest of the pipeline receives a
request view whose `Path` and `Query` are the rewritten values and whose every other member is the
original's, so routing, static files and the generated binders follow the rewrite with no change
(the Web area's ADR 1). While the rest of the pipeline runs, `IWebRewriteFeature` keeps the values
the client sent:

```csharp
using Assimalign.Cohesion.Http;
using Assimalign.Cohesion.Web.Rewrite;

// Inside an endpoint or a middleware registered after UseRewrite; context is the IHttpContext.
HttpPath clientPath = context.Features.Get<IWebRewriteFeature>()?.OriginalPath ?? context.Request.Path;
```

- **Middleware registered ahead of `UseRewrite`** keeps the client's URL, as do the server's request
  telemetry and `UseHttpLogging`. The span's `url.path` is the client's path, and its `http.route` is
  the route the rewritten path matched: `/legacy/orders/9` rewritten to `/orders/9` reports
  `url.path=/legacy/orders/9` and `http.route=/orders/{id:int}`.
- **A component that holds a context across `next`** sees the values of the context it holds, so a
  middleware that captures the context before the rewrite does not see the rewrite after `next`
  returns.
- **A URL built after the rewrite from `Request.Path`**, such as the cookie authentication handler's
  sign-in return URL, carries the rewritten path. Read `IWebRewriteFeature.OriginalPath` for the
  client's URL.
- **`context.Response.HttpContext`** is the original context, because the response is shared rather
  than wrapped. Reach the request through the context you were handed.

## Path branches

Inside `Map(path)`, the rules work on the branch's terms. They see the path below the branch's
prefix, which `IRewriteContext.PathBase` reports: a rule written inside `Map("/docs")` matches
`/old/intro`, not `/docs/old/intro`. `Map(path)` and the path-branch view it publishes,
`IWebPathBaseFeature`, are `Web.Routing`'s since #1379, which is why Web.Rewrite references that
package although it runs ahead of routing. A rewrite publishes a matching `IWebPathBaseFeature`, so
`/docs/old/intro` rewritten to `/new/intro` reads `/docs/new/intro` as `Request.Path`. A redirect's
target path is relative to the prefix, and an absolute `http` or `https` target leaves the branch. An
application-level rewrite ahead of a branch decides which branch runs, because `Map` matches the
path of the view it receives.

## Order

`UseRewrite` is row 9 of the [middleware order](middleware-order.md):

- **After `UseForwardedHeaders` and `UseHostFiltering`**, so the helpers read the client's scheme
  and a validated host, and after `UseHttpsRedirection` and `UseHsts`.
- **Inside the exception boundary and after `UseStatusCodePages`**, so a fault in a rule becomes a
  problem-details response and a rule's `400` gets a body.
- **Ahead of everything that reads the path**: `UseStaticFiles`, `UseRouting` and the endpoint.

With orchestration enabled, `Web.Hosting` answers the resource control plane and the Web.Health
endpoints ahead of the application's pipeline, so the rules never see those requests.

For the package's design, including the encoding table and the error model, see
[Web.Rewrite's design](../dotnet-apis/resources/web/assimalign-cohesion-web-rewrite/design.md).
Return to [Web](index.md).

## Sources

- **Package** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Rewrite/docs/OVERVIEW.md` and `cohesion/resources/Web/Assimalign.Cohesion.Web.Rewrite/docs/DESIGN.md`.
- **Verbs and options** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Rewrite/src/Extensions/RewriteExtensions.cs`, `cohesion/resources/Web/Assimalign.Cohesion.Web.Rewrite/src/RewriteOptions.cs` and `cohesion/resources/Web/Assimalign.Cohesion.Web.Rewrite/src/Abstractions/IRewriteContext.cs`.
- **Decision record** — `cohesion/docs/resources/Web/DECISIONS.md` (ADR 1).
- **Tests** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Rewrite/tests/RewriteEndToEndTests.cs` and `cohesion/resources/Web/Assimalign.Cohesion.Web.Rewrite/tests/RewriteRuleEvaluationTests.cs`.
- **NativeAOT guard** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Hosting/samples/Assimalign.Cohesion.Web.AotGuard/Program.cs`.
- **Middleware order** — `cohesion/docs/resources/Web/MIDDLEWARE_ORDER.md`.
