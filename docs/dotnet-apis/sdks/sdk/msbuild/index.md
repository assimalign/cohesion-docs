# MSBuild

The base SDK's import graph and the build files every layered SDK inherits.

## Evaluation order

1. `Sdk/Sdk.props` imports `Targets/Assimalign.Cohesion.Sdk.Defaults.props`, which supplies
   empty-value defaults before Microsoft defaults run.
2. `Microsoft.NET.Sdk/Sdk/Sdk.props` evaluates the Microsoft SDK props and consumer `Directory.Build.props`.
3. `Targets/Build.Version.props` defines `$(CohesionVersion)` from the snapshot generated when the
   SDK was packed, before any version-dependent import.
4. `Targets/Assimalign.Cohesion.Sdk.Common.props` sets the SDK marker and intermediate path, and
   copies a resource-area executable's `appsettings*.json` to build and publish output.
5. `Targets/Assimalign.Cohesion.Sdk.StronglyTypedSettings.props` registers the settings task.
6. `Targets/Assimalign.Cohesion.Sdk.FrameworkReference.props` registers every Cohesion framework
   without including one.
7. The consumer project body evaluates.
8. `Sdk/Sdk.targets` sets `DisableTransitiveFrameworkReferenceDownloads`, defaults a resource-area
   executable to self-contained for the host runtime identifier, and removes `None` items that
   duplicate the project's `Content`.
9. `Microsoft.NET.Sdk/Sdk/Sdk.targets` loads Microsoft build targets.
10. `Targets/Assimalign.Cohesion.Sdk.PinValidation.targets`,
    `Targets/Assimalign.Cohesion.Sdk.StronglyTypedSettings.targets`, and
    `Targets/Assimalign.Cohesion.Sdk.NameOnly.ProjectReference.targets` add pin validation,
    settings generation, and name-only reference conversion.

Each layered SDK imports the base props first and the base targets last, then its own `Targets/`
files. Evaluation order sets values and registers targets; target execution follows dependencies
and hooks, not file order.

- **Props** — [Every props file](props.md).
- **Targets** — [Target wiring](targets.md).
- **Tasks** — [Task ownership and tests](tasks.md).

[SDK home](../index.md)

## Sources

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Sdk/Sdk.props`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Sdk/Sdk.targets`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Tasks/docs/DESIGN.md`.
