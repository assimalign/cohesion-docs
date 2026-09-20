# ConfigurationStoreApplication

The `ConfigurationStoreApplication` type belongs to `Assimalign.Cohesion.ConfigurationStore.Hosting`.

> **Status:** Partial.

Namespace: `Assimalign.Cohesion.ConfigurationStore.Hosting` Assembly:
`Assimalign.Cohesion.ConfigurationStore.Hosting`

## Purpose

`ConfigurationStoreApplication` is the public concrete application and creation entry point for the
configuration store hosting module. The application, builder, and context are public; runtime
options remain internal.

## Factory behavior

- **`CreateBuilder(string[] args)`** — validates the argument array and returns a `ConfigurationStoreApplicationBuilder`.
- **When generated resource registration** — is present, the builder consumes the ambient endpoint,
  volume, environment, trust key, and credential through `ResourceRuntime`.
- **A plain application accepts** — `--endpoint <http-uri>` and `--data <directory>`.
- **Building materializes explicit host** — services, durable declarations, and the protocol listener.

## Exceptions

`CreateBuilder` throws `ArgumentNullException` when `args` is `null`. Building throws
`InvalidOperationException` when a registered service factory returns null.

## Usage

See the [source-backed usage examples](examples/index.md).

## Concrete composition (T10 / O34)

`ConfigurationStoreApplication.CreateBuilder(args)` returns the public concrete
`ConfigurationStoreApplicationBuilder`; its `Build()` returns the public
`ConfigurationStoreApplication : Host<ConfigurationStoreApplicationContext>`. The public
`ConfigurationStoreApplicationContext` implements `IConfigurationStoreApplicationContext`, reading
`ContentRootPath` from the host environment. The application explicitly forwards the root lifecycle
contract to `IHost`, and consumers use the concrete application for `RunAsync` and `await using`.
Runtime options and supporting services remain internal.

Background-work registration belongs to the concrete `ConfigurationStoreApplicationBuilder`:
`AddService(IHostService)` and
`AddService(Func<ConfigurationStoreApplicationContext, IHostService>)`. The factory receives the
same concrete context as Web's and `Database`'s `AddService`, so hosting consumers can use environment,
state, and hosted-service members beyond the small root contract. Factories run once per build
against the same context retained by the application; the hosted-service snapshot is installed after
factory evaluation. Services start in registration order and stop in reverse. No area-owned service
abstraction is introduced.

[Assembly overview](index.md) · [Examples](examples/index.md)

## Sources

- **Primary source** — `cohesion/resources/ConfigurationStore/Assimalign.Cohesion.ConfigurationStore.Hosting/docs/Assembly/Assimalign.Cohesion.ConfigurationStore.Hosting/ConfigurationStoreApplication/OVERVIEW.md`.
