# Database Resource Tests

This example exercises `Assimalign.Cohesion.Database.ApplicationModel` through its co-located test source.

> **Status:** Partial.

The example reproduces
`cohesion/resources/Database/Assimalign.Cohesion.Database.ApplicationModel/tests/DatabaseResourceTests.cs`
. It retains the test class and assertions so the setup, operation, and expected outcome stay
together. `Use` it in the source project’s test context, with its test dependencies and supporting
test objects.

## Behavior exercised

- **Case 1** — Constructor: wraps an immutable manifest snapshot with typed options.
- **Case 2** — `AddDatabase`: composes the manifest with replica and storage options.
- **Case 3** — `AddDatabase`: rejects a null manifest.

## Source example

```csharp
using System;
using Shouldly;
using Xunit;
using Assimalign.Cohesion.ApplicationModel;

namespace Assimalign.Cohesion.Database.ApplicationModel.Tests;

public class DatabaseResourceTests
{
    [Fact(DisplayName = "Cohesion Test [Database.ApplicationModel] - Constructor: wraps an immutable manifest snapshot with typed options")]
    public void Constructor_WithManifestAndOptions_ShouldExposeSnapshotAndTypedOptions()
    {
        // Arrange
        ResourceManifest manifest = DatabaseManifestFactory.Create();
        var options = new DatabaseResourceOptions { Replicas = 1 };
        options.Storage.Size = "20Gi";

        // Act
        var resource = new DatabaseResource(manifest, options);

        // Assert
        resource.Manifest.ShouldNotBeSameAs(manifest);
        resource.Name.ShouldBe(manifest.Name);
        resource.Artifact.ShouldBe(manifest.Artifact.Assembly);
        resource.Endpoints.Count.ShouldBe(manifest.Endpoints.Count);
        resource.Mounts.Count.ShouldBe(manifest.Mounts.Count);
        resource.Options.ShouldBeSameAs(options);
        resource.PlannerName.ShouldBe("Database planner");
    }

    [Fact(DisplayName = "Cohesion Test [Database.ApplicationModel] - AddDatabase: composes the manifest with replica and storage options")]
    public void AddDatabase_WithManifestAndOptions_ShouldReturnTypedResourceDescriptor()
    {
        // Arrange
        IApplicationBuilder builder = Application.CreateBuilder();
        ResourceManifest manifest = DatabaseManifestFactory.Create();
        var expectedOptions = new DatabaseResourceOptions { Replicas = 1 };
        expectedOptions.Storage.Size = "25Gi";

        // Act
        IApplicationResourceDescriptor descriptor = builder.AddDatabase(manifest, expectedOptions);

        // Assert
        DatabaseResource resource = descriptor.Resource.ShouldBeOfType<DatabaseResource>();
        DatabaseResourceOptions options = resource.Options.ShouldBeOfType<DatabaseResourceOptions>();
        options.ShouldBeSameAs(expectedOptions);
        options.Replicas.ShouldBe(1);
        options.Storage.Size.ShouldBe("25Gi");
    }

    [Fact(DisplayName = "Cohesion Test [Database.ApplicationModel] - AddDatabase: rejects a null manifest")]
    public void AddDatabase_WithNullManifest_ShouldThrow()
    {
        // Arrange
        IApplicationBuilder builder = Application.CreateBuilder();

        // Act
        ArgumentNullException error = Should.Throw<ArgumentNullException>(
            () => builder.AddDatabase(null!));

        // Assert
        error.ParamName.ShouldBe("manifest");
    }
}
```

[All examples](index.md) · [Assembly overview](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.ApplicationModel/tests/DatabaseResourceTests.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.ApplicationModel/tests/Assimalign.Cohesion.Database.ApplicationModel.Tests.csproj`.
