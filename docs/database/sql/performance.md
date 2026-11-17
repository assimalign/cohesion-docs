# SQL query performance

SQL chooses among eligible access paths using bounded probes under the statement snapshot. No
persisted statistics or standalone ANALYZE command exists. Each range count examines at most 512
physical entries and 32 page visits; invisible entries, descent pages and stopping keys consume
work. Incomplete counts are lower bounds, and unknown inputs use heuristics. Probes use the tree
read latch and validate table/index identities around the count, without transaction object locks.

[Partial indexes](language/statements/create-index.md#partial-indexes) can supply seeks, ranges,
join probes and unbounded subset scans when mandatory query conjuncts prove their predicate.
Eligible paths participate in bounded cardinality probes and cost selection. The complete query
predicate remains a residual filter; EXPLAIN records `partial_predicate` and `implied_by`.

[Join-order search](language/clauses/join.md#join-order-search) enumerates bushy and left-deep
orders for three through eight eligible SELECT relations and two through eight DML sources.
From nine through 64 eligible relations, deterministic connected greedy search chooses an
ordered seed pair, then adds one relation at a time, considering connected joins before cross
products. Planning work grows polynomially and stops at the 64-relation search limit; larger
graphs keep the written association under ordinary costing and remain executable.
Two-input SELECTs retain their existing cost path. Outer joins, derived/view/system inputs,
subquery predicates and unproven predicates retain bound association.

Above eight, a candidate must be estimated no more expensive, use no more retained memory
and preserve the written plan's access paths. The retention check counts every hash build and
nested-loop buffer, including decoded rows and array growth; an unknown or incomplete count
cannot justify moving a streamed input into a buffer. Safe filter pushdown can also improve
the written order and is eligible at equal work. Flat joined DML and existing hash builds keep
their order and consider only that pushdown; normalized trees keep their existing plan.
The same query and data produce the same plan across platforms. Output columns and results
remain unchanged; [EXPLAIN](language/statements/explain.md#join-order-search) exposes the choice
and search work. Search introduces no diagnostic codes.

Costing is lazy. Complete unique-key equality lookups avoid probes. The primary-key/base-table
total is probed only when the selected seek estimate exceeds 16 candidates. Join estimates reuse
input access counts and run only for hash choices or competing indexed directions; a complete
unique first-step probe skips pair costing, and later unique/indexed steps need no pair estimates.

Eligible equijoins between tables without countable indexes select an in-memory hash join
regardless of actual size, including empty or tiny inputs: both estimates use the 4,096-row
heuristic. Small exact indexed inputs and complete unique probes retain index nested
loops. Any ON reference to a derived String/Json/Character column prevents hash selection, even
in a residual term, preserving deferred collation/Unicode errors. Full predicates, duplicates,
NULL and LEFT semantics remain intact. See [joins](language/clauses/join.md#hash-joins-and-memory).

Configure the engine before creation:

```csharp
using Assimalign.Cohesion.Database.Sql;

var options = new SqlDatabaseEngineOptions { StatementMemoryBudget = 32L * 1024 * 1024 };
await using var engine = SqlDatabaseEngine.Create("orders", options);
```

The positive byte budget defaults to 64 MiB and covers retained SELECT buffers. Eligible root
ORDER BY, ordered DISTINCT, eligible hash joins and plain hash aggregation/DISTINCT spill where
they would otherwise refuse CS05067 (53200). Every eligible retained SELECT output also has
a spill owner, including nested results, set operations and partitioned windows. File engines use
configured temporary space; in-memory engines need an explicit TempDirectory. Fitting sorts retain their existing path.

## Statement memory

Joins charge hash builds, nested-loop inners, normalized child buffers and staged matches.
Grouping charges keys, aggregate state, DISTINCT tuples and ordered aggregate inputs. Full sorts,
bounded top-N entries, output rows, DISTINCT membership, derived/view/CTE caches, subquery values
and correlated vectors, and set operands, seen sets and counts share the statement grant.
Every buffer reserves before retention and releases on consumption or scope exit, including
failure and cancellation. Child execution releases its result charge; a consumer that keeps the
materialized result admits it again. Charges follow retained rows, never list capacity.

A cell of at least 256 bytes charges its payload once per buffer owner by reference identity,
plus 64 bytes per distinct wide cell. Further references pay array slots; smaller cells pay per
reference. Equal values in separate objects, including binary clones, each pay. Outputs and sort
keys share their collector's map. A value in two live buffers charges in each; hash builds still
charge each reference. Dead top-N identities release when replaced.

Eligible aggregation and DISTINCT consume input without retaining every input row. Join matches
can stream into projection, which stops at an unordered LIMIT; ON/WHERE still drain every
candidate, even at LIMIT 0. Eligible EXISTS/scalar bodies retain at most two result rows but
still evaluate every projection the ordinary body would reach. EXISTS stops reading the completed
result after one row; a scalar result's second row establishes its cardinality error.
Paths that preserve volatile evaluation or resolve deferred collations can still stage input.
Evaluation completes before delivery. Fitting results retain their existing materialization;
spilled results decode lazily within their reader grant.
Plan-sized traversal, current-row state, storage iterator scratch and transient planning are
outside it. INSERT SELECT does not spill and its source stays uncharged (#1828). DML subquery
sources keep their existing policy.

[CTAS](language/statements/create-table.md#create-table-as) charges like any SELECT, including
sorting, grouping and caches, then charges kept output and copy buffers. Pagination does not
reserve collection capacity. [Joined mutations](#joined-mutations) and recursive query state
retain their existing accounting. Refusal stops input immediately, even before a later input error,
unless an eligible output, sort, hash, window, set or join owner hands retained state to temporary storage.

[SQL/JSON paths](language/functions/json-path.md#limits) charge transient DOM, result references
and selected-result serialization to the same statement memory grant (CS05094 / 54000).
They also share a 1,000,000-visit work cap with window-frame recomputation.
If plain SELECT bytes fill the grant first, window or JSON memory refusal reports CS05067.
With no plain bytes live, window and JSON grants retain CS05079 and CS05094 respectively.

FULL JOIN always hashes the written right child, including duplicate and NULL-key rows, with
match flags charged to the same budget. There is no loop fallback: conditions without a safe
mandatory hash equality, or whose full ON evaluation is not proven non-throwing, fail CS05070
(no SQLSTATE) before input reads. EXPLAIN describes FullHashJoin with `unmatched=both`.

Use [EXPLAIN](language/statements/explain.md) to inspect costs without executing the wrapped data statement. EXPLAIN ANALYZE accepts SELECT query bodies and runs expressions/application functions normally; actual_rows reports measured candidates before residuals, not final output cardinality. Estimates distinguish exact probes, lower bounds and heuristics.

Optional cost probes that observe a committed table-definition change discard their estimates
and retain the bound access path, for SELECT and plain EXPLAIN. This race does not report
CS04006. Mutation execution still checks its target definition and can report CS04006
(no SQLSTATE); roll back and retry that mutation in a fresh transaction. Plain EXPLAIN
never executes its wrapped data statement. Analysis scopes cached view bodies by relation path, so separate references report
independent counts while repeated correlated calls to the same reference accumulate.

[SQL](index.md) · [Engine options](../../dotnet-apis/resources/database/assimalign-cohesion-database-sql/sql-database-engine-options.md)

## ORDER BY in index order

A selected [ordered index scan](language/clauses/order-by.md#index-order) removes sorting for
matching single-table ORDER BY and uses the same MVCC snapshot and full WHERE filter in either
direction. Non-DISTINCT LIMIT/OFFSET stops after enough qualifying rows; DISTINCT still reads
all candidates. Joins, grouping and windows retain their sort.

Costing credits avoided sorting and LIMIT/OFFSET work against random base-row fetches. It
estimates filter selectivity from bounded probes and residual-predicate heuristics, so a sparse
filter can favor a selective seek plus sorting over walking an ordering index. A broad unbounded
ordered scan can also lose to a table scan plus sorting. These estimates are transient, with no
persisted statistics. EXPLAIN identifies `OrderedIndexScan`, its direction and `sort_removed=true`.

## Recursive query memory

[WITH RECURSIVE](language/clauses/with.md#on-demand-evaluation-and-memory) reclaims single-pass consumed rows. Rescans and DISTINCT retain
history or membership; selected results still materialize before delivery. The default 64 MiB grant
holds roughly 500,000 single-BIGINT UNION ALL counter results by executor accounting; payloads and
other retained state change this capacity. COUNT(*) over the tested million-row counter instead
retains less than 32 KiB. Bounded-state infinite consumers require cancellation; retaining consumers
exhaust the grant with CS05067 (53200). There is no fixed row/depth limit or spill.

## Joined mutations

[UPDATE FROM](language/statements/update.md#from-sources) and
[DELETE USING](language/statements/delete.md#using-sources) use a join with the target pinned
outer. Safe target-only predicates can seek it; mandatory equality to a source can use an
index probe or hash build on that source. EXPLAIN shows the access paths, join algorithm and
`hash_budget=no_spill` detail. Hash builds count against StatementMemoryBudget; exhaustion is
CS05067 (53200) before writes. UPDATE FROM, DELETE USING and MERGE also charge retained
target identities, non-hash input buffers, derived copies, intermediate frames, associations,
selected values, encoded replacements and joined cascade state. Exhaustion refuses before any
write and releases reservations on every exit. Shared DML objects are charged once; INSERT SELECT
and DML subquery sources keep their existing policy. Transient expression values remain outside
the grant; plain SELECT result materialization now charges it.

Equality-driven steps follow written source order. If only b is connected to the target by
equality, list b first: `FROM b, a` or `USING b, a`. In
`UPDATE t SET v = b.v FROM a, b WHERE t.id = b.tid AND b.aid = a.id`, the engine visits the
t × a product before b's hash. Reversing the source list lets each equality drive its step.
Both orders are correct. Independent source graphs can use exact search through eight sources
and safe filter pushdown in their existing order from nine through 64; target-referencing
UPDATE/DELETE WHERE terms retain bound order.

Every [referential closure](language/constraints.md#referential-closure-memory), including plain ON DELETE CASCADE, charges
StatementMemoryBudget. About 840 bytes per two-column row limits a default 64 MiB closure to
about 79,900 rows, less when other statement work is retained. There is no cascade depth limit.
CS05067 (53200) refuses exhaustion before writes; cleanup releases abandoned attempts.

## Hash aggregation and DISTINCT spilling

Eligible root plain GROUP BY and streamed scan/system-view unordered SELECT DISTINCT spill at
their existing reservation boundary. Hash aggregation uses fixed-size states, eight partitions
per level and at most eight levels; exhausting that bound retains CS05067. Existing groups keep
accumulating while new-group inputs partition. A single skewed group's state must still fit.

| State or shape | Spill policy |
| --- | --- |
| Native COUNT; SUM/AVG with a proved numeric bound; fixed-width numeric, temporal, Boolean or UUID MIN/MAX; BOOL_AND/BOOL_OR/EVERY | Eligible; fixed state remains charged |
| Text/CHAR and binary MIN/MAX | CS05067 |
| SUM/AVG over stored integer columns, declared integer calls, sequences, bounded stored NUMERIC columns/casts or numeric constants | Can prove magnitude × physical statement temp quota fits Decimal |
| SUM/AVG over pseudo-type calls, derived/Null-typed columns, unbounded NUMERIC, floating sums or unproved compound inputs | CS05067 |
| STRING_AGG/ARRAY_AGG, aggregate DISTINCT/ORDER BY, statistical/application accumulators and grouping sets | Existing refusal/ownership; no hash spill permission |
| Deferred collation, second key family, unsupported cells or application-binary cells | CS05067 |
| Join-output hash DISTINCT | Existing memory refusal |
| Subquery-bearing group inputs/expressions or unproved MIN/MAX argument families | Existing refusal |

COUNT(x) partitions nullness only. The SUM/AVG proof prevents an overflow error from moving
later during replay. ARRAY_AGG remains unavailable (CS01016); the spill lane does not add it.
Keys, FILTER and accepted arguments evaluate once before evaluated values
are written. Results retain first-seen group order, the first complete DISTINCT representative,
collated spelling, calls, sequence state, counters and first expression errors. Statements that
fit keep their original charges. Completed groups pass through an ordinal sort before HAVING
and projection; unordered LIMIT/OFFSET stops projection at the fitting boundary.

For remaining grant G, a partition record including its 24 B allowance must fit
`min(16384, (min(G, Int32.MaxValue) - 32768) / 16 - 8192)`. Each phase subtracts active reader
buffers and must admit the external sorter. An input-order output spool uses the remaining-grant
bound without the 16,384 B partition cap. Scalar key plus COUNT(*) charges 352 B per live group;
8,000 groups exceed ten 262,144 B grants and complete with default temp quotas. This is a
measured example, not a row limit.

All source, partition, ordinal and downstream sort files share the physical statement quota.
Hash batches/passes vary with the process hash seed; a statement near the eight-level bound
can succeed in one process and refuse CS05067 in another. Keys require equality families and
the row codec, without requiring an index-backed collation.

## Hash join spilling

Eligible root INNER, LEFT, RIGHT and FULL hash joins spill to an indexed temporary build at
the original CS05067 boundary, including when other operators hold part of the statement grant.
Only plain column keys spill, including mixed numeric types: 1 matches 1.0. A computed key
such as CASE, COALESCE or arithmetic still refuses CS05067 when its hash build exceeds memory.
Correlated/lateral inputs, deferred collations, SQL/JSON, application-binary
build cells and joined DML retain their existing join-spill exclusions.
Join-output DISTINCT excludes hash-build spill; eligible nested-loop buffers can still spool.

The build sorts once by composed hash and insertion ordinal. Probes read only matching hash
chunks through a bounded cache and never write temp records. Equal-hash groups stream across
chunks, so skewed keys complete without a repartitioning/depth limit. FULL tracks matched
ordinals and restores unmatched build order, including NULL keys. Caller evaluation retains
rows, order, NULL extensions, candidate/record counts, calls and the first error. Fitting joins
retain their original representation, allocations and charges.

Spilled joins require at least 262,144 B of statement memory. There is one shared statement-wide
262,144 B allowance for bounded transient handoff/codec/file objects, shared with aggregation;
it is not multiplied per operator. Persistent directories, bitmaps, caches, readers and writers
are charged. Probe caches target one quarter of the grant, reduced by other reservations, and
must admit one decoded chunk. Records or minimum working sets that cannot fit retain CS05067.
Normalized join-tree left/intermediate buffers can stage under the exact sort row codec; an
application-produced binary probe cell still refuses CS05067 if its input needs staging.

At Seal, build runs and the indexed run coexist, requiring up to about twice the packed build
size plus framing, directory metadata and any staged probe input. With the default four-grant
temp quota, an incompressible build over about two grants refuses located CK05002. Consolidation
when an arena yields can also hit that quota. Random probes can read much more than the build
writes; physical snapshot/peak fields are not cumulative I/O measurements. No fixed ratio is
promised. CK05004 also checks the temporary join directory hash before probing.

File engines use their temporary space; in-memory engines require an explicit TempDirectory.
Joins, aggregation, staged buffers and sorting share one temp statement and physical quota.
Success, refusal, error, cancellation and result/session disposal release reservations/files;
startup sweeping handles leftovers after abrupt stops. CK0500x remains located and keeps the
durable database online. See [EXPLAIN hash fields](language/statements/explain.md#hash-spill-fields).

## Root sort spilling

Eligible retained SELECT output spills only where the previous build refused at its statement
grant. Fitting statements at the same grant and temp quota complete identically: rows, order,
CLR types/metadata, volatile calls, sequence values, first error and RecordsExamined. Statements
that still refuse retain their previous code. Spilling does not repeat expressions or storage visits.
File engines use configured temporary space; in-memory engines require an explicit TempDirectory.

| Owner | Spill behavior |
| --- | --- |
| Final-result spool, unordered CTAS source, primary/secondary ordered index scan | Drains evaluated output once to an encounter-order run; no sort is added |
| Independent nested SELECT and derived/view/CTE cache | Inherits outer permission; retains spilled output or drains a refusing cache copy once; reads decode in encounter order |
| Hash-aggregation output | Output can spool even when aggregate states fit; hash-state eligibility still applies |
| ORDER BY, bounded OFFSET + LIMIT and ordered DISTINCT sort | Stable ties, first sorted representative and pagination retained; ordered DISTINCT's seen set must fit |
| Partitioned window stage | Sorts saved input/keys, computes one charged partition at a time, then restores input ordinal order before the next group/final projection |
| UNION ALL | Appends converted operands in encounter order |
| UNION | Keeps the first occurrence under common-type comparison through bounded hash partitions and encounter ordinals |
| INTERSECT / EXCEPT, including ALL | Stages both converted inputs, partitions counts/representatives, then restores left order; ALL keeps the first min(m,n) left copies or skips the first n respectively |
| Nested-loop inner and normalized buffers | Spools pushed-filter rows once, then rereads for each outer row; no base-table rescan, repeated predicate or changed ON pair order |

Set operands finish in order; conversions precede comparison. NULL equality and collation-equal
representatives are unchanged. UNION/hash partitions use eight partitions per level and at most
eight levels. Mixed comparison families per column, capacity or repartition exhaustion retain
CS05067. A correlated consumer of an already spilled CTE can reread its run; this does not grant
spill permission to excluded correlated/lateral execution owners.

### Remaining refusals

Membership vectors (`NOT IN`, `IN` and quantified comparisons), recursive CTEs, a single window
partition larger than the grant, ordered DISTINCT's seen set, joined DML (#1828), SQL/JSON,
deferred collations and cells the temp codec cannot encode retain their refusals. Non-equi FULL
joins refuse CS05070 during planning. **INSERT SELECT does not spill and its source stays uncharged
(#1828).** CTAS is the table-writing statement whose source can spool.

The dialect also retains exclusions for correlated/deferred frames and lateral inputs, grouping
sets, ordered/opaque aggregate state, unproved hash expressions, ordered sequence deferral,
inferred SQL/JSON domain checks during CTAS assignment, unsupported key families and buffers
without a handoff. Computed hash-join keys retain their existing refusal. Application-produced
binary cells remain excluded; binary columns, parameters and built-in binary results are eligible.

Value-identity gates conservatively exclude catalogs with reference-bearing or pseudo-type
application/SQL function parameters, application casts or checked domains, even unused registrations.
Window eligibility is captured at planning and cannot change after concurrent DDL. Numeric
application functions/aggregates can remain eligible. Every spilled window group needs PARTITION BY
and fixed key collations; OVER () retains the base path. A partition still must fit the remaining
grant. Its first recorded refusal is CS05079 (54000), or CS05067 (53200) when plain SELECT bytes
filled the grant first. An unsuccessful window memory/quota/floor fallback preserves that code
and message, attaching its cause in exception data. No spilled-stage codec failure restarts evaluation.

### Record, workspace and quota limits

| Storage/workspace | Limit |
| --- | --- |
| Sequential output/cache and stage row | At most 16,384 encoded bytes, including an internal ordinal where present |
| Sequential output admission | For encoded L and decoded cost D, 9,984 + L + 24 + 2D bytes must fit the grant |
| Sequential writer/encoding scratch | Charged when possible; at most 26,392 fixed bytes beyond the grant while producer state drains |
| Sort record | Key + row + 16 framing bytes must fit (min(G, Int32.MaxValue) - 32,768) / 16 - 8,192; G at least 65,536 and bound at least 16 |
| Window sort arena | Remaining grant after active reader/fixed stage state, less 32,768 B construction allowance; record bound must admit a two-way merge |
| Window handoff scratch | At most 131,072 fixed bytes beyond the grant |
| Set/join stage handoff scratch | At most 65,536 fixed bytes beyond the grant |
| Nested-loop staged reader | 8,192 + L + 256 + 2D bytes charged for the buffer lifetime; each pass opens its own page and record destination |
| Relation/group-input cache reader | Workspace W + four decoded-row allowances + 64 bytes per column on the outer ledger |
| Scalar/EXISTS reader | Workspace plus two decoded-row allowances on the live outer ledger |
| CTAS copy after original admission refuses | Releases consumed source-cell payload charges; existing 64-row batch may use at most 520,472 B beyond the grant |

An unencodable row, an encoded row above its record limit or a first reader/decoded overlap
that cannot fit refuses CS05067 at every quota. Known retained and pending rows are validated
before the first temp write, so quota cannot hide their record refusal. CK05002 is a physical
statement-temp quota refusal where more quota can permit completion, not a replacement for a
record-size refusal. Newly permitted operator fallback that cannot finish preserves its original
CS05067/message with the temp cause in exception data; a pre-existing temp failure reached
before a new handoff retains its CK05 code.

The default physical statement quota is four grants (256 MiB at the default 64 MiB grant).
Ten grants of 20- or 50-character ASCII sort rows complete; CTAS completes ten grants of
20-character ASCII input. Twenty three-byte characters fit six grants but seven refuse CK05002:
payload capacity before rounding is 6.66 grants. Width, framing, run tails and intermediate
merges determine capacity; physical quota is not a fixed multiple of decoded rows.

CK05001–CK05006 are located statement/delivery errors; durable storage stays online and an
explicit transaction stays active. Cleanup cannot replace a statement error or completed wire
delivery. EXPLAIN ANALYZE adds spill bytes/runs on the operator that actually spilled, including
beneath sequence scopes, only after a spill. Plain EXPLAIN of a fitting statement is unchanged.

### Spilled result lifetime

Delivery is lazy after evaluation completes. A result supports repeated or concurrent
enumerations until disposed, session close or engine disposal; a later statement leaves it
valid. Dispose ExecuteAsync results even after enumeration completes or is canceled. A session
permits at most eight undisposed returned spilled client results; a ninth refuses CS05067 until
one is disposed. Relation caches, scalar/EXISTS bodies, CTAS sources and operator stage runs
are internal: they consume no client slot and close with the statement (or earlier stage exit).
All internal runs share the statement temp quota and are also registered for session/engine cleanup.

Active reader workspace and retained decoded rows, including one row of old/new decoding
overlap, share the result grant. A further reader exceeding it refuses CS05067 at its first
MoveNextAsync, naming live/requested bytes and leaving existing readers intact. Canceling one
reader ends only that reader. A real temp read failure closes the result and every reader sees
the same coded failure. Disposal invalidates readers and releases files exactly once.

A client canceling an exchange aborts its transport so blocked server delivery can unwind.
An uncanceled stalled delivery has no time bound; IdleTimeout applies at the next-frame read.
Temp quotas and MaxSessions bound retained files and accepted sessions until connection end
or shutdown.

## Window frame work

[Window stages](language/clauses/window.md) retain full partitions under the statement memory
grant. Eligible partitioned stages spill as described [above](#root-sort-spilling); a single
partition larger than the remaining grant still refuses CS05079 (or the original CS05067 when
plain bytes filled first). Safe native moving aggregates run in O(n) work using inverse
transitions or MIN/MAX queues, including exclusion. Application/order-sensitive aggregates
recompute under the shared 1,000,000-visit cap (CS05084 / 54000). Volatile or subquery inputs/FILTER
restart evaluation when the head moves; this fallback can also hit CS05084 for native aggregates.
ANALYZE reports frame_visits, transitions, inverse_transitions and boundary_visits. LIMIT cannot shorten this work.

## Sources

- **Retained outputs, windows and stage spooling** — SQL DESIGN and DIALECT at `a3daa99a0`; `SqlOutputSpool.cs`, `SqlWindowInputSpool.cs`, `SqlOperatorSpillRefusal.cs` and set/inner spill executors under the SQL project's `src/Internal/Execution/`; `SqlSetInnerSpillTests.cs` and `SqlSortSpillConformanceTests.ResultSpooling.cs`, `WindowSweeps.cs`, `Owners.cs` and `BatchIntegration.cs` in its tests (the latter three retain the SqlSortSpillConformanceTests prefix).

- **Hash spilling** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/docs/DESIGN.md`, `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md` and `SqlPlanExecutor.HashSpill.cs`, `SqlPlanExecutor.JoinSpill.cs`, `SqlHashJoinSpill.cs` under the SQL project's `src/Internal/Execution/` at `a3daa99a0`.
- **Root sort/CTAS spilling** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md` and `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlSortSpillConformanceTests*.cs`.

- **SELECT retention and memory** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlSelectRetentionTests.cs`, `SqlSelectMemoryTests.cs`, `SqlSelectWideMemoryTests.cs` and `SqlJointTypeMigrationTests.BatchIntegration.cs` in the same directory; implementation is `src/Internal/Execution/SqlPlanExecutor.Retention.cs` and `SqlSelectMemory.cs` in that project.
- **Large-join planning and memory admission** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Internal/Execution/SqlPlanExecutor.JoinSearch.Greedy.cs`, `SqlPlanExecutor.JoinSearch.cs` in the same directory, and `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlJoinSearchFixTests.cs`.
- **Planning work bounds and fallback** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlGreedyJoinSearchTests.cs` and `SqlLanguageConformanceTests.cs` in the same directory.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlDmlSourceReviewTests.cs`.

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Internal/Execution/SqlPlanExecutor.Costing.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Internal/Execution/SqlHashJoinTable.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlCostAndHashJoinTests.cs`.
