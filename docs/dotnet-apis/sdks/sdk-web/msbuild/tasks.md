# Tasks

The Web SDK uses shared SDK tasks and defines no area-specific task classes.

`Assimalign.Cohesion.Sdk.Web.Tasks` has a `Tasks/src/Assimalign.Cohesion.Sdk.Web.Tasks.csproj`
packaging project but no C# task implementation files. No area `UsingTask` is registered. The empty
`Targets/Sdk.Web.targets` adds no task invocation.

## Shared task contract

The base SDK owns settings generation and pin validation. Area values flow into
`CohesionCreateResourceManifest` through the ApplicationModel SDK’s resource targets; that SDK
also owns image publishing and manifest packing. The Web layer supplies declarative defaults.

## Verification in the source repository

`ResourceAreaDefaultsTests` checks the area default control-plane factories, matching probes, and
HTTPS certificate mounts in the ApplicationModel SDK test project. This family contains no local
`Tasks/tests` suite. [Web Event Source Support Tests](../assimalign-cohesion-sdk-web-tasks/examples/web-event-source-support-tests.md)
evaluates the shipped Web props with `PublishAot=true` and checks the unset default and explicit
values before the import, after it, and on the command line. These evaluations require no package
feed or application build. The shared integration tests cover manifest generation and error cases;
their presence is not a claim that tests were run for this documentation change.

[MSBuild](index.md) · [Packaging assembly](../assimalign-cohesion-sdk-web-tasks/index.md)

## Sources

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.ApplicationModel/Tasks/tests/WebEventSourceSupportTests.cs`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Web/Tasks/src/Assimalign.Cohesion.Sdk.Web.Tasks.csproj`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Web/Sdk/Sdk.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Web/Sdk/Sdk.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Web/Targets/Sdk.Web.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Web/Targets/Sdk.Web.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.ApplicationModel/Tasks/tests/ResourceAreaDefaultsTests.cs`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.ApplicationModel/Tasks/tests/ResourceManifestSdkIntegrationTests.cs`
