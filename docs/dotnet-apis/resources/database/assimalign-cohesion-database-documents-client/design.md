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

QueryAsync returns DocumentResultSet. Lone SELECT *, VALUE and RAW set IsDocumentQuery even for empty results and
retains owned JsonElement values in Documents and the single row cell. Projections use one JSON
Document object per row and header ordinals. MISSING members are omitted, explicit NULL retained.
DocumentRow.SupportsMissing is true; IsMissing separates absence from NULL, while GetValue and
indexers return null for either. Projected numbers are exact Decimal or Int64 according to column
metadata, objects/arrays JsonElement.
Duplicate header names or unknown/duplicate projected members are malformed responses.

VALUE/RAW uses one `value` column. The engine converts top-level MISSING to NULL before DISTINCT,
projected ordering and result counting. InMemory/TCP and embedded byte results agree. GetBytes(0)
for bare rows returns JSON, including quoted strings and `null`; lone star preserves raw stored
bytes. Named strings use unquoted UTF-8 and named NULL/MISSING uses empty memory. SELECT constructors
execute to 256 levels (`COHDOC015`); DISTINCT ordering outside the projection uses `COHDOC014`.

ExecuteAsync returns affected count: inserted/deleted document count, zero for DDL, -1 for reads. Collection
operations return names; server-side session-bound collection handles do not cross the wire.
Get returns null for absence, Put returns the new version, Delete returns whether a document was
removed. Expected-version checks retain engine semantics. OQL INSERT is insert-only.

[UPSERT](../../../../database/document/language/statements/upsert.md) uses the same Execute exchange,
returning written-pair count without Document frames. Existing keys are replaced; repeated input
keys fail COHDOC012, retaining diagnostics and a reusable connection. Generated INSERT/UPSERT,
constructor and VALUE/RAW bytes use the engine's one RFC 8259 encoder: literal UTF-8 Unicode with
only quotes, backslashes and control characters escaped. Wire framing is unchanged.

## Failures and connection reuse

OQL DELETE reports deleted count through Complete, with no Document frames. QueryAsync returns
empty rows/Columns/Documents with AffectedCount set; ExecuteAsync returns the count. Predicate
validation and coded failures retain the existing Failure/reuse contract, including a read-only
system-target refusal followed by a successful zero-match DELETE on the same rental.

Model Failure (72) terminates a rejected statement without Complete, including after partial rows.
The client discards accumulated results and throws DocumentClientException, preserving the protocol
category and DatabaseException.Diagnostics. A fully decoded Failure certifies a reusable session.
Shared Error, invalid order/counts/UTF-8/JSON, truncation, transport loss and unfinished cancellation
discard it. Mutations canceled after publication can have unknown outcomes.

OQL UPDATE also uses Complete without Document frames; AffectedCount is the matched count,
including unchanged matches. Every match receives a new version. QueryAsync returns empty Columns,
Documents and rows. SET/UNSET path failures (COHDOC016), target overlaps (COHDOC017) and invalid/deep
patched storage JSON (COHDOC018) have no assigned SQLSTATE and use the same reusable Failure path.
See [UPDATE](../../../../database/document/language/statements/update.md).

Server failure text is bounded to 4096 UTF-16 characters and diagnostics to 256, with explicit
truncation/omission markers. Retained codes/spans stay intact. See the complete
[OQL diagnostic table](../../../../database/document/language/diagnostics.md) for COHDOC001–023 and
parser codes; all these statement refusals use the same structured Failure carrier.

The [34 scalar functions](../../../../database/document/language/functions/scalar-functions.md)
share embedded execution semantics. COUNT, LENGTH, POSITION, ARRAY_LENGTH and epoch-millisecond
functions decode Int64 columns to long, including metadata on empty grouped results. Mixed numeric
columns promote to Decimal; heterogeneous columns normalize numeric cells to Decimal. Domain errors
such as SQRT(-1) retain COHDOC019 (no SQLSTATE) through Failure and leave the connection reusable.

Dispose returns a healthy rental once. Abort discards it. Operation registration shares the
wrapper's teardown lock, so repeated disposal cannot return a later rental of the same connection.
Client disposal closes idle sessions and outstanding sessions when they return.

## Protocol and scope

Shared protocol 2.1 handles startup; the 2.0 major changed Documents projection encoding. Documents
negotiates no core Diagnostics capability and retains its own Failure frame. OQL/CRUD/collections
execute over InMemory and TCP. Explicit wire transactions, remote database lifecycle, replication
and model-specific security policies remain unsupported.

[Assembly](index.md) · [Examples](examples/index.md) · [DocumentProtocol](../assimalign-cohesion-database-documents/document-protocol.md)

## Result paging

OQL [LIMIT/OFFSET](../../../../database/document/language/clauses/limit-offset.md) uses ordinary JSON
parameters. Complete counts returned Document frames and query AffectedCount stays -1. Empty pages
retain columns, IsDocumentQuery and static function types. Invalid counts preserve OQL0007 and
the operand span over InMemory and TCP; the authenticated session remains reusable.

## Array query results

[UNNEST and LEFT UNNEST](../../../../database/document/language/clauses/unnest.md) use the existing
query exchange. With expansion, star returns synthetic whole-document objects keyed by the FROM
alias (or collection name) and array variables. MISSING LEFT bindings are omitted; actual NULL
elements remain null. Without expansion, star retains the stored bytes. Grouped COUNT over
expanded rows retains Int64 metadata and values.

ANY/SOME/EVERY and array IN use the same result encoding. Duplicate parsed binding names retain
OQL0010; direct AST duplicate/empty bindings use COHDOC023. Neither code has an assigned SQLSTATE.
These failures use the existing structured Failure carrier and leave the connection reusable.

## Rental isolation

DocumentConnection wraps a new shared lease for each rental; the pool retains only its internal connector.
Database is captured before return, allowing operation diagnostics to finish after the core lease
is disposed. Ended leases permanently reject execution and report IsOpen false; cleanup cannot
reach another renter. See [DatabaseConnection](../assimalign-cohesion-database-client/database-connection.md)
for disposal, shutdown and the explicit-transaction reset gap (#1540).

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Client/docs/DESIGN.md`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Client/tests/DocumentClientWireTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Client/tests/DocumentClientDiagnosticTests.cs`.
