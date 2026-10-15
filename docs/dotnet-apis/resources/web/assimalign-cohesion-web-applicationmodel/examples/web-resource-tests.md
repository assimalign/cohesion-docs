# Web Resource Tests

This example exercises `Assimalign.Cohesion.Web.ApplicationModel` through its co-located test source.

> **Status:** Partial.

The example reproduces
`cohesion/resources/Web/Assimalign.Cohesion.Web.ApplicationModel/tests/WebResourceTests.cs`. It
retains the test class and assertions so the setup, operation, and expected outcome stay together.
`Use` it in the source project’s test context, with its test dependencies and supporting test objects.

## Behavior exercised

- **Case 1** — Constructor: wraps an immutable manifest snapshot with typed options.
- **Case 2** — `AddWeb`: composes the manifest with typed options.
- **Case 3** — `AddWeb`: rejects a null manifest.

## Source example

```csharp
using System;
using Shouldly;
using Xunit;
using Assimalign.Cohesion.ApplicationModel;

namespace Assimalign.Cohesion.Web.ApplicationModel.Tests;

public sealed class WebResourceTests
{
    [Fact(DisplayName = "Cohesion Test [Web.ApplicationModel] - Constructor: wraps an immutable manifest snapshot with typed options")]
    public void Constructor_WithManifestAndOptions_ShouldExposeSnapshotAndTypedOptions()
    {
        // Arrange
        ResourceManifest manifest = WebManifestFactory.Create();
        var options = new WebResourceOptions { Replicas = 3 };

        // Act
        var resource = new WebResource(manifest, options);

        // Assert
        resource.Manifest.ShouldNotBeSameAs(manifest);
        resource.Name.ShouldBe(manifest.Name);
        resource.Artifact.ShouldBe(manifest.Artifact.Assembly);
        resource.Endpoints.Count.ShouldBe(manifest.Endpoints.Count);
        resource.Mounts.Count.ShouldBe(1);
        resource.Mounts[0].Kind.ShouldBe(ResourceMountKind.Secret);
        resource.Options.ShouldBeSameAs(options);
        resource.PlannerName.ShouldBe("Web planner");
    }

    [Fact(DisplayName = "Cohesion Test [Web.ApplicationModel] - AddWeb: composes the manifest with typed options")]
    public void AddWeb_WithManifestAndOptions_ShouldReturnWebResourceDescriptor()
    {
        // Arrange
        IApplicationBuilder builder = Application.CreateBuilder();
        ResourceManifest manifest = WebManifestFactory.Create();
        var options = new WebResourceOptions { Replicas = 3 };

        // Act
        IWebResourceDescriptor descriptor = builder.AddWeb(manifest, options);

        // Assert
        WebResource resource = descriptor.Resource.ShouldBeOfType<WebResource>();
        resource.Options.ShouldBeSameAs(options);
    }

    [Fact(DisplayName = "Cohesion Test [Web.ApplicationModel] - AddWeb: rejects a null manifest")]
    public void AddWeb_WithNullManifest_ShouldThrow()
    {
        // Arrange
        IApplicationBuilder builder = Application.CreateBuilder();

        // Act
        ArgumentNullException error = Should.Throw<ArgumentNullException>(
            () => builder.AddWeb(null!));

        // Assert
        error.ParamName.ShouldBe("manifest");
    }

}
```

[All examples](index.md) · [Assembly overview](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.ApplicationModel/tests/WebResourceTests.cs`.
- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.ApplicationModel/tests/Assimalign.Cohesion.Web.ApplicationModel.Tests.csproj`.
