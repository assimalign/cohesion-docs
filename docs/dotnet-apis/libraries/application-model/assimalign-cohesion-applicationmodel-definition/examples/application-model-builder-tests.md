# Application Model Builder Tests

This example exercises `Assimalign.Cohesion.ApplicationModel.Definition` through its co-located test source.

> **Status:** Implemented.

The example reproduces
`cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Definition/tests/ApplicationModelBuilderTests.cs`. It
retains the test class and assertions so the setup, operation, and expected outcome stay together.
Use it in the source project’s test context, with its test dependencies and supporting test objects.

## Behavior exercised

- **Case 1** — The builder is the gateway's set, and its resources point back to it.
- **Case 2** — A builder needs a gateway name.
- **Case 3** — Building produces the gateway's model, a root without a parent.
- **Case 4** — Naming a parent makes the gateway a composite that joins it.
- **Case 5** — A gateway joins one parent, never itself.
- **Case 6** — Building closes composition.
- **Case 7** — A setting from a gateway that is not the parent fails the build, which stays open.

## Source example

```csharp
using System;

using Shouldly;
using Xunit;

namespace Assimalign.Cohesion.ApplicationModel.Definition.Tests;

public class ApplicationModelBuilderTests
{
    private const string DisplayPrefix = "Cohesion Test [ApplicationModel] - ApplicationModelBuilder: ";

    [Fact(DisplayName = DisplayPrefix + "The builder is the gateway's set, and its resources point back to it")]
    public void CreateBuilder_WithName_ShouldBeTheGatewaysSet()
    {
        // Arrange & Act
        IApplicationModelBuilder builder = ApplicationModel.CreateBuilder("east");
        IApplicationResourceDefinition api = builder.AddResource("Web", "orders-api");

        // Assert
        builder.Name.ShouldBe(new GatewayName("east"));
        builder.Id.ShouldBe(GatewayId.FromName("east"));
        builder.Parent.ShouldBeNull();
        builder.Resources.ShouldHaveSingleItem().ShouldBeSameAs(api);
        api.Set.ShouldBeSameAs(builder);
    }

    [Fact(DisplayName = DisplayPrefix + "A builder needs a gateway name")]
    public void CreateBuilder_WithDefaultName_ShouldThrow()
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => ApplicationModel.CreateBuilder(default));
    }

    [Fact(DisplayName = DisplayPrefix + "Building produces the gateway's model, a root without a parent")]
    public void Build_WithoutParent_ShouldBuildARootModel()
    {
        // Arrange
        IApplicationModelBuilder builder = ApplicationModel.CreateBuilder("root");
        builder.AddResource("IdentityHub", "identity");

        // Act
        IApplicationModel model = builder.Build();

        // Assert
        model.Name.ShouldBe(new GatewayName("root"));
        model.Id.ShouldBe(GatewayId.FromName("root"));
        model.Parent.ShouldBeNull();
        model.Role.ShouldBe(ApplicationGatewayRole.Root);
        model.Manifest.Resources.ShouldHaveSingleItem().Name.Value.ShouldBe("identity");
    }

    [Fact(DisplayName = DisplayPrefix + "Naming a parent makes the gateway a composite that joins it")]
    public void UseParent_WithName_ShouldNameTheParent()
    {
        // Arrange
        IApplicationModelBuilder builder = ApplicationModel.CreateBuilder("east");

        // Act
        IApplicationModelParent parent = builder.UseParent("root");
        IApplicationModel model = builder.Build();

        // Assert
        parent.Name.ShouldBe(new GatewayName("root"));
        parent.Id.ShouldBe(GatewayId.FromName("root"));
        builder.Parent.ShouldBeSameAs(parent);
        model.Parent.ShouldBe(new GatewayName("root"));
        model.Manifest.Parent.ShouldBe(new GatewayName("root"));
        model.Role.ShouldBe(ApplicationGatewayRole.Composite);
    }

    [Fact(DisplayName = DisplayPrefix + "A gateway joins one parent, never itself")]
    public void UseParent_WithInvalidParent_ShouldThrow()
    {
        // Arrange
        IApplicationModelBuilder builder = ApplicationModel.CreateBuilder("east");

        // Act & Assert
        Should.Throw<ArgumentException>(() => builder.UseParent(default));
        Should.Throw<ArgumentException>(() => builder.UseParent("east"));
        builder.UseParent("root");
        Should.Throw<InvalidOperationException>(() => builder.UseParent("west"));
    }

    [Fact(DisplayName = DisplayPrefix + "Building closes composition")]
    public void Build_ThenCompose_ShouldThrow()
    {
        // Arrange
        IApplicationModelBuilder builder = ApplicationModel.CreateBuilder("east");
        IApplicationResourceDefinition database = builder.AddResource("Database.Sql", "orders-db");
        IApplicationResourceDefinition api = builder.AddResource("Web", "orders-api");
        builder.Build();

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => builder.AddResource("Web", "customers-api"));
        Should.Throw<InvalidOperationException>(() => api.DependsOn(database, "data.read"));
        Should.Throw<InvalidOperationException>(() => builder.UseParent("root"));
        Should.Throw<InvalidOperationException>(() => builder.Build());
        api.Dependencies.ShouldBeEmpty();
    }

    [Fact(DisplayName = DisplayPrefix + "A setting from a gateway that is not the parent fails the build, which stays open")]
    public void Build_WithOutputOfAnotherGateway_ShouldThrowAndStayOpen()
    {
        // Arrange
        IApplicationModelBuilder builder = ApplicationModel.CreateBuilder("east");
        builder.AddResource("Web", "orders-api")
            .WithSetting("Identity:Endpoint", new ApplicationResourceOutput("root", "IdentityHub", "identity", OutputName.Endpoint));

        // Act & Assert
        Should.Throw<ArgumentException>(() => builder.Build()).Message.ShouldContain("root", Case.Sensitive);
        builder.UseParent("root");
        builder.Build().Manifest.Resources.ShouldHaveSingleItem().Settings.ShouldHaveSingleItem().Output!.Gateway.ShouldBe(new GatewayName("root"));
    }
}
```

[Examples](index.md) · [Assembly overview](../index.md)

## Sources

- **Primary source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Definition/tests/ApplicationModelBuilderTests.cs`.
- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Definition/tests/Assimalign.Cohesion.ApplicationModel.Definition.Tests.csproj`.
