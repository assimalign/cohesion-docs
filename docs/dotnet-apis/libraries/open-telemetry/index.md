# OpenTelemetry

Bounded OpenTelemetry Protocol (OTLP) log export over HTTP with JSON.

[Libraries](../index.md)

## Packages

| Package | Responsibility | Reference |
|---|---|---|
| `Assimalign.Cohesion.OpenTelemetry` | Exports bounded OpenTelemetry Protocol (OTLP) logs over HTTP using JSON. | [Overview](assimalign-cohesion-opentelemetry/index.md) |

## Dependencies and delivery

The repository places this area in delivery wave 3. The wave is a dependency-ordering guide, not a
release or completeness guarantee.

The root wave diagram groups OpenTelemetry with protocol and format work. Its current area README
describes the log exporter as a Layer 2 transport library; Hosting.Telemetry owns the logging
adapter.

| Assembly | Declared dependencies |
|---|---|
| `Assimalign.Cohesion.OpenTelemetry` | `Assimalign.Cohesion.Core` (CohesionProjectReference) |

## Sources

- **Source** — `cohesion/libraries/README.md`.

- **Source** — `cohesion/README.md`.

- **Source** — `cohesion/docs/programs/DELIVERY_ROADMAP.md`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/OpenTelemetry/README.md`.

- **Source** — `cohesion/libraries/OpenTelemetry/Assimalign.Cohesion.OpenTelemetry/src/Assimalign.Cohesion.OpenTelemetry.csproj`.

- **Source** — `cohesion/libraries/OpenTelemetry/Assimalign.Cohesion.OpenTelemetry/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/OpenTelemetry/Assimalign.Cohesion.OpenTelemetry/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/OpenTelemetry/Assimalign.Cohesion.OpenTelemetry/src`.
