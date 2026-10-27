# Database error codes

A runtime diagnostic identity has two uppercase letters and five decimal digits. Branch on the
complete code, such as `CS05067`, rather than a prefix, numeric suffix, message or SQLSTATE.
Identities are append-only; retired identities remain reserved and `00000` is unassigned.

| Prefix | Owner | Authoritative per-model registry |
| --- | --- | --- |
| CK | Shared kernel and infrastructure | [Kernel](https://github.com/assimalign/cohesion/blob/5e6f1c7a0/resources/Database/Assimalign.Cohesion.Database/docs/ERROR-CODES.md#kernel-migration) |
| CS | SQL | [SQL](https://github.com/assimalign/cohesion/blob/5e6f1c7a0/resources/Database/Assimalign.Cohesion.Database/docs/ERROR-CODES.md#sql-migration) |
| CD | Documents | [Documents](https://github.com/assimalign/cohesion/blob/5e6f1c7a0/resources/Database/Assimalign.Cohesion.Database/docs/ERROR-CODES.md#documents-migration) |
| CB | Blob | [Blob](https://github.com/assimalign/cohesion/blob/5e6f1c7a0/resources/Database/Assimalign.Cohesion.Database/docs/ERROR-CODES.md#blob-migration) |
| CG | Graph | [Graph](https://github.com/assimalign/cohesion/blob/5e6f1c7a0/resources/Database/Assimalign.Cohesion.Database/docs/ERROR-CODES.md#graph-migration) |
| CV | KeyValuePair | [KeyValuePair](https://github.com/assimalign/cohesion/blob/5e6f1c7a0/resources/Database/Assimalign.Cohesion.Database/docs/ERROR-CODES.md#keyvaluepair-migration) |
| CC | Cache (reserved) | [Registry](https://github.com/assimalign/cohesion/blob/5e6f1c7a0/resources/Database/Assimalign.Cohesion.Database/docs/ERROR-CODES.md) |

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
The source registry also lists [newly coded conditions](https://github.com/assimalign/cohesion/blob/5e6f1c7a0/resources/Database/Assimalign.Cohesion.Database/docs/ERROR-CODES.md#rules-audit-condition-additions).

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
proof (no SQLSTATE). Execution reuses CS05036 (21000) for repeated physical target modifications
or overlapping direct UPDATE/referential writes, and CS05067 (53200) for retained joined-DML state
exhaustion before writes. Plain EXPLAIN remains nonexecuting.

## SQL CREATE TABLE AS

[CTAS](sql/language/statements/create-table.md#create-table-as) adds CS03068 for missing, blank,
duplicate or wrong-width output names (42601, syntax_error). Existing CS03049 covers a destination
collision (42P07); IF NOT EXISTS instead skips. Explicit transactions use CS04003 (25001), and
retained source/copy memory exhaustion uses CS05067 (53200). SQL format 27 is unchanged.

## SQL table rewrite

[RewriteTableAsync](../dotnet-apis/resources/database/assimalign-cohesion-database-sql/sql-database.md)
reuses CS04006 (no SQLSTATE) for DML needing a retired layout, including stale foreign-key probes;
retry the whole transaction. Its exclusive source-table lock is acquired before queueing and uses
engine LockTimeout: expiry retains CS04044 / 55P03. Missing tables retain CS01051 / 42P01 and
changed descriptions CS04064 / 40001. SQL format 27 is unchanged.

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
share StatementMemoryBudget; CD05018 refuses exhaustion before result publication (no SQLSTATE).

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

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database/docs/ERROR-CODES.md` at `5e6f1c7a0`.
