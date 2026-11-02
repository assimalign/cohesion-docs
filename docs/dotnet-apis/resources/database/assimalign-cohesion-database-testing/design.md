# Assimalign.Cohesion.Database.Testing design

The test factory preserves a Database program's composition and controls its scoped lifetime.

> **Status:** Implemented.

[Overview](index.md) · [Examples](examples/index.md)

## Invocation

`TProgram` is the actual declaring type of `Assembly.EntryPoint`, not a composition interface.
A top-level executable exposes the compiler-generated marker with
`public partial class Program { }`. Supported entry points take no arguments or one `string[]`
and return `void`, `int`, `Task`, or `Task<int>`. A nonzero exit code or thrown exception faults
the program task; `StopAsync` observes that task.

The factory starts a dedicated background thread without the caller's execution context. It enters
a fresh, empty `AppEnvironmentScope` carrying the factory's shutdown token before invoking the
entry point. The program's ordinary host responds to that scoped shutdown signal. Factories do
not mutate process environment variables and can run independently when programs use distinct
endpoints and data paths.

## Lifecycle

| Operation | Behavior |
|---|---|
| `FromProgram` | Validates the entry-point marker and options; clones arguments. Nothing runs yet. |
| `StartAsync` | Launches once and returns without a readiness check. A pre-cancelled token prevents launch. |
| `StopAsync` | Signals scope shutdown, waits for the entry point, and clears `IsStarted` after a successful return. |
| `DisposeAsync` | Signals shutdown and waits within the configured disposal budget; repeated disposal returns. |

Tests observe the program's readiness themselves. The
[factory tests](examples/database-application-test-factory-tests.md) wait for Database.Hosting's
`ApplicationStarted` event, then verify `ApplicationStopped` after shutdown. No generated manifest,
resource context, bootstrap credential, or admin shutdown route is required.

## AOT and trimming

The sanctioned reflection operation is `Assembly.EntryPoint` and invocation of that compiler-rooted
method. The generic marker carries preservation annotations for its public and non-public methods.
There is no assembly scanning, dynamic loading, reflection-based serialization, or runtime code
generation. The package's documented trimming suppression covers the entry-point invocation.

## Ownership and boundaries

The program owns and disposes the application it builds. The factory controls the invocation's
shutdown and waits for the program to return; it does not construct another host or duplicate
engine composition. The area's hosting-isolation exemption allows this testing package to reference
Database.Hosting without extending that permission to production features.

The SDK sample uses configuration for its data path and endpoint. Its SQL engine builder declares
and provisions the schema inside application Build. The old generated-resource gateway E2E fixture
is absent at this revision; the separate
[managed integration tests](../assimalign-cohesion-database-hosting-applicationmodel/examples/index.md)
exercise the new gateway path.

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Testing/src/DatabaseApplicationTestFactory.cs`.
- **Contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Testing/src/Abstractions/IDatabaseApplicationTestFactory.cs`.
- **Tests** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Testing/tests/DatabaseApplicationTestFactoryTests.cs`.
- **Design** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Testing/docs/DESIGN.md`.
