# Assimalign.Cohesion.OpenTelemetry design

Design decisions and ownership boundaries for `Assimalign.Cohesion.OpenTelemetry`.

> **Status:** Partial.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

The exporter is transport-only; Hosting.Telemetry owns the logging adapter. A bounded queue, bounded
retry, and bounded shutdown prevent a collector outage from owning host startup. Traces, metrics,
gRPC, and binary protobuf export remain deferred.

Emitters of traces and metrics use the BCL `ActivitySource` and `Meter`, starting with the Web
server's (`Assimalign.Cohesion.Web.Hosting`, #1064), so a trace or metric exporter would subscribe
through `ActivityListener` and `MeterListener` (#317).

## gRPC deferral

The Cohesion HTTP server has no `application/grpc` framing or service dispatch. Trailers no longer
stand in the way: decision 18 (the Http area's ADR 2, `cohesion/docs/libraries/Http/DECISIONS.md`)
ships response trailers on HTTP/2 and HTTP/3, so a handler could stage `grpc-status` on
`IHttpResponse.Trailers` there, while HTTP/1.1 keeps the unsupported collection. Request trailers are
read on all three versions. `LocalPlanController.CanRealize` already refuses gRPC probes.
Implementing gRPC still requires that separate server capability — message framing, serialization,
and service dispatch — not just a different content type, and gRPC hosting stays outside the
HTTP/Web program.

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
