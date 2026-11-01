# MSBuild

The MediaHub SDK surrounds the consumer project with the base imports and its framework references.

## Evaluation order

1. `Assimalign.Cohesion.Sdk.MediaHub/Sdk/Sdk.props` sets the private `_CohesionResourceSdk` marker.
2. `Assimalign.Cohesion.Sdk/Sdk/Sdk.props` imports `Targets/Assimalign.Cohesion.Sdk.Defaults.props`,
   which supplies empty-value defaults (including `OutputType=Exe`) before Microsoft defaults run.
3. `Microsoft.NET.Sdk/Sdk/Sdk.props` evaluates the Microsoft SDK props and consumer `Directory.Build.props`.
4. `Targets/Build.Version.props` loads the version snapshot generated when the SDK was packed.
5. `Targets/Assimalign.Cohesion.Sdk.Common.props`, `Targets/Assimalign.Cohesion.Sdk.StronglyTypedSettings.props`,
   and `Targets/Assimalign.Cohesion.Sdk.FrameworkReference.props` set common properties, register the
   settings task, and register every Cohesion framework without including one.
6. `Assimalign.Cohesion.Sdk.MediaHub/Sdk/Sdk.props` adds the `Assimalign.Cohesion.App` and `Assimalign.Cohesion.App.MediaHub`
   framework references, then imports `Targets/Sdk.MediaHub.props`.
7. The consumer project body evaluates.
8. `Assimalign.Cohesion.Sdk/Sdk/Sdk.targets` sets `DisableTransitiveFrameworkReferenceDownloads`,
   defaults a resource executable to self-contained for the host runtime identifier, and removes
   `None` items that duplicate the project's `Content`.
9. `Microsoft.NET.Sdk/Sdk/Sdk.targets` loads Microsoft build targets.
10. The base imports pin validation, strongly typed settings, and name-only reference targets.
11. `Assimalign.Cohesion.Sdk.MediaHub/Sdk/Sdk.targets` imports `Targets/Sdk.MediaHub.targets`, which is empty.

- **Props** — [Every props file](props.md).
- **Targets** — [Target wiring](targets.md).
- **Tasks** — [Task ownership](tasks.md).

[SDK home](../index.md)

## Sources

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.MediaHub/Sdk/Sdk.props`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.MediaHub/Sdk/Sdk.targets`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.MediaHub/Targets/Sdk.MediaHub.props`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.MediaHub/Targets/Sdk.MediaHub.targets`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Sdk/Sdk.props`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Sdk/Sdk.targets`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Tasks/docs/DESIGN.md`.
