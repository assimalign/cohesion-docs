# Example: Hosting Resources Behavioral Tests

Exercise resource control planes, command dispatch, diagnostic failure hand-off, contributor diagnostics, and command outcome reporting using the original repository tests.

[Examples](index.md) · [Assembly overview](../index.md)

## Test-project context

These are complete test-file listings from the package test project. Keep them in that project
when running them: the project supplies its package references, generated sources, and any shared
fixtures. The using blocks make the test-framework import explicit where the original project
supplies it globally.

## Code

### ResourceCommandDispatchTests.cs

```csharp
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System;
using Shouldly;
using Xunit;

namespace Assimalign.Cohesion.Hosting.Resources.Tests;

public sealed class ResourceCommandDispatchTests
{
    [Fact(DisplayName = "Cohesion Test [Hosting.Resources] - Commands: replay, ownership, and deletion share one dispatch seam")]
    public async Task ExecuteCommandAsync_WithReplayAndConflictingOwner_ShouldPreserveOwnership()
    {
        // Arrange
        IResourceControlPlane plane = ResourceControlPlane.Create(["test.set"]);
        var handler = new RecordingResourceCommandHandler();
        plane.RegisterCommandHandler(handler);
        var command = new ResourceCommand("first", "test.set", "appa", "key", Encoding.UTF8.GetBytes("value"));

        // Act
        await plane.ExecuteCommandAsync(command, CancellationToken.None);
        await plane.ExecuteCommandAsync(command, CancellationToken.None);
        ResourceCommandRejectedException conflict = await Should.ThrowAsync<ResourceCommandRejectedException>(
            () => plane.ExecuteCommandAsync(command with { Owner = "appb" }, CancellationToken.None).AsTask());
        await Should.ThrowAsync<ResourceCommandRejectedException>(
            () => plane.DeleteCommandAsync(command with { Owner = "appb" }, CancellationToken.None).AsTask());

        // Assert
        handler.Executions.ShouldBe(1);
        conflict.Detail.ShouldContain("appa", Case.Sensitive);
        conflict.Detail.ShouldContain("key", Case.Sensitive);
        ResourceCommandRejectedException reusedId = await Should.ThrowAsync<ResourceCommandRejectedException>(
            () => plane.ExecuteCommandAsync(command with { Owner = "appb", Key = "different" }, CancellationToken.None).AsTask());
        reusedId.Detail.ShouldContain("id 'first'", Case.Sensitive);
        plane.Commands.Count.ShouldBe(1);
        await plane.DeleteCommandAsync(command, CancellationToken.None);
        await plane.DeleteCommandAsync(command, CancellationToken.None);
        handler.Deletions.ShouldBe(1);
        plane.Commands.ShouldBeEmpty();
        await plane.ExecuteCommandAsync(command with { Owner = "appb" }, CancellationToken.None);
        handler.Executions.ShouldBe(2);
    }

    [Fact(DisplayName = "Cohesion Test [Hosting.Resources] - Commands: invalid, unsupported, and refused commands are distinct")]
    public async Task ExecuteCommandAsync_WithInvalidOrRefusedCommand_ShouldRetainTypedDetail()
    {
        // Arrange
        IResourceControlPlane plane = ResourceControlPlane.Create(["test.set"]);
        var handler = new RecordingResourceCommandHandler { Reject = true };
        plane.RegisterCommandHandler(handler);
        var command = new ResourceCommand("first", "test.set", "appa", "key", ReadOnlyMemory<byte>.Empty);

        // Act / Assert
        await Should.ThrowAsync<ArgumentException>(() => plane.ExecuteCommandAsync(command with { Key = " " }, CancellationToken.None).AsTask());
        await Should.ThrowAsync<NotSupportedException>(() => plane.ExecuteCommandAsync(command with { Kind = "unknown" }, CancellationToken.None).AsTask());
        ResourceCommandRejectedException refused = await Should.ThrowAsync<ResourceCommandRejectedException>(
            () => plane.ExecuteCommandAsync(command, CancellationToken.None).AsTask());
        refused.Detail.ShouldBe("Provider rejected key 'key'.");
        plane.Commands.ShouldBeEmpty();
    }

    [Fact(DisplayName = "Cohesion Test [Hosting.Resources] - Commands: updates cannot be deleted by stale declarations")]
    public async Task DeleteCommandAsync_AfterOwnedUpdate_ShouldRefuseStaleIdentity()
    {
        // Arrange
        IResourceControlPlane plane = ResourceControlPlane.Create(["test.set"]);
        var handler = new RecordingResourceCommandHandler();
        plane.RegisterCommandHandler(handler);
        var original = new ResourceCommand("first", "test.set", "appa", "key", Encoding.UTF8.GetBytes("old"));
        var updated = original with { Id = "second", Payload = Encoding.UTF8.GetBytes("new") };

        // Act
        await plane.ExecuteCommandAsync(original, CancellationToken.None);
        await plane.ExecuteCommandAsync(updated, CancellationToken.None);

        // Assert
        await Should.ThrowAsync<ResourceCommandRejectedException>(() => plane.DeleteCommandAsync(original, CancellationToken.None).AsTask());
        await Should.ThrowAsync<ResourceCommandRejectedException>(() => plane.ExecuteCommandAsync(updated with { Payload = original.Payload }, CancellationToken.None).AsTask());
        handler.Executions.ShouldBe(2);
        handler.Deletions.ShouldBe(0);
        plane.Commands[0].Id.ShouldBe("second");
    }
}
```

### CommandFailureDiagnosticsTests.cs

```csharp
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using Shouldly;
using Xunit;

using Assimalign.Cohesion.Hosting.Resources.Internal;

namespace Assimalign.Cohesion.Hosting.Resources.Tests;

public sealed class CommandFailureDiagnosticsTests
{
    [Fact(DisplayName = "Cohesion Test [Hosting.Resources] - Command diagnostics: reused exceptions retain separate invocation markers")]
    public async Task MarkReported_ConcurrentInvocations_ShouldConsumeEachExactlyOnce()
    {
        // Arrange
        var exception = new ArgumentException("private handler data");
        ResourceCommand[] commands = Enumerable.Range(0, 32).Select(index => CreateCommand(index.ToString())).ToArray();

        // Act
        await Task.WhenAll(commands.Select(command => Task.Run(() => CommandFailureDiagnostics.MarkReported(exception, command))));
        bool[] first = commands.Select(command => CommandFailureDiagnostics.TryConsume(exception, command)).ToArray();
        bool[] second = commands.Select(command => CommandFailureDiagnostics.TryConsume(exception, command)).ToArray();

        // Assert
        first.ShouldAllBe(value => value);
        second.ShouldAllBe(value => !value);
    }

    [Fact(DisplayName = "Cohesion Test [Hosting.Resources] - Command diagnostics: equal commands from different invocations cannot suppress each other")]
    public void TryConsume_EqualButDistinctCommand_ShouldLeaveOriginalMarker()
    {
        // Arrange
        var exception = new ArgumentException("private handler data");
        ResourceCommand original = CreateCommand("same-id");
        ResourceCommand later = original with { };
        CommandFailureDiagnostics.MarkReported(exception, original);

        // Act / Assert
        CommandFailureDiagnostics.TryConsume(exception, later).ShouldBeFalse();
        CommandFailureDiagnostics.TryConsume(exception, null).ShouldBeFalse();
        CommandFailureDiagnostics.TryConsume(exception, original).ShouldBeTrue();
    }

    [Theory(DisplayName = "Cohesion Test [Hosting.Resources] - Command diagnostics: custom exception metadata cannot replace a failure")]
    [InlineData(false)]
    [InlineData(true)]
    public void MarkReported_UnusableExceptionData_ShouldFallBackWithoutThrowing(bool throws)
    {
        // Arrange
        var exception = new CustomDataException(throws);
        ResourceCommand command = CreateCommand("custom-data");

        // Act
        Should.NotThrow(() => CommandFailureDiagnostics.MarkReported(exception, command));

        // Assert: diagnostics are best effort for application-defined metadata carriers.
        CommandFailureDiagnostics.TryConsume(exception, command).ShouldBeFalse();
    }

    private static ResourceCommand CreateCommand(string id) => new(id, "known", "owner", "key", ReadOnlyMemory<byte>.Empty);

    private sealed class CustomDataException : ArgumentException
    {
        private readonly bool _throws;
        private readonly IDictionary _readOnly = new ReadOnlyDictionary<object, object>(new Dictionary<object, object>());

        internal CustomDataException(bool throws)
        {
            _throws = throws;
        }

        public override IDictionary Data => _throws ? throw new InvalidOperationException("custom metadata failure") : _readOnly;
    }
}
```

### HostingResourceEventSourceTests.cs

```csharp
using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Shouldly;
using Xunit;

using Assimalign.Cohesion.Hosting.Health;
using Assimalign.Cohesion.Hosting.Resources.Internal;
using Assimalign.Cohesion.Testing;

namespace Assimalign.Cohesion.Hosting.Resources.Tests;

[Collection(nameof(SerialCollection))]
public sealed class HostingResourceEventSourceTests
{
    [Fact(DisplayName = "Cohesion Test [Hosting.Resources] - Event source: the provider name is its assembly")]
    public void GetName_HostingResourceEventSource_ShouldEqualAssemblyName()
    {
        // Act
        string name = EventSource.GetName(typeof(HostingResourceEventSource));

        // Assert
        name.ShouldBe("Assimalign.Cohesion.Hosting.Resources");
        name.ShouldBe(typeof(HostingResourceEventSource).Assembly.GetName().Name);
    }

    [Fact(DisplayName = "Cohesion Test [Hosting.Resources] - Event source: the strict manifest is valid")]
    [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode", Justification = "Test-only manifest validation; the test assembly is not trimmed.")]
    [UnconditionalSuppressMessage("Trimming", "IL2111:DynamicallyAccessedMembers", Justification = "Test-only manifest validation; the test assembly is not trimmed.")]
    public void GenerateManifest_StrictMode_ShouldSucceed()
    {
        // Act
        string? manifest = EventSource.GenerateManifest(typeof(HostingResourceEventSource), null, EventManifestOptions.Strict);

        // Assert
        manifest.ShouldNotBeNull();
        manifest.ShouldContain("Assimalign.Cohesion.Hosting.Resources", Case.Sensitive);
    }

    [Fact(DisplayName = "Cohesion Test [Hosting.Resources] - Event source: a contributor failure reaches diagnostics once while the report retains fixed text")]
    public async Task CheckHealthAsync_ContributorThrows_ShouldReportFailureOnceWithoutChangingPublicReport()
    {
        // Arrange
        using var cancellation = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        await Task.Delay(TimeSpan.FromMilliseconds(1100), cancellation.Token);
        const string contributorName = "event-source-contributor-hygiene";
        const string message = "Dependency db-primary.corp.internal failed.";
        var exception = new InvalidOperationException(message, new Exception("Inner exception is never written."));
        IResourceControlPlane plane = CreateFailingPlane(contributorName, exception);
        using var recorder = new EventSourceRecorder(HostingResourceEventSource.Log, EventLevel.Verbose);
        long before = HostingResourceEventSource.Log.ContributorFailures;

        // Act
        ResourceHealthReport report = await plane.CheckHealthAsync(cancellation.Token);

        // Assert
        HostingResourceEventSource.Log.ContributorFailures.ShouldBe(before + 1);
        report.Contributions[contributorName].Description.ShouldBe(ResourceControlPlane.ContributorThrewDescription);
        report.Contributions[contributorName].Data.ShouldBeNull();
        EventWrittenEventArgs failed = recorder.Events.Where(item => IsContributorFailure(item, contributorName)).ShouldHaveSingleItem();
        failed.EventId.ShouldBe(1);
        failed.Level.ShouldBe(EventLevel.Error);
        failed.PayloadNames.ShouldBe(["contributorName", "exceptionType", "exceptionMessage", "suppressedCount"]);
        failed.Payload![1].ShouldBe(typeof(InvalidOperationException).FullName);
        failed.Payload[2].ShouldBe(message);
        failed.Payload.ShouldNotContain("Inner exception is never written.");
        recorder.Events.ShouldNotContain(item => item.EventId == 0);
    }

    [Fact(DisplayName = "Cohesion Test [Hosting.Resources] - Event source: a throwing exception message cannot replace the fixed health report")]
    public async Task CheckHealthAsync_ThrowingExceptionMessage_ShouldRetainFixedReportAndSafeDiagnostic()
    {
        // Arrange
        using var cancellation = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        await Task.Delay(TimeSpan.FromMilliseconds(1100), cancellation.Token);
        const string contributorName = "event-source-throwing-message";
        IResourceControlPlane plane = CreateFailingPlane(contributorName, new ThrowingMessageException());
        using var recorder = new EventSourceRecorder(HostingResourceEventSource.Log, EventLevel.Verbose);
        long before = HostingResourceEventSource.Log.ContributorFailures;

        // Act
        ResourceHealthReport report = await plane.CheckHealthAsync(cancellation.Token);

        // Assert
        HostingResourceEventSource.Log.ContributorFailures.ShouldBe(before + 1);
        report.Contributions[contributorName].Description.ShouldBe(ResourceControlPlane.ContributorThrewDescription);
        report.Contributions[contributorName].Data.ShouldBeNull();
        EventWrittenEventArgs failed = recorder.Events.Where(item => IsContributorFailure(item, contributorName)).ShouldHaveSingleItem();
        failed.Payload![1].ShouldBe(typeof(ThrowingMessageException).FullName);
        failed.Payload[2].ShouldBe("The exception message could not be read.");
        recorder.Events.ShouldNotContain(item => item.EventId == 0);
    }

    [Fact(DisplayName = "Cohesion Test [Hosting.Resources] - Event source: failures count without a listener and the counter publishes when enabled")]
    public async Task Counters_FailuresBeforeListener_ShouldRemainExactAndPublish()
    {
        // Arrange
        using var cancellation = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        IResourceControlPlane plane = CreateFailingPlane("event-source-counter", new InvalidOperationException("dependency failed"));
        long before = HostingResourceEventSource.Log.ContributorFailures;

        // Act
        await plane.CheckHealthAsync(cancellation.Token);
        await plane.CheckReadinessAsync(cancellation.Token);
        await plane.CheckLivenessAsync(cancellation.Token);
        using var recorder = new EventSourceRecorder(HostingResourceEventSource.Log, EventLevel.Verbose, TimeSpan.FromMilliseconds(100));

        // Assert
        HostingResourceEventSource.Log.ContributorFailures.ShouldBe(before + 3);
        await recorder.WaitForCountersAsync(["contributor-failures"], cancellation.Token);
        recorder.Events.ShouldNotContain(item => item.EventId == 0);
    }

    [Fact(DisplayName = "Cohesion Test [Hosting.Resources] - Event source: repeated failures are throttled and suppressed occurrences remain counted")]
    public async Task CheckHealthAsync_RepeatedFailures_ShouldThrottleEventsAndCountEveryFailure()
    {
        // Arrange
        using var cancellation = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        await Task.Delay(TimeSpan.FromMilliseconds(1100), cancellation.Token);
        const string contributorName = "event-source-throttling";
        IResourceControlPlane plane = CreateFailingPlane(contributorName, new InvalidOperationException("dependency failed"));
        using var recorder = new EventSourceRecorder(HostingResourceEventSource.Log, EventLevel.Verbose);
        long before = HostingResourceEventSource.Log.ContributorFailures;

        // Act
        await plane.CheckHealthAsync(cancellation.Token);
        await plane.CheckReadinessAsync(cancellation.Token);
        await plane.CheckLivenessAsync(cancellation.Token);
        recorder.Events.Where(item => IsContributorFailure(item, contributorName)).ShouldHaveSingleItem();
        await Task.Delay(TimeSpan.FromMilliseconds(1100), cancellation.Token);
        await plane.CheckHealthAsync(cancellation.Token);

        // Assert
        HostingResourceEventSource.Log.ContributorFailures.ShouldBe(before + 4);
        EventWrittenEventArgs[] events = recorder.Events.Where(item => IsContributorFailure(item, contributorName)).ToArray();
        events.Length.ShouldBe(2);
        events[1].Payload![3].ShouldBe(2);
        recorder.Events.ShouldNotContain(item => item.EventId == 0);
    }

    [Fact(DisplayName = "Cohesion Test [Hosting.Resources] - Event source: requested cancellation does not become a contributor failure")]
    public async Task CheckHealthAsync_CallerCancels_ShouldPropagateWithoutFailureEventOrCounter()
    {
        // Arrange
        using var cancellation = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        const string contributorName = "event-source-caller-cancellation";
        IResourceControlPlane plane = ResourceControlPlane.Create();
        plane.AddHealthContributor(new CallbackContributor(contributorName, token =>
        {
            cancellation.Cancel();
            return ValueTask.FromException<HealthContribution>(new OperationCanceledException(token));
        }));
        using var recorder = new EventSourceRecorder(HostingResourceEventSource.Log, EventLevel.Verbose);
        long before = HostingResourceEventSource.Log.ContributorFailures;

        // Act
        await Should.ThrowAsync<OperationCanceledException>(() => plane.CheckHealthAsync(cancellation.Token).AsTask());

        // Assert
        HostingResourceEventSource.Log.ContributorFailures.ShouldBe(before);
        recorder.Events.ShouldNotContain(item => IsContributorFailure(item, contributorName));
        recorder.Events.ShouldNotContain(item => item.EventId == 0);
    }

    [Fact(DisplayName = "Cohesion Test [Hosting.Resources] - Event source: command outcomes are emitted once with registered kinds and no command data")]
    public async Task ExecuteCommandAsync_CommandOutcomes_ShouldEmitOnceWithRegisteredKindsOnly()
    {
        // Arrange
        using var cancellation = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        await Task.Delay(TimeSpan.FromMilliseconds(1100), cancellation.Token);
        string kind = "registered-" + Guid.NewGuid().ToString("N");
        var handler = new DiagnosticCommandHandler(kind);
        IResourceControlPlane plane = CreateCommandPlane(handler);
        ResourceCommand command = CreateCommand(kind);
        using var recorder = new EventSourceRecorder(HostingResourceEventSource.Log, EventLevel.Verbose);
        long[] before = ReadCommandCounters();

        // Act
        await plane.ExecuteCommandAsync(command, cancellation.Token);
        await Should.ThrowAsync<NotSupportedException>(() => plane.ExecuteCommandAsync(command with { Kind = "unknown-kind-secret" }, cancellation.Token).AsTask());
        await Should.ThrowAsync<ResourceCommandRejectedException>(() => plane.ExecuteCommandAsync(command with { Owner = "another-owner-secret" }, cancellation.Token).AsTask());
        handler.Failure = new ArgumentException("handler-payload-secret", "handler-key-secret");
        await Should.ThrowAsync<ArgumentException>(() => plane.ExecuteCommandAsync(command with { Id = "failure-id-secret", Key = "failure-key-secret" }, cancellation.Token).AsTask());

        // Assert
        AssertCommandCounterChanges(before, applied: 1, unknown: 1, conflicts: 1, failures: 1);
        EventWrittenEventArgs applied = ForCommand(recorder, "CommandApplied", kind).ShouldHaveSingleItem();
        AssertCommandEvent(applied, 2, EventLevel.Informational, ["kind", "operation"]);
        EventWrittenEventArgs unknown = ForCommand(recorder, "CommandUnknownKind", string.Empty).ShouldHaveSingleItem();
        AssertCommandEvent(unknown, 3, EventLevel.Warning, ["kind", "operation", "suppressedCount"]);
        EventWrittenEventArgs conflict = ForCommand(recorder, "CommandConflict", kind).ShouldHaveSingleItem();
        AssertCommandEvent(conflict, 4, EventLevel.Warning, ["kind", "operation", "suppressedCount"]);
        EventWrittenEventArgs failure = ForCommand(recorder, "CommandHandlerFailed", kind).ShouldHaveSingleItem();
        AssertCommandEvent(failure, 5, EventLevel.Error, ["kind", "operation", "exceptionType", "suppressedCount"]);
        failure.Payload![2].ShouldBe(typeof(ArgumentException).FullName);
        foreach (EventWrittenEventArgs item in new[] { applied, unknown, conflict, failure })
        {
            item.Payload![1].ShouldBe("Apply");
            string.Join("|", item.Payload).ShouldNotContain("secret", Case.Sensitive);
        }
        recorder.Events.ShouldNotContain(item => item.EventId == 0);
    }

    [Fact(DisplayName = "Cohesion Test [Hosting.Resources] - Event source: replay, delete and handler refusal counters stay exact without a listener")]
    public async Task Counters_CommandOutcomesBeforeListener_ShouldCountEveryOutcomeAndPublish()
    {
        // Arrange
        using var cancellation = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        string kind = "counter-" + Guid.NewGuid().ToString("N");
        var handler = new DiagnosticCommandHandler(kind);
        IResourceControlPlane plane = CreateCommandPlane(handler);
        ResourceCommand command = CreateCommand(kind);
        long[] before = ReadCommandCounters();

        // Act
        await plane.ExecuteCommandAsync(command, cancellation.Token);
        await plane.ExecuteCommandAsync(command, cancellation.Token);
        await Should.ThrowAsync<ResourceCommandRejectedException>(() => plane.ExecuteCommandAsync(command with { Owner = "other-owner-secret" }, cancellation.Token).AsTask());
        handler.Failure = new ResourceCommandRejectedException("provider-refusal-secret");
        await Should.ThrowAsync<ResourceCommandRejectedException>(() => plane.ExecuteCommandAsync(command with { Id = "refusal-id-secret", Key = "refusal-key-secret" }, cancellation.Token).AsTask());
        handler.Failure = new InvalidOperationException("handler-failure-secret");
        await Should.ThrowAsync<InvalidOperationException>(() => plane.ExecuteCommandAsync(command with { Id = "failure-id-secret", Key = "failure-key-secret" }, cancellation.Token).AsTask());
        handler.Failure = null;
        await plane.DeleteCommandAsync(command, cancellation.Token);
        await plane.DeleteCommandAsync(command, cancellation.Token);
        await Should.ThrowAsync<NotSupportedException>(() => plane.DeleteCommandAsync(command with { Kind = "advertised-kind-secret" }, cancellation.Token).AsTask());
        using var recorder = new EventSourceRecorder(HostingResourceEventSource.Log, EventLevel.Verbose, TimeSpan.FromMilliseconds(100));

        // Assert
        AssertCommandCounterChanges(before, applied: 4, unknown: 1, conflicts: 2, failures: 1);
        handler.Executions.ShouldBe(3); // Initial apply and the two rejected handler calls; replay never executes.
        handler.Deletions.ShouldBe(1);
        await recorder.WaitForCountersAsync(["contributor-failures", "commands-applied", "unknown-command-kinds", "command-conflicts", "command-handler-failures"], cancellation.Token);
        recorder.Events.ShouldNotContain(item => item.EventId == 0);
    }

    [Fact(DisplayName = "Cohesion Test [Hosting.Resources] - Event source: command refusal and failure throttles are independent with exact counters")]
    public async Task ExecuteCommandAsync_RepeatedCommandFailures_ShouldThrottleEachEventAndCountEveryOutcome()
    {
        // Arrange
        using var cancellation = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        await Task.Delay(TimeSpan.FromMilliseconds(1100), cancellation.Token);
        string kind = "throttle-" + Guid.NewGuid().ToString("N");
        var handler = new DiagnosticCommandHandler(kind);
        IResourceControlPlane plane = CreateCommandPlane(handler);
        ResourceCommand command = CreateCommand(kind);
        await plane.ExecuteCommandAsync(command, cancellation.Token);
        handler.Failure = new InvalidOperationException("handler-payload-secret");
        using var recorder = new EventSourceRecorder(HostingResourceEventSource.Log, EventLevel.Verbose);
        long[] before = ReadCommandCounters();

        async Task FailEachAsync()
        {
            await Should.ThrowAsync<NotSupportedException>(() => plane.DeleteCommandAsync(command with { Kind = "unknown-kind-secret" }, cancellation.Token).AsTask());
            await Should.ThrowAsync<ResourceCommandRejectedException>(() => plane.DeleteCommandAsync(command with { Owner = "other-owner-secret" }, cancellation.Token).AsTask());
            await Should.ThrowAsync<InvalidOperationException>(() => plane.DeleteCommandAsync(command, cancellation.Token).AsTask());
        }

        // Act
        await FailEachAsync();
        await FailEachAsync();
        await FailEachAsync();
        ForCommand(recorder, "CommandUnknownKind", string.Empty).ShouldHaveSingleItem();
        ForCommand(recorder, "CommandConflict", kind).ShouldHaveSingleItem();
        ForCommand(recorder, "CommandHandlerFailed", kind).ShouldHaveSingleItem();
        await Task.Delay(TimeSpan.FromMilliseconds(1100), cancellation.Token);
        await FailEachAsync();

        // Assert
        AssertCommandCounterChanges(before, applied: 0, unknown: 4, conflicts: 4, failures: 4);
        foreach ((string name, string eventKind) in new[] { ("CommandUnknownKind", string.Empty), ("CommandConflict", kind), ("CommandHandlerFailed", kind) })
        {
            EventWrittenEventArgs[] events = ForCommand(recorder, name, eventKind);
            events.Length.ShouldBe(2);
            events[1].Payload![1].ShouldBe("Delete");
            events[1].Payload![events[1].PayloadNames!.IndexOf("suppressedCount")].ShouldBe(2);
            string.Join("|", events[1].Payload!).ShouldNotContain("secret", Case.Sensitive);
        }
        recorder.Events.ShouldNotContain(item => item.EventId == 0);
    }

    [Fact(DisplayName = "Cohesion Test [Hosting.Resources] - Event source: concurrent replays and deletions each report success once")]
    public async Task ExecuteCommandAsync_ConcurrentReplayAndDelete_ShouldCountDispatchesWithoutRepeatingHandlers()
    {
        // Arrange
        using var cancellation = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        string kind = "concurrent-" + Guid.NewGuid().ToString("N");
        var handler = new DiagnosticCommandHandler(kind)
        {
            Callback = async _ =>
            {
                await Task.Yield();
                return ReadOnlyMemory<byte>.Empty;
            },
        };
        IResourceControlPlane plane = CreateCommandPlane(handler);
        ResourceCommand command = CreateCommand(kind);
        using var recorder = new EventSourceRecorder(HostingResourceEventSource.Log, EventLevel.Verbose);
        long[] before = ReadCommandCounters();

        // Act
        await Task.WhenAll(Enumerable.Range(0, 16).Select(_ => plane.ExecuteCommandAsync(command, cancellation.Token).AsTask()));
        await Task.WhenAll(Enumerable.Range(0, 8).Select(_ => plane.DeleteCommandAsync(command, cancellation.Token).AsTask()));

        // Assert
        AssertCommandCounterChanges(before, applied: 24, unknown: 0, conflicts: 0, failures: 0);
        handler.Executions.ShouldBe(1);
        handler.Deletions.ShouldBe(1);
        EventWrittenEventArgs[] applied = ForCommand(recorder, "CommandApplied", kind);
        applied.Length.ShouldBe(24);
        applied.Count(item => Equals(item.Payload![1], "Apply")).ShouldBe(16);
        applied.Count(item => Equals(item.Payload![1], "Delete")).ShouldBe(8);
        recorder.Events.ShouldNotContain(item => item.EventId == 0);
    }

    [Fact(DisplayName = "Cohesion Test [Hosting.Resources] - Event source: caller cancellation in a handler produces no failure outcome")]
    public async Task ExecuteCommandAsync_CallerCancelsHandler_ShouldPreserveCancellationWithoutOutcome()
    {
        // Arrange
        using var cancellation = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        string kind = "cancellation-" + Guid.NewGuid().ToString("N");
        var handler = new DiagnosticCommandHandler(kind)
        {
            Callback = token =>
            {
                cancellation.Cancel();
                return ValueTask.FromException<ReadOnlyMemory<byte>>(new OperationCanceledException(token));
            },
        };
        IResourceControlPlane plane = CreateCommandPlane(handler);
        using var recorder = new EventSourceRecorder(HostingResourceEventSource.Log, EventLevel.Verbose);
        long[] before = ReadCommandCounters();

        // Act
        await Should.ThrowAsync<OperationCanceledException>(() => plane.ExecuteCommandAsync(CreateCommand(kind), cancellation.Token).AsTask());

        // Assert
        AssertCommandCounterChanges(before, applied: 0, unknown: 0, conflicts: 0, failures: 0);
        recorder.Events.ShouldNotContain(item => item.Payload != null && item.Payload.Contains(kind));
        recorder.Events.ShouldNotContain(item => item.EventId == 0);
    }

    [Theory(DisplayName = "Cohesion Test [Hosting.Resources] - Event source: handler unsupported operations and independent cancellation are failures, not unknown kinds")]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ExecuteCommandAsync_HandlerThrowsSpecialException_ShouldReportHandlerFailure(bool independentCancellation)
    {
        // Arrange
        using var cancellation = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        await Task.Delay(TimeSpan.FromMilliseconds(1100), cancellation.Token);
        string kind = "failure-" + Guid.NewGuid().ToString("N");
        Exception exception = independentCancellation
            ? new OperationCanceledException("independent-cancellation-secret")
            : new NotSupportedException("handler-unsupported-secret");
        var handler = new DiagnosticCommandHandler(kind) { Failure = exception };
        IResourceControlPlane plane = CreateCommandPlane(handler);
        using var recorder = new EventSourceRecorder(HostingResourceEventSource.Log, EventLevel.Verbose);
        long[] before = ReadCommandCounters();

        // Act
        Exception actual = await Should.ThrowAsync<Exception>(() => plane.ExecuteCommandAsync(CreateCommand(kind), cancellation.Token).AsTask());

        // Assert
        if (independentCancellation)
        {
            (actual is OperationCanceledException).ShouldBeTrue();
        }
        else
        {
            actual.ShouldBeSameAs(exception);
        }
        AssertCommandCounterChanges(before, applied: 0, unknown: 0, conflicts: 0, failures: 1);
        EventWrittenEventArgs failure = ForCommand(recorder, "CommandHandlerFailed", kind).ShouldHaveSingleItem();
        failure.Payload![2].ShouldBe(exception.GetType().FullName);
        string.Join("|", failure.Payload).ShouldNotContain("secret", Case.Sensitive);
        recorder.Events.ShouldNotContain(item => item.EventId == 0);
    }

    private static ResourceCommand CreateCommand(string kind)
        => new("id-secret", kind, "owner-secret", "key-secret", "payload-secret"u8.ToArray());

    private static IResourceControlPlane CreateCommandPlane(DiagnosticCommandHandler handler)
    {
        IResourceControlPlane plane = ResourceControlPlane.Create([handler.Kind, "advertised-kind-secret"]);
        plane.RegisterCommandHandler(handler);
        return plane;
    }

    private static long[] ReadCommandCounters()
    {
        HostingResourceEventSource source = HostingResourceEventSource.Log;
        return [source.CommandsApplied, source.UnknownCommandKinds, source.CommandConflicts, source.CommandHandlerFailures];
    }

    private static void AssertCommandCounterChanges(long[] before, long applied, long unknown, long conflicts, long failures)
        => ReadCommandCounters().ShouldBe(new[] { before[0] + applied, before[1] + unknown, before[2] + conflicts, before[3] + failures });

    private static EventWrittenEventArgs[] ForCommand(EventSourceRecorder recorder, string name, string kind)
        => recorder.Events.Where(item => item.EventName == name && Equals(item.Payload![0], kind)).ToArray();

    private static void AssertCommandEvent(EventWrittenEventArgs item, int id, EventLevel level, string[] payloadNames)
    {
        item.EventId.ShouldBe(id);
        item.Level.ShouldBe(level);
        item.PayloadNames.ShouldBe(payloadNames);
    }

    private sealed class DiagnosticCommandHandler : IResourceCommandHandler
    {
        internal DiagnosticCommandHandler(string kind) => Kind = kind;

        public string Kind { get; }
        internal Exception? Failure { get; set; }
        internal Func<CancellationToken, ValueTask<ReadOnlyMemory<byte>>>? Callback { get; init; }
        internal int Executions { get; private set; }
        internal int Deletions { get; private set; }

        public ValueTask<ReadOnlyMemory<byte>> ExecuteAsync(ResourceCommand command, CancellationToken cancellationToken = default)
        {
            Executions++;
            return CompleteAsync(cancellationToken);
        }

        public ValueTask<ReadOnlyMemory<byte>> DeleteAsync(ResourceCommand command, CancellationToken cancellationToken = default)
        {
            Deletions++;
            return CompleteAsync(cancellationToken);
        }

        private ValueTask<ReadOnlyMemory<byte>> CompleteAsync(CancellationToken cancellationToken)
            => Failure is { } exception
                ? ValueTask.FromException<ReadOnlyMemory<byte>>(exception)
                : Callback?.Invoke(cancellationToken) ?? ValueTask.FromResult(ReadOnlyMemory<byte>.Empty);
    }

    private static IResourceControlPlane CreateFailingPlane(string contributorName, Exception exception)
    {
        IResourceControlPlane plane = ResourceControlPlane.Create();
        plane.AddHealthContributor(new CallbackContributor(contributorName, _ => ValueTask.FromException<HealthContribution>(exception)));
        return plane;
    }

    private static bool IsContributorFailure(EventWrittenEventArgs item, string contributorName)
        => item.EventName == "ContributorFailed" && Equals(item.Payload![0], contributorName);

    private sealed class ThrowingMessageException : Exception
    {
        public override string Message => throw new InvalidOperationException("exception-message-secret");
    }

    private sealed class CallbackContributor : IHealthContributor
    {
        private readonly Func<CancellationToken, ValueTask<HealthContribution>> _callback;

        internal CallbackContributor(string name, Func<CancellationToken, ValueTask<HealthContribution>> callback)
        {
            Name = name;
            _callback = callback;
        }

        public string Name { get; }

        public ValueTask<HealthContribution> CheckAsync(CancellationToken cancellationToken = default)
            => _callback(cancellationToken);
    }
}
```

### ResourceControlPlaneTests.cs

```csharp
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Assimalign.Cohesion.Hosting.Health;
using Assimalign.Cohesion.Hosting.Resources.Internal;

using Shouldly;
using Xunit;

namespace Assimalign.Cohesion.Hosting.Resources.Tests;

public class ResourceControlPlaneTests
{
    private const string DisplayPrefix = "Cohesion Test [Hosting] - Resource control plane: ";

    [Fact(DisplayName = DisplayPrefix + "Assembly registration creates isolated control planes and carries stop grace")]
    public void RegisterControlPlane_ForAssembly_CreatesNewInstancesAndCarriesStopGrace()
    {
        // Arrange
        Assembly assembly = typeof(ResourceControlPlaneTests).Assembly;
        ResourceRuntime.RegisterControlPlane(
            assembly,
            static () => ResourceControlPlane.Create(),
            stopGraceSeconds: 42);
        using IDisposable scope = ResourceRuntime.CreateScope(new ResourceContext());

        // Act
        ResourceRuntime.TryCreateControlPlane(assembly, out IResourceControlPlane? first).ShouldBeTrue();
        ResourceRuntime.TryCreateControlPlane(assembly, out IResourceControlPlane? second).ShouldBeTrue();

        // Assert
        first.ShouldNotBeNull();
        second.ShouldNotBeNull();
        second.ShouldNotBeSameAs(first);

        var host = new TestHost(new TestHostOptions());
        ResourceRuntime.HostBuilt(host, first);
        ResourceHostRunner runner = host.Context.Runner.ShouldBeOfType<ResourceHostRunner>();
        runner.Options.StopGraceSeconds.ShouldBe(42);
    }

    [Fact(DisplayName = DisplayPrefix + "Health aggregation fails to the least healthy contribution")]
    public async Task CheckHealthAsync_WithContributors_AggregatesStatus()
    {
        // Arrange
        ResourceControlPlane controlPlane = ResourceControlPlane.Create();
        controlPlane.AddHealthContributor(new TestContributor(
            "ready",
            HealthContribution.Healthy("accepting work")));
        controlPlane.AddHealthContributor(new TestContributor(
            "dependency",
            HealthContribution.Degraded("slow")));
        controlPlane.ObserveEndpoint("http", new Uri("http://localhost:5080"));

        // Act
        ResourceHealthReport report = await controlPlane.CheckHealthAsync(CancellationToken.None);

        // Assert
        report.Status.ShouldBe(HealthStatus.Degraded);
        report.Contributions.Count.ShouldBe(2);
        controlPlane.ObservedEndpoints["http"].Port.ShouldBe(5080);
    }

    [Fact(DisplayName = DisplayPrefix + "Endpoint observation rejects a null address")]
    public void ObserveEndpoint_WithNullAddress_ShouldThrowArgumentNullException()
    {
        // Arrange
        ResourceControlPlane controlPlane = ResourceControlPlane.Create();

        // Act
        Action action = () => controlPlane.ObserveEndpoint("http", null!);

        // Assert
        Should.Throw<ArgumentNullException>(action).ParamName.ShouldBe("address");
    }

    [Fact(DisplayName = DisplayPrefix + "Endpoint observation rejects a relative address")]
    public void ObserveEndpoint_WithRelativeAddress_ShouldThrowArgumentException()
    {
        // Arrange
        ResourceControlPlane controlPlane = ResourceControlPlane.Create();

        // Act
        Action action = () => controlPlane.ObserveEndpoint("http", new Uri("relative", UriKind.Relative));

        // Assert
        Should.Throw<ArgumentException>(action).ParamName.ShouldBe("address");
    }

    [Fact(DisplayName = DisplayPrefix + "A contributor failure becomes a named unhealthy result with fixed text, without stopping aggregation")]
    public async Task CheckHealthAsync_WhenContributorThrows_RecordsFailureAndContinues()
    {
        // Arrange
        int followingInvocations = 0;
        ResourceControlPlane controlPlane = ResourceControlPlane.Create();
        controlPlane.AddHealthContributor(new CallbackContributor(
            "database",
            _ => ValueTask.FromException<HealthContribution>(
                new InvalidOperationException("Failed to connect to db-primary.corp.internal:5432."))));
        controlPlane.AddHealthContributor(new CallbackContributor(
            "worker",
            _ =>
            {
                followingInvocations++;
                return ValueTask.FromResult(HealthContribution.Healthy("running"));
            }));

        // Act
        ResourceHealthReport report = await controlPlane.CheckHealthAsync(CancellationToken.None);

        // Assert
        report.Status.ShouldBe(HealthStatus.Unhealthy);
        report.Contributions.Count.ShouldBe(2);
        report.Contributions["database"].ShouldBe(
            HealthContribution.Unhealthy(ResourceControlPlane.ContributorThrewDescription));
        report.Contributions["worker"].Status.ShouldBe(HealthStatus.Healthy);
        followingInvocations.ShouldBe(1);
    }

    [Fact(DisplayName = DisplayPrefix + "An unrelated contributor cancellation becomes an unhealthy result with fixed text")]
    public async Task CheckHealthAsync_WhenContributorCancelsIndependently_RecordsFailure()
    {
        // Arrange
        ResourceControlPlane controlPlane = ResourceControlPlane.Create();
        controlPlane.AddHealthContributor(new CallbackContributor(
            "queue",
            _ => ValueTask.FromException<HealthContribution>(
                new OperationCanceledException("queue probe canceled"))));

        // Act
        ResourceHealthReport report = await controlPlane.CheckHealthAsync(CancellationToken.None);

        // Assert
        report.Status.ShouldBe(HealthStatus.Unhealthy);
        report.Contributions["queue"].ShouldBe(
            HealthContribution.Unhealthy(ResourceControlPlane.ContributorThrewDescription));
    }

    [Fact(DisplayName = DisplayPrefix + "An exception a contributor places in its data is replaced with fixed text, keeping the other values and their order")]
    public async Task CheckHealthAsync_WhenContributorDataHoldsException_ReplacesItWithFixedText()
    {
        // Arrange
        const string hostName = "db-primary.corp.internal";
        Exception failure;
        try
        {
            throw new InvalidOperationException(
                $"Failed to connect to {hostName}:5432.",
                new TimeoutException($"No answer from {hostName}."));
        }
        catch (InvalidOperationException exception)
        {
            failure = exception;
        }

        ResourceControlPlane controlPlane = ResourceControlPlane.Create();
        controlPlane.AddHealthContributor(new TestContributor(
            "database",
            HealthContribution.Unhealthy(
                "database unreachable",
                new Dictionary<string, object>
                {
                    ["attempts"] = 3,
                    ["error"] = failure,
                    ["endpoint"] = "primary",
                })));

        // Act
        ResourceHealthReport report = await controlPlane.CheckHealthAsync(CancellationToken.None);

        // Assert
        HealthContribution contribution = report.Contributions["database"];
        contribution.Status.ShouldBe(HealthStatus.Unhealthy);
        contribution.Description.ShouldBe("database unreachable");
        IReadOnlyDictionary<string, object> data = contribution.Data.ShouldNotBeNull();
        data.Keys.ShouldBe(new[] { "attempts", "error", "endpoint" });
        data["attempts"].ShouldBe(3);
        data["error"].ShouldBe(ResourceControlPlane.ContributorExceptionDataValue);
        data["endpoint"].ShouldBe("primary");
    }

    [Fact(DisplayName = DisplayPrefix + "A contribution whose data holds no exception is reported as returned")]
    public async Task CheckHealthAsync_WhenContributorDataHoldsNoException_ReportsDataAsReturned()
    {
        // Arrange
        var data = new Dictionary<string, object> { ["queued"] = 12, ["note"] = "draining" };
        ResourceControlPlane controlPlane = ResourceControlPlane.Create();
        controlPlane.AddHealthContributor(new TestContributor("queue", HealthContribution.Degraded("slow", data)));

        // Act
        ResourceHealthReport report = await controlPlane.CheckHealthAsync(CancellationToken.None);

        // Assert
        report.Contributions["queue"].Data.ShouldBeSameAs(data);
    }

    [Fact(DisplayName = DisplayPrefix + "Caller cancellation propagates and stops aggregation")]
    public async Task CheckHealthAsync_WhenCallerCancels_PropagatesCancellation()
    {
        // Arrange
        using var cancellation = new CancellationTokenSource();
        int followingInvocations = 0;
        ResourceControlPlane controlPlane = ResourceControlPlane.Create();
        controlPlane.AddHealthContributor(new CallbackContributor(
            "blocking",
            cancellationToken =>
            {
                cancellation.Cancel();
                cancellationToken.ThrowIfCancellationRequested();
                return ValueTask.FromResult(HealthContribution.Healthy());
            }));
        controlPlane.AddHealthContributor(new CallbackContributor(
            "following",
            _ =>
            {
                followingInvocations++;
                return ValueTask.FromResult(HealthContribution.Healthy());
            }));

        // Act & Assert
        await Should.ThrowAsync<OperationCanceledException>(async () =>
            await controlPlane.CheckHealthAsync(cancellation.Token));
        followingInvocations.ShouldBe(0);
    }

    [Fact(DisplayName = DisplayPrefix + "HostBuilt attaches lifecycle options and observed endpoints")]
    public void HostBuilt_WithAmbientContext_ConfiguresOnlyTheBuiltHost()
    {
        // Arrange
        string contentRoot = Path.GetFullPath("resource-content");
        var endpoint = new Uri("http://127.0.0.1:5081");
        ResourceContext context = ResourceContext.FromEnvironment(
            new Dictionary<string, string?>
            {
                [AppEnvironment.Variables.ContentRoot] = contentRoot,
                [AppEnvironment.Variables.StopEvent] = "cohesion-stop-test",
                [AppEnvironment.Variables.Endpoint("http", "HOST")] = endpoint.Host,
                [AppEnvironment.Variables.Endpoint("http", "PORT")] = endpoint.Port.ToString(
                    System.Globalization.CultureInfo.InvariantCulture),
                [AppEnvironment.Variables.Endpoint("http", "SCHEME")] = endpoint.Scheme,
            });
        using IDisposable scope = ResourceRuntime.CreateScope(context);
        ResourceControlPlane controlPlane = ResourceControlPlane.Create();
        var host = new TestHost(new TestHostOptions());

        // Act
        ResourceRuntime.HostBuilt(host, controlPlane);

        // Assert
        ResourceHostRunner runner = host.Context.Runner.ShouldBeOfType<ResourceHostRunner>();
        ResourceHostOptions options = runner.Options;
        options.ContentRootPath.ShouldBe(FileSystemPath.Parse(contentRoot));
        options.StopEventName.ShouldBe("cohesion-stop-test");
        options.RunMode.ShouldBe(ResourceHostRunMode.Process);
        controlPlane.ObservedEndpoints["http"].ShouldBe(endpoint);
    }

    private sealed class TestContributor : IHealthContributor
    {
        private readonly HealthContribution _contribution;

        internal TestContributor(string name, HealthContribution contribution)
        {
            Name = name;
            _contribution = contribution;
        }

        public string Name { get; }

        public ValueTask<HealthContribution> CheckAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return ValueTask.FromResult(_contribution);
        }
    }

    private sealed class CallbackContributor : IHealthContributor
    {
        private readonly Func<CancellationToken, ValueTask<HealthContribution>> _callback;

        internal CallbackContributor(
            string name,
            Func<CancellationToken, ValueTask<HealthContribution>> callback)
        {
            Name = name;
            _callback = callback;
        }

        public string Name { get; }

        public ValueTask<HealthContribution> CheckAsync(
            CancellationToken cancellationToken = default)
        {
            return _callback.Invoke(cancellationToken);
        }
    }
}
```

## Walkthrough

### ResourceCommandDispatchTests

- **Covered behavior** — Commands: replay, ownership, and deletion share one dispatch seam.
- **Covered behavior** — Commands: invalid, unsupported, and refused commands are distinct.
- **Covered behavior** — Commands: updates cannot be deleted by stale declarations.

### CommandFailureDiagnosticsTests

- **Covered behavior** — Command diagnostics: reused exceptions retain separate invocation markers.
- **Covered behavior** — Command diagnostics: equal commands from different invocations cannot suppress each other.
- **Covered behavior** — Command diagnostics: custom exception metadata cannot replace a failure.

### HostingResourceEventSourceTests

- **Covered behavior** — Event source: the provider name is its assembly.
- **Covered behavior** — Event source: the strict manifest is valid.
- **Covered behavior** — Event source: a contributor failure reaches diagnostics once while the report retains fixed text.
- **Covered behavior** — Event source: a throwing exception message cannot replace the fixed health report.
- **Covered behavior** — Event source: failures count without a listener and the counter publishes when enabled.
- **Covered behavior** — Event source: repeated failures are throttled and suppressed occurrences remain counted.
- **Covered behavior** — Event source: requested cancellation does not become a contributor failure.
- **Covered behavior** — Event source: command outcomes are emitted once with registered kinds and no command data.
- **Covered behavior** — Event source: replay, delete and handler refusal counters stay exact without a listener.
- **Covered behavior** — Event source: command refusal and failure throttles are independent with exact counters.
- **Covered behavior** — Event source: concurrent replays and deletions each report success once.
- **Covered behavior** — Event source: caller cancellation in a handler produces no failure outcome.
- **Covered behavior** — Event source: handler unsupported operations and independent cancellation are failures, not unknown kinds.

### ResourceControlPlaneTests

- **Covered behavior** — Resource control plane: Assembly registration creates isolated control planes and carries stop grace.
- **Covered behavior** — Resource control plane: Health aggregation fails to the least healthy contribution.
- **Covered behavior** — Resource control plane: Endpoint observation rejects a null address.
- **Covered behavior** — Resource control plane: Endpoint observation rejects a relative address.
- **Covered behavior** — Resource control plane: A contributor failure becomes a named unhealthy result with fixed text, without stopping aggregation.
- **Covered behavior** — Resource control plane: An unrelated contributor cancellation becomes an unhealthy result with fixed text.
- **Covered behavior** — Resource control plane: An exception a contributor places in its data is replaced with fixed text, keeping the other values and their order.
- **Covered behavior** — Resource control plane: A contribution whose data holds no exception is reported as returned.
- **Covered behavior** — Resource control plane: Caller cancellation propagates and stops aggregation.
- **Covered behavior** — Resource control plane: HostBuilt attaches lifecycle options and observed endpoints.

The assertions define the expected result or failure boundary. Test-only doubles supply controlled
inputs; they are not additional shipped APIs.

## Sources

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting.Resources/tests/ResourceCommandDispatchTests.cs`.
- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting.Resources/tests/Assimalign.Cohesion.Hosting.Resources.TestResource.csproj`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting.Resources/tests/CommandFailureDiagnosticsTests.cs`.
- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting.Resources/tests/HostingResourceEventSourceTests.cs`.
- **Source** — `cohesion/build/TestSupport/EventSourceRecorder.cs`.
- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting.Resources/tests/Assimalign.Cohesion.Hosting.Resources.Tests.csproj`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting.Resources/tests/ResourceControlPlaneTests.cs`.
