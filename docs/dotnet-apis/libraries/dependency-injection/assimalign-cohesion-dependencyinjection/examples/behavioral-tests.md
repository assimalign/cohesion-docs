# Example: Dependency Injection Behavioral Tests

Exercise service provider builder behavior and event-source diagnostics using the original repository tests.

[Examples](index.md) · [Assembly overview](../index.md)

## Test-project context

These are complete test-file listings from the package test project. Keep them in that project
when running them: the project supplies its package references, generated sources, and any shared
fixtures. The using blocks make the test-framework import explicit where the original project
supplies it globally.

## Code

### ServiceProviderBuilderTests.cs

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

### ServiceEventSourceTests.cs

```csharp
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Runtime.CompilerServices;

using Shouldly;
using Xunit;

using Assimalign.Cohesion.DependencyInjection.Internal;

using static Assimalign.Cohesion.DependencyInjection.Tests.EventSourceFixtures;

using Assimalign.Cohesion.Testing;

namespace Assimalign.Cohesion.DependencyInjection.Tests;

/// <summary>
/// Verifies the container's event source against the repository's EventSource convention.
/// </summary>
/// <remarks>
/// <c>DynamicMethodBuilt</c> is exercised by <see cref="ServiceProviderDynamicCodeTests"/>, which needs a
/// JIT runtime. <c>ExpressionTreeGenerated</c> is never written while the IL resolver is built in, and
/// <c>ServiceRealizationFailed</c> only when background compilation fails; the manifest test covers
/// their shapes.
/// </remarks>
[Collection(nameof(EventSourceCollection))]
public sealed class ServiceEventSourceTests
{
    [Fact(DisplayName = "Cohesion Test [DependencyInjection] - ServiceEventSource: Should be named for its assembly")]
    public void GetName_ServiceEventSource_ShouldEqualAssemblyName()
    {
        // Act
        string name = EventSource.GetName(typeof(ServiceEventSource));

        // Assert
        name.ShouldBe(typeof(ServiceEventSource).Assembly.GetName().Name);
        name.ShouldBe("Assimalign.Cohesion.DependencyInjection");
    }

    [Fact(DisplayName = "Cohesion Test [DependencyInjection] - ServiceEventSource: Should generate a manifest in strict mode")]
    [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode", Justification = "Test-only manifest validation reflects over the event source; test assemblies are not trimmed.")]
    [UnconditionalSuppressMessage("Trimming", "IL2111:DynamicallyAccessedMembers", Justification = "Test-only manifest validation reflects over the event source; test assemblies are not trimmed.")]
    public void GenerateManifest_StrictMode_ShouldSucceed()
    {
        // Act
        string? manifest = EventSource.GenerateManifest(typeof(ServiceEventSource), assemblyPathToIncludeInManifest: null, EventManifestOptions.Strict);

        // Assert
        manifest.ShouldNotBeNull();
        manifest.ShouldContain("Assimalign.Cohesion.DependencyInjection", Case.Sensitive);
    }

    [Fact(DisplayName = "Cohesion Test [DependencyInjection] - ServiceEventSource: Should report each provider and scope event once with its declared payload")]
    public void ProviderLifecycle_BuildResolveAndDispose_ShouldReportEachEventOnce()
    {
        // Arrange
        using var recorder = new EventSourceRecorder(ServiceEventSource.Log, EventLevel.Verbose);
        using var builder = new ServiceProviderBuilder(new ServiceProviderOptions { EnableDynamicCode = false });
        builder.AddSingleton<ObservedSingleton>();
        builder.AddScoped<ObservedScopedDisposable>();
        var provider = (ServiceProvider)((IServiceProviderBuilder)builder).Build();
        int providerId = provider.GetHashCode();

        // Act
        provider.GetRequiredService<ObservedSingleton>();
        using (IServiceScope scope = provider.CreateScope())
        {
            scope.ServiceProvider.GetRequiredService<ObservedScopedDisposable>();
        }
        provider.Dispose();

        // Assert
        IReadOnlyList<EventWrittenEventArgs> events = recorder.Events;
        events.ShouldNotContain(e => e.EventId == 0, "EventSource reported an instrumentation error.");

        EventWrittenEventArgs built = Single(events, "ServiceProviderBuilt", "serviceProviderHashCode", providerId);
        built.PayloadNames.ShouldBe(["serviceProviderHashCode", "singletonServices", "scopedServices", "transientServices", "closedGenericsServices", "openGenericsServices"]);
        built.Level.ShouldBe(EventLevel.Informational);
        built.Payload.ShouldBe([providerId, 1, 1, 0, 0, 0]);

        EventWrittenEventArgs descriptors = Single(events, "ServiceProviderDescriptors", "serviceProviderHashCode", providerId);
        descriptors.PayloadNames.ShouldBe(["serviceProviderHashCode", "descriptors", "chunkIndex", "chunkCount"]);
        descriptors.Level.ShouldBe(EventLevel.Verbose);
        descriptors.Payload![1].ShouldBeOfType<string>().ShouldContain(typeof(ObservedSingleton).ToString(), Case.Sensitive);

        foreach (Type serviceType in new[] { typeof(ObservedSingleton), typeof(ObservedScopedDisposable) })
        {
            EventWrittenEventArgs callSite = events.Where(e => e.EventName == "CallSiteBuilt" && Equals(e.Payload![0], serviceType.ToString())).ShouldHaveSingleItem();
            callSite.PayloadNames.ShouldBe(["serviceType", "callSite", "chunkIndex", "chunkCount", "serviceProviderHashCode"]);
            callSite.Payload![4].ShouldBe(providerId);

            EventWrittenEventArgs resolved = events.Where(e => e.EventName == "ServiceResolved" && Equals(e.Payload![0], serviceType.ToString())).ShouldHaveSingleItem();
            resolved.PayloadNames.ShouldBe(["serviceType", "serviceProviderHashCode"]);
            resolved.Payload![1].ShouldBe(providerId);
        }

        // One for the scope, then one for the root scope when the provider is disposed.
        EventWrittenEventArgs[] disposed = events.Where(e => e.EventName == "ScopeDisposed" && Equals(e.Payload![0], providerId)).ToArray();
        disposed.Length.ShouldBe(2);
        disposed[0].PayloadNames.ShouldBe(["serviceProviderHashCode", "scopedServicesResolved", "disposableServices"]);
        disposed[0].Payload.ShouldBe([providerId, 1, 1]);
    }

    [Fact(DisplayName = "Cohesion Test [DependencyInjection] - ServiceEventSource: Should stop tracking providers that were never disposed once they are collected")]
    public void ServiceProviderBuilt_ForProvidersNeverDisposed_ShouldPruneCollectedProviders()
    {
        // Arrange: the event source remembers every provider for listeners that attach later, and one
        // that is never disposed used to stay in that list for the life of the process.
        const int providerCount = 2_000;
        BuildAndDropProviders(providerCount);
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        // Act
        using var builder = new ServiceProviderBuilder();
        using var provider = (ServiceProvider)((IServiceProviderBuilder)builder).Build();

        // Assert: pruning runs whenever the list has doubled since the last pruning, so it stays within
        // twice the providers alive at that point; a collection every 50 builds keeps that small.
        ServiceEventSource.Log.TrackedProviderCount.ShouldBeLessThan(200);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void BuildAndDropProviders(int count)
    {
        for (int i = 0; i < count; i++)
        {
            using var builder = new ServiceProviderBuilder();
            builder.AddSingleton<ObservedSingleton>();
            ((IServiceProviderBuilder)builder).Build();

            if (i % 50 == 0)
            {
                GC.Collect();
            }
        }
    }

    private static EventWrittenEventArgs Single(IEnumerable<EventWrittenEventArgs> events, string eventName, string payloadName, object payloadValue) =>
        events.Where(e => e.EventName == eventName && e.PayloadNames!.Contains(payloadName) && Equals(e.Payload![e.PayloadNames.IndexOf(payloadName)], payloadValue))
            .ShouldHaveSingleItem();
}
```

## Walkthrough

### ServiceProviderBuilderTests

- **Covered behavior** — Container: Should not be shared between builders.
- **Covered behavior** — Container: Should return the same container for the same builder.
- **Covered behavior** — Build: Should resolve only the owning builder's registrations.
- **Covered behavior** — Dispose: Should not throw.
- **Covered behavior** — Dispose: Should not throw when called more than once.
- **Covered behavior** — Dispose: Should not throw when disposed by a using block.
- **Covered behavior** — Dispose: Should not affect an already built provider.

### ServiceEventSourceTests

- **Covered behavior** — ServiceEventSource: Should be named for its assembly.
- **Covered behavior** — ServiceEventSource: Should generate a manifest in strict mode.
- **Covered behavior** — ServiceEventSource: Should report each provider and scope event once with its declared payload.
- **Covered behavior** — ServiceEventSource: Should stop tracking providers that were never disposed once they are collected.

The assertions define the expected result or failure boundary. Test-only doubles supply controlled
inputs; they are not additional shipped APIs.

## Sources

- **Source** — `cohesion/libraries/DependencyInjection/Assimalign.Cohesion.DependencyInjection/tests/ServiceProviderBuilderTests.cs`.
- **Source** — `cohesion/libraries/DependencyInjection/Assimalign.Cohesion.DependencyInjection/tests/Assimalign.Cohesion.DependencyInjection.Tests.csproj`.

- **Source** — `cohesion/libraries/DependencyInjection/Assimalign.Cohesion.DependencyInjection/tests/ServiceEventSourceTests.cs`.
- **Source** — `cohesion/build/TestSupport/EventSourceRecorder.cs`.
