# ILoggerEnricher

Adds attributes to every log entry before fan-out.

[Assembly overview](index.md) · [Design](design.md) · [Examples](examples/index.md)

Namespace: `Assimalign.Cohesion.Logging`.

Assembly: `Assimalign.Cohesion.Logging`.

## Remarks

Adds attributes to every log entry before fan-out. Examples: machine identity, current
trace / span ids, environment name, runtime version.

## Method

- **Contract** — `entry` is the entry being enriched (immutable; provided for context).

- **Contract** — `attributes` is a mutable view on the attribute bag. New keys are added to the entry's
  final attribute payload.

## Rules

- **Contract** — Enrichers run in registration order.

- **Contract** — Enrichers MUST NOT overwrite keys supplied by the entry author; assignments through the
  mutable view to an existing key are silently dropped. This guarantees caller intent wins.

- **Contract** — Implementations must be thread-safe. Enrichment runs on the caller's thread; long-running
  or blocking work belongs elsewhere.

- **Contract** — Exceptions thrown by an enricher are swallowed; the entry still ships.

## Members

| Member | Responsibility |
|---|---|
| `Enrich(ILoggerEntry, IDictionary<string, object?>)` | Adds attributes before provider dispatch without overwriting author-supplied keys. |

## Sources

- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging/docs/Assembly/Assimalign.Cohesion.Logging/ILoggerEnricher/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging/src/Abstractions/ILoggerEnricher.cs`.
