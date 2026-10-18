# Language (GQL)

GQL provides finite graph matching, scalar filtering, insertion, deletion, and catalog discovery.

> **Status:** Partial. Implements a bounded ISO GQL subset with documented Cohesion extensions.

This reference describes the executable Cohesion subset of ISO/IEC 39075:2024.
`INSERT` is the graph-insertion verb; `CREATE` is a compatibility extension and `SHOW` is a
Cohesion catalog extension. Every statement evaluates in the session's bound logical database.

## Reference

- **[Syntax conventions](syntax-conventions.md)** — diagrams, names, literals, and comments.
- **[Statements](statements/index.md)** — queries, mutations, and catalog introspection.
- **[Clauses](clauses/index.md)** — filtering, projection, ordering and paging.
- **[Patterns](patterns.md)** — nodes, relationships, maps, directions, and execution bounds.
- **[Expressions](expressions/index.md)** — operators, CASE/COALESCE/NULLIF, lists, maps and property access.
- **[Diagnostics](diagnostics.md)** — syntax, capability, planning, and execution errors.
- **[Unsupported features](unsupported.md)** — recognized constructs outside the profile.

## Supported-clause matrix

| Construct | Support | Scope |
|---|---|---|
| [`MATCH`, `OPTIONAL MATCH`](statements/match.md) | Supported | Finite chains; repeated clauses join shared variables and optional clauses NULL-extend unmatched rows |
| `WHERE` | Supported | Boolean/arithmetic expressions, NULL/truth tests, IN and string predicates |
| `RETURN` | Supported | Standalone or after MATCH/insertion; expressions, aliases, ALL/DISTINCT and named-binding star |
| [ORDER BY](clauses/order-by.md) | Supported | Expression keys, ASC/ASCENDING/DESC/DESCENDING, NULLS FIRST/LAST and stable ties |
| [OFFSET/SKIP and LIMIT](clauses/limit-offset.md) | Supported | Output paging after RETURN; offset precedes limit |
| `INSERT` | Supported | Node/path insertion with literal or parameter properties, optionally after matching |
| `CREATE` | Supported | Insertion compatibility extension |
| `DELETE` | Supported | Bound variables; connected-node deletion is restricted |
| `DETACH DELETE` | Supported | Bound variables; incident relationships cascade |
| `SHOW` | Supported | Five database-scoped metadata subjects |
| Parameters | Supported | Scalar/list/map values in expressions; stored pattern properties stay scalar-only |
| [CASE](expressions/case.md), [COALESCE](expressions/coalesce.md), [NULLIF](expressions/nullif.md) | Supported | Lazy conditional values; simple CASE accepts ISO when-operand lists |
| [Lists](expressions/list-values.md), [maps](expressions/map-values.md) and [access](expressions/property-access.md) | Supported | Transient nested values, with negotiated graph.composite-values over protocol 2.1 |
| EXPLAIN | Supported | Chosen data plan without execution |
| Functions | Recognized, not supported (`COHDBL001`) | No executable function table |
| Variable-length paths | Recognized, not supported (`COHDBL001`) | Only explicit finite chains |

The keyword table is larger than the executable clause list. Recognition does not imply support.
The parser conformance corpus checks the subset's syntax and abstract syntax tree; it is not an
ISO certification suite.

[EXPLAIN](statements/explain.md) describes a chosen plan. [Parameters](expressions/parameters.md)
share the SQL/OQL binding contract.

## See also

- **[Graph](../index.md)** — engine, storage, and delivery status.

## Sources

- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/docs/DESIGN.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlQueryParserTests.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlLanguageProfile.cs`.
