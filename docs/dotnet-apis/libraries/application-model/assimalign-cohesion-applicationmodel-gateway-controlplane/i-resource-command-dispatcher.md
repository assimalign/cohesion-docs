# IResourceCommandDispatcher

The protocol-client boundary between a serving gateway and an area resource control plane.

[Assembly overview](index.md) · [Design](design.md) · [Examples](examples/index.md)

Namespace: `Assimalign.Cohesion.ApplicationModel.Gateway.ControlPlane`.

Assembly: `Assimalign.Cohesion.ApplicationModel.Gateway.ControlPlane`.

## Remarks

The protocol-client boundary between a serving gateway and an area resource control plane.
Implementations identify one manifest resource kind and apply or delete the neutral
`ResourceCommand` envelope at the resource's observed default control-plane `System.Uri`. Each
dispatch receives the target resource's current bootstrap bearer credential from the serving
gateway.

`ApplyAsync` and `DeleteAsync` require a
`RemoteCertificateValidationCallback? serverCertificateValidator` parameter immediately before
the optional cancellation token. Apply it to the outbound transport. The serving gateway obtains
it from `IResourceTransportTrustProvider.CreateOutboundTrustValidator(application)` for HTTPS
targets, using its probe/store transport anchors. HTTP targets and applications without anchors
pass `null`, which retains platform default trust.

## Members

| Member | Responsibility |
|---|---|
| `ResourceKind` | Exact resource manifest kind handled by the dispatcher. |
| `ApplyAsync` | Applies a neutral resource command using the observed control-plane address and current bootstrap credential. |
| `DeleteAsync` | Deletes an owned command through the same transport and credential boundary. |
| `serverCertificateValidator` parameter | Required on apply and delete before the optional cancellation token; `null` retains platform default trust. |

## Sources

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway.ControlPlane/docs/Assembly/Assimalign.Cohesion.ApplicationModel.Gateway.ControlPlane/IResourceCommandDispatcher/OVERVIEW.md`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway.ControlPlane/src/IResourceCommandDispatcher.cs`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway.ControlPlane/src/IResourceCommandDispatcher.cs`.
