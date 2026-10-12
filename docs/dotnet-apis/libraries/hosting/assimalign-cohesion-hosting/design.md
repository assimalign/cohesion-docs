# Assimalign.Cohesion.Hosting design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Hosting`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

Plain Hosting depends only on Core. Host execution and run observers do not own resource mounts,
process signals, control planes, or health reporting. Those opt-in concerns extend the run seam from
sibling packages rather than changing every host.

## Lifecycle coordinator

A normal start/stop cycle runs these phases:

`OnStartingAsync` → services `StartingAsync`/`StartAsync`/`StartedAsync` → `OnStartedAsync` →
running → `OnStoppingAsync` → services `StoppingAsync`/`StopAsync`/`StoppedAsync` →
`OnStoppedAsync`

Startup is dependency-sensitive: serial startup stops at the first failure, while configured
concurrent startup collects failures. Either path performs best-effort reverse-order compensation,
marks the host `Failed`, completes its run signals, and preserves the original startup exception.
This finalization also runs when startup budget setup or rollback service resolution/enumeration
throws. A retry executes a fresh attempt and surfaces a persistent failure again. Compared with
10.0.0-preview.1, such retries now throw instead of silently succeeding while stuck in `Starting`
(#1079; behavioral breaking change).

Graceful shutdown is best-effort. Every service receives a stop attempt, in reverse registration
order or concurrently according to `StopServicesConcurrently`, and failures are reported after the
host reaches `Stopped`. Stopping an `Idle`, already stopping, `Stopped`, or compensated `Failed`
host is a no-op.

Per-run cancellation signals that shutdown should begin; it is not the graceful-drain budget.
`StopAsync` creates a fresh linked token and applies `ShutdownTimeout`, preventing the already
cancelled run token from pre-cancelling every service drain. Coordinator-owned reset makes a cleanly
stopped or compensated host restartable without relying on a derived hook calling `base`.

A run token already cancelled at entry performs one complete lifecycle: startup uses
`CancellationToken.None`, followed immediately by graceful shutdown with a fresh stop budget.
The run completes in `Stopped` and delivers the normal observer sequence. A startup failure still
rolls back to `Failed` and propagates. Both the concrete host and the plain `IHost` extension own
this semantic; applications need no shadows. Stop completion is joined only if stopping actually
began, so a rejected stop cannot strand a run on a completion signal that will never fire.

`IHostContext.WaitForShutdownAsync` is the public lifecycle observation seam. It completes when
shutdown is requested or the current lifetime begins stopping, stops, or fails; cancelling one
wait abandons only that caller and does not signal the host. A later start creates a fresh signal.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Core`. The [overview](index.md#dependencies)
distinguishes project references, package references, shared source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting/src/Assimalign.Cohesion.Hosting.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting/src`.
