# Map values

Builds transient values with ordinal string keys and expression-valued entries.

> **Status:** Implemented. Maps are query values; stored property maps require scalar values.

## Syntax

```syntaxsql
{ [ key : expression [ , ...n ] ] }
```

The braces are literal syntax. Keys use identifiers or double-quoted names, including keywords.

## Remarks

Each value expression evaluates in source order. Keys are unique under ordinal case-sensitive
comparison; k and K differ. Maps materialize as read-only IReadOnlyDictionary<string, object?>
with ordinal lookup. Nested lists/maps and NULL values are retained. Missing field lookup yields
NULL, with no separate MISSING state. [Member/subscript access](property-access.md) may chain
through nested map values. Parameters accept IReadOnlyDictionary<string, object?> implementations,
including Dictionary<string, object?>, and snapshot referenced trees once per execution.

Equality requires the same key set and equal corresponding values, regardless of entry order.
FALSE dominates UNKNOWN; matching NULL values produce UNKNOWN for ordinary equality. DISTINCT
groups corresponding NULLs. Ordering comparisons compare count, sorted ordinal keys, then values;
an encountered NULL/incomparable value yields UNKNOWN. See [comparison and limits](list-values.md).

Duplicate literal keys report CG00006; malformed syntax CG00002. Invalid programmatic map shape
uses CG05001. Invalid referenced parameter trees use CG03003; evaluated value/access failures
or limits CG02013. Possible composite wire results without graph.composite-values use CG07016
before reads/writes (no SQLSTATE). Constructors obey the [transactional limits](list-values.md#limits-and-wire-capability).
Composite syntax in stored MATCH/INSERT/CREATE property maps, whole property-map parameters and
map projection `a {.name}` remain CK00001.

## Examples

```sql
RETURN {k: {j: 1}}.k.j;
RETURN {k: 1}['missing'];
RETURN {k: NULL} = {k: NULL};
RETURN {k: NULL} <> {j: NULL};
```

Results are 1, NULL, NULL and TRUE. `RETURN {k: [1, NULL]}` returns one map cell with a two-element list.

[ORDER BY](../clauses/order-by.md) compares maps by member count, sorted ordinal keys and then
corresponding values. Nested values use the same total order, with NULL last ascending.

## See also

[Expressions](index.md) · [List values](list-values.md) · [Property access](property-access.md)

## Sources

- **Execution and results** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphValueTests.cs`.
- **Grammar** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlValueParserTests.cs`.
- **Value evaluation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/Internal/Evaluation/GraphExpressionEvaluator.Values.cs`.
