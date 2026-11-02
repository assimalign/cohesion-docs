# Assimalign.Cohesion.Database.Testing

Database testing runs a program's real entry point inside a scope with its own shutdown signal.

> **Status:** Implemented. Starting the program does not wait for listener readiness.

## Reference pages

- **[Design](design.md)** — Entry-point invocation, isolation, and ownership.
- **[Examples](examples/index.md)** — Source-backed factory and executable examples.

`DatabaseApplicationTestFactory.FromProgram<TProgram>()` uses the executable's actual entry-point
marker. For top-level statements, expose it with `public partial class Program { }`. The factory
runs that entry point on a background thread, entering a fresh `AppEnvironmentScope` without
inheriting the test's execution context. It does not change process environment variables.

## Usage

```csharp
using Assimalign.Cohesion.Database.Testing;

await using DatabaseApplicationTestFactory factory =
    DatabaseApplicationTestFactory.FromProgram<global::Program>();

await factory.StartAsync();
// Wait for the program's own observable readiness before sending requests.
await factory.StopAsync();
```

Use this in the test project that references the executable's public Program marker.
`StartAsync` returns after launch; `IsStarted` records launch, without implying server readiness.
`StopAsync` signals the scope's shutdown and waits for the entry point to return, surfacing program
failures. Repeated starts are no-ops. Disposal signals shutdown and bounds the wait with
`ShutdownTimeout`, which defaults to 30 seconds. Options also accept an `Arguments` array, copied
for the invocation.

The program composes its engines, schemas, and listeners normally. The factory supplies no gateway
platform or generated resource accessors. An `AddResource` call with no named platform remains
unmanaged. Use the
[managed-resource tests](../assimalign-cohesion-database-hosting-applicationmodel/examples/index.md)
for publication, snapshot restore, and gateway action calls.

## Public types

| Type | Role |
|---|---|
| `IDatabaseApplicationTestFactory` | Start, stop, launch state, and asynchronous disposal. |
| `DatabaseApplicationTestFactory` | Sealed factory with `FromProgram<TProgram>()` and its options overload. |
| `DatabaseApplicationTestFactoryOptions` | Entry arguments and a positive shutdown timeout. |

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Database.Hosting` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Hosting` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Core` | `CohesionProjectReference` |
| `Assimalign.Cohesion.IdentityModel.Token.JsonWebToken` | `CohesionProjectReference` |

The package is the area's explicit hosting-isolation exemption holder and has no test-framework
or assertion dependency.

[Parent: Database](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Testing/src/DatabaseApplicationTestFactory.cs`.
- **Lifecycle** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Testing/src/Abstractions/IDatabaseApplicationTestFactory.cs`.
- **Options** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Testing/src/DatabaseApplicationTestFactoryOptions.cs`.
- **Project** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Testing/src/Assimalign.Cohesion.Database.Testing.csproj`.
