# Assimalign.Cohesion.Database.Documents.Client design

The client layers OQL and document exchanges over Database.Client's authenticated connection pool.

> **Status:** Implemented for the documented operations.

## Ownership and dependencies

Documents.Client owns sealed client, rental, result and exception types. Database.Client owns
dialing, authentication, pool limits, leases and connection reuse. Documents owns model codecs;
Protocol owns family-bound framing. The caller supplies an IDuplexConnectionFactory. No runtime
reflection or POCO serializer is used.

Each rental runs one exchange at a time. Requests capture parameters/content before the first await.
Engine queries and client results materialize. The server formatter retains one encoded frame;
TCP backpressure bounds queued output, while InMemory output can queue more bytes if its reader
stops. One value must fit the shared 16 MiB frame limit.

## Results and MISSING

QueryAsync returns DocumentResultSet. SELECT * sets IsDocumentQuery even for empty results and
retains owned JsonElement values in Documents and the single row cell. Projections use one JSON
Document object per row and header ordinals. MISSING members are omitted, explicit NULL retained.
DocumentRow.SupportsMissing is true; IsMissing separates absence from NULL, while GetValue and
indexers return null for either. Projected numbers are exact decimal, objects/arrays JsonElement.
Duplicate header names or unknown/duplicate projected members are malformed responses.

ExecuteAsync returns affected count: inserted document count, zero for DDL, -1 for reads. Collection
operations return names; server-side session-bound collection handles do not cross the wire.
Get returns null for absence, Put returns the new version, Delete returns whether a document was
removed. Expected-version checks retain engine semantics. OQL INSERT is insert-only.

## Failures and connection reuse

Model Failure (72) terminates a rejected statement without Complete, including after partial rows.
The client discards accumulated results and throws DocumentClientException, preserving the protocol
category and DatabaseException.Diagnostics. A fully decoded Failure certifies a reusable session.
Shared Error, invalid order/counts/UTF-8/JSON, truncation, transport loss and unfinished cancellation
discard it. Mutations canceled after publication can have unknown outcomes.

Server failure text is bounded to 4096 UTF-16 characters and diagnostics to 256, with explicit
truncation/omission markers. Retained codes/spans stay intact. See the complete
[OQL diagnostic table](../../../../database/document/language/diagnostics.md) for COHDOC001–013 and
parser codes; all these statement refusals use the same structured Failure carrier.

Dispose returns a healthy rental once. Abort discards it. Operation registration shares the
wrapper's teardown lock, so repeated disposal cannot return a later rental of the same connection.
Client disposal closes idle sessions and outstanding sessions when they return.

## Protocol and scope

Shared protocol 2.1 handles startup; the 2.0 major changed Documents projection encoding. Documents
negotiates no core Diagnostics capability and retains its own Failure frame. OQL/CRUD/collections
execute over InMemory and TCP. Explicit wire transactions, remote database lifecycle, replication
and model-specific security policies remain unsupported.

[Assembly](index.md) · [Examples](examples/index.md) · [DocumentProtocol](../assimalign-cohesion-database-documents/document-protocol.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Client/docs/DESIGN.md`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Client/tests/DocumentClientWireTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Client/tests/DocumentClientDiagnosticTests.cs`.
