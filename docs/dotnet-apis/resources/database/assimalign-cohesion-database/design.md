# Assimalign.Cohesion.Database design

This page describes the boundaries and implementation shape of `Assimalign.Cohesion.Database`.

> **Status:** Partial.

The area root (architecture: resources/`Database`/DESIGN.md
(`cohesion/docs/resources/Database/DESIGN.md`)). The lifecycle and composition bases serve all five data models. Execution capability
bases serve four typed models and three language models. Model requests, parsers and
servers live in model packages; Hosting consumes the common lifecycle root and execution
callers use the concrete model session.

The root is also the area's **rollup**: it references every child root — the independently
consumable base components a database is made of (`Database.Types`, `Database.Language`,
`Database.Storage`, `Database.Transactions`, `Database.Execution`, `Database.Protocol`,
`Database.Security`, `Database.Indexing`) — so one reference to the root delivers the whole base
surface. Child roots never reference the root.

## Phase 29 composition contract (current)

The approved hosting composition (`cohesion/docs/programs/DATABASE_HOSTING_DESIGN.md`) supersedes
the historical builder/worker descriptions below. `IDatabaseApplicationBuilder` exposes exactly
borrowed `AddEngine(instance)`, owned
`AddEngine(Func<IDatabaseApplicationContext, IDatabaseEngine>)`, and one-shot `Build()`. There is
no builder engine enumeration, application-level `AddServer`, or `Use` stage. `IDatabaseApplication`
inherits `IAsyncDisposable`; its context observes every engine, nested servers, and ordinal
`GetEngine(name)` lookup. All four named engine operations use the existing `DatabaseName` value
object; its implicit string conversions preserve straightforward callers while implementations use
the typed contract.

`IDatabaseEngineBuilder` earns a shared seam because `AddWorker` is model-agnostic: the same factory
can attach a worker to any model. Shared construction/rollback source is owned in this project's
`shared/` and compiled by each model through `CohesionSharedSource`. Every model-specific interface
extends the base and carries that model's options. No separate generic application composition
algorithm consumes arbitrary model options. Strongly typed worker/server factory overloads are
deliberately omitted: the common engine factory contract already works, and explicit casts for
SQL/KeyValue servers avoid overload ambiguity and a second factory vocabulary.

Workers remain scheduled and quiesced by their engine.
`IDatabaseEngineWorker.Run(CancellationToken)` exposes the existing executable pump so workers
returned by the approved deferred factories can run without requiring a particular base
implementation. `IDatabaseEngine.Servers` enables hosting to discover nested servers. Engines own
factory-produced workers and servers; hosting snapshots servers for lifecycle only.
`Application`-created engines are disposed by the application; instance-registered engines remain
caller-owned.

```mermaid
classDiagram
    IDatabaseApplicationBuilder --> IDatabaseApplication : Build
    IDatabaseApplication --> IDatabaseApplicationContext : Context
    IDatabaseApplicationContext --> IDatabaseEngine : Engines
    IDatabaseEngineBuilder --> IDatabaseEngine : Build
    IDatabaseEngine --> IDatabaseServer : Servers
    IDatabaseEngine --> IDatabaseEngineWorker : Workers
```
## Why-this-not-that decisions

- **Child roots roll up under the root; they never reference it** (owner
  decision, 2026-07-13 — inverts the earlier Protocol→root and Transactions→root
  references). Unlike the Web area, a database has a vast base-component surface;
  the child roots exist to break it out for separation of concerns and
  testability, and each must stay *independently consumable* — a tool that only
  speaks the wire protocol, or a storage engine experiment, should not drag the
  area contracts in. That is only true if the dependency arrow points root →
  child. The rejected alternative — child roots referencing the root for shared
  vocabulary (`DatabaseException` ancestry, `TransactionId`, `ProtocolVersion`) —
  made two children (`Protocol`, `Transactions`) unaggregatable and forced the
  hosting module into a `COHRES002` exemption for the server's own machinery. With
  the inversion, vocabulary lives with its owning child (`ProtocolVersion` in
  `Protocol`, `TransactionId`/`TransactionState` in `Transactions`), the root
  consumes it through its child-root references, and consumers reach it
  transitively through the root. `Database.Indexing` joined the child
  roots on 2026-07-13 (owner direction): its only root coupling was exception
  ancestry, re-rooted onto its own `IndexException` — index infrastructure is a
  base component like storage and transactions.

- **Engine → database → session → transaction as four contracts**, not one god
  interface. Each has a distinct lifetime and threading model: engines are
  process-long and thread-safe; databases are shared handles; sessions are
  cheap, single-threaded execution scopes; transactions are explicit ACID
  brackets inside a session. Collapsing them (an `ExecuteAsync` on the engine,
  say) would smuggle session state into a shared object.
- **Engines are data machines — create → use → dispose, no lifecycle members**
  (owner decision, 2026-07-13 — **reverses the #903 decision** that put
  `StartAsync`/`StopAsync` on the root engine contract, and supersedes the
  short-lived `IDatabaseEngineLifecycle` segregation from earlier the same day,
  which was deleted before it ever shipped in a release). The new information
  that changed the calculus: once servers became per-model (below), "running"
  had an owner — the server fronts the engine on the network and is the thing
  that starts and stops — and the engine's start/stop ceremony was revealed as
  accidental service-shape, not data-machine substance. An engine is fully
  operational from creation (its background workers spawn with it) and disposal
  is its one transition: quiesce workers → durable flush → close databases.
  What #903 actually needed — a host that can *align* engine durability with
  its own lifecycle — is satisfied by disposal alone: the composition root that
  created the engine disposes it, and committed work is durable when
  `DisposeAsync` completes. The rejected alternative (keeping idempotent
  start/stop for restartability) bought a restartable engine object nobody
  needed — a "restart" is creating a fresh engine over the same storage root,
  which the recovery path already makes correct — at the cost of a
  four-state machine on every engine and a start-order protocol between host
  and composition root.
- **A minimal observational `State` stays on the engine** (judgment call,
  recorded): the approved data-machine contract needs no state machine, but
  worker-fault reporting needs *somewhere* to surface — the old contract threw
  the recorded fault from `StopAsync`, and with stop gone the only alternatives
  were throwing from `DisposeAsync` (hostile to `await using`, masks in-flight
  exceptions) or silence. `EngineState` therefore shrank from a six-state
  lifecycle enum to three observational conditions: `Running` (from creation),
  `Faulted` (a background-worker fault was recorded; the engine keeps serving —
  grouped commits self-help, checkpoints just stop truncating — but the owner
  should learn it runs degraded), `Disposed`. The default control-plane health
  aggregate delivered by #973 reads this surface; nothing drives transitions
  from outside.
- **The application exposes its composition through `IDatabaseApplicationContext`,
  and the context is plural** (owner direction, 2026-07-13 — the `Database`
  instance of the Web area's `IWebApplicationContext` pattern, converged with
  Web's shape). The context carries `Servers` (registration order — plural
  because servers are per-model, so one application may front SQL and Documents
  engines through two servers) and `Engines` (the server-less, embedded
  registrations; an engine fronted by a server is reachable through that
  server's context). `IDatabaseApplication` is the Web shape exactly: `Context`
  plus `StartAsync`/`StopAsync` (the loose `Engines` member it briefly carried
  is gone). Deferred composition callbacks on the builder receive the context —
  `AddServer(Func<IDatabaseApplicationContext, IDatabaseServer>)`, mirroring
  Web — replacing the earlier engine-list-receiving factory: the context view
  lets a factory observe servers registered ahead of it, not just engines, and
  keeps the callback signature stable as the context grows. An earlier cut of
  this contract (same day, superseded before merge) exposed a single nullable
  `Server`; per-model servers made plurality structural, not optional.
- **Execution belongs to session capabilities (owner reversal, 2026-10-16; #1576).**
  The earlier decision put two execute members on every `DatabaseSession` to serve one
  shared network front-end. Per-model servers replaced that front-end, removing its reason:
  Blob has no execution language, and KeyValuePair's command grammar is an internal wire
  encoding. The root session now owns lifecycle, state and explicit transactions only.
  `DatabaseExecutionSession<TRequest>` adds typed execution; Sql, Documents and Graph
  derive `DatabaseLanguageSession<TRequest>` for typed and text execution. KeyValuePair
  derives only the typed base, and Blob directly derives `DatabaseSession`.
  Both bases constrain requests to `DatabaseRequest`, validate in public non-virtual
  members, and call protected abstract cores. A foreign-model request fails compilation.
  The language base's typed forwarder is declared before its text member and calls the
  public typed NVI, so the compiler names the model's request type in its diagnostic.
- **The isolation-level seam consumes the Transactions child's enum directly**
  (2026-07-13, with the MVCC-integration design — area DESIGN.md §3.8).
  `DatabaseSession.BeginTransactionAsync(IsolationLevel, …)` and
  `DatabaseTransaction.IsolationLevel` speak `Database.Transactions`'
  `IsolationLevel` — the same pattern as `TransactionId`/`TransactionState`:
  post-inversion, the root consumes child vocabulary rather than duplicating
  it. The rejected alternative — a root-owned isolation enum mapped onto the
  child's — would create two vocabularies for one concept and a translation
  layer with no owner. The contract deliberately allows engines to run
  *stronger* than requested (the SQL engine's page-grain serialization today),
  never weaker, so the seam could land ahead of the MVCC session binding
  without lying about semantics. No speculative MVCC contracts were added to
  the root: the manager's contracts already live in the Transactions child
  root, and the binding is a model-engine concern (the design doc explains the
  placement).
- **`DatabaseParseException` as a first-class error category.** `Parse` failures
  and execution failures have different wire error codes (`ParseFailure` vs
  `ExecutionFailure`) and different caller responses (fix the text vs inspect
  the data). A subtype of `DatabaseException` keeps existing `catch` blocks
  working while giving the server an exact mapping — better than string-matching
  messages or per-model exception knowledge in the server.
- **`DatabaseRequest`/`DatabaseResult` live in `Database.Execution`, not here.** The
  root aggregates the execution family rather than owning it: execution is its
  own child root for model-neutral request/result contracts, referencing Types only.
  The unused shared pipeline was deleted in #1257. The same holds for the other child-root vocabularies the
  root's contracts speak: `TransactionId`/`TransactionState` live in
  `Database.Transactions` (`IDatabaseTransaction` consumes them), and
  `ProtocolVersion` lives in `Database.Protocol` (`IDatabaseServerSession`
  consumes it).
- **Background workers are engine-owned, unconditionally — the claim handshake
  is gone** (owner decision, 2026-07-13; supersedes the #902 claim model). The
  engine spawns its worker loops at creation — the latency-critical flusher and
  write-back loops on dedicated threads the engine itself owns, satisfying the
  Lane-H dedicated-thread guardrail with no host involvement — and quiesces
  them on dispose. `IDatabaseEngineWorker` shrank to an **observational**
  contract (name, kind, cadence — what a diagnostics or health surface needs);
  the pump machinery (`Run`/`RunIteration`/`WaitForTrigger`) lives on the
  guided base `DatabaseEngineWorker` for the owning engine's internal use only.
  The rejected (previous) design — `TryClaim`/`Release` plus host worker slots
  mapping claimed workers onto the hosting execution menu — existed to let a
  host own worker scheduling; per R10 the engine had to own the *work* anyway,
  so the handshake bought configurability nobody used at the price of a
  two-owner protocol whose failure modes (claim races, disabled slots,
  half-claimed inventories) all had to be designed away. One owner, no
  handshake: a worker can never run twice because exactly one engine-internal
  scheduler exists. Workers remain synchronous by design — every body is
  storage I/O (fsync, page writes, checkpoint), which has no async fast path.
- **Server *contracts* live here — and they are the only area-wide server
  requirement; servers are per-model, each model package carrying its own copy
  of the server machinery** (owner decision 2026-07-14, settled on review of
  the second model server's extraction evidence). A server fronts exactly
  **one** engine — `IDatabaseServerContext.Engine` is singular — so
  model-specific wire behavior has a home. Each model ships its own
  `IDatabaseServer`/`IDatabaseServerContext`/`IDatabaseServerSession`
  implementation in its model package (`SqlDatabaseServer` in `Database.Sql`,
  `KeyValueDatabaseServer` in `Database.KeyValuePair`), with the machinery
  (accept loop, session state machine and frame pump, guardrails, two-phase
  drain) internal to that package. The placement history is deliberate
  evidence discipline: the shared base that briefly existed at n=1 was folded
  into `Database.Sql` with an extraction trigger recorded; the second model
  server fired it and the proven core was extracted into a shared
  `Database.Server` — and the owner then reviewed that evidence and chose
  per-model duplication anyway (model independence over shared code; drift
  cost accepted; wire parity held by the protocol contract and per-model E2Es —
  the preserved prediction-vs-evidence table lives in the area DESIGN §3.10).
  The contracts stay here for the same `COHRES001` reason as before: feature
  libraries (quotas #167, health, `Database.Testing`) must be
  able to name the server without referencing any runtime. The context shape
  (`Context` = engine + sessions) mirrors the application context pattern —
  observational composition on a context, lifecycle on the owning object.
- **The application builder is a root seam; the implementation is not** (owner
  direction, 2026-07-13). `IDatabaseApplicationBuilder`/`IDatabaseApplication`
  live here so **model packages register their engines and servers without
  knowing the hosting implementation**: `Database.Sql` ships `AddSqlDatabase(...)` and
  `AddSqlServer(...)` as `extension(IDatabaseApplicationBuilder)` members and
  never references `Database.Hosting` (`COHRES001` intact); the hosting module
  ships the implementation (`DatabaseApplicationBuilder`) and the creation
  entry point (`DatabaseApplication.CreateBuilder()`). The concrete
  `DatabaseApplicationBuilder.AddService` in `Database.Hosting` accepts plain Hosting
  service instances and context factories; those services start before servers and
  stop after them in reverse order. The root references no hosting library and
  exposes no service-registration verb (O34). Multiple `AddServer` registrations are
  allowed — servers are per-model. This mirrors the Web area's placement
  (the root seam `IWebApplicationBuilder` in the `Web` root, the creation entry
  point `WebApplication.CreateBuilder()` in `Web.Hosting`, and registration
  verbs with the feature package, `AddAuthentication` in `Web.Authentication`;
  Web's are `builder.Services` component integrations since #1380, while
  Database's stay `extension(IDatabaseApplicationBuilder)` members) — and the
  pattern is the **cross-area expectation**: every area root provides
  `I<Area>ApplicationBuilder`, and feature/model registration verbs ship with
  their feature package (see `.claude/rules/resource-areas.md`). The rejected
  alternative — a builder type in the hosting module — would force every model
  package that wants a registration verb to reference the composition surface,
  which is precisely what the hosting-isolation rule forbids.
- **Model-specific schemas belong to their model family** (MVP features A1–A4,
  2026-09-17). `CompiledSchema` is an abstract model-agnostic identity carrying
  `Format`, `Name`, `Model`, `AllowsDestructiveChanges`, and the model's canonical
  document. The root computes SHA-256 over that document without inspecting its
  shape. `IDatabaseSchemaProvisioner` remains the common apply seam, returning
  the readonly `SchemaMigrationResult` value (`FromHash`, `ToHash`,
  `OperationCount`, `WasAlreadyApplied`). Relational declarations, builders,
  validation, serialization, and migration planning moved to the thin
  `Database.Sql.Schema` package, which build tooling can consume without the SQL
  engine or network stack. The former root schema's collection shape was removed;
  the document model will define its own vocabulary in its own model family.
  If another model needs a different shape, it does not belong in this root.
- **`Object` ownership separates code-first provisioning from ad-hoc statements.**
  `DatabaseObjectOwner.Adhoc` objects remain fully mutable through session
  statements. `DatabaseObjectOwner.Schema` objects can change only through schema
  apply; a session attempting to alter or drop one receives
  `DatabaseObjectLockedException` identifying the object, its compiled
  `OwningSchema`, and the operation. `OwningSchema` is provisioning identity,
  distinct from any model-specific namespace such as a SQL table's `Schema`.
  Model catalogs persist ownership and model engines enforce it. Neither the
  ownership contract nor the exception requires a relational object shape.
- **`ProtocolVersion` lives in `Database.Protocol`, and the root consumes it.**
  The struct is wire vocabulary, so it lives with the wire implementation —
  `ProtocolVersion.Current` ("the version this assembly implements") is a plain
  static property on the struct, with the claim and the implementation that
  makes it true in one assembly. The struct spent a period in the root with
  `Current` grafted on from `Protocol` as a C# 14 static extension member — an
  arrangement that existed *only* because `Protocol` referenced the root, which
  made root → `Protocol` impossible. The child-root inversion removed that
  constraint, so the split was collapsed back into the protocol package.

## Error model

`DatabaseException` (inherits `Exception` per the area-scoped exception rule) is the root for **the
contract root and everything built *above* it**: the model engines and their Catalog/Storage components
(`SqlCatalogException`, engine-thrown `DatabaseException`), the server, and Database.Embedded.
The six shared/model client exceptions derive directly from System.Exception and locally own their
Diagnostics snapshots. Catch the appropriate client exception for remote failures; catching
DatabaseException no longer handles them. The root
defines semantic subtypes, each because the distinction is part of a public contract:
`DatabaseNotFoundException` is the exact absence signal from `IDatabaseEngine.OpenDatabaseAsync` (so
provisioning and server binding do not confuse an operational failure with a missing database),
`DatabaseParseException` distinguishes fix-the-text from fix-the-data failures (the wire's
`ParseFailure`), and the retryable-abort pair `DatabaseTransactionAbortedException` /
`DatabaseTransactionDeadlockException` (the model-boundary surface of the transaction kernel's
aborts: a write-write conflict or deadlock victim is retryable by construction, and in-process
consumers deserve to catch that kind precisely rather than parse messages; on the wire both remain
`ExecutionFailure` with a precise message).

**Child roots own independent exception roots** — `StorageException`, `DatabaseTypeException`,
`ProtocolException`, `TransactionAbortedException` all inherit
`Exception` directly. This is the point of the child-root inversion: a child root must be
independently consumable, so its error surface cannot depend on the area contracts. (`Storage` and Types were already shaped this way; Protocol and Transactions joined them
when their root references were inverted, 2026-07-13.) Execution has no exception root
since its unused shared pipeline was removed in #1257.

The consequence, deliberately accepted: `catch (DatabaseException)` does **not** catch child-root
failures. The layer that owns both vocabularies translates at its boundary — the server session pump
maps `ProtocolException` to `ProtocolViolation` in a dedicated handler and the client core wraps it
in `DatabaseClientException`. A model engine that surfaces a child-root failure (storage conflict,
transaction abort) through the session contract is responsible for wrapping it in a
`DatabaseException` at the model boundary; a child-root exception that escapes raw reaches the wire
as the `Internal` error (and closes the session), which is the pre-existing behavior for
`StorageException` — the SQL engine's statement-level failures already surface as
`DatabaseException`, so the inversion changed no live wire mapping.

## Engine failure diagnostics

Engine codes are carried in Diagnostics. Translation searches inner exceptions and every
aggregate branch, preserves specific diagnoses and gives corruption precedence over ordinary
I/O. Attached-index damage uses DatabaseCorruptionException with CK06003, CK06102 and
the model code, retaining the index cause and page. The public Create overload accepts
IndexCorruptionException; see [signatures and example](database-corruption-exception.md).
Storage-failure/corruption embedded messages are ordinary explanatory text; wire writers add a
missing prefix. SQL/Key-Value Pair/Graph aborted-transaction refusals carry CS04005, CV04101
and CG04007 in Diagnostics with ordinary unprefixed Message text and the original cause.
Inspect Diagnostics for codes. See the [model mapping](../../../../database/overview.md#engine-failure-diagnostics).

[DatabaseTransactionAbortedException](database-transaction-aborted-exception.md) accepts the same
copied, ordered diagnostic sequence as DatabaseException. Graph catalog snapshot conflicts use
it to preserve their retryable abort kind while attaching CG04007; deterministic caller errors
use DatabaseException/CG03003. The wire formats the code once.

## Lifecycle pattern

Storage integrity failures are translated to
[DatabaseCorruptionException](database-corruption-exception.md) / CK06003 at engine boundaries.
The exception names the database and optional page and retains the storage cause. Root create/open
also translate coded and aggregate wrappers after model cleanup, preserving model diagnostics.
The same integrity translation applies to read/statement and transaction operations. Wire execution
failures retain the code through each model's existing path; damaged data is never successful absence.

- **Engines** — **no lifecycle members** (the data-machine decision above). An engine
  is operational from creation — background workers pumping, databases
  creatable — and disposal (`IAsyncDisposable` + `IDisposable`, idempotent) is
  its one transition: quiesce workers → durable flush → close every open
  database. Committed work is durable when `DisposeAsync` completes. `State` is
  observational only (`Running`/`Faulted`/`Disposed`).
- **Servers** — `StartAsync`/`StopAsync` on `IDatabaseServer` — "running" lives on
  the per-model server (and the application composing servers), never on the
  engine. Stop drains gracefully within the server's drain budget; disposal
  stops the server.
- **Sessions** — disposing rolls back any active transaction (documented on the
  interface; sessions must never commit implicitly on dispose).
- **Transactions** — disposing an uncommitted transaction rolls it back.

Explicit commit and rollback observe caller cancellation before claiming the end. Cancellation
observed then leaves the transaction usable with nothing committed. A started commit ignores
caller cancellation and reports its real outcome. All five models' automatic commits take over
after execution completes, including delivery after self-committed SQL DDL cleanup. Execution
and lock waits remain cancellable beforehand. Transport loss can still lose a committed result.
See [the shared coordinator](../assimalign-cohesion-database-transactions/transaction-coordinator.md#commit-and-rollback).

## AOT posture

Contracts, enums, value objects, and canonical-document hashing only. The root does not inspect
model schemas or discover types dynamically. SQL declaration compilation and source-generated JSON
serialization live in `Database.Sql.Schema`.

## Non-goals

- **No connection/network concepts (that** — is the per-model server machinery in
  the model packages — `SqlDatabaseServer` in `Database.Sql` — and
  `Database.Client`).
- **No DI, configuration, or** — `Assimalign.Cohesion.Hosting*` reference. Background-work
  registration and hosting implementation remain in `Database.Hosting` (O34).
- **No model-specific request or result types** — models subclass the
  `Database.Execution` family in their own packages.

## Declared dependencies

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Core` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Execution` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Indexing` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Language` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Protocol` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Security` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Storage` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Transactions` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Types` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Web` | `CohesionPrivateProjectReference` |

[Assembly overview](index.md) · [Examples](examples/index.md)

## Deferred checkpoint lanes

After collecting a deferred checkpoint lane, the shared worker rechecks whether work is due in
that pass and attempts at most one new checkpoint. Failures retain their retry delay; busy storage
waits for the next pass. Unfinished work is reported once per database per pass. Size and interval
remain soft triggers, so this does not impose a journal bound or close #1283.

ForeignStorage refusals are operational failures and reach normal failure reporting immediately.
Only ordinary storage-bracket contention is retried as busy. A statement that runs a deferred
checkpoint retains a ForeignStorage failure for the next worker checkpoint call, preserving the
statement's already-decided outcome.

## Maintenance lane isolation

Every implemented model runs checkpoint, page write-back, WAL group flush and version purge on
worker-owned dedicated lanes, with at most one operation per database and worker kind. SQL and
Key-Value Pair's index-maintenance workers use the same dispatcher, but their production bodies
remain empty and dispatch no idle work. Work admission starts after the last database admitted,
serving due databases round-robin. SQL/Key-Value provisional file sets also flush on lanes; publication
preserves the data file set's lane identity and waits for an outstanding provisional catalog flush.

The cap is **64 non-stuck operations per engine and worker kind**, with a hard ceiling of
**128 lane threads**, including parked and stuck runners. After four worker intervals, clamped to
1–4 seconds, a stuck operation releases its admission slot while retaining its lane and storage
ownership until I/O returns. This permits replacement work within the thread ceiling. At that
ceiling, progress requires a runner to return. These are dedicated OS threads, separate from the
worker pumps and application thread pool.

After initialization, idle due checks dispatch no lane and allocate nothing when pass observation is disabled. They
consult pending commits, a constant-time dirty-page count, deferred undo and owed purge passes.
Parked runners retire after 30 seconds. A detached write-back completion wakes collection while
healthy peers keep their regular interval; work deferred by capacity remains owed. Collected WAL
flushes recheck late commit registrations before sleeping. Purge retry/refusal state stays on the
pump, and a full pass becoming due while an earlier lane runs remains owed after collection.

Capacity deferral and stuck lanes report observational TimeoutException failures and warning
event 35, at most once per second per database/worker. Worker Fault and engine Faulted expose the
delay, while HasFailingWorker(database) remains false for the observation alone. A lane delay
neither takes the database offline nor replaces an unresolved operational fault or its retry state.
Completion clears the observation when no unfinished work remains.

Engine disposal stops pumps and joins every lane before closing storage. It has no timeout that
abandons in-flight I/O. Warning event 36 repeats once per second for each database/worker still
running, with elapsed disposal wait time. Investigate the named database's storage/device latency;
an unresponsive device can keep disposal waiting indefinitely. Size and interval checkpoint triggers
remain soft, and this isolation does not impose a hard journal bound.

## Worker failure reporting

SQL, Key-Value Pair, Document, Graph and Blob report persistent version-purge refusals once per
blocker episode after at least three actual refused attempts spanning FailureBackoff (one second)
on the engine clock. Not-due calls do not count. The refusal message identifies the work; ordinary
page-lock messages name the page and owning bracket. Changing the blocker or completing the
refused work resets the episode; unrelated writes and WAL progress do not. Close, drop and
replacement end the old database instance's episode. A busy full pass retries after FailureBackoff.

These reports set worker Fault and engine State to Faulted while the work remains unfinished,
but are observational: HasFailingWorker(database) stays false for an observation alone, clients
and new handshakes remain served, and neither the offline timer nor its minimum-pass count advances.
Completion clears the observation; the engine returns to Running when no other fault remains.
ForeignStorage failures instead use immediate operational reporting and the existing offline
policy. An observation-only completed pass ends an earlier operational streak; later operational
failures start a new count and window. Whole-pass, give-up and operational database faults take
precedence over observations in Fault publication, including cancellation before pass settlement.

### Protected reporting API

```csharp
protected int ReportFailure(string database, Exception exception);
protected int ReportFailure(string database, Exception exception, TimeSpan retryAfter,
    bool countsTowardOffline = true);
protected void ReportLaneDelay(string database, string reason, TimeSpan elapsed);
protected void ReportLaneDisposalWait(string database, TimeSpan elapsed);
```

The two-argument form uses FailureBackoff and counts toward offline. The optional flag on the
other overload lets a derived worker publish a failure without refusing service. The returned
episode count includes observation-only failed passes; the offline minimum counts only operational
passes, once per database per pass. Existing three-argument source calls retain their policy;
recompile derived-worker binaries against the changed protected signature.

Both forms throw ArgumentNullException for a null database or exception and InvalidOperationException
outside a running pass. The explicit-delay form throws ArgumentOutOfRangeException for a negative
retryAfter. Zero lets a worker pace its own retries. Model purge workers apply their blocker/time
threshold before invoking this API; the base does not infer contention from exception text.

ReportLaneDelay is called during a running pass for `capacity` or `stuck` obstruction. It publishes
an observation with zero retry delay unless an operational fault already exists, in which case it
keeps that fault unfinished. It also emits event 35 with the worker identity and elapsed operation
time. Its reporting path retains ReportFailure's running-pass requirement and null-database guard.
ReportLaneDisposalWait emits event 36 during lane release, with elapsed disposal time. Derived
workers must retain storage ownership while reporting a wait; neither helper cancels I/O.

The equivalent calls inside a lane-owning derived worker are:

```csharp
// During the pass, after admission was refused:
ReportLaneDelay("db", "capacity", TimeSpan.Zero);
// During release, while joining a running operation:
ReportLaneDisposalWait("db", elapsed);
```

MaintenanceCapacityTests verifies the built-in paths: engine Faulted without an offline database,
and event 36 repeating while disposal is incomplete and the database's storage remains open.

This fragment adapts ScriptedWorker's report-only path inside a derived worker:

```csharp
protected override void RunIterationCore(CancellationToken cancellationToken)
{
    cancellationToken.ThrowIfCancellationRequested();
    if (BeginDatabase("db"))
    {
        ReportFailure("db", new InvalidOperationException("Observed refusal."),
            TimeSpan.Zero, countsTowardOffline: false);
    }
}
```

After this pass, the attached worker has a Fault and the engine is Faulted, while an observation
alone leaves HasFailingWorker("db") false. A later pass that visits and completes the work clears it.

### Events

Provider `Assimalign.Cohesion.Database` retains the existing events; these changes add no code
or SQLSTATE.

| Id | Event | Level | Payload |
| --- | --- | --- | --- |
| 1 | WorkerFailed | Warning | workerName, workerKind, database, exceptionType, exceptionMessage, consecutiveFailures |
| 2 | WorkerRecovered | Informational | workerName, workerKind, database, failures |
| 35 | WorkerLaneDelayed | Warning | workerName, workerKind, database, reason, elapsedSeconds |
| 36 | WorkerLaneDisposalWaiting | Warning | workerName, workerKind, database, elapsedSeconds |

WorkerFailed uses the same per-database record for observational refusals. Its consecutiveFailures
is the episode's failed-pass count, including report-only passes; it is not the offline minimum's
operational count. Lane isolation lets healthy databases continue within the admission/thread caps.
WorkerLaneDelayed uses reason `capacity` (zero elapsed time) or `stuck` (operation runtime);
WorkerLaneDisposalWaiting measures disposal wait time. Both use the root provider for all five models
and add no diagnostic code or SQLSTATE. Opt-in internal pass observations cover all lane-worker kinds;
checkpoint due-state evidence remains checkpoint-specific.

## Structured failure-event codes

StatementFailed (event 28 of Assimalign.Cohesion.Database) selects a thrown DatabaseException's code from its non-null Diagnostics: the first Error diagnostic, otherwise the first entry, otherwise an empty code. Returned failures use the same selection. Other exception types contribute an empty code. The event records the exception type and code, never the exception/diagnostic message or statement text. Offline refusals and unconfirmed commits also carry their model identity in Diagnostics with unprefixed exception detail and the original cause. Event ids, payload layout and counters are unchanged.

Verified by DatabaseEventSourceTests.StatementFailed_ThrownDivisionByZero_ShouldReportDiagnosticCode and StatementFailed_ThrownDiagnostics_ShouldPreferFirstError in the source branch.

## Construction and debugger inspection

LockManager and StoragePageLatch now have public parameterless constructors; their static Create factories are removed. QueryAnalyzerContext and StorageStream are sealed; the Cache scaffold Class1 is also sealed and supplies no cache engine.

Database debugger displays and private nested proxies filter internal coordination state. They project reviewed safe public/protected values and stored references without running queries, enumerating results, resolving services, acquiring locks, reading storage/streams, invoking callbacks or creating lazy data. Types with no safe projection show an empty view and type name; effective inherited views are reused. Ref-like data cannot escape through a heap view. Debugger attributes/proxies add no public runtime API, instance fields to existing types, wire/disk format or normal execution work.

Messages and format templates use generated assembly-local resources. Diagnostic identities remain separate from unprefixed structured messages; framework argument guards retain their .NET types and parameter names. See [the code scheme](../../../../database/error-codes.md).

## Sources

- **Admission, fairness, idle cost and disposal** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/MaintenanceCapacityTests.cs`.
- **Lane ownership** — `cohesion/resources/Database/Assimalign.Cohesion.Database/shared/DatabaseWorkerLanes.cs`, `DatabaseLaneWorker.cs` and `DatabaseMaintenanceWorker.cs` in the same directory.
- **Events 35/36** — `cohesion/resources/Database/Assimalign.Cohesion.Database/src/Internal/EventSource/DatabaseEventSource.cs`.
- **Checkpoint isolation evidence** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/CheckpointPassObservationTests.cs` and `cohesion/resources/Database/Assimalign.Cohesion.Database/tests/TestObjects/CheckpointPace.cs`.
- **Reporting contract and fault precedence** — `cohesion/resources/Database/Assimalign.Cohesion.Database/src/DatabaseEngineWorker.cs` and `cohesion/resources/Database/Assimalign.Cohesion.Database/tests/DatabaseWorkerFailureWindowTests.cs`.
- **Derived-worker example** — `cohesion/resources/Database/Assimalign.Cohesion.Database/tests/TestObjects/ScriptedWorker.cs`.
- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database/docs/DESIGN.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database/src/Assimalign.Cohesion.Database.csproj`.
