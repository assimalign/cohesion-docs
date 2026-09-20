# Assimalign.Cohesion.ApplicationModel.Gateway.InProcess design

Design decisions and ownership boundaries for `Assimalign.Cohesion.ApplicationModel.Gateway.InProcess`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

The gateway invokes real resource entry points with isolated ambient `ResourceContext` values. The
consuming Composite supplies resource runtimes through project references; this package does not
reference area hosting modules. Offline rendering produces plan units without invoking entry points
or resolving runtime inputs.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.ApplicationModel`,
`Assimalign.Cohesion.ApplicationModel.Gateway`, `Assimalign.Cohesion.Hosting`,
`Assimalign.Cohesion.Hosting.Resources`. The [overview](index.md#dependencies) distinguishes project
references, package references, shared source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway.InProcess/src/Assimalign.Cohesion.ApplicationModel.Gateway.InProcess.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway.InProcess/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway.InProcess/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/ApplicationModel/README.md`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway.InProcess/src`.
