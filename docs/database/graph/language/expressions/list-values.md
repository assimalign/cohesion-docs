# List values

Builds ordered transient values for projections, comparisons, parameters and membership tests.

> **Status:** Implemented. Stored properties remain scalar-only.

## Syntax

```syntaxsql
[ [ expression [ , ...n ] ] ]
expression IN list_expression
```

The outer square brackets are literal syntax; the inner optional group permits an empty list.

## Remarks

Lists evaluate items in order and materialize as object?[] in embedded and typed-client rows.
They may nest lists and [maps](map-values.md). NULL elements remain elements; GQL has no MISSING.
[Parameters](parameters.md) accept BCL IList values, including object arrays and List<long>;
byte[] remains a binary scalar. Arrays must be one-dimensional and zero-based.
[Subscripts](property-access.md) retrieve elements, including negative indexes from the end.

IN accepts any list-valued expression, including a parameter or parenthesized list. It evaluates
the full list before testing membership; a later item error still fails after an earlier match.
Empty-list IN is FALSE even for NULL. Otherwise an equal member wins, then a NULL operand or
unknown element comparison yields UNKNOWN, otherwise FALSE. A NULL list yields NULL; a non-list
right operand reports COHDBG013.

## Comparison

List equality requires equal lengths and element-wise equality. FALSE dominates UNKNOWN;
otherwise any NULL comparison makes equality UNKNOWN. Maps compare equal key sets and values,
independent of entry order. IN, simple CASE and NULLIF share this rule. Numbers compare exact
represented values: 1 equals 1.0; Decimal 0.1m and Double 0.1d parameters differ.
DISTINCT compares the same structures but groups corresponding NULLs and retains runtime types.
List ordering comparisons are lexicographic; an encountered NULL or incomparable pair yields
UNKNOWN. [ORDER BY](../clauses/order-by.md) instead uses a total family/container order,
including lexicographic lists, with nested NULL last ascending.

## Limits and wire capability

| Limit per value | Bound |
| --- | --- |
| Depth | 64; root depth is zero |
| Total values | 65,536 including root and all descendants |
| Encoded bytes | 16,777,216 (16 MiB), including UTF-8 bytes, escaping and framing |

A result row must also fit 16 MiB. Referenced parameter trees are snapshotted once per execution;
unsupported/non-finite leaves, invalid dimensions, cycles or exceeded bounds use COHDBG003 before
reads/writes. Constructed values enforce limits during evaluation inside the statement transaction
with COHDBG013, including embedded execution. A refused result rolls back its statement's writes.
Result header sizes check before writer locks; row sizes check before commit.

Protocol 2.1 negotiates graph.composite-values independently of diagnostics. Without it, a possible
composite result uses COHDBG016 before reads/writer locks/writes, even for empty matches or skipped
CASE arms. No SQLSTATE is assigned. The typed client rejects composite parameters locally with
ExecutionFailure before sending a frame; the connection stays reusable. Scalar columns keep their
baseline tuple bytes. [GraphValueCodec](../../../../dotnet-apis/resources/database/assimalign-cohesion-database-graph/graph-value-codec.md)
provides the public component format. Path payloads are unchanged.

## Diagnostics

| Code | Meaning and trigger |
| --- | --- |
| `COHDBG003` | Invalid/oversized referenced parameter or composite stored-property parameter |
| `COHDBG013` | Evaluated value/access error or constructed value/row exceeds a limit |
| `COHDBG016` | Possible composite wire result without negotiated capability |
| `COHDBL001` | List/pattern comprehension, slicing or composite literal in a stored property map |
| `GQL0002` | Malformed list or membership syntax |

## Examples

```sql
RETURN [10,20,30][-1];
RETURN [1] IN [[1]];
RETURN [NULL] = [NULL];
RETURN [NULL,1] = [NULL,2];
RETURN [1,2] < [1,3];
```

Results are 30, TRUE, NULL, FALSE and TRUE, respectively. A RETURN of [1, NULL] exposes one list cell
with those two elements, rather than two columns.

## See also

[FOR/UNWIND](../clauses/for.md) expands lists; NULL gives zero rows and a non-list is COHDBG013.
Proven node/relationship kinds survive expansion, but final entity lists cannot cross the wire
(COHDBG001; #1662), even with graph.composite-values. Project scalar properties after expansion.

[Expressions](index.md) · [Map values](map-values.md) · [Property access](property-access.md) · [Diagnostics](../diagnostics.md)

## Sources

- **Execution and results** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphValueTests.cs`.
- **Grammar** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlValueParserTests.cs`.
- **Value evaluation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/Internal/GraphExpressionEvaluator.Values.cs`.
