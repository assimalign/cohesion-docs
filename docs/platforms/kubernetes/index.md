# Kubernetes

The Kubernetes gateway compiles resource plans into cluster objects and observes their lifecycle and endpoints.

> **Status:** Partial. Plan realization, Kind archive loading, system installation, and discovery work; several topology and forwarding seams remain open.

## Package and selection

The package is `Assimalign.Cohesion.ApplicationModel.Gateway.Kubernetes`. Include `Kubernetes` in
`CohesionGateways`, then select `--gateway kubernetes` through generated `UseGateway(args)`.
The `buildTransitive` provider item names `KubernetesGateway`, `KubernetesGatewayOptions`, and
`KubernetesGatewayCommandLine.Apply`.

```xml
<PropertyGroup>
  <CohesionGateways>Local;Kubernetes</CohesionGateways>
</PropertyGroup>
```

Use `CohesionPlatformsVersion` for the platform package identity; see
[provider setup](../index.md#selecting-a-provider). The provider advertises `RequiresJit=true`,
so automatic gateway compilation uses just-in-time (JIT) execution rather than NativeAOT.
Direct composition also exposes `UseKubernetesGateway`.

## Plan compilation and observation

| Plan fact | Kubernetes objects |
|---|---|
| `Deployment`, `StatefulSet`, `DaemonSet`, or `Job` | The matching workload controller. |
| Endpoint | Service and any declared exposure; StatefulSet also receives a governing headless Service. |
| Persistent claim | PersistentVolumeClaim (PVC) or StatefulSet claim template. |
| Configuration input | ConfigMap and projected files. |
| Secret/certificate input | Secret projection; certificate PEM remains one unsplit value in an Opaque Secret. |
| Health and runtime facts | Probes, runtime-contract environment, and deterministic ownership metadata. |

`KubernetesPlanCompiler` translates plans through one generic path. `KubernetesPlanController`
uses level-triggered server-side apply and checks ownership before writes. It prunes obsolete owned
objects and replaces changed immutable Jobs. Removed-resource persistent claims are retained.

One list/watch observer publishes workload state and ready Service endpoints. Stable Service DNS
addresses are used for in-cluster dependencies. Rotating runtime inputs can roll workloads without
changing `cohesion.io/plan-hash`, which hashes the plan only. Unsupported immutable changes fail
before applying objects.

## Kind on Podman

The local development target is Kind on Podman. With a Development model, selected
`kind-<cluster>` context, and an advertised image archive, gathering invokes
`kind load image-archive` before cluster-client startup. The child process inherits
`KIND_EXPERIMENTAL_PROVIDER=podman`. Successful loads are deduplicated by context and digest.

`ImageIndexPath` selects the application's validated image index. Every resource resolves its own
digest-pinned entry. A concrete entry registry is preserved. `ContainerRegistry` applies only when
the entry registry is omitted or null. If Kind is unavailable, gathering warns; a usable configured
registry must then supply the late-bound identity or gathering fails. General registry reachability
from arbitrary nodes remains separate work.

The opt-in archive-load smoke needs a running Podman engine, Kind and kubectl, an existing cluster,
and a matching prebuilt archive. That smoke validates image loading, not the complete installation,
external exposure, or authentication story.

## Rendering, bootstrap, and discovery

`--mode render` emits system objects followed by application plans in declaration order without
gathering images or contacting Kubernetes. Unresolved mounts are preview shapes with empty inputs.
`--mode bootstrap` emits and applies the system installation by default;
`--bootstrap-apply=false` emits it offline.

| Installation option | Contract |
|---|---|
| `SystemImage` | Required digest-pinned gateway executable image. |
| `SystemStorageSize` | Required persistent state/export capacity. |
| `SystemNamespace` | Defaults to `cohesion-system`. |
| `SystemServiceAccount` | Defaults to `cohesion-gateway`. |
| `SystemExposure` | `None`, `LoadBalancer`, or `Ingress`; defaults to `None`. |
| `SystemIngressHost` and `SystemIngressClass` | Both required for Ingress. |

System output includes a namespace, service account, role bindings, state PVC, trust-key Secret,
one-replica Deployment, and `cohesion-control-plane` Service. System labels separate it from
application-resource pruning. External Transport Layer Security (TLS) termination is an operator
concern because the upstream control-plane server binds HTTP.

The secret-free `cohesion-export` ConfigMap carries `export.json` and `control-plane.json`.
`ImportFromKubernetes` returns `IKubernetesApplicationModelResolver`; its
`ResolveControlPlaneAddressAsync` obtains a URL for `remote.Gateway(url)`. Reading this operator
channel uses kubeconfig identity and does not place bearer tokens into ConfigMaps.

## Lifecycle and limits

`StopAsync` releases observation and supervision while retaining cluster state. Teardown removes
managed application objects and, when all required built-in delete participants succeed, the
application namespace. System infrastructure is preserved. Custom-controlled or external-only
models retain namespaces when upstream contracts cannot report the needed deletion outcomes.

One gateway reconciles one cluster. Cross-cluster composition uses peer gateways and exported
models. Live multi-model sessions cannot share the upstream fixed-route system control-plane
listener; they fail explicitly. Offline multi-model rendering remains a review surface.

Managed development port forwarding, production root-owner role enforcement, and ordinary
reconciliation of telemetry-header bytes need additional upstream contracts. Kubernetes also
rejects `--realize`; Local, InProcess, and Docker are the supported development alternatives.

Return to [Platforms](../index.md).

## Sources

- **Area behavior and limits** — `cohesion-platforms/platforms/Kubernetes/README.md`.
- **Project overview** — `cohesion-platforms/platforms/Kubernetes/Assimalign.Cohesion.ApplicationModel.Gateway.Kubernetes/docs/OVERVIEW.md`.
- **Provider metadata** — `cohesion-platforms/platforms/Kubernetes/Assimalign.Cohesion.ApplicationModel.Gateway.Kubernetes/buildTransitive/Assimalign.Cohesion.ApplicationModel.Gateway.Kubernetes.props`.
- **SDK selection** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Tasks/docs/OVERVIEW.md`.
- **Runtime inputs** — `cohesion/docs/RUNTIME_CONTRACT.md`.
