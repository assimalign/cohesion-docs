# Assimalign.Cohesion.Database.Hosting.ApplicationModel design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Database.Hosting.ApplicationModel`.

> **Status:** Implemented. Endpoint allocation, gateway readiness, and the startup window have the limits below.

[Overview](index.md) · [Examples](examples/index.md)

## Composition

A managed `AddResource` call registers one singleton `IHostService` factory. Database.Hosting
resolves the container's lifecycle services once at Build, after freezing its engine/server
registry and creating every `AddService` product.

| Piece | Role |
|---|---|
| `DatabaseResourceOptions` | Selects the platform client and deployment through `ApplicationInstanceOptions`, and creates the added actions from application services. Adds only `Name`. |
| `DatabaseResource` (internal) | An `IApplicationInstance` with provider `Database`, the resource name, a fresh instance key, the deployment, exactly the added actions in order, and no health contributors. |
| `ApplicationInstanceControlPlane` | Dispatches the actions and restores state, using JSON metadata from the added actions' source-generated contexts. Duplicate action names fail composition. |
| `ApplicationInstanceService` | Publishes and restores in `StartingAsync`, opens the platform client in `StartAsync`, and closes it and stops lease renewal in `StopAsync`. |

Actions created by factories run once, in order, against the application's provider. The provider
owns disposable collaborators registered through factories; supplied instances remain borrowed.
The resource service retains container ownership and is never enrolled for a second disposal by
Database.Hosting.

## Lifecycle

1. **Build constructs the engines** — Engine factories open, recover, and provision declared
   databases, including inline schema migration, before Build returns. Deferred migrations can
   continue on engine-owned workers under their existing admission gates.
2. **StartingAsync publishes and restores** — Every lifecycle service's `StartingAsync` finishes
   before any service starts. The resource publishes its manifest, repeating the deployment, and
   restores the snapshots the gateway returns. Publication or restore failure fails startup.
3. **Services start** — `AddService` registrations start first, then container `IHostService`
   registrations, then the optional reopen service, each group in registration order. The resource
   service opens the selected platform client here, after restoration.
4. **Servers accept** — Servers start in engine/attachment order. These are the wire listeners
   the application's code configured; `AddResource` creates no listener or data endpoint.
5. **Stop reverses the order** — Servers drain while the control plane remains open. The reopen
   service stops, then container services in reverse order, then `AddService` services in reverse
   order. The resource service closes its client and stops renewing its lease during this sequence.

The [lifecycle tests](examples/database-resource-lifecycle-tests.md) use callbacks to verify that a
provisioned SQL database is usable at publication, restored state exists before the client opens,
and the server is closed before the client stops.

## Decisions

- **D1 — One explicit integration** — A gateway declares the resource generically with
  `builder.AddResource("Database", "orders-db")`; there is no Database gateway package.
- **D2 — The environment selects a platform** — Options read Core's `AppEnvironment`, including
  each in-process program's scope. A program with no named platform stays a plain application.
  There is no generated resource manifest or ambient instance registry.
- **D3 — Only application-added actions** — Catalog mutations need a management design that fits
  restoring a write's snapshot on every instance, each with its own durable catalog.
- **D4 — No instance health contributors yet** — Database host/admin health is separate; the
  gateway does not consume instance health yet.
- **D5 — Content uses source-generated JSON metadata** — There is no reflection fallback.

## Current limits

- **Endpoints** — Database binds only code-configured wire endpoints and reads no
  `COHESION_ENDPOINT_*` variable. The in-process platform allocates one `http` endpoint and reports
  an HTTP loopback `endpoint` output unrelated to the Database wire listener. Dependents currently
  pass the wire endpoint as a literal setting.
- **Readiness** — The gateway counts the first publication as ready, during `StartingAsync`,
  before Database servers listen and before the client opens. A dependent can deploy before the
  database accepts connections. Database's admin readiness gate does not change gateway readiness.
- **Startup window** — Publication waits for Build's engine open, recovery, and inline schema
  migration. The gateway's startup window must cover that work. Database host `StartupTimeout`
  bounds service startup and supplies no construction timeout.

The [in-process test](examples/database-resource-in-process-tests.md) waits for the Database host
to reach `Started` before invoking an action. Gateway `StopAsync` leaves workloads running; that
test disposes the gateway's owned in-process platform to stop the program cleanly.

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Hosting.ApplicationModel/docs/DESIGN.md`.
- **Host ordering** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Hosting/src/DatabaseApplicationBuilder.cs`.
- **Instance** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Hosting.ApplicationModel/src/Internal/DatabaseResource.cs`.
- **Platform and gateway** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway.InProcess/src/InProcessInfrastructure.cs` and `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway/src/ApplicationGateway.cs`.
- **Area design** — `cohesion/docs/libraries/ApplicationModel/DESIGN.md`.
