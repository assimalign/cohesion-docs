# Cohesion Documentation

Cohesion is a code-first, multi-service application framework for .NET.

Foundation libraries, an application model, MSBuild software development kits (SDKs), and
NuGet-distributed shared frameworks provide the building blocks for composing services. The model
supports in-process, out-of-process, and cross-machine realization through gateways. Implementation
status varies by area; each resource page distinguishes its working runtime from deferred features.

## Explore Cohesion

- **[Overview](overview.md)** — Architecture, runtime boundaries, repository layout, and delivery model.
- **[ApiManager](api-manager/index.md)** — Resource host and declarative shell for planned API management.
- **[ConfigurationStore](configuration-store/index.md)** — Durable namespaces, snapshot reads, and mutations.
- **[Database](database/index.md)** — Database engines, data models, and language references.
- **[EmailHub](email-hub/index.md)** — Resource host for the planned email delivery platform.
- **[EventHub](event-hub/index.md)** — Resource host for planned event streams and consumer coordination.
- **[IdentityHub](identity-hub/index.md)** — Minimal OpenID Connect issuer and identity command registry.
- **[IoTHub](iot-hub/index.md)** — Resource host for planned device and telemetry services.
- **[LoadBalancer](load-balancer/index.md)** — Resource host for planned backend and traffic policy.
- **[LogSpace](log-space/index.md)** — Authenticated log ingestion, append-only storage, and paged query.
- **[MediaHub](media-hub/index.md)** — Resource host for planned media processing and delivery.
- **[MessageHub](message-hub/index.md)** — Resource host for planned queues, topics, and settlement.
- **[NatGateway](nat-gateway/index.md)** — Resource host for planned network address translation.
- **[NotificationHub](notification-hub/index.md)** — Resource host for planned notification channels.
- **[Rezolvr](rezolvr/index.md)** — Durable DNS record declarations; DNS answer serving remains deferred.
- **[Scheduler](scheduler/index.md)** — Jobs bound to cron or fixed-delay timer schedules.
- **[SecretStore](secret-store/index.md)** — Protected secrets, application trust, and private certificates.
- **[VpnGateway](vpn-gateway/index.md)** — Resource host for planned virtual private network services.
- **[Web](web/index.md)** — HTTP hosting, routing, endpoints, middleware, and testing.
- **[Platforms](platforms/index.md)** — Shared container machinery and Docker/Kubernetes gateways.
- **[.NET APIs](dotnet-apis/index.md)** — Libraries, Resources, and SDK assembly and build references.

## How this documentation is organized

Resources are the L3 service platforms under `cohesion/resources`. Their pages explain composition,
runtime behavior, and current limits. Database additionally provides language references.

Platforms are deployment gateways and their shared container infrastructure in
`cohesion-platforms`. They translate platform-neutral resource plans into deployment objects.

The .NET APIs section follows the Libraries, Resources, and SDKs source layout. It covers areas,
assemblies, documented public types, examples, design, and MSBuild integration.

## Sources

- **Framework definition** — `cohesion/README.md` and `cohesion/docs/OVERVIEW.md`.
- **Resource scope and status** — `cohesion/resources/` area READMEs.
- **Platform scope** — `cohesion-platforms/README.md`.
- **Navigation contract** — `cohesion-docs/TEMPLATE.md`.
