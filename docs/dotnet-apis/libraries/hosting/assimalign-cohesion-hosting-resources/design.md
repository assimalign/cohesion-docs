# Assimalign.Cohesion.Hosting.Resources design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Hosting.Resources`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

Resources composes the Hosting run seam and Health vocabulary without reverse dependencies. Resource
SDKs opt into generated `ResourceRuntime` calls; assembly presence alone does not activate
supervision. Invocation context owns environment snapshots, mounts, control planes, and health
aggregation.

## Health reports withhold exceptions

`ResourceControlPlane.Create` returns a fresh control plane. A health query snapshots its
contributors, runs their asynchronous checks, and returns `ResourceHealthReport` with the least
healthy status; an empty set is healthy.

A contributor that throws, including with an `OperationCanceledException` the caller did not
request, becomes an `Unhealthy` contribution described by the fixed text
`The contributor threw an exception.`; caller cancellation propagates instead. The description is
never the exception's message. Area hosts publish these reports on the bare `/healthz`, `/readyz`
and `/livez` probes, which carry no credential check, and a dependency's exception message routinely
names hosts, databases, users and ports (CWE-209). Through 10.0.0-preview.1 the control plane
recorded the message as the description, so every area that exposes the probes published it
(#1409). The catch hands the exception to this assembly's internal event source before returning
the fixed report (#1400). `HealthContribution` retains no exception member, and the control plane
does not retain the exception after the event is written.

The same holds for an exception a contributor returns rather than throws. An `Exception` value in
`HealthContribution.Data` is replaced in the report by the fixed text
`The contributor reported an exception.`; keys, the other values and their order are kept. Area
writers serialize a data value they do not recognize through `ToString()`, and an exception's is its
type, message, inner exceptions and stack trace, so through 10.0.0-preview.1 the probes published
all of it. The control plane is the one place every shipped area's report passes, so the check
lives here, as it does for a thrown exception. The two area writers that serialize data
(Web.Hosting's `ResourceControlPlaneMiddleware` and ConfigurationStore.Hosting's endpoint)
also write the same text for an `Exception` value, as defense in depth for a custom
`IResourceControlPlane`, whose report reaches them unfiltered. Descriptions and string data are the
contributor's own text and are reported as returned.

The control plane is transport-neutral. Area Hosting packages decide how to expose health and
commands over HTTP or another protocol.

## Diagnostics

The internal `HostingResourceEventSource` is named `Assimalign.Cohesion.Hosting.Resources`.
It reports contributor failures and command execution outcomes from every area that uses the
shared control plane, including direct calls without an HTTP adapter. A tool can enable the provider by its assembly name;
an application can also configure its own event-source forwarder. Non-Web resource processes
receive no default forwarding from a nested Web host. This provider is outside the default
`Assimalign.Cohesion.Web.` prefix, so Web entry points also select it explicitly when forwarding.
No logging dependency is added here.

| Id | Event | Level | Payload | Throttle |
| ---: | --- | --- | --- | --- |
| 1 | `ContributorFailed` | Error | `contributorName`, `exceptionType`, `exceptionMessage`, `suppressedCount:int` | Once per second, process-wide |
| 2 | `CommandApplied` | Informational | `kind`, `operation` | None |
| 3 | `CommandUnknownKind` | Warning | `kind` (empty), `operation`, `suppressedCount:int` | Once per second, process-wide |
| 4 | `CommandConflict` | Warning | `kind`, `operation`, `suppressedCount:int` | Once per second, process-wide |
| 5 | `CommandHandlerFailed` | Error | `kind`, `operation`, `exceptionType`, `suppressedCount:int` | Once per second, process-wide |

The `contributor-failures` polling counter is the exact total of contributor exceptions caught
while aggregating health, readiness, and liveness. Its backing value increments even without a
listener and for suppressed events; the counter is created lazily when a listener enables the
source. Caller-requested cancellation propagates and neither raises an event nor increments the
counter. An unrelated contributor cancellation is a failure. There is no gauge.

Command outcomes are counted once per completed dispatch, even without listeners or when their
events are throttled. Their polling counters are also created lazily on the first enable:

| Counter | Exact transition |
| --- | --- |
| `commands-applied` | Successful apply or delete, including an identical replay or an already-absent deletion |
| `unknown-command-kinds` | No registered handler exists for the supplied kind; the HTTP adapter maps this to 501 |
| `command-conflicts` | The ledger or handler throws `ResourceCommandRejectedException`; HTTP maps this to 409 |
| `command-handler-failures` | Another exception escapes the actual handler invocation, except caller-requested cancellation |

`operation` is the fixed value `Apply` or `Delete`. A `kind` is written only after lookup in the
registered-handler dictionary succeeds. An unknown or merely advertised kind is written as the
empty string, never copied from the request. No command payload, key, identifier, owner/caller
identity, credential, exception message, or exception data is written. Command handler faults use
the event-source rule's type-only exception carve-out. A handler's `NotSupportedException` or an
independent cancellation is a handler failure; neither means its already-registered kind was
unknown. Cancellation requested by the caller propagates without any outcome counter or event.

Each throttled event has its own process-wide one-second window. Its next emission carries the
number of suppressed enabled-listener occurrences, saturating at `Int32.MaxValue`; all counters
remain exact. Applied outcomes indicate a successful dispatch, not necessarily a repeated
mutation: concurrent identical declarations execute their handler once while every replay
receives its own success outcome. An already-absent deletion also succeeds without invoking a
handler. No event is raised while holding the ledger's snapshot lock.

**Command boundary (#1434).** This source owns command execution; Web.Hosting owns
the HTTP envelope and status mapping. When a handler throws, an internal stateless helper marks
the original invocation in the exception's existing `Data` dictionary after recording the failure.
The marker is a BCL `ConditionalWeakTable<ResourceCommand, object>` with weak invocation keys,
so a reused exception can track concurrent calls without retaining their payloads. The Web
adapter consumes only its exact invocation marker and retains HTTP 400 for handler JSON, format,
and argument exceptions without also raising `CommandMalformed`. Custom control planes that do
not record an execution failure still produce the HTTP source's `phase=Handler` event. The helper
is shared source, not a shared provider or a shipped visibility grant; neither a new public
diagnostics API nor a command dependency on Web is introduced. A custom exception with unusable
`Data` prevents the private hand-off; diagnostics preserve the original exception and HTTP
behavior, but that exceptional case can also produce the HTTP malformed event.

Contributor names come from registered application contributors. Health-check exceptions follow
the event-source rule's application-exception policy: the type and `Message` are written, never
`ToString()`, stack traces, inner exceptions, or contribution data. If an exception overrides
`Message` with a throwing getter, the event writes `The exception message could not be read.`
instead, preserving the fixed health report. A dependency's message may
contain host, database, or user names, so configured sinks must be treated accordingly. The HTTP
report continues to contain only the fixed failure description introduced by #1409. This is an
additive diagnostic change from 10.0.0-preview.1; report and command behavior are unchanged.

**Boundary choice for #1400.** The internal hand-off remains inside Hosting.Resources: the
aggregation catch invokes this assembly's source directly. Web.Hosting cannot recover
the exception from the sanitized `ResourceHealthReport`. Routing it to that assembly's source
would require a new public exception/diagnostics seam or a shipped `InternalsVisibleTo` grant.
Neither is introduced. This producer-owned event is the smallest local alternative under the
session boundary rules; its placement differs from #1400's requested Web-source ownership and is
recorded as an escalation. The Web source owns HTTP credential refusals, malformed requests,
and stop acceptance, and does not duplicate contributor events. No ambient resource name is
read: this transport-neutral control plane has no reliable resource-context association.

## Standalone HTTP access (#1431)

A missing `ResourceContext.GatewayName` enables standalone development, not remote management.
Area HTTP adapters admit `/cohesion/v1` and every path beneath it only when the transport's remote
endpoint is an IP endpoint at `127.0.0.1`, `::1`, or IPv4-mapped `127.0.0.1`. Every other peer,
including an absent or non-IP endpoint, receives **403** before any management operation. The
explicit status explains the restriction. Forwarded headers never establish this trust. The
transport-neutral control-plane contract needs no HTTP dependency or new public member; enforcement
belongs to Web.Hosting's shared middleware and the area endpoints that handle their own
routes (ConfigurationStore, SecretStore, IdentityHub, LogSpace's log query, and Database's early
namespaced health mappings). Scheduler already requires a gateway identity to install its
control-plane listener.

Bare `/healthz`, `/readyz`, and `/livez` remain open for unauthenticated orchestrator and load-balancer
probes; their namespaced aliases are restricted. Existing listener gates still apply. A resource
with a gateway identity retains its existing credential policy, on loopback and remote connections.
This is a behavioral breaking change from 10.0.0-preview.1, accepted under owner decisions 46 and 15:
previously an unauthenticated remote caller could use standalone management routes on a reachable
listener. Standalone tools must connect over loopback; remote management requires gateway identity
and credentials. SecretStore also retains its existing standalone loopback bind restriction.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Core`, `Assimalign.Cohesion.Hosting`,
`Assimalign.Cohesion.Hosting.Health`, `System.Security.Cryptography.ProtectedData`, and this
package's `shared/CommandFailureDiagnostics.cs` through `CohesionSharedSource`. The
[overview](index.md#dependencies) distinguishes project references, package references, shared
source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting.Resources/src/Assimalign.Cohesion.Hosting.Resources.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting.Resources/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting.Resources/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting.Resources/src`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting.Resources/shared/CommandFailureDiagnostics.cs`.
