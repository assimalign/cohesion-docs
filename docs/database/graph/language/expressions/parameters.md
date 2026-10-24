# Parameters

Binds runtime scalar, list and map values in GQL WHERE, RETURN and node/relationship property maps.

## Syntax

```syntaxsql
$name
@name
$n
@n
```

## Remarks

Names bind case-sensitively by bare name after stripping one leading sigil from caller keys,
regardless of the caller dictionary's comparer. Numeric names contain ASCII digits only and are
names, not positions: $01 binds 01. Identifier names start with a letter or underscore and continue
with letters/digits/underscores. NULL is a bound value; unused values are ignored.

MATCH/INSERT/CREATE property maps, WHERE operands and RETURN expressions bind parameters through GraphQueryRequest,
GraphPathsQueryRequest, session text execution, and GraphConnection parameter maps. Runtime types,
including Int32 and Decimal, are preserved. Schema type checks use typed graph promotion rules;
comparisons, property matching, IN and DISTINCT use exact represented numeric values. Decimal
`0.1m` and Double `0.1d` compare unequal; Int64 `1` and Double `1.0` compare equal. Arithmetic
retains its [promotion table](operators.md#arithmetic-and-concatenation).

| Code | Meaning |
| --- | --- |
| CG01014 | Normalized caller keys collide, including unused keys |
| CG05001 | Referenced binding missing; fails planning before reads or writer locks |
| CG03003 | Invalid referenced tree: unsupported/non-finite leaf, cycle, dimensions or value bounds; composites in stored pattern properties |
| CG00002 | Malformed reference, bare sigil, identifier/DELETE-position parameter |
| CK00001 | $$name or a whole property-map parameter |

[List and map parameters](list-values.md) accept IList (except byte[] binary scalars) and
IReadOnlyDictionary<string, object?>. Arrays must be zero-based and one-dimensional. Referenced
trees snapshot once per execution and obey depth 64, 65,536 total values including root, and
16 MiB encoded bytes. Unused values remain ignored by engine planning. The wire client encodes
supplied values before sending; graph.composite-values must be negotiated under protocol 2.1.
Without it, the client refuses composite parameters locally with ExecutionFailure and keeps the
connection reusable. Stored MATCH/INSERT/CREATE properties remain scalar-only.

EXPLAIN performs the same validation. A pre-output terminal path refusal keeps the wire connection
usable; unfinished output/cancellation follows the normal discard rule. A failed statement in an
explicit engine transaction still aborts that transaction until rollback.

? and :name share SQL/OQL's diagnostic message:

```text
Unsupported parameter form; use $name, @name, or $1. Names must be an identifier (a letter or _, then letters, digits or _) or digits only.
```

## Examples

Bind name to string Ada and weight to Decimal 2.5; execute separately:

```sql
INSERT (:P {name: $name})-[:T {weight: @weight}]->(:P);
MATCH (a:P {name: @name})-[r:T {weight: $weight}]->(b)
WHERE $weight = r.weight RETURN a.name, r.weight;
```

INSERT affects three entities. MATCH returns (Ada, 2.5), retaining Decimal for weight.

```sql
RETURN $weight AS weight;
```

This standalone query returns one row containing Decimal `2.5`, with column name `weight`.

## See also

[Expressions](index.md) · [WHERE](../clauses/where.md) · [GraphQueryRequest](../../../../dotnet-apis/resources/database/assimalign-cohesion-database-graph/graph-query-request.md)

## Sources

- **Binding and types** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphParameterTests.cs`.
- **Wire** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Client/tests/GraphPathWireTests.cs`.
