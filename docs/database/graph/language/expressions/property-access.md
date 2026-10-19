# Subscripts and property access

Reads a list element, map field or stored node/relationship property from a value expression.

> **Status:** Implemented.

## Syntax

```syntaxsql
expression [ index_expression ]
expression . member_name
```

The square brackets in the first form are literal subscript delimiters.

## Remarks

List subscripts require an integral runtime index; floating 1.0 is invalid. Indexes are zero-based;
negative indexes count from the end. Out-of-range indexes yield NULL. Maps, nodes and relationships
accept string subscripts; member syntax uses a field name. Map keys compare ordinally and missing
keys/properties yield NULL. A NULL container or subscript yields NULL. GQL has no MISSING value.

Map access may chain, as in `{k:{j:1}}.k.j`. Node/relationship access also works through a value
expression: `(a).name`, `COALESCE(b,a).name` and `a['name']` read scalar stored properties and
pass the wire Execute shape check. The direct stored-property chain `a.b.c` remains COHDBL001;
`(a.b).c` on a scalar fails at evaluation with COHDBG013. Invalid container/index types also use
COHDBG013. Malformed syntax uses GQL0002; slicing remains COHDBL001. Lists/maps follow the
[shared limits and wire capability](list-values.md#limits-and-wire-capability).

## Examples

```sql
RETURN [10,20,30][-1], [1][5], {k:1}.missing;
RETURN {k: {j: 1}}.k.j;
```

The first row is (30, NULL, NULL); the second is (1).

Execute separately in a fresh database:

```sql
INSERT (:Person {name:'Alice'}), (:Person {name:'Bob'});
MATCH (a:Person) OPTIONAL MATCH (a)->(b)
RETURN (a).name, COALESCE(b,a).name, a['name'];
```

The rows are (Alice, Alice, Alice) and (Bob, Bob, Bob), in unspecified order.

## See also

[Expressions](index.md) · [List values](list-values.md) · [Map values](map-values.md) · [COALESCE](coalesce.md)

## Sources

- **Execution and results** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphValueTests.cs`.
- **Grammar** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlValueParserTests.cs`.
- **Value evaluation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/Internal/GraphExpressionEvaluator.Values.cs`.
