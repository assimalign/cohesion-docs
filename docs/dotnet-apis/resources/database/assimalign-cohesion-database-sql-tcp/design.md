# Assimalign.Cohesion.Database.Sql.Tcp design

This page describes the boundaries and implementation shape of `Assimalign.Cohesion.Database.Sql.Tcp`.

> **Status:** Implemented.

## Ownership

Transport selection belongs to the composition root. This optional integration references SQL and
Connections.Tcp; SQL never references this integration or a concrete transport. `App.Database`
includes the integration for SDK consumers.

The dependencies follow the direction shown here.

```mermaid
flowchart LR
    Integration["Database.Sql.Tcp"] --> Sql["Database.Sql"]
    Integration --> Tcp["Connections.Tcp"]
    Sql --> Connections["Connections"]
```

## Endpoint behavior

`Listen(Uri)` retains its namespace, signature, IP-literal/localhost/wildcard resolution,
validation, and listener ownership behavior. Referencing this package is the source migration; no
SQL engine API chooses a transport. The existing SQL server-options tests exercise the moved
extension unchanged. No DNS lookup, reflection, Microsoft.Extensions dependencies, or runtime code
generation is used.

## Declared dependencies

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Database.Sql` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Connections.Tcp` | `CohesionProjectReference` |

[Assembly overview](index.md) · [Examples](examples/index.md)

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Tcp/docs/DESIGN.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Tcp/src/Assimalign.Cohesion.Database.Sql.Tcp.csproj`.
