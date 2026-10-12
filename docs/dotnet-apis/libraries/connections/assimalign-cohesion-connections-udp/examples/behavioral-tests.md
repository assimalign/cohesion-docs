# Example: UDP Behavioral Tests

Exercise UDP connection factories and diagnostics using the original repository tests.

[Examples](index.md) · [Assembly overview](../index.md)

## Test-project context

These are the complete `UdpConnectionFactoryTests.cs` and `UdpConnectionEventSourceTests.cs` listings from the
package test project. Keep them in that project when running them: the project supplies its package
references, generated sources, and shared fixtures. The first listing makes the test-framework import
explicit where the original project supplies it globally. The diagnostics tests use the canonical
test-only `EventSourceRecorder`, linked through
`CohesionSharedSource Include="Assimalign.Cohesion.DependencyInjection.Tests"`.

## Code

```csharp
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace Assimalign.Cohesion.Connections.Udp.Tests;

public class UdpConnectionFactoryTests
{
    private static readonly TimeSpan _testTimeout = TimeSpan.FromSeconds(5);

    [Fact]
    public async Task Bind_WithEphemeralLoopbackEndPoint_ShouldPopulateLocalEndPointOnly()
    {
        // Arrange
        UdpConnectionFactory factory = new();

        // Act
        await using IDatagramConnection connection = factory.Bind(new IPEndPoint(IPAddress.Loopback, 0));

        // Assert
        IPEndPoint localEndPoint = connection.LocalEndPoint.ShouldBeOfType<IPEndPoint>();

        localEndPoint.Address.ShouldBe(IPAddress.Loopback);
        localEndPoint.Port.ShouldNotBe(0);
        connection.RemoteEndPoint.ShouldBeNull();
    }

    [Fact]
    public async Task Connect_ToBoundServerEndPoint_ShouldPopulateRemoteEndPoint()
    {
        // Arrange
        UdpConnectionFactory factory = new();

        await using IDatagramConnection server = factory.Bind(new IPEndPoint(IPAddress.Loopback, 0));

        // Act
        await using IDatagramConnection client = factory.Connect(server.LocalEndPoint);

        // Assert
        client.RemoteEndPoint.ShouldBe(server.LocalEndPoint);
        ((IPEndPoint)client.LocalEndPoint).Port.ShouldNotBe(0);
    }

    [Fact]
    public void Bind_WithNonIPEndPoint_ShouldThrowNotSupportedException()
    {
        // Arrange
        UdpConnectionFactory factory = new();

        UnixDomainSocketEndPoint endPoint = new("cohesion-udp-test.sock");

        // Act / Assert
        Should.Throw<NotSupportedException>(() => factory.Bind(endPoint));
    }

    [Fact]
    public void Connect_WithNonIPEndPoint_ShouldThrowNotSupportedException()
    {
        // Arrange
        UdpConnectionFactory factory = new();

        UnixDomainSocketEndPoint endPoint = new("cohesion-udp-test.sock");

        // Act / Assert
        Should.Throw<NotSupportedException>(() => factory.Connect(endPoint));
    }

    [Fact]
    public void Bind_WithNullArguments_ShouldThrowArgumentNullException()
    {
        // Arrange
        UdpConnectionFactory factory = new();

        // Act / Assert
        Should.Throw<ArgumentNullException>(() => factory.Bind((EndPoint)null!));
        Should.Throw<ArgumentNullException>(() => factory.Bind((UdpBindOptions)null!));
    }

    [Fact]
    public async Task Connect_WithNullArguments_ShouldThrowArgumentNullException()
    {
        // Arrange
        UdpConnectionFactory factory = new();

        // Act / Assert
        Should.Throw<ArgumentNullException>(() => factory.Connect(null!));
        Should.Throw<ArgumentNullException>(() => factory.Connect(new IPEndPoint(IPAddress.Loopback, 1), null!));

        await Should.ThrowAsync<ArgumentNullException>(async () => await factory.ConnectAsync(null!));
    }

    [Fact]
    public void Create_WithNullConfigure_ShouldThrowArgumentNullException()
    {
        // Arrange / Act / Assert
        Should.Throw<ArgumentNullException>(() => UdpConnectionFactory.Create(null!));
    }

    [Fact]
    public void Capabilities_OnFactory_ShouldDescribeUnreliableUnorderedUdpDatagram()
    {
        // Arrange
        UdpConnectionFactory factory = new();

        // Act
        ConnectionCapabilities capabilities = factory.Capabilities;

        // Assert
        capabilities.ShouldBe(new ConnectionCapabilities(
            ConnectionProtocol.Udp,
            ConnectionDelivery.Datagram,
            IsReliable: false,
            IsOrdered: false,
            IsMultiplexed: false,
            ConnectionSecurity.None));
    }

    [Fact]
    public async Task BindAsync_WithPreCanceledToken_ShouldReturnCanceledTask()
    {
        // Arrange
        using CancellationTokenSource cancellation = new(_testTimeout);
        using CancellationTokenSource preCanceled = new();

        preCanceled.Cancel();

        UdpConnectionFactory factory = UdpConnectionFactory.Create(
            options => options.EndPoint = new IPEndPoint(IPAddress.Loopback, 0));

        // Act
        Exception? exception = await Record.ExceptionAsync(async () => await factory.BindAsync(preCanceled.Token));

        // Assert
        exception.ShouldBeAssignableTo<OperationCanceledException>();
    }
}
```

## Walkthrough

- **Test entry point** — `Bind_WithEphemeralLoopbackEndPoint_ShouldPopulateLocalEndPointOnly` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `Connect_ToBoundServerEndPoint_ShouldPopulateRemoteEndPoint` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `Bind_WithNonIPEndPoint_ShouldThrowNotSupportedException` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `Connect_WithNonIPEndPoint_ShouldThrowNotSupportedException` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `Bind_WithNullArguments_ShouldThrowArgumentNullException` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `Connect_WithNullArguments_ShouldThrowArgumentNullException` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `Create_WithNullConfigure_ShouldThrowArgumentNullException` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `Capabilities_OnFactory_ShouldDescribeUnreliableUnorderedUdpDatagram` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `BindAsync_WithPreCanceledToken_ShouldReturnCanceledTask` contains the setup, invocation, and assertions for this case.

The assertions define the expected result or failure boundary. Test-only doubles supply controlled
inputs; they are not additional shipped APIs.

## Udp Connection Event Source Tests

### Code

```csharp
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using Shouldly;

using Xunit;

using Assimalign.Cohesion.Connections.Udp.Internal;
using Assimalign.Cohesion.DependencyInjection.Tests;

namespace Assimalign.Cohesion.Connections.Udp.Tests;

[Collection(nameof(EventSourceCollection))]
public class UdpConnectionEventSourceTests
{
    private static readonly TimeSpan _testTimeout = TimeSpan.FromSeconds(10);

    [Fact(DisplayName = "Cohesion Test [Connections.Udp] - UdpConnectionEventSource: Should be named for its assembly")]
    public void GetName_UdpConnectionEventSource_ShouldEqualAssemblyName()
    {
        // Act
        string name = EventSource.GetName(typeof(UdpConnectionEventSource));

        // Assert
        name.ShouldBe(typeof(UdpConnectionEventSource).Assembly.GetName().Name);
        name.ShouldBe("Assimalign.Cohesion.Connections.Udp");
    }

    [Fact(DisplayName = "Cohesion Test [Connections.Udp] - UdpConnectionEventSource: Should generate a manifest in strict mode")]
    [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode", Justification = "Test-only manifest validation reflects over the event source; test assemblies are not trimmed.")]
    [UnconditionalSuppressMessage("Trimming", "IL2111:DynamicallyAccessedMembers", Justification = "Test-only manifest validation reflects over the event source; test assemblies are not trimmed.")]
    public void GenerateManifest_StrictMode_ShouldSucceed()
    {
        // Act
        string? manifest = EventSource.GenerateManifest(typeof(UdpConnectionEventSource), assemblyPathToIncludeInManifest: null, EventManifestOptions.Strict);

        // Assert
        manifest.ShouldNotBeNull();
        manifest.ShouldContain("Assimalign.Cohesion.Connections.Udp", Case.Sensitive);
    }

    [Fact(DisplayName = "Cohesion Test [Connections.Udp] - UdpConnectionEventSource: Should report bound and connected sockets once each and restore the gauge")]
    public async Task ConnectionLifecycle_BoundAndConnectedSockets_ShouldReportEachTransitionOnceAndRestoreGauge()
    {
        // Arrange
        using EventSourceRecorder recorder = new(UdpConnectionEventSource.Log, EventLevel.Verbose);
        long currentBefore = UdpConnectionEventSource.Log.CurrentConnections;
        long totalBefore = UdpConnectionEventSource.Log.TotalConnections;
        UdpConnectionFactory factory = new();

        IDatagramConnection server = factory.Bind(new IPEndPoint(IPAddress.Loopback, 0));
        IDatagramConnection client = factory.Connect(server.LocalEndPoint);
        long currentWhileOpen = UdpConnectionEventSource.Log.CurrentConnections;

        // Act
        await client.DisposeAsync();
        await client.DisposeAsync();
        await server.DisposeAsync();

        // Assert
        currentWhileOpen.ShouldBe(currentBefore + 2);
        UdpConnectionEventSource.Log.TotalConnections.ShouldBe(totalBefore + 2);
        UdpConnectionEventSource.Log.CurrentConnections.ShouldBe(currentBefore);

        IReadOnlyList<EventWrittenEventArgs> events = recorder.Events;
        events.ShouldNotContain(e => e.EventId == 0, "EventSource reported an instrumentation error.");

        EventWrittenEventArgs[] opened = events.Where(e => e.EventName == "ConnectionOpened").ToArray();
        opened.Length.ShouldBe(2);
        opened[0].PayloadNames.ShouldBe(["connectionId", "mode", "localEndPoint", "remoteEndPoint"]);

        EventWrittenEventArgs serverOpened = opened.Where(e => Equals(e.Payload![1], "bind")).ShouldHaveSingleItem();
        serverOpened.Payload![2].ShouldBe(server.LocalEndPoint.ToString());
        serverOpened.Payload[3].ShouldBe(string.Empty);

        EventWrittenEventArgs clientOpened = opened.Where(e => Equals(e.Payload![1], "connect")).ShouldHaveSingleItem();
        clientOpened.Payload![3].ShouldBe(server.LocalEndPoint.ToString());

        // A second DisposeAsync is a no-op: each socket's close is reported exactly once.
        string[] closedIds = events.Where(e => e.EventName == "ConnectionClosed").Select(e => (string)e.Payload![0]!).ToArray();
        closedIds.Length.ShouldBe(2);
        closedIds.ShouldBe(opened.Select(e => (string)e.Payload![0]!).ToArray(), ignoreOrder: true);
    }

    [Fact(DisplayName = "Cohesion Test [Connections.Udp] - UdpConnectionEventSource: Should publish its connection counters")]
    public async Task Counters_EnabledWithInterval_ShouldPublishConnectionCounters()
    {
        // Arrange
        using CancellationTokenSource cancellation = new(_testTimeout);
        string[] counters = ["current-connections", "total-connections"];

        // Act
        using EventSourceRecorder recorder = new(UdpConnectionEventSource.Log, EventLevel.LogAlways, counterInterval: TimeSpan.FromMilliseconds(100));

        // Assert
        await recorder.WaitForCountersAsync(counters, cancellation.Token);
    }
}
```

### Walkthrough

- **Covered behavior** — UdpConnectionEventSource: Should be named for its assembly.
- **Covered behavior** — UdpConnectionEventSource: Should generate a manifest in strict mode.
- **Covered behavior** — UdpConnectionEventSource: Should report bound and connected sockets once each and restore the gauge.
- **Covered behavior** — UdpConnectionEventSource: Should publish its connection counters.

The assertions define the expected result or failure boundary. Test-only doubles supply controlled
inputs; they are not additional shipped APIs.

## Sources

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Udp/tests/UdpConnectionFactoryTests.cs`.
- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Udp/tests/Assimalign.Cohesion.Connections.Udp.Tests.csproj`.
- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Udp/tests/UdpConnectionEventSourceTests.cs`.
- **Source** — `cohesion/libraries/DependencyInjection/Assimalign.Cohesion.DependencyInjection/shared/EventSourceRecorder.cs`.
