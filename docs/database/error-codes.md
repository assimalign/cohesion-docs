# Database error codes

A runtime diagnostic identity has two uppercase letters and five decimal digits. Branch on the
complete code, such as `CS05067`, rather than a prefix, numeric suffix, message or SQLSTATE.
Identities are append-only; retired identities remain reserved and `00000` is unassigned.

| Prefix | Owner | Authoritative per-model registry |
| --- | --- | --- |
| CK | Shared kernel and infrastructure | [Kernel](https://github.com/assimalign/cohesion/blob/ed01f80b1/resources/Database/Assimalign.Cohesion.Database/docs/ERROR-CODES.md#kernel-migration) |
| CS | SQL | [SQL](https://github.com/assimalign/cohesion/blob/ed01f80b1/resources/Database/Assimalign.Cohesion.Database/docs/ERROR-CODES.md#sql-migration) |
| CD | Documents | [Documents](https://github.com/assimalign/cohesion/blob/ed01f80b1/resources/Database/Assimalign.Cohesion.Database/docs/ERROR-CODES.md#documents-migration) |
| CB | Blob | [Blob](https://github.com/assimalign/cohesion/blob/ed01f80b1/resources/Database/Assimalign.Cohesion.Database/docs/ERROR-CODES.md#blob-migration) |
| CG | Graph | [Graph](https://github.com/assimalign/cohesion/blob/ed01f80b1/resources/Database/Assimalign.Cohesion.Database/docs/ERROR-CODES.md#graph-migration) |
| CV | KeyValuePair | [KeyValuePair](https://github.com/assimalign/cohesion/blob/ed01f80b1/resources/Database/Assimalign.Cohesion.Database/docs/ERROR-CODES.md#keyvaluepair-migration) |
| CC | Cache (reserved) | [Registry](https://github.com/assimalign/cohesion/blob/ed01f80b1/resources/Database/Assimalign.Cohesion.Database/docs/ERROR-CODES.md) |

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
The source registry also lists [newly coded conditions](https://github.com/assimalign/cohesion/blob/ed01f80b1/resources/Database/Assimalign.Cohesion.Database/docs/ERROR-CODES.md#rules-audit-condition-additions).

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
CK00001 also covers nested GQL path modes, unbounded WALK even with LIMIT and path selectors.
Outer GQL modes and SQL UPDATE ... FROM / DELETE ... USING execute.
Optional SQL costing falls back on committed definition changes; mutation execution retains
CS04006 (no SQLSTATE). See [OQL transactions](document/language/statements/transactions.md),
[GQL modes](graph/language/patterns.md#path-modes) and [SQL performance](sql/performance.md).

## Joined SQL mutations

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CS05068 | UPDATE target has more than one source association; refused before SET evaluation or writes | 21000 (`cardinality_violation`) |

DELETE USING deduplicates physical targets instead. A cardinality failure preserves earlier
explicit-transaction work. See [UPDATE FROM](sql/language/statements/update.md#from-sources).

## Quantified comparisons

Quantified comparisons allocate no new code. They reuse CS05013 for a subquery with other than
one column (no SQLSTATE), CS01015 for name binding (42703 for unknown columns), and CS02055 for
incompatible compared non-NULL values (42804). Repeated grouping keys that do not match still
use CS05017 for ungrouped references. See [ANY/SOME/ALL](sql/language/expressions/quantified-comparisons.md).

## Graph shortest paths

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

- `cohesion/resources/Database/Assimalign.Cohesion.Database/docs/ERROR-CODES.md` at `ed01f80b1`.
