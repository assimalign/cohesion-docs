# ApplicationModel

Application graphs, portable realization plans, and gateway orchestration.

[Libraries](../index.md)

## Packages

| Package | Responsibility | Reference |
|---|---|---|
| `Assimalign.Cohesion.ApplicationModel` | Defines application graphs, resource manifests, portable plans, and desired resource commands. | [Overview](assimalign-cohesion-applicationmodel/index.md) |
| `Assimalign.Cohesion.ApplicationModel.Gateway` | Realizes application graphs in dependency order and tracks resource and command observations. | [Overview](assimalign-cohesion-applicationmodel-gateway/index.md) |
| `Assimalign.Cohesion.ApplicationModel.Gateway.ControlPlane` | Exposes authenticated application discovery and resource command delivery over HTTP. | [Overview](assimalign-cohesion-applicationmodel-gateway-controlplane/index.md) |
| `Assimalign.Cohesion.ApplicationModel.Gateway.InProcess` | Realizes composable resources as separately owned hosts inside a gateway process. | [Overview](assimalign-cohesion-applicationmodel-gateway-inprocess/index.md) |

## Dependencies and delivery

The repository places this area in delivery wave 4. The wave is a dependency-ordering guide, not a
release or completeness guarantee.

Although stored under `libraries/`, this area belongs to Layer 2 application runtime and
composition. The base model remains Core-only; gateway packages add realization and control-plane
dependencies. The area architecture record places runtime ownership in the consuming Composite and
keeps resource declarative packages independent of gateway assemblies.

| Assembly | Declared dependencies |
|---|---|
| `Assimalign.Cohesion.ApplicationModel` | `Assimalign.Cohesion.Core` (CohesionProjectReference) |
| `Assimalign.Cohesion.ApplicationModel.Gateway` | `Assimalign.Cohesion.ApplicationModel` (CohesionProjectReference), `Assimalign.Cohesion.ConfigurationStore.Client` (CohesionProjectReference), `Assimalign.Cohesion.Database.Client` (CohesionProjectReference), `Assimalign.Cohesion.Hosting.Resources` (CohesionProjectReference), `Assimalign.Cohesion.IdentityHub.Client` (CohesionProjectReference), `Assimalign.Cohesion.IdentityModel.Token.JsonWebToken` (CohesionProjectReference), `Assimalign.Cohesion.Rezolvr.Client` (CohesionProjectReference), `Assimalign.Cohesion.SecretStore.Client` (CohesionProjectReference), `Assimalign.Cohesion.Security.DataProtection` (CohesionProjectReference), `System.Security.Cryptography.ProtectedData` (CohesionPackageReference) |
| `Assimalign.Cohesion.ApplicationModel.Gateway.ControlPlane` | `Assimalign.Cohesion.ApplicationModel` (CohesionProjectReference), `Assimalign.Cohesion.ApplicationModel.Gateway` (CohesionProjectReference), `Assimalign.Cohesion.Connections.Tcp` (CohesionProjectReference), `Assimalign.Cohesion.Core` (CohesionProjectReference), `Assimalign.Cohesion.Hosting.Resources` (CohesionProjectReference), `Assimalign.Cohesion.Http` (CohesionProjectReference), `Assimalign.Cohesion.Http.Connections` (CohesionProjectReference), `Assimalign.Cohesion.IdentityModel` (CohesionProjectReference), `Assimalign.Cohesion.IdentityModel.Token.JsonWebToken` (CohesionProjectReference), `Assimalign.Cohesion.Web.Routing` (CohesionProjectReference) |
| `Assimalign.Cohesion.ApplicationModel.Gateway.InProcess` | `Assimalign.Cohesion.ApplicationModel` (CohesionProjectReference), `Assimalign.Cohesion.ApplicationModel.Gateway` (CohesionProjectReference), `Assimalign.Cohesion.Hosting` (CohesionProjectReference), `Assimalign.Cohesion.Hosting.Resources` (CohesionProjectReference) |

## Sources

- **Source** — `cohesion/libraries/README.md`.

- **Source** — `cohesion/README.md`.

- **Source** — `cohesion/docs/programs/DELIVERY_ROADMAP.md`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/ApplicationModel/README.md`.

- **Source** — `cohesion/docs/libraries/ApplicationModel/DESIGN.md`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel/src/Assimalign.Cohesion.ApplicationModel.csproj`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel/src`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway/src/Assimalign.Cohesion.ApplicationModel.Gateway.csproj`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway/src`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway.ControlPlane/src/Assimalign.Cohesion.ApplicationModel.Gateway.ControlPlane.csproj`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway.ControlPlane/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway.ControlPlane/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway.ControlPlane/src`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway.InProcess/src/Assimalign.Cohesion.ApplicationModel.Gateway.InProcess.csproj`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway.InProcess/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway.InProcess/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway.InProcess/src`.
