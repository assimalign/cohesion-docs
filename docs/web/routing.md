# Routing

Web routing matches request paths and methods while exposing typed route values and endpoint metadata.

> **Status:** Implemented.

## Composition

`Assimalign.Cohesion.Web.Routing` owns patterns, constraints, groups, metadata, matching, and
outbound links. Call `AddRouting` during builder composition and `UseRouting` on the application
pipeline. Both use the same application-local `IRouterFeature`; route state is not process-wide.
`UseRouting` fails if routing was not added first.

Endpoint mapping lives in `Assimalign.Cohesion.Web.Api`; see
[Endpoints and responses](endpoints.md). Register middleware that must surround an endpoint before
the routing terminal: a successful routing match invokes the selected handler without continuing
to downstream middleware.

## Matching and precedence

Matching evaluates the path and constraints before the request method. That distinction preserves
Hypertext Transfer Protocol (HTTP) method semantics:

| Match result | Pipeline behavior |
|---|---|
| `Matched` | Publish `IRouteMatchFeature` and invoke the endpoint as a terminal. |
| `MethodNotAllowed` | Return 405 with `Allow`; do not fall through. |
| `NoMatch` | Invoke the next middleware, allowing the eventual 404 path. |

`RoutePattern` computes inbound precedence. Candidates sort from literal segments through
constrained parameters, ordinary parameters, constrained catch-alls, and ordinary catch-alls.
For equal pattern precedence, host-constrained routes rank first, followed by registration order.
For example, `/api/status` outranks `/api/{id}`, regardless of registration order.

## Constraints and route values

Inline policies such as `{id:int}` and `{id:range(1,10)}` resolve through
`RouteParameterPolicyMap`. Unknown policies fail during route construction. A failed policy is a
path mismatch for that route, allowing a more general route to match.

Constraints apply when a value is present; they do not make an optional value mandatory.
`/api/items/{id:int?}` accepts `/api/items` and `/api/items/7`, but not `/api/items/abc`.
`RouteParameterPolicy` is the public extension point; built-in implementations are exposed by name.

## Metadata, groups, and links

`IRouterRouteMetadataCollection` carries endpoint metadata without reflection. Consumers use
`IRouteMatchFeature` to inspect the selected route and its values. `RouteHostMetadata` constrains
host matching through `RouteHostConstraint`.

`MapGroup` creates an `IRouterGroupBuilder` combining a path prefix, shared parameter policies,
and metadata with child routes. Named routes use `RouteNameMetadata`; `ILinkGenerator` produces
outbound paths or absolute URIs from route names and values. These are builder-time declarations
on the same router, rather than a separate registry.

Return to [Web](index.md).

## Sources

- **Routing contract** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Routing/docs/DESIGN.md`.
- **Endpoint integration** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Api/docs/OVERVIEW.md`.
