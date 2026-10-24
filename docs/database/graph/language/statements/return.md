# RETURN

RETURN evaluates standalone expressions, completes a read query, or projects values after insertion.

> **Status:** Implemented.

## Syntax

```syntaxsql
[ MATCH path [ , ...n ] [ WHERE predicate ] ]
RETURN [ DISTINCT | ALL ] { * | expression [ AS alias ] [ , ...n ] }
[ GROUP BY { grouping_name_or_expression [ , ...n ] | () } ]
[ ORDER BY sort_item [ , ...n ] ]
[ { OFFSET | SKIP } count ] [ LIMIT count ]
```

For insertion followed by projection, see [INSERT](insert.md) and [CREATE](create.md).

## Supported clauses

| Construct | Support |
|---|---|
| `MATCH` with optional `WHERE` before `RETURN` | Supported |
| `RETURN` after insertion | Supported |
| Standalone RETURN | Supported; one row |
| `DISTINCT`, `ALL`, wildcard, computed expressions | Supported; star cannot mix with explicit items |
| `RETURN` after deletion | Recognized, not supported (`CK00001`) |
| [Aggregates](../functions/aggregate-functions.md) and [GROUP BY](../clauses/group-by.md) | Supported; implicit keys, ISO RETURN names/expressions and GROUP BY () |

## Arguments

- **`expression`** — a [value expression](../expressions/index.md), including CASE/COALESCE/NULLIF, lists/maps and access,
  or a bound node/relationship/path variable.
- **`alias`** — an optional result-column alias.

## Remarks

Without incoming stages, RETURN evaluates once. LET/FOR can also supply value variables;
unbound names/parameters fail planning with CG05001. `RETURN *` without a named binding is
CG00002. Duplicate final names fail with CG01015 before reads or mutations, including EXPLAIN.
Missing properties yield NULL, with no separate MISSING value. DISTINCT compares complete tuples
using exact numeric equality, ordinal strings, NULL grouping and entity/path identity; arithmetic
keeps its own promotion. See the [RETURN clause](../clauses/return.md) for naming, examples and
embedded/wire result limits. [ORDER BY](../clauses/order-by.md) sorts after DISTINCT, followed by
[OFFSET/SKIP and LIMIT](../clauses/limit-offset.md). Paging never reduces mutations.

[Aggregates](../functions/aggregate-functions.md) run over implicit or explicit groups before
projection, DISTINCT and paging. COUNT is Int64, including on the wire; SUM/AVG use the documented
numeric promotion. A keyless aggregate returns one row even for an empty match, while keyed empty
matches return no rows. [GROUP BY](../clauses/group-by.md) resolves plain names to RETURN aliases.

[RETURN … NEXT](../clauses/next.md) carries projected aliases and bare-variable bindings into a
following segment. [WITH](../clauses/with.md) projects intermediate rows and applies WHERE after
its paging. Every stage materializes in source order; only final columns govern wire admission.

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

