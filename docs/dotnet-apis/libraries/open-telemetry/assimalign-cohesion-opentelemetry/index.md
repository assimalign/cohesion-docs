# Assimalign.Cohesion.OpenTelemetry

Exports bounded OpenTelemetry Protocol (OTLP) logs over HTTP using JSON.

> **Status:** Partial.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[OpenTelemetry](../index.md)

## Scope

The exporter is transport-only; Hosting.Telemetry owns the logging adapter. A bounded queue, bounded
retry, and bounded shutdown prevent a collector outage from owning host startup. Traces, metrics,
gRPC, and binary protobuf export remain deferred.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Core`](../../core/assimalign-cohesion-core/index.md) | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `IOtlpLogExporter` | `src/Abstractions/IOtlpLogExporter.cs` |
| `OtlpExporter` | `src/OtlpExporter.cs` |
| `OtlpExporterOptions` | `src/OtlpExporterOptions.cs` |
| `OtlpLogRecord` | `src/OtlpLogRecord.cs` |
| `OtlpProtocol` | `src/OtlpProtocol.cs` |
| `OtlpSignal` | `src/OtlpSignal.cs` |

## Sources

- **Source** — `cohesion/libraries/OpenTelemetry/Assimalign.Cohesion.OpenTelemetry/src/Assimalign.Cohesion.OpenTelemetry.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/OpenTelemetry/Assimalign.Cohesion.OpenTelemetry/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/OpenTelemetry/Assimalign.Cohesion.OpenTelemetry/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/OpenTelemetry/README.md`.

- **Source** — `cohesion/libraries/OpenTelemetry/Assimalign.Cohesion.OpenTelemetry/src`.

- **Source** — `cohesion/libraries/OpenTelemetry/Assimalign.Cohesion.OpenTelemetry/src/Abstractions/IOtlpLogExporter.cs`.

- **Source** — `cohesion/libraries/OpenTelemetry/Assimalign.Cohesion.OpenTelemetry/src/OtlpExporter.cs`.

- **Source** — `cohesion/libraries/OpenTelemetry/Assimalign.Cohesion.OpenTelemetry/src/OtlpExporterOptions.cs`.

- **Source** — `cohesion/libraries/OpenTelemetry/Assimalign.Cohesion.OpenTelemetry/src/OtlpLogRecord.cs`.

- **Source** — `cohesion/libraries/OpenTelemetry/Assimalign.Cohesion.OpenTelemetry/src/OtlpProtocol.cs`.

- **Source** — `cohesion/libraries/OpenTelemetry/Assimalign.Cohesion.OpenTelemetry/src/OtlpSignal.cs`.
