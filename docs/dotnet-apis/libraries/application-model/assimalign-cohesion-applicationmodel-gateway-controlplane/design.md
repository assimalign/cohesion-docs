# Assimalign.Cohesion.ApplicationModel.Gateway.ControlPlane design

Design decisions and ownership boundaries for `Assimalign.Cohesion.ApplicationModel.Gateway.ControlPlane`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

The server publishes the gateway's `ApplicationExportDocument` rather than defining another export
model. Server state and listener lifetime are application-scoped. Resolver clients require explicit
credentials and the expected application trust key; issuer command grants are enforced before
dispatch.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.ApplicationModel`,
`Assimalign.Cohesion.ApplicationModel.Gateway`, `Assimalign.Cohesion.Connections.Tcp`,
`Assimalign.Cohesion.Core`, `Assimalign.Cohesion.Hosting.Resources`, `Assimalign.Cohesion.Http`,
`Assimalign.Cohesion.Http.Connections`, `Assimalign.Cohesion.IdentityModel`,
`Assimalign.Cohesion.IdentityModel.Token.JsonWebToken`, `Assimalign.Cohesion.Web.Routing`. The
[overview](index.md#dependencies) distinguishes project references, package references, shared
source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway.ControlPlane/src/Assimalign.Cohesion.ApplicationModel.Gateway.ControlPlane.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway.ControlPlane/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway.ControlPlane/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/ApplicationModel/README.md`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway.ControlPlane/src`.
