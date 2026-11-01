# Assimalign.Cohesion.IoTHub.Hosting.ApplicationModel design

Design decisions and ownership boundaries for `Assimalign.Cohesion.IoTHub.Hosting.ApplicationModel`.

> **Status:** Implemented.

[Overview](index.md) · [Examples](examples/index.md)

## Composition

For a managed instance, `AddResource` makes one registration in `builder.Services`: a singleton
`IHostService` factory that the container runs once, when the application is built.

| Piece | Role |
|---|---|
| `IoTHubResourceOptions` | `GetGatewayClient()` selects the client of the platform the environment names, `Deployment` is the deployment that started the instance, and `CreateActions(provider)` creates the added actions. |
| `IoTHubResource` (internal) | The `IApplicationInstance`: provider `IoTHub`, the resource's name, a new instance key, the deployment, and the actions the application added. It contributes no health. |
| `ApplicationInstanceControlPlane` | The control plane over the resource, writing JSON through the contexts the added actions came with. |
| `ApplicationInstanceService` | Publishes and restores in `StartingAsync`, opens the control plane in `StartAsync`, and closes it and stops renewing in `StopAsync`. |

## Lifecycle

1. **Every `StartingAsync`** — The resource service publishes the manifest and restores the
   gateway's snapshots before any service starts, wherever `AddResource` sits in `Program.cs`. A
   refused publication fails the application's start.
2. **Services start** — The resource service starts the platform's client carrying the gateway's
   calls to the control plane.
3. **On stop** — Services stop in reverse order; the resource service closes the control plane and
   stops renewing.

## Decisions

- **A1 — One integration package per area** — There is no area definition package: a gateway
  declares the resource generically, `builder.AddResource("IoTHub", "devices")`.
- **A2 — A resource is a registration** — A managed resource is one singleton `IHostService`
  factory registration; an unmanaged one registers nothing. The verb is an ordinary extension
  member, not a generated component integration, because the package references the container.
- **A3 — The resource does no transport** — No server, listener, or client of its own. A platform
  the resource did not opt into fails the registration.
- **A4 — The resource reads its environment** — The options read the platform and deployment
  through Core's `AppEnvironment`, which reads the program's `AppEnvironmentScope` in process, so
  the same `Program.cs` runs on every platform it opts into.
- **A5 — Added actions come from the base options** — The control plane rejects a duplicate action
  name, which fails the application's start.
- **A6 — No health contributors yet**.

## Open questions

- **The area's own actions** — They arrive with the area program.
- **Authentication** — Publication and the gateway's calls are the platform's to authenticate.

## Sources

- **Primary source** — `cohesion/resources/IoTHub/Assimalign.Cohesion.IoTHub.Hosting.ApplicationModel/docs/DESIGN.md`.
- **Template** — `cohesion/resources/EmailHub/Assimalign.Cohesion.EmailHub.Hosting.ApplicationModel/docs/DESIGN.md`.
