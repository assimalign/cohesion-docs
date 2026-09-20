# Assimalign.Cohesion.Hosting.Telemetry

Adapts resource logging to console output and OpenTelemetry log export.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

- **[`ResourceTelemetry`](resource-telemetry.md)** — type reference.

[Hosting](../index.md)

## Scope

Configuration reads the invocation environment through `ResourceContext`, preserving isolation in
in-process gateways. Register the returned lifetime service before producers so reverse-order
shutdown drains producers first. Existing logging builders are configured but never built by the
adapter.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Hosting`](../../hosting/assimalign-cohesion-hosting/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.Hosting.Resources`](../../hosting/assimalign-cohesion-hosting-resources/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.Logging`](../../logging/assimalign-cohesion-logging/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.Logging.Console`](../../logging/assimalign-cohesion-logging-console/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.OpenTelemetry`](../../open-telemetry/assimalign-cohesion-opentelemetry/index.md) | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `ResourceTelemetry` | `src/ResourceTelemetry.cs` |

## Sources

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting.Telemetry/src/Assimalign.Cohesion.Hosting.Telemetry.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting.Telemetry/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting.Telemetry/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting.Telemetry/src`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting.Telemetry/src/ResourceTelemetry.cs`.
