# Logging

Structured logging contracts, factory composition, and console and debug sinks.

[Libraries](../index.md)

## Packages

| Package | Responsibility | Reference |
|---|---|---|
| `Assimalign.Cohesion.Logging` | Defines structured log entries, provider composition, filtering, enrichment, and scopes. | [Overview](assimalign-cohesion-logging/index.md) |
| `Assimalign.Cohesion.Logging.Console` | Writes structured log entries to configurable output and error writers. | [Overview](assimalign-cohesion-logging-console/index.md) |
| `Assimalign.Cohesion.Logging.Debug` | Writes structured log entries to diagnostic debug output. | [Overview](assimalign-cohesion-logging-debug/index.md) |

## Dependencies and delivery

The repository places this area in delivery wave 2. The wave is a dependency-ordering guide, not a
release or completeness guarantee.

| Assembly | Declared dependencies |
|---|---|
| `Assimalign.Cohesion.Logging` | `Assimalign.Cohesion.Core` (CohesionProjectReference) |
| `Assimalign.Cohesion.Logging.Console` | `Assimalign.Cohesion.Logging` (CohesionProjectReference) |
| `Assimalign.Cohesion.Logging.Debug` | `Assimalign.Cohesion.Logging` (CohesionProjectReference) |

## Sources

- **Source** — `cohesion/libraries/README.md`.

- **Source** — `cohesion/README.md`.

- **Source** — `cohesion/docs/programs/DELIVERY_ROADMAP.md`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging/src/Assimalign.Cohesion.Logging.csproj`.

- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging/src`.

- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging.Console/src/Assimalign.Cohesion.Logging.Console.csproj`.

- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging.Console/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging.Console/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging.Console/src`.

- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging.Debug/src/Assimalign.Cohesion.Logging.Debug.csproj`.

- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging.Debug/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging.Debug/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging.Debug/src`.
