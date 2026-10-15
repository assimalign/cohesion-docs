# ResourceTelemetry

Configure console logging and optional OpenTelemetry log export for a resource invocation.

[Assembly overview](index.md) · [Design](design.md) · [Examples](examples/index.md)

Namespace: `Assimalign.Cohesion.Hosting.Telemetry`.

Assembly: `Assimalign.Cohesion.Hosting.Telemetry`.

## Members

| Member | Responsibility |
|---|---|
| `IsEnabled(ResourceContext)` | Reads the invocation environment to determine whether resource telemetry is enabled. |
| `Configure(ResourceContext, ILoggerFactoryBuilder)` | Configures an existing builder without building it; the caller owns disposal. |
| `Configure(ResourceContext)` | Creates an owned `ILoggerFactory` when enabled, otherwise returns `null`. |
| `Configure(ResourceContext, ILoggerFactoryBuilder, out IHostService?)` | Configures an existing builder and returns a host lifetime service. |
| `Configure(ResourceContext, out IHostService?)` | Creates a factory and returns its host lifetime service when enabled. |

## Remarks

Configuration uses `ResourceContext.TryGetEnvironmentValue`, so both local and in-process gateways
read the invocation snapshot rather than process-wide environment variables. A gateway name and a
nonblank absolute HTTP or HTTPS telemetry endpoint enable export. An omitted protocol selects
`otlp-http`.

Register the returned lifetime service before log producers. Reverse-order `StopAsync` then drains
producers before the exporter. Existing builders are never built by `Configure`; the overloads
without a lifetime output leave factory disposal with the caller.

When disabled, configuration returns `false` or `null` without mutating an existing builder.
Invalid configuration raises `InvalidOperationException`; reserved gRPC raises
`NotSupportedException`; a null context raises `ArgumentNullException`. An unreachable collector
does not turn configuration into a startup network dependency.

## Sources

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting.Telemetry/docs/Assembly/Assimalign.Cohesion.Hosting.Telemetry/ResourceTelemetry/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting.Telemetry/src/ResourceTelemetry.cs`.
