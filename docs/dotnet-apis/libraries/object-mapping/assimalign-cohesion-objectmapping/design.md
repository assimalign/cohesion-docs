# Assimalign.Cohesion.ObjectMapping design

Design decisions and ownership boundaries for `Assimalign.Cohesion.ObjectMapping`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

Profiles record ordered actions and the mapper replays matching source/target mappings. The
low-level operation is target-first to support multiple sources merging into an existing target.
Scalar access uses compiled getters and setters; that implementation detail is not a blanket
reflection-free construction guarantee.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Core`, `Assimalign.Cohesion.SourceGeneration.ObjectMapping`.
The [overview](index.md#dependencies) distinguishes project references, package references, shared
source, and analyzer inputs.

## Generated mappings and dynamic-code fallback

The generator lives in its own ObjectMapping-area project under owner decision 59
(2026-10-13). The library bundles this analyzer in its package; in-repository consumers name
`Assimalign.Cohesion.SourceGeneration.ObjectMapping`. Its generated API, source hints,
interceptor namespace and mapping behavior are unchanged by the project move. See the
[generator design](../assimalign-cohesion-sourcegeneration-objectmapping/design.md)
for the compiler component's ownership and verification.

Profiles first try `TryConfigureGenerated`. Eligible class-based profiles and intercepted inline
profiles register delegate-based actions, avoiding `Expression.Compile()` at runtime. Unsupported
expression or convention mappings retain a reflection/dynamic-code fallback and carry
`RequiresDynamicCode` or `RequiresUnreferencedCode` annotations.

The generator accepts a straight fluent chain on supported mappings. Loops, conditionals, locals,
string-based member mapping, `MapAll*`, and custom `MapAction` fall back. Inline interception requires
the project's `InterceptorsNamespaces` to include `Assimalign.Cohesion.ObjectMapping.Generated`.
Top-level inline profiles can still report `IL2026` or `IL3050` despite interception; the documented
method-based or class-based setup avoids that suppression limitation.

Targets must be reference types. Numeric and nullable conversions are explicit rather than inferred,
and synchronous mapping does not imply automatic name-based flattening.

## Build-generated resources (#1467)

Exception messages are held in `src/Properties/ErrorMessages.resx`. The internal `ErrorMessages` accessor is generated in the project
root namespace by MSBuild's GenerateResource task through
`build/Targets/Build.ErrorMessages.targets`. The generated source stays under
`obj/`; no checked-in designer or Visual Studio custom tool is required. Ordinary
`dotnet build`, CI and NativeAOT publication use the same embedded resources.
Existing message text is preserved. Resource names describe failures in short PascalCase
noun phrases of at most 40 characters (#1471); callers use the renamed internal accessors.

## Sources

- **Source** — `cohesion/libraries/ObjectMapping/Assimalign.Cohesion.ObjectMapping/src/Assimalign.Cohesion.ObjectMapping.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/ObjectMapping/Assimalign.Cohesion.ObjectMapping/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/ObjectMapping/Assimalign.Cohesion.ObjectMapping/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/ObjectMapping/Assimalign.Cohesion.ObjectMapping/src`.
