# LogSpace

LogSpace receives authenticated OpenTelemetry JSON logs and serves bounded queries over durable segments.

> **Status:** Partial. Log ingestion, segment storage, and paged queries work; retention, archival, traces, metrics, and protobuf remain deferred.

## What it is

LogSpace is the observability service area. Its enabled host accepts OpenTelemetry Protocol
(OTLP) logs over HTTPS and persists append-only segments. Query and management use credentials
distinct from emitter telemetry tokens.

`Assimalign.Cohesion.LogSpace.Telemetry` remains empty scaffolding. Producer logging integration
belongs to `Assimalign.Cohesion.Hosting.Telemetry` and the shared OpenTelemetry library.

See the [LogSpace API reference](../dotnet-apis/resources/log-space/index.md).

## Project map

| Assembly | Responsibility |
|---|---|
| [`Assimalign.Cohesion.LogSpace`](../dotnet-apis/resources/log-space/assimalign-cohesion-logspace/index.md) | Hosting-free application, builder, and context contracts. |
| [`Assimalign.Cohesion.LogSpace.ApplicationModel`](../dotnet-apis/resources/log-space/assimalign-cohesion-logspace-applicationmodel/index.md) | Typed resource, StatefulSet planner, and default control plane. |
| [`Assimalign.Cohesion.LogSpace.Hosting`](../dotnet-apis/resources/log-space/assimalign-cohesion-logspace-hosting/index.md) | OTLP receiver, append-only segment store, token verification, and paged query. |
| [`Assimalign.Cohesion.LogSpace.Telemetry`](../dotnet-apis/resources/log-space/assimalign-cohesion-logspace-telemetry/index.md) | Empty placeholder; producer integration lives in Hosting.Telemetry. |

## Hosting model

`LogSpaceApplication.CreateBuilder(args)` composes the ambient resource. An enabled `otlp`
endpoint registers the receiver and segment flush service; `query` serves management and
`GET /cohesion/v1/logs`. A plain unregistered host remains a collection of explicit services.

Both listeners use the shared endpoint certificate accessor. A `tls` Secret mount can supply
both PEM certificate bundles. Self-signed fallback is allowed only for loopback `Local`;
`Development` and `Production` require a materialized certificate even on loopback.
The `data` mount selects storage, falling back to `<content-root>/logs`.

## Application model

`AddLogSpace(manifest, options)` returns `ILogSpaceResourceDescriptor` over a `LogSpaceResource`.
The planner requires a `StatefulSet`, HTTPS `otlp` and `query` endpoints, and a sized `data`
Volume. SDK defaults are ports 4318 and 8443, with a 10 GiB claim at `/data`;
`Storage.Size` can override capacity. Both endpoints are private by default.

`LogSpaceResourceControlPlane.Create()` supplies health and management, not ingestion.
There are no accepted domain command kinds. Producers can explicitly `DependsOn` the sink to
ensure it is running before telemetry configuration is injected.

## SDK and framework

`Assimalign.Cohesion.Sdk.LogSpace` delivers the runtime family through
`Assimalign.Cohesion.App.LogSpace`. The area's `.ApplicationModel` package is NuGet-only
and added to enabled resource executables; it remains outside the runtime shared framework.

See the [LogSpace SDK reference](../dotnet-apis/sdks/sdk-log-space/index.md).

## Ingestion and authorization

`POST /v1/logs` requires `application/json` and an ES256 telemetry token whose subject identifies
the emitter. Each record's `service.name` must match that subject. The receiver bounds request
bodies to one MiB and accepts at most 8192 records per request. Protobuf returns HTTP 415;
trace and metric POST routes remain reserved and return 501 after validation.

Telemetry scope cannot authorize query, stop, or command operations. Those routes require the
sink's ordinary bootstrap or developer credential. Outbound exporters use the delivered trust
bundle rather than bypassing certificate validation.

## Storage and query

Receiver threads enqueue records into a bounded channel. A dedicated flush service writes and
synchronizes append-only `logs-YYYYMMDD-NNNN.ndjson` segments every 250 milliseconds and on stop.
Segments rotate daily or at 16 MiB; receivers stop before the final flush. Storage failures mark
the store unavailable and yield HTTP 503 while retaining queued entries for a later flush.

| Query input | Meaning |
|---|---|
| `resource` | Filter the emitting service. |
| `since` | ISO-8601 timestamp filter. |
| `limit` | Page size, default 100 and maximum 1000. |
| `cursor` | Validated segment/line continuation bound to the filters. |

Responses use newline-delimited JSON (`application/x-ndjson`). `X-Cohesion-Next-Cursor` reports a
continuation when more scanning is possible. Pages are bounded to four million characters plus
at most one bounded record. The query scans segments; sidecar indexes do not yet accelerate it.
Retention, crash-tail repair, archival, and Database embedded storage are deferred.

## Getting started

The landing-zone LogSpace template supplies this host entry point.

```csharp
using Assimalign.Cohesion.LogSpace;
using Assimalign.Cohesion.LogSpace.Hosting;

LogSpaceApplicationBuilder builder = LogSpaceApplication.CreateBuilder(args);

await using LogSpaceApplication application = builder.Build();
await application.RunAsync();
```

Use an enabled resource manifest to activate the ingestion and query listeners; provide the
required data and certificate inputs for the selected environment.

Return to [Cohesion Documentation](../index.md).

## Sources

- **Area** — `cohesion/resources/LogSpace/README.md`.
- **Hosting** — `cohesion/resources/LogSpace/Assimalign.Cohesion.LogSpace.Hosting/docs/OVERVIEW.md` and `cohesion/resources/LogSpace/Assimalign.Cohesion.LogSpace.Hosting/docs/DESIGN.md`.
- **Application model** — `cohesion/resources/LogSpace/Assimalign.Cohesion.LogSpace.ApplicationModel/docs/OVERVIEW.md` and `cohesion/resources/LogSpace/Assimalign.Cohesion.LogSpace.ApplicationModel/docs/DESIGN.md`.
- **Runtime contract** — `cohesion/docs/RUNTIME_CONTRACT.md`.
- **Package boundaries** — `cohesion/.claude/rules/resource-areas.md`.
- **Supporting source** — `cohesion/resources/LogSpace/Assimalign.Cohesion.LogSpace.Hosting/docs/DESIGN.md`.
- **Supporting source** — `cohesion/tooling/templates/Assimalign.Cohesion.Templates/src/content/cohesion-landing-zone/Platform/Example.Platform.LogSpace/Program.cs`.
