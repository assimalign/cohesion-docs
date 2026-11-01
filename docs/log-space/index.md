# LogSpace

LogSpace is the observability resource area for OpenTelemetry log ingestion and bounded queries over durable segments.

> **Status:** Partial. The OTLP receiver, segment store, and paged query exist in the hosting module, but the host registers none of them; retention, archival, traces, metrics, and protobuf remain deferred.

## What it is

LogSpace is the observability service area. Its hosting module carries an OpenTelemetry Protocol
(OTLP) log receiver over HTTPS, an append-only segment store, and a paged query.
`LogSpaceApplicationBuilder.Build` registers only the application's own services, so a LogSpace
host does not ingest or serve logs until the area wires them.

`Assimalign.Cohesion.LogSpace.Telemetry` remains empty scaffolding.

See the [LogSpace API reference](../dotnet-apis/resources/log-space/index.md).

## Project map

| Assembly | Responsibility |
|---|---|
| [`Assimalign.Cohesion.LogSpace`](../dotnet-apis/resources/log-space/assimalign-cohesion-logspace/index.md) | Hosting-free application, builder, and context contracts. |
| [`Assimalign.Cohesion.LogSpace.Hosting`](../dotnet-apis/resources/log-space/assimalign-cohesion-logspace-hosting/index.md) | Host lifecycle, plus the internal OTLP receiver, segment store, and paged query. |
| [`Assimalign.Cohesion.LogSpace.Hosting.ApplicationModel`](../dotnet-apis/resources/log-space/assimalign-cohesion-logspace-hosting-applicationmodel/index.md) | `builder.Services.AddResource(name, ...)`: the sink as a managed resource instance. |
| [`Assimalign.Cohesion.LogSpace.Telemetry`](../dotnet-apis/resources/log-space/assimalign-cohesion-logspace-telemetry/index.md) | Empty placeholder. |

## Hosting model

`LogSpaceApplication.CreateBuilder(args)` returns the concrete builder, whose `Services` container
holds the application's host services. `Build` closes registration and resolves them once; services
start in registration order and stop in reverse.

## Managed resource

A sink becomes a managed resource instance with one registration in its `Program.cs`:
`builder.Services.AddResource("logs", resource => resource.UseInProcess())`. When the environment
names a platform the resource opted into, it publishes its manifest, restores the state its gateway
kept, and runs the gateway's actions. LogSpace exposes no actions of its own yet. A gateway declares
the resource generically: `builder.AddResource("LogSpace", "logs")`. See
[ApplicationModel](../dotnet-apis/libraries/application-model/index.md).

## SDK and framework

`Assimalign.Cohesion.Sdk.LogSpace` delivers the runtime family and the resource integration through
`Assimalign.Cohesion.App.LogSpace`. See the
[LogSpace SDK reference](../dotnet-apis/sdks/sdk-log-space/index.md).

## Receiver and query contracts

These contracts describe the hosting module's internal services and their tests; they apply once
the services are registered.

- **Ingestion** — `POST /v1/logs` requires `application/json`. Request bodies are bounded to one MiB
  and at most 8192 records. Protobuf returns HTTP 415; trace and metric POST routes are reserved and
  return 501 after validation.
- **Storage** — Receiver threads enqueue records into a bounded channel. A dedicated flush service
  writes and synchronizes append-only `logs-YYYYMMDD-NNNN.ndjson` segments every 250 milliseconds and
  on stop. Segments rotate daily or at 16 MiB. Storage failures yield HTTP 503 while retaining
  queued entries.
- **Query** — `GET /cohesion/v1/logs` accepts only transport peers at `127.0.0.1`, `::1`, or
  IPv4-mapped `127.0.0.1`; forwarded headers never grant access.

| Query input | Meaning |
|---|---|
| `resource` | Filter the emitting service. |
| `since` | ISO-8601 timestamp filter. |
| `limit` | Page size, default 100 and maximum 1000. |
| `cursor` | Validated segment/line continuation bound to the filters. |

Responses use newline-delimited JSON (`application/x-ndjson`), and `X-Cohesion-Next-Cursor` reports
a continuation. Pages are bounded to four million characters plus at most one bounded record.

## Getting started

```csharp
using Assimalign.Cohesion.ApplicationModel.Hosting.InProcess;
using Assimalign.Cohesion.LogSpace.Hosting;
using Assimalign.Cohesion.LogSpace.Hosting.ApplicationModel;

LogSpaceApplicationBuilder builder = LogSpaceApplication.CreateBuilder(args);
builder.Services.AddResource("logs", resource => resource.UseInProcess());

await using LogSpaceApplication application = builder.Build();
await application.RunAsync();
```

Return to [Cohesion Documentation](../index.md).

## Sources

- **Area** — `cohesion/resources/LogSpace/README.md`.
- **Hosting** — `cohesion/resources/LogSpace/Assimalign.Cohesion.LogSpace.Hosting/docs/OVERVIEW.md` and `cohesion/resources/LogSpace/Assimalign.Cohesion.LogSpace.Hosting/docs/DESIGN.md`.
- **Builder** — `cohesion/resources/LogSpace/Assimalign.Cohesion.LogSpace.Hosting/src/LogSpaceApplicationBuilder.cs`.
- **Resource integration** — `cohesion/resources/LogSpace/Assimalign.Cohesion.LogSpace.Hosting.ApplicationModel/docs/OVERVIEW.md`.
