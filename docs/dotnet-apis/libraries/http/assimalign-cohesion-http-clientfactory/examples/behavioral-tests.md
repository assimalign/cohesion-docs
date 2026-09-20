# Example: Http Client Factory Component Integration Tests

Exercise Http Client Factory Component Integration behavior using the original repository tests.

[Examples](index.md) · [Assembly overview](../index.md)

## Test-project context

This is the complete `HttpClientFactoryComponentIntegrationTests.cs` listing from the package test
project. Keep it in that project when running it: the project supplies its package references,
generated sources, and any shared fixtures. The using block below makes the test-framework import
explicit where the original project supplies it globally.

## Code

```csharp
using System.Net.Http;
using System;
using Shouldly;
using Xunit;
using Assimalign.Cohesion.DependencyInjection;
using Assimalign.Cohesion.Http;

namespace Assimalign.Cohesion.Http.ClientFactory.Tests;

/// <summary>
/// Behavioral tests for the component-integration projection onto
/// <see cref="IServiceProviderBuilder"/>.
/// </summary>
public class HttpClientFactoryComponentIntegrationTests
{
    [Fact(DisplayName = "Cohesion Test [Http.ClientFactory] - AddHttpClientFactory: resolves a configured named-client factory")]
    public void AddHttpClientFactory_WithNamedClient_ShouldResolveConfiguredFactory()
    {
        // Arrange
        var builder = new ServiceProviderBuilder();
        var expectedBaseAddress = new Uri("https://example.test/");

        // Act
        IServiceProvider provider = builder
            .AddHttpClientFactory(clients => clients.AddClient(
                "test",
                options => options.BaseAddress = expectedBaseAddress))
            .Build();

        try
        {
            IHttpClientFactory factory = provider.GetRequiredService<IHttpClientFactory>();
            using HttpClient client = factory.Create("test");

            // Assert
            factory.ShouldNotBeNull();
            client.ShouldNotBeNull();
            client.BaseAddress.ShouldBe(expectedBaseAddress);
        }
        finally
        {
            ((IDisposable)provider).Dispose();
        }
    }

    [Fact(DisplayName = "Cohesion Test [Http.ClientFactory] - AddHttpClientFactory: provider disposal disposes the resolved factory")]
    public void AddHttpClientFactory_WhenProviderDisposed_ShouldDisposeResolvedFactory()
    {
        // Arrange
        var builder = new ServiceProviderBuilder();
        IServiceProvider provider = builder
            .AddHttpClientFactory(clients => clients.AddClient("test"))
            .Build();
        IHttpClientFactory factory = provider.GetRequiredService<IHttpClientFactory>();

        // Act
        ((IDisposable)provider).Dispose();

        // Assert — Create is the strongest public disposal signal exposed by the factory.
        Should.Throw<ObjectDisposedException>(() => factory.Create("test"));
    }
}
```

## Walkthrough

- **Covered behavior** — AddHttpClientFactory: resolves a configured named-client factory.
- **Covered behavior** — AddHttpClientFactory: provider disposal disposes the resolved factory.

The assertions define the expected result or failure boundary. Test-only doubles supply controlled
inputs; they are not additional shipped APIs.

## Sources

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ClientFactory/tests/HttpClientFactoryComponentIntegrationTests.cs`.
- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ClientFactory/tests/Assimalign.Cohesion.Http.ClientFactory.Tests.csproj`.
