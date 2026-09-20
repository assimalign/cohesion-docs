# Design

Gateway generation separates portable orchestration metadata from application runtime realization.

> **Status:** Partial. First-restore manifest dependency selection remains incomplete.

## Static composition surface

The task reads JSON manifests and explicit provider contributions. Same-application resources
generate realization verbs; cross-application resources generate external declarations. A referenced
Composite gateway also creates an application declaration resolved through its control plane. Typed
mappings preserve area options and descriptors; unmapped kinds use generic resource APIs. There is
no reflection-based provider discovery.

`Gateway.CreateBuilder(args)` calls the runtime application builder with the compiled application
name. The generated application attribute is for inspection, not runtime identity discovery.
Provider dispatch respects the parsed builder request, then `COHESION_GATEWAY`, then the Local-only
default. The selected provider receives common options followed by its optional command-line hook.

## Package boundary and restoration

Gateway suppresses the implicit `Assimalign.Cohesion.App` reference before importing base props.
There is no `Assimalign.Cohesion.App.Gateway` framework. The normal orchestration path uses NuGet
packages. The active in-process exception currently adds `Assimalign.Cohesion.App`,
`Assimalign.Cohesion.App.Web`, and `Assimalign.Cohesion.App.Database` explicitly.

The shipped dependency bootstrap includes the seven Web, Database, ConfigurationStore, SecretStore,
IdentityHub, Rezolvr, and LogSpace ApplicationModel packages and the SecretStore, Database, and
ConfigurationStore clients. Manifest-derived requirements are discovered after restore and cannot
rewrite `project.assets.json`. A restore-visible producer dependency descriptor remains future
work. Other kinds use generic `ResourceOptions` /`AddResource` mapping. Docker/Kubernetes providers
belong to external platform packages; the SDK contains their package selection but does not supply
their implementations.

Required ApplicationModel and client outputs report what manifests need; they cannot add compile
dependencies to a completed restore. The intended producer descriptor must expose orchestration-only
dependencies during the first restore and be validated against the resolved manifest. This is a
documented future contract, not implemented behavior.

## In-process exception

Project resources can be nested only with both InProcess selection and explicit opt-in. Generated
bindings select enabled, composable, same-application resources and isolate content under
`cohesion/resources/<resource-name>`. Build/publish carries their selected runtime files, but this
post-restore selection does not merge child package identities, version solving, or child-only
`buildTransitive` behavior into the gateway’s lock file.

## Control plane and images

The control-plane package is fixed, not selectable as a provider. Generated composition installs its
resolver client in every mode and a listener factory for realizing `Run` and `Apply` modes.
`Describe`, `Render`, and `Bootstrap` remain listener-free.

Image gathering consumes source and package entries in declaration order, validates unique owners,
verifies archives, and rewrites archive locations relative to `application.images.json`. Active
InProcess-only selection emits one composite image; mixed sets retain member images and append the
composite. Package-only resources form the provisional pinned-image boundary.

## Source verification

Package-backed fixtures cover generated boundaries, disabled references, hosting isolation,
two-factor in-process selection, transitive resource runtime assets, typed descriptor/command
generation, and image ordering/relocation. These test contracts are indexed on the
[task page](../msbuild/tasks.md); this documentation change did not run a .NET build.

[Assembly](index.md) · [SDK home](../index.md)

## Sources

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Sdk/Sdk.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Sdk/Sdk.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Targets/Sdk.Gateway.Images.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Targets/Sdk.Gateway.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Targets/Sdk.Gateway.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Tasks/docs/OVERVIEW.md`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Tasks/docs/DESIGN.md`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Tasks/src/Tasks/CohesionCreateResourceVerbs.cs`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Tasks/src/Tasks/GatewaySourceWriter.cs`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Tasks/src/Tasks/CohesionGatherImageIndexes.cs`
