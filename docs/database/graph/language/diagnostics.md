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
| `GQL0005` | Pattern length, comparison count, or expression nesting limit exceeded |
| `GQL0006` | Duplicate literal property key |
| `GQL0007` | `Graph catalog introspection is read-only.` |

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

Planner and data failures use stable code prefixes on `DatabaseException`.
Kernel aborts surface as `DatabaseTransactionAbortedException`; deadlocks retain their specialized
subtype. Schema ownership is enforced with `DatabaseObjectLockedException`, rather than a graph
ownership diagnostic code.

## Boundaries

| Limit | Value | Error |
|---|---|---|
| Relationships in one pattern | 64 | `GQL0005` |
| Predicate nesting | 128 levels | `GQL0005` |
| Scalar comparisons | 128 | `GQL0005` |
| Materialized intermediate matches | 100,000 per pattern | `COHDBG004` |
| Candidate examinations | 1,000,000 per statement | `COHDBG004` |

## See also

- **[Language (GQL)](index.md)** — reference hub.
- **[Unsupported features](unsupported.md)** — capability matrix.
- **[SHOW](statements/show.md)** — read-only catalog composition.

## Sources

- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/docs/DESIGN.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlQueryParserTests.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlCatalogParserTests.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/docs/DESIGN.md`.

