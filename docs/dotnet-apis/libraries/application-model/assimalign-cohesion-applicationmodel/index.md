# Assimalign.Cohesion.ApplicationModel

Defines application graphs, resource manifests, portable plans, and desired resource commands.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[ApplicationModel](../index.md)

## Scope

Descriptors own graph identity, and `Build()` validates dependencies and commands before
realization. Commands contain portable desired state, so payloads must not contain secrets. Area
packages add typed composition verbs without moving resource runtimes into the base model.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Core`](../../core/assimalign-cohesion-core/index.md) | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `Application` | `src/Application.cs` |
| `ApplicationName` | `src/ValueObjects/ApplicationName.cs` |
| `GatewayCommand` | `src/ValueObjects/GatewayCommand.cs` |
| `IApplication` | `src/Abstractions/IApplication.cs` |
| `IApplicationBuilder` | `src/Abstractions/IApplicationBuilder.cs` |
| `IApplicationEnvironment` | `src/Abstractions/IApplicationEnvironment.cs` |
| `IApplicationModel` | `src/Abstractions/IApplicationModel.cs` |
| `IResourceCommand` | `src/Abstractions/IResourceCommand.cs` |
| `IResourceCommandDescriptor` | `src/Abstractions/IResourceCommandDescriptor.cs` |
| `ResourceCommands` | `src/Commands/ResourceCommands.cs` |
| `ApplicationBuilderExternalExtensions` | `src/Extensions/ApplicationBuilderExternalExtensions.cs` |
| `ApplicationDeclaration` | `src/ApplicationSets/ApplicationDeclaration.cs` |
| `ApplicationExportDocument` | `src/Documents/ApplicationExportDocument.cs` |
| `ApplicationExportEndpoint` | `src/Documents/ApplicationExportEndpoint.cs` |
| `ApplicationExportResource` | `src/Documents/ApplicationExportResource.cs` |
| `ApplicationModelCommandDocument` | `src/Documents/ApplicationModelCommandDocument.cs` |

## Sources

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel/src/Assimalign.Cohesion.ApplicationModel.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/ApplicationModel/README.md`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel/src`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel/src/Application.cs`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel/src/ValueObjects/ApplicationName.cs`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel/src/ValueObjects/GatewayCommand.cs`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel/src/Abstractions/IApplication.cs`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel/src/Abstractions/IApplicationBuilder.cs`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel/src/Abstractions/IApplicationEnvironment.cs`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel/src/Abstractions/IApplicationModel.cs`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel/src/Abstractions/IResourceCommand.cs`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel/src/Abstractions/IResourceCommandDescriptor.cs`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel/src/Commands/ResourceCommands.cs`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel/src/Extensions/ApplicationBuilderExternalExtensions.cs`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel/src/ApplicationSets/ApplicationDeclaration.cs`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel/src/Documents/ApplicationExportDocument.cs`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel/src/Documents/ApplicationExportEndpoint.cs`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel/src/Documents/ApplicationExportResource.cs`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel/src/Documents/ApplicationModelCommandDocument.cs`.
