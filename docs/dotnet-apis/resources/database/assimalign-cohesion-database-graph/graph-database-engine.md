# GraphDatabaseEngine

The `GraphDatabaseEngine` type is part of the documented `Assimalign.Cohesion.Database.Graph` API.

> **Status:** Partial.

Manages logical graph databases and their four maintenance workers.

Namespace: `Assimalign.Cohesion.Database.Graph`.

## Documented behavior

- **`GraphDatabaseEngine.Create(name, options)`** — creates an operational engine deriving from
  `DatabaseEngine`; `Workers` exposes the four maintenance duties. `Dispose` the engine to flush
  and release all databases.
- **`GraphDatabaseEngine.CreateBuilder(name)`** — exposes dependency-free options and nested component
  factories for standalone construction or a hosting-aware engine factory.
- **`GraphDatabaseEngine.Servers`** — exposes nested servers whose lifetime the engine owns.
  `Create`/open/drop/lookup now accept `DatabaseName`, including its implicit string conversion.

## Database lifecycle

```csharp
public new ValueTask<GraphDatabase> CreateDatabaseAsync(DatabaseName name, CancellationToken cancellationToken = default);
public new ValueTask<GraphDatabase> OpenDatabaseAsync(DatabaseName name, CancellationToken cancellationToken = default);
public ValueTask DropDatabaseAsync(DatabaseName name, CancellationToken cancellationToken = default);
```

Missing journals/backups and unrecoverable files are refused with CG06018 in
DatabaseException.Diagnostics; no missing sidecar is recreated. A directory without its data file
keeps DatabaseNotFoundException and the code. Known incomplete creation directs drop/recreate;
ambiguous files must be preserved. Drop deletes graph.dat first. A torn uncommitted tail may recover;
transient I/O asks for an open retry without restore advice. No SQLSTATE is assigned.

Disposed engines throw ObjectDisposedException and cancellation throws OperationCanceledException.
See [lifecycle examples and remedies](design.md#database-creation-open-and-drop).

## Format refusal

OpenDatabaseAsync and enumeration through GetDatabasesAsync can throw DatabaseException for an
unsupported graph record or tree-registration format. Format 4 is required; older formats produce
CG06017 in Diagnostics before logical recovery scrub. The public exception is
[DatabaseCorruptionException](../assimalign-cohesion-database/database-corruption-exception.md)
with CK06003, CG06018 and CG06017; its inner StorageCorruptionException retains the
coded message. Recreate the database; no upgrade path or SQLSTATE is assigned.
GraphEngineTests checks that a cleanly closed refused file set stays unchanged.

## Related reference

- **Assembly** — [`Assimalign.Cohesion.Database.Graph`](index.md).
- **Design** — [Lifetime and implementation decisions](design.md).
- **Examples** — [Source-backed usage](examples/index.md).

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/docs/Assembly/Assimalign.Cohesion.Database.Graph/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/GraphDatabaseEngine.cs`.
