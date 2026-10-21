# Diagnostics

OQL reports coded parser, planner and execution failures with available source locations.

## Parser codes

| Code | Meaning |
| --- | --- |
| `COHDBL001` | Recognized form outside the executable SQL++ subset |
| `OQL0001` | Empty/comment-only statement |
| `OQL0002` | Malformed syntax, identifier, path, parameter, literal LIKE escape or multiple statements |
| `OQL0003` | Unterminated string, quoted identifier or block comment |
| `OQL0004` | Malformed numeric literal syntax |
| `OQL0005` | More than 128 recursive expression parse levels |
| `OQL0006` | Invalid function arity or star operand; only COUNT accepts star |
| `OQL0007` | Invalid LIMIT/OFFSET count at its operand, during parsing or planning; NULL is invalid |
| `OQL0008` | Unknown function name; span covers its name after arguments are parsed |
| `OQL0010` | Duplicate name in one FROM/UNNEST or quantifier binding list; span covers the repeated name or inferred final path step |
| `OQL0009` | Index exceeds 32 key paths; located at the 33rd path |

Numeric tokens retain their digits in a JsonElement. Valid syntax that is out of decimal range,
underflows, or loses nonzero fractional digits fails during planning/evaluation as COHDOC007,
rather than being rounded by the parser.

## Engine codes

| Code | Meaning and trigger | SQLSTATE where supplied |
| --- | --- | --- |
| `COHDOC001` | Invalid statement, index definition, expression, metadata or grouping context | — |
| `COHDOC002` | Collection does not exist | — |
| `COHDOC003` | Duplicate projected name; supply distinct aliases | — |
| `COHDOC004` | System collection is read-only | — |
| `COHDOC005` | Referenced parameter has no value | — |
| `COHDOC006` | Division or remainder by zero | 22012 |
| `COHDOC007` | Numeric value cannot fit decimal exactly, scalar overflow, or string result/work buffer exceeds 1,048,576 UTF-16 units | 22003 |
| `COHDOC008` | Invalid operand/parameter type, document key or JSON value | — |
| `COHDOC009` | Caller keys collide after one leading parameter sigil is stripped | — |
| `COHDOC010` | CREATE COLLECTION finds an existing collection without IF NOT EXISTS | 42P07 |
| `COHDOC011` | DROP COLLECTION targets a schema-owned collection, including IF EXISTS | — |
| `COHDOC012` | INSERT finds an existing key, or INSERT/UPSERT repeats a key in one statement | — |
| `COHDOC013` | Evaluated LIKE escape is invalid or pattern ends in its escape character | — |
| `COHDOC014` | SELECT DISTINCT ORDER BY key is absent from the projection after alias rewriting; reported at that key | — |
| `COHDOC015` | SELECT object/array constructor result exceeds 256 levels, including nested values and a named projection wrapper; reported at the constructor | — |
| `COHDOC016` | Invalid UPDATE target, root replacement, UNSET ending in an index, incompatible traversal, missing array or out-of-range index | — |
| `COHDOC017` | Duplicate or prefix-overlapping UPDATE targets after alias normalization, including SET/UNSET overlap | — |
| `COHDOC018` | Patched UPDATE JSON is invalid or exceeds storage depth 128 | — |
| `COHDOC019` | Scalar argument outside its domain: negative SQRT, invalid POWER domain, or fractional ROUND/TRUNC scale | — |
| `COHDOC020` | Duplicate valued scalar UNIQUE key during a write/build | 23505 |
| `COHDOC021` | Array/object UNIQUE key component | — |
| `COHDOC023` | Direct SELECT AST repeats a FROM/UNNEST binding name or supplies an empty UNNEST name | — |
| `COHDOC022` | Index path count outside 1..32 in a direct AST/catalog call | — |
| `COHDOC024` | CREATE INDEX finds an existing name without IF NOT EXISTS | — |
| `COHDOC025` | DROP INDEX finds no index without IF EXISTS | — |
| `COHDOC026` | Encoded composite index key exceeds 1,024 bytes during build or mutation | — |

[Scalar functions](functions/scalar-functions.md) validate literal kinds and contexts with
COHDOC001 before reading documents. A wrong-kind stored argument produces NULL. Fractional
REPLACE counts use COHDOC008; COHDOC019 has no assigned SQLSTATE. Evaluation failures abort an
explicit transaction.

For example, repeating `INSERT INTO items (KEY, VALUE) VALUES ('a', 1)` on the same visible
collection reports COHDOC012. A malformed static escape such as ESCAPE 'ab' is OQL0002 instead
of COHDOC013. Ordinary NULL and MISSING results are values, not diagnostics.

COHDOC016–018 have no assigned SQLSTATE. [UPDATE](statements/update.md) reports target locations
where available and validates all patched documents before the first write.

INSERT/UPSERT repeated-key failures use `Document key 'a' appears more than once in one statement.`
(COHDOC012, no SQLSTATE). Other shared key/value, binding and JSON validation messages name the
written verb, including UPSERT. See [UPSERT diagnostics](statements/upsert.md#diagnostics).

Paging count errors retain OQL0007 and the operand span over the wire; unbound parameters retain
COHDOC005. Neither code has an assigned SQLSTATE. [LIMIT/OFFSET](clauses/limit-offset.md)
distinguishes syntax errors (OQL0002) and the unsupported comma spelling (COHDBL001).

[CREATE INDEX](statements/create-index.md) uses COHDOC020 for duplicate valued scalar keys during
writes/builds, COHDOC021 for array/object UNIQUE components, and COHDOC022 for direct AST/catalog
path counts outside 1..32. COHDOC021/022 and OQL0009 have no assigned SQLSTATE. Constraints are
immediate per row; a failed explicit transaction requires RollbackAsync. These constraint failures
are distinct from DocumentDuplicateKeyException/COHDOC012 (document identity collisions).

[Array bindings](clauses/unnest.md) reserve the unaliased collection name for duplicate checks.
OQL0010 and COHDOC023 have no assigned SQLSTATE. META on an UNNEST/quantified variable, including
one shadowing the source alias, uses COHDOC001. Quantifiers may shadow outer bindings.

Index-name/key-size failures use DatabaseException with structured COHDOC024–026 diagnostics
and the original DocumentCatalogException. They abort an explicit transaction; roll back before
continuing. In autocommit the next statement can run. No SQLSTATE is assigned.

## Locations and wire behavior

Embedded command and query results expose a non-null DatabaseResult.Diagnostics collection,
empty when nothing is reported. DatabaseException.Diagnostics and DocumentClientException.Diagnostics
are also non-null, possibly empty. Remote DocumentResultSet has no Diagnostics member; remote
failure details continue to arrive through the model's Failure message and client exception.

Remote failures use [DocumentClientException](../../../dotnet-apis/resources/database/assimalign-cohesion-database-documents-client/document-client-exception.md), which derives directly from
System.Exception and declares its own Diagnostics snapshot. Catch that client exception for remote
operations; catching DatabaseException now covers only engine-side failures.

DatabaseException.Diagnostics is never null and orders located entries by source start before
unlocated entries. Offsets are zero-based UTF-16 with exclusive ends; lines are one-based.
Planner/binder failures retain available AST spans. Query requests reject all parser errors
before a recovered AST reaches planning. In-process messages are code-free where the carrier
supplies a separate diagnostic; the server composes CODE: message from the first Error entry.

The Documents model sends terminal Failure (72), carrying the wire error category and coded
diagnostics with spans. This remains its structured diagnostic path on protocol 2.1; it does not
negotiate the core Diagnostics frame. Statement refusal preserves the connection; malformed
UTF-8/payloads and message order are protocol violations. The client exception retains diagnostics.

## Database files

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| `COHDBD004` | Storage operation or open I/O failure, including I/O inside catalog/index-registration wrappers; original cause retained | — |
| `COHDBD005` | Catalog/index failure; additional model diagnostic for attached-index corruption | — |
| `COHDBD001` | Explicit transaction is aborted; operations/commit refused until rollback | — |
| `COHDBI002` | Additional attached-index corruption diagnostic beside COHDBS003 and COHDBD005 | — |
| `COHDBD003` | Missing journal/backup, incomplete creation or unrecoverable files during create/open; preserve ambiguous files and restore the complete file set, or drop/recreate only a known incomplete create as directed | — |
| `COHDBS003` | Detected storage corruption; DatabaseCorruptionException names the database and optional page and retains the storage cause; refused opens also retain COHDBD003 | — |

COHDBD003 is a database lifecycle diagnostic carried by DatabaseException.Diagnostics, rather than a
query evaluation error. A leftover directory without its data file remains DatabaseNotFoundException
with the same diagnostic. Missing sidecars are never recreated. Transient open I/O failures ask for
a retry once resolved and carry no restore/drop advice. No SQLSTATE is assigned to COHDBD003.

COHDBS003 also fails damaged reads/statements and transaction operations instead of silently
omitting a document. Legitimate reclamation keeps its absence semantics. The existing wire
execution-failure path carries the diagnostic and leaves the connection usable. Follow the
message's verified-backup advice. See [storage recovery](../../overview.md#database-file-sets-and-lifecycle)
and [DatabaseCorruptionException](../../../dotnet-apis/resources/database/assimalign-cohesion-database/database-corruption-exception.md).

Storage, catalog and index failures use structured model diagnostics with their original
InnerException. Wrapped I/O remains COHDBD004. Attached-index corruption leads with COHDBS003
and retains COHDBI002 and COHDBD005. Begin uses the same translator as operations. An I/O open
failure releases resources and publishes no database, permitting retry after resolution. An
operation failure aborts an explicit transaction; rollback permits embedded-session reuse.
The wire returns ExecutionFailure with diagnostics and preserves the connection. COHDBD001
is in Diagnostics; only the wire adds its message prefix.

## Operation cleanup

A content chunk failure or execution cancellation aborts through the engine's transaction end
gate, once; storage does not abort the borrowed context as well. An explicit session then
refuses commands/commit with COHDBD001 until rollback, retaining the original cause. Earlier
uncommitted chunks and writes are undone; no partial document becomes visible.

After successful automatic commit, a snapshot-cleanup failure preserves success and emits
`StatementCleanupFailed`, event **13**, Warning, from `Assimalign.Cohesion.Database.Documents`.
Its payload is `database`, `exceptionType`, with no exception message or document input.
`OutOfMemoryException` still propagates; cleanup failures before commit success or inside an
explicit transaction still fail the operation. The session hold is released in all cases.
This warning has no diagnostic code or SQLSTATE. See [engine diagnostics](../../../dotnet-apis/resources/database/assimalign-cohesion-database-documents/design.md#cleanup-diagnostics).

## See also

[Language (OQL)](index.md) · [Parameters](expressions/parameters.md) · [MISSING](expressions/missing.md) · [Unsupported](unsupported.md)

## Terminal wire delivery

Provider `Assimalign.Cohesion.Database.Documents` reports terminal-session deadline aborts:

| Id | Event | Level | Payload |
| --- | --- | --- | --- |
| 14 | TerminalDrainTimedOut | Warning | sessionId, drainTimeoutMilliseconds, suppressedCount |

Warnings are limited to one per second per provider; the next emitted warning carries the number
suppressed. The process-wide total-terminal-drain-timeouts polling counter counts every terminal
session deadline that wins the abort transition, even with events disabled or warnings suppressed.
Normal peer-close linger expiry and a server hard abort do not emit this event. Capacity refusals
have no session and do not increment this session counter. This warning adds no diagnostic code
or SQLSTATE.

The server drains complete shared terminal Errors and capacity refusals within its configured
ShutdownDrainTimeout, with TerminalDrainLinger bounding peer close after output completion.
TLS delivery remains limited by #1086. See [server options](../../../dotnet-apis/resources/database/assimalign-cohesion-database-documents/document-database-server-options.md).

## Sources

- **Storage integrity** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/StorageCorruptionTests.cs`.

- **File-set refusals and recovery** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentDatabaseNameTests.cs`.
- **Carriers** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/OqlDiagnosticTests.cs` and `OqlDiagnosticsFixPassTests.cs` in the same directory.
- **Numbers and duplicate keys** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentInsertTests.cs`.
- **Failures** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Protocol/src/DocumentProtocolFailureMessage.cs`.
