# Example: Http Connection Hierarchy Tests

Exercise Http Connection Hierarchy behavior using the original repository tests.

[Examples](index.md) · [Assembly overview](../index.md)

## Test-project context

This is the complete `HttpConnectionHierarchyTests.cs` listing from the package test project. Keep it
in that project when running it: the project supplies its package references, generated sources, and
any shared fixtures. The using block below makes the test-framework import explicit where the
original project supplies it globally.

## Code

```csharp
using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Threading;
using System.Threading.Tasks;

using Shouldly;
using Xunit;

using Assimalign.Cohesion.Connections;
using Assimalign.Cohesion.Connections.InMemory;
using Assimalign.Cohesion.Http.Connections.Tests.TestObjects;

namespace Assimalign.Cohesion.Http.Connections.Tests;

public class HttpConnectionHierarchyTests
{
    [Theory(DisplayName = "Cohesion Test [Http.Connections] - Lifecycle: HTTP connections forward identity and close on disposal, abort and peer close")]
    [InlineData(HttpProtocol.Http11, "dispose")]
    [InlineData(HttpProtocol.Http11, "abort")]
    [InlineData(HttpProtocol.Http11, "peer")]
    [InlineData(HttpProtocol.Http20, "dispose")]
    [InlineData(HttpProtocol.Http20, "abort")]
    [InlineData(HttpProtocol.Http20, "peer")]
    [InlineData(HttpProtocol.Http30, "dispose")]
    [InlineData(HttpProtocol.Http30, "abort")]
    [InlineData(HttpProtocol.Http30, "peer")]
    public async Task ConnectionClosed_OnLifecycleEnd_ShouldSignalAndRemoveAcceptedConnection(HttpProtocol protocol, string operation)
    {
        // Arrange
        using CancellationTokenSource timeout = new(TimeSpan.FromSeconds(10));
        IConnection transport;
        IConnection peer;
        HttpConnectionListenerOptions options = new();
        if (protocol == HttpProtocol.Http30)
        {
            (MultiplexedConnection server, MultiplexedConnection client) = InMemoryMultiplexedConnectionPair.Create();
            transport = server;
            peer = client;
            options.UseHttp3(new TestMultiplexedConnectionListener(server));
        }
        else
        {
            (DuplexConnection server, DuplexConnection client) = InMemoryConnectionPair.Create();
            transport = server;
            peer = client;
            if (protocol == HttpProtocol.Http11)
            {
                options.UseHttp1(new TestConnectionListener(server));
            }
            else
            {
                options.UseHttp2(new TestConnectionListener(server));
            }
        }
        await using IConnection ownedPeer = peer;
        await using HttpConnectionListener listener = new(options);
        await using IHttpConnection connection = await listener.AcceptAsync(timeout.Token);
        CancellationToken closed = connection.ConnectionClosed;
        TaskCompletionSource signaled = new(TaskCreationOptions.RunContinuationsAsynchronously);
        using CancellationTokenRegistration registration = closed.Register(() => signaled.TrySetResult());
        listener.Connections.Count.ShouldBe(1);
        connection.Protocol.ShouldBe(protocol);
        connection.Id.ShouldBe(transport.Id);
        connection.LocalEndPoint.ShouldBe(transport.LocalEndPoint);
        connection.RemoteEndPoint.ShouldBe(transport.RemoteEndPoint);
        connection.Capabilities.ShouldBe(transport.Capabilities);
        connection.Features.ShouldBeSameAs(transport.Features);

        // Act
        switch (operation)
        {
            case "dispose":
                await connection.DisposeAsync();
                closed.IsCancellationRequested.ShouldBeTrue();
                break;
            case "abort":
                connection.Abort();
                break;
            default:
                await peer.DisposeAsync();
                break;
        }
        await signaled.Task.WaitAsync(timeout.Token);
        await connection.DisposeAsync();
        closed.IsCancellationRequested.ShouldBeTrue();
        await connection.DisposeAsync();
        closed.IsCancellationRequested.ShouldBeTrue();

        // Assert
        closed.IsCancellationRequested.ShouldBeTrue();
        listener.Connections.Count.ShouldBe(0);
    }

    [Fact(DisplayName = "Cohesion Test [Http.Connections] - DisposeAsync: HTTP2 output cleanup failure still disposes the transport")]
    public async Task DisposeAsync_Http2OutputCompletionFails_ShouldCloseTransportAndRemoveConnection()
    {
        // Arrange
        using CancellationTokenSource timeout = new(TimeSpan.FromSeconds(10));
        TestConnection transport = new(completeInput: false);
        InvalidOperationException failure = new("output completion failed");
        transport.OutputOverride = new ObservingPipeWriter(transport.Output)
        {
            CompletingAsync = () => ValueTask.FromException(failure)
        };
        await using HttpConnectionListener listener = HttpConnectionListener.Create(options =>
            options.UseHttp2(new TestConnectionListener(transport)));
        await using IHttpConnection connection = await listener.AcceptAsync(timeout.Token);
        CancellationToken closed = connection.ConnectionClosed;
        connection.Open();
        listener.Connections.Count.ShouldBe(1);

        // Act
        Exception? observed = await Record.ExceptionAsync(() => connection.DisposeAsync().AsTask().WaitAsync(timeout.Token));

        // Assert
        observed.ShouldBeSameAs(failure);
        transport.IsDisposed.ShouldBeTrue();
        closed.IsCancellationRequested.ShouldBeTrue();
        listener.Connections.ShouldBeEmpty();
    }

    [Fact(DisplayName = "Cohesion Test [Http.Connections] - DisposeAsync: HTTP3 GOAWAY failure still disposes the transport")]
    public async Task DisposeAsync_Http3ControlWriteFails_ShouldCloseTransportAndRemoveConnection()
    {
        // Arrange
        using CancellationTokenSource timeout = new(TimeSpan.FromSeconds(10));
        TestMultiplexedConnection transport = new();
        await using HttpConnectionListener listener = HttpConnectionListener.Create(options =>
            options.UseHttp3(new TestMultiplexedConnectionListener(transport)));
        await using IHttpConnection connection = await listener.AcceptAsync(timeout.Token);
        CancellationToken closed = connection.ConnectionClosed;
        IHttpConnectionContext context = connection.Open();
        await using IAsyncEnumerator<IHttpContext> requests = context.ReceiveAsync(timeout.Token).GetAsyncEnumerator();
        (await requests.MoveNextAsync()).ShouldBeFalse();
        TestConnection controlStream = transport.ControlStream.ShouldNotBeNull();
        InvalidOperationException failure = new("control write failed");
        controlStream.OutputOverride = new ObservingPipeWriter(controlStream.Output)
        {
            WritingAsync = _ => ValueTask.FromException(failure)
        };
        listener.Connections.Count.ShouldBe(1);

        // Act
        Exception? observed = await Record.ExceptionAsync(() => connection.DisposeAsync().AsTask().WaitAsync(timeout.Token));

        // Assert
        observed.ShouldBeSameAs(failure);
        transport.IsDisposed.ShouldBeTrue();
        closed.IsCancellationRequested.ShouldBeTrue();
        listener.Connections.ShouldBeEmpty();
    }

    [Fact(DisplayName = "Cohesion Test [Http.Connections] - Features: TLS is readable through the HTTP connection root")]
    public async Task Features_OnTlsTransport_ShouldExposeTheTransportCollection()
    {
        // Arrange
        TestTlsConnection transport = new([], SslApplicationProtocol.Http11);
        await using HttpConnectionListener listener = HttpConnectionListener.Create(options =>
            options.UseHttp1(new TestConnectionListener(TestTlsConnection.TlsCapabilities, transport)));

        // Act
        await using IHttpConnection connection = await listener.AcceptAsync(CancellationToken.None);

        // Assert
        connection.Features.ShouldBeSameAs(transport.Features);
        connection.Tls.ShouldBeSameAs(transport);
    }

    [Fact(DisplayName = "Cohesion Test [Http.Connections] - Accept: Root and HTTP accepts share a queue and one live collection")]
    public async Task AcceptAsync_ThroughBothInterfaces_ShouldShareTheQueueAndCollection()
    {
        // Arrange
        TestConnection first = new(completeInput: false);
        TestConnection second = new(completeInput: false);
        await using HttpConnectionListener listener = HttpConnectionListener.Create(options =>
            options.UseHttp1(new TestConnectionListener(first, second)));
        IConnectionListener root = listener;
        IConnectionListener<IHttpConnection> typed = listener;

        // Act
        await using IConnection firstAccepted = await root.AcceptAsync(CancellationToken.None);
        await using IHttpConnection secondAccepted = await typed.AcceptAsync(CancellationToken.None);
        await listener.DisposeAsync();

        // Assert
        firstAccepted.Id.ShouldBe(first.Id);
        secondAccepted.Id.ShouldBe(second.Id);
        root.Connections.ShouldBeSameAs(typed.Connections);
        root.Connections.Count.ShouldBe(2);
        firstAccepted.ConnectionClosed.IsCancellationRequested.ShouldBeFalse();
        secondAccepted.ConnectionClosed.IsCancellationRequested.ShouldBeFalse();
    }
}
```

## Walkthrough

- **Covered behavior** — Lifecycle: HTTP/1.1 connections forward identity and close on disposal (`HttpProtocol.Http11`, `dispose`).
- **Covered behavior** — Lifecycle: HTTP/1.1 connections forward identity and close on abort (`HttpProtocol.Http11`, `abort`).
- **Covered behavior** — Lifecycle: HTTP/1.1 connections forward identity and close on peer close (`HttpProtocol.Http11`, `peer`).
- **Covered behavior** — Lifecycle: HTTP/2 connections forward identity and close on disposal (`HttpProtocol.Http20`, `dispose`).
- **Covered behavior** — Lifecycle: HTTP/2 connections forward identity and close on abort (`HttpProtocol.Http20`, `abort`).
- **Covered behavior** — Lifecycle: HTTP/2 connections forward identity and close on peer close (`HttpProtocol.Http20`, `peer`).
- **Covered behavior** — Lifecycle: HTTP/3 connections forward identity and close on disposal (`HttpProtocol.Http30`, `dispose`).
- **Covered behavior** — Lifecycle: HTTP/3 connections forward identity and close on abort (`HttpProtocol.Http30`, `abort`).
- **Covered behavior** — Lifecycle: HTTP/3 connections forward identity and close on peer close (`HttpProtocol.Http30`, `peer`).
- **Covered behavior** — DisposeAsync: HTTP2 output cleanup failure still disposes the transport.
- **Covered behavior** — DisposeAsync: HTTP3 GOAWAY failure still disposes the transport.
- **Covered behavior** — Features: TLS is readable through the HTTP connection root.
- **Covered behavior** — Accept: Root and HTTP accepts share a queue and one live collection.

The assertions define the expected result or failure boundary. Test-only doubles supply controlled
inputs; they are not additional shipped APIs.

## Sources

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Connections/tests/HttpConnectionHierarchyTests.cs`.
- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Connections/tests/Assimalign.Cohesion.Http.Connections.Tests.csproj`.
