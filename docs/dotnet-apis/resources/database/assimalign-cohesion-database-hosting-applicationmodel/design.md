# Assimalign.Cohesion.Database.Hosting.ApplicationModel design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Database.Hosting.ApplicationModel`.

> **Status:** Implemented. Endpoint allocation, readiness, management actions, and health have the limits below.

[Overview](index.md) · [Examples](examples/index.md)

## Composition

A managed `AddResource` call registers one singleton `IHostService` factory. Database.Hosting
invokes the container's lifecycle factories once at Build, after freezing its engine/server
registry and creating every `AddService` product.

| Piece | Role |
|---|---|
| `DatabaseResourceOptions` | Selects the platform client and deployment through `ApplicationInstanceOptions`, and creates the added actions from application services. Adds only `Name`. |
| `DatabaseResource` (internal) | An `IApplicationInstance` with provider `Database`, the resource name, a fresh instance key, the deployment, exactly the added actions in order, and no health contributors. |
| `ApplicationInstanceControlPlane` | Dispatches the actions and restores state, using JSON metadata from the added actions' source-generated contexts. Duplicate action names fail composition. |
| `ApplicationInstanceService` | Publishes and restores in `StartingAsync`, opens the platform client in `StartAsync`, and closes it and stops lease renewal in `StopAsync`. |

Actions created by factories run once, in order, against the application's provider. The provider
owns disposable collaborators registered through factories; supplied instances remain borrowed.
Database.Hosting owns the resource service and disposes it once in the services phase, before
engines. Its factory is invoked directly, so the provider does not capture it for disposal.

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
4. **Health and servers accept** — Hosting starts health on a usable managed `http` assignment,
   after container resource services and before model servers. Servers then start in engine/attachment
   order, using explicit listeners or their named model assignments. `AddResource` adds no wire server.
   The in-process platform currently allocates only `http`, so wire listeners stay explicit there.
5. **Stop reverses the order** — Servers drain while the control plane remains open. The reopen
   service stops, then container services in reverse order, then `AddService` services in reverse
   order. The resource service closes its client and stops renewing its lease during this sequence.

The [lifecycle tests](examples/database-resource-lifecycle-tests.md) use callbacks to verify that a
provisioned SQL database is usable at publication, restored state is visible to an `AddService`
service before the client opens, and the server is closed before the client stops.

Gateway `StopAsync` and `Completion` leave the Database application `Started` and accepting
connections, by the gateway's contract (G37/G40). Gateway `DisposeAsync` stops its run and then
disposes the platform it owns (G42). In-process platform disposal shuts down its programs; this is
the end-to-end test's shutdown criterion. A borrowed platform remains its caller's responsibility.

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

The package design records seven open questions:

1. **Named endpoint allocation — owner: ApplicationModel.** Database consumes `sql`, `documents`,
   `graph`, `kv` and `blob` assignments when managed and no explicit listener is configured. The
   in-process platform still assigns only `http` and one HTTP loopback output, now serving Database
   health. Dependents pass wire addresses as literal settings until named allocation and outputs exist.
2. **Readiness — owner: ApplicationModel.Gateway (G18).** The gateway counts first publication as
   ready, in `StartingAsync` before servers listen or the client opens. A dependent can deploy
   before Database accepts connections. Admin readiness does not change gateway readiness.
3. **Startup window — owner: gateway application.** Publication waits for Build's engine open,
   recovery, and inline schema migration. The gateway's startup window must cover that work.
   Database host `StartupTimeout` bounds service startup and supplies no construction timeout.
4. **Database-owned actions — owner: Database.** Management operations need a design that fits
   instance snapshot restore and each instance's durable catalog before Database adds actions.
5. **Instance health — owners: ApplicationModel.Hosting/Gateway.** Health aggregation must be
   settled (Hosting open question 2; Gateway G18) before Database contributes health.
6. **Publication-to-open window — owners: ApplicationModel.Hosting/Database.** `AddService`
   services start after publication and restore, before the control plane opens. A slow service
   widens Hosting open question 7's window. Database users cannot reorder these services as Web
   and EmailHub users can.
7. **Instance health and actions — owner: Database.** Hosting now binds usable plain-HTTP `http`
   assignments, with the existing readiness gate and loopback-only namespaced probes. Mapping
   observations to instance health contributors and adding Database actions remain later work.

The [in-process test](examples/database-resource-in-process-tests.md) waits for the Database host
to reach `Started` before invoking an action and surfaces a failed startup rather than polling a
terminal state. It disposes the gateway to shut down the owned in-process platform and program.

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Hosting.ApplicationModel/docs/DESIGN.md`.
- **Host ordering** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Hosting/src/DatabaseApplicationBuilder.cs`.
- **Instance** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Hosting.ApplicationModel/src/Internal/DatabaseResource.cs`.
- **Platform and gateway** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway.InProcess/src/InProcessInfrastructure.cs` and `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway/src/ApplicationGateway.cs`.
- **Area design** — `cohesion/docs/libraries/ApplicationModel/DESIGN.md`.
