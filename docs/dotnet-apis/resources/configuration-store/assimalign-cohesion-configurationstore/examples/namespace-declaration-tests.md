# Namespace Declaration Tests

This example exercises `Assimalign.Cohesion.ConfigurationStore` through its co-located test source.

> **Status:** Implemented.

The example reproduces
`cohesion/resources/ConfigurationStore/Assimalign.Cohesion.ConfigurationStore/tests/NamespaceDeclarationTests.cs`
. It retains the test class and assertions so the setup, operation, and expected outcome stay
together. `Use` it in the source project’s test context, with its test dependencies and supporting
test objects.

## Behavior exercised

- **Case 1** — `AddNamespace`: captures one fluent declaration and rejects duplicates.
- **Case 2** — `AddNamespace`: rejects blank names, keys, and null callbacks.

## Source example

```csharp
using System;
using Shouldly;
using Xunit;
using Assimalign.Cohesion.ConfigurationStore.Hosting;

namespace Assimalign.Cohesion.ConfigurationStore.Tests;

public sealed class NamespaceDeclarationTests
{
    [Fact(DisplayName = "Cohesion Test [ConfigurationStore] - AddNamespace: captures one fluent declaration and rejects duplicates")]
    public void AddNamespace_WithValidDeclaration_ShouldReturnBuilderAndRejectDuplicate()
    {
        IConfigurationStoreApplicationBuilder builder = ConfigurationStoreApplication.CreateBuilder([]);

        IConfigurationStoreApplicationBuilder returned = builder.AddNamespace(
            "app",
            ns => ns.Set("Mode", "production").Set("Optional", null));

        returned.ShouldBeSameAs(builder);
        Should.Throw<InvalidOperationException>(() =>
            builder.AddNamespace("app", ns => ns.Set("Mode", "development")));
    }

    [Fact(DisplayName = "Cohesion Test [ConfigurationStore] - AddNamespace: rejects blank names, keys, and null callbacks")]
    public void AddNamespace_WithInvalidDeclaration_ShouldRejectInput()
    {
        IConfigurationStoreApplicationBuilder builder = ConfigurationStoreApplication.CreateBuilder([]);

        Should.Throw<ArgumentException>(() => builder.AddNamespace(" ", _ => { }));
        Should.Throw<ArgumentNullException>(() => builder.AddNamespace("app", null!));
        Should.Throw<ArgumentException>(() => builder.AddNamespace("app", ns => ns.Set(" ", "value")));
        Should.Throw<ArgumentException>(() => builder.AddNamespace("other", ns => ns.Set("section/key", "value")));
    }
}
```

[All examples](index.md) · [Assembly overview](../index.md)

## Sources

- **Primary source** — `cohesion/resources/ConfigurationStore/Assimalign.Cohesion.ConfigurationStore/tests/NamespaceDeclarationTests.cs`.
- **Source** — `cohesion/resources/ConfigurationStore/Assimalign.Cohesion.ConfigurationStore/tests/Assimalign.Cohesion.ConfigurationStore.Tests.csproj`.
