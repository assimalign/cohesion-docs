# Design

The base task assembly converts build metadata into deterministic generated contracts without runtime discovery.

> **Status:** Partial. Image generation has explicit unavailable routes and deferred transfer tooling.

## Build and runtime boundary

The SDK owns properties, items, targets, generated source, and diagnostics. Runtime service
composition stays in hosting libraries and the consumer entry point. Settings generation emits
explicit configuration reads rather than a reflection binder; resource generation emits statically
referenced accessors and control-plane registration.

Generated identifiers split on non-alphanumeric separators, capitalize segment starts, preserve
remaining characters, and prefix a leading digit with an underscore. The established application
spelling maps `appa` to `AppA`. `CohesionIdentifier` is shared with Gateway generation.

## Incremental settings and manifests

The settings fingerprint records class, namespace, and input paths; file and task-assembly
timestamps participate in target `Inputs`. The output and `Compile` entry exist only with
`CohesionAppSettingsClass`. Cleanup also removes prior opted-out artifacts. JSON shapes merge
deterministically, and incompatible shapes fail instead of generating order-dependent types.

Resource generation uses the project/import/reference-manifest inputs and produces the three target
outputs together. Commands remain sorted bare names; payloads are runtime application-model data.
Portable packages clear local project/apphost locations and expose manifests through
`buildTransitive`.

## Images and pin validation

Image publication separates input validation, fresh-process payload publication, verified cache
selection, one container creation, digest validation, and index writing. Release never silently
falls back to JIT. The in-container build contract and digest-preserving transfer tool remain
incomplete.

Pin validation checks agreement among present known identities rather than comparing consumers with
a hardcoded repository package version. It allows a consistent local identity and accepts the .NET
JSON comment/trailing-comma conventions.

## Packaging and verification

The shared SDK packaging targets ship `Sdk/`, `Targets/`, and built `Tasks/` output, including a
frozen version props file. Package-backed integration fixtures exercise SDK resolution, imports,
generated compilation, manifest packing, pins, and incremental behavior. The
[task reference](../msbuild/tasks.md) names the actual test classes and methods.

[Assembly](index.md) · [SDK home](../index.md)

## Sources

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Tasks/docs/DESIGN.md`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Tasks/docs/OVERVIEW.md`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Tasks/src/Internal/Utilities/CohesionIdentifier.cs`
- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Assimalign.Cohesion.Sdk.StronglyTypedSettings.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Sdk.Resource.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Sdk.Image.targets`
- **Source** — `cohesion/sdks/Directory.Build.targets`
