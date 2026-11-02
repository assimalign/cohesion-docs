# Tasks

The Gateway SDK runs no task.

The package carries no `Tasks/` folder: its packaging project sets `CohesionSdkPackTasks=false` and
takes no MSBuild package reference. The base SDK's [tasks](../../sdk/msbuild/tasks.md) run
unchanged.

## Verification in the source repository

`GatewaySdkTests` in `Tasks/tests` builds consumer fixtures against the packaged SDK: a gateway
that compiles against the implicit gateway package, and an in-process gateway that references an
`Sdk.Web` resource project, builds, and runs. Their presence is not a claim that tests were run
for this documentation.

[MSBuild](index.md) · [Packaging project](../assimalign-cohesion-sdk-gateway-tasks/index.md)

## Sources

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Tasks/src/Assimalign.Cohesion.Sdk.Gateway.Tasks.csproj`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Tasks/tests/GatewaySdkTests.cs`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Tasks/docs/DESIGN.md`.
