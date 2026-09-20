# Hosting

Plain host execution, opt-in resource supervision, health, and telemetry composition.

[Libraries](../index.md)

## Packages

| Package | Responsibility | Reference |
|---|---|---|
| `Assimalign.Cohesion.Hosting` | Coordinates plain host and hosted-service lifecycles. | [Overview](assimalign-cohesion-hosting/index.md) |
| `Assimalign.Cohesion.Hosting.Health` | Defines transport-neutral health contributions and aggregate report values. | [Overview](assimalign-cohesion-hosting-health/index.md) |
| `Assimalign.Cohesion.Hosting.Resources` | Adds opt-in resource context and supervisor behavior to plain hosts. | [Overview](assimalign-cohesion-hosting-resources/index.md) |
| `Assimalign.Cohesion.Hosting.Telemetry` | Adapts resource logging to console output and OpenTelemetry log export. | [Overview](assimalign-cohesion-hosting-telemetry/index.md) |

## Dependencies and delivery

The repository places this area in delivery wave 4. The wave is a dependency-ordering guide, not a
release or completeness guarantee.

| Assembly | Declared dependencies |
|---|---|
| `Assimalign.Cohesion.Hosting` | `Assimalign.Cohesion.Core` (CohesionProjectReference) |
| `Assimalign.Cohesion.Hosting.Health` | `Assimalign.Cohesion.Core` (CohesionProjectReference) |
| `Assimalign.Cohesion.Hosting.Resources` | `Assimalign.Cohesion.Core` (CohesionProjectReference), `Assimalign.Cohesion.Hosting` (CohesionProjectReference), `Assimalign.Cohesion.Hosting.Health` (CohesionProjectReference), `System.Security.Cryptography.ProtectedData` (CohesionPackageReference) |
| `Assimalign.Cohesion.Hosting.Telemetry` | `Assimalign.Cohesion.Hosting` (CohesionProjectReference), `Assimalign.Cohesion.Hosting.Resources` (CohesionProjectReference), `Assimalign.Cohesion.Logging` (CohesionProjectReference), `Assimalign.Cohesion.Logging.Console` (CohesionProjectReference), `Assimalign.Cohesion.OpenTelemetry` (CohesionProjectReference) |

## Sources

- **Source** — `cohesion/libraries/README.md`.

- **Source** — `cohesion/README.md`.

- **Source** — `cohesion/docs/programs/DELIVERY_ROADMAP.md`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting/src/Assimalign.Cohesion.Hosting.csproj`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting/src`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting.Health/src/Assimalign.Cohesion.Hosting.Health.csproj`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting.Health/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting.Health/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting.Health/src`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting.Resources/src/Assimalign.Cohesion.Hosting.Resources.csproj`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting.Resources/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting.Resources/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting.Resources/src`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting.Telemetry/src/Assimalign.Cohesion.Hosting.Telemetry.csproj`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting.Telemetry/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting.Telemetry/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting.Telemetry/src`.
