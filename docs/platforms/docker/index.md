# Docker

The Docker gateway realizes resource plans through Docker-compatible engines, with Podman as its local target.

> **Status:** Partial. Compilation, reconciliation, observation, and rendering are implemented; live sensitive-input staging and full engine coverage remain incomplete.

## Package and selection

The package is `Assimalign.Cohesion.ApplicationModel.Gateway.Docker`. Add `Docker` to the gateway
project's `CohesionGateways` allowlist and select `--gateway docker` through generated
`UseGateway(args)`. The package contributes its gateway/options types and
`DockerGatewayCommandLine.Apply` through `CohesionGatewayProvider` metadata.

```xml
<PropertyGroup>
  <CohesionGateways>Local;Docker</CohesionGateways>
</PropertyGroup>
```

`CohesionPlatformsVersion` selects the external package version. See
[provider setup](../index.md#selecting-a-provider) for the SDK and pinning contract.
Direct composition also exposes `UseDockerGateway(args, configure)`.

## Plan compilation

| Plan fact | Docker realization |
|---|---|
| Resource workload | One container on an application network, with stable service aliases. |
| `DaemonSet` | One container on the selected engine. |
| `Job` | One execution; unsupported replica counts fail validation. |
| Persistent claim | Named volume. |
| Configuration mount | Staged file content before start. |
| Secret and bootstrap input | Sensitive archive entries targeting temporary in-memory file-system paths. |
| Endpoint and probe | Scheme-aware binding, explicit probes, or declared control-plane readiness. |

`DockerPlanCompiler` is pure. A level-triggered controller applies its output, while an
events-plus-inspect observer owns lifecycle and endpoint observations. Private readiness probes
receive loopback-only ephemeral host bindings. Explicitly disabled readiness remains disabled.

The gateway supervises `Always`, `OnFailure`, and `Never` restart policies; engine restart stays
`no` so one component owns restarts and can restage sensitive inputs. Configuration/startup exits
remain final. Liveness degradation is observed without re-gating dependents.

## Images and local development

Podman's Docker-compatible application programming interface (API) is the supported local engine.
`--docker-host` selects an endpoint. Repeatable `--image-archive=<digest-reference>=<path>` inputs
identify existing archives. The image index, when configured, must agree with the resource manifest.
A pinned registry wins over `ContainerRegistry`; execution uses a verified immutable engine image
identifier. Gather never builds.

Tests that require a compatible daemon skip when none is available. The source explicitly leaves
the full Docker/Podman end-to-end matrix open, including OCI-layout loading. Engine pulls currently
have no private-registry authentication carrier.

## Rendering and control plane

`--mode render --gateway docker` invokes `IApplicationGatewayRenderer.RenderAsync` to produce a
Compose-style review document. It preserves declaration order, resolves artifact identities
offline, and neither opens the engine nor resolves credentials. Unresolved mounts remain visible
with empty preview content. The renderer is not a Compose deployment controller.

`--control-plane-bind` accepts localhost, an IP address, or an absolute HTTP URI; a bare host uses
port zero. With no override, the bind is loopback with an ephemeral port. The shared gateway
ControlPlane package owns authentication and writes discovery beside `export.json`, normally
under `.cohesion/<application>/control-plane.json`.

## Lifecycle and current limits

`StopAsync` stops containers and releases supervision while retaining persistent networks and
claims. Teardown deletes in reverse order and continues after failures. Shared networks remain
when custom-controller delete outcomes cannot establish that removal is safe.

Sensitive archive upload occurs after container start, leaving a process-start race. Visibility of
temporary mounts also depends on the engine's archive implementation. The implementation does not
fall back to persisting secrets in disk volumes, but compilation tests do not prove portable,
atomic live secret population.

The normal upstream reconciliation path does not expose telemetry-header bytes to this provider;
the compiler's optional telemetry input remains empty there. Managed port forwarding and
cross-gateway topology integration are also open.

The package advertises `RequiresJit=false` and `IsAotCompatible=true`; this is a scoped Docker
choice, not a platform-repository mandate.

Return to [Platforms](../index.md).

## Sources

- **Area behavior** — `cohesion-platforms/platforms/Docker/README.md`.
- **Project design** — `cohesion-platforms/platforms/Docker/Assimalign.Cohesion.ApplicationModel.Gateway.Docker/docs/DESIGN.md`.
- **Provider metadata** — `cohesion-platforms/platforms/Docker/Assimalign.Cohesion.ApplicationModel.Gateway.Docker/buildTransitive/Assimalign.Cohesion.ApplicationModel.Gateway.Docker.props`.
- **SDK selection** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Tasks/docs/OVERVIEW.md`.
