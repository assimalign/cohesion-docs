# Scheduler

Scheduler binds declared jobs to cron or fixed-delay timer providers and runs their schedules.

> **Status:** Partial. Local schedule execution and graceful drain work; persistence and distributed coordination remain deferred.

## What it is

Scheduler is an executable job-scheduling resource. `AddJob` creates dormant work; it executes
only after a provider binds it with `AddCronSchedule` or `AddTimerSchedule`. Different registered
schedules have concurrent evaluation loops. Declaring a job alone never runs it.

See the [Scheduler API reference](../dotnet-apis/resources/scheduler/index.md).

## Project map

| Assembly | Responsibility |
|---|---|
| [`Assimalign.Cohesion.Scheduler`](../dotnet-apis/resources/scheduler/assimalign-cohesion-scheduler/index.md) | Application, job, schedule, provider, and context contracts. |
| [`Assimalign.Cohesion.Scheduler.Cron`](../dotnet-apis/resources/scheduler/assimalign-cohesion-scheduler-cron/index.md) | Five-field cron parsing and occurrence evaluation. |
| [`Assimalign.Cohesion.Scheduler.Hosting`](../dotnet-apis/resources/scheduler/assimalign-cohesion-scheduler-hosting/index.md) | Schedule execution and host lifecycle. |
| [`Assimalign.Cohesion.Scheduler.Hosting.ApplicationModel`](../dotnet-apis/resources/scheduler/assimalign-cohesion-scheduler-hosting-applicationmodel/index.md) | `builder.Services.AddResource(name, ...)`: the scheduler as a managed resource instance. |
| [`Assimalign.Cohesion.Scheduler.Timer`](../dotnet-apis/resources/scheduler/assimalign-cohesion-scheduler-timer/index.md) | Fixed-delay timer occurrences. |

## Hosting model

`SchedulerApplication.CreateBuilder(args)` returns the concrete builder. `Build()` validates
provider bindings against the exact job instances in the declaration registry, materializes
explicit host services, and adds the schedule executor last.

Stopping cancels trigger waits and prevents new occurrences, then joins active work within the
shared host shutdown budget. It does not cancel an already executing occurrence merely because
the next trigger was cancelled.

## Managed resource

A scheduler becomes a managed resource instance with one registration in its `Program.cs`:
`builder.Services.AddResource("jobs", resource => resource.UseInProcess())`. When the environment
names a platform the resource opted into, it publishes its manifest, restores the state its gateway
kept, and runs the gateway's actions. Scheduler exposes no actions of its own yet; the application
can add its own with `AddAction`. A gateway declares the resource generically:
`builder.AddResource("Scheduler", "jobs")`. Horizontal scaling still awaits distributed occurrence
coordination, so run one instance. See [ApplicationModel](../dotnet-apis/libraries/application-model/index.md).

## SDK and framework

`Assimalign.Cohesion.Sdk.Scheduler` delivers the runtime family and the resource integration
through `Assimalign.Cohesion.App.Scheduler`. See the
[Scheduler SDK reference](../dotnet-apis/sdks/sdk-scheduler/index.md).

## Jobs and providers

`ISchedulerApplicationContext` exposes read-only `Jobs` and `ScheduleProviders` snapshots.
`ISchedule` exposes occurrence timestamps, status, priority, retry metadata, and bound jobs.
Retry metadata does not imply implemented retry-policy execution.

| Provider | Timing contract |
|---|---|
| Cron | Exactly five fields: minute, hour, day of month, month, day of week. Seconds and year are unsupported. |
| Timer | Wait for the due time, run bound enabled jobs sequentially, then wait the fixed interval after completion. |

Cron supports wildcards, lists, inclusive ranges, and positive steps. Sunday is 0 or 7. Restricted
day-of-month and day-of-week fields use OR semantics; when either is wildcard, the restricted
field governs. Occurrence lookup uses an exclusive lower bound and minute precision.

Timer's explicit overload accepts a first due time, positive interval, and `TimeProvider`.
Fixed delay prevents overlap within one schedule. Persistence, retry execution, misfire handling,
leader election, and distributed ownership remain future work.

## Getting started

This example extends the area README's host entry point with a timer-bound job. The simple
`AddTimerSchedule` overload uses the interval as both the initial delay and the delay after
each completed occurrence.

```csharp
using System;
using System.Threading.Tasks;

using Assimalign.Cohesion.Scheduler;
using Assimalign.Cohesion.Scheduler.Hosting;
using Assimalign.Cohesion.Scheduler.Timer;

SchedulerApplicationBuilder builder = SchedulerApplication.CreateBuilder(args);

IScheduleJob job = builder.AddJob("heartbeat", (context, cancellationToken) =>
{
    Console.WriteLine("Scheduled occurrence");
    return ValueTask.CompletedTask;
});
builder.AddTimerSchedule(TimeSpan.FromMinutes(1), job);

await using SchedulerApplication application = builder.Build();
await application.RunAsync();
```

Return to [Cohesion Documentation](../index.md).

## Sources

- **Area** — `cohesion/resources/Scheduler/README.md`.
- **Hosting** — `cohesion/resources/Scheduler/Assimalign.Cohesion.Scheduler.Hosting/docs/OVERVIEW.md` and `cohesion/resources/Scheduler/Assimalign.Cohesion.Scheduler.Hosting/docs/DESIGN.md`.
- **Resource integration** — `cohesion/resources/Scheduler/Assimalign.Cohesion.Scheduler.Hosting.ApplicationModel/docs/OVERVIEW.md`.
- **Runtime contract** — `cohesion/docs/RUNTIME_CONTRACT.md`.
- **Package boundaries** — `cohesion/.claude/rules/resource-areas.md`.
- **Supporting source** — `cohesion/resources/Scheduler/Assimalign.Cohesion.Scheduler/docs/OVERVIEW.md`.
- **Supporting source** — `cohesion/resources/Scheduler/Assimalign.Cohesion.Scheduler.Cron/docs/DESIGN.md`.
- **Supporting source** — `cohesion/resources/Scheduler/Assimalign.Cohesion.Scheduler.Timer/docs/DESIGN.md`.
- **Supporting source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Tasks/docs/OVERVIEW.md`.
- **Job declaration** — `cohesion/resources/Scheduler/Assimalign.Cohesion.Scheduler/src/Extensions/SchedulerJobExtensions.cs`.
- **Timer binding** — `cohesion/resources/Scheduler/Assimalign.Cohesion.Scheduler.Timer/src/Extensions/SchedulerTimerExtensions.cs`.
