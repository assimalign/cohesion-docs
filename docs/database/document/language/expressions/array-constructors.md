# Array constructors

Builds a JSON array by evaluating expressions in source order.

## Syntax

```syntaxsql
"[" [ expression [ , ...n ] ] "]"
```

Quoted brackets are literal array delimiters; omit the diagram quotes. Empty arrays use `[]`.

## Remarks

Elements may be scalars, paths, parameters, aggregates or nested object/array constructors.
MISSING elements become JSON null and keep their positions; explicit NULL also becomes null.
Unlike [object members](object-constructors.md), elements are never omitted. Arrays compose with
SELECT, VALUE/RAW, grouping, predicates and INSERT. Array comprehensions and IN's array operand
form remain unsupported (`COHDBL001`); path access on constructor results is also outside the subset.

SELECT constructor results are limited to 256 levels, including nested stored/parameter values
and the wrapper object for named projections (`COHDOC015` at the constructor). INSERT retains
the 128-level JSON storage limit (`COHDOC008`). Text expression parsing is separately limited to
128 recursive levels (`OQL0005`). These limits have no SQLSTATE assigned by the source.

## Examples

For the [SELECT fixture](../statements/select.md#distinct-and-bare-values):

```sql
SELECT [name, category, MISSING, NULL, {x: 1}] AS pair
FROM items WHERE name = 'beta';
SELECT VALUE [] FROM items WHERE name = 'beta';
```

Results are `{"pair":["beta","a",null,null,{"x":1}]}` and one bare `[]`.

## See also

[Expressions](index.md) · [Object constructors](object-constructors.md) · [SELECT](../statements/select.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlProjectionParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentQueryTests.Projections.cs`.
