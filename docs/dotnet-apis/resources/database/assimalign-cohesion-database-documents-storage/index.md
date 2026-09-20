# Assimalign.Cohesion.Database.Documents.Storage

`Assimalign.Cohesion.Database.Documents.Storage` stores immutable UTF-8 JSON content as chunk chains over `Database.Storage`. `Database.Transactions` supplies the shared logical coordinator, version stamps, undo ledger, and recovery classification.

> **Status:** Partial.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.

`Assimalign.Cohesion.Database.Documents.Storage` stores immutable UTF-8 JSON content as chunk chains
over `Database.Storage`. `Database.Transactions` supplies the shared logical coordinator, version
stamps, undo ledger, and recovery classification. The package has no dependency on the document
engine, Hosting, ApplicationModel, or a serializer that discovers runtime types.

`Create` a file set with `DocumentStorage.Create`; construct a `TransactionCoordinator` using
`WriteAheadJournal` and `Records`. `WriteContentAsync` validates a document, writes its chunks in
the supplied transaction, and returns a `DocumentContentReference`. Publish that reference through
Documents.Catalog before committing. `ReadContent` validates integrity and returns the original
bytes. JSON objects, arrays, and scalar roots are supported, including mixed document shapes.

The streaming chunk helpers follow the Blob storage precedent. They transport bytes and require
their caller to validate document syntax before publication; the engine uses the validated memory
helper. Documents are currently materialized for query evaluation and limited to `Int32.MaxValue`
content bytes. This is a format/implementation boundary rather than an eager allocation promise.

The original `InsertDocument`, `ReadDocument`, `UpdateDocument`, and `DeleteDocument` methods are
retained low-level, unstamped record helpers for existing consumers. They are not the engine's
document write path and must not be mixed into engine-owned file sets. See [DESIGN.md](design.md)
for the exact format, compatibility policy, and recovery order.

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Database.Storage` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Transactions` | `CohesionProjectReference` |

[Parent: Database](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Storage/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Storage/src/Assimalign.Cohesion.Database.Documents.Storage.csproj`.
