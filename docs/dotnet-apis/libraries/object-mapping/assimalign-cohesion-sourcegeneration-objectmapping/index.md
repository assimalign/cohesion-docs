# Assimalign.Cohesion.SourceGeneration.ObjectMapping

This Roslyn incremental generator provides compile-time mapping configuration for
`Assimalign.Cohesion.ObjectMapping`. It owns the generator for `libraries/ObjectMapping`
under the analyzer placement rule (owner decision 59, 2026-10-13).

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[ObjectMapping](../index.md)

## Scope

`MapperProfileGenerator` recognizes eligible partial `MapperProfile<TTarget, TSource>`
classes and inline `MapperBuilder.AddProfile<TTarget, TSource>` lambdas. It emits delegate
mapping configuration so these paths avoid reflection and expression compilation at runtime.
Unsupported configuration keeps the library's existing fallback behavior.

The project targets `netstandard2.0` with `IsAotCompatible=false`, inherited from the
analyzers area's build configuration. Those settings describe the compiler component;
its generated mapping code remains NativeAOT-compatible.

In-repository consumers reference it with:

```xml
<CohesionAnalyzerReference Include="Assimalign.Cohesion.SourceGeneration.ObjectMapping" />
```

The ObjectMapping library package bundles the generator under `analyzers/dotnet/cs`.
Inline profiles also require the existing interceptor namespace opt-in:

```xml
<InterceptorsNamespaces>$(InterceptorsNamespaces);Assimalign.Cohesion.ObjectMapping.Generated</InterceptorsNamespaces>
```

See [Design](design.md) for generation boundaries and verification, and the
[ObjectMapping design](../assimalign-cohesion-objectmapping/design.md)
for the runtime mapping contract.

## Principal public types

| Type | Source file |
|---|---|
| `MapperProfileGenerator` | `src/MapperProfileGenerator.cs` |

## MapperProfileGenerator

`MapperProfileGenerator` implements Roslyn's `IIncrementalGenerator` and is discovered by
the C# compiler through its `Generator` attribute. `Initialize` registers the syntax pipelines
for class-based profiles and inline profile registrations.

The generator is a build-time component. Applications use the ObjectMapping runtime API;
they do not instantiate this type. Direct Roslyn hosts can construct the generator and adapt it
with `AsSourceGenerator()` for a generator driver.

See [the design](design.md) for supported input shapes and generated output.

## Sources

- **Source** — `cohesion/analyzers/Assimalign.Cohesion.SourceGeneration.ObjectMapping/docs/OVERVIEW.md`.

- **Source** — `cohesion/analyzers/Assimalign.Cohesion.SourceGeneration.ObjectMapping/src/Assimalign.Cohesion.SourceGeneration.ObjectMapping.csproj`.

- **Source** — `cohesion/analyzers/Assimalign.Cohesion.SourceGeneration.ObjectMapping/docs/Assembly/Assimalign.Cohesion.SourceGeneration.ObjectMapping/MapperProfileGenerator/OVERVIEW.md`.
