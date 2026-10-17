# RETURN

RETURN evaluates standalone expressions, completes a read query, or projects values after insertion.

> **Status:** Implemented.

## Syntax

```syntaxsql
[ MATCH path [ , ...n ] [ WHERE predicate ] ]
RETURN [ DISTINCT | ALL ] { * | expression [ AS alias ] [ , ...n ] }
```

For insertion followed by projection, see [INSERT](insert.md) and [CREATE](create.md).

## Supported clauses

| Construct | Support |
|---|---|
| `MATCH` with optional `WHERE` before `RETURN` | Supported |
| `RETURN` after insertion | Supported |
| Standalone RETURN | Supported; one row |
| `DISTINCT`, `ALL`, wildcard, computed expressions | Supported; star cannot mix with explicit items |
| `RETURN` after deletion | Recognized, not supported (`COHDBL001`) |
| Functions and aggregation | Recognized, not supported (`COHDBL001`) |

## Arguments

- **`expression`** — a scalar expression or bound node/relationship/path variable.
- **`alias`** — an optional result-column alias.

## Remarks

Without MATCH or insertion, RETURN evaluates once. Bound variables still require a pattern;
unbound names/parameters fail planning with COHDBG001. `RETURN *` without a named binding is
GQL0002. Duplicate final names fail with COHDBG015 before reads or mutations, including EXPLAIN.
Missing properties yield NULL, with no separate MISSING value. DISTINCT compares complete tuples
using exact numeric equality, ordinal strings, NULL grouping and entity/path identity; arithmetic
keeps its own promotion. See the [RETURN clause](../clauses/return.md) for naming, examples and
embedded/wire result limits.

## Examples

These statements need no stored graph elements:

```sql
RETURN 1 + 1 AS two;
RETURN DISTINCT 1 = 1.0 AS equal, NULL IS NULL AS absent;
```

Results are `(2)` and `(TRUE, TRUE)`, each a single row. DISTINCT preserves the single row.

## See also

- **[Statements](index.md)** — statement navigation.
- **[RETURN clause](../clauses/return.md)** — projection reference.
- **[MATCH](match.md)** — query bindings.

## Sources

- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/docs/DESIGN.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlQueryParserTests.cs`.
- **Execution** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphReturnTests.cs`.

