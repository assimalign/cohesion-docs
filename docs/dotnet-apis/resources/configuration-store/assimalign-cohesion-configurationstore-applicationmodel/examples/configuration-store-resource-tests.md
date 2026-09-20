# Configuration Store Resource Tests

This example exercises `Assimalign.Cohesion.ConfigurationStore.ApplicationModel` through its co-located test source.

> **Status:** Partial.

The example reproduces
`cohesion/resources/ConfigurationStore/Assimalign.Cohesion.ConfigurationStore.ApplicationModel/tests/ConfigurationStoreResourceTests.cs`
. It retains the test class and assertions so the setup, operation, and expected outcome stay
together. `Use` it in the source project’s test context, with its test dependencies and supporting
test objects.

## Behavior exercised

- **Case 1** — Constructor: wraps an immutable manifest snapshot with typed options.
- **Case 2** — `AddConfigurationStore`: composes the manifest with typed options.
- **Case 3** — `AddConfigurationStore`: rejects a null manifest.

## Source example

```csharp
using System;
using Shouldly;
using Xunit;
using Assimalign.Cohesion.ApplicationModel;

namespace Assimalign.Cohesion.ConfigurationStore.ApplicationModel.Tests;

public sealed class ConfigurationStoreResourceTests
{
    [Fact(DisplayName = "Cohesion Test [ConfigurationStore.ApplicationModel] - Constructor: wraps an immutable manifest snapshot with typed options")]
    public void Constructor_WithManifestAndOptions_ShouldExposeSnapshotAndTypedOptions()
    {
        // Arrange
        ResourceManifest manifest = ConfigurationStoreManifestFactory.Create();
        var options = new ConfigurationStoreResourceOptions { Replicas = 2 };
        options.Storage.Size = "20Gi";

        // Act
        var resource = new ConfigurationStoreResource(manifest, options);

        // Assert
        resource.Manifest.ShouldNotBeSameAs(manifest);
        resource.Name.ShouldBe(manifest.Name);
        resource.Artifact.ShouldBe(manifest.Artifact.Assembly);
        resource.Endpoints.Count.ShouldBe(manifest.Endpoints.Count);
        resource.Mounts.Count.ShouldBe(manifest.Mounts.Count);
        resource.Options.ShouldBeSameAs(options);
        resource.PlannerName.ShouldBe("ConfigurationStore planner");
    }

    [Fact(DisplayName = "Cohesion Test [ConfigurationStore.ApplicationModel] - AddConfigurationStore: composes the manifest with typed options")]
    public void AddConfigurationStore_WithManifestAndOptions_ShouldReturnTypedResourceDescriptor()
    {
        // Arrange
        IApplicationBuilder builder = Application.CreateBuilder();
        ResourceManifest manifest = ConfigurationStoreManifestFactory.Create();
        var expectedOptions = new ConfigurationStoreResourceOptions { Replicas = 2 };
        expectedOptions.Storage.Size = "25Gi";

        // Act
        IApplicationResourceDescriptor descriptor = builder.AddConfigurationStore(
            manifest,
            expectedOptions);

        // Assert
        ConfigurationStoreResource resource = descriptor.Resource
            .ShouldBeOfType<ConfigurationStoreResource>();
        ConfigurationStoreResourceOptions options = resource.Options
            .ShouldBeOfType<ConfigurationStoreResourceOptions>();
        options.ShouldBeSameAs(expectedOptions);
        options.Replicas.ShouldBe(2);
        options.Storage.Size.ShouldBe("25Gi");
    }

    [Fact(DisplayName = "Cohesion Test [ConfigurationStore.ApplicationModel] - AddConfigurationStore: rejects a null manifest")]
    public void AddConfigurationStore_WithNullManifest_ShouldThrow()
    {
        // Arrange
        IApplicationBuilder builder = Application.CreateBuilder();

        // Act
        ArgumentNullException error = Should.Throw<ArgumentNullException>(
            () => builder.AddConfigurationStore(null!));

        // Assert
        error.ParamName.ShouldBe("manifest");
    }
}
```

[All examples](index.md) · [Assembly overview](../index.md)

## Sources

- **Primary source** — `cohesion/resources/ConfigurationStore/Assimalign.Cohesion.ConfigurationStore.ApplicationModel/tests/ConfigurationStoreResourceTests.cs`.
- **Source** — `cohesion/resources/ConfigurationStore/Assimalign.Cohesion.ConfigurationStore.ApplicationModel/tests/Assimalign.Cohesion.ConfigurationStore.ApplicationModel.Tests.csproj`.
