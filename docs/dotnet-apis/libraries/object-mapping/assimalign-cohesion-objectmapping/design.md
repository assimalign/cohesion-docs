# Assimalign.Cohesion.ObjectMapping design

Design decisions and ownership boundaries for `Assimalign.Cohesion.ObjectMapping`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

Profiles record ordered actions and the mapper replays matching source/target mappings. The
low-level operation is target-first to support multiple sources merging into an existing target.
Scalar access uses compiled getters and setters; that implementation detail is not a blanket
reflection-free construction guarantee.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Core`, `Assimalign.Cohesion.SourceGeneration`.
The [overview](index.md#dependencies) distinguishes project references, package references, shared
source, and analyzer inputs.

## Generated mappings and dynamic-code fallback

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

## Sources

- **Source** — `cohesion/libraries/ObjectMapping/Assimalign.Cohesion.ObjectMapping/src/Assimalign.Cohesion.ObjectMapping.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/ObjectMapping/Assimalign.Cohesion.ObjectMapping/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/ObjectMapping/Assimalign.Cohesion.ObjectMapping/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/ObjectMapping/Assimalign.Cohesion.ObjectMapping/src`.
