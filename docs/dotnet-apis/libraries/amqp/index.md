# Amqp

Advanced Message Queuing Protocol (AMQP) wire handling and connection bindings.

[Libraries](../index.md)

## Packages

| Package | Responsibility | Reference |
|---|---|---|
| `Assimalign.Cohesion.Amqp` | Reserves the Advanced Message Queuing Protocol (AMQP) session and messaging layer. | [Overview](assimalign-cohesion-amqp/index.md) |
| `Assimalign.Cohesion.Amqp.Connections` | Negotiates AMQP connections and encodes protocol frames and messages over Cohesion carriers. | [Overview](assimalign-cohesion-amqp-connections/index.md) |

## Dependencies and delivery

The repository places this area in delivery wave 3. The wave is a dependency-ordering guide, not a
release or completeness guarantee.

| Assembly | Declared dependencies |
|---|---|
| `Assimalign.Cohesion.Amqp` | No explicit Cohesion or package reference in the project file |
| `Assimalign.Cohesion.Amqp.Connections` | `Assimalign.Cohesion.Core` (CohesionProjectReference), `Assimalign.Cohesion.Connections` (CohesionProjectReference) |

## Sources

- **Source** — `cohesion/libraries/README.md`.

- **Source** — `cohesion/README.md`.

- **Source** — `cohesion/docs/programs/DELIVERY_ROADMAP.md`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Amqp/Assimalign.Cohesion.Amqp/src/Assimalign.Cohesion.Amqp.csproj`.

- **Source** — `cohesion/libraries/Amqp/Assimalign.Cohesion.Amqp/src`.

- **Source** — `cohesion/libraries/Amqp/Assimalign.Cohesion.Amqp.Connections/src/Assimalign.Cohesion.Amqp.Connections.csproj`.

- **Source** — `cohesion/libraries/Amqp/Assimalign.Cohesion.Amqp.Connections/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Amqp/Assimalign.Cohesion.Amqp.Connections/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Amqp/Assimalign.Cohesion.Amqp.Connections/src`.
