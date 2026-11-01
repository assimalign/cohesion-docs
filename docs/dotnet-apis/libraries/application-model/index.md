# ApplicationModel

The shared identity and protocol, the logical model a gateway runs, the instance runtime, and the gateway that deploys and manages resources.

> **Status:** Partial. The in-process platform runs end to end; the Kubernetes and Docker pairs live in `cohesion-platforms`.

[Libraries](../index.md)

## Packages

| Package | Responsibility | Reference |
|---|---|---|
| `Assimalign.Cohesion.ApplicationModel` | The shared identity: `IApplicationResource`, provider and resource names with their ids, and `PlatformName`. | [Overview](assimalign-cohesion-applicationmodel/index.md) |
| `Assimalign.Cohesion.ApplicationModel.Protocol` | What a gateway and an instance exchange: the instance manifest, publication, action invocation, snapshots, and the JSON content serializer. | [Overview](assimalign-cohesion-applicationmodel-protocol/index.md) |
| `Assimalign.Cohesion.ApplicationModel.Definition` | The logical model one gateway runs: resources, dependencies, settings, outputs, platform extensions, and the parent it joins. | [Overview](assimalign-cohesion-applicationmodel-definition/index.md) |
| `Assimalign.Cohesion.ApplicationModel.Hosting` | The instance runtime: instance, actions and state, control plane, the resource service, and `ApplicationInstanceOptions`. | [Overview](assimalign-cohesion-applicationmodel-hosting/index.md) |
| `Assimalign.Cohesion.ApplicationModel.Hosting.InProcess` | The in-process platform's instance half: `UseInProcess()` on a resource's options. | [Overview](assimalign-cohesion-applicationmodel-hosting-inprocess/index.md) |
| `Assimalign.Cohesion.ApplicationModel.Gateway` | The gateway: `ApplicationGateway.CreateBuilder(name)`, deployment in dependency order, publications, joins, and operator actions. | [Overview](assimalign-cohesion-applicationmodel-gateway/index.md) |
| `Assimalign.Cohesion.ApplicationModel.Gateway.InProcess` | The in-process platform's gateway half: `InProcessInfrastructure` runs each resource's program on a thread of the gateway's process. | [Overview](assimalign-cohesion-applicationmodel-gateway-inprocess/index.md) |

## How the pieces fit

A **gateway** runs one application model and deploys its resources through a **platform**. A
**resource instance** is an ordinary area application (`Program.cs`) that calls
`builder.Services.AddResource(name, resource => resource.UseInProcess())`: it publishes a manifest to
its gateway, restores the state the gateway kept, and runs the gateway's actions on its control
plane. The two sides share only the identity and the protocol; the logical side (Definition) and the
physical side (Hosting) never reference each other.

A platform is a pair of packages that owns every transport between the sides. Its instance half is
the client a resource publishes through; its gateway half is the infrastructure provider that
deploys workloads, receives publications, and calls instances. The in-process pair ships here; the
Kubernetes and Docker pairs are described under [Platforms](../../../platforms/index.md).

| Assembly | Declared dependencies |
|---|---|
| `Assimalign.Cohesion.ApplicationModel` | `Assimalign.Cohesion.Core` |
| `Assimalign.Cohesion.ApplicationModel.Protocol` | `Assimalign.Cohesion.Core`, `Assimalign.Cohesion.ApplicationModel` |
| `Assimalign.Cohesion.ApplicationModel.Definition` | `Assimalign.Cohesion.Core`, `Assimalign.Cohesion.ApplicationModel` |
| `Assimalign.Cohesion.ApplicationModel.Hosting` | `Assimalign.Cohesion.Core`, `Assimalign.Cohesion.Hosting`, `Assimalign.Cohesion.ApplicationModel.Protocol` |
| `Assimalign.Cohesion.ApplicationModel.Hosting.InProcess` | `Assimalign.Cohesion.Core`, `Assimalign.Cohesion.ApplicationModel.Protocol`, `Assimalign.Cohesion.ApplicationModel.Hosting` |
| `Assimalign.Cohesion.ApplicationModel.Gateway` | `Assimalign.Cohesion.ApplicationModel.Definition`, `Assimalign.Cohesion.ApplicationModel.Protocol` |
| `Assimalign.Cohesion.ApplicationModel.Gateway.InProcess` | `Assimalign.Cohesion.ApplicationModel.Gateway`, `Assimalign.Cohesion.ApplicationModel.Hosting`, `Assimalign.Cohesion.ApplicationModel.Hosting.InProcess` |

All references are `CohesionProjectReference` items. No shipped resource-area project may resolve an
`Assimalign.Cohesion.ApplicationModel.Gateway*` assembly (`COHRES003`), and libraries never
reference resources (`COHLIB001`): each area implements the instance seams in its own
`<Area>.Hosting.ApplicationModel` package. `ApplicationModel.Hosting` ships in the `App` hosting
kernel, so every `Sdk.<Area>` application can call `AddResource` without a package reference.

## Sources

- **Primary source** — `cohesion/libraries/ApplicationModel/README.md`.
- **Area design** — `cohesion/docs/libraries/ApplicationModel/DESIGN.md`.
- **Boundaries** — `cohesion/.claude/rules/resource-areas.md`.
- **Kernel membership** — `cohesion/libraries/App/Assimalign.Cohesion.App.props`.
