# Rezolvr

Rezolvr declares and persists DNS records while its DNS answer-serving runtime remains deferred.

> **Status:** Partial. Authenticated record commands and durable ownership work; the DNS listener remains dormant.

## What it is

Rezolvr is the Domain Name System (DNS) server product area, intended for authoritative zones,
forwarding, recursion, caching, transfers, and administration. It is not Cohesion's service
discovery subsystem. Discovery uses observed endpoints, platform Service DNS, and export documents.

The current runtime stores A and CNAME declarations but does not answer DNS queries.

See the [Rezolvr API reference](../dotnet-apis/resources/rezolvr/index.md).

## Project map

| Assembly | Responsibility |
|---|---|
| [`Assimalign.Cohesion.Rezolvr`](../dotnet-apis/resources/rezolvr/assimalign-cohesion-rezolvr/index.md) | Hosting-free application contracts and DNS service abstractions. |
| [`Assimalign.Cohesion.Rezolvr.ApplicationModel`](../dotnet-apis/resources/rezolvr/assimalign-cohesion-rezolvr-applicationmodel/index.md) | Typed resource, Deployment planner, and record command descriptors. |
| [`Assimalign.Cohesion.Rezolvr.Client`](../dotnet-apis/resources/rezolvr/assimalign-cohesion-rezolvr-client/index.md) | Authenticated apply/delete client for control-plane commands. |
| [`Assimalign.Cohesion.Rezolvr.Hosting`](../dotnet-apis/resources/rezolvr/assimalign-cohesion-rezolvr-hosting/index.md) | Resource management listener and durable record-command registry. |

## Hosting model

`RezolvrApplication.CreateBuilder(args)` creates the concrete builder. Explicit services start
in registration order and stop in reverse. Enabled resources expose the default plane on the
ambient `admin` endpoint: health, readiness, liveness, endpoint discovery, command envelopes,
and graceful stop. Managed routes verify ES256 bootstrap credentials.

Records are atomically stored in `records.json` below the `data` mount, or under the content
root's `data` directory when that mount is absent. Ownership is restored before the management
listener starts. The plain unregistered host opens no listener; `ResolverEndpointService`
remains dormant.

Non-persistent Configuration and Secret mounts remain manifest-driven runtime inputs. The current
Deployment planner does not establish a persistent-volume deployment contract for the record store.

The SDK retains `dns` over User Datagram Protocol (UDP) on port 53, `dns-tcp` over Transmission
Control Protocol (TCP) on port 53, and `admin` over HTTP on port 8081. Only the management endpoint
has a listener in the current host; declaring the DNS endpoints does not activate DNS serving.

## Application model

`AddRezolvr(manifest, options)` returns `IRezolvrResourceDescriptor` over a `RezolvrResource`.
The planner produces a platform-neutral `Deployment` plan; typed descriptors support dependency
edges, `AddARecord`, and `AddCnameRecord`. `RezolvrResourceControlPlane.Create()` advertises both
record-command kinds.

The guarded declarative package uses base class library address values, keeping DNS assemblies
outside its `COHAM001` dependency closure. The generic planner requires `StatefulSet` for a
persistent Volume, while Rezolvr requires `Deployment`; reconciling these constraints is deferred.

## SDK and framework

`Assimalign.Cohesion.Sdk.Rezolvr` delivers the runtime family through
`Assimalign.Cohesion.App.Rezolvr`. The area's `.ApplicationModel` package is NuGet-only
and added to enabled resource executables; it remains outside the runtime shared framework.
The `.Client` package is also NuGet-only.

See the [Rezolvr SDK reference](../dotnet-apis/sdks/sdk-rezolvr/index.md).

## Record commands

| Descriptor verb | Wire kind | Value |
|---|---|---|
| `AddARecord` | `rezolvr.add-a-record` | An IPv4 `IPAddress`, serialized as a string. |
| `AddCnameRecord` | `rezolvr.add-cname-record` | A DNS target name. |

The record name is the ownership key. Time to live (TTL) is a positive integer in seconds,
defaulting to 300. These declarations persist record intent and ownership; successful command
application is not evidence of an active DNS listener.

`RezolvrCommandClient.Create` takes a full control-plane URI and bearer credential.
`SendCommandAsync` and `DeleteCommandAsync` return `ResourceCommandObservation` values preserving
status and detail. The client does not acquire credentials or resolve sources.

## Getting started

The `cohesion-rezolvr` template supplies this minimal `Program.cs`.

```csharp
using Assimalign.Cohesion.Rezolvr;
using Assimalign.Cohesion.Rezolvr.Hosting;

RezolvrApplicationBuilder builder = RezolvrApplication.CreateBuilder(args);

await using RezolvrApplication application = builder.Build();
await application.RunAsync();
```

Return to [Cohesion Documentation](../index.md).

## Sources

- **Area** — `cohesion/resources/Rezolvr/README.md`.
- **Hosting** — `cohesion/resources/Rezolvr/Assimalign.Cohesion.Rezolvr.Hosting/docs/OVERVIEW.md` and `cohesion/resources/Rezolvr/Assimalign.Cohesion.Rezolvr.Hosting/docs/DESIGN.md`.
- **Application model** — `cohesion/resources/Rezolvr/Assimalign.Cohesion.Rezolvr.ApplicationModel/docs/OVERVIEW.md` and `cohesion/resources/Rezolvr/Assimalign.Cohesion.Rezolvr.ApplicationModel/docs/DESIGN.md`.
- **Client** — `cohesion/resources/Rezolvr/Assimalign.Cohesion.Rezolvr.Client/docs/OVERVIEW.md`.
- **Runtime contract** — `cohesion/docs/RUNTIME_CONTRACT.md`.
- **Package boundaries** — `cohesion/.claude/rules/resource-areas.md`.
- **Template** — `cohesion/tooling/templates/Assimalign.Cohesion.Templates/src/content/cohesion-rezolvr/Program.cs`.
