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
(#1409). `HealthContribution` has no exception member, so the control plane does not keep the
exception; a contributor that wants its failure diagnosed records it itself before throwing, or
returns a contribution whose description it has chosen.

The same holds for an exception a contributor returns rather than throws. An `Exception` value in
`HealthContribution.Data` is replaced in the report by the fixed text
`The contributor reported an exception.`; keys, the other values and their order are kept. Area
writers serialize a data value they do not recognize through `ToString()`, and an exception's is its
type, message, inner exceptions and stack trace, so through 10.0.0-preview.1 the probes published
all of it. The control plane is the one place every shipped area's report passes, so the check
lives here, as it does for a thrown exception. The two area writers that serialize data
(Web.Hosting.Resources' `ResourceControlPlaneMiddleware` and ConfigurationStore.Hosting's endpoint)
also write the same text for an `Exception` value, as defense in depth for a custom
`IResourceControlPlane`, whose report reaches them unfiltered. Descriptions and string data are the
contributor's own text and are reported as returned.

The control plane is transport-neutral. Area Hosting packages decide how to expose health and
commands over HTTP or another protocol.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Core`, `Assimalign.Cohesion.Hosting`,
`Assimalign.Cohesion.Hosting.Health`, `System.Security.Cryptography.ProtectedData`. The
[overview](index.md#dependencies) distinguishes project references, package references, shared
source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting.Resources/src/Assimalign.Cohesion.Hosting.Resources.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting.Resources/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting.Resources/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting.Resources/src`.
