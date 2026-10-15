# InProcessGatewayOptions

`InProcessGatewayOptions` extends `ApplicationGatewayOptions` with settings specific to nested resource hosts.

[Assembly overview](index.md) · [Design](design.md) · [Examples](examples/index.md)

Namespace: `Assimalign.Cohesion.ApplicationModel.Gateway.InProcess`.

Assembly: `Assimalign.Cohesion.ApplicationModel.Gateway.InProcess`.

## Remarks

`InProcessGatewayOptions` extends `ApplicationGatewayOptions` with settings specific to nested
resource hosts.

- **Contract** — `StateDirectory` roots persisted loopback ports and private claims.

- **Contract** — `ProbeInterval` and `ProbeTimeout` bound health observation.

- **Contract** — `LivenessFailureThreshold` defaults to three consecutive failures.

- **Contract** — `InitialRestartBackoff`, `MaximumRestartBackoff`, and `MaximumRestartAttempts` bound retries.

Invalid empty paths, non-positive probe durations or thresholds, negative retry values, and a
maximum backoff below the initial backoff are rejected by the `InProcessGateway` constructor.

## Members

| Member | Responsibility |
|---|---|
| `StateDirectory` | Root for persisted loopback ports and private claims. |
| `ProbeInterval` | Interval between health observations. |
| `ProbeTimeout` | Duration bound on a health probe. |
| `LivenessFailureThreshold` | Consecutive failures before action; defaults to three. |
| `InitialRestartBackoff` | Initial restart delay. |
| `MaximumRestartBackoff` | Upper bound on restart delay. |
| `MaximumRestartAttempts` | Bound on restart attempts. |

## Sources

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway.InProcess/docs/Assembly/Assimalign.Cohesion.ApplicationModel.Gateway.InProcess/InProcessGatewayOptions/OVERVIEW.md`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway.InProcess/src/InProcessGatewayOptions.cs`.
