# Language (GQL)

GQL provides finite graph matching, scalar filtering, insertion, deletion, and catalog discovery.

> **Status:** Partial. Implements a bounded ISO GQL subset with documented Cohesion extensions.

This reference describes the executable Cohesion subset of ISO/IEC 39075:2024.
`INSERT` is the graph-insertion verb; `CREATE` is a compatibility extension and `SHOW` is a
Cohesion catalog extension. Every statement evaluates in the session's bound logical database.

## Reference

- **[Syntax conventions](syntax-conventions.md)** — diagrams, names, literals, and comments.
- **[Statements](statements/index.md)** — queries, mutations, and catalog introspection.
- **[Clauses](clauses/index.md)** — filtering and projection.
- **[Patterns](patterns.md)** — nodes, relationships, maps, directions, and execution bounds.
- **[Expressions](expressions/index.md)** — scalar operators and literals.
- **[Diagnostics](diagnostics.md)** — syntax, capability, planning, and execution errors.
- **[Unsupported features](unsupported.md)** — recognized constructs outside the profile.

## Supported-clause matrix

| Construct | Support | Scope |
|---|---|---|
| `MATCH` | Supported | Finite comma-separated node/relationship chains |
| `WHERE` | Supported | Scalar comparisons joined by `AND`; predicate parentheses |
| `RETURN` | Supported | Bound variables and one-level properties; optional aliases |
| `INSERT` | Supported | Literal node/path insertion, optionally after matching |
| `CREATE` | Supported | Insertion compatibility extension |
| `DELETE` | Supported | Bound variables; connected-node deletion is restricted |
| `DETACH DELETE` | Supported | Bound variables; incident relationships cascade |
| `SHOW` | Supported | Five database-scoped metadata subjects |
| Parameters and functions | Recognized, not supported (`COHDBL001`) | No executable function table |
| Variable-length paths | Recognized, not supported (`COHDBL001`) | Only explicit finite chains |

The keyword table is larger than the executable clause list. Recognition does not imply support.
The parser conformance corpus checks the subset's syntax and abstract syntax tree; it is not an
ISO certification suite.

## See also

- **[Graph](../index.md)** — engine, storage, and delivery status.

## Sources

- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/docs/DESIGN.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlQueryParserTests.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlLanguageProfile.cs`.

