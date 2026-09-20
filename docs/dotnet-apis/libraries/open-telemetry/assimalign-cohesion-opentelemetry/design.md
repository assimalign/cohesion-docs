# Assimalign.Cohesion.OpenTelemetry design

Design decisions and ownership boundaries for `Assimalign.Cohesion.OpenTelemetry`.

> **Status:** Partial.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

The exporter is transport-only; Hosting.Telemetry owns the logging adapter. A bounded queue, bounded
retry, and bounded shutdown prevent a collector outage from owning host startup. Traces, metrics,
gRPC, and binary protobuf export remain deferred.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Core`. The [overview](index.md#dependencies)
distinguishes project references, package references, shared source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/OpenTelemetry/Assimalign.Cohesion.OpenTelemetry/src/Assimalign.Cohesion.OpenTelemetry.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/OpenTelemetry/Assimalign.Cohesion.OpenTelemetry/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/OpenTelemetry/Assimalign.Cohesion.OpenTelemetry/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/OpenTelemetry/README.md`.

- **Source** — `cohesion/libraries/OpenTelemetry/Assimalign.Cohesion.OpenTelemetry/src`.
