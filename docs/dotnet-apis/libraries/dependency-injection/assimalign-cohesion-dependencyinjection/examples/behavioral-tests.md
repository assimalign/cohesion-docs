# Example: Service Provider Builder Tests

Exercise Service Provider Builder behavior using the original repository tests.

[Examples](index.md) · [Assembly overview](../index.md)

## Test-project context

This is the complete `ServiceProviderBuilderTests.cs` listing from the package test project. Keep it
in that project when running it: the project supplies its package references, generated sources, and
any shared fixtures. The using block below makes the test-framework import explicit where the
original project supplies it globally.

## Code

```csharp
using System;
using Shouldly;
using Xunit;

namespace Assimalign.Cohesion.DependencyInjection.Tests;

public class ServiceProviderBuilderTests
{
    [Fact(DisplayName = "Cohesion Test [DependencyInjection] - Container: Should not be shared between builders")]
    public void Container_ForSeparateBuilders_ShouldNotShareAContainer()
    {
        // Arrange
        var first = new ServiceProviderBuilder();
        var second = new ServiceProviderBuilder();

        // Act
        first.AddSingleton<IMarkerService>(new FirstService());

        // Assert
        first.Container.ShouldNotBeSameAs(second.Container);
        first.Container.Count.ShouldBe(1);
        second.Container.Count.ShouldBe(0);
    }

    [Fact(DisplayName = "Cohesion Test [DependencyInjection] - Container: Should return the same container for the same builder")]
    public void Container_OnRepeatedAccess_ShouldReturnTheSameContainer()
    {
        // Arrange
        var builder = new ServiceProviderBuilder();

        // Act
        var container = builder.Container;

        // Assert
        builder.Container.ShouldBeSameAs(container);
    }

    [Fact(DisplayName = "Cohesion Test [DependencyInjection] - Build: Should resolve only the owning builder's registrations")]
    public void Build_ForSeparateBuilders_ShouldResolveOwnRegistrations()
    {
        // Arrange
        var first = new ServiceProviderBuilder();
        var second = new ServiceProviderBuilder();

        first.AddSingleton<IMarkerService>(new FirstService());
        second.AddSingleton<IMarkerService>(new SecondService());

        // Act
        var firstProvider = ((IServiceProviderBuilder)first).Build();
        var secondProvider = ((IServiceProviderBuilder)second).Build();

        // Assert
        firstProvider.GetRequiredService<IMarkerService>().ShouldBeOfType<FirstService>();
        secondProvider.GetRequiredService<IMarkerService>().ShouldBeOfType<SecondService>();
    }

    [Fact(DisplayName = "Cohesion Test [DependencyInjection] - Dispose: Should not throw")]
    public void Dispose_OnBuilderWithRegistrations_ShouldNotThrow()
    {
        // Arrange
        var builder = new ServiceProviderBuilder();

        builder.AddSingleton<IMarkerService>(new FirstService());

        // Act & Assert
        Should.NotThrow(builder.Dispose);
    }

    [Fact(DisplayName = "Cohesion Test [DependencyInjection] - Dispose: Should not throw when called more than once")]
    public void Dispose_WhenCalledMoreThanOnce_ShouldNotThrow()
    {
        // Arrange
        var builder = new ServiceProviderBuilder();

        builder.Dispose();

        // Act & Assert
        Should.NotThrow(builder.Dispose);
    }

    [Fact(DisplayName = "Cohesion Test [DependencyInjection] - Dispose: Should not throw when disposed by a using block")]
    public void Dispose_WithinAUsingBlock_ShouldNotThrow()
    {
        // Act & Assert
        Should.NotThrow(() =>
        {
            using var builder = new ServiceProviderBuilder();

            builder.AddSingleton<IMarkerService>(new FirstService());
        });
    }

    [Fact(DisplayName = "Cohesion Test [DependencyInjection] - Dispose: Should not affect an already built provider")]
    public void Dispose_AfterBuild_ShouldNotAffectTheBuiltProvider()
    {
        // Arrange
        IServiceProvider provider;

        // Act
        using (var builder = new ServiceProviderBuilder())
        {
            builder.AddSingleton<IMarkerService>(new FirstService());

            provider = ((IServiceProviderBuilder)builder).Build();
        }

        // Assert
        provider.GetRequiredService<IMarkerService>().ShouldBeOfType<FirstService>();
    }

    private interface IMarkerService
    {
    }

    private sealed class FirstService : IMarkerService
    {
    }

    private sealed class SecondService : IMarkerService
    {
    }
}
```

## Walkthrough

- **Covered behavior** — Container: Should not be shared between builders.
- **Covered behavior** — Container: Should return the same container for the same builder.
- **Covered behavior** — Build: Should resolve only the owning builder's registrations.
- **Covered behavior** — Dispose: Should not throw.
- **Covered behavior** — Dispose: Should not throw when called more than once.
- **Covered behavior** — Dispose: Should not throw when disposed by a using block.
- **Covered behavior** — Dispose: Should not affect an already built provider.

The assertions define the expected result or failure boundary. Test-only doubles supply controlled
inputs; they are not additional shipped APIs.

## Sources

- **Source** — `cohesion/libraries/DependencyInjection/Assimalign.Cohesion.DependencyInjection/tests/ServiceProviderBuilderTests.cs`.
- **Source** — `cohesion/libraries/DependencyInjection/Assimalign.Cohesion.DependencyInjection/tests/Assimalign.Cohesion.DependencyInjection.Tests.csproj`.
