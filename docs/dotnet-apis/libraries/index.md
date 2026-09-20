# Libraries

Explore Cohesion foundation library areas, their assemblies, and delivery dependencies.

[.NET APIs](../index.md)

The foundation library areas provide reusable contracts and implementations for Cohesion resources.
Most belong to Layer 1; ApplicationModel and runtime composition also participate in Layer 2.
Implemented scope is documented per assembly, because an area can contain both working packages and
reserved projects.

## Areas

| Area | Purpose |
|---|---|
| [Amqp](amqp/index.md) | Advanced Message Queuing Protocol (AMQP) wire handling and connection bindings. |
| [ApplicationModel](application-model/index.md) | Application graphs, portable realization plans, and gateway orchestration. |
| [Cache](cache/index.md) | Cache contracts and an in-process memory-cache implementation. |
| [Configuration](configuration/index.md) | A shared configuration model with command-line, environment, and file-format providers. |
| [Connections](connections/index.md) | Connection contracts and concrete stream, multiplexed, datagram, and security drivers. |
| [Content](content/index.md) | Format-neutral content contracts and binary, text, document, and media format packages. |
| [Core](core/index.md) | Dependency-light primitives and the shared resource environment contract. |
| [DependencyInjection](dependency-injection/index.md) | Service registration, resolution, provider construction, and scope lifetime management. |
| [Dns](dns/index.md) | Domain Name System (DNS) contracts, wire models, resolvers, and transport packages. |
| [FileSystem](file-system/index.md) | Portable file-system contracts with physical, memory, isolated, and aggregate providers. |
| [Hosting](hosting/index.md) | Plain host execution, opt-in resource supervision, health, and telemetry composition. |
| [Http](http/index.md) | Hypertext Transfer Protocol (HTTP) contracts, connection handling, and optional exchange features. |
| [IdentityModel](identity-model/index.md) | Canonical identity contracts with independent protocol and token-document branches. |
| [Logging](logging/index.md) | Structured logging contracts, factory composition, and console and debug sinks. |
| [ObjectMapping](object-mapping/index.md) | Explicit profile-based mapping between object models. |
| [ObjectPool](object-pool/index.md) | Object rental, return policies, factories, and retained instances. |
| [ObjectValidation](object-validation/index.md) | Fluent validation profiles, reusable rules, and structured failure results. |
| [OpenApi](open-api/index.md) | Version-aware OpenAPI models, authoring, serialization, validation, and integration contracts. |
| [OpenTelemetry](open-telemetry/index.md) | Bounded OpenTelemetry Protocol (OTLP) log export over HTTP with JSON. |
| [Resilience](resilience/index.md) | Execution pipelines with retry, timeout, circuit breaker, fallback, hedging, and rate limiting. |
| [Security](security/index.md) | Certificate loading and purpose-bound data protection. |

## Delivery dependency order

The repository groups foundation work in four waves. These are ordering constraints rather than
claims that every API in a wave is complete.

1. **Anchors** — Core and Security.
2. **Core infrastructure** — DependencyInjection, Configuration, Logging, FileSystem, Connections,
   Cache, and Resilience.
3. **Protocol and format** — Http, Amqp, Content, IdentityModel, OpenTelemetry, and OpenApi.
4. **Composition and high-risk work** — Hosting, ApplicationModel, and Dns.

ObjectMapping, ObjectPool, and ObjectValidation appear in the library inventory but are not assigned
to a named wave in the root diagram. The broader roadmap places SDK/tooling and application runtime
after foundation work, with service platforms depending on those layers. It does not turn a
placeholder library into an implemented capability.

Projects inherit `IsAotCompatible=true` from the library build properties. That setting expresses
the build policy; individual pages retain documented runtime-code-generation and incomplete-source
limitations rather than treating the flag as proof that every scenario has been published with
Native Ahead-of-Time (NativeAOT) compilation.

## Sources

- **Source** — `cohesion/libraries/README.md`.

- **Source** — `cohesion/README.md`.

- **Source** — `cohesion/docs/programs/DELIVERY_ROADMAP.md`.

- **Source** — `cohesion/libraries/Directory.Build.props`.
