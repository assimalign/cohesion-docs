# Assimalign.Cohesion.Web.Hosting.Health

The hosting-family adapter extends `IHealthChecksBuilder` with `AddContributor`.

> **Status:** Partial.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.

The hosting-family adapter extends `IHealthChecksBuilder` with `AddContributor`. It references
`Assimalign.Cohesion.Web.Health` and `Assimalign.Cohesion.Hosting.Health`. The Web health model and
HTTP endpoints remain in `Web.Health`.

## Usage

See the [source-backed usage examples](examples/index.md).

The contributor name identifies the check. Omitted tags include readiness and liveness; explicit
tags replace those defaults. Failure status and timeout use Web health policy. The application owns
contributor lifetime; the adapter only evaluates it.

`App.Web` exposes this package. `Database.Hosting` consumes it through a private project reference
paired with the adapter assembly in the `App.Database` runtime pack. See [Design](design.md) for the
dependency and cancellation contracts.

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Hosting.Health` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Web.Health` | `CohesionProjectReference` |

[Parent: Web](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Hosting.Health/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Hosting.Health/src/Assimalign.Cohesion.Web.Hosting.Health.csproj`.
