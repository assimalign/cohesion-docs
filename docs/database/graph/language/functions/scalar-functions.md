# Scalar functions

GQL executes core entity, path, string, numeric, conversion and transaction-clock functions.

## Syntax

```syntaxsql
function_name ( argument [ , ...n ] )
CURRENT_DATE
CURRENT_TIMESTAMP
```

Names are case-insensitive; aliases below resolve to the same operation. Ordinary function words
remain usable as variables and keys. CURRENT_DATE/CURRENT_TIMESTAMP have no parentheses and
are reserved at unquoted binding sites. Quoted names must also be referenced quoted.

## Arguments and results

All scalar calls are strict: any NULL argument gives NULL before kind/domain checks, although
every argument expression still evaluates. Niladic clocks have no arguments. GQL has no MISSING
value; a missing property supplies NULL. Planning validates every signature and binding, including
skipped branches and empty matches.

| Function and aliases | Arguments | Non-NULL result |
| --- | --- | --- |
| element_id, elementId | node or relationship | Stable database-local String identity; id() is unsupported and directs callers here |
| labels | node | Ordinal-sorted label list |
| type | relationship | Type String |
| properties | node, relationship or map | Read-only property map |
| keys | node, relationship or map | Ordinal-sorted key list |
| startNode, endNode | relationship | Stored source/target node in the statement snapshot |
| path_length, length | path | Int64 relationship count |
| nodes | path | Non-NULL nodes in traversal order |
| relationships, edges | path | Non-NULL relationships in traversal order; stored directions retained |
| lower/toLower, upper/toUpper | string | Invariant-case String |
| trim | string | String with leading/trailing U+0020 spaces removed |
| substring | string, numeric start [, length] | Zero-based Unicode code-point slice; past-end gives empty String |
| replace | string, search, replacement | Ordinal String replacement; empty search inserts at every UTF-16 code-unit boundary and both ends |
| char_length, character_length | string | Int64 Unicode code-point count |
| size | list or string | Int64 item or code-point count |
| abs, ceil/ceiling, floor | number | Input numeric type; unsigned inputs become Decimal |
| round | number [, nonnegative whole precision] | Input numeric type; unsigned inputs become Decimal; half away from zero |
| sqrt | nonnegative number | Finite Double |
| ln, log | ln(value), log(value), log(base,value) | Finite Double; one-argument log is natural logarithm |
| mod | two numbers | Same promotion and zero-divisor rules as % |
| toString | number, Boolean, string or temporal | Invariant String; lowercase Boolean text |
| toInteger | number, Boolean or string | Int64, truncating fractions toward zero; Boolean becomes 1/0 |
| toFloat | number or string | Finite Double |
| toBoolean | Boolean, integer or string | Boolean; numeric zero is false |
| CURRENT_DATE | none | UTC DateOnly at transaction start |
| CURRENT_TIMESTAMP | none | UTC DateTimeOffset at transaction start |

Element/path and basic string/numeric ISO forms coexist with Cypher aliases and Cohesion
extensions. In particular labels/type/properties/keys/startNode/endNode, nodes/relationships,
substring/replace, round and toX conversions are extensions. SQL's SUBSTRING FROM/FOR spelling
is outside this GQL subset. CAST has its own [conversion rules](../expressions/cast.md).

## Numeric and string behavior

abs/ceil/floor/round preserve numeric CLR types and exact integer identity above 2^53. Neo4j
returns Float for ceil/floor/round; Cohesion keeps the input type. Round uses half away from zero,
including negative ties. sqrt requires a nonnegative argument. Logarithms require positive
values; a written base must be positive and unequal to 1. Invalid domains raise CG02013,
with no NaN/infinity result. MOD follows [arithmetic promotion](../expressions/operators.md).

Substring indexes/counts use Unicode code points and start at zero. Floating coordinates truncate
toward zero before checking the resulting nonnegative Int32 range; thus -0.5 becomes zero.
Invalid ranges use CG02013. Replacement checks expansion size before allocation, including
empty-search insertion. Existing composite depth, value-count and encoded-byte bounds apply to
function outputs.

toInteger/toFloat/toBoolean return NULL for unparsable strings, following Neo4j conversion behavior.
String/Decimal range overflow and nonfinite results still fail CG02013. toInteger saturates
finite floating overflow to the Int64 endpoints; CAST refuses it. toString retains a fractional
digit for floating integers and uses Cypher scientific-notation thresholds. CAST renders Boolean
text uppercase; toString renders it lowercase.

## Transaction clock and wire results

One UTC instant is captured before the transaction begins. Every row/stage and every statement
in an explicit API transaction shares it, including ReadCommitted transactions. Each autocommit
statement captures its own instant. Graph wire statements are always autocommit. CURRENT_DATE
uses that instant's UTC date; CURRENT_TIMESTAMP returns it as DateTimeOffset.

Dates/timestamps can be compared and ordered but cannot be stored as Graph properties. Proven
temporal property values fail planning with CG03003; dynamic values use CG02013 and roll back.
Total ordering places zoned timestamps, then dates, between paths and strings.

labels/keys/properties and other list/map wire results require graph.composite-values.
Entity-valued results such as nodes(p), relationships(p) and startNode(r) fail Execute admission
before writes. Scalar access such as nodes(p)[0].name is legal; a carried bare node can use
ExecutePaths. No new wire tag or format is introduced.

## Diagnostics

| Code | Meaning and trigger |
| --- | --- |
| CG00010 | Unknown function name, reported during parsing |
| CG00002 | Parsed scalar/aggregate arity mismatch, or parentheses on a niladic clock |
| CG05001 | Invalid hand-built arity or unbound function argument |
| CG03003 | Provably incompatible argument kind, including non-integral numeric toBoolean; temporal stored property |
| CG02013 | Dynamic wrong kind, invalid conversion/domain/range, zero divisor, overflow or output bound |
| CG07016 | Possible composite wire result without negotiated capability |
| CK00001 | Recognized unsupported calls, including ELEMENTS(), CARDINALITY, temporal constructors, reduce, head/last/tail/reverse/range, LTRIM/RTRIM/BTRIM, LEFT/RIGHT, LOG10 and toXList conversions |

These codes have no assigned SQLSTATE. EXPLAIN checks signatures without evaluating functions.
Runtime failures precede ResultHeader and preserve the authenticated wire session.

## Examples

Each standalone RETURN executes once:

```sql
RETURN substring('A😀BC',1,2) AS part, char_length('A😀B') AS n;
RETURN round(-2.5) AS rounded, ceil(9007199254740993) AS exact;
RETURN replace('abc','','X') AS text;
RETURN toInteger('2.5') AS n, toFloat('bad') AS f, toBoolean('bad') AS b;
RETURN toString(FALSE) AS cypher, CAST(FALSE AS STRING) AS sql;
RETURN CURRENT_TIMESTAMP = CURRENT_TIMESTAMP AS same, CURRENT_DATE AS today;
```

Results are `('😀B',3)`, `(-3.0,9007199254740993)`, `'XaXbXcX'`, `(2,NULL,NULL)`,
`('false','FALSE')`, and `(TRUE,the transaction's UTC date)`. `RETURN sqrt(-1)` and
`RETURN log(1,2)` fail CG02013. `RETURN abs(NULL)` returns NULL.

With a path inserted separately:

```sql
INSERT (a:P {name:'A'})-[:T {w:1}]->(:P {name:'B'});
MATCH p = (a:P {name:'A'})-[:T]->(b) RETURN length(p) AS hops, nodes(p)[1].name AS last;
```

The query returns `(1,'B')` through either embedded rows or wire QueryAsync. With the same
matched a, r, b and path p, these expressions have the following results:

| Expression | Result |
| --- | --- |
| element_id(a) = elementId(a) | TRUE |
| labels(a) | ['P'] |
| type(r) | 'T' |
| properties(a) | {name:'A'} |
| keys(a) | ['name'] |
| startNode(r).name, endNode(r).name | 'A', 'B' |
| path_length(p), length(p) | 1, 1 |
| nodes(p)[0].name | 'A' |
| relationships(p)[0].w, edges(p)[0].w | 1, 1 |

Use `MATCH p = (a:P {name:'A'})-[r:T]->(b) RETURN` followed by the desired expression(s).
List/map row results require composite negotiation over the wire. For standalone calls:

| Statement | Result |
| --- | --- |
| RETURN lower('AbC'), upper('AbC'), trim('  x  ') | 'abc', 'ABC', 'x' |
| RETURN substring('A😀BC',2), size([NULL,1]) | 'BC', 2 |
| RETURN abs(-2), floor(2.5), ceiling(2.5), round(1.25,1) | 2, 2.0, 3.0, 1.3 |
| RETURN sqrt(4), ln(1), log(1), log(2,8), mod(5,2) | 2.0, 0.0, 0.0, 3.0, 1 |
| RETURN toFloat('2'), toBoolean(0), toString(2.0) | 2.0, FALSE, '2.0' |

## See also

[Functions](index.md) · [CAST](../expressions/cast.md) · [Patterns](../patterns.md) · [Diagnostics](../diagnostics.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphFunctionTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Client/tests/GraphFunctionWireTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/docs/DESIGN.md`.
