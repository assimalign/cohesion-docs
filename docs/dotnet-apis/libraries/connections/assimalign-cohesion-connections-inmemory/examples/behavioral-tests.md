# Example: In Memory Connection Behavioral Tests

Exercise in-memory listeners, multiplexed streams, coded teardown, and peer-close behavior using the original repository tests.

[Examples](index.md) · [Assembly overview](../index.md)

## Test-project context

These are the complete `InMemoryConnectionListenerTests.cs`, `InMemoryMultiplexedConnectionTests.cs`,
`MultiplexedPeerCloseTests.cs`, and `MultiplexedRootTests.cs` listings from the package test project.
Keep them in that project when running them: the project supplies its package references, generated
sources, and any shared fixtures.

## Code

```csharp
using System;
using System.Threading;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace Assimalign.Cohesion.Connections.InMemory.Tests;

public class InMemoryConnectionListenerTests
{
    private static readonly TimeSpan _testTimeout = TimeSpan.FromSeconds(5);

    [Fact(DisplayName = "Cohesion Test [Connections.InMemory] - BindAsync: Logical bind should complete until listener disposal")]
    public async Task BindAsync_BeforeAndAfterDispose_ShouldRespectTerminalDisposal()
    {
        // Arrange
        InMemoryConnectionListener listener = new();

        // Act
        await listener.BindAsync();
        await listener.DisposeAsync();

        // Assert
        await Should.ThrowAsync<ObjectDisposedException>(async () => await listener.BindAsync());
    }

    [Fact(DisplayName = "Cohesion Test [Connections.InMemory] - Listener: Dial then accept should yield a connected pair")]
    public async Task Dial_ThenAccept_ShouldYieldConnectedPair()
    {
        // Arrange
        using CancellationTokenSource cancellation = new(_testTimeout);
        await using InMemoryConnectionListener listener = new();
        InMemoryConnectionFactory factory = listener.CreateFactory();

        // Act — a pending accept resolves as soon as the factory dials, mirroring a real listener.
        ValueTask<Connection> acceptTask = listener.AcceptAsync(cancellation.Token);

        Connection client = await factory.ConnectAsync(listener.EndPoint, cancellation.Token);
        Connection server = await acceptTask;

        byte[] payload = [7, 8, 9];
        await client.Output.WriteAsync(payload, cancellation.Token);
        byte[] received = await server.Input.ReadExactlyAsync(payload.Length, cancellation.Token);

        // Assert
        received.ShouldBe(payload);
        client.RemoteEndPoint.ShouldBe(listener.EndPoint);
        server.LocalEndPoint.ShouldBe(listener.EndPoint);

        await client.DisposeAsync();
        await server.DisposeAsync();
    }

    [Fact(DisplayName = "Cohesion Test [Connections.InMemory] - Listener: Accept should re-arm across multiple dials")]
    public async Task Accept_AcrossMultipleDials_ShouldReArm()
    {
        // Arrange
        using CancellationTokenSource cancellation = new(_testTimeout);
        await using InMemoryConnectionListener listener = new();
        InMemoryConnectionFactory factory = listener.CreateFactory();

        // Act & Assert
        for (int i = 0; i < 3; i++)
        {
            Connection client = await factory.ConnectAsync(listener.EndPoint, cancellation.Token);
            Connection server = await listener.AcceptAsync(cancellation.Token);

            server.ShouldNotBeNull();
            client.Id.ShouldNotBe(server.Id);

            await client.DisposeAsync();
            await server.DisposeAsync();
        }
    }

    [Fact(DisplayName = "Cohesion Test [Connections.InMemory] - Listener: Disposing should cancel a pending accept")]
    public async Task DisposeAsync_WithPendingAccept_ShouldCancelAccept()
    {
        // Arrange
        using CancellationTokenSource cancellation = new(_testTimeout);
        InMemoryConnectionListener listener = new();

        ValueTask<Connection> acceptTask = listener.AcceptAsync(cancellation.Token);

        // Act
        await listener.DisposeAsync();

        // Assert
        await Should.ThrowAsync<OperationCanceledException>(async () => await acceptTask);
    }

    [Fact(DisplayName = "Cohesion Test [Connections.InMemory] - Listener: Dialing a disposed listener should throw")]
    public async Task Connect_AfterDispose_ShouldThrowConnectionAborted()
    {
        // Arrange
        using CancellationTokenSource cancellation = new(_testTimeout);
        InMemoryConnectionListener listener = new();
        InMemoryConnectionFactory factory = listener.CreateFactory();

        await listener.DisposeAsync();

        // Act & Assert
        await Should.ThrowAsync<ConnectionAbortedException>(
            async () => await factory.ConnectAsync(listener.EndPoint, cancellation.Token));
    }

    [Fact(DisplayName = "Cohesion Test [Connections.InMemory] - Listener: Endpoint and capabilities should be honored")]
    public async Task Listener_EndPointAndCapabilities_ShouldBeHonored()
    {
        // Arrange
        InMemoryEndPoint endPoint = new("test-server");
        ConnectionCapabilities capabilities = InMemoryConnectionPair.DefaultCapabilities with
        {
            Security = ConnectionSecurity.Tls
        };

        // Act
        await using InMemoryConnectionListener listener = new(endPoint, capabilities);

        // Assert
        listener.EndPoint.ShouldBe(endPoint);
        listener.Capabilities.Security.ShouldBe(ConnectionSecurity.Tls);
        listener.CreateFactory().Capabilities.Security.ShouldBe(ConnectionSecurity.Tls);
    }
}
```

## Walkthrough

- **Covered behavior** — BindAsync: Logical bind should complete until listener disposal.
- **Covered behavior** — Listener: Dial then accept should yield a connected pair.
- **Covered behavior** — Listener: Accept should re-arm across multiple dials.
- **Covered behavior** — Listener: Disposing should cancel a pending accept.
- **Covered behavior** — Listener: Dialing a disposed listener should throw.
- **Covered behavior** — Listener: Endpoint and capabilities should be honored.

The assertions define the expected result or failure boundary. Test-only doubles supply controlled
inputs; they are not additional shipped APIs.

## In Memory Multiplexed Connection Tests

### Code

```csharp
using System;
using System.Threading;
using System.Threading.Tasks;

using Shouldly;

using Xunit;

namespace Assimalign.Cohesion.Connections.InMemory.Tests;

public class InMemoryMultiplexedConnectionTests
{
    private static readonly TimeSpan _testTimeout = TimeSpan.FromSeconds(5);

    [Fact(DisplayName = "Cohesion Test [Connections.InMemory] - Features: Both multiplexed stream ends register directional abort")]
    public async Task Features_OnMultiplexedStreams_ShouldRegisterDirectionalAbort()
    {
        // Arrange
        using CancellationTokenSource cancellation = new(_testTimeout);
        (MultiplexedConnection client, MultiplexedConnection server) = InMemoryMultiplexedConnectionPair.Create();
        await using (client)
        await using (server)
        {
            // Act
            await using MultiplexedStream clientStream = await client.OpenStreamAsync(cancellationToken: cancellation.Token);
            await using MultiplexedStream serverStream = await server.AcceptStreamAsync(cancellation.Token);
            IMultiplexedStreamAbort clientAbort = clientStream.DirectionalAbort.ShouldNotBeNull();
            IMultiplexedStreamAbort serverAbort = serverStream.DirectionalAbort.ShouldNotBeNull();

            // Assert
            clientAbort.Name.ShouldBe(IMultiplexedStreamAbort.FeatureName);
            serverAbort.Name.ShouldBe(IMultiplexedStreamAbort.FeatureName);
            clientStream.Features.Get(IMultiplexedStreamAbort.FeatureName).ShouldBeSameAs(clientAbort);
            serverStream.Features.Get(IMultiplexedStreamAbort.FeatureName).ShouldBeSameAs(serverAbort);
            clientAbort.ShouldBeSameAs(clientStream);
            serverAbort.ShouldBeSameAs(serverStream);
            client.Features.Get(IMultiplexedStreamAbort.FeatureName).ShouldBeNull();
            server.Features.Get(IMultiplexedStreamAbort.FeatureName).ShouldBeNull();
        }
    }

    [Fact(DisplayName = "Cohesion Test [Connections.InMemory] - Multiplexed BindAsync: Logical bind should complete until listener disposal")]
    public async Task BindAsync_BeforeAndAfterDispose_ShouldRespectTerminalDisposal()
    {
        // Arrange
        InMemoryMultiplexedConnectionListener listener = new();

        // Act
        await listener.BindAsync();
        await listener.DisposeAsync();

        // Assert
        await Should.ThrowAsync<ObjectDisposedException>(async () => await listener.BindAsync());
    }

    [Fact(DisplayName = "Cohesion Test [Connections.InMemory] - Multiplexed: Opening a stream should be accepted by the peer and round-trip")]
    public async Task OpenStream_ShouldBeAcceptedByPeerAndRoundTrip()
    {
        // Arrange
        using CancellationTokenSource cancellation = new(_testTimeout);
        (MultiplexedConnection client, MultiplexedConnection server) = InMemoryMultiplexedConnectionPair.Create();

        // Act
        MultiplexedStream clientStream = await client.OpenStreamAsync(cancellationToken: cancellation.Token);
        MultiplexedStream serverStream = await server.AcceptStreamAsync(cancellation.Token);

        byte[] payload = [4, 5, 6];
        await clientStream.Output.WriteAsync(payload, cancellation.Token);
        byte[] received = await serverStream.Input.ReadExactlyAsync(payload.Length, cancellation.Token);

        // Assert
        received.ShouldBe(payload);
        clientStream.Capabilities.IsMultiplexed.ShouldBeFalse();
        client.Capabilities.IsMultiplexed.ShouldBeTrue();

        await client.DisposeAsync();
        await server.DisposeAsync();
    }

    [Fact(DisplayName = "Cohesion Test [Connections.InMemory] - Multiplexed: Multiple streams should be independently accepted")]
    public async Task OpenStream_MultipleStreams_ShouldBeIndependentlyAccepted()
    {
        // Arrange
        using CancellationTokenSource cancellation = new(_testTimeout);
        (MultiplexedConnection client, MultiplexedConnection server) = InMemoryMultiplexedConnectionPair.Create();

        // Act
        MultiplexedStream first = await client.OpenStreamAsync(cancellationToken: cancellation.Token);
        MultiplexedStream second = await client.OpenStreamAsync(cancellationToken: cancellation.Token);

        MultiplexedStream serverFirst = await server.AcceptStreamAsync(cancellation.Token);
        MultiplexedStream serverSecond = await server.AcceptStreamAsync(cancellation.Token);

        // Assert
        first.Id.ShouldNotBe(second.Id);
        serverFirst.Id.ShouldNotBe(serverSecond.Id);

        await client.DisposeAsync();
        await server.DisposeAsync();
    }

    [Fact(DisplayName = "Cohesion Test [Connections.InMemory] - Multiplexed: An outbound unidirectional stream should be write-only for the opener and read-only for the peer")]
    public async Task OpenStream_WriteOnly_ShouldMirrorAsReadOnlyOnPeer()
    {
        // Arrange
        using CancellationTokenSource cancellation = new(_testTimeout);
        (MultiplexedConnection client, MultiplexedConnection server) = InMemoryMultiplexedConnectionPair.Create();

        // Act
        MultiplexedStream clientStream = await client.OpenStreamAsync(ConnectionDirection.WriteOnly, cancellation.Token);
        MultiplexedStream serverStream = await server.AcceptStreamAsync(cancellation.Token);

        byte[] payload = [1, 1, 2, 3, 5];
        await clientStream.Output.WriteAsync(payload, cancellation.Token);
        byte[] received = await serverStream.Input.ReadExactlyAsync(payload.Length, cancellation.Token);

        // Assert
        clientStream.Direction.ShouldBe(ConnectionDirection.WriteOnly);
        serverStream.Direction.ShouldBe(ConnectionDirection.ReadOnly);
        received.ShouldBe(payload);

        // The read-only peer end must reject writes.
        Should.Throw<InvalidOperationException>(() => serverStream.Output.GetMemory());

        await client.DisposeAsync();
        await server.DisposeAsync();
    }

    [Fact(DisplayName = "Cohesion Test [Connections.InMemory] - Multiplexed: Opening a read-only stream should throw")]
    public async Task OpenStream_ReadOnly_ShouldThrowArgumentException()
    {
        // Arrange
        (MultiplexedConnection client, MultiplexedConnection _) = InMemoryMultiplexedConnectionPair.Create();

        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(
            async () => await client.OpenStreamAsync(ConnectionDirection.ReadOnly));

        await client.DisposeAsync();
    }

    [Fact(DisplayName = "Cohesion Test [Connections.InMemory] - Multiplexed: Accepting after the connection closes should throw OperationCanceled")]
    public async Task AcceptStream_AfterDispose_ShouldThrowOperationCanceled()
    {
        // Arrange
        (MultiplexedConnection client, MultiplexedConnection _) = InMemoryMultiplexedConnectionPair.Create();

        // Act
        await client.DisposeAsync();

        // Assert
        client.State.ShouldBe(ConnectionState.Closed);
        await Should.ThrowAsync<OperationCanceledException>(async () => await client.AcceptStreamAsync());
    }

    [Fact(DisplayName = "Cohesion Test [Connections.InMemory] - Multiplexed: Listener dial then accept should yield a connected multiplexed pair")]
    public async Task Dial_ThenAccept_ShouldYieldMultiplexedPair()
    {
        // Arrange
        using CancellationTokenSource cancellation = new(_testTimeout);
        await using InMemoryMultiplexedConnectionListener listener = new();
        InMemoryMultiplexedConnectionFactory factory = listener.CreateFactory();

        // Act
        ValueTask<MultiplexedConnection> acceptTask = listener.AcceptAsync(cancellation.Token);

        MultiplexedConnection client = await factory.ConnectAsync(listener.EndPoint, cancellation.Token);
        MultiplexedConnection server = await acceptTask;

        MultiplexedStream clientStream = await client.OpenStreamAsync(cancellationToken: cancellation.Token);
        MultiplexedStream serverStream = await server.AcceptStreamAsync(cancellation.Token);

        byte[] payload = [42];
        await clientStream.Output.WriteAsync(payload, cancellation.Token);
        byte[] received = await serverStream.Input.ReadExactlyAsync(1, cancellation.Token);

        // Assert
        received.ShouldBe(payload);
        client.Capabilities.IsMultiplexed.ShouldBeTrue();

        await client.DisposeAsync();
        await server.DisposeAsync();
    }

    /// <summary>How the peer end of a stream abandons it.</summary>
    public enum StreamAbort
    {
        /// <summary>Aborts the stream: both directions.</summary>
        Abort,

        /// <summary>Completes its output with an error: the in-memory RESET_STREAM.</summary>
        ResetSending,

        /// <summary>Completes its input with an error: the in-memory STOP_SENDING.</summary>
        StopReceiving,
    }

    [Theory(DisplayName = "Cohesion Test [Connections.InMemory] - Multiplexed: A peer abandoning a stream should signal the other end's ConnectionClosed")]
    [InlineData(StreamAbort.Abort)]
    [InlineData(StreamAbort.ResetSending)]
    [InlineData(StreamAbort.StopReceiving)]
    public async Task StreamConnectionClosed_OnPeerAbandoningTheStream_ShouldFire(StreamAbort abort)
    {
        // Arrange
        using CancellationTokenSource cancellation = new(_testTimeout);
        (MultiplexedConnection client, MultiplexedConnection server) = InMemoryMultiplexedConnectionPair.Create();
        MultiplexedStream clientStream = await client.OpenStreamAsync(cancellationToken: cancellation.Token);
        MultiplexedStream serverStream = await server.AcceptStreamAsync(cancellation.Token);
        ConnectionAbortedException reason = new("The peer abandoned the stream.");

        // Act
        switch (abort)
        {
            case StreamAbort.Abort:
                clientStream.Abort(reason);
                break;

            case StreamAbort.ResetSending:
                clientStream.Output.Complete(reason);
                break;

            case StreamAbort.StopReceiving:
                clientStream.Input.Complete(reason);
                break;
        }

        // Assert — the other end learns of it without reading or writing; its own state is unchanged.
        serverStream.ConnectionClosed.IsCancellationRequested.ShouldBeTrue();
        serverStream.State.ShouldBe(ConnectionState.Open);

        await client.DisposeAsync();
        await server.DisposeAsync();
    }

    [Fact(DisplayName = "Cohesion Test [Connections.InMemory] - Multiplexed AbortRead: The peer's writes should fail with the caller's code")]
    public async Task AbortRead_WithErrorCode_ShouldFailThePeersWritesWithTheCode()
    {
        // Arrange
        using CancellationTokenSource cancellation = new(_testTimeout);
        (MultiplexedConnection client, MultiplexedConnection server) = InMemoryMultiplexedConnectionPair.Create();
        MultiplexedStream clientStream = await client.OpenStreamAsync(cancellationToken: cancellation.Token);
        MultiplexedStream serverStream = await server.AcceptStreamAsync(cancellation.Token);

        // Act — the in-memory STOP_SENDING.
        serverStream.DirectionalAbort.ShouldNotBeNull().AbortRead(0x100);

        // Assert — the client learns of it at once and its next write carries the code; the server's own
        // stream keeps its lifecycle and can still send.
        ConnectionResetException reset = await Should.ThrowAsync<ConnectionResetException>(
            async () => await clientStream.Output.WriteAsync(new byte[] { 1 }, cancellation.Token));

        reset.ApplicationErrorCode.ShouldBe(0x100);
        clientStream.ConnectionClosed.IsCancellationRequested.ShouldBeTrue();
        serverStream.State.ShouldBe(ConnectionState.Open);
        serverStream.ConnectionClosed.IsCancellationRequested.ShouldBeFalse();

        await serverStream.Output.WriteAsync(new byte[] { 7 }, cancellation.Token);
        (await clientStream.Input.ReadExactlyAsync(1, cancellation.Token)).ShouldBe(new byte[] { 7 });

        await client.DisposeAsync();
        await server.DisposeAsync();
    }

    [Fact(DisplayName = "Cohesion Test [Connections.InMemory] - Multiplexed AbortWrite: The peer's reads should fail with the caller's code")]
    public async Task AbortWrite_WithErrorCode_ShouldFailThePeersReadsWithTheCode()
    {
        // Arrange
        using CancellationTokenSource cancellation = new(_testTimeout);
        (MultiplexedConnection client, MultiplexedConnection server) = InMemoryMultiplexedConnectionPair.Create();
        MultiplexedStream clientStream = await client.OpenStreamAsync(cancellationToken: cancellation.Token);
        MultiplexedStream serverStream = await server.AcceptStreamAsync(cancellation.Token);
        ValueTask<System.IO.Pipelines.ReadResult> pendingRead = clientStream.Input.ReadAsync(cancellation.Token);

        // Act — the in-memory RESET_STREAM.
        serverStream.DirectionalAbort.ShouldNotBeNull().AbortWrite(0x10e);

        // Assert — the client's read in flight fails with the code.
        ConnectionResetException reset = await Should.ThrowAsync<ConnectionResetException>(async () => await pendingRead);

        reset.ApplicationErrorCode.ShouldBe(0x10e);
        clientStream.ConnectionClosed.IsCancellationRequested.ShouldBeTrue();
        serverStream.State.ShouldBe(ConnectionState.Open);

        await client.DisposeAsync();
        await server.DisposeAsync();
    }

    [Fact(DisplayName = "Cohesion Test [Connections.InMemory] - Multiplexed AbortRead: A read in flight on the aborting end should fail instead of hanging")]
    public async Task AbortRead_WithReadInFlight_ShouldFailTheRead()
    {
        // Arrange
        using CancellationTokenSource cancellation = new(_testTimeout);
        (MultiplexedConnection client, MultiplexedConnection server) = InMemoryMultiplexedConnectionPair.Create();
        MultiplexedStream clientStream = await client.OpenStreamAsync(cancellationToken: cancellation.Token);
        MultiplexedStream serverStream = await server.AcceptStreamAsync(cancellation.Token);
        Task<System.IO.Pipelines.ReadResult> pendingRead = serverStream.Input.ReadAsync(cancellation.Token).AsTask();

        // Act
        serverStream.DirectionalAbort.ShouldNotBeNull().AbortRead(0x100);

        // Assert — the waiting read and every later one fail; the client never had to send or end anything.
        await Should.ThrowAsync<ConnectionAbortedException>(() => pendingRead.WaitAsync(_testTimeout));
        await Should.ThrowAsync<ConnectionAbortedException>(async () => await serverStream.Input.ReadAsync(cancellation.Token));

        await client.DisposeAsync();
        await server.DisposeAsync();
    }

    [Fact(DisplayName = "Cohesion Test [Connections.InMemory] - Multiplexed abort: A stream later aborted should keep the codes its directions were aborted with")]
    public async Task Abort_AfterAbortingBothDirections_ShouldKeepTheCodes()
    {
        // Arrange
        using CancellationTokenSource cancellation = new(_testTimeout);
        (MultiplexedConnection client, MultiplexedConnection server) = InMemoryMultiplexedConnectionPair.Create();
        MultiplexedStream clientStream = await client.OpenStreamAsync(cancellationToken: cancellation.Token);
        MultiplexedStream serverStream = await server.AcceptStreamAsync(cancellation.Token);

        // Act — a reset with a code, then the lifecycle abort with a reason of its own.
        IMultiplexedStreamAbort abort = serverStream.DirectionalAbort.ShouldNotBeNull();
        abort.AbortWrite(0x10b);
        abort.AbortRead(0x10b);
        serverStream.Abort(new InvalidOperationException("the reason stays local"));

        // Assert
        ConnectionResetException read = await Should.ThrowAsync<ConnectionResetException>(
            async () => await clientStream.Input.ReadAsync(cancellation.Token));
        ConnectionResetException write = await Should.ThrowAsync<ConnectionResetException>(
            async () => await clientStream.Output.WriteAsync(new byte[] { 1 }, cancellation.Token));

        read.ApplicationErrorCode.ShouldBe(0x10b);
        write.ApplicationErrorCode.ShouldBe(0x10b);
        serverStream.State.ShouldBe(ConnectionState.Aborted);
        serverStream.ConnectionClosed.IsCancellationRequested.ShouldBeTrue();

        await client.DisposeAsync();
        await server.DisposeAsync();
    }

    [Fact(DisplayName = "Cohesion Test [Connections.InMemory] - Multiplexed abort: A direction a unidirectional stream lacks should not be signaled")]
    public async Task AbortReadAndWrite_OnMissingDirection_ShouldHaveNoEffect()
    {
        // Arrange — the opener writes only; the peer reads only.
        using CancellationTokenSource cancellation = new(_testTimeout);
        (MultiplexedConnection client, MultiplexedConnection server) = InMemoryMultiplexedConnectionPair.Create();
        MultiplexedStream writeOnly = await client.OpenStreamAsync(ConnectionDirection.WriteOnly, cancellation.Token);
        MultiplexedStream readOnly = await server.AcceptStreamAsync(cancellation.Token);

        // Act
        writeOnly.DirectionalAbort.ShouldNotBeNull().AbortRead(0x100);
        readOnly.DirectionalAbort.ShouldNotBeNull().AbortWrite(0x100);

        // Assert — neither end was told the other abandoned the stream, and the data still flows.
        writeOnly.ConnectionClosed.IsCancellationRequested.ShouldBeFalse();
        readOnly.ConnectionClosed.IsCancellationRequested.ShouldBeFalse();

        await writeOnly.Output.WriteAsync(new byte[] { 3 }, cancellation.Token);
        (await readOnly.Input.ReadExactlyAsync(1, cancellation.Token)).ShouldBe(new byte[] { 3 });

        await client.DisposeAsync();
        await server.DisposeAsync();
    }

    [Theory(DisplayName = "Cohesion Test [Connections.InMemory] - Multiplexed abort: A code outside the QUIC range should be rejected")]
    [InlineData(-1L)]
    [InlineData(1L << 62)]
    public async Task AbortReadAndWrite_WithCodeOutOfRange_ShouldThrowArgumentOutOfRange(long errorCode)
    {
        // Arrange
        using CancellationTokenSource cancellation = new(_testTimeout);
        (MultiplexedConnection client, MultiplexedConnection server) = InMemoryMultiplexedConnectionPair.Create();
        MultiplexedStream clientStream = await client.OpenStreamAsync(cancellationToken: cancellation.Token);
        IMultiplexedStreamAbort abort = clientStream.DirectionalAbort.ShouldNotBeNull();

        // Act / Assert
        Should.Throw<ArgumentOutOfRangeException>(() => abort.AbortRead(errorCode));
        Should.Throw<ArgumentOutOfRangeException>(() => abort.AbortWrite(errorCode));

        await client.DisposeAsync();
        await server.DisposeAsync();
    }

    [Fact(DisplayName = "Cohesion Test [Connections.InMemory] - Byte-stream pair: Should not offer a code-carrying abort")]
    public void ConnectionPair_OnByteStreamPair_ShouldNotImplementStreamAbort()
    {
        // Arrange / Act
        (Connection client, Connection server) = InMemoryConnectionPair.Create();

        // Assert — a single-stream transport has no per-direction code to carry.
        client.Features.Get(IMultiplexedStreamAbort.FeatureName).ShouldBeNull();
        server.Features.Get(IMultiplexedStreamAbort.FeatureName).ShouldBeNull();
    }

    [Fact(DisplayName = "Cohesion Test [Connections.InMemory] - Multiplexed: A peer ending its side of a stream should not signal the other end's ConnectionClosed")]
    public async Task StreamConnectionClosed_OnPeerGracefulHalfClose_ShouldNotFire()
    {
        // Arrange
        using CancellationTokenSource cancellation = new(_testTimeout);
        (MultiplexedConnection client, MultiplexedConnection server) = InMemoryMultiplexedConnectionPair.Create();
        MultiplexedStream clientStream = await client.OpenStreamAsync(cancellationToken: cancellation.Token);
        MultiplexedStream serverStream = await server.AcceptStreamAsync(cancellation.Token);

        // Act — the FIN.
        await clientStream.Output.CompleteAsync();

        // Assert
        serverStream.ConnectionClosed.IsCancellationRequested.ShouldBeFalse();

        await client.DisposeAsync();
        await server.DisposeAsync();
    }
}
```

### Walkthrough

- **Covered behavior** — Features: Both multiplexed stream ends register directional abort.
- **Covered behavior** — Multiplexed BindAsync: Logical bind should complete until listener disposal.
- **Covered behavior** — Multiplexed: Opening a stream should be accepted by the peer and round-trip.
- **Covered behavior** — Multiplexed: Multiple streams should be independently accepted.
- **Covered behavior** — Multiplexed: An outbound unidirectional stream should be write-only for the opener and read-only for the peer.
- **Covered behavior** — Multiplexed: Opening a read-only stream should throw.
- **Covered behavior** — Multiplexed: Accepting after the connection closes should throw OperationCanceled.
- **Covered behavior** — Multiplexed: Listener dial then accept should yield a connected multiplexed pair.
- **Covered behavior** — Multiplexed: A peer abandoning a stream should signal the other end's ConnectionClosed. Inline data: `(StreamAbort.Abort)`, `(StreamAbort.ResetSending)`, `(StreamAbort.StopReceiving)`.
- **Covered behavior** — Multiplexed AbortRead: The peer's writes should fail with the caller's code.
- **Covered behavior** — Multiplexed AbortWrite: The peer's reads should fail with the caller's code.
- **Covered behavior** — Multiplexed AbortRead: A read in flight on the aborting end should fail instead of hanging.
- **Covered behavior** — Multiplexed abort: A stream later aborted should keep the codes its directions were aborted with.
- **Covered behavior** — Multiplexed abort: A direction a unidirectional stream lacks should not be signaled.
- **Covered behavior** — Multiplexed abort: A code outside the QUIC range should be rejected. Inline data: `(-1L)`, `(1L << 62)`.
- **Covered behavior** — Byte-stream pair: Should not offer a code-carrying abort.
- **Covered behavior** — Multiplexed: A peer ending its side of a stream should not signal the other end's ConnectionClosed.

The assertions define the expected result or failure boundary. Test-only doubles supply controlled
inputs; they are not additional shipped APIs.

## Multiplexed Peer Close Tests

### Code

```csharp
using System;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;

using Shouldly;
using Xunit;

namespace Assimalign.Cohesion.Connections.InMemory.Tests;

public class MultiplexedPeerCloseTests
{
    [Theory(DisplayName = "Cohesion Test [Connections.InMemory] - Peer close: Either end aborting or disposing terminates accepts and active streams with its code")]
    [InlineData(true, true)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(false, false)]
    public async Task Close_FromEitherEnd_ShouldNotifyPeerAndFailActiveStreams(bool closeFromClient, bool abort)
    {
        // Arrange
        using CancellationTokenSource cancellation = new(TimeSpan.FromSeconds(5));
        (MultiplexedConnection client, MultiplexedConnection server) = InMemoryMultiplexedConnectionPair.Create();
        await using (client)
        await using (server)
        {
            MultiplexedConnection closer = closeFromClient ? client : server;
            MultiplexedConnection peer = closeFromClient ? server : client;
            await using MultiplexedStream local = await closer.OpenStreamAsync(cancellationToken: cancellation.Token);
            await using MultiplexedStream inbound = await peer.AcceptStreamAsync(cancellation.Token);
            await using MultiplexedStream outbound = await peer.OpenStreamAsync(cancellationToken: cancellation.Token);
            await using MultiplexedStream remote = await closer.AcceptStreamAsync(cancellation.Token);
            Task<MultiplexedStream> peerAccept = peer.AcceptStreamAsync(cancellation.Token).AsTask();
            Task<MultiplexedStream> localAccept = closer.AcceptStreamAsync(cancellation.Token).AsTask();
            Task<ReadResult> inboundRead = inbound.Input.ReadAsync(cancellation.Token).AsTask();
            Task<ReadResult> outboundRead = outbound.Input.ReadAsync(cancellation.Token).AsTask();
            long expectedCode = abort ? 42 : 0;

            // Act
            if (abort)
            {
                closer.Abort(expectedCode, new InvalidOperationException("Local close reason"));
            }
            else
            {
                await closer.DisposeAsync();
            }
            // Assert: notification must come from the driver, before either end is closed again.
            peer.ConnectionClosed.IsCancellationRequested.ShouldBeTrue();
            closer.ConnectionClosed.IsCancellationRequested.ShouldBeTrue();
            closer.Abort(99);
            peer.Abort(100);
            // A later close at either end cannot replace the first pair close.
            closer.State.ShouldBe(abort ? ConnectionState.Aborted : ConnectionState.Closed);
            peer.State.ShouldBe(ConnectionState.Aborted);
            await Should.ThrowAsync<OperationCanceledException>(() => peerAccept);
            await Should.ThrowAsync<OperationCanceledException>(() => localAccept);
            await Should.ThrowAsync<OperationCanceledException>(async () => await peer.AcceptStreamAsync(cancellation.Token));
            (await Should.ThrowAsync<ConnectionResetException>(() => inboundRead)).ApplicationErrorCode.ShouldBe(expectedCode);
            (await Should.ThrowAsync<ConnectionResetException>(() => outboundRead)).ApplicationErrorCode.ShouldBe(expectedCode);

            foreach (MultiplexedStream stream in new[] { local, inbound, outbound, remote })
            {
                stream.ConnectionClosed.IsCancellationRequested.ShouldBeTrue();
                stream.IsInputCompleted.ShouldBeFalse();
                (await Should.ThrowAsync<ConnectionResetException>(
                    async () => await stream.Input.ReadAsync(cancellation.Token))).ApplicationErrorCode.ShouldBe(expectedCode);
                (await Should.ThrowAsync<ConnectionResetException>(
                    async () => await stream.Output.WriteAsync(new byte[] { 1 }, cancellation.Token))).ApplicationErrorCode.ShouldBe(expectedCode);
            }
            cancellation.IsCancellationRequested.ShouldBeFalse();
        }
    }

    [Theory(DisplayName = "Cohesion Test [Connections.InMemory] - Peer close: Queued streams fail without being accepted after close")]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Dispose_WithQueuedStream_ShouldFailOpenerAndEndPeerAccept(bool closeFromClient)
    {
        // Arrange
        using CancellationTokenSource cancellation = new(TimeSpan.FromSeconds(5));
        (MultiplexedConnection client, MultiplexedConnection server) = InMemoryMultiplexedConnectionPair.Create();
        await using (client)
        await using (server)
        {
            MultiplexedConnection opener = closeFromClient ? client : server;
            MultiplexedConnection peer = closeFromClient ? server : client;
            await using MultiplexedStream stream = await opener.OpenStreamAsync(cancellationToken: cancellation.Token);

            // Act
            await opener.DisposeAsync();

            // Assert
            peer.ConnectionClosed.IsCancellationRequested.ShouldBeTrue();
            await Should.ThrowAsync<OperationCanceledException>(async () => await peer.AcceptStreamAsync(cancellation.Token));
            (await Should.ThrowAsync<ConnectionResetException>(
                async () => await stream.Input.ReadAsync(cancellation.Token))).ApplicationErrorCode.ShouldBe(0);
        }
    }

    [Fact(DisplayName = "Cohesion Test [Connections.InMemory] - Peer close: Connection callbacks see coded loss and can dispose their stream")]
    public async Task Abort_WithReentrantPeerCallback_ShouldPublishLossBeforeNotification()
    {
        // Arrange
        using CancellationTokenSource cancellation = new(TimeSpan.FromSeconds(5));
        (MultiplexedConnection client, MultiplexedConnection server) = InMemoryMultiplexedConnectionPair.Create();
        await using (client)
        await using (server)
        {
            await using MultiplexedStream local = await client.OpenStreamAsync(cancellationToken: cancellation.Token);
            await using MultiplexedStream remote = await server.AcceptStreamAsync(cancellation.Token);
            Exception? observed = null;
            bool called = false;
            using CancellationTokenRegistration registration = server.ConnectionClosed.Register(() =>
            {
                called = true;
                try
                {
                    remote.Output.GetMemory(1);
                }
                catch (ConnectionResetException exception)
                {
                    observed = exception;
                }
                remote.DisposeAsync().GetAwaiter().GetResult();
            });

            // Act
            client.Abort(73);

            // Assert
            called.ShouldBeTrue();
            observed.ShouldBeOfType<ConnectionResetException>().ApplicationErrorCode.ShouldBe(73);
            (await Should.ThrowAsync<ConnectionResetException>(
                async () => await remote.Input.ReadAsync(cancellation.Token))).ApplicationErrorCode.ShouldBe(73);
        }
    }

    [Theory(DisplayName = "Cohesion Test [Connections.InMemory] - Peer close: No-code abort uses zero from either end")]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Abort_WithoutCode_ShouldReportZeroConnectionLoss(bool closeFromClient)
    {
        // Arrange
        using CancellationTokenSource cancellation = new(TimeSpan.FromSeconds(5));
        (MultiplexedConnection client, MultiplexedConnection server) = InMemoryMultiplexedConnectionPair.Create();
        await using (client)
        await using (server)
        {
            MultiplexedConnection closer = closeFromClient ? client : server;
            MultiplexedConnection peer = closeFromClient ? server : client;
            await using MultiplexedStream local = await closer.OpenStreamAsync(cancellationToken: cancellation.Token);
            await using MultiplexedStream remote = await peer.AcceptStreamAsync(cancellation.Token);

            // Act
            closer.Abort(new InvalidOperationException("Only the code crosses the connection"));

            // Assert
            peer.ConnectionClosed.IsCancellationRequested.ShouldBeTrue();
            (await Should.ThrowAsync<ConnectionResetException>(
                async () => await remote.Input.ReadAsync(cancellation.Token))).ApplicationErrorCode.ShouldBe(0);
        }
    }

    [Fact(DisplayName = "Cohesion Test [Connections.InMemory] - Peer close: A prior FIN does not hide loss on an active stream")]
    public async Task IsInputCompleted_AfterParentLossFollowingFin_ShouldReportFalse()
    {
        // Arrange
        using CancellationTokenSource cancellation = new(TimeSpan.FromSeconds(5));
        (MultiplexedConnection client, MultiplexedConnection server) = InMemoryMultiplexedConnectionPair.Create();
        await using (client)
        await using (server)
        {
            await using MultiplexedStream local = await client.OpenStreamAsync(cancellationToken: cancellation.Token);
            await using MultiplexedStream remote = await server.AcceptStreamAsync(cancellation.Token);
            await local.Output.CompleteAsync();
            remote.IsInputCompleted.ShouldBeTrue();

            // Act
            client.Abort(81);

            // Assert
            remote.IsInputCompleted.ShouldBeFalse();
            (await Should.ThrowAsync<ConnectionResetException>(
                async () => await remote.Input.ReadAsync(cancellation.Token))).ApplicationErrorCode.ShouldBe(81);
        }
    }

    [Fact(DisplayName = "Cohesion Test [Connections.InMemory] - Peer close: Throwing callbacks cannot strand peer or stream notifications")]
    public async Task Abort_WithThrowingCallback_ShouldStillClosePeerAndStreams()
    {
        // Arrange
        using CancellationTokenSource cancellation = new(TimeSpan.FromSeconds(5));
        (MultiplexedConnection client, MultiplexedConnection server) = InMemoryMultiplexedConnectionPair.Create();
        await using (client)
        await using (server)
        {
            await using MultiplexedStream local = await client.OpenStreamAsync(cancellationToken: cancellation.Token);
            await using MultiplexedStream remote = await server.AcceptStreamAsync(cancellation.Token);
            using CancellationTokenRegistration registration = client.ConnectionClosed.Register(
                () => throw new InvalidOperationException("A consumer callback failed"));

            // Act
            Should.NotThrow(() => client.Abort(89));

            // Assert
            server.ConnectionClosed.IsCancellationRequested.ShouldBeTrue();
            local.ConnectionClosed.IsCancellationRequested.ShouldBeTrue();
            remote.ConnectionClosed.IsCancellationRequested.ShouldBeTrue();
            (await Should.ThrowAsync<ConnectionResetException>(
                async () => await remote.Input.ReadAsync(cancellation.Token))).ApplicationErrorCode.ShouldBe(89);
        }
    }

    [Fact(DisplayName = "Cohesion Test [Connections.InMemory] - Peer close: Racing stream creation cannot escape connection loss")]
    public async Task OpenStreamAsync_RacingPeerAbort_ShouldRejectOrReturnATerminatedStream()
    {
        // Arrange
        using CancellationTokenSource cancellation = new(TimeSpan.FromSeconds(10));
        for (int attempt = 0; attempt < 16; attempt++)
        {
            (MultiplexedConnection client, MultiplexedConnection server) = InMemoryMultiplexedConnectionPair.Create();
            await using (client)
            await using (server)
            {
                TaskCompletionSource start = new(TaskCreationOptions.RunContinuationsAsynchronously);
                MultiplexedStream? opened = null;
                ConnectionException? rejected = null;
                Task open = Task.Run(async () =>
                {
                    await start.Task;
                    try
                    {
                        opened = await client.OpenStreamAsync(cancellationToken: cancellation.Token);
                    }
                    catch (ConnectionException exception)
                    {
                        rejected = exception;
                    }
                });
                Task close = Task.Run(async () =>
                {
                    await start.Task;
                    server.Abort(91);
                });

                // Act
                start.SetResult();
                await Task.WhenAll(open, close).WaitAsync(cancellation.Token);

                // Assert
                client.ConnectionClosed.IsCancellationRequested.ShouldBeTrue();
                if (opened is not null)
                {
                    await using (opened)
                    {
                        (await Should.ThrowAsync<ConnectionResetException>(
                            async () => await opened.Input.ReadAsync(cancellation.Token))).ApplicationErrorCode.ShouldBe(91);
                    }
                }
                else
                {
                    rejected.ShouldBeOfType<ConnectionResetException>().ApplicationErrorCode.ShouldBe(91);
                }
            }
        }
    }
}
```

### Walkthrough

- **Covered behavior** — Peer close: Either end aborting or disposing terminates accepts and active streams with its code. Inline data: `(true, true)`, `(true, false)`, `(false, true)`, `(false, false)`.
- **Covered behavior** — Peer close: Queued streams fail without being accepted after close. Inline data: `(true)`, `(false)`.
- **Covered behavior** — Peer close: Connection callbacks see coded loss and can dispose their stream.
- **Covered behavior** — Peer close: No-code abort uses zero from either end. Inline data: `(true)`, `(false)`.
- **Covered behavior** — Peer close: A prior FIN does not hide loss on an active stream.
- **Covered behavior** — Peer close: Throwing callbacks cannot strand peer or stream notifications.
- **Covered behavior** — Peer close: Racing stream creation cannot escape connection loss.

The assertions define the expected result or failure boundary. Test-only doubles supply controlled
inputs; they are not additional shipped APIs.

## Multiplexed Root Tests

### Code

```csharp
using System;
using System.Threading;
using System.Threading.Tasks;

using Shouldly;
using Xunit;

using Assimalign.Cohesion.Connections.InMemory.Internal;

namespace Assimalign.Cohesion.Connections.InMemory.Tests;

public class MultiplexedRootTests
{
    [Theory(DisplayName = "Cohesion Test [Connections.InMemory] - Root stream abort: Throwing callbacks cannot prevent peer notification or stream removal")]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Abort_WithThrowingStreamCallback_ShouldNotifyPeerAndRemoveStream(bool throwsOnPeer)
    {
        // Arrange
        using CancellationTokenSource cancellation = new(TimeSpan.FromSeconds(5));
        int localClosed = 0;
        int peerClosed = 0;
        var (local, peer) = InMemoryConnection.CreateStreamPair(
            InMemoryConnectionPair.DefaultCapabilities,
            endPointA: null,
            endPointB: null,
            ConnectionDirection.Bidirectional,
            onClosedA: _ => localClosed++,
            onClosedB: _ => peerClosed++);
        await using (local)
        await using (peer)
        {
            using CancellationTokenRegistration registration = (throwsOnPeer ? peer : local).ConnectionClosed.Register(
                () => throw new InvalidOperationException("A consumer callback failed"));

            // Act
            Should.NotThrow(() => local.Abort(42));

            // Assert
            local.State.ShouldBe(ConnectionState.Aborted);
            local.ConnectionClosed.IsCancellationRequested.ShouldBeTrue();
            peer.ConnectionClosed.IsCancellationRequested.ShouldBeTrue();
            localClosed.ShouldBe(1);
            peerClosed.ShouldBe(0);
            (await Should.ThrowAsync<ConnectionResetException>(
                async () => await peer.Input.ReadAsync(cancellation.Token))).ApplicationErrorCode.ShouldBe(42);
            (await Should.ThrowAsync<ConnectionResetException>(
                async () => await peer.Output.WriteAsync(new byte[] { 1 }, cancellation.Token))).ApplicationErrorCode.ShouldBe(42);
            local.Abort(43);
            await local.DisposeAsync();
            localClosed.ShouldBe(1);
        }
    }

    [Theory(DisplayName = "Cohesion Test [Connections.InMemory] - Root stream abort: Both directions retain their first code and the lifecycle ends")]
    [InlineData(false, false)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(true, true)]
    public async Task Abort_WithStreamCode_ShouldEndBothDirectionsAndPreserveFirstCode(bool alreadyResetWrite, bool alreadyResetRead)
    {
        // Arrange
        using CancellationTokenSource cancellation = new(TimeSpan.FromSeconds(5));
        (MultiplexedConnection client, MultiplexedConnection server) = InMemoryMultiplexedConnectionPair.Create();
        await using (client)
        await using (server)
        {
            await using MultiplexedStream sender = await client.OpenStreamAsync(cancellationToken: cancellation.Token);
            await using MultiplexedStream receiver = await server.AcceptStreamAsync(cancellation.Token);
            if (alreadyResetWrite)
            {
                sender.DirectionalAbort.ShouldNotBeNull().AbortWrite(17);
            }
            if (alreadyResetRead)
            {
                sender.DirectionalAbort.ShouldNotBeNull().AbortRead(19);
            }

            // Act
            sender.Abort(42, new InvalidOperationException("Local reason"));
            sender.Abort(43);
            await sender.DisposeAsync();

            // Assert
            ConnectionResetException read = await Should.ThrowAsync<ConnectionResetException>(
                async () => await receiver.Input.ReadAsync(cancellation.Token));
            ConnectionResetException write = await Should.ThrowAsync<ConnectionResetException>(
                async () => await receiver.Output.WriteAsync(new byte[] { 1 }, cancellation.Token));
            read.ApplicationErrorCode.ShouldBe(alreadyResetWrite ? 17 : 42);
            write.ApplicationErrorCode.ShouldBe(alreadyResetRead ? 19 : 42);
            sender.State.ShouldBe(ConnectionState.Aborted);
            sender.ConnectionClosed.IsCancellationRequested.ShouldBeTrue();
            receiver.IsInputCompleted.ShouldBeFalse();
        }
    }

    [Fact(DisplayName = "Cohesion Test [Connections.InMemory] - Root stream abort: Disposal retains its clean close")]
    public async Task Abort_AfterStreamDisposal_ShouldPreserveCleanEnd()
    {
        // Arrange
        using CancellationTokenSource cancellation = new(TimeSpan.FromSeconds(5));
        (MultiplexedConnection client, MultiplexedConnection server) = InMemoryMultiplexedConnectionPair.Create();
        await using (client)
        await using (server)
        {
            await using MultiplexedStream sender = await client.OpenStreamAsync(cancellationToken: cancellation.Token);
            await using MultiplexedStream receiver = await server.AcceptStreamAsync(cancellation.Token);
            await sender.DisposeAsync();

            // Act
            sender.Abort(42);

            // Assert
            sender.State.ShouldBe(ConnectionState.Closed);
            receiver.IsInputCompleted.ShouldBeTrue();
            var read = await receiver.Input.ReadAsync(cancellation.Token);
            read.IsCompleted.ShouldBeTrue();
            receiver.Input.AdvanceTo(read.Buffer.End);
        }
    }

    [Theory(DisplayName = "Cohesion Test [Connections.InMemory] - Input completion: Only clean peer output completion is known complete")]
    [InlineData(false)]
    [InlineData(true)]
    public async Task IsInputCompleted_AfterPeerOutputCompletion_ShouldReflectCleanEnd(bool failed)
    {
        // Arrange
        using CancellationTokenSource cancellation = new(TimeSpan.FromSeconds(5));
        (MultiplexedConnection client, MultiplexedConnection server) = InMemoryMultiplexedConnectionPair.Create();
        await using (client)
        await using (server)
        {
            await using MultiplexedStream sender = await client.OpenStreamAsync(cancellationToken: cancellation.Token);
            await using MultiplexedStream receiver = await server.AcceptStreamAsync(cancellation.Token);
            receiver.IsInputCompleted.ShouldBeFalse();
            await sender.Output.WriteAsync(new byte[] { 1 }, cancellation.Token);

            // Act: FIN can be known before buffered bytes have been consumed.
            await sender.Output.CompleteAsync(failed ? new ConnectionAbortedException() : null);

            // Assert
            receiver.IsInputCompleted.ShouldBe(!failed);
            if (!failed)
            {
                (await receiver.Input.ReadExactlyAsync(1, cancellation.Token)).ShouldBe(new byte[] { 1 });
                receiver.DirectionalAbort.ShouldNotBeNull().AbortRead(42);
                receiver.IsInputCompleted.ShouldBeFalse();
            }
        }
    }

    [Theory(DisplayName = "Cohesion Test [Connections.InMemory] - Input completion: A later peer FIN cannot hide a local stream abort")]
    [InlineData(false, false)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public async Task IsInputCompleted_AfterLocalAbortThenPeerFin_ShouldRemainFalse(bool completeInputFirst, bool codedAbort)
    {
        // Arrange
        using CancellationTokenSource cancellation = new(TimeSpan.FromSeconds(5));
        (MultiplexedConnection client, MultiplexedConnection server) = InMemoryMultiplexedConnectionPair.Create();
        await using (client)
        await using (server)
        {
            await using MultiplexedStream sender = await client.OpenStreamAsync(cancellationToken: cancellation.Token);
            await using MultiplexedStream receiver = await server.AcceptStreamAsync(cancellation.Token);
            if (completeInputFirst)
            {
                receiver.Input.Complete();
            }

            // Act
            if (codedAbort)
            {
                receiver.Abort(42);
            }
            else
            {
                receiver.Abort();
            }
            receiver.IsInputCompleted.ShouldBeFalse();
            await sender.Output.CompleteAsync();

            // Assert
            receiver.IsInputCompleted.ShouldBeFalse();
        }
    }

    [Theory(DisplayName = "Cohesion Test [Connections.InMemory] - Input completion: A later peer FIN cannot hide a failed local input completion")]
    [InlineData(false)]
    [InlineData(true)]
    public async Task IsInputCompleted_AfterFailedLocalInputThenPeerFin_ShouldRemainFalse(bool completeAsync)
    {
        // Arrange
        using CancellationTokenSource cancellation = new(TimeSpan.FromSeconds(5));
        (MultiplexedConnection client, MultiplexedConnection server) = InMemoryMultiplexedConnectionPair.Create();
        await using (client)
        await using (server)
        {
            await using MultiplexedStream sender = await client.OpenStreamAsync(cancellationToken: cancellation.Token);
            await using MultiplexedStream receiver = await server.AcceptStreamAsync(cancellation.Token);
            InvalidOperationException failure = new("The local reader failed");

            // Act
            if (completeAsync)
            {
                await receiver.Input.CompleteAsync(failure);
            }
            else
            {
                receiver.Input.Complete(failure);
            }
            receiver.IsInputCompleted.ShouldBeFalse();
            await sender.Output.CompleteAsync();

            // Assert
            receiver.IsInputCompleted.ShouldBeFalse();
        }
    }

    [Theory(DisplayName = "Cohesion Test [Connections.InMemory] - Coded abort: Roots reject codes outside the driver range")]
    [InlineData(-1L)]
    [InlineData(1L << 62)]
    public async Task Abort_WithInvalidRootCode_ShouldRejectWithoutClosing(long code)
    {
        // Arrange
        using CancellationTokenSource cancellation = new(TimeSpan.FromSeconds(5));
        (MultiplexedConnection client, MultiplexedConnection server) = InMemoryMultiplexedConnectionPair.Create();
        await using (client)
        await using (server)
        {
            await using MultiplexedStream stream = await client.OpenStreamAsync(cancellationToken: cancellation.Token);

            // Act / Assert
            Should.Throw<ArgumentOutOfRangeException>(() => stream.Abort(code));
            Should.Throw<ArgumentOutOfRangeException>(() => client.Abort(code));
            stream.State.ShouldBe(ConnectionState.Open);
            client.State.ShouldBe(ConnectionState.Open);
        }
    }
}
```

### Walkthrough

- **Covered behavior** — Root stream abort: Throwing callbacks cannot prevent peer notification or stream removal. Inline data: `(false)`, `(true)`.
- **Covered behavior** — Root stream abort: Both directions retain their first code and the lifecycle ends. Inline data: `(false, false)`, `(true, false)`, `(false, true)`, `(true, true)`.
- **Covered behavior** — Root stream abort: Disposal retains its clean close.
- **Covered behavior** — Input completion: Only clean peer output completion is known complete. Inline data: `(false)`, `(true)`.
- **Covered behavior** — Input completion: A later peer FIN cannot hide a local stream abort. Inline data: `(false, false)`, `(true, false)`, `(true, true)`.
- **Covered behavior** — Input completion: A later peer FIN cannot hide a failed local input completion. Inline data: `(false)`, `(true)`.
- **Covered behavior** — Coded abort: Roots reject codes outside the driver range. Inline data: `(-1L)`, `(1L << 62)`.

The assertions define the expected result or failure boundary. Test-only doubles supply controlled
inputs; they are not additional shipped APIs.

## Sources

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.InMemory/tests/InMemoryConnectionListenerTests.cs`.
- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.InMemory/tests/Assimalign.Cohesion.Connections.InMemory.Tests.csproj`.
- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.InMemory/tests/InMemoryMultiplexedConnectionTests.cs`.
- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.InMemory/tests/MultiplexedPeerCloseTests.cs`.
- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.InMemory/tests/MultiplexedRootTests.cs`.
