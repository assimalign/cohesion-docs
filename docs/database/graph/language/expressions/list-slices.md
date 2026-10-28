# List slices

Copies a range of list elements in their original order.

## Syntax

```syntaxsql
list_expression [ from .. to ]
list_expression [ from .. ]
list_expression [ .. to ]
```

Square brackets and the two dots are literal syntax. At least one bound is required.

## Remarks

The start is inclusive and the end exclusive. An omitted start means zero; an omitted end means
list length. Negative bounds count from the end. Bounds clamp to the list, and a reversed or
empty interval returns `[]`. A NULL source or an explicit NULL bound returns NULL. Source and
bound expressions evaluate before selecting the NULL result.

Planning refuses both omitted bounds (`xs[..]`, CG00012) and known non-integer bound types
(CG02014), even under EXPLAIN. This includes `1.0`, `1.5`, `toFloat(1)` and text bounds.
Dynamic finite numeric bounds truncate toward zero, then must fit signed Int32 before clamping;
Decimal conversion follows the same rule exactly. A non-list source or invalid bound kind/range
uses CG02014. Strings and binary scalars are not list sources.

Slices compose with subscripts and [comprehensions](list-comprehensions.md). Scalar results such
as `[11,12][0..1][0]` may be stored properties; list-valued properties retain CK00001.
Copied elements share the statement work/memory budgets and existing
[list result limits](list-values.md#limits-and-wire-capability).

## Diagnostics

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CG00002 | Malformed slice, including a second range separator | — |
| CG00012 | Both bounds omitted; refused during planning, including EXPLAIN | — |
| CG02014 | Invalid list source or bound kind/range | — |
| CK00001 | List-valued stored property | — |

## Examples

```sql
RETURN [0,1,2,3][1..3];
RETURN [0,1,2,3][-2..];
RETURN [0,1,2,3][..-1];
RETURN [0,1,2,3][-99..99];
RETURN [0,1,2,3][3..1];
RETURN [1,2][NULL..];
```

Execute separately. Results are `[1,2]`, `[2,3]`, `[0,1,2]`, `[0,1,2,3]`, `[]` and NULL.
With parameters from=1.9 and to=3.9, `RETURN [0,1,2,3][$from..$to]` returns `[1,2]`.

## See also

[Expressions](index.md) · [List values](list-values.md) · [Property access](property-access.md) · [Diagnostics](../diagnostics.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphListComprehensionTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphCollectionSliceAdmissionTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/Internal/Evaluation/GraphExpressionEvaluator.Values.cs`.
