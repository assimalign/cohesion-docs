# GQL diagnostics

GQL reports stable capability, syntax, planning, and execution diagnostics.

> **Status:** Implemented.

## Parser diagnostics

| Code | Meaning |
|---|---|
| `COHDBL001` | Recognized construct outside the executable profile |
| `GQL0001` | Empty statement |
| `GQL0002` | Malformed supported syntax/composition, including ORDER BY after paging, repeated paging clauses, OFFSET/SKIP after LIMIT or a noninteger/negative literal count |
| `GQL0003` | Unterminated string, quoted name, or block comment |
| `GQL0004` | Invalid or out-of-range numeric literal |
| `GQL0005` | Text repetition-bound sum exceeds 64 relationships |
| `GQL0006` | Duplicate literal property or map-constructor key |
| `GQL0007` | `Graph catalog introspection is read-only.` |
| `GQL0008` | -- touches a node/subpath, edge filler or postfix/Cypher quantifier and hides an arrow |
| `GQL0009` | Parsing thread has insufficient stack for nested expressions/subpaths; engine reports COHDBG008 |
| `GQL0010` | Unknown function name, reported while parsing |

The parser returns diagnostics on `GqlQueryStatement` rather than throwing for malformed query
text. Diagnostic locations use zero-based absolute UTF-16 offsets, exclusive end positions, and
one-based line numbers.

Capability checks precede syntax parsing. Unsupported recognized clauses therefore retain their
capability diagnostic instead of producing a downstream syntax error. Quoted text, comments,
label names, and property keys do not trigger keyword capability checks.

## Planning and execution diagnostics

| Code | Meaning |
|---|---|
| `COHDBG001` | Invalid hand-built function arity or unbound argument; invalid pattern, variable binding, traversal, grouping/aggregate placement or sort specification; hidden DISTINCT sort binding or missing paging/key parameter |
| `COHDBG002` | Unknown label or relationship type |
| `COHDBG003` | Provably incompatible function/CAST kind or temporal stored property; schema/data mismatch, restricted deletion, LET/FOR rebinding, value used as entity, known scalar member owner or later reference to deleted variable; deterministic catalog identity/name/owner/index-target error |
| `COHDBG004` | Materialized MATCH/FOR row, candidate-expansion or runtime 64-hop trail limit exceeded |
| `COHDBG005` | Session/database binding mismatch |
| `COHDBG006` | Storage/index, malformed catalog record or open I/O failure translated at the engine boundary, including wrapped causes; original InnerException retained |
| `COHDBI002` | Attached-index corruption retained beside leading COHDBS003 and additional COHDBG006; no assigned SQLSTATE |
| `COHDBG007` | Retryable catalog snapshot conflict, or aborted explicit transaction refusing statements/BEGIN/COMMIT until rollback; original cause retained |
| `COHDBG008` | Statement too complex for available stack (SQLSTATE 54001) |
| `COHDBG009` | Catalog metadata or graph element exceeds the 8,092-byte record, or indexed property exceeds the 1,016-byte key bound |
| `COHDBG010` | Warning: read names an unknown label; GQLSTATUS 01N50 |
| `COHDBG011` | Warning: read names an unknown relationship type; GQLSTATUS 01N51 |
| `COHDBG012` | Database offline after durable storage failure |
| `COHDBG013` | Evaluated value error: non-list FOR/UNWIND input, non-Boolean condition, invalid arithmetic/concatenation/access, zero divisor, overflow, non-finite result, function/CAST domain, kind or conversion failure, unsupported computed property, SUM/AVG nonnumeric operand or constructed/collected value/row limit |
| `COHDBG014` | Caller parameter keys collide after stripping one sigil |
| `COHDBG015` | Duplicate final RETURN column name under ordinal, case-sensitive comparison; planning rejects the second expression before reads/mutations, even on empty matches or under EXPLAIN; no SQLSTATE assigned |
| `COHDBG016` | Possible composite wire projection without negotiated graph.composite-values; before reads, writer locks or writes, even for empty matches or skipped branches; no SQLSTATE assigned |
| `COHDBG017` | Unsupported graph record or tree-registration format on open, including formats 1, 2 and 3; requires format 4 and recreation, with no upgrade path or assigned SQLSTATE |
| `COHDBG019` | Database/resource disposed during admitted execution, result streaming or a wire lifetime guard; ExecutionFailure preserves the authenticated session; no assigned SQLSTATE |
| `COHDBS003` | Detected storage corruption; DatabaseCorruptionException carries database, optional page and original storage cause; no SQLSTATE assigned |

[Scalar calls](functions/scalar-functions.md) use parsed-arity GQL0002 and unknown-name GQL0010;
hand-built arity/unbound arguments use COHDBG001. Proven incompatible kinds use COHDBG003,
dynamic kinds and domain/conversion failures COHDBG013. EXPLAIN validates without evaluation.
These codes have no assigned SQLSTATE; ELEMENTS and recognized unsupported calls use COHDBL001.

[Quantified patterns](patterns.md#diagnostics) use GQL0002 for INSERT/node quantifiers and invalid
intervals, GQL0004 for Int32 overflow, GQL0005 for text bound sums above 64, and COHDBG001
for invalid direct AST shapes/bounds. Group/singleton conflicts, group property maps, direct group
member access and group deletion use COHDBG003. Questioned/nested quantifiers and subpath
assignment/WHERE use COHDBL001. These codes have no assigned SQLSTATE.

[Composition](clauses/next.md) adds no new codes. Dropped/unbound names and aggregate calls in WITH
WHERE use COHDBG001; duplicate carried bindings use COHDBG015, including under EXPLAIN. Leading/
trailing NEXT, NEXT after bare MATCH and WITH immediately followed by NEXT are GQL0002. FOR
WITH ORDINALITY/OFFSET, NEXT YIELD, CALL and FOREACH retain COHDBL001. NULL FOR/UNWIND inputs
give zero rows; non-lists use COHDBG013. These composition codes have no assigned SQLSTATE.
Final lists of nodes/relationships over wire Execute remain COHDBG001 (#1662), even with composite
negotiation. A read-only composed query may carry one mandatory final entity through ExecutePaths.

Remote failures use GraphClientException, which derives directly from System.Exception and owns
its Diagnostics snapshot. Catch GraphClientException for remote operations; catching DatabaseException
no longer handles remote failures. See the [migration guide](../../model-project-migration.md#remote-failures).

Planner rejections carry stable codes in `DatabaseException.Diagnostics`, with available source
spans and code-free messages. Diagnostics is never null; located entries sort by start offset before
unlocated entries. The server composes `CODE: message`. Missing parameters use COHDBG001 and
unsupported/non-finite parameter values use COHDBG003; both fail before execution.
For example, `RETURN 1 AS x, 2 AS x` reports COHDBG015; `x` and `X` are distinct names.
Implicit names can also collide with aliases. Path requests check for exactly one bare entity/path
projection first and retain COHDBG001 for invalid shapes. Wire Execute rejects entity projections
with COHDBG001 before reads, writer locks or writes, even when no rows would match; use ExecutePaths.
Kernel aborts surface as `DatabaseTransactionAbortedException`; deadlocks retain their specialized
subtype. Schema ownership is enforced with `DatabaseObjectLockedException`, rather than a graph
ownership diagnostic code.

Catalog snapshot conflicts use DatabaseTransactionAbortedException with structured COHDBG007.
Reusing/renaming an identity, assigning another identity to a name, retargeting an index property
or naming a missing owner is a deterministic COHDBG003 caller error, not a retryable abort.
No SQLSTATE is assigned to COHDBG003, COHDBG006, COHDBG007 or COHDBG009. Storage/index/I/O
failure takes precedence over accompanying disposal or catalog conflict; kernel abort/unconfirmed
commit outcomes retain their transaction exception kind.

After an admitted statement failure, an explicit transaction refuses further work with an Error
diagnostic COHDBG007. The in-process Message starts with ordinary text, and InnerException retains
the original failure; the wire still formats `COHDBG007: message`. Roll back before continuing.
Pre-statement direct-API lifetime checks retain ObjectDisposedException; a commit/rollback refused
by a closing coordinator uses DatabaseException with COHDBG019 and the disposal cause.

Ordering and paging add no codes or SQLSTATE categories. Missing key/count parameters retain
COHDBG001. Invalid bound count values (NULL, negative, floating, nonnumeric or out of Int64 range)
use COHDBG013 before reads or writer locks; non-finite/unsupported CLR inputs retain COHDBG003.
[ORDER BY](clauses/order-by.md) and [paging](clauses/limit-offset.md) apply to output after all
mutations and DISTINCT; LIMIT does not bypass COHDBG004 bounds.

SET/REMOVE add no diagnostic codes. Leading clauses, nested targets, separated + = and property
+= use GQL0002; duplicate map keys GQL0006. Whole-map parameters/expressions, literal composite
properties and MERGE ON CREATE/ON MATCH SET use COHDBL001. Scalar/path/group targets,
relationship label changes and computed composite properties use COHDBG003; ExecutePaths with
updates uses COHDBG001 and SHOW composition GQL0007. No SQLSTATE is assigned to these update
codes. Stale Snapshot updates surface DatabaseTransactionAbortedException; ReadCommitted
re-snapshots after the writer lock. See [SET](clauses/set.md) and [REMOVE](clauses/remove.md).

COHDBG017 is an open-time format refusal before logical recovery scrub. Graph.Storage throws
StorageCorruptionException with the code in its message; the engine retains COHDBG017 and
COHDBG018 alongside COHDBS003 in DatabaseCorruptionException.Diagnostics. Recreate a database
whose graph record format is unsupported; there is no upgrade reader.
The engine requires format 4. A cleanly closed older file set stays byte-identical on refusal;
physical WAL recovery can precede the fence for a crashed file set.

COHDBS003 also applies to damaged reads, statements, transactions and create/open failures,
including wrapped or aggregate storage corruption. It names the database and page when known
and advises restoration from a verified backup. A read fails rather than omitting damaged nodes
or relationships; legitimate reclamation retains absence semantics. Wire ExecutionFailure keeps
the connection usable. See [storage recovery](../../overview.md#database-file-sets-and-lifecycle)
and [DatabaseCorruptionException](../../../dotnet-apis/resources/database/assimalign-cohesion-database/database-corruption-exception.md).

## Boundaries

An unknown label/type in OPTIONAL MATCH still warns with COHDBG010/011, but the clause preserves
its input with NULL for new variables. It does not empty the whole query. A later mandatory clause
with an unknown name can still empty it. Optional null extension leaves an explicit transaction active.

| Limit | Value | Error |
|---|---|---|
| Text repetition-bound sum | 64 relationships | `GQL0005` |
| Runtime trail length | 64; qualifying 65th edge fails before endpoint tests | `COHDBG004` |
| Recursive expression depth | Available thread stack | `GQL0009` / `COHDBG008` |
| Materialized intermediate matches | 100,000 across matching clauses | `COHDBG004` |
| Physical candidate visits | 1,000,000 per statement: anchors plus selected visible relationships, including merge-initialization heads, type discovery and dead ends; discovery/first head share one charge; overflow reports actual physical/viable totals | `COHDBG004` |
| Composite value depth (root zero) | 64 | Parameter COHDBG003; constructed COHDBG013 |
| Total values per value, including root | 65,536 | Parameter COHDBG003; constructed COHDBG013 |
| Encoded value or result-row bytes | 16,777,216 | Parameter COHDBG003; constructed/row COHDBG013 |
| Complete encoded catalog record, including UTF-8 names and owning schema | 8,092 bytes; exact fit accepted, overflow refused before physical apply | `COHDBG009` |

Referenced parameter trees snapshot before reads/writes; invalid dimensions, cycles and unsupported
or non-finite leaves use COHDBG003. Constructed bounds check during evaluation inside the statement
transaction, before commit. A refused result rolls back its writes. Invalid containers/indexes use
COHDBG013. [Value limits](expressions/list-values.md#limits-and-wire-capability) include UTF-8,
escaping and framing. Malformed composite wire components use ProtocolViolation.

## Negotiated wire diagnostics

Protocol 2.1 with diagnostics sends Warning/Error entries before Error or ResultComplete/PathsComplete.
Information is omitted. Bounds are 4,096 entries and 1 MiB; overflow retains a leading prefix plus
`COHDBP001` with retained/original/omitted counts. The original failure or success remains unchanged
and the session stays usable. Negotiated 2.0 retains baseline Error/result bytes without the frame.
Malformed UTF-8, diagnostics or ordering are protocol violations.

GraphPathResult.Diagnostics finalizes at EOF. Unknown-name reads can complete with warnings and
zero paths. A terminal rejection before path output retains the connection; cancellation or an
unfinished path stream discards it.

Malformed catalog records and index/storage errors report COHDBG006; execution-resource disposal
reports COHDBG019. They are terminal ExecutionFailure responses and leave the authenticated session
ready. A disposed protocol reader/writer instead faults the session without another error write.
Disposal failure during cleanup after a successful autocommit does not change its success, except
OutOfMemoryException still propagates. Graph EventSource Warning 13, StatementCleanupFailed,
records database and exceptionType; do not retry a committed statement because of that warning.

Aggregates add no codes or SQLSTATE categories. Wrong arity, SUM(*) and COUNT(DISTINCT/ALL *)
use GQL0002. Nested/misplaced aggregates, GROUP BY mismatch/aggregate aliases, hidden group sort
references and final aggregate path projections use COHDBG001. SUM/AVG overflow, nonnumeric arguments and
COLLECT_LIST bounds use COHDBG013. Unnegotiated wire COLLECT_LIST uses COHDBG016 before output.
See [aggregate diagnostics](functions/aggregate-functions.md#diagnostics).

## Database files

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| `COHDBG018` | Missing journal/backup, incomplete creation or unrecoverable files during create/open; preserve ambiguous files and restore the complete file set, or drop/recreate only a known incomplete create as directed | — |

This is a database lifecycle diagnostic carried by DatabaseException.Diagnostics, rather than a
query evaluation error. A leftover directory without its data file remains DatabaseNotFoundException
with the same diagnostic. Missing sidecars are never recreated. Transient open I/O failures ask for
a retry once resolved and carry no restore/drop advice. No SQLSTATE is assigned to COHDBG018.

Runtime storage/index failures use COHDBG006 in Diagnostics with ordinary embedded messages.
Begin uses the same translator. Attached-index corruption leads with COHDBS003 and retains
COHDBI002 and COHDBG006 with the index cause/page. An I/O open failure releases resources,
publishes no database and permits retry after resolution. An operation failure aborts an
explicit transaction until rollback. Wire ExecutionFailure adds the prefix and keeps the session.

## See also

- **[Language (GQL)](index.md)** — reference hub.
- **[Unsupported features](unsupported.md)** — capability matrix.
- **[SHOW](statements/show.md)** — read-only catalog composition.

## Sources

- **Storage integrity and format refusals** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/StorageCorruptionTests.cs` and `GraphEngineTests.cs` in the same directory.

- **File-set refusals and recovery** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphDatabaseNameTests.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/docs/DESIGN.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlQueryParserTests.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlCatalogParserTests.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/docs/DESIGN.md`.
- **Catalog, wire and cleanup cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphCatalogErrorTests.cs`, `GraphEngineFailureWireTests.cs`, `GraphOperationOutcomeTests.cs` and `GraphHygieneTests.cs`.
