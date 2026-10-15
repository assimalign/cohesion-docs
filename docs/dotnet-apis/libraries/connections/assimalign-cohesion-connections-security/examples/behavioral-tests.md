# Example: TLS Behavioral Tests

Exercise TLS layers, composition, certificate policy, handshake facts, lifecycle, options, feature propagation, and handshake isolation using the original repository tests.

[Examples](index.md) · [Assembly overview](../index.md)

## Test-project context

These are the complete listings from the package test project:

- `TlsConnectionLayerTests.cs`
- `TlsClientCertificateTests.cs`
- `TlsConnectionInfoTests.cs`
- `TlsConnectionTests.cs`
- `TlsHandshakeIsolationTests.cs`
- `TlsConnectionExtensionsTests.cs`
- `TlsOptionsTests.cs`

Keep them in that project when running them: the project supplies its package references, generated
sources, and shared fixtures, including `TestCertificateFixture`, `TestConnectionFeature`,
`TestConnectionListener`, `TestConnectionFactory`, and `BlockingConnectionListener`.

## Code

```csharp
using System;
using System.Net.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Shouldly;

using Xunit;

using Assimalign.Cohesion.Connections;
using Assimalign.Cohesion.Connections.InMemory;

namespace Assimalign.Cohesion.Connections.Security.Tests;

public class TlsConnectionLayerTests : IClassFixture<TestCertificateFixture>
{
    private static readonly TimeSpan _testTimeout = TimeSpan.FromSeconds(5);

    private readonly TestCertificateFixture _fixture;

    public TlsConnectionLayerTests(TestCertificateFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void Describe_WithUnsecuredCapabilities_ShouldFlipSecurityToTlsAndPreserveRest()
    {
        // Arrange
        TlsConnectionLayer layer = new(new TlsServerOptions());
        ConnectionCapabilities capabilities = InMemoryConnectionPair.DefaultCapabilities;

        // Act
        ConnectionCapabilities described = layer.Describe(capabilities);

        // Assert
        described.ShouldBe(capabilities with { Security = ConnectionSecurity.Tls });
    }

    [Fact]
    public async Task UpgradeAsync_WithServerOptions_ShouldCompleteServerSideHandshake()
    {
        // Arrange
        using CancellationTokenSource timeout = new(_testTimeout);
        (IDuplexConnection client, IDuplexConnection server) = InMemoryConnectionPair.Create();
        TlsConnectionLayer layer = new(new TlsServerOptions
        {
            AuthenticationOptions =
            {
                ServerCertificate = _fixture.Certificate
            }
        });
        await using SslStream clientSsl = new(
            client.AsStream(),
            leaveInnerStreamOpen: false,
            userCertificateValidationCallback: static (_, _, _, _) => true);

        // Act: drive the layer's server handshake against a raw client SslStream on the far end.
        Task<IDuplexConnection> serverUpgrade = layer.UpgradeAsync(server, timeout.Token).AsTask();
        Task clientHandshake = clientSsl.AuthenticateAsClientAsync(
            new SslClientAuthenticationOptions { TargetHost = "localhost" },
            timeout.Token);
        await Task.WhenAll(serverUpgrade, clientHandshake);
        IDuplexConnection secured = await serverUpgrade;

        // Assert
        secured.Capabilities.Security.ShouldBe(ConnectionSecurity.Tls);

        // Bytes the raw client writes arrive decrypted on the upgraded connection.
        byte[] request = Encoding.UTF8.GetBytes("ping");
        await clientSsl.WriteAsync(request, timeout.Token);
        await clientSsl.FlushAsync(timeout.Token);
        byte[] received = await secured.Input.ReadExactlyAsync(request.Length, timeout.Token);
        received.ShouldBe(request);

        // And bytes the upgraded connection writes arrive decrypted at the raw client.
        byte[] response = Encoding.UTF8.GetBytes("pong");
        await secured.Output.WriteAsync(response, timeout.Token);
        byte[] clientReceived = new byte[response.Length];
        await clientSsl.ReadExactlyAsync(clientReceived, timeout.Token);
        clientReceived.ShouldBe(response);
    }

    [Fact]
    public void Ctor_WithNullServerOptions_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        Should.Throw<ArgumentNullException>(() => new TlsConnectionLayer((TlsServerOptions)null!));
    }

    [Fact]
    public void Ctor_WithNullClientOptions_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        Should.Throw<ArgumentNullException>(() => new TlsConnectionLayer((TlsClientOptions)null!));
    }

    [Fact]
    public void UpgradeAsync_WithNullConnection_ShouldThrowArgumentNullException()
    {
        // Arrange
        TlsConnectionLayer layer = new(new TlsServerOptions());

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => _ = layer.UpgradeAsync(null!));
    }
}
```

## Walkthrough

- **Test entry point** — `Describe_WithUnsecuredCapabilities_ShouldFlipSecurityToTlsAndPreserveRest` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `UpgradeAsync_WithServerOptions_ShouldCompleteServerSideHandshake` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `Ctor_WithNullServerOptions_ShouldThrowArgumentNullException` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `Ctor_WithNullClientOptions_ShouldThrowArgumentNullException` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `UpgradeAsync_WithNullConnection_ShouldThrowArgumentNullException` contains the setup, invocation, and assertions for this case.

The assertions define the expected result or failure boundary. Test-only doubles supply controlled
inputs; they are not additional shipped APIs.

## Tls Client Certificate Tests

### Code

```csharp
using System;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

using Shouldly;

using Xunit;

using Assimalign.Cohesion.Connections;
using Assimalign.Cohesion.Connections.InMemory;

namespace Assimalign.Cohesion.Connections.Security.Tests;

/// <summary>
/// Covers the client-certificate policy of <see cref="TlsServerOptions"/>
/// (<see cref="TlsServerOptions.RequireClientCertificate"/> and
/// <see cref="TlsServerOptions.AllowClientCertificate"/>): the certificate is requested in the
/// handshake, a missing one is refused or tolerated, and a presented one must pass validation.
/// </summary>
public class TlsClientCertificateTests : IClassFixture<TestCertificateFixture>
{
    private static readonly TimeSpan _testTimeout = TimeSpan.FromSeconds(10);

    private readonly TestCertificateFixture _fixture;

    public TlsClientCertificateTests(TestCertificateFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = "Cohesion Test [Connections.Security] - RequireClientCertificate: Should request a certificate in the handshake")]
    public void RequireClientCertificate_OnOptions_ShouldRequestCertificateAndInstallValidator()
    {
        // Arrange
        TlsServerOptions options = new();

        // Act
        TlsServerOptions result = options.RequireClientCertificate();

        // Assert — CertificateRequest is sent during the handshake (RFC 8446 §4.3.2).
        result.ShouldBeSameAs(options);
        options.AuthenticationOptions.ClientCertificateRequired.ShouldBeTrue();
        options.AuthenticationOptions.RemoteCertificateValidationCallback.ShouldNotBeNull();
    }

    [Fact(DisplayName = "Cohesion Test [Connections.Security] - RequireClientCertificate: Should complete the handshake for an accepted certificate")]
    public async Task RequireClientCertificate_WithAcceptedCertificate_ShouldCompleteHandshake()
    {
        // Arrange
        using CancellationTokenSource timeout = new(_testTimeout);
        X509Certificate2? validated = null;
        TlsServerOptions serverOptions = CreateServerOptions().RequireClientCertificate((certificate, _, _) =>
        {
            validated = certificate;
            return true;
        });

        // Act
        (Task<IDuplexConnection> server, Task<IDuplexConnection> _) = Handshake(serverOptions, _fixture.ClientCertificate, timeout.Token);
        IDuplexConnection secured = await server;

        // Assert
        validated.ShouldNotBeNull();
        validated.Thumbprint.ShouldBe(_fixture.ClientCertificate.Thumbprint);
        secured.Tls.ShouldNotBeNull().RemoteCertificate!.Thumbprint.ShouldBe(_fixture.ClientCertificate.Thumbprint);
    }

    [Fact(DisplayName = "Cohesion Test [Connections.Security] - RequireClientCertificate: Should refuse a client that presents no certificate")]
    public async Task RequireClientCertificate_WithoutCertificate_ShouldFailServerHandshake()
    {
        // Arrange
        using CancellationTokenSource timeout = new(_testTimeout);
        TlsServerOptions serverOptions = CreateServerOptions().RequireClientCertificate(static (_, _, _) => true);

        // Act
        (Task<IDuplexConnection> server, Task<IDuplexConnection> client) = Handshake(serverOptions, clientCertificate: null, timeout.Token);

        // Assert
        await Should.ThrowAsync<AuthenticationException>(server);
        await ObserveAsync(client);
    }

    [Fact(DisplayName = "Cohesion Test [Connections.Security] - RequireClientCertificate: Should refuse a certificate the validator rejects")]
    public async Task RequireClientCertificate_WhenValidatorRejects_ShouldFailServerHandshake()
    {
        // Arrange
        using CancellationTokenSource timeout = new(_testTimeout);
        TlsServerOptions serverOptions = CreateServerOptions().RequireClientCertificate(static (_, _, _) => false);

        // Act
        (Task<IDuplexConnection> server, Task<IDuplexConnection> client) = Handshake(serverOptions, _fixture.ClientCertificate, timeout.Token);

        // Assert
        await Should.ThrowAsync<AuthenticationException>(server);
        await ObserveAsync(client);
    }

    [Fact(DisplayName = "Cohesion Test [Connections.Security] - AllowClientCertificate: Should complete the handshake without a certificate")]
    public async Task AllowClientCertificate_WithoutCertificate_ShouldCompleteHandshake()
    {
        // Arrange
        using CancellationTokenSource timeout = new(_testTimeout);
        TlsServerOptions serverOptions = CreateServerOptions().AllowClientCertificate(static (_, _, _) => true);

        // Act
        (Task<IDuplexConnection> server, Task<IDuplexConnection> client) = Handshake(serverOptions, clientCertificate: null, timeout.Token);
        IDuplexConnection secured = await server;
        await client;

        // Assert
        secured.Tls.ShouldNotBeNull().RemoteCertificate.ShouldBeNull();
    }

    [Fact(DisplayName = "Cohesion Test [Connections.Security] - AllowClientCertificate: Should refuse an untrusted certificate under the platform's default validation")]
    public async Task AllowClientCertificate_WithDefaultValidation_ShouldRefuseUntrustedCertificate()
    {
        // Arrange — the self-signed client certificate does not chain to a root the machine trusts.
        using CancellationTokenSource timeout = new(_testTimeout);
        TlsServerOptions serverOptions = CreateServerOptions().AllowClientCertificate();

        // Act
        (Task<IDuplexConnection> server, Task<IDuplexConnection> client) = Handshake(serverOptions, _fixture.ClientCertificate, timeout.Token);

        // Assert
        await Should.ThrowAsync<AuthenticationException>(server);
        await ObserveAsync(client);
    }

    [Fact(DisplayName = "Cohesion Test [Connections.Security] - AllowClientCertificate: Should replace an earlier policy these methods installed")]
    public async Task AllowClientCertificate_AfterRequireClientCertificate_ShouldReplacePolicy()
    {
        // Arrange
        using CancellationTokenSource timeout = new(_testTimeout);
        TlsServerOptions serverOptions = CreateServerOptions()
            .RequireClientCertificate(static (_, _, _) => true)
            .AllowClientCertificate(static (_, _, _) => true);

        // Act — the later "allow" policy admits a client without a certificate.
        (Task<IDuplexConnection> server, Task<IDuplexConnection> client) = Handshake(serverOptions, clientCertificate: null, timeout.Token);
        IDuplexConnection secured = await server;
        await client;

        // Assert
        secured.Tls.ShouldNotBeNull().RemoteCertificate.ShouldBeNull();
    }

    [Fact(DisplayName = "Cohesion Test [Connections.Security] - RequireClientCertificate: Should not overwrite an application's validation callback")]
    public void RequireClientCertificate_WithForeignValidationCallback_ShouldThrow()
    {
        // Arrange
        TlsServerOptions options = new()
        {
            AuthenticationOptions =
            {
                RemoteCertificateValidationCallback = static (_, _, _, _) => true
            }
        };

        // Act / Assert
        Should.Throw<InvalidOperationException>(() => options.RequireClientCertificate());
        Should.Throw<InvalidOperationException>(() => options.AllowClientCertificate());
    }

    private TlsServerOptions CreateServerOptions()
    {
        return new TlsServerOptions
        {
            AuthenticationOptions =
            {
                ServerCertificate = _fixture.Certificate
            }
        };
    }

    private static (Task<IDuplexConnection> Server, Task<IDuplexConnection> Client) Handshake(
        TlsServerOptions serverOptions,
        X509Certificate2? clientCertificate,
        CancellationToken cancellationToken)
    {
        (IDuplexConnection client, IDuplexConnection server) = InMemoryConnectionPair.Create();
        TlsClientOptions clientOptions = new()
        {
            AuthenticationOptions =
            {
                TargetHost = "localhost",
                RemoteCertificateValidationCallback = static (_, _, _, _) => true
            }
        };

        if (clientCertificate is not null)
        {
            TlsConnectionInfoTests.UseClientCertificate(clientOptions, clientCertificate);
        }

        Task<IDuplexConnection> serverUpgrade = server.UpgradeToTlsAsync(serverOptions, cancellationToken).AsTask();
        Task<IDuplexConnection> clientUpgrade = client.UpgradeToTlsAsync(clientOptions, cancellationToken).AsTask();

        return (serverUpgrade, clientUpgrade);
    }

    private static async Task ObserveAsync(Task<IDuplexConnection> task)
    {
        try
        {
            IDuplexConnection connection = await task;
            await connection.DisposeAsync();
        }
        catch (Exception exception) when (exception is AuthenticationException or System.IO.IOException or OperationCanceledException)
        {
            // Under TLS 1.3 the client may complete its side before the server refuses it, or learn of
            // the refusal from the alert; either way only the server's verdict is under test.
        }
    }
}
```

### Walkthrough

- **Test entry point** — `RequireClientCertificate_OnOptions_ShouldRequestCertificateAndInstallValidator` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `RequireClientCertificate_WithAcceptedCertificate_ShouldCompleteHandshake` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `RequireClientCertificate_WithoutCertificate_ShouldFailServerHandshake` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `RequireClientCertificate_WhenValidatorRejects_ShouldFailServerHandshake` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `AllowClientCertificate_WithoutCertificate_ShouldCompleteHandshake` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `AllowClientCertificate_WithDefaultValidation_ShouldRefuseUntrustedCertificate` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `AllowClientCertificate_AfterRequireClientCertificate_ShouldReplacePolicy` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `RequireClientCertificate_WithForeignValidationCallback_ShouldThrow` contains the setup, invocation, and assertions for this case.

## Tls Connection Info Tests

### Code

```csharp
using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

using Shouldly;

using Xunit;

using Assimalign.Cohesion.Connections;
using Assimalign.Cohesion.Connections.InMemory;
using Assimalign.Cohesion.Connections.Security.Tests.TestObjects;

namespace Assimalign.Cohesion.Connections.Security.Tests;

/// <summary>
/// Pins what a secured connection reports about its handshake through
/// <see cref="ITlsConnectionInfo"/>, on both the server and the client side of a TLS session run over
/// an in-memory connection pair.
/// </summary>
public class TlsConnectionInfoTests : IClassFixture<TestCertificateFixture>
{
    private static readonly TimeSpan _testTimeout = TimeSpan.FromSeconds(10);

    private readonly TestCertificateFixture _fixture;

    public TlsConnectionInfoTests(TestCertificateFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = "Cohesion Test [Connections.Security] - ApplicationProtocol: Should report h2 when both sides offer it")]
    public async Task ApplicationProtocol_WhenBothSidesOfferH2_ShouldReportH2()
    {
        // Arrange
        using CancellationTokenSource timeout = new(_testTimeout);

        // Act
        (IDuplexConnection client, IDuplexConnection server) = await UpgradePairAsync(
            [SslApplicationProtocol.Http2, SslApplicationProtocol.Http11],
            [SslApplicationProtocol.Http2, SslApplicationProtocol.Http11],
            timeout.Token);

        // Assert
        server.Tls.ShouldNotBeNull().ApplicationProtocol.ShouldBe(SslApplicationProtocol.Http2);
        client.Tls.ShouldNotBeNull().ApplicationProtocol.ShouldBe(SslApplicationProtocol.Http2);
    }

    [Fact(DisplayName = "Cohesion Test [Connections.Security] - ApplicationProtocol: Should report http/1.1 when the client offers only it")]
    public async Task ApplicationProtocol_WhenClientOffersOnlyHttp11_ShouldReportHttp11()
    {
        // Arrange
        using CancellationTokenSource timeout = new(_testTimeout);

        // Act
        (IDuplexConnection _, IDuplexConnection server) = await UpgradePairAsync(
            [SslApplicationProtocol.Http2, SslApplicationProtocol.Http11],
            [SslApplicationProtocol.Http11],
            timeout.Token);

        // Assert
        server.Tls.ShouldNotBeNull().ApplicationProtocol.ShouldBe(SslApplicationProtocol.Http11);
    }

    [Fact(DisplayName = "Cohesion Test [Connections.Security] - ApplicationProtocol: Should report no protocol when the client sends no ALPN extension")]
    public async Task ApplicationProtocol_WhenClientOffersNone_ShouldReportDefault()
    {
        // Arrange
        using CancellationTokenSource timeout = new(_testTimeout);

        // Act
        (IDuplexConnection _, IDuplexConnection server) = await UpgradePairAsync(
            [SslApplicationProtocol.Http2, SslApplicationProtocol.Http11],
            clientProtocols: null,
            timeout.Token);

        // Assert
        server.Tls.ShouldNotBeNull().ApplicationProtocol.Protocol.IsEmpty.ShouldBeTrue();
    }

    [Fact(DisplayName = "Cohesion Test [Connections.Security] - TlsProtocol and CipherSuite: Should report what both peers negotiated")]
    public async Task TlsProtocolAndCipherSuite_OnCompletedHandshake_ShouldMatchOnBothPeers()
    {
        // Arrange
        using CancellationTokenSource timeout = new(_testTimeout);

        // Act
        (IDuplexConnection client, IDuplexConnection server) = await UpgradePairAsync(null, null, timeout.Token);
        ITlsConnectionInfo serverTls = server.Tls.ShouldNotBeNull();
        ITlsConnectionInfo clientTls = client.Tls.ShouldNotBeNull();

        // Assert — a modern platform negotiates TLS 1.2 or 1.3, and both ends agree on the session.
        serverTls.TlsProtocol.ShouldBeOneOf(SslProtocols.Tls12, SslProtocols.Tls13);
        serverTls.TlsProtocol.ShouldBe(clientTls.TlsProtocol);
        serverTls.CipherSuite.ShouldNotBe(default);
        serverTls.CipherSuite.ShouldBe(clientTls.CipherSuite);
    }

    [Fact(DisplayName = "Cohesion Test [Connections.Security] - RemoteCertificate: Should report the server's certificate on the client side")]
    public async Task RemoteCertificate_OnClientSide_ShouldBeServerCertificate()
    {
        // Arrange
        using CancellationTokenSource timeout = new(_testTimeout);

        // Act
        (IDuplexConnection client, IDuplexConnection _) = await UpgradePairAsync(null, null, timeout.Token);

        // Assert
        client.Tls.ShouldNotBeNull().RemoteCertificate!.Thumbprint.ShouldBe(_fixture.Certificate.Thumbprint);
    }

    [Fact(DisplayName = "Cohesion Test [Connections.Security] - RemoteCertificate: Should be null on the server when no client certificate was requested")]
    public async Task RemoteCertificate_WithoutClientCertificate_ShouldBeNullOnServer()
    {
        // Arrange
        using CancellationTokenSource timeout = new(_testTimeout);

        // Act
        (IDuplexConnection _, IDuplexConnection server) = await UpgradePairAsync(null, null, timeout.Token);

        // Assert
        server.Tls.ShouldNotBeNull().RemoteCertificate.ShouldBeNull();
    }

    [Fact(DisplayName = "Cohesion Test [Connections.Security] - RemoteCertificate: Should report the client's certificate on the server and release it with the connection")]
    public async Task RemoteCertificate_WithClientCertificate_ShouldBeOwnedByServerConnection()
    {
        // Arrange
        using CancellationTokenSource timeout = new(_testTimeout);

        // Act
        (IDuplexConnection _, IDuplexConnection server) = await UpgradePairAsync(
            null,
            null,
            timeout.Token,
            serverOptions => serverOptions.AllowClientCertificate(static (_, _, _) => true),
            clientOptions => UseClientCertificate(clientOptions, _fixture.ClientCertificate));
        X509Certificate2 certificate = server.Tls.ShouldNotBeNull().RemoteCertificate.ShouldNotBeNull();
        string thumbprint = certificate.Thumbprint;
        await server.DisposeAsync();

        // Assert — the connection owned the certificate and released it with itself.
        thumbprint.ShouldBe(_fixture.ClientCertificate.Thumbprint);
        certificate.Handle.ShouldBe(IntPtr.Zero);
    }

    [Fact(DisplayName = "Cohesion Test [Connections.Security] - Features: TLS exposes its handshake and the wrapped connection's features")]
    public async Task Features_WithFeatureBelowTlsLayer_ShouldExposeTlsAndInnerFeature()
    {
        // Arrange
        using CancellationTokenSource timeout = new(_testTimeout);
        TestConnectionFeature innerFeature = new();

        // Act
        (IDuplexConnection client, IDuplexConnection server) = await UpgradePairAsync(
            null,
            null,
            timeout.Token,
            configureConnection: connection => connection.Features.Set(innerFeature));
        await using IDuplexConnection clientLifetime = client;
        await using IDuplexConnection serverLifetime = server;

        // Assert
        server.Tls.ShouldNotBeNull().Name.ShouldBe(ITlsConnectionInfo.FeatureName);
        server.Features.Get(ITlsConnectionInfo.FeatureName).ShouldBeSameAs(server.Tls);
        server.Features.Get<TestConnectionFeature>().ShouldBeSameAs(innerFeature);
        server.Features.Get(innerFeature.Name).ShouldBeSameAs(innerFeature);
    }

    internal static void UseClientCertificate(TlsClientOptions options, X509Certificate2 certificate)
    {
        options.AuthenticationOptions.ClientCertificates = new X509CertificateCollection { certificate };
        options.AuthenticationOptions.LocalCertificateSelectionCallback = (_, _, _, _, _) => certificate;
    }

    private async Task<(IDuplexConnection Client, IDuplexConnection Server)> UpgradePairAsync(
        List<SslApplicationProtocol>? serverProtocols,
        List<SslApplicationProtocol>? clientProtocols,
        CancellationToken cancellationToken,
        Action<TlsServerOptions>? configureServer = null,
        Action<TlsClientOptions>? configureClient = null,
        Action<IDuplexConnection>? configureConnection = null)
    {
        (IDuplexConnection client, IDuplexConnection server) = InMemoryConnectionPair.Create();
        TlsServerOptions serverOptions = new()
        {
            AuthenticationOptions =
            {
                ServerCertificate = _fixture.Certificate,
                ApplicationProtocols = serverProtocols
            }
        };
        TlsClientOptions clientOptions = new()
        {
            AuthenticationOptions =
            {
                TargetHost = "localhost",
                ApplicationProtocols = clientProtocols,
                RemoteCertificateValidationCallback = static (_, _, _, _) => true
            }
        };
        configureConnection?.Invoke(server);
        configureServer?.Invoke(serverOptions);
        configureClient?.Invoke(clientOptions);

        Task<IDuplexConnection> serverUpgrade = server.UpgradeToTlsAsync(serverOptions, cancellationToken).AsTask();
        Task<IDuplexConnection> clientUpgrade = client.UpgradeToTlsAsync(clientOptions, cancellationToken).AsTask();

        await Task.WhenAll(serverUpgrade, clientUpgrade);

        return (await clientUpgrade, await serverUpgrade);
    }
}
```

### Walkthrough

- **Test entry point** — `ApplicationProtocol_WhenBothSidesOfferH2_ShouldReportH2` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `ApplicationProtocol_WhenClientOffersOnlyHttp11_ShouldReportHttp11` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `ApplicationProtocol_WhenClientOffersNone_ShouldReportDefault` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `TlsProtocolAndCipherSuite_OnCompletedHandshake_ShouldMatchOnBothPeers` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `RemoteCertificate_OnClientSide_ShouldBeServerCertificate` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `RemoteCertificate_WithoutClientCertificate_ShouldBeNullOnServer` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `RemoteCertificate_WithClientCertificate_ShouldBeOwnedByServerConnection` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `Features_WithFeatureBelowTlsLayer_ShouldExposeTlsAndInnerFeature` contains the setup, invocation, and assertions for this case.

## Tls Connection Tests

### Code

```csharp
using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Shouldly;

using Xunit;

using Assimalign.Cohesion.Connections;
using Assimalign.Cohesion.Connections.InMemory;

namespace Assimalign.Cohesion.Connections.Security.Tests;

public class TlsConnectionTests : IClassFixture<TestCertificateFixture>
{
    private static readonly TimeSpan _testTimeout = TimeSpan.FromSeconds(5);

    private readonly TestCertificateFixture _fixture;

    public TlsConnectionTests(TestCertificateFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task UpgradeToTlsAsync_WithClientAndServerPair_ShouldExchangeBytesInBothDirections()
    {
        // Arrange
        using CancellationTokenSource timeout = new(_testTimeout);
        (IDuplexConnection client, IDuplexConnection server) = InMemoryConnectionPair.Create();

        // Act
        (IDuplexConnection securedClient, IDuplexConnection securedServer) = await UpgradePairAsync(client, server, timeout.Token);

        // Assert: bytes written by the client arrive decrypted at the server.
        byte[] clientPayload = Encoding.UTF8.GetBytes("hello from client");
        await securedClient.Output.WriteAsync(clientPayload, timeout.Token);
        byte[] serverReceived = await securedServer.Input.ReadExactlyAsync(clientPayload.Length, timeout.Token);
        serverReceived.ShouldBe(clientPayload);

        // And bytes written by the server arrive decrypted at the client.
        byte[] serverPayload = Encoding.UTF8.GetBytes("hello from server");
        await securedServer.Output.WriteAsync(serverPayload, timeout.Token);
        byte[] clientReceived = await securedClient.Input.ReadExactlyAsync(serverPayload.Length, timeout.Token);
        clientReceived.ShouldBe(serverPayload);
    }

    [Fact]
    public async Task UpgradeToTlsAsync_OnSecuredConnection_ShouldReportTlsSecurityAndPreserveOtherCapabilities()
    {
        // Arrange
        using CancellationTokenSource timeout = new(_testTimeout);
        (IDuplexConnection client, IDuplexConnection server) = InMemoryConnectionPair.Create();

        // Act
        (IDuplexConnection securedClient, IDuplexConnection securedServer) = await UpgradePairAsync(client, server, timeout.Token);

        // Assert
        securedClient.Capabilities.ShouldBe(client.Capabilities with { Security = ConnectionSecurity.Tls });
        securedServer.Capabilities.ShouldBe(server.Capabilities with { Security = ConnectionSecurity.Tls });
    }

    [Fact]
    public async Task UpgradeToTlsAsync_OnSecuredConnection_ShouldDelegateIdentityToInnerConnection()
    {
        // Arrange
        using CancellationTokenSource timeout = new(_testTimeout);
        (IDuplexConnection client, IDuplexConnection server) = InMemoryConnectionPair.Create();

        // Act
        (IDuplexConnection securedClient, IDuplexConnection _) = await UpgradePairAsync(client, server, timeout.Token);

        // Assert
        securedClient.Id.ShouldBe(client.Id);
        securedClient.LocalEndPoint.ShouldBeSameAs(client.LocalEndPoint);
        securedClient.RemoteEndPoint.ShouldBeSameAs(client.RemoteEndPoint);
        securedClient.Direction.ShouldBe(client.Direction);
    }

    [Fact]
    public async Task DisposeAsync_OnSecuredConnection_ShouldDisposeInnerConnection()
    {
        // Arrange
        using CancellationTokenSource timeout = new(_testTimeout);
        (IDuplexConnection client, IDuplexConnection server) = InMemoryConnectionPair.Create();
        (IDuplexConnection securedClient, IDuplexConnection _) = await UpgradePairAsync(client, server, timeout.Token);

        // Act
        await securedClient.DisposeAsync();
        securedClient.ConnectionClosed.IsCancellationRequested.ShouldBeTrue();

        // Assert
        client.State.ShouldBe(ConnectionState.Closed);
    }

    [Theory(DisplayName = "Cohesion Test [Connections.Security] - Features: TLS hides directional abort when plaintext survives an inner abort")]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Features_WithBufferedPlaintext_ShouldHideInnerDirectionalAbort(bool readAtServer)
    {
        // Arrange
        using CancellationTokenSource timeout = new(_testTimeout);
        (MultiplexedConnection client, MultiplexedConnection server) = InMemoryMultiplexedConnectionPair.Create();
        await using MultiplexedConnection clientLifetime = client;
        await using MultiplexedConnection serverLifetime = server;
        await using MultiplexedConnectionStream clientStream = await client.OpenStreamAsync(cancellationToken: timeout.Token);
        await using MultiplexedConnectionStream serverStream = await server.AcceptStreamAsync(timeout.Token);
        (IDuplexConnection securedClient, IDuplexConnection securedServer) = await UpgradePairAsync(clientStream, serverStream, timeout.Token);
        await using IDuplexConnection securedClientLifetime = securedClient;
        await using IDuplexConnection securedServerLifetime = securedServer;
        IDuplexConnection sender = readAtServer ? securedClient : securedServer;
        IDuplexConnection receiver = readAtServer ? securedServer : securedClient;
        IMultiplexedConnectionStream innerReceiver = readAtServer ? serverStream : clientStream;
        IMultiplexedStreamAbort innerAbort = innerReceiver.DirectionalAbort.ShouldNotBeNull();
        byte[] payload = [1, 2, 3];
        await sender.Output.WriteAsync(payload, timeout.Token);
        ReadResult read = await receiver.Input.ReadAtLeastAsync(payload.Length, timeout.Token);
        read.Buffer.ToArray().ShouldBe(payload);
        receiver.Input.AdvanceTo(read.Buffer.Start, read.Buffer.Start);

        // Act: aborting the transport cannot invalidate plaintext retained by TLS's reader.
        innerAbort.AbortRead(42);
        bool hasBufferedPlaintext = receiver.Input.TryRead(out ReadResult buffered);

        // Assert: this inner feature cannot honor its buffered-read guarantee at the TLS layer.
        hasBufferedPlaintext.ShouldBeTrue();
        buffered.Buffer.ToArray().ShouldBe(payload);
        receiver.Input.AdvanceTo(buffered.Buffer.End);
        receiver.Features.Get<IMultiplexedStreamAbort>().ShouldBeNull();
        receiver.Features.Get(IMultiplexedStreamAbort.FeatureName).ShouldBeNull();
        receiver.Features.ShouldNotContain(feature => feature.Name == IMultiplexedStreamAbort.FeatureName);
        innerReceiver.DirectionalAbort.ShouldBeSameAs(innerAbort);
        receiver.Tls.ShouldNotBeNull();
    }

    [Theory(DisplayName = "Cohesion Test [Connections.Security] - ConnectionClosed: TLS preserves local and idle peer closure")]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ConnectionClosed_OnDisposeOrAbort_ShouldNotifyBothEnds(bool abort)
    {
        // Arrange
        using CancellationTokenSource timeout = new(_testTimeout);
        (IDuplexConnection client, IDuplexConnection server) = InMemoryConnectionPair.Create();
        (IDuplexConnection securedClient, IDuplexConnection securedServer) = await UpgradePairAsync(client, server, timeout.Token);
        await using IDuplexConnection clientLifetime = securedClient;
        await using IDuplexConnection serverLifetime = securedServer;

        // Act
        if (abort)
        {
            securedClient.Abort();
            securedClient.ConnectionClosed.IsCancellationRequested.ShouldBeTrue();
        }
        await securedClient.DisposeAsync();

        // Assert
        securedClient.ConnectionClosed.IsCancellationRequested.ShouldBeTrue();
        securedServer.ConnectionClosed.IsCancellationRequested.ShouldBeTrue();
        await securedServer.DisposeAsync();
        securedServer.ConnectionClosed.IsCancellationRequested.ShouldBeTrue();
    }

    private async Task<(IDuplexConnection Client, IDuplexConnection Server)> UpgradePairAsync(
        IDuplexConnection client,
        IDuplexConnection server,
        CancellationToken cancellationToken)
    {
        TlsServerOptions serverOptions = new()
        {
            AuthenticationOptions =
            {
                ServerCertificate = _fixture.Certificate
            }
        };
        TlsClientOptions clientOptions = new()
        {
            AuthenticationOptions =
            {
                TargetHost = "localhost",
                RemoteCertificateValidationCallback = static (_, _, _, _) => true
            }
        };

        Task<IDuplexConnection> serverUpgrade = server.UpgradeToTlsAsync(serverOptions, cancellationToken).AsTask();
        Task<IDuplexConnection> clientUpgrade = client.UpgradeToTlsAsync(clientOptions, cancellationToken).AsTask();

        await Task.WhenAll(serverUpgrade, clientUpgrade);

        return (await clientUpgrade, await serverUpgrade);
    }
}
```

### Walkthrough

- **Test entry point** — `UpgradeToTlsAsync_WithClientAndServerPair_ShouldExchangeBytesInBothDirections` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `UpgradeToTlsAsync_OnSecuredConnection_ShouldReportTlsSecurityAndPreserveOtherCapabilities` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `UpgradeToTlsAsync_OnSecuredConnection_ShouldDelegateIdentityToInnerConnection` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `DisposeAsync_OnSecuredConnection_ShouldDisposeInnerConnection` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `Features_WithBufferedPlaintext_ShouldHideInnerDirectionalAbort` contains the setup, invocation, and assertions for this case. Theory cases: `true`, `false`.
- **Test entry point** — `ConnectionClosed_OnDisposeOrAbort_ShouldNotifyBothEnds` contains the setup, invocation, and assertions for this case. Theory cases: `false`, `true`.

## Tls Handshake Isolation Tests

### Code

```csharp
using System;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

using Shouldly;

using Xunit;

using Assimalign.Cohesion.Connections;
using Assimalign.Cohesion.Connections.InMemory;

namespace Assimalign.Cohesion.Connections.Security.Tests;

/// <summary>
/// Covers how a TLS-layered listener (<c>listener.UseTls(options)</c>) contains a handshake that fails or
/// stalls (#1304): the handshake runs per connection, a failure or timeout closes only that connection while
/// another client is secured on the same listener, and <see cref="TlsServerOptions.MaxConcurrentHandshakes"/>
/// bounds the connections held at once.
/// </summary>
public class TlsHandshakeIsolationTests : IClassFixture<TestCertificateFixture>
{
    // A hang guard, never a budget: every wait completes as soon as the listener does its part.
    private static readonly TimeSpan _testTimeout = TimeSpan.FromSeconds(10);

    private readonly TestCertificateFixture _fixture;

    public TlsHandshakeIsolationTests(TestCertificateFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = "Cohesion Test [Connections.Security] - UseTls: Bytes that are not a TLS handshake should close only that connection")]
    public async Task UseTls_AfterNonTlsBytes_ShouldCloseThatConnectionAndSecureTheNext()
    {
        // Arrange
        using CancellationTokenSource timeout = new(_testTimeout);
        (IDuplexConnection garbageClient, IDuplexConnection garbageServer) = InMemoryConnectionPair.Create();
        (IDuplexConnection client, IDuplexConnection server) = InMemoryConnectionPair.Create();
        BlockingConnectionListener transport = new();
        transport.Enqueue(garbageServer);
        transport.Enqueue(server);
        await using IDuplexConnectionListener listener = transport.UseTls(CreateServerOptions());
        await garbageClient.Output.WriteAsync("HELLO-1304"u8.ToArray(), timeout.Token);

        // Act
        Task<IDuplexConnection> accept = listener.AcceptAsync(timeout.Token).AsTask();
        IDuplexConnection secured = await AcceptWithClientHandshakeAsync(accept, client, clientCertificate: null, timeout.Token);

        // Assert
        secured.Id.ShouldBe(server.Id);
        await WhenClosedAsync(garbageServer, timeout.Token);
    }

    [Fact(DisplayName = "Cohesion Test [Connections.Security] - UseTls: A client the certificate policy refuses should close only that connection")]
    public async Task UseTls_WhenTheRequiredClientCertificateIsMissing_ShouldCloseThatConnectionAndSecureTheNext()
    {
        // Arrange
        using CancellationTokenSource timeout = new(_testTimeout);
        string expected = _fixture.ClientCertificate.Thumbprint;
        TlsServerOptions options = CreateServerOptions().RequireClientCertificate((presented, _, _) => presented.Thumbprint == expected);
        (IDuplexConnection refusedClient, IDuplexConnection refusedServer) = InMemoryConnectionPair.Create();
        (IDuplexConnection client, IDuplexConnection server) = InMemoryConnectionPair.Create();
        BlockingConnectionListener transport = new();
        transport.Enqueue(refusedServer);
        transport.Enqueue(server);
        await using IDuplexConnectionListener listener = transport.UseTls(options);
        Task refusedHandshake = ObserveAsync(refusedClient.UpgradeToTlsAsync(CreateClientOptions(clientCertificate: null), timeout.Token).AsTask());

        // Act
        Task<IDuplexConnection> accept = listener.AcceptAsync(timeout.Token).AsTask();
        IDuplexConnection secured = await AcceptWithClientHandshakeAsync(accept, client, _fixture.ClientCertificate, timeout.Token);

        // Assert
        secured.Id.ShouldBe(server.Id);
        secured.Tls.ShouldNotBeNull().RemoteCertificate!.Thumbprint.ShouldBe(expected);
        await WhenClosedAsync(refusedServer, timeout.Token);
        await refusedHandshake;
    }

    [Fact(DisplayName = "Cohesion Test [Connections.Security] - UseTls: A silent client should not delay another client's handshake")]
    public async Task UseTls_WhileAClientStaysSilent_ShouldSecureAnotherClient()
    {
        // Arrange — left alone, the silent client would hold its handshake for minutes.
        using CancellationTokenSource timeout = new(_testTimeout);
        TlsServerOptions options = CreateServerOptions();
        options.HandshakeTimeout = TimeSpan.FromMinutes(5);
        (DuplexConnection _, IDuplexConnection silentServer) = InMemoryConnectionPair.Create();
        (IDuplexConnection client, IDuplexConnection server) = InMemoryConnectionPair.Create();
        BlockingConnectionListener transport = new();
        transport.Enqueue(silentServer);
        transport.Enqueue(server);
        IDuplexConnectionListener listener = transport.UseTls(options);

        // Act
        Task<IDuplexConnection> accept = listener.AcceptAsync(timeout.Token).AsTask();
        IDuplexConnection secured = await AcceptWithClientHandshakeAsync(accept, client, clientCertificate: null, timeout.Token);
        bool silentOpenWhileServing = !silentServer.ConnectionClosed.IsCancellationRequested;
        await listener.DisposeAsync();

        // Assert — disposal cancels the handshake still waiting and closes its connection.
        secured.Id.ShouldBe(server.Id);
        silentOpenWhileServing.ShouldBeTrue();
        silentServer.ConnectionClosed.IsCancellationRequested.ShouldBeTrue();
    }

    [Fact(DisplayName = "Cohesion Test [Connections.Security] - UseTls: A handshake that times out should close its connection and keep the listener accepting")]
    public async Task UseTls_WhenASilentClientExceedsTheHandshakeTimeout_ShouldCloseItAndKeepAccepting()
    {
        // Arrange
        using CancellationTokenSource timeout = new(_testTimeout);
        TlsServerOptions options = CreateServerOptions();
        options.HandshakeTimeout = TimeSpan.FromSeconds(2);
        (DuplexConnection _, IDuplexConnection silentServer) = InMemoryConnectionPair.Create();
        (IDuplexConnection client, IDuplexConnection server) = InMemoryConnectionPair.Create();
        BlockingConnectionListener transport = new();
        transport.Enqueue(silentServer);
        await using IDuplexConnectionListener listener = transport.UseTls(options);
        Task<IDuplexConnection> accept = listener.AcceptAsync(timeout.Token).AsTask();

        // Act — the silent client's handshake times out first; a client that arrives afterwards is served.
        await WhenClosedAsync(silentServer, timeout.Token);
        transport.Enqueue(server);
        IDuplexConnection secured = await AcceptWithClientHandshakeAsync(accept, client, clientCertificate: null, timeout.Token);

        // Assert
        secured.Id.ShouldBe(server.Id);
    }

    [Fact(DisplayName = "Cohesion Test [Connections.Security] - UseTls: Should stop accepting from the transport at MaxConcurrentHandshakes")]
    public async Task UseTls_AtMaxConcurrentHandshakes_ShouldStopAcceptingFromTheTransport()
    {
        // Arrange — one slot, taken by a client that never speaks.
        using CancellationTokenSource timeout = new(_testTimeout);
        TlsServerOptions options = CreateServerOptions();
        options.HandshakeTimeout = TimeSpan.FromMinutes(5);
        options.MaxConcurrentHandshakes = 1;
        (IDuplexConnection silentClient, IDuplexConnection silentServer) = InMemoryConnectionPair.Create();
        (IDuplexConnection client, IDuplexConnection server) = InMemoryConnectionPair.Create();
        BlockingConnectionListener transport = new();
        transport.Enqueue(silentServer);
        transport.Enqueue(server);
        await using IDuplexConnectionListener listener = transport.UseTls(options);
        Task<IDuplexConnection> accept = listener.AcceptAsync(timeout.Token).AsTask();
        await WaitUntilAsync(() => transport.AcceptCalls >= 1, timeout.Token);

        // Act — the pause gives a listener that ignored the limit time to take the second connection; it
        // cannot make a correct listener fail. The silent client then hangs up, which ends its handshake
        // and frees the slot.
        await Task.Delay(TimeSpan.FromMilliseconds(200), timeout.Token);
        int acceptsWhileFull = transport.AcceptCalls;
        await silentClient.DisposeAsync();
        IDuplexConnection secured = await AcceptWithClientHandshakeAsync(accept, client, clientCertificate: null, timeout.Token);

        // Assert
        acceptsWhileFull.ShouldBe(1);
        secured.Id.ShouldBe(server.Id);
        silentServer.ConnectionClosed.IsCancellationRequested.ShouldBeTrue();
    }

    private TlsServerOptions CreateServerOptions()
    {
        return new TlsServerOptions
        {
            AuthenticationOptions =
            {
                ServerCertificate = _fixture.Certificate
            }
        };
    }

    private static TlsClientOptions CreateClientOptions(X509Certificate2? clientCertificate)
    {
        TlsClientOptions options = new()
        {
            AuthenticationOptions =
            {
                TargetHost = "localhost",
                RemoteCertificateValidationCallback = static (_, _, _, _) => true
            }
        };

        if (clientCertificate is not null)
        {
            TlsConnectionInfoTests.UseClientCertificate(options, clientCertificate);
        }

        return options;
    }

    /// <summary>
    /// Runs a well-behaved client's handshake on <paramref name="client"/> while the listener accepts, and
    /// returns the secured connection the listener produced.
    /// </summary>
    private static async Task<IDuplexConnection> AcceptWithClientHandshakeAsync(
        Task<IDuplexConnection> accept,
        IDuplexConnection client,
        X509Certificate2? clientCertificate,
        CancellationToken cancellationToken)
    {
        Task<IDuplexConnection> clientHandshake = client.UpgradeToTlsAsync(CreateClientOptions(clientCertificate), cancellationToken).AsTask();
        IDuplexConnection accepted = await accept;
        await clientHandshake;

        return accepted;
    }

    private static async Task WhenClosedAsync(IDuplexConnection connection, CancellationToken cancellationToken)
    {
        TaskCompletionSource closed = new(TaskCreationOptions.RunContinuationsAsynchronously);
        using CancellationTokenRegistration registration = connection.ConnectionClosed.Register(
            static state => ((TaskCompletionSource)state!).TrySetResult(),
            closed);

        await closed.Task.WaitAsync(cancellationToken);
    }

    private static async Task WaitUntilAsync(Func<bool> condition, CancellationToken cancellationToken)
    {
        while (!condition())
        {
            await Task.Delay(TimeSpan.FromMilliseconds(10), cancellationToken);
        }
    }

    private static async Task ObserveAsync(Task<IDuplexConnection> handshake)
    {
        try
        {
            IDuplexConnection connection = await handshake;
            await connection.DisposeAsync();
        }
        catch (Exception exception) when (exception is AuthenticationException or System.IO.IOException or OperationCanceledException)
        {
            // Under TLS 1.3 the client may finish its side before the server refuses it, or learn of the
            // refusal from the alert; only the server's handling is under test.
        }
    }
}
```

### Walkthrough

- **Test entry point** — `UseTls_AfterNonTlsBytes_ShouldCloseThatConnectionAndSecureTheNext` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `UseTls_WhenTheRequiredClientCertificateIsMissing_ShouldCloseThatConnectionAndSecureTheNext` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `UseTls_WhileAClientStaysSilent_ShouldSecureAnotherClient` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `UseTls_WhenASilentClientExceedsTheHandshakeTimeout_ShouldCloseItAndKeepAccepting` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `UseTls_AtMaxConcurrentHandshakes_ShouldStopAcceptingFromTheTransport` contains the setup, invocation, and assertions for this case.

## Tls Connection Extensions Tests

### Code

```csharp
using System;
using System.Net;
using System.Net.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Shouldly;

using Xunit;

using Assimalign.Cohesion.Connections;
using Assimalign.Cohesion.Connections.InMemory;

namespace Assimalign.Cohesion.Connections.Security.Tests;

public class TlsConnectionExtensionsTests : IClassFixture<TestCertificateFixture>
{
    private static readonly TimeSpan _testTimeout = TimeSpan.FromSeconds(5);

    private readonly TestCertificateFixture _fixture;

    public TlsConnectionExtensionsTests(TestCertificateFixture fixture)
    {
        _fixture = fixture;
    }

    private TlsServerOptions CreateServerOptions() => new()
    {
        AuthenticationOptions =
        {
            ServerCertificate = _fixture.Certificate
        }
    };

    private static TlsClientOptions CreateClientOptions() => new()
    {
        AuthenticationOptions =
        {
            TargetHost = "localhost",
            RemoteCertificateValidationCallback = static (_, _, _, _) => true
        }
    };

    [Fact]
    public async Task UseTls_OnListener_ShouldSecureAcceptedConnections()
    {
        // Arrange
        using CancellationTokenSource timeout = new(_testTimeout);
        (IDuplexConnection client, IDuplexConnection server) = InMemoryConnectionPair.Create();
        TestConnectionListener listener = new();
        listener.Enqueue(server);
        await using SslStream clientSsl = new(
            client.AsStream(),
            leaveInnerStreamOpen: false,
            userCertificateValidationCallback: static (_, _, _, _) => true);

        // Act: accept through the TLS-layered listener while a raw client handshakes on the far end.
        IDuplexConnectionListener secured = listener.UseTls(CreateServerOptions());
        secured.Id.ShouldBe(listener.Id);
        Task<IDuplexConnection> acceptTask = secured.AcceptAsync(timeout.Token).AsTask();
        Task clientHandshake = clientSsl.AuthenticateAsClientAsync(
            new SslClientAuthenticationOptions { TargetHost = "localhost" },
            timeout.Token);
        await Task.WhenAll(acceptTask, clientHandshake);
        IDuplexConnection accepted = await acceptTask;

        // Assert
        accepted.Capabilities.Security.ShouldBe(ConnectionSecurity.Tls);

        byte[] payload = Encoding.UTF8.GetBytes("secured accept");
        await clientSsl.WriteAsync(payload, timeout.Token);
        await clientSsl.FlushAsync(timeout.Token);
        byte[] received = await accepted.Input.ReadExactlyAsync(payload.Length, timeout.Token);
        received.ShouldBe(payload);
    }

    [Fact]
    public void UseTls_OnListener_ShouldReportTlsCapabilities()
    {
        // Arrange
        TestConnectionListener listener = new();

        // Act
        IDuplexConnectionListener secured = listener.UseTls(CreateServerOptions());

        // Assert
        secured.Capabilities.ShouldBe(listener.Capabilities with { Security = ConnectionSecurity.Tls });
    }

    [Fact]
    public async Task UseTls_OnFactory_ShouldSecureEstablishedConnections()
    {
        // Arrange
        using CancellationTokenSource timeout = new(_testTimeout);
        (IDuplexConnection client, IDuplexConnection server) = InMemoryConnectionPair.Create();
        TestConnectionFactory factory = new();
        factory.Enqueue(client);
        await using SslStream serverSsl = new(server.AsStream(), leaveInnerStreamOpen: false);

        // Act: connect through the TLS-layered factory while a raw server handshakes on the far end.
        IDuplexConnectionFactory secured = factory.UseTls(CreateClientOptions());
        Task<IDuplexConnection> connectTask = secured.ConnectAsync(new IPEndPoint(IPAddress.Loopback, 22000), timeout.Token).AsTask();
        Task serverHandshake = serverSsl.AuthenticateAsServerAsync(
            new SslServerAuthenticationOptions { ServerCertificate = _fixture.Certificate },
            timeout.Token);
        await Task.WhenAll(connectTask, serverHandshake);
        IDuplexConnection connected = await connectTask;

        // Assert
        connected.Capabilities.Security.ShouldBe(ConnectionSecurity.Tls);

        byte[] payload = Encoding.UTF8.GetBytes("secured connect");
        await connected.Output.WriteAsync(payload, timeout.Token);
        byte[] received = new byte[payload.Length];
        await serverSsl.ReadExactlyAsync(received, timeout.Token);
        received.ShouldBe(payload);
    }

    [Fact]
    public void UseTls_WithNullArguments_ShouldThrowArgumentNullException()
    {
        // Arrange
        TestConnectionListener listener = new();
        TestConnectionFactory factory = new();
        (IDuplexConnection client, DuplexConnection _) = InMemoryConnectionPair.Create();

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => listener.UseTls(null!));
        Should.Throw<ArgumentNullException>(() => ((IDuplexConnectionListener)null!).UseTls(new TlsServerOptions()));
        Should.Throw<ArgumentNullException>(() => factory.UseTls(null!));
        Should.Throw<ArgumentNullException>(() => ((IDuplexConnectionFactory)null!).UseTls(new TlsClientOptions()));
        Should.Throw<ArgumentNullException>(() => _ = client.UpgradeToTlsAsync((TlsServerOptions)null!));
        Should.Throw<ArgumentNullException>(() => _ = client.UpgradeToTlsAsync((TlsClientOptions)null!));
        Should.Throw<ArgumentNullException>(() => _ = ((IDuplexConnection)null!).UpgradeToTlsAsync(new TlsServerOptions()));
    }
}
```

### Walkthrough

- **Test entry point** — `UseTls_OnListener_ShouldSecureAcceptedConnections` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `UseTls_OnListener_ShouldReportTlsCapabilities` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `UseTls_OnFactory_ShouldSecureEstablishedConnections` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `UseTls_WithNullArguments_ShouldThrowArgumentNullException` contains the setup, invocation, and assertions for this case.

## Tls Options Tests

### Code

```csharp
using System;
using System.Threading;
using System.Threading.Tasks;

using Shouldly;

using Xunit;

using Assimalign.Cohesion.Connections;
using Assimalign.Cohesion.Connections.InMemory;

namespace Assimalign.Cohesion.Connections.Security.Tests;

public class TlsOptionsTests : IClassFixture<TestCertificateFixture>
{
    private static readonly TimeSpan _testTimeout = TimeSpan.FromSeconds(5);

    private readonly TestCertificateFixture _fixture;

    public TlsOptionsTests(TestCertificateFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void Ctor_OnTlsServerOptions_ShouldDefaultToTenSecondTimeoutAndAuthenticationOptions()
    {
        // Arrange & Act
        TlsServerOptions options = new();

        // Assert
        options.HandshakeTimeout.ShouldBe(TimeSpan.FromSeconds(10));
        options.AuthenticationOptions.ShouldNotBeNull();
    }

    [Fact(DisplayName = "Cohesion Test [Connections.Security] - TlsServerOptions: MaxConcurrentHandshakes should default to 512")]
    public void MaxConcurrentHandshakes_ByDefault_ShouldBe512()
    {
        // Arrange & Act
        TlsServerOptions options = new();

        // Assert
        options.MaxConcurrentHandshakes.ShouldBe(512);
    }

    [Theory(DisplayName = "Cohesion Test [Connections.Security] - TlsServerOptions: MaxConcurrentHandshakes should reject a value below one")]
    [InlineData(0)]
    [InlineData(-1)]
    public void MaxConcurrentHandshakes_WithNonPositiveValue_ShouldThrowArgumentOutOfRangeException(int value)
    {
        // Arrange
        TlsServerOptions options = new();

        // Act / Assert
        Should.Throw<ArgumentOutOfRangeException>(() => options.MaxConcurrentHandshakes = value);
        options.MaxConcurrentHandshakes.ShouldBe(512);
    }

    [Fact]
    public void Ctor_OnTlsClientOptions_ShouldDefaultToTenSecondTimeoutAndAuthenticationOptions()
    {
        // Arrange & Act
        TlsClientOptions options = new();

        // Assert
        options.HandshakeTimeout.ShouldBe(TimeSpan.FromSeconds(10));
        options.AuthenticationOptions.ShouldNotBeNull();
    }

    [Fact]
    public async Task UpgradeToTlsAsync_AsClientAgainstSilentPeer_ShouldCancelAfterHandshakeTimeout()
    {
        // Arrange: the server end never participates, so the client handshake can only time out.
        (IDuplexConnection client, DuplexConnection _) = InMemoryConnectionPair.Create();
        TlsClientOptions options = new()
        {
            HandshakeTimeout = TimeSpan.FromMilliseconds(100),
            AuthenticationOptions =
            {
                TargetHost = "localhost",
                RemoteCertificateValidationCallback = static (_, _, _, _) => true
            }
        };

        // Act
        Exception? exception = await RecordWithinTestTimeoutAsync(() => client.UpgradeToTlsAsync(options).AsTask());

        // Assert
        exception.ShouldNotBeNull();
        exception.ShouldBeAssignableTo<OperationCanceledException>();
    }

    [Fact]
    public async Task UpgradeToTlsAsync_AsServerAgainstSilentPeer_ShouldCancelAfterHandshakeTimeout()
    {
        // Arrange: no client hello ever arrives, so the server handshake can only time out.
        (DuplexConnection _, IDuplexConnection server) = InMemoryConnectionPair.Create();
        TlsServerOptions options = new()
        {
            HandshakeTimeout = TimeSpan.FromMilliseconds(100),
            AuthenticationOptions =
            {
                ServerCertificate = _fixture.Certificate
            }
        };

        // Act
        Exception? exception = await RecordWithinTestTimeoutAsync(() => server.UpgradeToTlsAsync(options).AsTask());

        // Assert
        exception.ShouldNotBeNull();
        exception.ShouldBeAssignableTo<OperationCanceledException>();
    }

    [Fact]
    public async Task UpgradeToTlsAsync_WithAlreadyCanceledToken_ShouldThrowOperationCanceledException()
    {
        // Arrange
        (IDuplexConnection client, DuplexConnection _) = InMemoryConnectionPair.Create();
        TlsClientOptions options = new()
        {
            AuthenticationOptions =
            {
                TargetHost = "localhost",
                RemoteCertificateValidationCallback = static (_, _, _, _) => true
            }
        };
        using CancellationTokenSource canceled = new();
        canceled.Cancel();

        // Act
        Exception? exception = await RecordWithinTestTimeoutAsync(() => client.UpgradeToTlsAsync(options, canceled.Token).AsTask());

        // Assert
        exception.ShouldNotBeNull();
        exception.ShouldBeAssignableTo<OperationCanceledException>();
    }

    /// <summary>
    /// Records the exception thrown by <paramref name="action"/>, failing fast if it neither
    /// completes nor faults within the test timeout (so a broken timeout cannot hang the run).
    /// </summary>
    private static async Task<Exception?> RecordWithinTestTimeoutAsync(Func<Task> action)
    {
        Task<Exception?> record = Record.ExceptionAsync(action);
        Task completed = await Task.WhenAny(record, Task.Delay(_testTimeout));

        completed.ShouldBeSameAs(record, "The TLS handshake neither completed nor faulted within the test timeout.");

        return await record;
    }
}
```

### Walkthrough

- **Test entry point** — `Ctor_OnTlsServerOptions_ShouldDefaultToTenSecondTimeoutAndAuthenticationOptions` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `MaxConcurrentHandshakes_ByDefault_ShouldBe512` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `MaxConcurrentHandshakes_WithNonPositiveValue_ShouldThrowArgumentOutOfRangeException` contains the setup, invocation, and assertions for this case. Theory cases: `0`, `-1`.
- **Test entry point** — `Ctor_OnTlsClientOptions_ShouldDefaultToTenSecondTimeoutAndAuthenticationOptions` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `UpgradeToTlsAsync_AsClientAgainstSilentPeer_ShouldCancelAfterHandshakeTimeout` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `UpgradeToTlsAsync_AsServerAgainstSilentPeer_ShouldCancelAfterHandshakeTimeout` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `UpgradeToTlsAsync_WithAlreadyCanceledToken_ShouldThrowOperationCanceledException` contains the setup, invocation, and assertions for this case.

## Sources

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Security/tests/TlsConnectionLayerTests.cs`.
- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Security/tests/TlsClientCertificateTests.cs`.
- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Security/tests/TlsConnectionInfoTests.cs`.
- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Security/tests/TlsConnectionTests.cs`.
- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Security/tests/TlsHandshakeIsolationTests.cs`.
- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Security/tests/TlsConnectionExtensionsTests.cs`.
- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Security/tests/TlsOptionsTests.cs`.
- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Security/tests/Assimalign.Cohesion.Connections.Security.Tests.csproj`.
- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Security/tests/TestObjects/TestCertificateFixture.cs`.
- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Security/tests/TestObjects/TestConnectionFeature.cs`.
- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Security/tests/TestObjects/TestConnectionListener.cs`.
- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Security/tests/TestObjects/TestConnectionFactory.cs`.
- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Security/tests/TestObjects/BlockingConnectionListener.cs`.
