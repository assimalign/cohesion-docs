# Example: Connection Behavioral Tests

Exercise connection direction, feature collections, the unified hierarchy, live listener collections, and layered-listener behavior and diagnostics using the original repository tests.

[Examples](index.md) · [Assembly overview](../index.md)

## Test-project context

These are the complete `ConnectionDirectionTests.cs`, `ConnectionLayerEventSourceTests.cs`,
`ConnectionFeatureCollectionTests.cs`, `ConnectionFeaturesTests.cs`, `GuidedBaseTests.cs`,
`ConnectionListenerTests.cs`, `ConnectionLayerTests.cs`, and `LayeredConnectionListenerTests.cs` listings
from the package test project. Keep them in that project when running them: the project supplies its
package references, generated sources, and shared fixtures. The diagnostics tests use the canonical
test-only `build/TestSupport/EventSourceRecorder.cs`, linked with MSBuild `Compile`/`Link`.

## Code

```csharp
using Shouldly;

using Xunit;

namespace Assimalign.Cohesion.Connections.Tests;

public class ConnectionDirectionTests
{
    [Fact]
    public void Values_OnConnectionDirection_ShouldRemainStable()
    {
        // Arrange & Act & Assert
        ((int)ConnectionDirection.Bidirectional).ShouldBe(0);
        ((int)ConnectionDirection.ReadOnly).ShouldBe(1);
        ((int)ConnectionDirection.WriteOnly).ShouldBe(2);
    }
}
```

## Walkthrough

- **Test entry point** — `Values_OnConnectionDirection_ShouldRemainStable` contains the setup, invocation, and assertions for this case.

The assertions define the expected result or failure boundary. Test-only doubles supply controlled
inputs; they are not additional shipped APIs.

## Connection Layer Event Source Tests

### Code

```csharp
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Tracing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Shouldly;

using Xunit;

using Assimalign.Cohesion.Connections.Internal;
using Assimalign.Cohesion.Testing;

namespace Assimalign.Cohesion.Connections.Tests;

[Collection(nameof(EventSourceCollection))]
public class ConnectionLayerEventSourceTests
{
    private static readonly TimeSpan _testTimeout = TimeSpan.FromSeconds(10);

    [Fact(DisplayName = "Cohesion Test [Connections] - ConnectionLayerEventSource: Should be named for its assembly")]
    public void GetName_ConnectionLayerEventSource_ShouldEqualAssemblyName()
    {
        // Act
        string name = EventSource.GetName(typeof(ConnectionLayerEventSource));

        // Assert
        name.ShouldBe(typeof(ConnectionLayerEventSource).Assembly.GetName().Name);
        name.ShouldBe("Assimalign.Cohesion.Connections");
    }

    [Fact(DisplayName = "Cohesion Test [Connections] - ConnectionLayerEventSource: Should generate a manifest in strict mode")]
    [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode", Justification = "Test-only manifest validation reflects over the event source; test assemblies are not trimmed.")]
    [UnconditionalSuppressMessage("Trimming", "IL2111:DynamicallyAccessedMembers", Justification = "Test-only manifest validation reflects over the event source; test assemblies are not trimmed.")]
    public void GenerateManifest_StrictMode_ShouldSucceed()
    {
        // Act
        string? manifest = EventSource.GenerateManifest(typeof(ConnectionLayerEventSource), assemblyPathToIncludeInManifest: null, EventManifestOptions.Strict);

        // Assert
        manifest.ShouldNotBeNull();
        manifest.ShouldContain("Assimalign.Cohesion.Connections", Case.Sensitive);
    }

    [Fact(DisplayName = "Cohesion Test [Connections] - ConnectionLayerEventSource: Should report a failed upgrade once and restore the gauge")]
    public async Task UpgradeFailed_OnRejectedConnection_ShouldReportOnceAndRestoreTheGauge()
    {
        // Arrange
        using CancellationTokenSource timeout = new(_testTimeout);
        using EventSourceRecorder recorder = new(ConnectionLayerEventSource.Log, EventLevel.Verbose);
        long currentBefore = ConnectionLayerEventSource.Log.CurrentUpgrades;
        long failedBefore = ConnectionLayerEventSource.Log.FailedUpgrades;
        TestConnection rejected = new();
        TestConnection ready = new();
        ControlledConnectionLayer layer = new();
        layer.Fail(rejected, new IOException("Not a handshake."));
        BlockingConnectionListener inner = new();
        inner.Enqueue(rejected);
        inner.Enqueue(ready);
        IDuplexConnectionListener listener = inner.Use(layer);

        // Act — disposal waits for every upgrade, so the failure has been reported once it returns.
        IDuplexConnection accepted = await listener.AcceptAsync(timeout.Token);
        await listener.DisposeAsync();

        // Assert
        accepted.ShouldBeSameAs(ready);
        ConnectionLayerEventSource.Log.CurrentUpgrades.ShouldBe(currentBefore);
        ConnectionLayerEventSource.Log.FailedUpgrades.ShouldBe(failedBefore + 1);

        IReadOnlyList<EventWrittenEventArgs> events = recorder.Events;
        events.ShouldNotContain(e => e.EventId == 0, "EventSource reported an instrumentation error.");

        EventWrittenEventArgs failed = events.ShouldHaveSingleItem();
        failed.EventName.ShouldBe("UpgradeFailed");
        failed.Level.ShouldBe(EventLevel.Warning);
        failed.PayloadNames.ShouldBe(["connectionId", "remoteEndPoint", "exceptionType", "exceptionMessage"]);
        failed.Payload![0].ShouldBe(rejected.Id.ToString());
        failed.Payload[1].ShouldBe(rejected.RemoteEndPoint!.ToString());
        failed.Payload[2].ShouldBe(typeof(IOException).FullName);
        failed.Payload[3].ShouldBe("Not a handshake.");
    }

    [Fact(DisplayName = "Cohesion Test [Connections] - ConnectionLayerEventSource: Should not report upgrades that disposal cancels")]
    public async Task UpgradeFailed_OnUpgradeCanceledByDisposal_ShouldNotReport()
    {
        // Arrange
        using CancellationTokenSource timeout = new(_testTimeout);
        using EventSourceRecorder recorder = new(ConnectionLayerEventSource.Log, EventLevel.Verbose);
        long currentBefore = ConnectionLayerEventSource.Log.CurrentUpgrades;
        long failedBefore = ConnectionLayerEventSource.Log.FailedUpgrades;
        TestConnection stalled = new();
        ControlledConnectionLayer layer = new();
        layer.Hold(stalled);
        BlockingConnectionListener inner = new();
        inner.Enqueue(stalled);
        IDuplexConnectionListener listener = inner.Use(layer);
        Task<IDuplexConnection> pending = listener.AcceptAsync(timeout.Token).AsTask();
        await layer.WhenStarted(stalled).WaitAsync(timeout.Token);

        // Act
        await listener.DisposeAsync();

        // Assert
        await Should.ThrowAsync<ObjectDisposedException>(() => pending);
        stalled.ConnectionClosed.IsCancellationRequested.ShouldBeTrue();
        ConnectionLayerEventSource.Log.CurrentUpgrades.ShouldBe(currentBefore);
        ConnectionLayerEventSource.Log.FailedUpgrades.ShouldBe(failedBefore);
        recorder.Events.ShouldBeEmpty();
    }

    [Fact(DisplayName = "Cohesion Test [Connections] - ConnectionLayerEventSource: Should publish its upgrade counters")]
    public async Task Counters_EnabledWithInterval_ShouldPublishUpgradeCounters()
    {
        // Arrange
        using CancellationTokenSource timeout = new(_testTimeout);
        string[] counters = ["current-upgrades", "failed-upgrades"];

        // Act
        using EventSourceRecorder recorder = new(ConnectionLayerEventSource.Log, EventLevel.LogAlways, counterInterval: TimeSpan.FromMilliseconds(100));

        // Assert
        await recorder.WaitForCountersAsync(counters, timeout.Token);
    }
}
```

### Walkthrough

- **Covered behavior** — ConnectionLayerEventSource: Should be named for its assembly.
- **Covered behavior** — ConnectionLayerEventSource: Should generate a manifest in strict mode.
- **Covered behavior** — ConnectionLayerEventSource: Should report a failed upgrade once and restore the gauge.
- **Covered behavior** — ConnectionLayerEventSource: Should not report upgrades that disposal cancels.
- **Covered behavior** — ConnectionLayerEventSource: Should publish its upgrade counters.

The assertions define the expected result or failure boundary. Test-only doubles supply controlled
inputs; they are not additional shipped APIs.

## Connection Feature Collection Tests

### Code

```csharp
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Shouldly;
using Xunit;

namespace Assimalign.Cohesion.Connections.Tests;

public class ConnectionFeatureCollectionTests
{
    [Fact(DisplayName = "Cohesion Test [Connections] - Get: Wrapped features pass through by name and type")]
    public void Get_WithInnerFeature_ShouldPassThrough()
    {
        // Arrange
        var feature = new TestConnectionFeature("Shared");
        var inner = new ConnectionFeatureCollection();
        inner.Set(feature);
        IConnectionFeatureCollection features = new ConnectionFeatureCollection(inner);

        // Act and assert
        features.Get("Shared").ShouldBeSameAs(feature);
        features.Get<TestConnectionFeature>().ShouldBeSameAs(feature);
        features.Get("shared").ShouldBeNull();
        features.Get("Absent").ShouldBeNull();
    }

    [Fact(DisplayName = "Cohesion Test [Connections] - Set: Replaces the named slot regardless of feature type")]
    public void Set_WithSameName_ShouldReplaceLocalAndHideInner()
    {
        // Arrange
        var inner = new ConnectionFeatureCollection();
        inner.Set(new TestConnectionFeature("Shared"));
        IConnectionFeatureCollection features = new ConnectionFeatureCollection(inner);
        features.Set(new TestConnectionFeature("Shared"));
        var replacement = new OtherFeature("Shared");

        // Act
        features.Set(replacement);

        // Assert
        features.Get("Shared").ShouldBeSameAs(replacement);
        features.Get<TestConnectionFeature>().ShouldBeNull();
        features.ShouldHaveSingleItem().ShouldBeSameAs(replacement);
    }

    [Fact(DisplayName = "Cohesion Test [Connections] - Remove: Hides a wrapped feature without changing the wrapped collection")]
    public void Remove_WithInnerFeature_ShouldHideName()
    {
        // Arrange
        var feature = new TestConnectionFeature("Shared");
        var inner = new ConnectionFeatureCollection();
        inner.Set(feature);
        IConnectionFeatureCollection features = new ConnectionFeatureCollection(inner);

        // Act
        bool removed = features.Remove("Shared");

        // Assert
        removed.ShouldBeTrue();
        features.Remove("Shared").ShouldBeFalse();
        features.Get("Shared").ShouldBeNull();
        features.Get<TestConnectionFeature>().ShouldBeNull();
        features.ShouldBeEmpty();
        inner.Get("Shared").ShouldBeSameAs(feature);
    }

    [Fact(DisplayName = "Cohesion Test [Connections] - Remove: Removing a replacement also hides its wrapped slot")]
    public void Remove_WithReplacement_ShouldNotRevealInner()
    {
        // Arrange
        var inner = new ConnectionFeatureCollection();
        inner.Set(new TestConnectionFeature("Shared"));
        var features = new ConnectionFeatureCollection(inner);
        features.Set(new OtherFeature("Shared"));

        // Act
        bool removed = features.Remove("Shared");

        // Assert
        removed.ShouldBeTrue();
        features.Get("Shared").ShouldBeNull();
        features.Get<TestConnectionFeature>().ShouldBeNull();
        features.ShouldBeEmpty();
    }

    [Fact(DisplayName = "Cohesion Test [Connections] - Remove: A hidden name stays hidden when the inner collection changes")]
    public void Remove_WithAbsentName_ShouldHideFutureInnerFeatureUntilSet()
    {
        // Arrange
        var inner = new ConnectionFeatureCollection();
        var features = new ConnectionFeatureCollection(inner);
        var replacement = new TestConnectionFeature("Future");

        // Act
        bool removed = features.Remove("Future");
        inner.Set(new TestConnectionFeature("Future"));

        // Assert
        removed.ShouldBeFalse();
        features.Get("Future").ShouldBeNull();
        features.Get<TestConnectionFeature>().ShouldBeNull();
        features.Set(replacement);
        features.Get("Future").ShouldBeSameAs(replacement);
        features.ShouldHaveSingleItem().ShouldBeSameAs(replacement);
    }

    [Fact(DisplayName = "Cohesion Test [Connections] - Get: Own features precede different-named wrapped features of the same type")]
    public void Get_WithMultipleLayers_ShouldFindOwnFeatureBeforeInner()
    {
        // Arrange
        var inner = new ConnectionFeatureCollection();
        inner.Set(new TestConnectionFeature("Deep"));
        var middle = new ConnectionFeatureCollection(inner);
        var nearest = new TestConnectionFeature("Middle");
        middle.Set(nearest);
        var features = new ConnectionFeatureCollection(middle);
        var own = new TestConnectionFeature("Own");
        features.Set(own);

        // Act and assert
        features.Get<TestConnectionFeature>().ShouldBeSameAs(own);
        features.Remove("Own");
        features.Get<TestConnectionFeature>().ShouldBeSameAs(nearest);
        features.Remove("Middle");
        features.Get<TestConnectionFeature>().ShouldBeSameAs(inner.Get("Deep"));
    }

    [Fact(DisplayName = "Cohesion Test [Connections] - GetEnumerator: Returns own features then only effective wrapped features")]
    public void GetEnumerator_WithReplacementAndRemoval_ShouldYieldEffectiveSetInOrder()
    {
        // Arrange
        var inner = new ConnectionFeatureCollection();
        inner.Set(new TestConnectionFeature("Replaced"));
        inner.Set(new TestConnectionFeature("Hidden"));
        var visible = new TestConnectionFeature("Visible");
        inner.Set(visible);
        var middle = new ConnectionFeatureCollection(inner);
        var replacement = new OtherFeature("Replaced");
        middle.Set(replacement);
        middle.Remove("Hidden");
        var features = new ConnectionFeatureCollection(middle);
        var own = new TestConnectionFeature("Own");
        features.Set(own);

        // Act
        IConnectionFeature[] effective = features.ToArray();
        object[] untyped = ((IEnumerable)features).Cast<object>().ToArray();

        // Assert
        effective.ShouldBe(new IConnectionFeature[] { own, replacement, visible });
        untyped.ShouldBe(new object[] { own, replacement, visible });
    }

    [Fact(DisplayName = "Cohesion Test [Connections] - GetEnumerator: In-flight enumeration retains its local snapshot")]
    public void GetEnumerator_WithConcurrentMutation_ShouldRetainLocalSnapshot()
    {
        // Arrange
        var features = new ConnectionFeatureCollection();
        var first = new TestConnectionFeature("First");
        var second = new TestConnectionFeature("Second");
        features.Set(first);
        features.Set(second);
        using IEnumerator<IConnectionFeature> iterator = features.GetEnumerator();
        iterator.MoveNext().ShouldBeTrue();

        // Act
        features.Remove("Second");
        features.Set(new TestConnectionFeature("Third"));

        // Assert
        iterator.Current.ShouldBeSameAs(first);
        iterator.MoveNext().ShouldBeTrue();
        iterator.Current.ShouldBeSameAs(second);
        iterator.MoveNext().ShouldBeFalse();
    }

    [Fact(DisplayName = "Cohesion Test [Connections] - Set: Rejects null features and empty names")]
    public void Set_WithInvalidFeature_ShouldThrow()
    {
        // Arrange
        IConnectionFeatureCollection features = new ConnectionFeatureCollection();

        // Act and assert
        Should.Throw<ArgumentNullException>(() => features.Set<TestConnectionFeature>(null!));
        Should.Throw<ArgumentException>(() => features.Set(new TestConnectionFeature(string.Empty)));
        Should.Throw<ArgumentNullException>(() => features.Set(new TestConnectionFeature(null!)));
        features.ShouldBeEmpty();
    }

    [Theory(DisplayName = "Cohesion Test [Connections] - Named access: Rejects null and empty names")]
    [InlineData(null)]
    [InlineData("")]
    public void GetAndRemove_WithInvalidName_ShouldThrow(string? name)
    {
        // Arrange
        IConnectionFeatureCollection features = new ConnectionFeatureCollection();

        // Act and assert
        Should.Throw<ArgumentException>(() => features.Get(name!));
        Should.Throw<ArgumentException>(() => features.Remove(name!));
    }

    [Fact(DisplayName = "Cohesion Test [Connections] - Get: Rejects a null collection")]
    public void Get_WithNullCollection_ShouldThrow()
    {
        // Arrange
        IConnectionFeatureCollection features = null!;

        // Act and assert
        Should.Throw<ArgumentNullException>(() => features.Get<TestConnectionFeature>());
    }

    [Fact(DisplayName = "Cohesion Test [Connections] - Get: Respects custom collection enumeration and outer hiding")]
    public void Get_WithCustomInnerCollection_ShouldRespectEffectiveEnumeration()
    {
        // Arrange
        var hidden = new TestConnectionFeature("Hidden");
        var visible = new TestConnectionFeature("Visible");
        IConnectionFeatureCollection custom = new CustomCollection(hidden, visible);
        var features = new ConnectionFeatureCollection(custom);
        features.Remove("Hidden");

        // Act and assert
        custom.Get<TestConnectionFeature>().ShouldBeSameAs(hidden);
        features.Get<TestConnectionFeature>().ShouldBeSameAs(visible);
        features.ShouldHaveSingleItem().ShouldBeSameAs(visible);
    }

    [Fact(DisplayName = "Cohesion Test [Connections] - Get: Layered typed lookups allocate no memory")]
    public void Get_WithConcreteLayers_ShouldAllocateNothing()
    {
        // Arrange
        var inner = new ConnectionFeatureCollection();
        inner.Set(new TestConnectionFeature("Hidden"));
        var match = new TestConnectionFeature("Visible");
        inner.Set(match);
        var middle = new ConnectionFeatureCollection(inner);
        middle.Remove("Hidden");
        IConnectionFeatureCollection features = new ConnectionFeatureCollection(middle);
        for (int index = 0; index < 1000; index++)
        {
            features.Get<TestConnectionFeature>();
            features.Get<OtherFeature>();
        }

        // Act
        long before = GC.GetAllocatedBytesForCurrentThread();
        TestConnectionFeature? result = null;
        OtherFeature? absent = null;
        for (int index = 0; index < 1000; index++)
        {
            result = features.Get<TestConnectionFeature>();
            absent = features.Get<OtherFeature>();
        }
        long allocated = GC.GetAllocatedBytesForCurrentThread() - before;

        // Assert
        result.ShouldBeSameAs(match);
        absent.ShouldBeNull();
        allocated.ShouldBe(0);
    }

    [Fact(DisplayName = "Cohesion Test [Connections] - Features: Concurrent mutations retain independent registrations and safe reads")]
    public async Task SetAndRemove_WithConcurrentReaders_ShouldRemainSafe()
    {
        // Arrange
        var inner = new ConnectionFeatureCollection();
        inner.Set(new TestConnectionFeature("Inherited"));
        var features = new ConnectionFeatureCollection(inner);
        using var start = new ManualResetEventSlim();
        Task[] writers = Enumerable.Range(0, 4).Select(writer => Task.Run(() =>
        {
            start.Wait();
            string name = "Writer" + writer;
            for (int index = 0; index < 1000; index++)
            {
                features.Set(new TestConnectionFeature(name));
                features.Remove(name);
            }
            features.Set(new TestConnectionFeature(name));
        })).ToArray();
        Task[] readers = Enumerable.Range(0, 4).Select(_ => Task.Run(() =>
        {
            start.Wait();
            for (int index = 0; index < 1000; index++)
            {
                features.Get("Writer0");
                features.Get<TestConnectionFeature>();
                string[] names = features.Select(feature => feature.Name).ToArray();
                names.Distinct(StringComparer.Ordinal).Count().ShouldBe(names.Length);
            }
        })).ToArray();

        // Act
        start.Set();
        await Task.WhenAll(writers.Concat(readers));

        // Assert
        features.Count().ShouldBe(5);
        for (int writer = 0; writer < 4; writer++)
        {
            features.Get("Writer" + writer).ShouldNotBeNull();
        }
    }

    private sealed class OtherFeature : IConnectionFeature
    {
        public OtherFeature(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }

    private sealed class CustomCollection : ConnectionFeatureCollection, IEnumerable<IConnectionFeature>
    {
        private readonly IConnectionFeature[] _effective;

        public CustomCollection(params IConnectionFeature[] effective)
        {
            _effective = effective;
            foreach (IConnectionFeature feature in effective.Reverse())
            {
                Set(feature);
            }
        }

        IEnumerator<IConnectionFeature> IEnumerable<IConnectionFeature>.GetEnumerator()
            => ((IEnumerable<IConnectionFeature>)_effective).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _effective.GetEnumerator();
    }
}
```

### Walkthrough

- **Covered behavior** — Get: Wrapped features pass through by name and type.
- **Covered behavior** — Set: Replaces the named slot regardless of feature type.
- **Covered behavior** — Remove: Hides a wrapped feature without changing the wrapped collection.
- **Covered behavior** — Remove: Removing a replacement also hides its wrapped slot.
- **Covered behavior** — Remove: A hidden name stays hidden when the inner collection changes.
- **Covered behavior** — Get: Own features precede different-named wrapped features of the same type.
- **Covered behavior** — GetEnumerator: Returns own features then only effective wrapped features.
- **Covered behavior** — GetEnumerator: In-flight enumeration retains its local snapshot.
- **Covered behavior** — Set: Rejects null features and empty names.
- **Covered behavior** — Named access: Rejects null and empty names. Inline data: `(null)`, `("")`.
- **Covered behavior** — Get: Rejects a null collection.
- **Covered behavior** — Get: Respects custom collection enumeration and outer hiding.
- **Covered behavior** — Get: Layered typed lookups allocate no memory.
- **Covered behavior** — Features: Concurrent mutations retain independent registrations and safe reads.

The assertions define the expected result or failure boundary. Test-only doubles supply controlled
inputs; they are not additional shipped APIs.

## Connection Features Tests

### Code

```csharp
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Shouldly;
using Xunit;

namespace Assimalign.Cohesion.Connections.Tests;

public class ConnectionFeaturesTests
{
    [Theory(DisplayName = "Cohesion Test [Connections] - Features: All guided bases defer collection creation until first access")]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void Features_BeforeFirstAccess_ShouldNotReadInnerOrConfigure(int shape)
    {
        // Arrange
        var probe = new FeatureConnectionProbe();

        // Act
        (object connection, Func<IConnectionFeatureCollection> getFeatures) = Create(shape, probe);

        // Assert
        probe.InnerReads.ShouldBe(0);
        probe.ConfigurationCount.ShouldBe(0);
        (connection is IConnectionFeatureCollection).ShouldBeFalse();
        IConnectionFeatureCollection features = getFeatures();
        features.ShouldBeOfType<ConnectionFeatureCollection>();
        getFeatures().ShouldBeSameAs(features);
        probe.InnerReads.ShouldBe(1);
        probe.ConfigurationCount.ShouldBe(1);
    }

    [Theory(DisplayName = "Cohesion Test [Connections] - Features: All guided bases configure their published collection over InnerFeatures")]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void Features_WithInnerAndOwnRegistrations_ShouldPublishConfiguredLayer(int shape)
    {
        // Arrange
        var inherited = new TestConnectionFeature("Inherited");
        var own = new TestConnectionFeature("Own");
        var inner = new ConnectionFeatureCollection();
        inner.Set(inherited);
        IConnectionFeatureCollection? configured = null;
        var probe = new FeatureConnectionProbe
        {
            Inner = inner,
            Configure = features =>
            {
                configured = features;
                features.Set(own);
            }
        };
        (_, Func<IConnectionFeatureCollection> getFeatures) = Create(shape, probe);

        // Act
        IConnectionFeatureCollection features = getFeatures();

        // Assert
        features.ShouldBeSameAs(configured);
        features.Get("Inherited").ShouldBeSameAs(inherited);
        features.Get<TestConnectionFeature>().ShouldBeSameAs(own);
        features.ToArray().ShouldBe(new IConnectionFeature[] { own, inherited });
    }

    [Theory(DisplayName = "Cohesion Test [Connections] - Features: Concurrent first access configures once and waits for the completed collection")]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public async Task Features_WithConcurrentFirstAccess_ShouldConfigureOnceBeforePublication(int shape)
    {
        // Arrange
        using var ready = new CountdownEvent(8);
        using var start = new ManualResetEventSlim();
        using var configuring = new ManualResetEventSlim();
        using var release = new ManualResetEventSlim();
        var own = new TestConnectionFeature("Own");
        var probe = new FeatureConnectionProbe
        {
            Configure = features =>
            {
                configuring.Set();
                if (!release.Wait(TimeSpan.FromSeconds(10)))
                {
                    throw new TimeoutException("The test did not release feature configuration.");
                }
                features.Set(own);
            }
        };
        (_, Func<IConnectionFeatureCollection> getFeatures) = Create(shape, probe);
        Task<IConnectionFeatureCollection>[] readers = Enumerable.Range(0, 8).Select(_ => Task.Factory.StartNew(() =>
        {
            ready.Signal();
            if (!start.Wait(TimeSpan.FromSeconds(10)))
            {
                throw new TimeoutException("The test did not start feature readers.");
            }
            IConnectionFeatureCollection features = getFeatures();
            features.Get("Own").ShouldBeSameAs(own);
            return features;
        }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default)).ToArray();

        // Act
        try
        {
            ready.Wait(TimeSpan.FromSeconds(10)).ShouldBeTrue();
            start.Set();
            configuring.Wait(TimeSpan.FromSeconds(10)).ShouldBeTrue();
            readers.All(reader => !reader.IsCompleted).ShouldBeTrue();
        }
        finally
        {
            start.Set();
            release.Set();
        }
        IConnectionFeatureCollection[] results = await Task.WhenAll(readers);

        // Assert
        probe.ConfigurationCount.ShouldBe(1);
        probe.InnerReads.ShouldBe(1);
        results.All(features => ReferenceEquals(features, results[0])).ShouldBeTrue();
        getFeatures().ShouldBeSameAs(results[0]);
    }

    [Theory(DisplayName = "Cohesion Test [Connections] - Features: Failed configuration is not invoked again or partially published")]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void Features_WithFailedConfiguration_ShouldRetainFailure(int shape)
    {
        // Arrange
        var failure = new InvalidOperationException("Feature configuration failed.");
        var probe = new FeatureConnectionProbe
        {
            Configure = features =>
            {
                features.Set(new TestConnectionFeature("Partial"));
                throw failure;
            }
        };
        (_, Func<IConnectionFeatureCollection> getFeatures) = Create(shape, probe);

        // Act and assert
        Should.Throw<InvalidOperationException>(() => getFeatures()).ShouldBeSameAs(failure);
        Should.Throw<InvalidOperationException>(() => getFeatures()).ShouldBeSameAs(failure);
        probe.ConfigurationCount.ShouldBe(1);
    }

    private static (object DuplexConnection, Func<IConnectionFeatureCollection> GetFeatures) Create(int shape, FeatureConnectionProbe probe)
    {
        switch (shape)
        {
            case 0:
                IDuplexConnection stream = new FeatureProbeConnection(probe);
                return (stream, () => stream.Features);
            case 1:
                IMultiplexedConnection multiplexed = new FeatureProbeMultiplexedConnection(probe);
                return (multiplexed, () => multiplexed.Features);
            default:
                IDatagramConnection datagram = new FeatureProbeDatagramConnection(probe);
                return (datagram, () => datagram.Features);
        }
    }
}
```

### Walkthrough

- **Covered behavior** — Features: All guided bases defer collection creation until first access. Inline data: `(0)`, `(1)`, `(2)`.
- **Covered behavior** — Features: All guided bases configure their published collection over InnerFeatures. Inline data: `(0)`, `(1)`, `(2)`.
- **Covered behavior** — Features: Concurrent first access configures once and waits for the completed collection. Inline data: `(0)`, `(1)`, `(2)`.
- **Covered behavior** — Features: Failed configuration is not invoked again or partially published. Inline data: `(0)`, `(1)`, `(2)`.

The assertions define the expected result or failure boundary. Test-only doubles supply controlled
inputs; they are not additional shipped APIs.

## Guided Base Tests

### Code

```csharp
using System;
using System.IO.Pipelines;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using Shouldly;

using Xunit;

namespace Assimalign.Cohesion.Connections.Tests;

public class GuidedBaseTests
{
    private static readonly EndPoint _testEndPoint = new IPEndPoint(IPAddress.Loopback, 16000);

    [Fact(DisplayName = "Cohesion Test [Connections] - BindAsync: Guided listener base should forward through interface calls")]
    public async Task BindAsync_OnConnectionListenerBase_ShouldForwardThroughInterfaceCall()
    {
        // Arrange
        TestConnectionListener listener = new();
        IDuplexConnectionListener viaInterface = listener;

        // Act
        await listener.BindAsync();
        await viaInterface.BindAsync();

        // Assert
        listener.BindCount.ShouldBe(2);
    }

    [Fact]
    public async Task AcceptAsync_OnConnectionListenerBase_ShouldReturnSameInstanceThroughTypedAndInterfaceCalls()
    {
        // Arrange
        TestConnection first = new();
        TestConnection second = new();
        TestConnectionListener listener = new();
        listener.Enqueue(first);
        listener.Enqueue(second);

        // Act
        IDuplexConnection typed = await listener.AcceptAsync();
        IConnection viaInterface = await ((IConnectionListener)listener).AcceptAsync();

        // Assert
        typed.ShouldBeSameAs(first);
        viaInterface.ShouldBeSameAs(second);
    }

    [Fact]
    public async Task ConnectAsync_OnConnectionFactoryBase_ShouldReturnSameInstanceThroughTypedAndInterfaceCalls()
    {
        // Arrange
        TestConnection first = new();
        TestConnection second = new();
        TestConnectionFactory factory = new();
        factory.Enqueue(first);
        factory.Enqueue(second);

        // Act
        IDuplexConnection typed = await factory.ConnectAsync(_testEndPoint);
        IConnection viaInterface = await ((IConnectionFactory)factory).ConnectAsync(_testEndPoint);

        // Assert
        typed.ShouldBeSameAs(first);
        viaInterface.ShouldBeSameAs(second);
    }

    [Fact(DisplayName = "Cohesion Test [Connections] - OpenStreamAsync: Guided multiplexed base should default to bidirectional")]
    public async Task OpenStreamAsync_OnMultiplexedConnectionBase_ShouldDefaultToBidirectional()
    {
        // Arrange
        TestConnection stream = new();
        TestMultiplexedConnection connection = new();
        connection.Enqueue(stream);

        // Act
        MultiplexedConnectionStream opened = await connection.OpenStreamAsync();

        // Assert
        connection.LastOpenedDirection.ShouldBe(ConnectionDirection.Bidirectional);
        opened.ShouldBeSameAs(stream);
    }

    [Fact(DisplayName = "Cohesion Test [Connections] - OpenStreamAsync: Guided multiplexed base should return the stream through its interface")]
    public async Task OpenStreamAsync_OnMultiplexedConnectionInterface_ShouldForwardToTypedImplementation()
    {
        // Arrange
        TestConnection stream = new();
        TestMultiplexedConnection connection = new();
        connection.Enqueue(stream);
        IMultiplexedConnection viaInterface = connection;

        // Act
        IMultiplexedConnectionStream opened = await viaInterface.OpenStreamAsync();

        // Assert
        connection.LastOpenedDirection.ShouldBe(ConnectionDirection.Bidirectional);
        opened.ShouldBeSameAs(stream);
    }

    [Fact(DisplayName = "Cohesion Test [Connections] - AcceptStreamAsync: Guided multiplexed base should return the stream through its interface")]
    public async Task AcceptStreamAsync_OnMultiplexedConnectionInterface_ShouldForwardToTypedImplementation()
    {
        // Arrange
        TestConnection stream = new();
        TestMultiplexedConnection connection = new();
        connection.Enqueue(stream);
        IMultiplexedConnection viaInterface = connection;

        // Act
        IMultiplexedConnectionStream accepted = await viaInterface.AcceptStreamAsync();

        // Assert
        accepted.ShouldBeSameAs(stream);
    }

    [Fact]
    public void Direction_OnConnectionBase_ShouldDefaultToBidirectional()
    {
        // Arrange
        MinimalConnection connection = new();

        // Act
        ConnectionDirection direction = connection.Direction;

        // Assert
        direction.ShouldBe(ConnectionDirection.Bidirectional);
    }

    [Fact(DisplayName = "Cohesion Test [Connections] - Abort: Guided multiplexed connection exposes its coded lifecycle through the interface")]
    public async Task Abort_OnMultiplexedConnectionInterface_ShouldEndConnectionAndStreams()
    {
        // Arrange
        await using TestConnection stream = new();
        await using TestMultiplexedConnection connection = new();
        connection.Enqueue(stream);
        IMultiplexedConnection viaInterface = connection;

        // Act
        viaInterface.Abort(42);

        // Assert
        connection.LastAbortCode.ShouldBe(42);
        stream.LastAbortCode.ShouldBe(42);
        viaInterface.State.ShouldBe(ConnectionState.Aborted);
        viaInterface.ConnectionClosed.IsCancellationRequested.ShouldBeTrue();
        stream.ConnectionClosed.IsCancellationRequested.ShouldBeTrue();
    }

    [Fact(DisplayName = "Cohesion Test [Connections] - Abort: Guided multiplexed stream exposes its coded lifecycle through the interface")]
    public async Task Abort_OnMultiplexedStreamInterface_ShouldEndStream()
    {
        // Arrange
        await using TestConnection stream = new();
        IMultiplexedConnectionStream viaInterface = stream;

        // Act
        viaInterface.Abort(42);

        // Assert
        stream.LastAbortCode.ShouldBe(42);
        viaInterface.State.ShouldBe(ConnectionState.Aborted);
        viaInterface.ConnectionClosed.IsCancellationRequested.ShouldBeTrue();
    }

    [Fact(DisplayName = "Cohesion Test [Connections] - IsInputCompleted: Guided multiplexed stream exposes conservative completion through the interface")]
    public async Task IsInputCompleted_OnMultiplexedStreamInterface_ShouldReturnImplementationValue()
    {
        // Arrange
        await using TestConnection stream = new();
        IMultiplexedConnectionStream viaInterface = stream;

        // Act
        bool completed = viaInterface.IsInputCompleted;

        // Assert
        completed.ShouldBeFalse();
        viaInterface.Direction.ShouldBe(ConnectionDirection.Bidirectional);
        viaInterface.Features.ShouldBeSameAs(stream.Features);
    }

    /// <summary>
    /// A minimal <see cref="DuplexConnection"/> that does not override <see cref="DuplexConnection.Direction"/>,
    /// proving the guided base supplies the bidirectional default.
    /// </summary>
    private sealed class MinimalConnection : DuplexConnection
    {
        private readonly Pipe _pipe = new();

        public override ConnectionId Id => ConnectionId.Empty;

        public override EndPoint? LocalEndPoint => null;

        public override EndPoint? RemoteEndPoint => null;

        public override PipeReader Input => _pipe.Reader;

        public override PipeWriter Output => _pipe.Writer;

        public override ConnectionCapabilities Capabilities => TestConnection.DefaultCapabilities;

        public override ConnectionState State => ConnectionState.Open;

        public override CancellationToken ConnectionClosed => CancellationToken.None;

        public override void Abort(Exception? reason = null)
        {
        }

        public override ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}
```

### Walkthrough

- **Covered behavior** — BindAsync: Guided listener base should forward through interface calls.
- **Test entry point** — `AcceptAsync_OnConnectionListenerBase_ShouldReturnSameInstanceThroughTypedAndInterfaceCalls` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `ConnectAsync_OnConnectionFactoryBase_ShouldReturnSameInstanceThroughTypedAndInterfaceCalls` contains the setup, invocation, and assertions for this case.
- **Covered behavior** — OpenStreamAsync: Guided multiplexed base should default to bidirectional.
- **Covered behavior** — OpenStreamAsync: Guided multiplexed base should return the stream through its interface.
- **Covered behavior** — AcceptStreamAsync: Guided multiplexed base should return the stream through its interface.
- **Test entry point** — `Direction_OnConnectionBase_ShouldDefaultToBidirectional` contains the setup, invocation, and assertions for this case.
- **Covered behavior** — Abort: Guided multiplexed connection exposes its coded lifecycle through the interface.
- **Covered behavior** — Abort: Guided multiplexed stream exposes its coded lifecycle through the interface.
- **Covered behavior** — IsInputCompleted: Guided multiplexed stream exposes conservative completion through the interface.

The assertions define the expected result or failure boundary. Test-only doubles supply controlled
inputs; they are not additional shipped APIs.

## Connection Listener Tests

### Code

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

using Shouldly;
using Xunit;

namespace Assimalign.Cohesion.Connections.Tests;

public class ConnectionListenerTests
{
    [Fact(DisplayName = "Cohesion Test [Connections] - AcceptAsync: Typed and root accepts drain one queue and expose one live set")]
    public async Task AcceptAsync_TypedAndRootCalls_ShouldDrainOneQueueAndShareCollection()
    {
        // Arrange
        await using var listener = new QueueListener();
        await using var first = new RootConnection();
        await using var second = new RootConnection();
        listener.Enqueue(first);
        listener.Enqueue(second);
        IConnectionListener root = listener;
        IConnectionListener<RootConnection> typed = listener;

        // Act
        RootConnection acceptedFirst = await typed.AcceptAsync(CancellationToken.None);
        IConnection acceptedSecond = await root.AcceptAsync(CancellationToken.None);

        // Assert
        acceptedFirst.ShouldBeSameAs(first);
        acceptedSecond.ShouldBeSameAs(second);
        root.Connections.ShouldBeSameAs(typed.Connections);
        root.Connections.Count.ShouldBe(2);
        root.Connections.ShouldContain(first);
        root.Connections.ShouldContain(second);
    }

    [Theory(DisplayName = "Cohesion Test [Connections] - Connections: Every lifetime end removes the accepted connection")]
    [InlineData("close")]
    [InlineData("abort")]
    [InlineData("dispose")]
    [InlineData("peer close")]
    public async Task Connections_LifetimeEnds_ShouldRemoveEntry(string ending)
    {
        // Arrange
        await using var listener = new QueueListener();
        await using var connection = new RootConnection();
        listener.Enqueue(connection);
        await listener.AcceptAsync(CancellationToken.None);
        listener.Connections.Count.ShouldBe(1);

        // Act
        switch (ending)
        {
            case "abort": connection.Abort(); break;
            case "dispose": await connection.DisposeAsync(); break;
            case "peer close": connection.PeerClose(); break;
            default: connection.Close(); break;
        }

        // Assert
        listener.Connections.Count.ShouldBe(0);
        listener.Connections.ShouldBeEmpty();
    }

    [Fact(DisplayName = "Cohesion Test [Connections] - AcceptAsync: A connection closed before hand-out never remains listed")]
    public async Task AcceptAsync_AlreadyClosed_ShouldNotLeaveEntry()
    {
        // Arrange
        await using var listener = new QueueListener();
        await using var connection = new RootConnection();
        connection.Close();
        listener.Enqueue(connection);

        // Act
        IConnection accepted = await ((IConnectionListener)listener).AcceptAsync(CancellationToken.None);

        // Assert
        accepted.ShouldBeSameAs(connection);
        listener.Connections.Count.ShouldBe(0);
        listener.Connections.ShouldBeEmpty();
    }

    [Fact(DisplayName = "Cohesion Test [Connections] - Connections: Enumeration tolerates concurrent accepts and closures")]
    public async Task Connections_ConcurrentChurn_ShouldEnumerateAndFinishWithExactCount()
    {
        // Arrange
        await using var listener = new QueueListener();
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(20));
        using var finished = new CancellationTokenSource();
        var enumerating = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        Task observer = Task.Run(() =>
        {
            enumerating.SetResult();
            while (!finished.IsCancellationRequested)
            {
                foreach (RootConnection connection in listener.Connections)
                {
                    connection.ShouldNotBeNull();
                }
                listener.Connections.Count.ShouldBeGreaterThanOrEqualTo(0);
            }
        }, timeout.Token);
        await enumerating.Task.WaitAsync(timeout.Token);

        // Act
        try
        {
            await Task.WhenAll(Enumerable.Range(0, 4).Select(_ => Task.Run(async () =>
            {
                for (int i = 0; i < 250; i++)
                {
                    listener.Enqueue(new RootConnection());
                    RootConnection connection = await listener.AcceptAsync(timeout.Token);
                    await connection.DisposeAsync();
                }
            }, timeout.Token)));
        }
        finally
        {
            finished.Cancel();
            await observer.WaitAsync(timeout.Token);
        }

        // Assert
        listener.Connections.Count.ShouldBe(0);
        listener.Connections.ShouldBeEmpty();
    }

    [Fact(DisplayName = "Cohesion Test [Connections] - DisposeAsync: Listener disposal preserves handed-out connections")]
    public async Task DisposeAsync_AcceptedConnection_ShouldKeepOwnerLifetime()
    {
        // Arrange
        var listener = new QueueListener();
        await using var connection = new RootConnection();
        listener.Enqueue(connection);
        await listener.AcceptAsync(CancellationToken.None);

        // Act
        await listener.DisposeAsync();

        // Assert
        connection.ConnectionClosed.IsCancellationRequested.ShouldBeFalse();
        listener.Connections.Count.ShouldBe(1);
        await connection.DisposeAsync();
        listener.Connections.Count.ShouldBe(0);
    }

    [Fact(DisplayName = "Cohesion Test [Connections] - Id: Layered listener retains its bound endpoint identity")]
    public async Task Id_LayeredListener_ShouldMatchInnerListener()
    {
        // Arrange
        var inner = new TestConnectionListener();
        await using IDuplexConnectionListener layered = inner.Use(new RecordingConnectionLayer("identity", new List<string>()));

        // Act
        ListenerId id = layered.Id;

        // Assert
        id.ShouldBe(inner.Id);
        id.ShouldNotBe(ListenerId.Empty);
    }

    private sealed class QueueListener : ConnectionListener<RootConnection>
    {
        private readonly Channel<RootConnection> _queue = Channel.CreateUnbounded<RootConnection>();
        public override ListenerId Id { get; } = ListenerId.New();
        public void Enqueue(RootConnection connection) => _queue.Writer.TryWrite(connection);
        protected override ValueTask<RootConnection> AcceptCoreAsync(CancellationToken cancellationToken = default)
            => _queue.Reader.ReadAsync(cancellationToken);
        public override ValueTask DisposeAsync()
        {
            _queue.Writer.TryComplete();
            return ValueTask.CompletedTask;
        }
    }

    private sealed class RootConnection : Connection
    {
        private readonly CancellationTokenSource _closed = new();
        public override ConnectionId Id => ConnectionId.Empty;
        public override EndPoint? LocalEndPoint => null;
        public override EndPoint? RemoteEndPoint => null;
        public override ConnectionCapabilities Capabilities => default;
        public override ConnectionState State => _closed.IsCancellationRequested ? ConnectionState.Closed : ConnectionState.Open;
        public override CancellationToken ConnectionClosed => _closed.Token;
        public override void Abort(Exception? reason = null) => _closed.Cancel();
        public void Close() => _closed.Cancel();
        public void PeerClose() => _closed.Cancel();
        public override ValueTask DisposeAsync()
        {
            _closed.Cancel();
            return ValueTask.CompletedTask;
        }
    }
}
```

### Walkthrough

- **Covered behavior** — AcceptAsync: Typed and root accepts drain one queue and expose one live set.
- **Covered behavior** — Connections: Every lifetime end removes the accepted connection. Inline data: `("close")`, `("abort")`, `("dispose")`, `("peer close")`.
- **Covered behavior** — AcceptAsync: A connection closed before hand-out never remains listed.
- **Covered behavior** — Connections: Enumeration tolerates concurrent accepts and closures.
- **Covered behavior** — DisposeAsync: Listener disposal preserves handed-out connections.
- **Covered behavior** — Id: Layered listener retains its bound endpoint identity.

The assertions define the expected result or failure boundary. Test-only doubles supply controlled
inputs; they are not additional shipped APIs.

## Connection Layer Tests

### Code

```csharp
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

using Shouldly;

using Xunit;

namespace Assimalign.Cohesion.Connections.Tests;

public class ConnectionLayerTests
{
    private static readonly EndPoint _testEndPoint = new IPEndPoint(IPAddress.Loopback, 16000);

    [Fact]
    public async Task Use_OnListener_ShouldYieldLayerUpgradedConnection()
    {
        // Arrange
        TestConnection inner = new();
        TestConnectionListener listener = new();
        listener.Enqueue(inner);
        RecordingConnectionLayer layer = new("tls", []);

        // Act
        IDuplexConnectionListener layered = listener.Use(layer);
        IDuplexConnection accepted = await layered.AcceptAsync();

        // Assert
        LayerWrappedConnection wrapped = accepted.ShouldBeOfType<LayerWrappedConnection>();
        wrapped.LayerName.ShouldBe("tls");
        wrapped.Inner.ShouldBeSameAs(inner);
    }

    [Fact]
    public async Task Use_OnListenerWithTwoLayers_ShouldApplyLayersInRegistrationOrder()
    {
        // Arrange
        TestConnection inner = new();
        TestConnectionListener listener = new();
        listener.Enqueue(inner);
        List<string> upgrades = [];
        RecordingConnectionLayer first = new("first", upgrades);
        RecordingConnectionLayer second = new("second", upgrades);

        // Act
        IDuplexConnection accepted = await listener.Use(first).Use(second).AcceptAsync();

        // Assert
        upgrades.ShouldBe(["first", "second"]);

        // The outermost wrapper belongs to the layer registered last.
        LayerWrappedConnection outer = accepted.ShouldBeOfType<LayerWrappedConnection>();
        outer.LayerName.ShouldBe("second");
        LayerWrappedConnection innerWrapper = outer.Inner.ShouldBeOfType<LayerWrappedConnection>();
        innerWrapper.LayerName.ShouldBe("first");
        innerWrapper.Inner.ShouldBeSameAs(inner);
    }

    [Fact]
    public void Capabilities_OnLayeredListener_ShouldReflectLayerDescribe()
    {
        // Arrange
        TestConnectionListener listener = new();
        RecordingConnectionLayer layer = new(
            "tls",
            [],
            describe: static capabilities => capabilities with { Security = ConnectionSecurity.Tls });

        // Act
        IDuplexConnectionListener layered = listener.Use(layer);

        // Assert
        layered.Capabilities.ShouldBe(listener.Capabilities with { Security = ConnectionSecurity.Tls });
    }

    [Fact]
    public async Task Use_OnFactory_ShouldYieldLayerUpgradedConnection()
    {
        // Arrange
        TestConnection inner = new();
        TestConnectionFactory factory = new();
        factory.Enqueue(inner);
        RecordingConnectionLayer layer = new("tls", []);

        // Act
        IDuplexConnectionFactory layered = factory.Use(layer);
        IDuplexConnection connected = await layered.ConnectAsync(_testEndPoint);

        // Assert
        LayerWrappedConnection wrapped = connected.ShouldBeOfType<LayerWrappedConnection>();
        wrapped.Inner.ShouldBeSameAs(inner);
        factory.LastEndPoint.ShouldBeSameAs(_testEndPoint);
    }

    [Fact]
    public async Task Use_OnFactoryWithTwoLayers_ShouldApplyLayersInRegistrationOrder()
    {
        // Arrange
        TestConnection inner = new();
        TestConnectionFactory factory = new();
        factory.Enqueue(inner);
        List<string> upgrades = [];
        RecordingConnectionLayer first = new("first", upgrades);
        RecordingConnectionLayer second = new("second", upgrades);

        // Act
        IDuplexConnection connected = await factory.Use(first).Use(second).ConnectAsync(_testEndPoint);

        // Assert
        upgrades.ShouldBe(["first", "second"]);
        LayerWrappedConnection outer = connected.ShouldBeOfType<LayerWrappedConnection>();
        outer.LayerName.ShouldBe("second");
        LayerWrappedConnection innerWrapper = outer.Inner.ShouldBeOfType<LayerWrappedConnection>();
        innerWrapper.LayerName.ShouldBe("first");
        innerWrapper.Inner.ShouldBeSameAs(inner);
    }

    [Fact(DisplayName = "Cohesion Test [Connections] - ConnectAsync: A failed upgrade should dispose the dialed connection")]
    public async Task ConnectAsync_WhenTheUpgradeFails_ShouldDisposeTheDialedConnectionAndRethrow()
    {
        // Arrange
        TestConnection dialed = new();
        TestConnectionFactory factory = new();
        factory.Enqueue(dialed);
        IOException failure = new("The server's certificate was refused.");
        ControlledConnectionLayer layer = new();
        layer.Fail(dialed, failure);
        IDuplexConnectionFactory layered = factory.Use(layer);

        // Act
        Exception? exception = await Record.ExceptionAsync(async () => await layered.ConnectAsync(_testEndPoint));

        // Assert
        exception.ShouldBeSameAs(failure);
        dialed.ConnectionClosed.IsCancellationRequested.ShouldBeTrue();
    }

    [Fact]
    public void Capabilities_OnLayeredFactory_ShouldReflectLayerDescribe()
    {
        // Arrange
        TestConnectionFactory factory = new();
        RecordingConnectionLayer layer = new(
            "tls",
            [],
            describe: static capabilities => capabilities with { Security = ConnectionSecurity.Tls });

        // Act
        IDuplexConnectionFactory layered = factory.Use(layer);

        // Assert
        layered.Capabilities.ShouldBe(factory.Capabilities with { Security = ConnectionSecurity.Tls });
    }

    [Fact]
    public async Task Use_WithPassThroughLayer_ShouldReturnInnerConnectionInstance()
    {
        // Arrange
        TestConnection inner = new();
        TestConnectionListener listener = new();
        listener.Enqueue(inner);
        RecordingConnectionLayer passThrough = new("noop", [], wrapConnection: false);

        // Act
        IDuplexConnection accepted = await listener.Use(passThrough).AcceptAsync();

        // Assert
        accepted.ShouldBeSameAs(inner);
    }

    [Fact]
    public async Task Use_OnListener_ShouldDelegateEndPointAndDisposeToInner()
    {
        // Arrange
        TestConnectionListener listener = new();
        RecordingConnectionLayer layer = new("noop", [], wrapConnection: false);
        IDuplexConnectionListener layered = listener.Use(layer);

        // Act
        await layered.DisposeAsync();

        // Assert
        layered.EndPoint.ShouldBeSameAs(listener.EndPoint);
        listener.IsDisposed.ShouldBeTrue();
    }

    [Fact(DisplayName = "Cohesion Test [Connections] - BindAsync: Layered listener should delegate to inner listener")]
    public async Task BindAsync_OnLayeredListener_ShouldDelegateToInnerListener()
    {
        // Arrange
        TestConnectionListener listener = new();
        RecordingConnectionLayer layer = new("noop", [], wrapConnection: false);
        IDuplexConnectionListener layered = listener.Use(layer);

        // Act
        await layered.BindAsync();

        // Assert
        listener.BindCount.ShouldBe(1);
    }

    [Fact]
    public void Use_WithNullLayer_ShouldThrowArgumentNullException()
    {
        // Arrange
        TestConnectionListener listener = new();
        TestConnectionFactory factory = new();

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => listener.Use(null!));
        Should.Throw<ArgumentNullException>(() => factory.Use(null!));
    }
}
```

### Walkthrough

- **Test entry point** — `Use_OnListener_ShouldYieldLayerUpgradedConnection` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `Use_OnListenerWithTwoLayers_ShouldApplyLayersInRegistrationOrder` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `Capabilities_OnLayeredListener_ShouldReflectLayerDescribe` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `Use_OnFactory_ShouldYieldLayerUpgradedConnection` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `Use_OnFactoryWithTwoLayers_ShouldApplyLayersInRegistrationOrder` contains the setup, invocation, and assertions for this case.
- **Covered behavior** — ConnectAsync: A failed upgrade should dispose the dialed connection.
- **Test entry point** — `Capabilities_OnLayeredFactory_ShouldReflectLayerDescribe` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `Use_WithPassThroughLayer_ShouldReturnInnerConnectionInstance` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `Use_OnListener_ShouldDelegateEndPointAndDisposeToInner` contains the setup, invocation, and assertions for this case.
- **Covered behavior** — BindAsync: Layered listener should delegate to inner listener.
- **Test entry point** — `Use_WithNullLayer_ShouldThrowArgumentNullException` contains the setup, invocation, and assertions for this case.

The assertions define the expected result or failure boundary. Test-only doubles supply controlled
inputs; they are not additional shipped APIs.

## Layered Connection Listener Tests

### Code

```csharp
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Shouldly;

using Xunit;

namespace Assimalign.Cohesion.Connections.Tests;

/// <summary>
/// Covers how a layered listener (<c>listener.Use(layer)</c>) runs upgrades (#1304): each connection's
/// upgrade runs on its own task, a failed or stalled upgrade affects only its connection, the number of
/// connections held at once is bounded, and disposal releases every connection not yet returned.
/// </summary>
public class LayeredConnectionListenerTests
{
    // A hang guard, never a budget: every wait completes as soon as the listener does its part.
    private static readonly TimeSpan _testTimeout = TimeSpan.FromSeconds(10);

    [Fact(DisplayName = "Cohesion Test [Connections] - AcceptAsync: A stalled upgrade should not delay another connection")]
    public async Task AcceptAsync_WhileAnotherUpgradeStalls_ShouldReturnTheUpgradedConnection()
    {
        // Arrange
        using CancellationTokenSource timeout = new(_testTimeout);
        TestConnection stalled = new();
        TestConnection ready = new();
        ControlledConnectionLayer layer = new();
        layer.Hold(stalled);
        BlockingConnectionListener inner = new();
        inner.Enqueue(stalled);
        inner.Enqueue(ready);
        await using IDuplexConnectionListener listener = inner.Use(layer);

        // Act
        IDuplexConnection accepted = await listener.AcceptAsync(timeout.Token);

        // Assert
        accepted.ShouldBeSameAs(ready);
        stalled.ConnectionClosed.IsCancellationRequested.ShouldBeFalse();
    }

    [Fact(DisplayName = "Cohesion Test [Connections] - AcceptAsync: A failed upgrade should close its connection and keep accepting")]
    public async Task AcceptAsync_WhenAnUpgradeFails_ShouldDisposeThatConnectionAndReturnTheNext()
    {
        // Arrange
        using CancellationTokenSource timeout = new(_testTimeout);
        TestConnection rejected = new();
        TestConnection ready = new();
        ControlledConnectionLayer layer = new();
        layer.Fail(rejected, new IOException("Not a handshake."));
        BlockingConnectionListener inner = new();
        inner.Enqueue(rejected);
        inner.Enqueue(ready);
        await using IDuplexConnectionListener listener = inner.Use(layer);

        // Act
        IDuplexConnection accepted = await listener.AcceptAsync(timeout.Token);

        // Assert
        accepted.ShouldBeSameAs(ready);
        await WhenClosedAsync(rejected, timeout.Token);
    }

    [Fact(DisplayName = "Cohesion Test [Connections] - AcceptAsync: Canceling a wait should leave the upgrade running")]
    public async Task AcceptAsync_WhenTheWaitIsCanceled_ShouldReturnTheConnectionToALaterCall()
    {
        // Arrange
        using CancellationTokenSource timeout = new(_testTimeout);
        using CancellationTokenSource abandon = new();
        TestConnection upgrading = new();
        ControlledConnectionLayer layer = new();
        layer.Hold(upgrading);
        BlockingConnectionListener inner = new();
        inner.Enqueue(upgrading);
        await using IDuplexConnectionListener listener = inner.Use(layer);
        Task<IDuplexConnection> abandoned = listener.AcceptAsync(abandon.Token).AsTask();
        await layer.WhenStarted(upgrading).WaitAsync(timeout.Token);

        // Act
        abandon.Cancel();
        await Should.ThrowAsync<OperationCanceledException>(() => abandoned);
        layer.Release(upgrading);
        IDuplexConnection accepted = await listener.AcceptAsync(timeout.Token);

        // Assert
        accepted.ShouldBeSameAs(upgrading);
        upgrading.ConnectionClosed.IsCancellationRequested.ShouldBeFalse();
    }

    [Fact(DisplayName = "Cohesion Test [Connections] - AcceptAsync: Should stop accepting from the inner listener at the upgrade limit")]
    public async Task AcceptAsync_AtTheUpgradeLimit_ShouldNotAcceptUntilASlotFrees()
    {
        // Arrange
        using CancellationTokenSource timeout = new(_testTimeout);
        TestConnection first = new();
        TestConnection second = new();
        ControlledConnectionLayer layer = new();
        layer.Hold(first);
        BlockingConnectionListener inner = new();
        inner.Enqueue(first);
        inner.Enqueue(second);
        await using IDuplexConnectionListener listener = inner.Use(layer, maxConcurrentUpgrades: 1);
        Task<IDuplexConnection> firstAccept = listener.AcceptAsync(timeout.Token).AsTask();
        await layer.WhenStarted(first).WaitAsync(timeout.Token);

        // Act — the one slot is held by the stalled upgrade. The pause gives a listener that ignored the
        // limit time to take the queued connection; it cannot make a correct listener fail.
        await Task.Delay(TimeSpan.FromMilliseconds(200), timeout.Token);
        int acceptsWhileFull = inner.AcceptCalls;
        bool secondStartedWhileFull = layer.WhenStarted(second).IsCompleted;
        layer.Release(first);
        IDuplexConnection firstAccepted = await firstAccept;
        IDuplexConnection secondAccepted = await listener.AcceptAsync(timeout.Token);

        // Assert
        acceptsWhileFull.ShouldBe(1);
        secondStartedWhileFull.ShouldBeFalse();
        firstAccepted.ShouldBeSameAs(first);
        secondAccepted.ShouldBeSameAs(second);
    }

    [Fact(DisplayName = "Cohesion Test [Connections] - AcceptAsync: Should return upgrading connections before the inner listener's failure")]
    public async Task AcceptAsync_WhenTheInnerListenerFaults_ShouldReturnUpgradingConnectionsThenRethrow()
    {
        // Arrange
        using CancellationTokenSource timeout = new(_testTimeout);
        InvalidOperationException failure = new("The endpoint failed.");
        TestConnection upgrading = new();
        ControlledConnectionLayer layer = new();
        layer.Hold(upgrading);
        BlockingConnectionListener inner = new();
        inner.Enqueue(upgrading);
        await using IDuplexConnectionListener listener = inner.Use(layer);
        Task<IDuplexConnection> firstAccept = listener.AcceptAsync(timeout.Token).AsTask();
        await layer.WhenStarted(upgrading).WaitAsync(timeout.Token);

        // Act
        inner.Fault(failure);
        layer.Release(upgrading);
        IDuplexConnection accepted = await firstAccept;
        InvalidOperationException observed = await Should.ThrowAsync<InvalidOperationException>(
            () => listener.AcceptAsync(timeout.Token).AsTask());

        // Assert
        accepted.ShouldBeSameAs(upgrading);
        observed.ShouldBeSameAs(failure);
    }

    [Fact(DisplayName = "Cohesion Test [Connections] - DisposeAsync: Should close every connection the listener has not returned")]
    public async Task DisposeAsync_WithUpgradesPendingAndConnectionsQueued_ShouldDisposeEveryConnectionNotReturned()
    {
        // Arrange
        using CancellationTokenSource timeout = new(_testTimeout);
        TestConnection upgrading = new();
        TestConnection first = new();
        TestConnection second = new();
        ControlledConnectionLayer layer = new();
        layer.Hold(upgrading);
        BlockingConnectionListener inner = new();
        inner.Enqueue(upgrading);
        inner.Enqueue(first);
        inner.Enqueue(second);
        IDuplexConnectionListener listener = inner.Use(layer);
        IDuplexConnection returned = await listener.AcceptAsync(timeout.Token);
        TestConnection queued = ReferenceEquals(returned, first) ? second : first;
        await layer.WhenStarted(upgrading).WaitAsync(timeout.Token);
        await layer.WhenStarted(queued).WaitAsync(timeout.Token);

        // Act
        await listener.DisposeAsync();

        // Assert — the stalled upgrade was canceled and the queued connection drained; the returned one
        // belongs to its caller.
        inner.IsDisposed.ShouldBeTrue();
        upgrading.ConnectionClosed.IsCancellationRequested.ShouldBeTrue();
        queued.ConnectionClosed.IsCancellationRequested.ShouldBeTrue();
        returned.ConnectionClosed.IsCancellationRequested.ShouldBeFalse();
        await Should.ThrowAsync<ObjectDisposedException>(() => listener.AcceptAsync(timeout.Token).AsTask());
    }

    [Theory(DisplayName = "Cohesion Test [Connections] - Use: Should reject an upgrade limit below one")]
    [InlineData(0)]
    [InlineData(-1)]
    public void Use_WithNonPositiveUpgradeLimit_ShouldThrowArgumentOutOfRangeException(int maxConcurrentUpgrades)
    {
        // Arrange
        BlockingConnectionListener inner = new();
        ControlledConnectionLayer layer = new();

        // Act / Assert
        Should.Throw<ArgumentOutOfRangeException>(() => inner.Use(layer, maxConcurrentUpgrades));
    }

    private static async Task WhenClosedAsync(IConnection connection, CancellationToken cancellationToken)
    {
        TaskCompletionSource closed = new(TaskCreationOptions.RunContinuationsAsynchronously);
        using CancellationTokenRegistration registration = connection.ConnectionClosed.Register(
            static state => ((TaskCompletionSource)state!).TrySetResult(),
            closed);

        await closed.Task.WaitAsync(cancellationToken);
    }
}
```

### Walkthrough

- **Covered behavior** — AcceptAsync: A stalled upgrade should not delay another connection.
- **Covered behavior** — AcceptAsync: A failed upgrade should close its connection and keep accepting.
- **Covered behavior** — AcceptAsync: Canceling a wait should leave the upgrade running.
- **Covered behavior** — AcceptAsync: Should stop accepting from the inner listener at the upgrade limit.
- **Covered behavior** — AcceptAsync: Should return upgrading connections before the inner listener's failure.
- **Covered behavior** — DisposeAsync: Should close every connection the listener has not returned.
- **Covered behavior** — Use: Should reject an upgrade limit below one. Inline data: `(0)`, `(-1)`.

The assertions define the expected result or failure boundary. Test-only doubles supply controlled
inputs; they are not additional shipped APIs.

## Sources

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/tests/ConnectionDirectionTests.cs`.
- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/tests/Assimalign.Cohesion.Connections.Tests.csproj`.
- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/tests/ConnectionLayerEventSourceTests.cs`.
- **Source** — `cohesion/build/TestSupport/EventSourceRecorder.cs`.
- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/tests/ConnectionFeatureCollectionTests.cs`.
- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/tests/ConnectionFeaturesTests.cs`.
- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/tests/GuidedBaseTests.cs`.
- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/tests/TestObjects/FeatureConnectionProbe.cs`.
- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/tests/TestObjects/TestConnectionFeature.cs`.
- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/tests/TestObjects/TestConnection.cs`.
- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/tests/TestObjects/TestMultiplexedConnection.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/tests/ConnectionListenerTests.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/tests/ConnectionLayerTests.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/tests/LayeredConnectionListenerTests.cs`.
- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/tests/TestObjects/BlockingConnectionListener.cs`.
- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/tests/TestObjects/ControlledConnectionLayer.cs`.
- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/tests/TestObjects/LayerWrappedConnection.cs`.
- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/tests/TestObjects/RecordingConnectionLayer.cs`.
- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/tests/TestObjects/TestConnectionFactory.cs`.
- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/tests/TestObjects/TestConnectionListener.cs`.
