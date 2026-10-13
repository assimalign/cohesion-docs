# Assimalign.Cohesion.Hosting.Telemetry design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Hosting.Telemetry`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

Configuration reads the invocation environment through `ResourceContext`, preserving isolation in
in-process gateways. Register the returned lifetime service before producers so reverse-order
shutdown drains producers first. Existing logging builders are configured but never built by the
adapter.

Traces and metrics need no Logging span or instrument primitive: emitters use the BCL
`ActivitySource` and `Meter`, starting with the Web server's (`Assimalign.Cohesion.Web.Server`, #1064),
and exporting them through an `ActivityListener`/`MeterListener` is the OpenTelemetry foundation's
remaining work (#317).

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Hosting`,
`Assimalign.Cohesion.Hosting.Resources`, `Assimalign.Cohesion.Logging`,
`Assimalign.Cohesion.Logging.Console`, `Assimalign.Cohesion.OpenTelemetry`. The
[overview](index.md#dependencies) distinguishes project references, package references, shared
source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting.Telemetry/src/Assimalign.Cohesion.Hosting.Telemetry.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting.Telemetry/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting.Telemetry/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting.Telemetry/src`.
