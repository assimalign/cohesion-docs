# Identity Hub Resource Tests

This example exercises `Assimalign.Cohesion.IdentityHub.ApplicationModel` through its co-located test source.

> **Status:** Partial.

The example reproduces
`cohesion/resources/IdentityHub/Assimalign.Cohesion.IdentityHub.ApplicationModel/tests/IdentityHubResourceTests.cs`
. It retains the test class and assertions so the setup, operation, and expected outcome stay
together. `Use` it in the source project’s test context, with its test dependencies and supporting
test objects.

## Behavior exercised

- **Case 1** — `AddIdentityHub`: composes the manifest with typed options.
- **Case 2** — `AddIdentityHub`: rejects a null manifest.

## Source example

```csharp
using System;
using Shouldly;
using Xunit;
using Assimalign.Cohesion.ApplicationModel;

namespace Assimalign.Cohesion.IdentityHub.ApplicationModel.Tests;

public sealed class IdentityHubResourceTests
{
    [Fact(DisplayName = "Cohesion Test [IdentityHub.ApplicationModel] - AddIdentityHub: composes the manifest with typed options")]
    public void AddIdentityHub_WithManifestAndOptions_ShouldReturnTypedResourceDescriptor()
    {
        // Arrange
        IApplicationBuilder builder = Application.CreateBuilder();
        ResourceManifest manifest = IdentityHubManifestFactory.Create();
        var options = new IdentityHubResourceOptions();
        options.Storage.Size = "20Gi";

        // Act
        IApplicationResourceDescriptor descriptor = builder.AddIdentityHub(manifest, options);

        // Assert
        IdentityHubResource resource = descriptor.Resource.ShouldBeOfType<IdentityHubResource>();
        resource.Options.ShouldBeSameAs(options);
        resource.PlannerName.ShouldBe("IdentityHub planner");
    }

    [Fact(DisplayName = "Cohesion Test [IdentityHub.ApplicationModel] - AddIdentityHub: rejects a null manifest")]
    public void AddIdentityHub_WithNullManifest_ShouldThrow()
    {
        // Arrange
        IApplicationBuilder builder = Application.CreateBuilder();

        // Act
        ArgumentNullException error = Should.Throw<ArgumentNullException>(
            () => builder.AddIdentityHub(null!));

        // Assert
        error.ParamName.ShouldBe("manifest");
    }
}
```

[All examples](index.md) · [Assembly overview](../index.md)

## Sources

- **Primary source** — `cohesion/resources/IdentityHub/Assimalign.Cohesion.IdentityHub.ApplicationModel/tests/IdentityHubResourceTests.cs`.
- **Source** — `cohesion/resources/IdentityHub/Assimalign.Cohesion.IdentityHub.ApplicationModel/tests/Assimalign.Cohesion.IdentityHub.ApplicationModel.Tests.csproj`.
