# Assimalign.Cohesion.Web.Hosting.Resources

Install the resource protocol on a private Web application's pipeline:.

> **Status:** Partial.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.

Install the resource protocol on a private Web application's pipeline:

See the [source-backed usage examples](examples/index.md).

Install before application middleware. An optional trailing port gates every route; null serves
every listener. Hosts composing manually call
`ResourceControlPlaneMiddleware.Validate(resourceContext)` and `InvokeAsync(...)`. Stop uses the
Web root response-completion feature to send its acknowledgement before requesting shutdown. The
feature serves health, readiness, liveness, endpoint discovery, stop, and command envelopes.
Gateway-managed namespaced routes authenticate against the published application trust key;
standalone resources work without a gateway. Domain command handlers remain the owning area's
responsibility.

This hosting-family library references the Web root, Hosting.Resources, Hosting.Health, and
`IdentityModel.Token.JsonWebToken`. It never references Web.Hosting. Roots and feature libraries
cannot reference it; other areas' hosting modules consume it privately. App.Web exposes it publicly.

See [DESIGN.md](design.md) for protocol, authentication, ownership, and parity constraints.

## Manual composition API

`ResourceControlPlaneMiddleware.Validate(ResourceContext resourceContext)` performs the verb's eager
managed-identity validation. It rejects a null context, a blank managed resource name, a missing
application identity or public trust key, and an invalid P-256 JWK. Standalone contexts need no
identity.

`ResourceControlPlaneMiddleware.InvokeAsync(IResourceControlPlane controlPlane, ResourceContext resourceContext, bool isApplicationReady, int? controlPlanePort, IHttpContext context, WebApplicationMiddleware next)`
returns a `Task` for one exchange. Required reference arguments cannot be null. A mismatched
non-null port forwards every path to `next`; null applies no port gate. Unknown namespaced paths
authenticate before returning 404.

Call `Validate` during composition, then register `InvokeAsync` ahead of routes the terminal should
own. The owning host supplies readiness and listener selection. Stop registers a callback on
`IWebResponseCompletionFeature` when present; the callback requests stop with
`CancellationToken.None` after transmission. A custom server without the feature uses the request
token for the direct-stop fallback.

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Web` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Hosting.Resources` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Hosting.Health` | `CohesionProjectReference` |
| `Assimalign.Cohesion.IdentityModel.Token.JsonWebToken` | `CohesionProjectReference` |

[Parent: Web](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Hosting.Resources/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Hosting.Resources/src/Assimalign.Cohesion.Web.Hosting.Resources.csproj`.
