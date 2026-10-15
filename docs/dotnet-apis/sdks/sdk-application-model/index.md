# Assimalign.Cohesion.Sdk.ApplicationModel

The ApplicationModel SDK supplies resource manifests, generated resource APIs, certificates, and image production.

This auxiliary MSBuild SDK is imported by resource-area SDKs when the project body sets
`CohesionApplicationModel=enabled`. Gateway imports it unconditionally. Disabled area consumers
remain plain applications. The import resolves at the importing SDK's exact packaged version;
it does not require another `global.json` pin.

An enabled executable receives `resource.json`, `Resource.g.cs`, and
`ResourceControlPlane.g.cs`. Resource-reference `ConnectionFactory()` accessors return
`IDuplexConnectionFactory` for database clients and other byte-stream consumers. Application
composition remains in the authored `Program.cs`.

- **Overview** — [Imports, generated outputs, and diagnostics](overview.md).
- **Task assembly** — [`Assimalign.Cohesion.Sdk.ApplicationModel.Tasks`](assimalign-cohesion-sdk-applicationmodel-tasks/index.md).
- **Design** — [Evaluation order, generation, and package boundaries](assimalign-cohesion-sdk-applicationmodel-tasks/design.md).
- **Family** — [All SDKs](../index.md).

## Sources

- **Primary source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.ApplicationModel/Tasks/docs/OVERVIEW.md`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.ApplicationModel/Tasks/docs/DESIGN.md`.
