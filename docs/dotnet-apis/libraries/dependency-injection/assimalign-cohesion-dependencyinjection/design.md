# Assimalign.Cohesion.DependencyInjection design

Design decisions and ownership boundaries for `Assimalign.Cohesion.DependencyInjection`.

> **Status:** Partial.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

Registration descriptors are separate from call-site execution and scope ownership.
`ServiceProviderOptions.EnableDynamicCode=false` selects the interpreted resolver before a compiled
engine is created. That option does not remove constructor reflection; explicit factories or
instances are needed for reflection-free construction. When the runtime cannot generate code
(NativeAOT), the call-site factory rejects an `IEnumerable<T>` over a value type, and an open
generic closed over a value type, with `InvalidOperationException` when it builds the call site.

A scope disposes every disposable service it captured, newest first and each instance once. A
failing disposal does not stop the rest; afterward a single failure is rethrown as itself and
several as an `AggregateException`. A service created after the scope is disposed is disposed at
once, and its resolution throws `ObjectDisposedException`. A cycle through constructor parameters
is rejected while the call site is built. A cycle through a factory throws
`InvalidOperationException` instead of deadlocking, including between scoped factories in a child
scope; a cycle made only of transient services is still not detected.

With `ValidateScopes`, validation is keyed per registration (service type and slot) and walks each
call site once, so a graph that shares dependencies does not make it exponential. Single and
enumerable resolution share call sites by slot: enumeration gives slots to exact registrations
before open generic ones, because `GetService` prefers the exact registration.

The library is a fork of the .NET 7-era `Microsoft.Extensions.DependencyInjection` without keyed
services; the source design lists the upstream fixes ported since and the deliberate differences.
It reports through one event source, `Assimalign.Cohesion.DependencyInjection`, whose only
`Informational` event is the provider-built summary. It publishes no counters.

`ServiceDescriptor.ImplementationType` is annotated for public constructors, so the constructors
the call-site factory selects survive trimming. The remaining `MakeGenericType` and
`MakeArrayType` sites are safe under NativeAOT because of the value-type checks above. Hosting
modules register factories and instances only, so none relies on those shapes; for code that does,
the checks turn a latent native crash into a descriptive startup failure.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Core`. The [overview](index.md#dependencies)
distinguishes project references, package references, shared source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/DependencyInjection/Assimalign.Cohesion.DependencyInjection/src/Assimalign.Cohesion.DependencyInjection.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/DependencyInjection/Assimalign.Cohesion.DependencyInjection/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/DependencyInjection/Assimalign.Cohesion.DependencyInjection/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/DependencyInjection/Assimalign.Cohesion.DependencyInjection/src`.
