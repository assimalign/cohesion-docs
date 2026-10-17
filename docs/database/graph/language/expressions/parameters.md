# Parameters

Binds runtime scalar values in GQL WHERE operands and node/relationship property maps.

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
names, not positions: $01 binds 01. Identifier names start with a letter or underscore and
continue with letters/digits/underscores. NULL is a bound value; unused values are ignored.

MATCH/INSERT/CREATE property maps and WHERE operands bind parameters through GraphQueryRequest,
GraphPathsQueryRequest, session text execution, and GraphConnection parameter maps. Runtime types,
including Int32 and Decimal, are preserved. Schema matching uses typed graph promotion rules.

| Code | Meaning |
| --- | --- |
| COHDBG014 | Normalized caller keys collide, including unused keys |
| COHDBG001 | Referenced binding missing; fails planning before reads or writer locks |
| COHDBG003 | Referenced value has unsupported type or is non-finite; list/map values excluded |
| GQL0002 | Malformed reference, bare sigil, identifier/DELETE-position parameter |
| COHDBL001 | RETURN $p, $$name or a whole property-map parameter |

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

## See also

[Expressions](index.md) · [WHERE](../clauses/where.md) · [GraphQueryRequest](../../../../dotnet-apis/resources/database/assimalign-cohesion-database-graph/graph-query-request.md)

## Sources

- **Binding and types** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphParameterTests.cs`.
- **Wire** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Client/tests/GraphPathWireTests.cs`.
