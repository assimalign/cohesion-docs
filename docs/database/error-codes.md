# Database error codes

A runtime diagnostic identity has two uppercase letters and five decimal digits. Branch on the
complete code, such as `CS05067`, rather than a prefix, numeric suffix, message or SQLSTATE.
Identities are append-only; retired identities remain reserved and `00000` is unassigned.

| Prefix | Owner | Authoritative per-model registry |
| --- | --- | --- |
| CK | Shared kernel and infrastructure | [Kernel](https://github.com/assimalign/cohesion/blob/1f654d2d2/resources/Database/Assimalign.Cohesion.Database/docs/ERROR-CODES.md#kernel-migration) |
| CS | SQL | [SQL](https://github.com/assimalign/cohesion/blob/1f654d2d2/resources/Database/Assimalign.Cohesion.Database/docs/ERROR-CODES.md#sql-migration) |
| CD | Documents | [Documents](https://github.com/assimalign/cohesion/blob/1f654d2d2/resources/Database/Assimalign.Cohesion.Database/docs/ERROR-CODES.md#documents-migration) |
| CB | Blob | [Blob](https://github.com/assimalign/cohesion/blob/1f654d2d2/resources/Database/Assimalign.Cohesion.Database/docs/ERROR-CODES.md#blob-migration) |
| CG | Graph | [Graph](https://github.com/assimalign/cohesion/blob/1f654d2d2/resources/Database/Assimalign.Cohesion.Database/docs/ERROR-CODES.md#graph-migration) |
| CV | KeyValuePair | [KeyValuePair](https://github.com/assimalign/cohesion/blob/1f654d2d2/resources/Database/Assimalign.Cohesion.Database/docs/ERROR-CODES.md#keyvaluepair-migration) |
| CC | Cache (reserved) | [Registry](https://github.com/assimalign/cohesion/blob/1f654d2d2/resources/Database/Assimalign.Cohesion.Database/docs/ERROR-CODES.md) |

The suffix allocates two category digits and three condition digits within these ranges.
Categories help organize the registry; callers still match the complete identity.

| Suffix range | Category |
| --- | --- |
| 00001–00100 | Language and syntax |
| 00101–00999 | Reserved |
| 01000–01999 | Binding and name resolution |
| 02000–02999 | Values, types and encoding |
| 03000–03999 | Schema, catalog and constraints |
| 04000–04999 | Transactions, locking and transaction state |
| 05000–05999 | Execution, capabilities and budgets |
| 06000–06999 | Storage, durability and format |
| 07000–07999 | Protocol, transport, authentication and sessions |
| 08000–08999 | Configuration and argument validation |
| 09000–09999 | Hosting, pooling and lifecycle |
| 10000–99999 | Reserved |

`Diagnostic.Code` carries the identity separately from its unprefixed `Message`. Legacy
message-only failures carry one `CODE: message` prefix; this includes the SQL declared-script
`CS03201`–`CS03205` refusals. Wire Error text also composes one prefix for legacy readers, while
structured diagnostic messages remain unprefixed. Do not parse a human-readable message when a
diagnostic carrier is available.

The migration preserves SQLSTATEs, numeric wire error kinds, frame layouts, protocol versions
and disk formats. Standard .NET argument guards retain their exception contracts. Build and SDK
diagnostics, including `COHDBSDK…`, `COHDBCLIENT001`, `COHRES…`, `COHAM001` and `COHSDK002`,
are independent analyzer identities and remain unchanged.

Use the model references for current conditions: [SQL](sql/language/diagnostics.md),
[Documents](document/language/diagnostics.md), [Graph](graph/language/diagnostics.md),
[Key-Value Pair](key-value-pair/diagnostics.md), [Blob](blob/operations.md) and [Cache](cache/index.md).
The source registry also lists [newly coded conditions](https://github.com/assimalign/cohesion/blob/1f654d2d2/resources/Database/Assimalign.Cohesion.Database/docs/ERROR-CODES.md#rules-audit-condition-additions).

The omitted SQL semicolon is informational `CS00009`; it does not refuse parsing or execution.
`CS00100` is retired and remains reserved. Recognized unsupported clauses use `CK00001`.
Hard SQL deadlocks carry `CS04065`, mapped to SQLSTATE `40P01` (`deadlock_detected`). These
SQLSTATE mappings are documented code mappings, not separate wire fields.

## Transaction and capability conditions

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CK04004 | Nested OQL BEGIN/START; faults the existing transaction | — |
| CD04104 | Document COMMIT/ROLLBACK without an explicit transaction | — |
| CD04105 | OQL collection/index DDL inside an explicit transaction | — |
| CD07002 | Text transaction control without the negotiated transaction-status capability | — |
| CK07002 | Malformed negotiated statement Ready: exactly I/T/E plus a retained-state 0/1 byte is required | — |

CD04101 retains the aborted-transaction condition and original failure until recovery.
CK00001 also covers nested GQL path modes/selectors, unselected unbounded WALK even with LIMIT,
and selector/inline WHERE execution. Selected paths support binding and EXPLAIN in this step.
Outer GQL modes and SQL UPDATE ... FROM / DELETE ... USING execute.
Optional SQL costing falls back on committed definition changes; mutation execution retains
CS04006 (no SQLSTATE). See [OQL transactions](document/language/statements/transactions.md),
[GQL modes](graph/language/patterns.md#path-modes) and [SQL performance](sql/performance.md).

## Joined SQL mutations

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CS05068 | UPDATE target has more than one source association; refused before SET evaluation or writes | 21000 (`cardinality_violation`) |
| CS05069 | Direct RIGHT, FULL or unaliased parenthesized joined-DML source; before reads/writes; derived SELECT containing RIGHT or FULL executes | — |
| CS05070 | FULL JOIN lacks a safe mandatory hash equality or proven non-throwing ON evaluation; before input reads | — |

DELETE USING deduplicates physical targets instead. A cardinality failure preserves earlier
explicit-transaction work. See [UPDATE FROM](sql/language/statements/update.md#from-sources).

## Quantified comparisons

Quantified comparisons allocate no new code. They reuse CS05013 for a subquery with other than
one column (no SQLSTATE), CS01015 for name binding (42703 for unknown columns), and CS02055 for
incompatible compared non-NULL values (42804). Repeated grouping keys that do not match still
use CS05017 for ungrouped references. See [ANY/SOME/ALL](sql/language/expressions/quantified-comparisons.md).

## SQL TRUNCATE

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CS03067 | TRUNCATE RESTRICT omits a required referencing table, even an empty one | 0A000 (`feature_not_supported`) |
| CS04066 | TRUNCATE in an explicit transaction; prior work and active state remain usable | 25001 (`active_sql_transaction`) |
| CS04064 | Concurrent TRUNCATE closure/identity change requires retry | 40001 (`serialization_failure`) |

See [TRUNCATE](sql/language/statements/truncate.md) for recursive CASCADE, identity policies and
snapshot retention. Retired-source DML keeps CS04006 (no SQLSTATE); read-only old snapshots continue.

## SQL ALTER COLUMN TYPE

[TYPE with USING](sql/language/statements/alter-table.md#alter-column-type) adds CS02064 for a
missing assignment cast on an old value, USING result or dependent default (42804,
datatype_mismatch). Full-definition default/implicit collation changes retain CS05013 (no SQLSTATE).
Identity, view, CHECK/domain, FK, index, lock and conversion failures retain their existing codes;
see [SQL diagnostics](sql/language/diagnostics.md#alter-column-type-conditions).

## SQL MERGE

[MERGE](sql/language/statements/merge.md) adds CS01053 for merge_action() outside MERGE RETURNING
and CS05071 for a WHEN following an unconditional clause of the same match kind. Neither has an
assigned SQLSTATE. FULL associations reuse CS05070 for missing safe hash equality or ON safety
proof (no SQLSTATE). Execution reuses CS05036 (21000) for repeated physical target modifications,
including after rechecks, and CS05067 (53200) for retained joined-DML state exhaustion before writes.
Direct/referential overlaps compose without CS05036. CS04064 (40001) covers target recheck/apply-time
modified-row version conflicts, including stale Snapshot targets; shared cascade/parent checks and
host-ended transactions retain their ordinary uncoded aborts. Direct uniqueness (CS03037, 23505)
precedes final FK (CS03039, 23503) and referential SET-action constraint failures. Plain EXPLAIN
remains nonexecuting; SQL storage stays at format 30.

## SQL CREATE TABLE AS

[CTAS](sql/language/statements/create-table.md#create-table-as) adds CS03068 for missing, blank,
duplicate or wrong-width output names (42601, syntax_error). Existing CS03049 covers a destination
collision (42P07); IF NOT EXISTS instead skips. Explicit transactions use CS04003 (25001), and
retained source/copy memory exhaustion uses CS05067 (53200). SQL format 30 is unchanged.

## SQL table rewrite

[RewriteTableAsync](../dotnet-apis/resources/database/assimalign-cohesion-database-sql/sql-database.md)
reuses CS04006 (no SQLSTATE) for DML needing a retired layout, including stale foreign-key probes;
retry the whole transaction. Its exclusive source-table lock is acquired before queueing and uses
engine LockTimeout: expiry retains CS04044 / 55P03. Missing tables retain CS01051 / 42P01 and
changed descriptions CS04064 / 40001. SQL format 30 is unchanged.

## Document array indexes

The typed [array-key API](../dotnet-apis/resources/database/assimalign-cohesion-database-documents/document-index-key.md)
adds CD03025 for multiple array keys, UNIQUE array indexes or IncludeMissing with a leading array
key, and CD03026 for an encoded index definition above 8,092 bytes (before posting extraction or
tree creation). Neither has an assigned SQLSTATE. Unsupported typed WHEN constants/tree sizes
retain ArgumentException. Index v3 is required; v1/v2 open refusals retain CD06105 (no SQLSTATE).

OQL array DDL adds located CD01028 (projection/WHEN path outside its variable), CD02027
(unsupported WHEN scalar constant), CD03027 (excluded source/projection/WHEN expression) and
CD03028 (WHEN above 32 levels/128 nodes), before catalog writes. None has an assigned SQLSTATE.
See [CREATE INDEX](document/language/statements/create-index.md).

## Document comprehensions

OQL [ARRAY/FIRST comprehensions](document/language/expressions/comprehensions.md) add CD01027 for
an empty variable in a directly constructed AST (no SQLSTATE). Aggregate or metadata misuse
retains CD05001; retired ARRAY()/FIRST() calls retain CK00001.

## Document aggregate quantifiers and memory

OQL [aggregates](document/language/functions/aggregate-functions.md) add CD00011 for parsed
ALL/DISTINCT on a scalar call or star, and CD01029 for an invalid enum quantifier, scalar
quantifier or quantified star in a directly constructed call AST. Neither has an assigned SQLSTATE.
Final statistic overflow retains CD02007 (22003). Retained SELECT sources, group staging, native
accumulators, DISTINCT sets, ARRAY_AGG inputs/results, projection/order storage and RETURNING
share StatementMemoryBudget with ordered candidate buckets/keys/identities, GROUP AS wrappers/arrays
and DML pending buffers. CD05018 refuses
exhaustion before result publication; pending-buffer exhaustion precedes any write (no SQLSTATE).

OQL [GROUP AS and group aliases](document/language/clauses/group-by.md) add CD00012 for GROUP AS
without GROUP BY, CD01030 for invalid/colliding declarations or direct-AST alias-list shape, and
CD01031 for a group array used in an aggregate row input. None has an assigned SQLSTATE.

## Document RETURNING

OQL [RETURNING](document/language/clauses/returning.md) adds CD01026 (empty direct-AST output name),
CD05018 (retained-result budget exceeded) and CD05019 (serialized depth above 256 including a named
wrapper). None has an assigned SQLSTATE. The existing CD05016 enforces the 16 MiB frame cap inside
the write transaction. See [Document diagnostics](document/language/diagnostics.md#engine-codes).

## Graph shortest paths

[GQL selectors](graph/language/path-selectors.md) add CG00011 for a missing/nonpositive/nonliteral
Int64 SHORTEST count and CG01016 for a search prefilter referencing a peer-path-only variable
in the same MATCH, with or without a selector. Neither has an assigned SQLSTATE. Counted ANY
retains CK00001. Selector execution uses CG05004 for work or unresolved 64-hop searches and
CG05020 for retained history/state/output; no partial result escapes. Neither has a SQLSTATE.

Typed shortest-path reconstruction adds CG05022 when a selected node or relationship is absent
while the operation remains active (no SQLSTATE). Session/transaction termination keeps its
lifecycle refusal. See [Graph diagnostics](graph/language/diagnostics.md#typed-shortest-search).

## Graph MERGE conditions

| Code | Registry meaning | SQLSTATE |
| --- | --- | --- |
| CG03004 | NULL, non-finite or non-scalar MERGE equality, proven or evaluated | — |
| CG03005 | Bound node is NULL, absent or incompatible with its node pattern | — |
| CG03006 | Labels/properties on a repeated or bound MERGE node | — |
| CG03007 | Relationship variable declared before MERGE | — |
| CK04002 | MERGE statement/session ended while the database is live | — |
| CG04024 | Retryable stale Snapshot MERGE pattern decision | — |
| CG05023 | Invalid bound MERGE pattern lock identity | — |

[MERGE](graph/language/statements/merge.md) executes whole-pattern match/create and ordered SET
branches. Snapshot reports CG04024; ReadCommitted re-matches. Bare list/map equalities retain
parser CK00001; other proven/evaluated invalid equalities use CG03004. None of these conditions has
an assigned SQLSTATE. See [Graph diagnostics](graph/language/diagnostics.md#planning-and-execution-diagnostics).

## Graph format requirement

Graph record and tree-registration format 5 is required. `CG06017` refuses earlier formats,
including format 4, at open before logical recovery scrub. Recreate the database; there is no
in-place upgrade or assigned SQLSTATE. See [Graph format diagnostics](graph/language/diagnostics.md#planning-and-execution-diagnostics).

## Developer function conditions

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CG05021 | Graph callback/result/accumulator contract failure | — |
| CD05017 | Document callback/result/accumulator contract failure | — |
| CV00019 | CALL lacks its function name | — |
| CV01018 | Unknown Key-Value function | — |
| CV01019 | No Key-Value overload for the argument count | — |
| CV01020 | No Key-Value overload for the exact non-NULL types | — |
| CV01021 | Multiple NULL-compatible Key-Value overloads | — |
| CV02016 | Key-Value result violates the declared primitive type | — |
| CV05003 | Key-Value callback failed | — |
| CK07003 | Same-session callback reentry refused before nested execution | — |

External callback I/O/timeouts retain the callback code and cause. Existing DatabaseException,
out-of-memory, insufficient stack and actual statement cancellation retain their identity.
See [Graph](graph/language/diagnostics.md#application-function-conditions),
[Documents](document/language/diagnostics.md#application-function-conditions) and
[Key-Value](key-value-pair/diagnostics.md#registered-call-functions).

## SQL WITH RECURSIVE

[WITH RECURSIVE](sql/language/clauses/with.md) binds all list names in dependency order. The non-recursive
term fixes output width, types, modifiers, domains and collations; use the cast or COLLATE hint
to preserve that contract. Recursive definitions and the consuming statement bind, then planning,
EXPLAIN and execution refuse with CK00001, even for unused recursion. Acyclic lists execute.
Mutual recursion, recursive UNION ORDER BY/LIMIT/FETCH/OFFSET, SELECT locking tails and WITH
RECURSIVE ... MERGE remain CK00001. Plain WITH ... MERGE executes. No format changes apply.

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CS01054 | Recursive definition lacks top-level anchor UNION [ALL] recursive term | 42P19 |
| CS01055 | Self-reference in the non-recursive term | 42P19 |
| CS01056 | Self-reference inside a scalar, IN, quantified or EXISTS subquery | 42P19 |
| CS01057 | Self-reference on the nullable side of an outer join | 42P19 |
| CS01058 | Self-reference under INTERSECT ALL | 42P19 |
| CS01059 | Self-reference on either side of EXCEPT ALL or the right of EXCEPT DISTINCT | 42P19 |
| CS01060 | Recursive term does not have exactly one self-reference | 42P19 |
| CS01061 | Aggregate at the query level containing the self-reference | 42P19 |
| CS02065 | Anchor, recursive output or declared column-list width mismatch | 42601 |
| CS02066 | UNION changes the anchor type, domain or modifiers; cast hint | 42804 |
| CS02067 | UNION changes or conflicts with anchor collation; COLLATE hint | 42P21 |

## OQL null placement

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CD00013 | NULLS without FIRST or LAST | — |
| CD01032 | Invalid null-placement enum in a direct ordering AST | — |

[Ordering eligibility](document/language/statements/explain.md#index-ordering-eligibility) reports
plan reasons, consumption and sort elision/retention; it adds no diagnostic or storage format.

## GQL list operations

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CG00012 | Both slice bounds omitted; planning refusal, including EXPLAIN | — |
| CG01017 | Missing/empty iterator declaration in a direct comprehension AST | — |
| CG02014 | Invalid list source or slice-bound kind/range | — |
| CG05024 | Aggregate inside a comprehension predicate or projection | — |
| CG01018 | Missing/empty reduce/list-predicate declaration or identical reduce names | — |
| CG02015 | Reduce/list-predicate source is neither list nor NULL; known kinds fail planning | — |
| CG05025 | Malformed reduce/list-predicate body/quantifier AST or body aggregate | — |
| CG05026 | Reduce shape/type planning exceeds 65,536 node visits or closure steps | — |

See [GQL list comprehensions](graph/language/expressions/list-comprehensions.md) and
[slices](graph/language/expressions/list-slices.md), [reduce](graph/language/expressions/reduce.md)
and [list predicates](graph/language/expressions/list-predicates.md). Known non-Boolean predicates
use CG02013, including empty/NULL inputs and EXPLAIN. [Pattern comprehensions](graph/language/expressions/pattern-comprehensions.md) execute.

## SQL sequence catalog conditions

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CS02002 | Extended to identity or noncycling standalone sequence exhaustion | 22003 |
| CS03069 | Invalid resolved sequence options or restart/current value | 22023 |
| CS03070 | Occupied sequence/table/view relation name involving a sequence | 42P07 |
| CS03071 | Sequence namespace absent | 3F000 |
| CS03072 | Sequence definition exceeds the catalog record limit | 54000 |
| CS03073 | Named sequence or object identity is absent | 42P01 |
| CS03074 | Unsupported persisted sequence expression or sequence DDL in a provisioning script; constant column allocation defaults execute | 0A000 |
| CS03075 | setval value outside sequence bounds | 22003 |
| CS03076 | Unsupported sequence evaluation context, including NEXT VALUE FOR in RETURNING, or detached evaluation | 0A000 |
| CS03077 | Malformed sequence reference or invalid runtime call argument | 22023 |
| CS04067 | Dropped/replaced sequence incarnation | 40001 |
| CS04068 | currval is undefined for this sequence in this session | 55000 |
| CS04069 | lastval is undefined or its sequence was dropped | 55000 |
| CS04070 | DISCARD ALL inside an explicit transaction | 25001 |
| CS06061 | Malformed sequence definition, directory or reservation | XX001 |

These cover [sequence SQL](sql/language/statements/create-sequence.md),
[session functions](sql/language/functions/sequences.md) and [DISCARD ALL](sql/language/statements/discard-all.md),
as well as internal [catalog/allocation conditions](../dotnet-apis/resources/database/assimalign-cohesion-database-sql/catalog-design.md#standalone-sequences).

## SQL aggregate FILTER

[Grouping sets and masks](sql/language/clauses/group-by.md) add:

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CS00010 | GROUPING SETS inside ROLLUP/CUBE | 42601 |
| CS01062 | GROUPING argument does not match its associated level's grouping expressions | 42803 |
| CS01063 | GROUPING requires 1–31 non-star arguments | 54023 |
| CS05074 | GROUPING outside grouped output or in grouping/aggregate inputs | 42803 |
| CS05075 | More than 4096 sets before DISTINCT | 54001 |
| CS05067 | Retained per-set aggregate/DISTINCT/order state or mask plan exhausts the statement budget | 53200 |

Aggregate FILTER retains:

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CS05072 | FILTER on a scalar/non-aggregate call | 42809 |
| CS05073 | Same-scope aggregate inside FILTER | 42803 |
| CS02003 | Non-Boolean FILTER condition | 42804 |

See [FILTER evaluation and placement](sql/language/clauses/filter.md).

## OQL expression subqueries

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CD00014 | Direct IN subquery requires SELECT RAW or SELECT VALUE | — |
| CD00015 | Derived FROM SELECT has no alias | — |
| CD01033 | FROM expression has no explicit alias or implicit field name | — |
| CD01034 | Unbound source variable without an input-row scope | — |

[Subqueries](document/language/expressions/subqueries.md) share the existing CD05018 memory budget
and CD05015 projection-depth guard. Correlation and DML subqueries use CK00001; derived and expression FROM sources execute.

## Correlated GQL EXISTS

These codes have no assigned SQLSTATE. See [EXISTS](graph/language/expressions/exists.md).

| Code | Meaning and trigger |
| --- | --- |
| CG00013 | EXISTS lacks a nonempty braced pattern |
| CG01019 | Proven incompatible correlated pattern kind |
| CG02016 | Incompatible runtime correlated entity kind |
| CG05027 | Malformed/missing correlated subplan or missing argument |

## SQL window functions

See [OVER and WINDOW](sql/language/clauses/window.md) for placement, inheritance and memory rules.

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CS00011 | Window argument count or call modifier | 42601 |
| CS01064 | Duplicate WINDOW name | 42P20 |
| CS01065 | Unknown/self/forward window reference | 42704 |
| CS01066 | Copied PARTITION BY or overridden ORDER BY | 42P20 |
| CS02068 | Non-integer offset or bucket count | 42804 |
| CS02069 | Incompatible navigation value/default types | 42804 |
| CS05076 | Window placement or missing OVER | 42P20 |
| CS05077 | Nested window | 42P20 |
| CS05078 | Nonpositive NTILE count | 22014 |
| CS05079 | Window state exceeds statement memory grant | 54000 |

## Identity integration

[Identity columns](sql/language/clauses/identity.md) use owned sequences and early-bound column defaults.

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CS03078 | ADD GENERATED requires NOT NULL | 55000 |
| CS03079 | Column has no identity for the ALTER operation | 55000 |
| CS03080 | Sequence required by identity or column default | 2BP01 |
| CS03081 | Invalid OWNED BY target or identity ownership change | 55000 |

Allocator exhaustion retains CS02002 (22003). A disappearing identity incarnation or an unplanned
live referential sequence-default dependency uses retryable CS04067 (40001); retry to replan.
Backfill memory exhaustion uses CS05067 (53200) before publication.

## Pattern comprehensions

[Pattern comprehensions](graph/language/expressions/pattern-comprehensions.md) collect every correlated inner match in order.
The optional path name shadows locally; other existing pattern names correlate. NULL arguments
produce no matches, duplicates and NULL projections remain, and zero-hop relationship quantifiers
are accepted. Inner aggregates are refused. EXPLAIN shows CollectApply. Collection shares the
statement operation, snapshot, update overlay and work/memory budgets (CG05004/CG05020).
Scalar composite results use graph.composite-values; entity lists remain CG05001 until #1662.

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CG00014 | Pattern comprehension requires a nonempty path and projection | — |
| CG05028 | Malformed pattern comprehension, aggregate projection or missing collection plan | — |

## Foreign-key update declarations

ON UPDATE metadata adds no diagnostic identity. Changing a referenced key under CASCADE,
SET NULL or SET DEFAULT uses structured CK00001 before row writes, including case-only changes.
Duplicate/incomplete action syntax retains CS00003. Invalid catalog actions use CS06059,
unsupported table extensions CS06060, and earlier catalog formats CS06041. Compiled v8 artifacts
are refused with InvalidDocument at schema.format. See [constraints](sql/language/constraints.md#on-update-declarations).

## Sources

- **Batch 11** — `cohesion/resources/Database/Assimalign.Cohesion.Database/docs/ERROR-CODES.md` at `ae48b04cb`, checked against the executable tests.

- **Batch 10** — `cohesion/resources/Database/Assimalign.Cohesion.Database/docs/ERROR-CODES.md` at `1f654d2d2`; sequence sorting accounting follows executor code and `SqlSequenceFifthPassTests.Accounting.cs`.
- **Batch 9** — `cohesion/resources/Database/Assimalign.Cohesion.Database/docs/ERROR-CODES.md` at `46803a9ba`.
