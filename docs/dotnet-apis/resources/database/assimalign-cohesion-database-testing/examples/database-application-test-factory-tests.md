# Database Application Test Factory Tests

This example exercises `Assimalign.Cohesion.Database.Testing` through its co-located test source.

> **Status:** Partial.

The example reproduces
`cohesion/resources/Database/Assimalign.Cohesion.Database.Testing/tests/DatabaseApplicationTestFactoryTests.cs`
. It retains the test class and assertions so the setup, operation, and expected outcome stay
together. `Use` it in the source project’s test context, with its test dependencies and supporting
test objects.

## Behavior exercised

- **Case 1** — FromProgram: drives the real entry point under an isolated resource context.
- **Case 2** — FromProgram: rejects a marker that is not its assembly entry-point type.
- **Case 3** — FromProgram: pre-cancelled startup does not launch the resource program.

## Source example

```csharp
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Shouldly;
using Xunit;
using Assimalign.Cohesion.Database.Testing.TestHost;

namespace Assimalign.Cohesion.Database.Testing.Tests;

/// <summary>
/// Entry-point, ambient-context, and lifecycle coverage for
/// <see cref="DatabaseApplicationTestFactory"/>.
/// </summary>
public class DatabaseApplicationTestFactoryTests
{
    private static readonly TimeSpan TestTimeout = TimeSpan.FromSeconds(30);

    [Fact(DisplayName = "Cohesion Test [Database.Testing] - FromProgram: drives the real entry point under an isolated resource context")]
    public async Task FromProgram_WithDefaultContext_ShouldDriveEntryPointAndControlPlane()
    {
        // Arrange
        using var cancellation = new CancellationTokenSource(TestTimeout);
        await using DatabaseApplicationTestFactory factory =
            DatabaseApplicationTestFactory.FromProgram<Program>();

        // Act
        await factory.StartAsync(cancellation.Token);

        // Assert
        factory.IsStarted.ShouldBeTrue();
        factory.ResourceContext.ApplicationName.ShouldBe("tests");
        factory.ResourceContext.GatewayName.ShouldBe("inprocess");
        factory.ResourceContext.Endpoints.Keys.ShouldContain("db");
        factory.ResourceContext.Endpoints.Keys.ShouldContain("admin");
        factory.ResourceContext.Mounts["data"].Path.ShouldNotBeNull();
        factory.ResourceContext.BootstrapCredential.IsEmpty.ShouldBeFalse();
        factory.ResourceContext.ApplicationTrustKey.IsEmpty.ShouldBeFalse();
        Encoding.UTF8.GetString(factory.ResourceContext.BootstrapCredential.Span).Split('.').Length.ShouldBe(3);

        using var client = new HttpClient
        {
            BaseAddress = factory.ResourceContext.Endpoints["admin"],
        };
        using HttpResponseMessage response = await client.GetAsync("/healthz", cancellation.Token);
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        using HttpResponseMessage anonymous = await client.GetAsync(
            "/cohesion/v1/endpoints",
            cancellation.Token);
        anonymous.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);

        using var authorizedRequest = new HttpRequestMessage(
            HttpMethod.Get,
            "/cohesion/v1/endpoints");
        authorizedRequest.Headers.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            Encoding.UTF8.GetString(factory.ResourceContext.BootstrapCredential.Span));
        using HttpResponseMessage authorized = await client.SendAsync(
            authorizedRequest,
            cancellation.Token);
        authorized.StatusCode.ShouldBe(HttpStatusCode.OK);

        await factory.StopAsync(cancellation.Token);
        factory.IsStarted.ShouldBeFalse();
    }

    [Fact(DisplayName = "Cohesion Test [Database.Testing] - FromProgram: rejects a marker that is not its assembly entry-point type")]
    public void FromProgram_WithNonEntryPointMarker_ShouldRejectMarker()
    {
        // Act
        InvalidOperationException exception = Should.Throw<InvalidOperationException>(
            () => DatabaseApplicationTestFactory.FromProgram<DatabaseApplicationTestFactoryTests>());

        // Assert
        exception.Message.ShouldContain("not the entry-point Program", Case.Insensitive);
    }

    [Fact(DisplayName = "Cohesion Test [Database.Testing] - FromProgram: pre-cancelled startup does not launch the resource program")]
    public async Task StartAsync_WithPreCancelledToken_ShouldNotLaunchProgram()
    {
        await using DatabaseApplicationTestFactory factory =
            DatabaseApplicationTestFactory.FromProgram<Program>();
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();

        await Should.ThrowAsync<OperationCanceledException>(
            () => factory.StartAsync(cancellation.Token));

        factory.IsStarted.ShouldBeFalse();
        await factory.StopAsync(CancellationToken.None);
    }
}
```

[All examples](index.md) · [Assembly overview](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Testing/tests/DatabaseApplicationTestFactoryTests.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Testing/tests/Assimalign.Cohesion.Database.Testing.TestHost.csproj`.
