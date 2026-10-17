# RETURN

RETURN projects expressions or named graph bindings, with optional duplicate elimination.

> **Status:** Implemented.

## Syntax

```syntaxsql
RETURN [ DISTINCT | ALL ] { * | expression [ AS alias ] [ , ...n ] }

<expression> ::= scalar_expression | bound_variable
```

## Supported clauses

| Construct | Support |
|---|---|
| Bound node/relationship/path variables | Supported in embedded rows; one binding through the path API |
| Literals, parameters, properties and computed expressions | Supported; same value expressions as WHERE |
| `AS` aliases | Supported |
| `ALL`, `DISTINCT`, `*` | Supported; ALL is the default, star cannot mix with explicit items |
| Functions and aggregation | Recognized, not supported (`COHDBL001`) |
| `ORDER BY`, `LIMIT`, `OFFSET`, `SKIP` | Recognized, not supported (`COHDBL001`) |

## Arguments

- **`expression`** — a [value expression](../expressions/operators.md), including a literal,
  parameter, one-level property, bound variable, arithmetic, concatenation, null/truth or labeled test.
- **`alias`** — an optional result-column name.
- **`*`** — every named binding once, in first-declaration order.

## Remarks

Columns preserve item order. An alias names its column exactly; otherwise the name is normalized
source text, with comments removed and token spacing normalized, preserving spelling, quoted text
and grouping. Thus `a . age /* comment */ + 1` names `a.age + 1`. This naming rule is a Cohesion
choice. Aliases do not introduce bindings for neighboring items. Final names must be unique under
ordinal, case-sensitive comparison: `x` and `X` are different.

Star expands first declarations, including a path variable before its elements. For
`MATCH p = (a)-[r]->(b) RETURN *`, columns are `p`, `a`, `r`, `b`. Anonymous elements and repeated
references are omitted. Star requires at least one named binding and cannot accompany explicit items.

Missing properties project NULL; GQL has no MISSING value. NULL propagation follows the individual
operators. `DISTINCT` compares the complete projected tuple: NULLs group together, strings compare
ordinally, and numbers compare by exact represented value across runtime types. `1 = 1.0` is TRUE;
Decimal `0.1m` and Double `0.1d` supplied through parameters are distinct. Comparisons and IN use
the same equality. Signed zeros compare equal, large integers keep every bit, and subnormals remain
distinct from zero. Arithmetic retains its separate [promotion rules](../expressions/operators.md#arithmetic-and-concatenation).
The first representative retains its runtime types. Nodes/relationships compare by identity;
paths compare ordered node/relationship identities, including traversal direction.

RETURN may stand alone, finish a read match, or follow insertion. It cannot follow deletion or SHOW.
A standalone RETURN evaluates once and returns one row. An empty MATCH returns zero rows while
retaining result-column metadata. DISTINCT runs after insertion: it removes output duplicates
without suppressing writes for matched bindings. No result-row ordering is promised.

### Embedded and wire results

Embedded rows can expose GraphNode, GraphRelationship or GraphPath through DatabaseRow.GetValue.
The wire Execute exchange, used by GraphConnection.QueryAsync and ExecuteAsync, permits only
statically scalar result kinds. Entity projections fail before data access, writer locks or
mutation, even for empty matches; the error directs callers to ExecutePaths. Scalar expressions
such as `a.age + 1` and `a = b` pass. EXPLAIN returns scalar plan rows even for entity projections.

ExecutePaths/QueryPathsAsync accepts exactly one bare node, relationship or path binding from a
read-only MATCH, optionally DISTINCT, or star over one named binding. Computed, multiple or
multi-binding star projections fail the path-request shape check before duplicate-name checks.

## Diagnostics

| Code | Meaning and trigger |
| --- | --- |
| `COHDBG015` | Duplicate final column name at the second expression; fails planning before reads/writes, even on empty matches or under EXPLAIN |
| `COHDBG001` | Unbound variable/parameter, invalid programmatic RETURN shape, invalid path-request projection, or entity result through wire Execute |
| `COHDBG003` | Path property access, or unsupported/non-finite referenced parameter/programmatic AST literal |
| `COHDBG013` | Evaluated scalar value error, including arithmetic overflow or zero divisor |
| `GQL0002` | Malformed RETURN, mixing star/items, or star without a named binding |
| `GQL0004` | Numeric text outside the signed Int64 or finite Double range |
| `COHDBL001` | Functions, aggregation, map projection, or RETURN after deletion |

The source assigns no SQLSTATE to COHDBG015. Wire failures retain ExecutionFailure and a usable
connection when rejected before output; negotiated diagnostics preserve the engine code and span.

## Examples

Execute separately:

```sql
INSERT (:Person {name:'Alice', age:42}), (:Person {name:'Bob', age:17});
MATCH (a:Person), (b:Person)
RETURN DISTINCT a.age + 1 AS next, a.name || '!';
RETURN 1 + 1 AS two, 1 = 1.0 AS equal;
MATCH (a:Person {name:'Alice'}) RETURN a.name AS person, a.age, a.absent;
```

The first query returns `(43, Alice!)` and `(18, Bob!)` once each, with columns `next` and
`a.name || '!'`. The standalone query returns `(2, TRUE)`. The last returns `(Alice, 42, NULL)`.

```sql
MATCH (a:Person {name:'Alice'}) RETURN *;
RETURN 1 AS x, 2 AS x;
```

The star returns one `a` column containing Alice's GraphNode in embedded rows or a singleton
path through QueryPathsAsync; wire QueryAsync rejects it with COHDBG001 before reading.
The second statement reports COHDBG015. `RETURN 1 AS x, 2 AS X` succeeds with `(1, 2)`.

## See also

- **[Clauses](index.md)** — clause navigation.
- **[RETURN statement composition](../statements/return.md)** — complete read syntax.
- **[INSERT](../statements/insert.md)** — projection after insertion.

## Sources

- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/docs/DESIGN.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlQueryParserTests.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/docs/DESIGN.md`.
- **Projection and DISTINCT** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphReturnTests.cs`.
- **Exact numeric comparison** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphNumericComparisonTests.cs`.
- **Wire results and rejections** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Client/tests/GraphReturnWireTests.cs`.

