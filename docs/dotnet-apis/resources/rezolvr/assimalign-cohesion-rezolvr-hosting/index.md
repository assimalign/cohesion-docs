# Assimalign.Cohesion.Rezolvr.Hosting

`RezolvrApplication.CreateBuilder(args)` returns the public concrete `RezolvrApplicationBuilder`.

> **Status:** Partial.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.
- **[`RezolvrApplication`](rezolvr-application.md)** — Documented public type.

`RezolvrApplication.CreateBuilder(args)` returns the public concrete `RezolvrApplicationBuilder`.
Explicit services preserve registration/start order and reverse stop order. Enabled resources
discover their area control plane and serve health, readiness, liveness, endpoint discovery, stop,
and command envelopes on the ambient `admin` endpoint (http). The plain host opens no listener
without registration.

Managed namespaced routes use ES256 bootstrap verification. The private Web implementation stays out
of consumer reference packs. Record commands persist A and CNAME declarations; serving DNS answers
remains deferred.

See [DESIGN.md](design.md) .

## Commands

| Wire kind | Descriptor verb | Ownership key |
|---|---|---|
| `rezolvr.add-a-record` | `AddARecord` | record name |
| `rezolvr.add-cname-record` | `AddCnameRecord` | record name |

A-record declarations use a BCL IPv4 IPAddress serialized as a string. CNAME declarations carry a
DNS target string; TTL is a positive integer in seconds, defaulting to 300. `COHAM001` keeps Dns
assemblies outside the ApplicationModel dependency closure.

Hosting stores records atomically in `records.json` under
`ResourceContext.GetMount("data", Path.GetFullPath(Path.Combine(ContentRootPath, "data")))`.
Without a data mount, storage therefore lives in the content-root-derived data directory. No
CohesionMount or CohesionWorkloadKind change is made: GenericPlanner requires StatefulSet for a
Volume while RezolvrPlanner requires Deployment. The command registry survives restart and restores
ownership before the listener starts.

Records are stored, not served as DNS answers. ResolverEndpointService remains parked. DNS serving
and reconciling a durable Volume with the Deployment contract are deferred area work.

## Concrete composition (T10 / O34)

`RezolvrApplication.CreateBuilder(args)` returns the public concrete `RezolvrApplicationBuilder`;
its `Build()` returns the public `RezolvrApplication : Host<RezolvrApplicationContext>`. The public
`RezolvrApplicationContext` implements `IRezolvrApplicationContext`, reading `ContentRootPath` from
the host environment. The application explicitly forwards the root lifecycle contract to `IHost`,
and consumers use the concrete application for `RunAsync` and `await using`. Runtime options and
supporting services remain internal.

Background-work registration belongs to the concrete `RezolvrApplicationBuilder`:
`AddService(IHostService)` and `AddService(Func<RezolvrApplicationContext, IHostService>)`. The
factory receives the same concrete context as Web's and `Database`'s `AddService`, so hosting consumers
can use environment, state, and hosted-service members beyond the small root contract. Factories run
once per build against the same context retained by the application; the hosted-service snapshot is
installed after factory evaluation. Services start in registration order and stop in reverse. No
area-owned service abstraction is introduced.

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Rezolvr` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Hosting` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Hosting.Health` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Hosting.Resources` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Hosting.Telemetry` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Web` | `CohesionPrivateProjectReference` |
| `Assimalign.Cohesion.Web.Hosting` | `CohesionPrivateProjectReference` |
| `Assimalign.Cohesion.Web.Hosting.Resources` | `CohesionPrivateProjectReference` |
| `Assimalign.Cohesion.Http` | `CohesionPrivateProjectReference` |
| `Assimalign.Cohesion.Http.Connections` | `CohesionPrivateProjectReference` |
| `Assimalign.Cohesion.Connections.Tcp` | `CohesionPrivateProjectReference` |

[Parent: Rezolvr](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Rezolvr/Assimalign.Cohesion.Rezolvr.Hosting/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/Rezolvr/Assimalign.Cohesion.Rezolvr.Hosting/src/Assimalign.Cohesion.Rezolvr.Hosting.csproj`.
