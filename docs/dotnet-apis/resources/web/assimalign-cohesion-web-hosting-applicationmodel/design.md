# Assimalign.Cohesion.Web.Hosting.ApplicationModel design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Web.Hosting.ApplicationModel`.

> **Status:** Implemented.

[Overview](index.md) · [Examples](examples/index.md)

## Composition

For a managed instance, `AddResource` makes one registration in `builder.Services`, a singleton
`IHostService` factory. When the application is built, the container runs it: it resolves the
registered settings (`IEnumerable<IWebApplicationSetting>`), creates the added actions from the
application's `IServiceProvider`, and builds these pieces.

| Piece | Role |
|---|---|
| `WebResourceOptions` | `GetGatewayClient()` selects the client of the platform the environment names, `Deployment` is the deployment that started the instance, and `CreateActions(provider)` creates the added actions. |
| `WebResource` (internal) | The `IApplicationInstance`: provider `Web`, the resource's name, a new instance key, the deployment, one action per registered setting it knows, then the added actions. |
| `ApplicationInstanceControlPlane` | The control plane over the resource, writing JSON through the resource's own context and the added actions' contexts. |
| `ApplicationInstanceService` | Publishes and restores in `StartingAsync`, opens the control plane in `StartAsync`, and closes it and stops renewing in `StopAsync`. |

## Lifecycle

1. **Every `StartingAsync`** — The resource service publishes the manifest and restores the
   gateway's snapshots before any service or server starts. A refused publication fails the start.
2. **Application services start** — The resource service starts the platform's client carrying the
   gateway's calls to the control plane.
3. **The application's servers start** and serve with the restored state.
4. **On stop** — The servers drain first while the control plane is still reachable; then services
   stop in reverse order and the resource service closes the control plane.

## Decisions

- **W4 — Actions come from the application's settings** — Each registered setting the resource
  knows becomes an action; the `cors` setting (`IWebApplicationSetting<CorsPolicy?>`) becomes the
  `cors` action. The action keeps no copy of the state: a read reports the setting, and a write or a
  restore sets it. A policy CORS refuses is rejected and the setting keeps its value.
- **W5 — The resource's own actions are hidden; the application adds its own** — `AddAction` is the
  base options' verb, shared by every area. A name already in use fails the application's build.
- **W6 — Content is JSON, written per action through its own context** — A platform stores
  snapshots as opaque bytes and needs no resource's types.
- **W7 — The resource reads its environment** — The same `Program.cs` runs in process and on every
  platform it opts into; Web.Hosting binds the platform's `http` endpoint when the application
  configures no listener of its own.
- **W8 — The gateway manages the resource through its platform** — A write runs on one instance, is
  recorded, and is restored on the others and on every later publication.
- **W9 — The resource does no transport** — `AddResource` registers the resource service with the
  selected client and nothing else: no server, listener, or HTTP client.
- **W10 — A resource is a registration** — The verb extends `IServiceProviderBuilder`, beside the
  features' registrations, and is an ordinary extension member rather than a generated component
  integration.

W1 to W3 described a private control-plane listener on Web.Server's engine; W9 superseded them.

## Open questions

- **Authentication** — Publication and the gateway's calls are the platform's to authenticate.
- **Predicate policies** — A CORS policy built with `SetIsOriginAllowed` holds code and has no data
  form.
- **Named policies** — Only the default policy is a setting.

## Sources

- **Primary source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Hosting.ApplicationModel/docs/DESIGN.md`.
- **Area design** — `cohesion/docs/libraries/ApplicationModel/DESIGN.md`.
