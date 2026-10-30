# GQL diagnostics

Graph uses `CG` across syntax, binding, value, schema, transaction, execution, storage, protocol, configuration and lifecycle categories. See the [code scheme and registry](../../error-codes.md).

GQL reports stable capability, syntax, planning, and execution diagnostics.

> **Status:** Implemented.

## Parser diagnostics

| Code | Meaning |
|---|---|
| `CK00001` | Recognized construct outside the executable profile |
| `CG00001` | Empty statement |
| `CG00002` | Malformed supported syntax/composition, including ORDER BY after paging, repeated paging clauses, OFFSET/SKIP after LIMIT or a noninteger/negative literal count |
| `CG00003` | Unterminated string, quoted name, or block comment |
| `CG00004` | Invalid or out-of-range numeric literal |
| `CG00005` | Text repetition-bound sum exceeds 64 relationships |
| `CG00006` | Duplicate literal property or map-constructor key |
| `CG00007` | `Graph catalog introspection is read-only.` |
| `CG00008` | -- touches a node/subpath, edge filler or postfix/Cypher quantifier and hides an arrow |
| `CG00009` | Parsing thread has insufficient stack for nested expressions/subpaths; engine reports CG05008 |
| `CG00010` | Unknown function name, reported while parsing |

The parser returns diagnostics on `GqlQueryStatement` rather than throwing for malformed query
text. Diagnostic locations use zero-based absolute UTF-16 offsets, exclusive end positions, and
one-based line numbers.

Capability checks precede syntax parsing. Unsupported recognized clauses therefore retain their
capability diagnostic instead of producing a downstream syntax error. Quoted text, comments,
label names, and property keys do not trigger keyword capability checks.

## Planning and execution diagnostics

Database-wide schema names and named type/existence constraints are exposed through the typed
[GraphSchema API](../../../../dotnet-apis/resources/database/assimalign-cohesion-database-graph/graph-schema.md).
Text [schema DDL](statements/create-index.md) uses the same named rules and autocommit coordination.
These conditions have no assigned SQLSTATE:

| Code | Meaning and trigger |
| --- | --- |
| CG01021 | Named index or constraint requested for DROP is absent |
| CG01022 | Schema property references a variable other than the FOR declaration |
| CG03008 | Database-wide index/constraint name is already occupied; names are ordinal and case-sensitive |
| CG03009 | Duplicate constraint kind or incompatible property type rules |
| CG03010 | Existing data violates the newly created named constraint; creation rolls back |
| CG04025 | Schema write inside an explicit transaction, including typed writes and conditional no-ops; aborts the transaction |
| CG06019 | Catalog record version is not v2; recreate the database, with no legacy reader |

CG06019 refuses before logical recovery scrub. Physical journal redo may already have run;
this is distinct from graph entity/tree format 5 and its CG06017/CG06018 checks. An empty catalog
has no versioned records to refuse. SHOW CONSTRAINTS returns independently named rules.
DDL validates GraphQueryLimits inside the admitted operation (CG08021). Owner scans and
backfill consume work (CG05004); cursor buffers and retained backfill entries consume memory
(CG05020). Exhaustion rolls back rules, reservations and new tokens. These codes have no
assigned SQLSTATE. CG04025 is checked before limits or writer-lock acquisition.

[List comprehensions](expressions/list-comprehensions.md) and [slices](expressions/list-slices.md)
add these conditions; none has an assigned SQLSTATE:

| Code | Meaning and trigger |
| --- | --- |
| CG00012 | Slice omits both bounds; refused during planning, including EXPLAIN |
| CG01017 | Missing/empty iterator declaration in a hand-built AST |
| CG02014 | Invalid list-operation source or slice-bound kind/range |
| CG05024 | Aggregate inside a comprehension filter or projection |
| CG01018 | Missing/empty reduce/list-predicate declaration or identical reduce binding names |
| CG02015 | Reduce/list-predicate source is neither a list nor NULL; planning rejects known incompatible kinds |
| CG05025 | Malformed reduce/list-predicate body or quantifier AST, or a body aggregate |
| CG05026 | Reduce shape/type planning exceeds 65,536 visited nodes or closure steps |

Known non-Boolean filters use CG02013 even for empty/NULL input. Unbound names retain CG05001.
Scalar property results, [reduce](expressions/reduce.md) and
[all/any/none/single](expressions/list-predicates.md) execute. List-valued properties and pattern
comprehensions retain CK00001. Reduction source/predicate kinds validate on empty/NULL input and
under EXPLAIN; unknown kinds are checked at evaluation. CG05026 is independent of execution budgets.

| Code | Meaning |
|---|---|
| `CG05001` | Invalid hand-built function arity or unbound argument; invalid pattern, variable binding, traversal, grouping/aggregate placement or sort specification; hidden DISTINCT sort binding or missing paging/key parameter |
| `CG01002` | Unknown label or relationship type |
| `CG03003` | Provably incompatible function/CAST kind or temporal stored property; schema/data mismatch, restricted deletion, LET/FOR rebinding, value used as entity, known scalar member owner or later reference to deleted variable; deterministic catalog identity/name/owner/index-target error |
| `CG05004` | Physical-work budget or runtime 64-hop trail limit exceeded |
| `CG07005` | Session/database binding mismatch |
| `CG06006` | Storage/index, malformed catalog record or open I/O failure translated at the engine boundary, including wrapped causes; original InnerException retained |
| `CK06102` | Attached-index corruption retained beside leading CK06003 and additional CG06006; no assigned SQLSTATE |
| `CG04007` | Retryable catalog snapshot conflict, or aborted explicit transaction refusing statements/BEGIN/COMMIT until rollback; original cause retained |
| `CG03004` | MERGE equality is NULL, non-finite or non-scalar, proven or evaluated; no assigned SQLSTATE |
| `CG03005` | Bound MERGE node is NULL, absent or incompatible with its node pattern; no assigned SQLSTATE |
| `CG03006` | Repeated or already-bound MERGE node has labels or properties; no assigned SQLSTATE |
| `CG03007` | MERGE relationship variable was declared before the clause; no assigned SQLSTATE |
| `CK04002` | MERGE statement/session ended while the database is live, including lock waits and late grants; no assigned SQLSTATE |
| `CG04024` | Retryable stale Snapshot MERGE pattern decision; no assigned SQLSTATE |
| `CG05023` | Invalid bound MERGE pattern lock identity; no assigned SQLSTATE |
| `CG05008` | Statement too complex for available stack (SQLSTATE 54001) |
| `CG02009` | Catalog metadata or graph element exceeds the 8,092-byte record, or indexed property exceeds the 1,016-byte key bound |
| `CG01010` | Warning: read names an unknown label; GQLSTATUS 01N50 |
| `CG01011` | Warning: read names an unknown relationship type; GQLSTATUS 01N51 |
| `CG06012` | Database offline after durable storage failure |
| `CG02013` | Evaluated value error: non-list FOR/UNWIND input, non-Boolean condition, invalid arithmetic/concatenation/access, zero divisor, overflow, non-finite result, function/CAST domain, kind or conversion failure, unsupported computed property, SUM/AVG nonnumeric operand or constructed/collected value/row limit |
| `CG01014` | Caller parameter keys collide after stripping one sigil |
| `CG01015` | Duplicate final RETURN column name under ordinal, case-sensitive comparison; planning rejects the second expression before reads/mutations, even on empty matches or under EXPLAIN; no SQLSTATE assigned |
| `CG07016` | Possible composite wire projection without negotiated graph.composite-values; before reads, writer locks or writes, even for empty matches or skipped branches; no SQLSTATE assigned |
| `CG06017` | Unsupported graph record or tree-registration format on open, including formats 1, 2, 3 and 4; requires format 5 and recreation, with no upgrade path or assigned SQLSTATE |
| `CG09019` | Database/resource disposed during admitted execution, result streaming or a wire lifetime guard; ExecutionFailure preserves the authenticated session; no assigned SQLSTATE |
| `CK06003` | Detected storage corruption; DatabaseCorruptionException carries database, optional page and original storage cause; no SQLSTATE assigned |

[MERGE](statements/merge.md) executes whole-pattern match/create and ordered ON CREATE/ON MATCH SET.
Snapshot conflicts use CG04024; ReadCommitted refreshes and re-matches. CG05023 guards pattern-lock
identities. Complete statically list/map-valued property expressions retain CK00001; other proven/evaluated invalid
equalities use CG03004. Graph record/tree-registration format remains 5.

[Scalar calls](functions/scalar-functions.md) use parsed-arity CG00002 and unknown-name CG00010;
hand-built arity/unbound arguments use CG05001. Proven incompatible kinds use CG03003,
dynamic kinds and domain/conversion failures CG02013. EXPLAIN validates without evaluation.
These codes have no assigned SQLSTATE; ELEMENTS and recognized unsupported calls use CK00001.

[Quantified patterns](patterns.md#diagnostics) use CG00002 for INSERT/node quantifiers and invalid
intervals, CG00004 for Int32 overflow, CG00005 for text bound sums above 64, and CG05001
for invalid direct AST shapes/bounds. Group/singleton conflicts, group property maps, direct group
member access and group deletion use CG03003. Questioned/nested quantifiers and subpath
assignment/WHERE use CK00001. These codes have no assigned SQLSTATE.

[Composition](clauses/next.md) adds no new codes. Dropped/unbound names and aggregate calls in WITH
WHERE use CG05001; duplicate carried bindings use CG01015, including under EXPLAIN. Leading/
trailing NEXT, NEXT after bare MATCH and WITH immediately followed by NEXT are CG00002. FOR
WITH ORDINALITY/OFFSET, NEXT YIELD, CALL and FOREACH retain CK00001. NULL FOR/UNWIND inputs
give zero rows; non-lists use CG02013. These composition codes have no assigned SQLSTATE.
Final lists of nodes/relationships over wire Execute remain CG05001 (#1662), even with composite
negotiation. A read-only composed query may carry one mandatory final entity through ExecutePaths.

Remote failures use GraphClientException, which derives directly from System.Exception and owns
its Diagnostics snapshot. Catch GraphClientException for remote operations; catching DatabaseException
no longer handles remote failures. See the [migration guide](../../model-project-migration.md#remote-failures).

Planner rejections carry stable codes in `DatabaseException.Diagnostics`, with available source
spans and code-free messages. Diagnostics is never null; located entries sort by start offset before
unlocated entries. The server composes `CODE: message`. Missing parameters use CG05001 and
unsupported/non-finite parameter values use CG03003; both fail before execution.
For example, `RETURN 1 AS x, 2 AS x` reports CG01015; `x` and `X` are distinct names.
Implicit names can also collide with aliases. Path requests check for exactly one bare entity/path
projection first and retain CG05001 for invalid shapes. Wire Execute rejects entity projections
with CG05001 before reads, writer locks or writes, even when no rows would match; use ExecutePaths.
Kernel aborts surface as `DatabaseTransactionAbortedException`; deadlocks retain their specialized
subtype. Schema ownership is enforced with `DatabaseObjectLockedException`, rather than a graph
ownership diagnostic code.

Catalog snapshot conflicts use DatabaseTransactionAbortedException with structured CG04007.
Reusing/renaming an identity, assigning another identity to a name, retargeting an index property
or naming a missing owner is a deterministic CG03003 caller error, not a retryable abort.
No SQLSTATE is assigned to CG03003, CG06006, CG04007 or CG02009. Storage/index/I/O
failure takes precedence over accompanying disposal or catalog conflict; kernel abort/unconfirmed
commit outcomes retain their transaction exception kind.

After an admitted statement failure, an explicit transaction refuses further work with an Error
diagnostic CG04007. The in-process Message starts with ordinary text, and InnerException retains
the original failure; the wire still formats `CG04007: message`. Roll back before continuing.
Pre-statement direct-API lifetime checks retain ObjectDisposedException; a commit/rollback refused
by a closing coordinator uses DatabaseException with CG09019 and the disposal cause.

Ordering and paging add no codes or SQLSTATE categories. Missing key/count parameters retain
CG05001. Invalid bound count values (NULL, negative, floating, nonnumeric or out of Int64 range)
use CG02013 before reads or writer locks; non-finite/unsupported CLR inputs retain CG03003.
[ORDER BY](clauses/order-by.md) and [paging](clauses/limit-offset.md) apply to output after all
mutations and DISTINCT. Unordered read LIMIT can stop before visiting a branch; visited work
remains subject to CG05004 and retained memory to CG05020.

SET/REMOVE add no diagnostic codes. Leading clauses, nested targets, separated + = and property
+= use CG00002; duplicate map keys CG00006. Whole-map parameters/expressions, literal composite
properties use CK00001; MERGE ON CREATE/ON MATCH SET executes. Scalar/path/group targets,
relationship label changes and computed composite properties use CG03003; ExecutePaths with
updates uses CG05001 and SHOW composition CG00007. No SQLSTATE is assigned to these update
codes. Stale Snapshot updates surface DatabaseTransactionAbortedException; ReadCommitted
re-snapshots after the writer lock. See [SET](clauses/set.md) and [REMOVE](clauses/remove.md).

CG06017 is an open-time format refusal before logical recovery scrub. Graph.Storage throws
StorageCorruptionException with the code in its message; the engine retains CG06017 and
CG06018 alongside CK06003 in DatabaseCorruptionException.Diagnostics. Recreate a database
whose graph record format is unsupported; there is no upgrade reader.
The engine requires format 5. A cleanly closed older file set stays byte-identical on refusal;
physical WAL recovery can precede the fence for a crashed file set.

A graph record shorter than its 18-byte format header is located truncation corruption,
not an unsupported format. It reports `CK06003` with page/slot context and an
`EndOfStreamException` cause. Only a readable unsupported version receives `CG06017` and
the recreate-database remedy.

CK06003 also applies to damaged reads, statements, transactions and create/open failures,
including wrapped or aggregate storage corruption. It names the database and page when known
and advises restoration from a verified backup. A read fails rather than omitting damaged nodes
or relationships; legitimate reclamation retains absence semantics. Wire ExecutionFailure keeps
the connection usable. See [storage recovery](../../overview.md#database-file-sets-and-lifecycle)
and [DatabaseCorruptionException](../../../dotnet-apis/resources/database/assimalign-cohesion-database/database-corruption-exception.md).

## Boundaries

An unknown label/type in OPTIONAL MATCH still warns with CG01010/CG01011, but the clause preserves
its input with NULL for new variables. It does not empty the whole query. A later mandatory clause
with an unknown name can still empty it. Optional null extension leaves an explicit transaction active.

| Limit | Value | Error |
|---|---|---|
| Text repetition-bound sum | 64 relationships | `CG00005` |
| Runtime trail length | 64; qualifying 65th edge fails before endpoint tests | `CG05004` |
| Recursive expression depth | Available thread stack | `CG00009` / `CG05008` |
| Accounted retained execution memory | Configured engine ceiling, optionally lowered per request | `CG05020` |
| Physical candidate visits | Configured per-statement ceiling: anchors plus selected visible relationships, including merge-initialization heads, type discovery and dead ends; discovery/first head share one charge; overflow reports actual physical/viable totals | `CG05004` |
| Composite value depth (root zero) | 64 | Parameter CG03003; constructed CG02013 |
| Total values per value, including root | 65,536 | Parameter CG03003; constructed CG02013 |
| Encoded value or result-row bytes | 16,777,216 | Parameter CG03003; constructed/row CG02013 |
| Complete encoded catalog record, including UTF-8 names and owning schema | 8,092 bytes; exact fit accepted, overflow refused before physical apply | `CG02009` |

Referenced parameter trees snapshot before reads/writes; invalid dimensions, cycles and unsupported
or non-finite leaves use CG03003. Constructed bounds check during evaluation inside the statement
transaction, before commit. A refused result rolls back its writes. Invalid containers/indexes use
CG02013. [Value limits](expressions/list-values.md#limits-and-wire-capability) include UTF-8,
escaping and framing. Malformed composite wire components use ProtocolViolation.

## Negotiated wire diagnostics

Embedded row, mutation and path results expose non-null DatabaseResult.Diagnostics, empty when
nothing is reported; GraphSchemaResult<T> uses the same collection rule. Remote GraphResultSet,
GraphPathResult and GraphClientException diagnostics are also never null. GraphPathResult's
collection is empty until diagnostics arrive, and its final contents are available at EOF.

Protocol 2.1 with diagnostics sends Warning/Error entries before Error or ResultComplete/PathsComplete.
Information is omitted. Bounds are 4,096 entries and 1 MiB; overflow retains a leading prefix plus
`CK07001` with retained/original/omitted counts. The original failure or success remains unchanged
and the session stays usable. Negotiated 2.0 retains baseline Error/result bytes without the frame.
Malformed UTF-8, diagnostics or ordering are protocol violations.

GraphPathResult.Diagnostics finalizes at EOF. Unknown-name reads can complete with warnings and
zero paths. A terminal rejection before path output retains the connection; cancellation or an
unfinished path stream discards it.

Malformed catalog records and index/storage errors report CG06006; execution-resource disposal
reports CG09019. They are terminal ExecutionFailure responses and leave the authenticated session
ready. A disposed protocol reader/writer instead faults the session without another error write.
Disposal failure during cleanup after a successful autocommit does not change its success, except
OutOfMemoryException still propagates. Graph EventSource Warning 13, StatementCleanupFailed,
records database and exceptionType; do not retry a committed statement because of that warning.

Aggregates add no codes or SQLSTATE categories. Wrong arity, SUM(*) and COUNT(DISTINCT/ALL *)
use CG00002. Nested/misplaced aggregates, GROUP BY mismatch/aggregate aliases, hidden group sort
references and final aggregate path projections use CG05001. SUM/AVG overflow, nonnumeric arguments and
COLLECT_LIST bounds use CG02013. Unnegotiated wire COLLECT_LIST uses CG07016 before output.
See [aggregate diagnostics](functions/aggregate-functions.md#diagnostics).

## Database files

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| `CG06018` | Missing journal/backup, incomplete creation or unrecoverable files during create/open; preserve ambiguous files and restore the complete file set, or drop/recreate only a known incomplete create as directed | — |

This is a database lifecycle diagnostic carried by DatabaseException.Diagnostics, rather than a
query evaluation error. A leftover directory without its data file remains DatabaseNotFoundException
with the same diagnostic. Missing sidecars are never recreated. Transient open I/O failures ask for
a retry once resolved and carry no restore/drop advice. No SQLSTATE is assigned to CG06018.

Runtime storage/index failures use CG06006 in Diagnostics with ordinary embedded messages.
Begin uses the same translator. Attached-index corruption leads with CK06003 and retains
CK06102 and CG06006 with the index cause/page. An I/O open failure releases resources,
publishes no database and permits retry after resolution. An operation failure aborts an
explicit transaction until rollback. Wire ExecutionFailure adds the prefix and keeps the session.

## See also

- **[Language (GQL)](index.md)** — reference hub.
- **[Unsupported features](unsupported.md)** — capability matrix.
- **[SHOW](statements/show.md)** — read-only catalog composition.

## Path delivery counters

The path delivery changes add no diagnostic codes or SQLSTATE mappings. Provider
Assimalign.Cohesion.Database.Graph reports event 15, PathPhaseCompleted (Verbose, Sessions),
with sessionId, phase (Execute/Encode/Send) and durationMilliseconds. It publishes cumulative
path-execute-calls, path-encode-calls, path-send-calls and corresponding
path-execute-milliseconds, path-encode-milliseconds and path-send-milliseconds counters.

Assimalign.Cohesion.Database.Graph.Client publishes path-decode-calls,
path-decode-milliseconds and path-backpressure-waits. Event 5, PathDecodeCompleted (Verbose,
Queries), carries durationMilliseconds; event 4, PathReleaseFailed (Error), carries only the
exceptionType for a secondary rental-return failure, preserving the original path failure.

Assimalign.Cohesion.Database.Protocol publishes transport-write-calls,
transport-flush-calls and pooled-buffers-in-use for batched writers and borrowed reads.
Backing values are maintained without a listener. Transport counts include failed attempts;
ordinary writers are excluded. See [phase meanings and timing boundaries](../../../dotnet-apis/resources/database/assimalign-cohesion-database-graph/design.md#path-performance-counters),
[client counters](../../../dotnet-apis/resources/database/assimalign-cohesion-database-graph-client/design.md#path-performance-and-diagnostics)
and [protocol counter scope](../../../dotnet-apis/resources/database/assimalign-cohesion-database-protocol/design.md#protocol-eventsource).

## Terminal wire delivery

Provider `Assimalign.Cohesion.Database.Graph` reports terminal-session deadline aborts:

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
TLS delivery remains limited by #1086. See [server options](../../../dotnet-apis/resources/database/assimalign-cohesion-database-graph/graph-database-server-options.md).

## Maintenance worker events

Provider `Assimalign.Cohesion.Database` reports maintenance lane delays for this model:

| Id | Event | Level | Payload |
| --- | --- | --- | --- |
| 35 | WorkerLaneDelayed | Warning | workerName, workerKind, database, reason, elapsedSeconds |
| 36 | WorkerLaneDisposalWaiting | Warning | workerName, workerKind, database, elapsedSeconds |

Event 35 names `capacity` deferral (zero elapsed time) or a `stuck` operation (its runtime),
limited to once per second per database/worker. The accompanying worker failure is observational:
engine Faulted can coexist with a serving database, and the report does not advance the offline
policy or replace an unresolved operational fault. Event 36 repeats once per second for each
running lane while disposal waits, measuring elapsed disposal time. Storage stays open until I/O
returns. These events add no diagnostic code or SQLSTATE. See [lane limits and worker health](../../../dotnet-apis/resources/database/assimalign-cohesion-database/design.md#maintenance-lane-isolation).

## Failure tracing

The shared StatementFailed event takes its payload code from non-null Diagnostics: first Error, otherwise first entry, otherwise empty. It does not extract codes from messages or include statement/message text. Offline and unconfirmed-commit failures carry their model code in that carrier. See [failure-event selection](../../../dotnet-apis/resources/database/assimalign-cohesion-database/design.md#structured-failure-event-codes).

## Execution budgets

CG05020 reports accounted retained-memory exhaustion. CG05004 reports physical-work or runtime trail exhaustion. CG08021 rejects nonpositive request limits or values above engine ceilings. None has an assigned SQLSTATE. Configure [engine ceilings and lower embedded limits](../../../dotnet-apis/resources/database/assimalign-cohesion-database-graph/graph-database-engine-options.md).

## Typed shortest search

Typed shortest searches use the same memory/work ceilings but have no 64-hop cap. Exhaustion
fails rather than returning no-path. During reconstruction, CG05022 means a selected node or
relationship is absent while the operation is active. Session closure retains CK07004 and an ended
operation CK04002; these take precedence over CG05022. None has an assigned SQLSTATE. See the
[typed API](../../../dotnet-apis/resources/database/assimalign-cohesion-database-graph/graph-database.md#shortest-distance-and-path).

## Path modes

Outer WALK, TRAIL, SIMPLE and ACYCLIC execute; omitted mode means TRAIL. CK00001 refuses a
mode inside a parenthesized/quantified subpath or an unselected unbounded WALK even with LIMIT.
Selectors permit unbounded WALK during execution under the same statement budgets. CG00002 covers text
insertion modes; CG05001 covers undefined direct AST modes or
insertion modes. All modes retain CG05004 for hop/work limits and CG05020 for retained-memory
limits. No SQLSTATE is assigned to these conditions. See [path modes](patterns.md#path-modes).

## Path selector conditions

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CG00011 | SHORTEST count is missing or is not a positive Int64 literal | — |
| CG01016 | Search prefilter references a peer-path-only binding in the same MATCH, regardless of comma order | — |
| CK00001 | Counted ANY or nested selector/mode; unselected unbounded WALK | — |

[Selectors](path-selectors.md) execute, including inline prefilters without a selector. Work or
unresolved 64-hop searches use CG05004; retained history/state/output uses CG05020. Neither has
an assigned SQLSTATE. Exhaustion yields no partial selection and rolls back statement writes.
Invalid direct AST selector kinds/counts and insertion selectors retain CG05001 (no SQLSTATE).

## Application function conditions

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CG05021 | Application callback failed, result violated its declaration, or aggregate state was null/reused/invalid | — |
| CK07003 | Same-session callback reentry refused before nested execution | — |

CG02013 retains dynamic argument-shape/adapter-cast failures before callback entry; CG03003 covers provably incompatible kinds.
Application I/O, timeout and callback-thrown conversion/overflow failures use CG05021,
retaining the original cause. DatabaseException, out-of-memory, insufficient stack and actual
statement cancellation keep their identity. Unknown names retain CG00010 but ordinary engine
requests resolve them during binding/execution (ExecutionFailure, no StatementParseFailed);
standalone parsers retain unknown-name parse diagnostics. See [functions](functions/index.md#application-functions).

## Correlated GQL EXISTS

These codes have no assigned SQLSTATE. See [EXISTS](expressions/exists.md).

| Code | Meaning and trigger |
| --- | --- |
| CG00013 | EXISTS lacks a nonempty braced pattern |
| CG01019 | Proven incompatible correlated pattern kind |
| CG02016 | Incompatible runtime correlated entity kind |
| CG05027 | Malformed/missing correlated subplan or missing argument |

## Pattern comprehensions

[Pattern comprehensions](expressions/pattern-comprehensions.md) collect every correlated inner match in order.
The optional path name shadows locally; other existing pattern names correlate. NULL arguments
produce no matches, duplicates and NULL projections remain, and zero-hop relationship quantifiers
are accepted. Inner aggregates are refused. EXPLAIN shows CollectApply. Collection shares the
statement operation, snapshot, update overlay and work/memory budgets (CG05004/CG05020).
Scalar composite results use graph.composite-values; entity lists remain CG05001 until #1662.

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CG00014 | Pattern comprehension requires a nonempty path and projection | — |
| CG05028 | Malformed pattern comprehension, aggregate projection or missing collection plan | — |

## GQL composite queries

[UNION, EXCEPT, INTERSECT and OTHERWISE](clauses/union.md) execute with isolated operand scopes, matching case-sensitive
columns, type reconciliation and one statement snapshot. UNION keeps the first structural
occurrence; UNION ALL preserves duplicates. Paging is operand-local and NEXT binds above UNION.
All operands validate before execution. Other memory/work exhaustion retains CG05020/CG05004.
EXCEPT/INTERSECT default to DISTINCT or accept ALL multiset counts. They retain eligible left
occurrences and share structural row equality with UNION. OTHERWISE returns the whole left result
when nonempty, including NULLs/duplicates, or lazily evaluates the right on zero rows. Left errors
propagate. All four operators have equal precedence and associate left; each binds above NEXT.
EXCEPT/INTERSECT quantifiers may mix; only UNION/UNION ALL mixing is refused. Each parent hash visit
or forwarded row consumes shared work, including duplicate visits. Retained EXCEPT/INTERSECT state
and frames use CG05032; ordinary operand/output/forwarding frames use CG05020. No SQLSTATE is assigned.

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CG00015 | Missing composite operand, including ALL/DISTINCT after OTHERWISE | — |
| CG00016 | Mixed UNION distinct and UNION ALL modes within one tree | — |
| CG01020 | Operand column names, order or count differ | — |
| CG02017 | Proven or runtime result shapes are incompatible | — |
| CG05029 | Modifying operand or stage in a composite pipeline | — |
| CG05030 | UNION duplicate-elimination state exceeds the shared memory grant | — |
| CG05031 | Internal statement lookup unexpectedly lacks its key; original KeyNotFoundException is retained | — |
| CG05032 | EXCEPT/INTERSECT retained frames, rows or hash state exceed the shared memory grant | — |

## Sources

- **Batch 14** — `cohesion/resources/Database/Assimalign.Cohesion.Database/docs/ERROR-CODES.md` at `c3dc498bf`, checked against the batch-14 code and tests.

- **Batch 13** — `cohesion/resources/Database/Assimalign.Cohesion.Database/docs/ERROR-CODES.md` at `4ef359bad`, checked against `GraphUnionTests.cs` and `GraphUnionExecutionTests.cs`.

- **Batch 12** — `cohesion/resources/Database/Assimalign.Cohesion.Database/docs/ERROR-CODES.md` at `4607aa81f`, checked against code and test cases.

- **Batch 11** — `cohesion/resources/Database/Assimalign.Cohesion.Database/docs/ERROR-CODES.md` at `ae48b04cb`, checked against the executable tests.

- **Storage integrity and format refusals** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/StorageCorruptionTests.cs` and `GraphEngineTests.cs` in the same directory.

- **File-set refusals and recovery** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphDatabaseNameTests.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/docs/DESIGN.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlQueryParserTests.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlCatalogParserTests.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/docs/DESIGN.md`.
- **Catalog, wire and cleanup cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphCatalogErrorTests.cs`, `GraphEngineFailureWireTests.cs`, `GraphOperationOutcomeTests.cs` and `GraphHygieneTests.cs`.
