# Hosting

Plain host execution: the lifecycle coordinator, hosted services, and the complete run.

[Libraries](../index.md)

## Packages

| Package | Responsibility | Reference |
|---|---|---|
| `Assimalign.Cohesion.Hosting` | Coordinates plain host and hosted-service lifecycles. | [Overview](assimalign-cohesion-hosting/index.md) |

## Dependencies and delivery

The repository places this area in delivery wave 4. The wave is a dependency-ordering guide, not a
release or completeness guarantee.

| Assembly | Declared dependencies |
|---|---|
| `Assimalign.Cohesion.Hosting` | `Assimalign.Cohesion.Core` (CohesionProjectReference) |

The area is one package. Resource management is not a hosting concern: a resource instance joins
its host through [`Assimalign.Cohesion.ApplicationModel.Hosting`](../application-model/assimalign-cohesion-applicationmodel-hosting/index.md),
and a Web application serves health through
[`Assimalign.Cohesion.Web.Health`](../../resources/web/assimalign-cohesion-web-health/index.md).

## Sources

- **Source** — `cohesion/libraries/README.md`.
- **Source** — `cohesion/docs/programs/DELIVERY_ROADMAP.md`.
- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting/src/Assimalign.Cohesion.Hosting.csproj`.
- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting/docs/OVERVIEW.md`.
- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting/docs/DESIGN.md`.
