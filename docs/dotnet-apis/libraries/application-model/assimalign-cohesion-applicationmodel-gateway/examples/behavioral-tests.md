# Example: Application Gateway Behavioral Tests

Exercise application gateway resource extensions, credential issuance, and trust behavior using the original repository tests.

[Examples](index.md) · [Assembly overview](../index.md)

## Test-project context

These are the complete `ApplicationGatewayResourceExtensionsTests.cs`, `GatewayCredentialIssuerTests.cs`, and `GatewayTrustTests.cs` listings from the package test project.
Keep them in that project when running them: the project supplies its package references, generated
sources, and shared fixtures. The resource extensions listing makes the test-framework import
explicit where the original project supplies it globally.

## Code

```csharp
using System.Collections.Generic;
using System.IO;
using System;
using Shouldly;
using Xunit;
using Assimalign.Cohesion.ApplicationModel;

namespace Assimalign.Cohesion.ApplicationModel.Gateway.Tests;

public sealed class ApplicationGatewayResourceExtensionsTests
{
    private const string Image =
        "docker.io/library/redis@sha256:0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef";

    [Fact(DisplayName = "Cohesion Test [ApplicationModel.Gateway] - AddContainer: Creates a digest-pinned generic plan")]
    public void AddContainer_DigestPinnedImage_ShouldCreateGenericPlan()
    {
        // Arrange
        var reconciled = new List<string>();
        var deleted = new List<string>();
        var options = new ApplicationGatewayOptions
        {
            ExportDirectory = Path.Combine(Path.GetTempPath(), "cohesion-container-tests", Guid.NewGuid().ToString("N")),
        };
        var gateway = new TestGateway(
            new InMemoryResourceStateManager(),
            [new RecordingController(reconciled, deleted)],
            options: options);
        IApplicationBuilder builder = Application.CreateBuilder(ApplicationName.Parse("appa"), [])
            .UseGateway(gateway);

        // Act
        IApplicationResourceDescriptor descriptor = builder.AddContainer(
            "redis",
            Image,
            container => container
                .AddEndpoint(new ResourceEndpoint("tcp", "tcp", 6379))
                .UseReadinessProbe(ProbeSpec.Tcp("tcp"))
                .AddEnvironment("REDIS_MODE", "standalone"));
        IApplicationModel model = builder.Build().Model;

        // Assert
        descriptor.Resource.Name.ShouldBe((ResourceName)"redis");
        model.Descriptors[0].Plan.ShouldNotBeNull().Container.Artifact.ShouldBe(ArtifactRef.Self);
        ResourceManifest manifest = model.Manifests[0];
        manifest.Kind.ShouldBe("Container");
        manifest.Artifact.Image.ShouldBe(Image);
        manifest.Probes.Readiness.ShouldNotBeNull().Tcp.ShouldBe(true);
        manifest.EnvironmentVariables["REDIS_MODE"].ShouldBe("standalone");
    }

    [Fact(DisplayName = "Cohesion Test [ApplicationModel.Gateway] - AddContainer: Rejects a mutable image tag")]
    public void AddContainer_MutableImageTag_ShouldThrow()
    {
        // Arrange
        IApplicationBuilder builder = Application.CreateBuilder(ApplicationName.Parse("appa"), []);

        // Act
        Action add = () => builder.AddContainer(
            "redis",
            "docker.io/library/redis:latest",
            container => container
                .AddEndpoint(new ResourceEndpoint("tcp", "tcp", 6379))
                .UseReadinessProbe(ProbeSpec.Tcp("tcp")));

        // Assert
        Should.Throw<ArgumentException>(add).Message.ShouldContain("sha256");
    }

    [Fact(DisplayName = "Cohesion Test [ApplicationModel.Gateway] - AddExecutable: Rejects a probe that names an undeclared endpoint")]
    public void AddExecutable_UnknownProbeEndpoint_ShouldThrow()
    {
        // Arrange
        IApplicationBuilder builder = Application.CreateBuilder(ApplicationName.Parse("appa"), []);

        // Act
        Action add = () => builder.AddExecutable(
            "worker",
            "worker.exe",
            executable => executable
                .AddEndpoint(new ResourceEndpoint("health", "http", 8080))
                .UseReadinessProbe(ProbeSpec.Tcp("missing")));

        // Assert
        Should.Throw<InvalidOperationException>(add).Message.ShouldContain("missing");
    }

    [Fact(DisplayName = "Cohesion Test [ApplicationModel.Gateway] - Local validation: Rejects image-only resources before gather")]
    public void Validate_LocalGatewayWithContainer_ShouldRejectBeforeGather()
    {
        // Arrange
        string stateDirectory = Path.Combine(
            Path.GetTempPath(),
            "cohesion-container-tests",
            Guid.NewGuid().ToString("N"));
        var gateway = new LocalGateway(new LocalGatewayOptions { StateDirectory = stateDirectory });
        IApplicationBuilder builder = Application
            .CreateBuilder(ApplicationName.Parse("appa"), ["--environment", "Development"])
            .UseGateway(gateway);
        builder.AddContainer(
            "redis",
            Image,
            container => container
                .AddEndpoint(new ResourceEndpoint("tcp", "tcp", 6379))
                .UseReadinessProbe(ProbeSpec.Tcp("tcp")));
        // Act
        Action validate = () => builder.Build();

        // Assert
        Should.Throw<InvalidOperationException>(validate).Message.ShouldContain("image-only");
    }
}
```

## Walkthrough

- **Covered behavior** — AddContainer: Creates a digest-pinned generic plan.
- **Covered behavior** — AddContainer: Rejects a mutable image tag.
- **Covered behavior** — AddExecutable: Rejects a probe that names an undeclared endpoint.
- **Covered behavior** — Local validation: Rejects image-only resources before gather.

The assertions define the expected result or failure boundary. Test-only doubles supply controlled
inputs; they are not additional shipped APIs.

## Gateway Credential Issuer Tests

### Code

```csharp
using System;
using System.Buffers;
using System.Buffers.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Shouldly;
using Xunit;

using Assimalign.Cohesion.ApplicationModel;
using Assimalign.Cohesion.ApplicationModel.Gateway.Internal;
using Assimalign.Cohesion.IdentityModel;
using Assimalign.Cohesion.IdentityModel.Token.JsonWebToken;

namespace Assimalign.Cohesion.ApplicationModel.Gateway.Tests;

/// <summary>
/// Every credential the gateway mints goes through <see cref="ApplicationProviders.CredentialIssuer"/>
/// first and falls back to the default ES256 application-key issuer, whose output must stay exactly
/// what the gateway minted before credential issuers existed.
/// </summary>
public sealed class GatewayCredentialIssuerTests : IDisposable
{
    private const string displayPrefix = "Cohesion Test [ApplicationModel.Gateway] - Credential issuer: ";
    private const string gatewayName = "test-gateway";
    private const string developerName = "developer@example.test";
    private static readonly DateTimeOffset _now = new(2026, 9, 26, 12, 0, 0, TimeSpan.Zero);
    private static readonly TimeSpan _bootstrapLifetime = TimeSpan.FromHours(24);
    private static readonly TimeSpan _developerLifetime = TimeSpan.FromHours(8);
    private static readonly TimeSpan _testTimeout = TimeSpan.FromSeconds(30);

    private readonly ECDsa _signingKey = ECDsa.Create(ECCurve.NamedCurves.nistP256);
    private readonly string _keyId;
    private readonly string _root = Directory.CreateDirectory(Path.Combine(
        Path.GetTempPath(),
        "cohesion-gateway-credential-tests",
        Guid.NewGuid().ToString("N"))).FullName;

    public GatewayCredentialIssuerTests()
    {
        using var key = new GatewayTrustKey(ECDsa.Create(_signingKey.ExportParameters(includePrivateParameters: true)));
        _keyId = key.KeyId;
    }

    public void Dispose()
    {
        _signingKey.Dispose();
        if (Directory.Exists(_root))
        {
            Directory.Delete(_root, recursive: true);
        }
    }

    [Theory(DisplayName = displayPrefix + "The default issuer keeps every purpose's application-key credential byte for byte")]
    [InlineData(false)]
    [InlineData(true)]
    public async Task IssueCredential_DefaultIssuer_ShouldKeepEveryPurposeByteCompatible(bool registerDeferringIssuer)
    {
        // Arrange
        using var cancellation = new CancellationTokenSource(_testTimeout);
        RecordingCredentialIssuer? issuer = registerDeferringIssuer ? new RecordingCredentialIssuer() : null;

        // Act
        IssuedCredentials issued = await RunScenarioAsync(issuer, cancellation.Token);

        // Assert: each purpose equals the token the pre-issuer gateway signed for the same inputs.
        AssertDefaultCredential(issued.Bootstrap, "api", gatewayName, _bootstrapLifetime);
        AssertDefaultCredential(issued.Access, "api", gatewayName, _bootstrapLifetime);
        AssertDefaultCredential(issued.Telemetry, "logs", "api", _bootstrapLifetime, telemetry: true);
        AssertDefaultCredential(issued.PeerControlPlane, "cohesion-export", gatewayName, _developerLifetime, gatewayUse: true);
        AssertDefaultCredential(issued.RemoteCommand, "cohesion-export", gatewayName, _developerLifetime, gatewayUse: true);
        AssertDefaultCredential(issued.Developer, "cohesion-export", developerName, _developerLifetime);
        issued.TelemetryHeaders.ShouldBe("Authorization: Bearer " + issued.Telemetry + "\n");
        // One default token serves a resource's bootstrap and every gateway call to it in the pass.
        issued.Access.ShouldBe(issued.Bootstrap);

        if (issuer is not null)
        {
            ApplicationCredentialRequest[] requests = issuer.Requests.ToArray();
            requests.ShouldAllBe(request => request.Application == (ApplicationName)"appa" && request.Gateway == (ResourceName)gatewayName);
            requests.ShouldContain(request => Matches(request, ApplicationCredentialPurpose.ResourceBootstrap, "api", gatewayName, _bootstrapLifetime));
            requests.ShouldContain(request => Matches(request, ApplicationCredentialPurpose.ResourceBootstrap, "logs", gatewayName, _bootstrapLifetime));
            requests.ShouldContain(request => Matches(request, ApplicationCredentialPurpose.ResourceAccess, "api", gatewayName, _bootstrapLifetime));
            requests.ShouldContain(request => Matches(request, ApplicationCredentialPurpose.Telemetry, "logs", "api", _bootstrapLifetime));
            requests.ShouldContain(request => Matches(request, ApplicationCredentialPurpose.PeerControlPlane, "cohesion-export", gatewayName, _developerLifetime));
            requests.ShouldContain(request => Matches(request, ApplicationCredentialPurpose.RemoteCommand, "cohesion-export", gatewayName, _developerLifetime));
            requests.ShouldContain(request => Matches(request, ApplicationCredentialPurpose.Developer, "cohesion-export", developerName, _developerLifetime));
        }
    }

    [Fact(DisplayName = displayPrefix + "A registered issuer mints the purposes it handles and the default issuer the rest")]
    public async Task IssueCredential_IssuerForSomePurposes_ShouldFallBackForTheRest()
    {
        // Arrange: an identity provider issues bootstrap, telemetry (with its own scheme), and remote
        // command credentials, and defers resource access, peer discovery, and developer tokens.
        using var cancellation = new CancellationTokenSource(_testTimeout);
        var issuer = new RecordingCredentialIssuer(request => request.Purpose switch
        {
            ApplicationCredentialPurpose.ResourceBootstrap =>
                new ApplicationCredential("Bearer", $"idp-bootstrap-{request.Audience}", _now.AddHours(1)),
            ApplicationCredentialPurpose.Telemetry =>
                new ApplicationCredential("DPoP", $"idp-telemetry-{request.Audience}-{request.Subject}", _now.AddHours(1)),
            ApplicationCredentialPurpose.RemoteCommand =>
                new ApplicationCredential("bearer", "idp-remote-command", _now.AddHours(1)),
            _ => null,
        });

        // Act
        IssuedCredentials issued = await RunScenarioAsync(issuer, cancellation.Token);

        // Assert
        issued.Bootstrap.ShouldBe("idp-bootstrap-api");
        issued.TelemetryHeaders.ShouldBe("Authorization: DPoP idp-telemetry-logs-api\n");
        issued.RemoteCommand.ShouldBe("idp-remote-command");
        AssertDefaultCredential(issued.Access, "api", gatewayName, _bootstrapLifetime);
        AssertDefaultCredential(issued.PeerControlPlane, "cohesion-export", gatewayName, _developerLifetime, gatewayUse: true);
        AssertDefaultCredential(issued.Developer, "cohesion-export", developerName, _developerLifetime);
    }

    [Theory(DisplayName = displayPrefix + "A non-bearer credential for a bearer-only carrier is refused")]
    [InlineData(ApplicationCredentialPurpose.Developer, "DPoP", "idp-developer")]
    [InlineData(ApplicationCredentialPurpose.Developer, "Bearer", "idp\r\nX-Injected: yes")]
    [InlineData(ApplicationCredentialPurpose.ResourceBootstrap, "Basic", "idp-bootstrap")]
    public async Task IssueCredential_UnpresentableCredential_ShouldBeRefused(
        ApplicationCredentialPurpose purpose,
        string scheme,
        string value)
    {
        // Arrange
        using var cancellation = new CancellationTokenSource(_testTimeout);
        var issuer = new RecordingCredentialIssuer(request => request.Purpose == purpose
            ? new ApplicationCredential(scheme, value, _now.AddHours(1))
            : null);
        TestGateway gateway = CreateGateway(new CredentialCapturingController());
        IApplicationBuilder builder = Application
            .CreateBuilder((ApplicationName)"appa", ["--environment", AppEnvironment.Keys.Local])
            .UseGateway(gateway);
        builder.AddResource(new TestResource("api"));
        builder.Providers.CredentialIssuer = issuer;
        IApplicationModel model = builder.Build().Model;

        try
        {
            // Act
            Exception? failure = purpose == ApplicationCredentialPurpose.Developer
                ? await Record.ExceptionAsync(() => gateway.IssueDeveloperTokenAsync(model, developerName, cancellation.Token))
                : await Record.ExceptionAsync(() => ((IApplicationGateway)gateway).StartAsync(model, cancellation.Token));

            // Assert
            failure.ShouldBeOfType<InvalidOperationException>().Message.ShouldContain(purpose.ToString(), Case.Sensitive);
        }
        finally
        {
            await ((IApplicationGateway)gateway).StopAsync(CancellationToken.None);
        }
    }

    [Theory(DisplayName = displayPrefix + "An expired cached resource credential is reissued instead of presented")]
    [InlineData(false)]
    [InlineData(true)]
    public async Task GetResourceCommandCredential_CachedCredentialExpired_ShouldReissue(bool registerIssuer)
    {
        // Arrange: the served control plane dispatches with the pass cache long after the pass. The
        // identity provider's access credentials live one hour; the default issuer's live the
        // bootstrap lifetime.
        using var cancellation = new CancellationTokenSource(_testTimeout);
        var clock = new FixedUtcTimeProvider(_now);
        int minted = 0;
        RecordingCredentialIssuer? issuer = registerIssuer
            ? new RecordingCredentialIssuer(request => request.Purpose == ApplicationCredentialPurpose.ResourceAccess
                ? new ApplicationCredential(
                    "Bearer",
                    $"idp-access-{Interlocked.Increment(ref minted)}",
                    clock.GetUtcNow().AddHours(1))
                : null)
            : null;
        TimeSpan lifetime = registerIssuer ? TimeSpan.FromHours(1) : _bootstrapLifetime;
        var controller = new CredentialCapturingController();
        TestGateway gateway = CreateGateway(controller, clock: clock);
        IApplicationBuilder builder = Application
            .CreateBuilder((ApplicationName)"appa", ["--environment", AppEnvironment.Keys.Local])
            .UseGateway(gateway);
        builder.AddResource(CreateManifest("api", "appa", "http", "http", 8080));
        if (issuer is not null)
        {
            builder.Providers.CredentialIssuer = issuer;
        }

        IApplicationModel model = builder.Build().Model;
        var credentials = (IResourceCommandCredentialProvider)gateway;
        IApplicationGateway control = gateway;

        try
        {
            await control.StartAsync(model, cancellation.Token);

            // Act
            string first = await credentials.GetResourceCommandCredentialAsync(model.Name, "api", cancellation.Token);
            clock.Advance(lifetime - TimeSpan.FromMinutes(1));
            string beforeExpiry = await credentials.GetResourceCommandCredentialAsync(model.Name, "api", cancellation.Token);
            clock.Advance(TimeSpan.FromMinutes(2));
            string afterExpiry = await credentials.GetResourceCommandCredentialAsync(model.Name, "api", cancellation.Token);

            // Assert
            beforeExpiry.ShouldBe(first);
            afterExpiry.ShouldNotBe(first);
            if (registerIssuer)
            {
                first.ShouldBe("idp-access-1");
                afterExpiry.ShouldBe("idp-access-2");
            }
            else
            {
                // Until it expires, the default access token is the pass's bootstrap token.
                first.ShouldBe(Encoding.UTF8.GetString(controller.Inputs["api"].BootstrapCredential.Span));
                using JsonDocument payload = JsonDocument.Parse(Base64Url.DecodeFromChars(afterExpiry.Split('.')[1]));
                payload.RootElement.GetProperty("iat").GetInt64().ShouldBe(clock.GetUtcNow().ToUnixTimeSeconds());
            }
        }
        finally
        {
            await control.StopAsync(CancellationToken.None);
        }
    }

    // One pass that exercises every mint site: a telemetry sink and an emitter that depends on it,
    // a remote reference discovered through a peer gateway with a command delivered to it, then a
    // resource-access credential and a developer token.
    private async Task<IssuedCredentials> RunScenarioAsync(
        RecordingCredentialIssuer? issuer,
        CancellationToken cancellationToken)
    {
        var peer = new RecordingPeerControlPlaneClient(CreatePeerExport());
        var controller = new CredentialCapturingController();
        TestGateway gateway = CreateGateway(controller, peer);
        IApplicationBuilder builder = Application
            .CreateBuilder((ApplicationName)"appa", ["--environment", AppEnvironment.Keys.Local])
            .UseGateway(gateway);
        IApplicationResourceDescriptor logs = builder.AddResource(CreateManifest("logs", "appa", "otlp", "https", 4318));
        IApplicationResourceDescriptor api = builder.AddResource(CreateManifest("api", "appa", "http", "http", 8080));
        api.DependsOn(logs);
        IApplicationResourceDescriptor remote = builder.RemoteReference(
            new ExternalResourceDeclaration("remote-api", "peer", ["https"], optional: false, CreatePeerManifest()),
            options => options.Gateway(new Uri("https://peer.test:9443")));
        using JsonDocument payload = JsonDocument.Parse("{\"value\":\"configured\"}");
        builder.AddCommand(ResourceCommands.Create("test.apply", "setting", remote.Resource, "appa",
            payload.RootElement, GatewayCommandJsonContext.Default.JsonElement));
        builder.Providers.Telemetry = ResourceTelemetrySink.FromResource(logs);
        if (issuer is not null)
        {
            builder.Providers.CredentialIssuer = issuer;
        }

        IApplicationModel model = builder.Build().Model;
        IApplicationGateway control = gateway;
        try
        {
            await control.StartAsync(model, cancellationToken);
            string access = await ((IResourceCommandCredentialProvider)gateway)
                .GetResourceCommandCredentialAsync(model.Name, "api", cancellationToken);
            string developer = await gateway.IssueDeveloperTokenAsync(model, developerName, cancellationToken);
            IResourceTelemetry telemetry = controller.Telemetry["api"].ShouldNotBeNull();
            string headers = Encoding.UTF8.GetString(telemetry.HeadersDocument.Span);
            return new IssuedCredentials(
                Encoding.UTF8.GetString(controller.Inputs["api"].BootstrapCredential.Span),
                access,
                headers[(headers.LastIndexOf(' ') + 1)..].TrimEnd('\n'),
                headers,
                peer.DiscoveryCredentials.ShouldHaveSingleItem(),
                peer.CommandCredentials.ShouldHaveSingleItem(),
                developer);
        }
        finally
        {
            await control.StopAsync(CancellationToken.None);
        }
    }

    private TestGateway CreateGateway(
        IApplicationResourceController controller,
        IControlPlaneClient? peer = null,
        TimeProvider? clock = null)
    {
        var options = new ApplicationGatewayOptions
        {
            ExportDirectory = _root,
            TimeProvider = clock ?? new FixedUtcTimeProvider(_now),
            TrustKeyRepository = new FixedTrustKeyRepository(_signingKey),
            BootstrapCredentialLifetime = _bootstrapLifetime,
            DeveloperTokenLifetime = _developerLifetime,
            ControlPlaneClient = peer,
        };
        return new TestGateway(new InMemoryResourceStateManager(), [controller], options: options, name: gatewayName);
    }

    // The exact credential the gateway's default issuer signed before credential issuers existed,
    // written with that recipe's literal claim names. ES256 signatures and jti are random, so the
    // JOSE header must match byte for byte and the payload byte for byte once jti is masked.
    private void AssertDefaultCredential(
        string compact,
        string audience,
        string subject,
        TimeSpan lifetime,
        bool telemetry = false,
        bool gatewayUse = false)
    {
        var descriptor = new JsonWebTokenDescriptor
        {
            Id = Guid.NewGuid().ToString("N"),
            Issuer = "appa",
            Subject = new SubjectIdentifier(subject, issuer: "appa"),
            TokenType = "JWT",
            IssuedAt = _now,
            NotBefore = _now,
            ExpiresAt = _now.Add(lifetime),
        };
        descriptor.Audiences.Add(audience);
        if (telemetry)
        {
            descriptor.Claims.Add(new IdentityClaim("scope", "telemetry"));
        }
        if (gatewayUse)
        {
            descriptor.Claims.Add(new IdentityClaim("cohesion_token_use", "gateway"));
        }

        string expected = JsonWebTokenWriter.CreateEs256(_signingKey, _keyId).Write(descriptor);
        string[] actualParts = compact.Split('.');
        string[] expectedParts = expected.Split('.');
        actualParts.Length.ShouldBe(3);
        actualParts[0].ShouldBe(expectedParts[0]);
        MaskTokenId(actualParts[1]).ShouldBe(MaskTokenId(expectedParts[1]));
        JsonWebToken token = JsonWebToken.Parse(compact);
        token.Id.ShouldNotBeNullOrWhiteSpace();
        IJsonWebTokenSignatureVerifier verifier = EcdsaJsonWebTokenSignatureVerifier.Create(_signingKey, _keyId);
        verifier.Verify(token.Algorithm!, Encoding.ASCII.GetBytes(token.SigningInput!), Base64Url.DecodeFromChars(token.Parts!.Signature))
            .ShouldBeTrue();
    }

    private static string MaskTokenId(string encodedPayload)
    {
        using JsonDocument payload = JsonDocument.Parse(Base64Url.DecodeFromChars(encodedPayload));
        var buffer = new ArrayBufferWriter<byte>();
        using (var writer = new Utf8JsonWriter(buffer))
        {
            writer.WriteStartObject();
            foreach (JsonProperty claim in payload.RootElement.EnumerateObject())
            {
                if (claim.NameEquals("jti"))
                {
                    writer.WriteString("jti", "<masked>");
                }
                else
                {
                    claim.WriteTo(writer);
                }
            }

            writer.WriteEndObject();
        }

        return Encoding.UTF8.GetString(buffer.WrittenSpan);
    }

    private static bool Matches(
        ApplicationCredentialRequest request,
        ApplicationCredentialPurpose purpose,
        string audience,
        string subject,
        TimeSpan lifetime) =>
        request.Purpose == purpose &&
        request.Audience == audience &&
        request.Subject == subject &&
        request.Lifetime == lifetime;

    private static ApplicationExportDocument CreatePeerExport()
    {
        var peerGateway = new TestGateway(
            new InMemoryResourceStateManager(),
            [new CredentialCapturingController()],
            name: "peer-gateway");
        IApplicationBuilder builder = Application
            .CreateBuilder((ApplicationName)"peer", ["--environment", AppEnvironment.Keys.Local])
            .UseGateway(peerGateway);
        builder.AddResource(CreatePeerManifest());
        using var peerKey = new GatewayTrustKey(ECDsa.Create(ECCurve.NamedCurves.nistP256));
        return ApplicationExportDocument.Create(
            builder.Build().Model,
            "1",
            new Dictionary<ResourceName, IReadOnlyList<ApplicationExportEndpoint>>
            {
                ["remote-api"] = [new ApplicationExportEndpoint("https", "peer.test:443", "https://peer.test:443")],
            },
            trustKey: peerKey.PublicJwk);
    }

    private static ResourceManifest CreatePeerManifest() =>
        CreateManifest("remote-api", "peer", "https", "https", 443) with
        {
            Commands = [new ResourceManifestCommand("test.apply")],
        };

    private static ResourceManifest CreateManifest(
        string name,
        string application,
        string endpoint,
        string scheme,
        int port) => new()
        {
            Name = name,
            Application = application,
            Kind = "test",
            ApplicationModel = "Assimalign.Cohesion.Test.ApplicationModel",
            Artifact = new ResourceManifestArtifact
            {
                Assembly = name + ".dll",
                AppHost = name,
            },
            Endpoints =
            [
                new ResourceManifestEndpoint
                {
                    Name = endpoint,
                    Scheme = scheme,
                    Protocol = "tcp",
                    ContainerPort = port,
                },
            ],
            ControlPlane = new ResourceManifestControlPlane
            {
                Endpoint = endpoint,
                Path = "/cohesion/v1",
            },
            Lifecycle = new ResourceManifestLifecycle
            {
                Workload = WorkloadKind.Deployment,
                Replicas = 1,
                RestartPolicy = "Never",
            },
        };

    private sealed record IssuedCredentials(
        string Bootstrap,
        string Access,
        string Telemetry,
        string TelemetryHeaders,
        string PeerControlPlane,
        string RemoteCommand,
        string Developer);
}
```

### Walkthrough

- **Covered behavior** — The default issuer keeps every purpose's application-key credential byte for byte.
- **Covered behavior** — A registered issuer mints the purposes it handles and the default issuer the rest.
- **Covered behavior** — A non-bearer credential for a bearer-only carrier is refused.
- **Covered behavior** — An expired cached resource credential is reissued instead of presented.

## Gateway Trust Tests

### Code

```csharp
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Shouldly;
using Xunit;

using Assimalign.Cohesion.ApplicationModel;
using Assimalign.Cohesion.ApplicationModel.Gateway.Internal;
using Assimalign.Cohesion.IdentityModel.Token.JsonWebToken;

namespace Assimalign.Cohesion.ApplicationModel.Gateway.Tests;

public sealed class GatewayTrustTests
{
    [Fact(DisplayName = "Cohesion Test [ApplicationModel.Gateway] - Trust allow kinds round-trip with backward-compatible unrestricted documents")]
    public void TrustedIssuerDocument_ShouldPreserveAllowedKinds()
    {
        using var key = new GatewayTrustKey(ECDsa.Create(ECCurve.NamedCurves.nistP256));
        string[] kinds = ["rezolvr.add-a-record", "identityhub.add-client", "rezolvr.add-a-record"];
        var restricted = new TrustedIssuer("peer", key.PublicJwk, kinds);
        kinds[0] = "changed";
        restricted.AllowedCommandKinds.ShouldBe(["identityhub.add-client", "rezolvr.add-a-record"]);
        byte[] document = TrustedIssuerDocument.Write([restricted]);
        TrustedIssuerDocument.Parse(document)[0].AllowedCommandKinds.ShouldBe(restricted.AllowedCommandKinds);
        byte[] legacy = TrustedIssuerDocument.Write([new TrustedIssuer("peer", key.PublicJwk)]);
        Encoding.UTF8.GetString(legacy).ShouldNotContain("allowedCommandKinds", Case.Sensitive);
        TrustedIssuerDocument.Parse(legacy)[0].AllowedCommandKinds.ShouldBeEmpty();
        var empty = new TrustedIssuer("peer", key.PublicJwk, []);
        empty.AllowedCommandKinds.ShouldBeEmpty();
    }

    private static readonly TimeSpan _testTimeout = TimeSpan.FromSeconds(30);

    [Fact(DisplayName = "Cohesion Test [ApplicationModel.Gateway] - LoadOrCreateAsync: Should persist a distinct P-256 trust key per application")]
    public async Task LoadOrCreateAsync_WhenReloaded_ShouldPersistApplicationTrustKey()
    {
        // Arrange
        string root = CreateTestDirectory();
        using var cancellation = new CancellationTokenSource(_testTimeout);
        ApplicationName applicationA = ApplicationName.Parse("appa");
        ApplicationName applicationB = ApplicationName.Parse("appb");
        ResourceName gateway = "test-gateway";

        try
        {
            // Act
            using var first = new GatewayTrustKey(await new GatewayTrustKeyStore(root)
                .LoadOrCreateAsync(applicationA, gateway, cancellation.Token));
            using var reloaded = new GatewayTrustKey(await new GatewayTrustKeyStore(root)
                .LoadOrCreateAsync(applicationA, gateway, cancellation.Token));
            using var otherApplication = new GatewayTrustKey(await new GatewayTrustKeyStore(root)
                .LoadOrCreateAsync(applicationB, gateway, cancellation.Token));

            // Assert
            first.PrivateKey.KeySize.ShouldBe(256);
            reloaded.PrivateKey.KeySize.ShouldBe(256);
            reloaded.KeyId.ShouldBe(first.KeyId);
            reloaded.PublicJwk.GetRawText().ShouldBe(first.PublicJwk.GetRawText());
            otherApplication.KeyId.ShouldNotBe(first.KeyId);
            AssertPublicJwk(first.PublicJwk, first.KeyId);
            File.Exists(Path.Combine(
                root,
                applicationA.ToString(),
                "trust",
                gateway.ToString(),
                "private-key.p8.protected")).ShouldBeTrue();
        }
        finally
        {
            DeleteTestDirectory(root);
        }
    }

    [Fact(DisplayName = "Cohesion Test [ApplicationModel.Gateway] - ReconcileAsync: Should rotate an audience-bound ES256 bootstrap token on every pass")]
    public async Task ReconcileAsync_OnEveryPass_ShouldRotateValidEs256BootstrapToken()
    {
        // Arrange
        string root = CreateTestDirectory();
        using var cancellation = new CancellationTokenSource(_testTimeout);
        var options = new ApplicationGatewayOptions
        {
            ExportDirectory = root,
            BootstrapCredentialLifetime = TimeSpan.FromHours(24),
        };
        var controller = new InputHistoryController();
        var gateway = new TestGateway(
            new InMemoryResourceStateManager(),
            [controller],
            options: options,
            name: "test-gateway");
        IApplicationModel model = BuildModel(gateway, "appa", "secret-store");
        IApplicationGateway control = gateway;
        bool started = false;

        try
        {
            // Act
            await control.StartAsync(model, cancellation.Token);
            started = true;
            await control.ReconcileAsync(model, cancellation.Token);

            // Assert
            controller.Inputs.Count.ShouldBe(2);
            ResourceInputs firstInputs = controller.Inputs[0];
            ResourceInputs secondInputs = controller.Inputs[1];
            string firstCompact = Encoding.ASCII.GetString(firstInputs.BootstrapCredential.Span);
            string secondCompact = Encoding.ASCII.GetString(secondInputs.BootstrapCredential.Span);
            JsonWebToken first = JsonWebToken.Parse(firstCompact);
            JsonWebToken second = JsonWebToken.Parse(secondCompact);

            firstCompact.ShouldNotBe(secondCompact);
            first.Id.ShouldNotBeNullOrWhiteSpace();
            second.Id.ShouldNotBeNullOrWhiteSpace();
            second.Id.ShouldNotBe(first.Id);
            AssertToken(first, "secret-store", TimeSpan.FromHours(24));
            AssertToken(second, "secret-store", TimeSpan.FromHours(24));

            firstInputs.ApplicationTrustKey.ToArray()
                .ShouldBe(secondInputs.ApplicationTrustKey.ToArray());
            using JsonDocument publicJwkDocument = JsonDocument.Parse(
                firstInputs.ApplicationTrustKey);
            JsonElement publicJwk = publicJwkDocument.RootElement;
            AssertPublicJwk(publicJwk, first.Header.KeyId.ShouldNotBeNull());
            VerifyWithPublicJwk(first, publicJwk).ShouldBeTrue();
            VerifyWithPublicJwk(second, publicJwk).ShouldBeTrue();
        }
        finally
        {
            if (started)
            {
                await control.StopAsync(CancellationToken.None);
            }

            DeleteTestDirectory(root);
        }
    }

    [Fact(DisplayName = "Cohesion Test [ApplicationModel.Gateway] - IssueDeveloperTokenAsync: Should issue an ES256 cohesion-export token bounded to eight hours")]
    public async Task IssueDeveloperTokenAsync_WhenRequested_ShouldIssueBoundedExportToken()
    {
        // Arrange
        string root = CreateTestDirectory();
        using var cancellation = new CancellationTokenSource(_testTimeout);
        var options = new ApplicationGatewayOptions
        {
            ExportDirectory = root,
            DeveloperTokenLifetime = TimeSpan.FromHours(8),
        };
        var gateway = new TestGateway(
            new InMemoryResourceStateManager(),
            [new InputHistoryController()],
            options: options,
            name: "test-gateway");
        IApplicationModel model = BuildModel(gateway, "appa", "api");
        IApplicationTrustGateway trustGateway = gateway;

        try
        {
            // Act
            string compact = await trustGateway.IssueDeveloperTokenAsync(
                model,
                "developer@example.test",
                cancellation.Token);
            JsonWebToken token = JsonWebToken.Parse(compact);
            IReadOnlyList<TrustedIssuer> issuers = trustGateway.GetTrustedIssuers(model.Name);

            // Assert
            token.Id.ShouldNotBeNullOrWhiteSpace();
            AssertToken(token, "cohesion-export", TimeSpan.FromHours(8));
            token.Subject.ShouldNotBeNull().Value.ShouldBe("developer@example.test");
            issuers.Count.ShouldBe(1);
            issuers[0].Issuer.ShouldBe("appa");
            VerifyWithPublicJwk(token, issuers[0].PublicKey).ShouldBeTrue();
        }
        finally
        {
            // IssueDeveloperTokenAsync initializes trust without opening a model session;
            // StopAsync clears that owned key in both the success and assertion-failure paths.
            await ((IApplicationGateway)gateway).StopAsync(CancellationToken.None);
            DeleteTestDirectory(root);
        }
    }

    [Fact(DisplayName = "Cohesion Test [ApplicationModel.Gateway] - Trust repository: Platform override owns load and rotation")]
    public async Task TrustKeyRepository_WhenConfigured_ShouldOwnPersistenceAndRotation()
    {
        // Arrange
        string root = CreateTestDirectory();
        using var cancellation = new CancellationTokenSource(_testTimeout);
        var repository = new RecordingTrustKeyRepository();
        var options = new ApplicationGatewayOptions
        {
            ExportDirectory = root,
            TrustKeyRepository = repository,
        };
        var gateway = new TestGateway(
            new InMemoryResourceStateManager(),
            [new InputHistoryController()],
            options: options,
            name: "test-gateway");
        IApplicationModel model = BuildModel(gateway, "appa", "api");
        IApplicationTrustGateway trustGateway = gateway;

        try
        {
            // Act
            JsonWebToken first = JsonWebToken.Parse(await trustGateway.IssueDeveloperTokenAsync(
                model,
                "developer@example.test",
                cancellation.Token));
            await trustGateway.RotateTrustKeyAsync(model, cancellation.Token);
            JsonWebToken second = JsonWebToken.Parse(await trustGateway.IssueDeveloperTokenAsync(
                model,
                "developer@example.test",
                cancellation.Token));

            // Assert
            repository.LoadCount.ShouldBe(1);
            repository.RotateCount.ShouldBe(1);
            second.Header.KeyId.ShouldNotBe(first.Header.KeyId);
            File.Exists(Path.Combine(root, "appa", "trust", "test-gateway", "private-key.p8.protected"))
                .ShouldBeFalse();
        }
        finally
        {
            await ((IApplicationGateway)gateway).StopAsync(CancellationToken.None);
            DeleteTestDirectory(root);
        }
    }

    [Fact(DisplayName = "Cohesion Test [ApplicationModel.Gateway] - TrustedIssuers: Loads peer keys from the registered trust store")]
    public async Task StartAsync_WithBoundTrustStore_ShouldLoadApplicationTrustedIssuers()
    {
        // Arrange
        string root = CreateTestDirectory();
        using var cancellation = new CancellationTokenSource(_testTimeout);
        using var peerKey = new GatewayTrustKey(ECDsa.Create(ECCurve.NamedCurves.nistP256));
        var store = new RecordingTrustedIssuerStore([new TrustedIssuer("peer", peerKey.PublicJwk)], "KeyStore");
        var gateway = new TestGateway(
            new InMemoryResourceStateManager(),
            [new TrustStoreController()],
            options: new ApplicationGatewayOptions { ExportDirectory = root },
            name: "test-gateway");
        IApplicationBuilder builder = Application
            .CreateBuilder(ApplicationName.Parse("appa"), ["--environment", AppEnvironment.Keys.Production])
            .UseGateway(gateway);
        builder.AddResource(CreateTrustStoreManifest());
        builder.Providers.TrustStore = new ResourceProviderBinding<ITrustedIssuerStore>("secrets", store);
        IApplicationModel model = builder.Build().Model;
        IApplicationGateway control = gateway;
        bool started = false;

        try
        {
            // Act
            await control.StartAsync(model, cancellation.Token);
            started = true;
            IReadOnlyList<TrustedIssuer> issuers = gateway.GetTrustedIssuers(model.Name);

            // Assert
            issuers.Count.ShouldBe(2);
            issuers[0].Issuer.ShouldBe("appa");
            issuers[1].Issuer.ShouldBe("peer");
            store.Reads.ShouldNotBeEmpty();
            ResourceProviderConnection connection = store.Reads[^1].ShouldNotBeNull();
            connection.Caller.ShouldBe(ApplicationName.Parse("appa"));
            connection.Resource.ShouldBe((ResourceName)"secrets");
            connection.ControlPlaneAddress.ShouldBe(new Uri("https://127.0.0.1:8443/cohesion/v1"));
            JsonWebToken.Parse(connection.BearerCredential).Audiences.ShouldBe(["secrets"]);
        }
        finally
        {
            if (started)
            {
                await control.StopAsync(CancellationToken.None);
            }

            DeleteTestDirectory(root);
        }
    }

    [Fact(DisplayName = "Cohesion Test [ApplicationModel.Gateway] - TrustedIssuers: The trust store precedes an independent remote on the first production pass")]
    public async Task StartAsync_WhenRemoteIsDeclaredBeforeTrustStore_ShouldLoadTrustFirst()
    {
        // Arrange
        string root = CreateTestDirectory();
        using var cancellation = new CancellationTokenSource(_testTimeout);
        using var peerKey = new GatewayTrustKey(ECDsa.Create(ECCurve.NamedCurves.nistP256));
        var store = new RecordingTrustedIssuerStore([new TrustedIssuer("peer", peerKey.PublicJwk)], "KeyStore");
        var gateway = new TestGateway(
            new InMemoryResourceStateManager(),
            [new TrustStoreController()],
            options: new ApplicationGatewayOptions { ExportDirectory = root },
            name: "test-gateway");
        var resolver = new TrustAwareExternalResolver(gateway);
        IApplicationBuilder builder = Application
            .CreateBuilder(ApplicationName.Parse("appa"), ["--environment", AppEnvironment.Keys.Production])
            .UseGateway(gateway);
        builder.RemoteReference(
            CreateExternalDeclaration(),
            options => options.Bind(resolver));
        builder.AddResource(CreateTrustStoreManifest());
        builder.Providers.TrustStore = new ResourceProviderBinding<ITrustedIssuerStore>("secrets", store);
        IApplicationModel model = builder.Build().Model;
        IApplicationGateway control = gateway;
        bool started = false;

        try
        {
            // Act
            await control.StartAsync(model, cancellation.Token);
            started = true;

            // Assert
            resolver.PeerWasTrusted.ShouldBeTrue();
        }
        finally
        {
            if (started)
            {
                await control.StopAsync(CancellationToken.None);
            }

            DeleteTestDirectory(root);
        }
    }

    [Theory(DisplayName = "Cohesion Test [ApplicationModel.Gateway] - TrustedIssuers: An unavailable trust store is fatal only outside Local")]
    [InlineData(AppEnvironment.Keys.Local, false)]
    [InlineData(AppEnvironment.Keys.Production, true)]
    public async Task StartAsync_TrustStoreReadFailure_ShouldFailOnlyOutsideLocal(string environment, bool expectFailure)
    {
        // Arrange
        string root = CreateTestDirectory();
        using var cancellation = new CancellationTokenSource(_testTimeout);
        var store = new RecordingTrustedIssuerStore(resourceKind: "KeyStore")
        {
            ReadFailure = new System.Net.Http.HttpRequestException("store offline"),
        };
        var gateway = new TestGateway(
            new InMemoryResourceStateManager(),
            [new TrustStoreController()],
            options: new ApplicationGatewayOptions { ExportDirectory = root },
            name: "test-gateway");
        IApplicationBuilder builder = Application
            .CreateBuilder(ApplicationName.Parse("appa"), ["--environment", environment])
            .UseGateway(gateway);
        builder.AddResource(CreateTrustStoreManifest());
        builder.Providers.TrustStore = new ResourceProviderBinding<ITrustedIssuerStore>("secrets", store);
        IApplicationModel model = builder.Build().Model;
        IApplicationGateway control = gateway;

        try
        {
            // Act
            Exception? failure = await Record.ExceptionAsync(() => control.StartAsync(model, cancellation.Token));

            // Assert
            if (expectFailure)
            {
                failure.ShouldBeOfType<InvalidOperationException>()
                    .Message.ShouldContain("could not load TrustedIssuers from its trust store 'secrets'", Case.Sensitive);
            }
            else
            {
                failure.ShouldBeNull();
                gateway.GetTrustedIssuers(model.Name).ShouldHaveSingleItem().Issuer.ShouldBe("appa");
            }

            store.Reads.ShouldNotBeEmpty();
        }
        finally
        {
            await control.StopAsync(CancellationToken.None);
            DeleteTestDirectory(root);
        }
    }

    [Theory(DisplayName = "Cohesion Test [ApplicationModel.Gateway] - TrustedIssuers: Only Local retains a fallback after store not-found")]
    [InlineData(AppEnvironment.Keys.Local, true)]
    [InlineData(AppEnvironment.Keys.Development, false)]
    [InlineData(AppEnvironment.Keys.Staging, false)]
    [InlineData(AppEnvironment.Keys.Production, false)]
    public async Task StartAsync_StoreNotFound_ShouldRestrictLocalFallback(string environment, bool expectPeer)
    {
        // Arrange
        string root = CreateTestDirectory();
        using var cancellation = new CancellationTokenSource(_testTimeout);
        using var peerKey = new GatewayTrustKey(ECDsa.Create(ECCurve.NamedCurves.nistP256));
        var fallback = new TrustedIssuer("peer", peerKey.PublicJwk);
        await TrustedIssuerDocument.WriteFileAsync(
            Path.Combine(root, "appa", "trust", "trusted-issuers.json"),
            [fallback],
            cancellation.Token);
        // A trust store that has persisted nothing yet answers null.
        var store = new RecordingTrustedIssuerStore(issuers: null, resourceKind: "KeyStore");
        var gateway = new TestGateway(
            new InMemoryResourceStateManager(),
            [new TrustStoreController()],
            options: new ApplicationGatewayOptions { ExportDirectory = root },
            name: "test-gateway");
        IApplicationBuilder builder = Application
            .CreateBuilder(ApplicationName.Parse("appa"), ["--environment", environment])
            .UseGateway(gateway);
        builder.AddResource(CreateTrustStoreManifest());
        builder.Providers.TrustStore = new ResourceProviderBinding<ITrustedIssuerStore>("secrets", store);
        IApplicationModel model = builder.Build().Model;
        IApplicationGateway control = gateway;
        bool started = false;

        try
        {
            // Act
            await control.StartAsync(model, cancellation.Token);
            started = true;

            // Assert
            IReadOnlyList<TrustedIssuer> issuers = gateway.GetTrustedIssuers(model.Name);
            issuers.Count.ShouldBe(expectPeer ? 2 : 1);
            issuers[0].Issuer.ShouldBe("appa");
            if (expectPeer) { issuers[1].Issuer.ShouldBe("peer"); }
        }
        finally
        {
            if (started)
            {
                await control.StopAsync(CancellationToken.None);
            }

            DeleteTestDirectory(root);
        }
    }

    [Fact(DisplayName = "Cohesion Test [ApplicationModel.Gateway] - Trust add: Platform endpoint seam supports a production one-shot grant")]
    public async Task AddTrustedIssuerAsync_WithPlatformStoreEndpoint_ShouldPersistProductionGrant()
    {
        // Arrange
        string root = CreateTestDirectory();
        using var cancellation = new CancellationTokenSource(_testTimeout);
        var store = new RecordingTrustedIssuerStore(resourceKind: "KeyStore");
        Uri storeEndpoint = new("https://secrets.appa.test:8443/");
        var gateway = new TestGateway(
            new InMemoryResourceStateManager(),
            [new InputHistoryController()],
            options: new ApplicationGatewayOptions { ExportDirectory = root },
            name: "test-gateway",
            trustStoreEndpointResolver: (_, _) => storeEndpoint);
        IApplicationBuilder builder = Application
            .CreateBuilder(ApplicationName.Parse("appa"), ["--environment", AppEnvironment.Keys.Production])
            .UseGateway(gateway);
        builder.AddResource(CreateTrustStoreManifest());
        builder.Providers.TrustStore = new ResourceProviderBinding<ITrustedIssuerStore>("secrets", store);
        IApplicationModel model = builder.Build().Model;

        var peerGateway = new TestGateway(
            new InMemoryResourceStateManager(),
            [new InputHistoryController()],
            name: "peer-gateway");
        IApplicationModel peerModel = BuildModel(peerGateway, "peer", "api");
        using var peerKey = new GatewayTrustKey(ECDsa.Create(ECCurve.NamedCurves.nistP256));
        ApplicationExportDocument export = ApplicationExportDocument.Create(
            peerModel,
            "1",
            trustKey: peerKey.PublicJwk);

        try
        {
            // Act
            await gateway.AddTrustedIssuerAsync(model, "peer", export, cancellation.Token);

            // Assert
            (ResourceProviderConnection? connection, string owner, TrustedIssuer issuer) = store.Adds.ShouldHaveSingleItem();
            connection.ShouldNotBeNull().ControlPlaneAddress.ShouldBe(new Uri("https://secrets.appa.test:8443/cohesion/v1"));
            JsonWebToken.Parse(connection.BearerCredential).Audiences.ShouldBe(["secrets"]);
            owner.ShouldBe(model.Owner);
            issuer.Issuer.ShouldBe("peer");
            issuer.AllowedCommandKinds.ShouldBeEmpty();
            gateway.GetTrustedIssuers(model.Name).Count.ShouldBe(2);
            File.Exists(Path.Combine(root, "appa", "trust", "trusted-issuers.json")).ShouldBeFalse();
        }
        finally
        {
            await ((IApplicationGateway)gateway).StopAsync(CancellationToken.None);
            DeleteTestDirectory(root);
        }
    }

    [Fact(DisplayName = "Cohesion Test [ApplicationModel.Gateway] - Trust add: A restricted grant reaches the trust store with its allowed command kinds")]
    public async Task AddTrustedIssuerAsync_RestrictedGrant_ShouldHandTheStoreItsAllowedKinds()
    {
        // Arrange
        string root = CreateTestDirectory();
        using var cancellation = new CancellationTokenSource(_testTimeout);
        var store = new RecordingTrustedIssuerStore();
        var gateway = new TestGateway(
            new InMemoryResourceStateManager(),
            [new InputHistoryController()],
            options: new ApplicationGatewayOptions { ExportDirectory = root },
            name: "test-gateway");
        IApplicationBuilder builder = Application
            .CreateBuilder(ApplicationName.Parse("appa"), ["--environment", AppEnvironment.Keys.Production])
            .UseGateway(gateway);
        builder.AddResource(new TestResource("api"));
        // A trust store outside the model authenticates itself and receives no connection.
        builder.Providers.TrustStore = new ResourceProviderBinding<ITrustedIssuerStore>(null, store);
        IApplicationModel model = builder.Build().Model;
        IApplicationModel peerModel = BuildModel(gateway, "peer", "api");
        using var peerKey = new GatewayTrustKey(ECDsa.Create(ECCurve.NamedCurves.nistP256));
        ApplicationExportDocument export = ApplicationExportDocument.Create(peerModel, "1", trustKey: peerKey.PublicJwk);

        try
        {
            // Act
            await gateway.AddTrustedIssuerAsync(
                model,
                "peer",
                export,
                ["rezolvr.add-a-record", "identityhub.add-client"],
                cancellation.Token);

            // Assert
            (ResourceProviderConnection? connection, string owner, TrustedIssuer issuer) = store.Adds.ShouldHaveSingleItem();
            connection.ShouldBeNull();
            owner.ShouldBe(model.Owner);
            issuer.AllowedCommandKinds.ShouldBe(["identityhub.add-client", "rezolvr.add-a-record"]);
            gateway.GetTrustedIssuers(model.Name).Single(candidate => candidate.Issuer == "peer")
                .AllowedCommandKinds.ShouldBe(["identityhub.add-client", "rezolvr.add-a-record"]);
        }
        finally
        {
            await ((IApplicationGateway)gateway).StopAsync(CancellationToken.None);
            DeleteTestDirectory(root);
        }
    }

    [Fact(DisplayName = "Cohesion Test [ApplicationModel.Gateway] - Trust add: Local without a trust store writes the local trusted-issuers file")]
    public async Task AddTrustedIssuerAsync_WithoutTrustStoreInLocal_ShouldWriteLocalFile()
    {
        // Arrange
        string root = CreateTestDirectory();
        using var cancellation = new CancellationTokenSource(_testTimeout);
        var gateway = new TestGateway(
            new InMemoryResourceStateManager(),
            [new InputHistoryController()],
            options: new ApplicationGatewayOptions { ExportDirectory = root },
            name: "test-gateway");
        IApplicationModel model = BuildModel(gateway, "appa", "api");
        IApplicationModel peerModel = BuildModel(gateway, "peer", "api");
        using var peerKey = new GatewayTrustKey(ECDsa.Create(ECCurve.NamedCurves.nistP256));
        ApplicationExportDocument export = ApplicationExportDocument.Create(peerModel, "1", trustKey: peerKey.PublicJwk);

        try
        {
            // Act
            await gateway.AddTrustedIssuerAsync(model, "peer", export, cancellation.Token);

            // Assert
            string path = Path.Combine(root, "appa", "trust", "trusted-issuers.json");
            File.Exists(path).ShouldBeTrue();
            TrustedIssuerDocument.Parse(await File.ReadAllBytesAsync(path, cancellation.Token))
                .Select(issuer => issuer.Issuer).ShouldBe(["appa", "peer"]);
            gateway.GetTrustedIssuers(model.Name).Count.ShouldBe(2);
        }
        finally
        {
            await ((IApplicationGateway)gateway).StopAsync(CancellationToken.None);
            DeleteTestDirectory(root);
        }
    }

    [Theory(DisplayName = "Cohesion Test [ApplicationModel.Gateway] - Trust add: Deployed environments refuse missing or plaintext own stores")]
    [InlineData(AppEnvironment.Keys.Development, false)]
    [InlineData(AppEnvironment.Keys.Development, true)]
    [InlineData(AppEnvironment.Keys.Production, false)]
    [InlineData(AppEnvironment.Keys.Production, true)]
    public async Task AddTrustedIssuerAsync_WithoutSecureOwnStore_ShouldRefuseLocalFallback(string environment, bool loopback)
    {
        // Arrange
        string root = CreateTestDirectory();
        using var cancellation = new CancellationTokenSource(_testTimeout);
        var gateway = new TestGateway(
            new InMemoryResourceStateManager(),
            [new InputHistoryController()],
            options: new ApplicationGatewayOptions { ExportDirectory = root },
            name: "test-gateway",
            trustStoreEndpointResolver: (_, _) => loopback ? new Uri("http://localhost:8443/") : null);
        IApplicationBuilder builder = Application.CreateBuilder("appa", ["--environment", environment]).UseGateway(gateway);
        if (loopback)
        {
            builder.AddResource(CreateTrustStoreManifest());
            builder.Providers.TrustStore = new ResourceProviderBinding<ITrustedIssuerStore>(
                "secrets",
                new RecordingTrustedIssuerStore(resourceKind: "KeyStore"));
        }
        else
        {
            builder.AddResource(new TestResource("api"));
        }
        IApplicationModel model = builder.Build().Model;
        IApplicationBuilder peerBuilder = Application.CreateBuilder("peer", ["--environment=Local"]).UseGateway(gateway);
        peerBuilder.AddResource(new TestResource("api"));
        using var peerKey = new GatewayTrustKey(ECDsa.Create(ECCurve.NamedCurves.nistP256));
        ApplicationExportDocument export = ApplicationExportDocument.Create(peerBuilder.Build().Model, "1", trustKey: peerKey.PublicJwk);

        try
        {
            // Act
            InvalidOperationException error = await Should.ThrowAsync<InvalidOperationException>(
                () => gateway.AddTrustedIssuerAsync(model, "peer", export, cancellation.Token));

            // Assert
            error.Message.ShouldContain(loopback ? "non-TLS" : "Local-only");
            File.Exists(Path.Combine(root, "appa", "trust", "trusted-issuers.json")).ShouldBeFalse();
            gateway.GetTrustedIssuers(model.Name).ShouldNotContain(issuer => issuer.Issuer == "peer");
        }
        finally
        {
            await ((IApplicationGateway)gateway).StopAsync(CancellationToken.None);
            DeleteTestDirectory(root);
        }
    }

    [Fact(DisplayName = "Cohesion Test [ApplicationModel.Gateway] - TrustedIssuer: Rejects a JWK without the required ES256 public-key shape")]
    public void Constructor_InvalidPublicJwk_ShouldThrow()
    {
        // Arrange
        using JsonDocument document = JsonDocument.Parse("{}");

        // Act
        Action create = () => _ = new TrustedIssuer("peer", document.RootElement);

        // Assert
        Should.Throw<ArgumentException>(create).Message.ShouldContain("kty");
    }

    [Fact(DisplayName = "Cohesion Test [ApplicationModel.Gateway] - TrustedIssuer: Rejects off-curve P-256 coordinates")]
    public void Constructor_OffCurveCoordinates_ShouldThrow()
    {
        // Arrange
        string zeroCoordinate = Base64Url.EncodeToString(new byte[32]);
        using JsonDocument document = JsonDocument.Parse(
            $$"""{"kty":"EC","crv":"P-256","x":"{{zeroCoordinate}}","y":"{{zeroCoordinate}}","kid":"invalid","alg":"ES256","use":"sig"}""");

        // Act
        Action create = () => _ = new TrustedIssuer("peer", document.RootElement);

        // Assert
        Should.Throw<ArgumentException>(create).Message.ShouldContain("valid P-256 point");
    }

    private static IApplicationModel BuildModel(
        IApplicationGateway gateway,
        string application,
        string resource)
    {
        IApplicationBuilder builder = Application
            .CreateBuilder(ApplicationName.Parse(application), ["--environment", AppEnvironment.Keys.Local])
            .UseGateway(gateway);
        builder.AddResource(new TestResource(resource));
        return builder.Build().Model;
    }

    // A neutral store kind: the gateway reaches a trust store only through its registration.
    private static ResourceManifest CreateTrustStoreManifest() => new()
    {
        Name = "secrets",
        Application = "appa",
        Kind = "KeyStore",
        ApplicationModel = "Assimalign.Cohesion.Test.ApplicationModel",
        Artifact = new ResourceManifestArtifact
        {
            Assembly = "test.dll",
            AppHost = "test",
        },
        Endpoints =
        [
            new ResourceManifestEndpoint
            {
                Name = "api",
                Scheme = "https",
                Protocol = "tcp",
                ContainerPort = 8443,
            },
        ],
        ControlPlane = new ResourceManifestControlPlane
        {
            Endpoint = "api",
            Path = "/cohesion/v1",
        },
        Lifecycle = new ResourceManifestLifecycle
        {
            Workload = WorkloadKind.Deployment,
            Replicas = 1,
            RestartPolicy = "Never",
        },
    };

    private static ExternalResourceDeclaration CreateExternalDeclaration() => new(
        "external-api",
        "peer",
        ["https"],
        optional: false,
        new ResourceManifest
        {
            Name = "external-api",
            Application = "peer",
            Kind = "test",
            ApplicationModel = "Assimalign.Cohesion.Test.ApplicationModel",
            Artifact = new ResourceManifestArtifact
            {
                Assembly = "peer.dll",
                AppHost = "peer",
            },
            Endpoints =
            [
                new ResourceManifestEndpoint
                {
                    Name = "https",
                    Scheme = "https",
                    Protocol = "tcp",
                    ContainerPort = 443,
                },
            ],
            ControlPlane = new ResourceManifestControlPlane
            {
                Endpoint = "https",
                Path = "/cohesion/v1",
            },
            Lifecycle = new ResourceManifestLifecycle
            {
                Workload = WorkloadKind.Deployment,
                Replicas = 1,
                RestartPolicy = "Never",
            },
        });

    private static void AssertToken(
        JsonWebToken token,
        string audience,
        TimeSpan maximumLifetime)
    {
        token.Algorithm.ShouldBe("ES256");
        token.Header.KeyId.ShouldNotBeNullOrWhiteSpace();
        token.Audiences.ShouldBe([audience]);
        token.IssuedAt.HasValue.ShouldBeTrue();
        token.ExpiresAt.HasValue.ShouldBeTrue();

        TimeSpan lifetime = token.ExpiresAt!.Value - token.IssuedAt!.Value;
        lifetime.ShouldBeGreaterThan(TimeSpan.Zero);
        lifetime.ShouldBeLessThanOrEqualTo(maximumLifetime);
    }

    private static void AssertPublicJwk(JsonElement publicJwk, string expectedKeyId)
    {
        publicJwk.ValueKind.ShouldBe(JsonValueKind.Object);
        publicJwk.GetProperty("kty").GetString().ShouldBe("EC");
        publicJwk.GetProperty("crv").GetString().ShouldBe("P-256");
        publicJwk.GetProperty("alg").GetString().ShouldBe("ES256");
        publicJwk.GetProperty("use").GetString().ShouldBe("sig");
        publicJwk.GetProperty("kid").GetString().ShouldBe(expectedKeyId);
        publicJwk.TryGetProperty("d", out _).ShouldBeFalse();
    }

    private static bool VerifyWithPublicJwk(JsonWebToken token, JsonElement publicJwk)
    {
        byte[] x = Base64Url.DecodeFromChars(
            publicJwk.GetProperty("x").GetString().ShouldNotBeNull());
        byte[] y = Base64Url.DecodeFromChars(
            publicJwk.GetProperty("y").GetString().ShouldNotBeNull());
        try
        {
            using ECDsa publicKey = ECDsa.Create();
            publicKey.ImportParameters(new ECParameters
            {
                Curve = ECCurve.NamedCurves.nistP256,
                Q = new ECPoint
                {
                    X = x,
                    Y = y,
                },
            });

            IJsonWebTokenSignatureVerifier verifier = EcdsaJsonWebTokenSignatureVerifier.Create(
                publicKey,
                publicJwk.GetProperty("kid").GetString());
            string algorithm = token.Algorithm.ShouldNotBeNull();
            byte[] signingInput = Encoding.ASCII.GetBytes(token.SigningInput.ShouldNotBeNull());
            byte[] signature = Base64Url.DecodeFromChars(
                token.Parts.ShouldNotBeNull().Signature);
            return verifier.CanVerify(algorithm, token.Header.KeyId) &&
                verifier.Verify(algorithm, signingInput, signature);
        }
        finally
        {
            CryptographicOperations.ZeroMemory(x);
            CryptographicOperations.ZeroMemory(y);
        }
    }

    private static string CreateTestDirectory()
    {
        string path = Path.Combine(
            Path.GetTempPath(),
            "cohesion-gateway-trust-tests",
            Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(path);
        return path;
    }

    private static void DeleteTestDirectory(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, recursive: true);
        }
    }

    private sealed class InputHistoryController : IApplicationResourceController
    {
        public List<ResourceInputs> Inputs { get; } = new();

        public bool CanRealize(ResourcePlan plan, out string? reason)
        {
            reason = null;
            return true;
        }

        public Task ReconcileAsync(
            IResourceControlContext context,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Inputs.Add(context.Inputs.ShouldNotBeNull());
            context.State.SetState(context.Resource.Id, ResourceLifecycle.Running);
            return Task.CompletedTask;
        }

        public Task StopAsync(
            IResourceControlContext context,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            context.State.SetState(context.Resource.Id, ResourceLifecycle.Stopped);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(
            IResourceControlContext context,
            CancellationToken cancellationToken = default) => StopAsync(context, cancellationToken);
    }

    private sealed class TrustStoreController : IApplicationResourceController
    {
        public bool CanRealize(ResourcePlan plan, out string? reason)
        {
            reason = null;
            return true;
        }

        public Task ReconcileAsync(
            IResourceControlContext context,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            context.State.SetState(
                context.Resource.Id,
                ResourceLifecycle.Running,
                observedEndpoints: [new ResourceEndpoint("api", "https", 8443, Host: "127.0.0.1")]);
            return Task.CompletedTask;
        }

        public Task StopAsync(
            IResourceControlContext context,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            context.State.SetState(context.Resource.Id, ResourceLifecycle.Stopped);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(
            IResourceControlContext context,
            CancellationToken cancellationToken = default) => StopAsync(context, cancellationToken);
    }

    private sealed class TrustAwareExternalResolver : IExternalResourceResolver
    {
        private readonly TestGateway _gateway;

        /// <summary>
        /// Initializes a new instance of the <see cref="TrustAwareExternalResolver"/> class.
        /// </summary>
        /// <param name="gateway">The gateway whose trusted issuers are inspected during resolution.</param>
        public TrustAwareExternalResolver(TestGateway gateway)
        {
            _gateway = gateway;
        }

        public bool PeerWasTrusted { get; private set; }

        public ValueTask<ExternalResourceResolution> ResolveAsync(
            ExternalResourceResolutionContext context,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            IReadOnlyList<TrustedIssuer> issuers = _gateway.GetTrustedIssuers("appa");
            for (int index = 0; index < issuers.Count; index++)
            {
                if (string.Equals(issuers[index].Issuer, "peer", StringComparison.Ordinal))
                {
                    PeerWasTrusted = true;
                    break;
                }
            }

            return ValueTask.FromResult(
                PeerWasTrusted
                    ? new ExternalResourceResolution(
                        resolved: true,
                        [new ResourceEndpoint("https", "https", 443, IsPublic: true, Host: "peer.test")],
                        context.Declaration.ManifestHash,
                        ApplicationExportDocument.CurrentSchemaVersion,
                        "Resolved after loading the peer trust grant.")
                    : ExternalResourceResolution.Unresolved(
                        "The peer trust grant was not loaded before external resolution."));
        }
    }

    private sealed class RecordingTrustKeyRepository : IGatewayTrustKeyRepository
    {
        public int LoadCount { get; private set; }

        public int RotateCount { get; private set; }

        public Task<ECDsa> LoadOrCreateAsync(
            ApplicationName application,
            ResourceName gateway,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            LoadCount++;
            return Task.FromResult(ECDsa.Create(ECCurve.NamedCurves.nistP256));
        }

        public Task<ECDsa> RotateAsync(
            ApplicationName application,
            ResourceName gateway,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            RotateCount++;
            return Task.FromResult(ECDsa.Create(ECCurve.NamedCurves.nistP256));
        }
    }
}
```

### Walkthrough

- **Covered behavior** — Trust allow kinds round-trip with backward-compatible unrestricted documents.
- **Covered behavior** — LoadOrCreateAsync: Should persist a distinct P-256 trust key per application.
- **Covered behavior** — ReconcileAsync: Should rotate an audience-bound ES256 bootstrap token on every pass.
- **Covered behavior** — IssueDeveloperTokenAsync: Should issue an ES256 cohesion-export token bounded to eight hours.
- **Covered behavior** — Trust repository: Platform override owns load and rotation.
- **Covered behavior** — TrustedIssuers: Loads peer keys from the registered trust store.
- **Covered behavior** — TrustedIssuers: The trust store precedes an independent remote on the first production pass.
- **Covered behavior** — TrustedIssuers: An unavailable trust store is fatal only outside Local.
- **Covered behavior** — TrustedIssuers: Only Local retains a fallback after store not-found.
- **Covered behavior** — Trust add: Platform endpoint seam supports a production one-shot grant.
- **Covered behavior** — Trust add: A restricted grant reaches the trust store with its allowed command kinds.
- **Covered behavior** — Trust add: Local without a trust store writes the local trusted-issuers file.
- **Covered behavior** — Trust add: Deployed environments refuse missing or plaintext own stores.
- **Covered behavior** — TrustedIssuer: Rejects a JWK without the required ES256 public-key shape.
- **Covered behavior** — TrustedIssuer: Rejects off-curve P-256 coordinates.

## Sources

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway/tests/ApplicationGatewayResourceExtensionsTests.cs`.
- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway/tests/Assimalign.Cohesion.ApplicationModel.Gateway.Tests.csproj`.
- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway/tests/GatewayCredentialIssuerTests.cs`.
- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway/tests/GatewayTrustTests.cs`.
