# Document

The Documents engine stores versioned JSON documents in named collections within a logical database.

> **Status:** Partial. Local document operations, OQL queries, index definitions, and durable storage are implemented; the document client and protocol server are not implemented.

## Collections and sessions

`DocumentDatabaseEngine.Create` creates an engine. `IDocumentDatabase` provides collection creation
and lookup, and each session binds to one database. Collection names, document identifiers, and JSON
property names are case-sensitive. Collections obtained through `session.Database` participate in
that session's transaction and remain bound to its lifetime.

Documents contain UTF-8 encoded JavaScript Object Notation (JSON). Object, array, and scalar roots
are accepted. Collections may contain documents with different shapes; the engine does not infer
a fixed schema. Missing fields and incompatible path traversal evaluate to null in queries.

## Reading and writing

- **Query and index language** — [Object Query Language (OQL)](language/index.md) provides `SELECT`,
  `CREATE INDEX`, and `DROP INDEX` through the session's parse, plan, and execute pipeline.
- **Document replacement** — `IDocumentCollection.PutAsync` replaces the complete JSON value by
  identifier and returns a new version. An optional expected version must match a visible document.
- **Document deletion** — `IDocumentCollection.DeleteAsync` removes the identified document using
  the supplied session. OQL does not provide document `INSERT`, `UPDATE`, or `DELETE` statements.
- **Collection changes** — direct database operations use automatic transactions; operations through
  `session.Database` use its active transaction.

Automatic operations commit on success and roll back on failure. A failing operation in an explicit
transaction rolls back that transaction. Snapshot and read-committed isolation are supported;
serializable isolation is rejected. Writers are serialized per logical database, and conflicting
changes raise `DatabaseTransactionAbortedException`.

## Storage and indexes

The storage package validates JSON and persists stamped chunk chains. The catalog maintains
versioned identities and nonunique scalar-path indexes in shared B+Trees. Index creation populates
existing documents immediately, while replacement and deletion maintain indexes in the same
transaction as document content.

File-backed databases use `document.dat`, `document.log`, and `document.bak`. Memory-backed databases
use the same storage and transaction implementation over memory streams. Queries and JSON values
are materialized in managed memory; chunked persistence does not imply bounded query memory.
Document content is limited to `Int32.MaxValue` bytes and JSON nesting to 128 levels.

String predicates, ordering, and grouping use ordinal, case-sensitive comparison. SQL collations
do not change document behavior. Schema-owned collections reject collection and index definition
changes, while their document contents remain mutable.

## Reference

- **Language** — [Language (OQL)](language/index.md).
- **Database context** — [Database](../index.md) and [Overview](../overview.md).
- **Related language** — [Language (SQL)](../sql/language/index.md).
- **API reference** — [Database resource packages](../../dotnet-apis/resources/database/index.md).

## Sources

- **Engine** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/docs/OVERVIEW.md`.
- **Semantics and limits** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/docs/DESIGN.md`.
- **Catalog** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Catalog/docs/OVERVIEW.md`.
- **Storage** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Storage/docs/OVERVIEW.md`.
