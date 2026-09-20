# Assimalign.Cohesion.Database.Embedded

The in-process consumption facade for Cohesion database engines: other resources (configuration stores, secret stores, schedulers, hubs) embed a data layer by composing model engines directly inside their process — no server, no wire protocol, no host — with the same engines and ACID guarantees as the hosted mode.

> **Status:** Partial.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.

The in-process consumption facade for Cohesion database engines: other resources (configuration
stores, secret stores, schedulers, hubs) embed a data layer by composing model engines directly
inside their process — no server, no wire protocol, no host — with the same engines and ACID
guarantees as the hosted mode.

See the [source-backed usage examples](examples/index.md).

## Scope

- **`EmbeddedDatabase`** — engine composition, lookup by name or model, reverse-order disposal (implemented, tested)
- **`EmbeddedDatabaseOptions`** — engine registration

## Dependencies

- **`Assimalign.Cohesion.Database` (contract root)** — and nothing else. Consumers add references to the model engines they embed.

## Consumers

Any Cohesion resource that needs a durable data layer without running a separate database service.
The hosted counterpart is `Database.Hosting` (engines + wire-protocol server in a standalone
process).

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Database` | `CohesionProjectReference` |

[Parent: Database](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Embedded/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Embedded/src/Assimalign.Cohesion.Database.Embedded.csproj`.
