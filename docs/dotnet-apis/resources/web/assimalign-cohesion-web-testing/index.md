# Assimalign.Cohesion.Web.Testing

Full-pipeline integration testing for both manually composed Web applications and enabled Web resource executables.

> **Status:** Partial.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.

## Purpose

Full-pipeline integration testing for both manually composed Web applications and enabled Web
resource executables. The manual factory uses the in-memory connection driver. The
`FromProgram<Program>()` factory invokes the executable under an invocation-local
`Hosting.Resources` `ResourceContext`, waits for its registered default control plane, and uses its
ambient loopback `http` endpoint.

## Scope

- **`IWebApplicationTestFactory` / `WebApplicationTestFactory`** — the per-test application host.
- **`IWebApplicationProgramTestFactory`** — exposes the `Program` invocation's `ResourceContext`.
- **`WebApplicationTestFactoryOptions`** — protocol selection and client base address.
- **`WebApplicationProgramTestFactoryOptions`** — context, arguments, and lifecycle budgets.
- **`WebApplicationTestProtocol`** — `Http1` (default) or prior-knowledge `Http2`. HTTP/3 is a
  documented non-goal (see `DESIGN.md`).

The factory intentionally has no assertion helpers, no fixture base classes, and no test framework
coupling — it composes and hosts; the test framework and assertion library are the caller's
business.

## Usage

See the [source-backed usage examples](examples/index.md).

Prior-knowledge HTTP/2 over the same in-memory pair:

See the [source-backed usage examples](examples/index.md).

Drive the exact `Program` used in production:

See the [source-backed usage examples](examples/index.md).

## Dependencies

- **`Assimalign.Cohesion.Web.Hosting`** — the `WebApplicationBuilder` / `WebApplication`
  composition root the factory drives.
- **`Assimalign.Cohesion.Http.Connections`** — the `UseHttp1` / `UseHttp2` listener registration
  seams.
- **`Assimalign.Cohesion.Connections.InMemory`** — the in-memory transport driver (listener +
  dialing factory).
- **`Assimalign.Cohesion.Connections`** — the `Connection` contract and duplex-pipe stream
  adapter the client side rides.
- **`Assimalign.Cohesion.Hosting`** — host lifecycle capture for the `Program`-backed mode.
- **`Assimalign.Cohesion.Hosting.Resources`** — ambient resource scopes, entry registration,
  resource context, and control-plane bridge for that mode.

The client side is otherwise pure BCL (`SocketsHttpHandler`, `HttpClient`).

## Design

See [`DESIGN.md`](design.md) for the composition model, lifecycle contract, protocol scope
(including why HTTP/3 is out), parallel-isolation guarantees, and AOT posture.

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Web` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Web.Hosting` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Http.Connections` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Connections` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Connections.InMemory` | `CohesionProjectReference` |
| `Assimalign.Cohesion.DependencyInjection` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Hosting` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Hosting.Resources` | `CohesionProjectReference` |
| `Assimalign.Cohesion.IdentityModel.Token.JsonWebToken` | `CohesionProjectReference` |

[Parent: Web](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Testing/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Testing/src/Assimalign.Cohesion.Web.Testing.csproj`.
