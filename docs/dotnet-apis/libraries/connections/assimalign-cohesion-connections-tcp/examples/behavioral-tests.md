# Example: File Handle End Point Tests

Exercise File Handle End Point behavior using the original repository tests.

[Examples](index.md) · [Assembly overview](../index.md)

## Test-project context

This is the complete `FileHandleEndPointTests.cs` listing from the package test project. Keep it in
that project when running it: the project supplies its package references, generated sources, and
any shared fixtures. The using block below makes the test-framework import explicit where the
original project supplies it globally.

## Code

```csharp
using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using System;
using Shouldly;
using Xunit;

namespace Assimalign.Cohesion.Connections.Tcp.Tests;

/// <summary>
/// Covers the socket-activation / file-descriptor hand-off path (systemd <c>.socket</c>, launchd, or a
/// supervising parent process): the listener adopts an already-bound, already-listening socket by its
/// handle and accepts on it directly, without re-binding or re-listening.
/// </summary>
public class FileHandleEndPointTests
{
    private static readonly TimeSpan TestTimeout = TimeSpan.FromSeconds(5);

    [Fact(DisplayName = "Cohesion Test [Connections.Tcp] - FileHandle: Should adopt an inherited listening socket and accept on it")]
    public async Task AcceptAsync_WithFileHandleEndPoint_ShouldAdoptInheritedSocketAndAccept()
    {
        // Arrange — a "parent" has already bound and started listening; the child inherits the descriptor.
        using CancellationTokenSource cancellation = new(TestTimeout);

        Socket activated = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        activated.Bind(new IPEndPoint(IPAddress.Loopback, 0));
        activated.Listen(16);

        IPEndPoint bound = (IPEndPoint)activated.LocalEndPoint!;
        ulong fileHandle = (ulong)activated.SafeHandle.DangerousGetHandle();

        await using TcpConnectionListener listener = TcpConnectionListener.Create(
            options => options.EndPoint = new FileHandleEndPoint(fileHandle, FileHandleType.Tcp));

        ValueTask<Connection> acceptTask = listener.AcceptAsync(cancellation.Token);

        TcpConnectionFactory factory = new();

        // Act — dialing the inherited listening socket's address should be accepted by the adopting listener.
        await using Connection client = await factory.ConnectAsync(bound, cancellation.Token);
        await using Connection server = await acceptTask;

        // Assert
        server.ShouldNotBeNull();
        ((IPEndPoint)server.LocalEndPoint!).Port.ShouldBe(bound.Port);

        // The adopting listener owns the descriptor (ownsHandle: true) and closes it on dispose; mark the
        // simulated parent's handle invalid so it does not double-close the same descriptor.
        activated.SafeHandle.SetHandleAsInvalid();
    }
}
```

## Walkthrough

- **Covered behavior** — FileHandle: Should adopt an inherited listening socket and accept on it.

The assertions define the expected result or failure boundary. Test-only doubles supply controlled
inputs; they are not additional shipped APIs.

## Sources

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Tcp/tests/FileHandleEndPointTests.cs`.
- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Tcp/tests/Assimalign.Cohesion.Connections.Tcp.Tests.csproj`.
