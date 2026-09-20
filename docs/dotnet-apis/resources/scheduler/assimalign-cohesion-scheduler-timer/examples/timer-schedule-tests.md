# Timer Schedule Tests

This example exercises `Assimalign.Cohesion.Scheduler.Timer` through its co-located test source.

> **Status:** Implemented.

The example reproduces
`cohesion/resources/Scheduler/Assimalign.Cohesion.Scheduler.Timer/tests/TimerScheduleTests.cs`. It
retains the test class and assertions so the setup, operation, and expected outcome stay together.
`Use` it in the source project’s test context, with its test dependencies and supporting test objects.

## Behavior exercised

- **Case 1** — `RunAsync`: executes a due occurrence.
- **Case 2** — `AddTimerSchedule`: rejects invalid intervals.

## Source example

```csharp
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Shouldly;
using Xunit;
using Assimalign.Cohesion.Scheduler;

namespace Assimalign.Cohesion.Scheduler.Timer.Tests;

public sealed class TimerScheduleTests
{
    [Fact(DisplayName = "Cohesion Test [Scheduler.Timer] - RunAsync: executes a due occurrence")]
    public async Task RunAsync_WhenDue_ShouldExecuteOccurrence()
    {
        using var cancellation = new CancellationTokenSource();
        IScheduleContext? observed = null;
        var job = new TestJob((context, token) =>
        {
            observed = context;
            token.CanBeCanceled.ShouldBeFalse();
            cancellation.Cancel();
            return ValueTask.CompletedTask;
        });
        var builder = new RecordingBuilder();
        builder.AddTimerSchedule(
            "heartbeat",
            TimeSpan.Zero,
            TimeSpan.FromMinutes(1),
            job,
            TimeProvider.System);
        ISchedule schedule = builder.Provider!.GetSchedules().Single();

        await schedule.RunAsync(cancellation.Token);

        observed.ShouldNotBeNull();
        observed.Name.ShouldBe("heartbeat");
        schedule.LastRunTime.ShouldBe(observed.ScheduledTime);
        schedule.Status.ShouldBe(ScheduleStatus.Stopped);
    }

    [Fact(DisplayName = "Cohesion Test [Scheduler.Timer] - AddTimerSchedule: rejects invalid intervals")]
    public void AddTimerSchedule_WithNonPositiveInterval_ShouldThrow()
    {
        var builder = new RecordingBuilder();
        var job = new TestJob(static (_, _) => ValueTask.CompletedTask);

        Should.Throw<ArgumentOutOfRangeException>(
            () => builder.AddTimerSchedule(TimeSpan.Zero, job));
    }

    private sealed class RecordingBuilder : ISchedulerApplicationBuilder
    {
        public IScheduleProvider? Provider { get; private set; }

        public ISchedulerApplicationBuilder AddJob(IScheduleJob job) => this;

        public ISchedulerApplicationBuilder AddScheduleProvider(IScheduleProvider provider)
        {
            Provider = provider;
            return this;
        }

        public ISchedulerApplication Build() => throw new NotSupportedException();
    }

    private sealed class TestJob(
        Func<IScheduleContext, CancellationToken, ValueTask> execute) : IScheduleJob
    {
        public JobId Id { get; } = JobId.New();

        public string? Name => "test";

        public JobState State => JobState.Enabled;

        public ValueTask ExecuteAsync(
            IScheduleContext context,
            CancellationToken cancellationToken = default) =>
            execute(context, cancellationToken);
    }
}
```

[All examples](index.md) · [Assembly overview](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Scheduler/Assimalign.Cohesion.Scheduler.Timer/tests/TimerScheduleTests.cs`.
- **Source** — `cohesion/resources/Scheduler/Assimalign.Cohesion.Scheduler.Timer/tests/Assimalign.Cohesion.Scheduler.Timer.Tests.csproj`.
