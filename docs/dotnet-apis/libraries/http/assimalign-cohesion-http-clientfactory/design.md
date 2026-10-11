# Assimalign.Cohesion.Http.ClientFactory design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Http.ClientFactory`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

Client disposal does not immediately dispose a shared handler. Expired handlers are retired after
references drain, allowing rotation to refresh network state. Named options configure headers,
timeouts, handlers, and the factory-owned redirect policy.

## Reference-free composition integration

The library has no dependency on Cohesion dependency injection. Its assembly-level
`ComponentIntegrationAttribute` names the `IServiceProviderBuilder` seam as a string and the public
`HttpClientFactoryBuilder.Build()` instance method, so it contributes a verb with no added public
surface. When a consuming compilation references both libraries, the component-integration
generator recognizes the instance method as a builder template and projects
`AddHttpClientFactory(Action<HttpClientFactoryBuilder>? configure = null)` into that consumer.

The callback is optional for every builder template (owner decision 39, #1395), but a bare
`AddHttpClientFactory()` still fails at registration: `Build()` rejects a factory with no named
client with `InvalidOperationException`, so the requirement stays where it was, in the builder. The
generated adapter eagerly constructs the builder, invokes the configuration action when one is
given, calls `Build()` so its validation runs at registration time, and registers the result through
a producer lambda so the container captures it for disposal. Without the dependency-injection
assembly the generator emits no verb.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Http`. The [overview](index.md#dependencies)
distinguishes project references, package references, shared source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ClientFactory/src/Assimalign.Cohesion.Http.ClientFactory.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ClientFactory/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ClientFactory/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Http/README.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ClientFactory/src`.
