# Scheduler Job Extensions Tests

This example exercises `Assimalign.Cohesion.Scheduler` through its co-located test source.

> **Status:** Partial.

The example reproduces
`cohesion/resources/Scheduler/Assimalign.Cohesion.Scheduler/tests/SchedulerJobExtensionsTests.cs`.
It retains the test class and assertions so the setup, operation, and expected outcome stay
together. `Use` it in the source project’s test context, with its test dependencies and supporting
test objects.

## Behavior exercised

- **Case 1** — `AddJob`: declares a dormant enabled job.

## Source example

```csharp
using System;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace Assimalign.Cohesion.Scheduler.Tests;

public sealed class SchedulerJobExtensionsTests
{
    [Fact(DisplayName = "Cohesion Test [Scheduler] - AddJob: declares a dormant enabled job")]
    public void AddJob_WithDelegate_ShouldRegisterEnabledJob()
    {
        var builder = new RecordingBuilder();

        IScheduleJob job = builder.AddJob(
            "reconcile",
            static (_, _) => ValueTask.CompletedTask);

        builder.Job.ShouldBeSameAs(job);
        job.Name.ShouldBe("reconcile");
        job.State.ShouldBe(JobState.Enabled);
        builder.Provider.ShouldBeNull();
    }

    private sealed class RecordingBuilder : ISchedulerApplicationBuilder
    {
        public IScheduleJob? Job { get; private set; }

        public IScheduleProvider? Provider { get; private set; }

        public ISchedulerApplicationBuilder AddJob(IScheduleJob job)
        {
            Job = job;
            return this;
        }

        public ISchedulerApplicationBuilder AddScheduleProvider(IScheduleProvider provider)
        {
            Provider = provider;
            return this;
        }

        public ISchedulerApplication Build() => throw new NotSupportedException();
    }
}
```

[All examples](index.md) · [Assembly overview](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Scheduler/Assimalign.Cohesion.Scheduler/tests/SchedulerJobExtensionsTests.cs`.
- **Source** — `cohesion/resources/Scheduler/Assimalign.Cohesion.Scheduler/tests/Assimalign.Cohesion.Scheduler.Tests.csproj`.
