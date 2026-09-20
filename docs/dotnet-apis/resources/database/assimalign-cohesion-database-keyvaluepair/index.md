# Assimalign.Cohesion.Database.KeyValuePair

The key-value model engine: `KeyValueDatabaseEngine`, an ordered key space with point operations (get/put/delete/exists), ordered range scans, and per-entry etags for conditional writes — the second model engine on the shared database kernel, and deliberately the kernel-generality proof (area DESIGN §3.10).

> **Status:** Partial.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.

The key-value model engine: `KeyValueDatabaseEngine`, an ordered key space with point operations
(get/put/delete/exists), ordered range scans, and per-entry etags for conditional writes — the
second model engine on the shared database kernel, and deliberately the kernel-generality proof
(area DESIGN §3.10).

## Purpose

Implements the area root's `IDatabaseEngine` /`IDatabase`/`IDatabaseSession` contracts for the
key-value model by composing the shared kernel — storage (pages/WAL/recovery), transactions (MVCC
snapshots, lock manager), indexing (B+Tree) — never re-implementing it. Keys and values are opaque
byte sequences; keys order by unsigned lexicographic byte comparison.

## Scope

- **`KeyValueDatabaseEngine` + `KeyValueDatabaseEngineOptions`** — the data machine:
  create → use → dispose, engine-owned background workers, two file sets per
  database (`<name>` + `<name>.catalog`).
- **`IKeyValueDatabase`** — the typed model surface (get/put/delete/exists/scan with
  etag-conditional writes).
- **Sessions address exactly one** — database (A5). All five typed operations reject
  sessions from another database; text and typed requests execute only against
  the receiving session's database. `Database`/server administration stays on the
  host-owned engine and is absent from the command grammar.
- **The typed request family** — (`KeyValueGetRequest`, `KeyValuePutRequest`,
  `KeyValueDeleteRequest`, `KeyValueExistsRequest`, `KeyValueScanRequest`) —
  the model's members of the shared `Database.Execution` request family.
- **The text command grammar (`GET`/`PUT`/`DELETE`/`EXISTS`/`SCAN`)** — the contract
  in COMMANDS.md (`cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/docs/COMMANDS.md`), parsed by the session's text-execute seam so the
  wire protocol's Execute message serves the model with zero protocol changes.
- **`KeyValueDatabaseServer` (+ `KeyValueDatabaseServerOptions`)** — the model's
  wire-protocol server, carrying its own full copy of the server machinery
  (servers are per-model and each model package owns its copy — owner decision
  2026-07-14; see DESIGN.md for the placement history).

## Dependencies

- **`Assimalign.Cohesion.Database`** — the area root (contracts + child-root rollup).
- **`Assimalign.Cohesion.Database.KeyValuePair.Storage`** — the model's storage
  binding (`KeyValueStorage`).
- **`Assimalign.Cohesion.Database.KeyValuePair.Catalog`** — index registrations +
  entry-space format marker.
- **`Assimalign.Cohesion.Database.Storage` / `.Types`** — durability options surface
  and the shared tuple codec (direct references; the rest of the kernel arrives
  through the root).
- **`Assimalign.Cohesion.Connections`** — the transport listeners the model's
  wire-protocol server binds and accepts from (the server owns listener
  lifecycle once start is attempted).

## Usage

See the [source-backed usage examples](examples/index.md).

See [DESIGN.md](design.md) for the architecture and the decisions behind it.

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Database` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Connections` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.KeyValuePair.Storage` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.KeyValuePair.Catalog` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Storage` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Types` | `CohesionProjectReference` |

[Parent: Database](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/src/Assimalign.Cohesion.Database.KeyValuePair.csproj`.
