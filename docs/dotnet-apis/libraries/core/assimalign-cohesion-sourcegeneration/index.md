# Assimalign.Cohesion.SourceGeneration

The root source-generator project serves contracts owned by `libraries/Core`, following owner
decision 59 of 2026-10-13. It provides `ComponentIntegrationGenerator`, which reads Core's
`ComponentIntegrationAttribute` declarations and emits registration verbs when a consuming
compilation references the named composition seam.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Core](../index.md)

## Scope

Core packages include this analyzer, and the App targeting pack carries it for SDK consumers.
In-repository projects that call generated verbs reference it with:

```xml
<CohesionAnalyzerReference Include="Assimalign.Cohesion.SourceGeneration" />
```

The analyzer targets `netstandard2.0` with `IsAotCompatible=false` because Roslyn loads it inside the
compiler. It has no runtime-library dependency; its references are Roslyn tooling packages.
Its tests live in `Assimalign.Cohesion.SourceGeneration.Tests` and exercise generated integrations
through consuming compilations. Analyzer test projects use the `<Project>.Tests` name and inherit
executable test-host defaults from `analyzers/Directory.Build.props`. Diagnostics keep ids `COHCMP0001` through `COHCMP0007` and are tracked
in the project's `AnalyzerReleases` files.

Area-owned generators live in `Assimalign.Cohesion.SourceGeneration.<Area>`. See [Design](design.md)
for the integration model, diagnostics, generated code, and delivery constraints. The repository's analyzer rule (`.claude/rules/analyzers.md`) governs placement, project/test naming and release tracking.

## Principal public types

| Type | Source file |
|---|---|
| `ComponentIntegrationGenerator` | `src/ComponentIntegrationGenerator.cs` |

## Sources

- **Source** — `cohesion/analyzers/Assimalign.Cohesion.SourceGeneration/docs/OVERVIEW.md`.

- **Source** — `cohesion/analyzers/Assimalign.Cohesion.SourceGeneration/src/Assimalign.Cohesion.SourceGeneration.csproj`.
