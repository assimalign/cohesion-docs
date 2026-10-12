# Example: Named Pipe Behavioral Tests

Exercise named-pipe endpoints and diagnostics using the original repository tests.

[Examples](index.md) · [Assembly overview](../index.md)

## Test-project context

These are the complete `NamedPipeEndPointTests.cs` and `NamedPipeConnectionEventSourceTests.cs` listings from the
package test project. Keep them in that project when running them: the project supplies its package
references, generated sources, and shared fixtures. The first listing makes the test-framework import
explicit where the original project supplies it globally. The diagnostics tests use the canonical
test-only `build/TestSupport/EventSourceRecorder.cs`, linked with MSBuild `Compile`/`Link`.

## Code

```csharp
using System.Net.Sockets;
using System;
using Shouldly;
using Xunit;

namespace Assimalign.Cohesion.Connections.NamedPipes.Tests;

public class NamedPipeEndPointTests
{
    [Fact(DisplayName = "Cohesion Test [Connections.NamedPipes] - EndPoint: Should default the server name to the local host")]
    public void Constructor_WithPipeNameOnly_ShouldDefaultToLocalServer()
    {
        // Arrange / Act
        NamedPipeEndPoint endPoint = new("orders");

        // Assert
        endPoint.PipeName.ShouldBe("orders");
        endPoint.ServerName.ShouldBe(".");
        endPoint.IsLocal.ShouldBeTrue();
        endPoint.AddressFamily.ShouldBe(AddressFamily.Unspecified);
    }

    [Fact(DisplayName = "Cohesion Test [Connections.NamedPipes] - EndPoint: Should format as a Windows pipe path")]
    public void ToString_ShouldFormatAsWindowsPipePath()
    {
        // Arrange
        NamedPipeEndPoint endPoint = new("orders", "build-server");

        // Act / Assert
        endPoint.ToString().ShouldBe(@"\\build-server\pipe\orders");
        endPoint.IsLocal.ShouldBeFalse();
    }

    [Fact(DisplayName = "Cohesion Test [Connections.NamedPipes] - EndPoint: Should compare case-insensitively")]
    public void Equals_WithDifferentCasing_ShouldBeEqual()
    {
        // Arrange
        NamedPipeEndPoint a = new("Orders");
        NamedPipeEndPoint b = new("orders", ".");

        // Act / Assert
        a.Equals(b).ShouldBeTrue();
        a.GetHashCode().ShouldBe(b.GetHashCode());
    }

    [Theory(DisplayName = "Cohesion Test [Connections.NamedPipes] - EndPoint: Should reject null or empty names")]
    [InlineData(null)]
    [InlineData("")]
    public void Constructor_WithInvalidPipeName_ShouldThrowArgumentException(string? pipeName)
    {
        // Act / Assert
        Should.Throw<ArgumentException>(() => new NamedPipeEndPoint(pipeName!));
    }

    [Fact(DisplayName = "Cohesion Test [Connections.NamedPipes] - EndPoint: Should reject an empty server name")]
    public void Constructor_WithEmptyServerName_ShouldThrowArgumentException()
    {
        // Act / Assert
        Should.Throw<ArgumentException>(() => new NamedPipeEndPoint("orders", ""));
    }
}
```

## Walkthrough

- **Covered behavior** — EndPoint: Should default the server name to the local host.
- **Covered behavior** — EndPoint: Should format as a Windows pipe path.
- **Covered behavior** — EndPoint: Should compare case-insensitively.
- **Covered behavior** — EndPoint: Should reject null or empty names.
- **Covered behavior** — EndPoint: Should reject an empty server name.

The assertions define the expected result or failure boundary. Test-only doubles supply controlled
inputs; they are not additional shipped APIs.

## Named Pipe Connection Event Source Tests

### Code

```csharp
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Shouldly;

using Xunit;

using Assimalign.Cohesion.Connections.NamedPipes.Internal;
using Assimalign.Cohesion.Testing;

namespace Assimalign.Cohesion.Connections.NamedPipes.Tests;

[Collection(nameof(EventSourceCollection))]
public class NamedPipeConnectionEventSourceTests
{
    private static readonly TimeSpan _testTimeout = TimeSpan.FromSeconds(10);

    [Fact(DisplayName = "Cohesion Test [Connections.NamedPipes] - NamedPipeConnectionEventSource: Should be named for its assembly")]
    public void GetName_NamedPipeConnectionEventSource_ShouldEqualAssemblyName()
    {
        // Act
        string name = EventSource.GetName(typeof(NamedPipeConnectionEventSource));

        // Assert
        name.ShouldBe(typeof(NamedPipeConnectionEventSource).Assembly.GetName().Name);
        name.ShouldBe("Assimalign.Cohesion.Connections.NamedPipes");
    }

    [Fact(DisplayName = "Cohesion Test [Connections.NamedPipes] - NamedPipeConnectionEventSource: Should generate a manifest in strict mode")]
    [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode", Justification = "Test-only manifest validation reflects over the event source; test assemblies are not trimmed.")]
    [UnconditionalSuppressMessage("Trimming", "IL2111:DynamicallyAccessedMembers", Justification = "Test-only manifest validation reflects over the event source; test assemblies are not trimmed.")]
    public void GenerateManifest_StrictMode_ShouldSucceed()
    {
        // Act
        string? manifest = EventSource.GenerateManifest(typeof(NamedPipeConnectionEventSource), assemblyPathToIncludeInManifest: null, EventManifestOptions.Strict);

        // Assert
        manifest.ShouldNotBeNull();
        manifest.ShouldContain("Assimalign.Cohesion.Connections.NamedPipes", Case.Sensitive);
    }

    [Fact(DisplayName = "Cohesion Test [Connections.NamedPipes] - NamedPipeConnectionEventSource: Should report accepted and dialed connections once each and restore the gauge")]
    public async Task ConnectionLifecycle_ClientAndServer_ShouldReportEachTransitionOnceAndRestoreGauge()
    {
        // Arrange
        using CancellationTokenSource cancellation = new(_testTimeout);
        using EventSourceRecorder recorder = new(NamedPipeConnectionEventSource.Log, EventLevel.Verbose);
        long currentBefore = NamedPipeConnectionEventSource.Log.CurrentConnections;
        long totalBefore = NamedPipeConnectionEventSource.Log.TotalConnections;
        NamedPipeEndPoint endPoint = new(NamedPipeTestName.Create());

        NamedPipeConnectionListener listener = NamedPipeConnectionListener.Create(options => options.EndPoint = endPoint);
        await listener.BindAsync(cancellation.Token);

        ValueTask<Connection> accept = listener.AcceptAsync(cancellation.Token);
        Connection client = await new NamedPipeConnectionFactory().ConnectAsync(endPoint, cancellation.Token);
        Connection server = await accept;
        long currentWhileOpen = NamedPipeConnectionEventSource.Log.CurrentConnections;

        // Act
        client.Abort();
        await client.DisposeAsync();
        await server.DisposeAsync();
        await listener.DisposeAsync();

        // Assert
        currentWhileOpen.ShouldBe(currentBefore + 2);
        NamedPipeConnectionEventSource.Log.TotalConnections.ShouldBe(totalBefore + 2);
        NamedPipeConnectionEventSource.Log.CurrentConnections.ShouldBe(currentBefore);

        IReadOnlyList<EventWrittenEventArgs> events = recorder.Events;
        events.ShouldNotContain(e => e.EventId == 0, "EventSource reported an instrumentation error.");

        EventWrittenEventArgs bound = events.Where(e => e.EventName == "ListenerBound").ShouldHaveSingleItem();
        bound.PayloadNames.ShouldBe(["listenerId", "endPoint"]);
        bound.Payload![1].ShouldBe(endPoint.ToString());
        string listenerId = bound.Payload[0].ShouldBeOfType<string>();

        EventWrittenEventArgs serverOpened = events.Where(e => IsFor(e, "ConnectionOpened", server.Id)).ShouldHaveSingleItem();
        serverOpened.PayloadNames.ShouldBe(["connectionId", "listenerId", "localEndPoint", "remoteEndPoint"]);
        serverOpened.Payload![1].ShouldBe(listenerId);
        events.Where(e => IsFor(e, "ConnectionOpened", client.Id)).ShouldHaveSingleItem().Payload![1].ShouldBe(string.Empty);

        // The client was aborted and then disposed: the close is still reported exactly once.
        events.Where(e => IsFor(e, "ConnectionClosed", client.Id)).ShouldHaveSingleItem();
        events.Where(e => IsFor(e, "ConnectionClosed", server.Id)).ShouldHaveSingleItem();
        events.Where(e => e.EventName == "ListenerClosed").ShouldHaveSingleItem().Payload![0].ShouldBe(listenerId);
    }

    [Fact(DisplayName = "Cohesion Test [Connections.NamedPipes] - NamedPipeConnectionEventSource: Should publish its connection counters")]
    public async Task Counters_EnabledWithInterval_ShouldPublishConnectionCounters()
    {
        // Arrange
        using CancellationTokenSource cancellation = new(_testTimeout);
        string[] counters = ["current-connections", "total-connections", "connections-per-second"];

        // Act
        using EventSourceRecorder recorder = new(NamedPipeConnectionEventSource.Log, EventLevel.LogAlways, counterInterval: TimeSpan.FromMilliseconds(100));

        // Assert
        await recorder.WaitForCountersAsync(counters, cancellation.Token);
    }

    private static bool IsFor(EventWrittenEventArgs eventData, string eventName, ConnectionId connectionId)
        => eventData.EventName == eventName && Equals(eventData.Payload![0], connectionId.ToString());
}
```

### Walkthrough

- **Covered behavior** — NamedPipeConnectionEventSource: Should be named for its assembly.
- **Covered behavior** — NamedPipeConnectionEventSource: Should generate a manifest in strict mode.
- **Covered behavior** — NamedPipeConnectionEventSource: Should report accepted and dialed connections once each and restore the gauge.
- **Covered behavior** — NamedPipeConnectionEventSource: Should publish its connection counters.

The assertions define the expected result or failure boundary. Test-only doubles supply controlled
inputs; they are not additional shipped APIs.

## Sources

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.NamedPipes/tests/NamedPipeEndPointTests.cs`.
- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.NamedPipes/tests/Assimalign.Cohesion.Connections.NamedPipes.Tests.csproj`.
- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.NamedPipes/tests/NamedPipeConnectionEventSourceTests.cs`.
- **Source** — `cohesion/build/TestSupport/EventSourceRecorder.cs`.
