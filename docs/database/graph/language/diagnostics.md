# GQL diagnostics

GQL reports stable capability, syntax, planning, and execution diagnostics.

> **Status:** Implemented.

## Parser diagnostics

| Code | Meaning |
|---|---|
| `COHDBL001` | Recognized construct outside the executable profile |
| `GQL0001` | Empty statement |
| `GQL0002` | Malformed supported syntax or invalid statement composition |
| `GQL0003` | Unterminated string, quoted name, or block comment |
| `GQL0004` | Invalid or out-of-range numeric literal |
| `GQL0005` | 64-relationship pattern bound exceeded |
| `GQL0006` | Duplicate literal property key |
| `GQL0007` | `Graph catalog introspection is read-only.` |
| `GQL0008` | Cypher bare arrow after a pattern element conflicts with GQL comment syntax |
| `GQL0009` | Parsing thread has insufficient stack for nested expressions; engine reports COHDBG008 |

The parser returns diagnostics on `GqlQueryStatement` rather than throwing for malformed query
text. Diagnostic locations use zero-based absolute UTF-16 offsets, exclusive end positions, and
one-based line numbers.

Capability checks precede syntax parsing. Unsupported recognized clauses therefore retain their
capability diagnostic instead of producing a downstream syntax error. Quoted text, comments,
label names, and property keys do not trigger keyword capability checks.

## Planning and execution diagnostics

| Code | Meaning |
|---|---|
| `COHDBG001` | Invalid pattern, variable binding, or traversal specification |
| `COHDBG002` | Unknown label or relationship type |
| `COHDBG003` | Schema/data mismatch, restricted deletion, or invalid graph mutation |
| `COHDBG004` | Materialized path-match or candidate-expansion limit exceeded |
| `COHDBG005` | Session/database binding mismatch |
| `COHDBG006` | Storage failure translated at the engine boundary |
| `COHDBG007` | Explicit transaction is aborted; later statements/commit refused until rollback |
| `COHDBG008` | Statement too complex for available stack (SQLSTATE 54001) |
| `COHDBG009` | Element or indexed property exceeds the stored-record/key bound |
| `COHDBG010` | Warning: read names an unknown label; GQLSTATUS 01N50 |
| `COHDBG011` | Warning: read names an unknown relationship type; GQLSTATUS 01N51 |
| `COHDBG012` | Database offline after durable storage failure |
| `COHDBG013` | Scalar value error: non-Boolean condition, invalid arithmetic/concatenation, zero divisor, overflow or non-finite result |
| `COHDBG014` | Caller parameter keys collide after stripping one sigil |
| `COHDBG015` | Duplicate final RETURN column name under ordinal, case-sensitive comparison; planning rejects the second expression before reads/mutations, even on empty matches or under EXPLAIN; no SQLSTATE assigned |

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

## Boundaries

| Limit | Value | Error |
|---|---|---|
| Relationships in one pattern | 64 | `GQL0005` |
| Recursive expression depth | Available thread stack | `GQL0009` / `COHDBG008` |
| Materialized intermediate matches | 100,000 across matching clauses | `COHDBG004` |
| Candidate examinations | 1,000,000 per statement | `COHDBG004` |

## Negotiated wire diagnostics

Protocol 2.1 with diagnostics sends Warning/Error entries before Error or ResultComplete/PathsComplete.
Information is omitted. Bounds are 4,096 entries and 1 MiB; overflow retains a leading prefix plus
`COHDBP001` with retained/original/omitted counts. The original failure or success remains unchanged
and the session stays usable. Negotiated 2.0 retains baseline Error/result bytes without the frame.
Malformed UTF-8, diagnostics or ordering are protocol violations.

GraphPathResult.Diagnostics finalizes at EOF. Unknown-name reads can complete with warnings and
zero paths. A terminal rejection before path output retains the connection; cancellation or an
unfinished path stream discards it.

## See also

- **[Language (GQL)](index.md)** — reference hub.
- **[Unsupported features](unsupported.md)** — capability matrix.
- **[SHOW](statements/show.md)** — read-only catalog composition.

## Sources

- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/docs/DESIGN.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlQueryParserTests.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlCatalogParserTests.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/docs/DESIGN.md`.
