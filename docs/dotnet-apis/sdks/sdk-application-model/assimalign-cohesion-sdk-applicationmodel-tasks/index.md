# Assimalign.Cohesion.Sdk.ApplicationModel.Tasks

The ApplicationModel task assembly generates resource manifests, typed accessors, certificates, and image metadata.

`Assimalign.Cohesion.Sdk.ApplicationModel.Tasks.dll` ships inside the auxiliary ApplicationModel
SDK. Its resource, manifest, certificate, and image tasks run during the build; strongly typed
settings and SDK pin validation remain in the base task assembly.

Generated resource-reference `ConnectionFactory()` accessors return `IDuplexConnectionFactory`
and request that same contract from `ResourceRuntime`. The generated surface selects the duplex
contract because its consumers exchange framed bytes; `IConnectionFactory` can produce other
connection kinds.

- **Design** — [Build-time generation and package boundaries](design.md).
- **Overview** — [Consumer imports and generated outputs](../overview.md).
- **SDK** — [ApplicationModel SDK](../index.md).

## Sources

- **Primary source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.ApplicationModel/Tasks/docs/OVERVIEW.md`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.ApplicationModel/Tasks/docs/DESIGN.md`.
