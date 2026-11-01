# Configuration Store Resource Registration Tests

This example exercises `Assimalign.Cohesion.ConfigurationStore.Hosting.ApplicationModel` through its co-located test source.

> **Status:** Implemented.

The example reproduces
`cohesion/resources/ConfigurationStore/Assimalign.Cohesion.ConfigurationStore.Hosting.ApplicationModel/tests/ConfigurationStoreResourceRegistrationTests.cs`. It
retains the test class and assertions so the setup, operation, and expected outcome stay together.
Use it in the source project’s test context, with its test dependencies and supporting test objects.

## Behavior exercised

- **Case 1** — An unmanaged resource registers nothing.
- **Case 2** — A managed resource registers one lifecycle service.
- **Case 3** — Building the application creates the resource with the actions the application added.
- **Case 4** — A platform the resource has no client for fails the registration.
- **Case 5** — A default name or missing registrations are refused.

## Source example

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Shouldly;
using Xunit;

using Assimalign.Cohesion.ApplicationModel;
using Assimalign.Cohesion.ApplicationModel.Hosting;
using Assimalign.Cohesion.DependencyInjection;
using Assimalign.Cohesion.Hosting;

namespace Assimalign.Cohesion.ConfigurationStore.Hosting.ApplicationModel.Tests;

/// <summary>
/// <c>builder.Services.AddResource(...)</c> is a registration: a managed resource is one lifecycle
/// service the application's container creates when the application is built, and an unmanaged one is
/// nothing at all.
/// </summary>
public sealed class ConfigurationStoreResourceRegistrationTests
{
    private const string DisplayPrefix = "Cohesion Test [ConfigurationStore.Hosting.ApplicationModel] - Registration: ";

    [Fact(DisplayName = DisplayPrefix + "An unmanaged resource registers nothing")]
    public void AddResource_WithoutPlatform_ShouldRegisterNothing()
    {
        // Arrange
        ConfigurationStoreApplicationBuilder builder = ConfigurationStoreApplication.CreateBuilder([]);
        int registrations = builder.Services.Container.Count();

        // Act
        builder.Services.AddResource("settings", resource => resource.Platform = null);

        // Assert
        builder.Services.Container.Count().ShouldBe(registrations);
    }

    [Fact(DisplayName = DisplayPrefix + "A managed resource registers one lifecycle service")]
    public void AddResource_WithPlatform_ShouldRegisterOneLifecycleService()
    {
        // Arrange
        ConfigurationStoreApplicationBuilder builder = ConfigurationStoreApplication.CreateBuilder([]);

        // Act
        builder.Services.AddResource("settings", resource =>
        {
            resource.Platform = "test";
            resource.UseGatewayClient(new TestGatewayClient());
        });

        // Assert
        ServiceDescriptor registration = builder.Services.Container.Single(descriptor => descriptor.ServiceType == typeof(IHostService));
        registration.Lifetime.ShouldBe(ServiceLifetime.Singleton);
    }

    [Fact(DisplayName = DisplayPrefix + "Building the application creates the resource with the actions the application added")]
    public async Task Build_WithManagedResource_ShouldCreateTheInstanceServiceFromTheContainer()
    {
        // Arrange
        ConfigurationStoreApplicationBuilder builder = ConfigurationStoreApplication.CreateBuilder([]);
        builder.Services.AddSingleton(new Greeting("from the application"));
        builder.Services.AddResource("settings", resource =>
        {
            resource.Platform = "test";
            resource.UseGatewayClient(new TestGatewayClient());
            resource.AddAction(services => new GreetingAction(services.GetRequiredService<Greeting>().Text), GreetingJsonContext.Default);
        });

        // Act
        await using ConfigurationStoreApplication application = builder.Build();

        // Assert
        IApplicationInstanceService service = application.Context.ServiceProvider
            .GetRequiredService<IEnumerable<IHostService>>()
            .OfType<IApplicationInstanceService>()
            .ShouldHaveSingleItem();
        IApplicationInstance instance = service.ControlPlane.Instance;
        instance.Provider.ShouldBe((ProviderName)"ConfigurationStore");
        instance.Name.ShouldBe((ResourceName)"settings");
        instance.HealthContributors.ShouldBeEmpty();
        instance.Actions.ShouldHaveSingleItem().Name.ShouldBe((ActionName)"greeting");
    }

    [Fact(DisplayName = DisplayPrefix + "A platform the resource has no client for fails the registration")]
    public void AddResource_WithPlatformWithoutClient_ShouldThrow()
    {
        // Arrange
        ConfigurationStoreApplicationBuilder builder = ConfigurationStoreApplication.CreateBuilder([]);

        // Act
        Action act = () => builder.Services.AddResource("settings", resource => resource.Platform = "test");

        // Assert
        act.ShouldThrow<InvalidOperationException>().Message.ShouldContain("'test'", Case.Sensitive);
    }

    [Fact(DisplayName = DisplayPrefix + "A default name or missing registrations are refused")]
    public void AddResource_WithDefaultNameOrNullServices_ShouldThrow()
    {
        // Arrange
        ConfigurationStoreApplicationBuilder builder = ConfigurationStoreApplication.CreateBuilder([]);

        // Act & Assert
        Should.Throw<ArgumentException>(() => builder.Services.AddResource(default));
        Should.Throw<ArgumentNullException>(() => ((IServiceProviderBuilder)null!).AddResource("settings"));
    }
}
```

[Examples](index.md) · [Assembly overview](../index.md)

## Sources

- **Primary source** — `cohesion/resources/ConfigurationStore/Assimalign.Cohesion.ConfigurationStore.Hosting.ApplicationModel/tests/ConfigurationStoreResourceRegistrationTests.cs`.
- **Source** — `cohesion/resources/ConfigurationStore/Assimalign.Cohesion.ConfigurationStore.Hosting.ApplicationModel/tests/Assimalign.Cohesion.ConfigurationStore.Hosting.ApplicationModel.Tests.csproj`.
