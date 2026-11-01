# Tasks

The Web SDK uses shared base tasks and defines no Web-specific task classes.

`Assimalign.Cohesion.Sdk.Web.Tasks` is a packaging project with no C# task implementation files.
No Web `UsingTask` is registered.

## Verification in the source repository

`WebEventSourceSupportTests` in the base SDK's `Tasks/tests` project evaluates
`EventSourceSupport` for `PublishAot` consumers across every override position. Its presence is
not a claim that tests were run for this documentation.

[MSBuild](index.md) · [Packaging assembly](../assimalign-cohesion-sdk-web-tasks/index.md)

## Sources

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Web/Tasks/src/Assimalign.Cohesion.Sdk.Web.Tasks.csproj`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Tasks/tests/WebEventSourceSupportTests.cs`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Web/Sdk/Sdk.props`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Web/Sdk/Sdk.targets`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Web/Targets/Sdk.Web.props`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Web/Targets/Sdk.Web.targets`.
