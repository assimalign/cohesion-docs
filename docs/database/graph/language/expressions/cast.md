# CAST

CAST converts a scalar using Cohesion SQL's conversion rules.

## Syntax

```syntaxsql
CAST ( expression AS type )
```

## Supported targets

| Target spellings | Result |
| --- | --- |
| INT, INTEGER | Int32 |
| INT64, BIGINT | Int64 |
| REAL | Single |
| FLOAT, FLOAT64, DOUBLE [PRECISION] | Double |
| DECIMAL, NUMERIC | Decimal |
| STRING, CHAR, VARCHAR | Unbounded String |
| BOOL, BOOLEAN | Boolean |
| DATE | DateOnly |
| ZONED DATETIME, TIMESTAMP WITH TIME ZONE | DateTimeOffset |

## Remarks

NULL converts to NULL. GQL has no MISSING value; a missing property supplies NULL. Numeric-to-integer
conversion refuses fractions, including fractional text. Numeric text is invariant, without
grouping, and must fit without silently rounding. REAL/DOUBLE refuse nonzero underflow to zero;
Decimal-to-REAL converts through invariant decimal text directly, avoiding double rounding.

Boolean accepts Boolean or TRUE/FALSE text; Boolean-to-number is not a supported CAST. Boolean
String results are uppercase. Temporal strings follow SQL's invariant ISO grammar; absent offset
means UTC and a date becomes midnight UTC. Timestamp String results omit the fractional part at
whole seconds. Dates/timestamps are transient values and cannot be stored as Graph properties.

The [Cypher toX functions](../functions/scalar-functions.md) have their own behavior: toInteger
truncates and accepts Boolean, and unparsable toInteger/toFloat/toBoolean strings give NULL.
CAST invalid text or fractions raise an error. CAST is contextual; `cast` is a legal binding
when it is not followed by an opening parenthesis.

## Diagnostics

| Code | Meaning and trigger |
| --- | --- |
| COHDBG003 | Provably unsupported operand kind or Boolean numeric conversion, including under EXPLAIN |
| COHDBG013 | Invalid dynamic kind, text, fraction, overflow, underflow or value |
| COHDBL001 | Recognized unsupported target: type modifiers, LIST, DURATION, BYTES, ZONED TIME, INT32, SMALLINT, FLOAT32, TIMESTAMP WITHOUT TIME ZONE or LOCAL DATETIME |
| GQL0002 | Unknown target spelling or malformed CAST syntax |

These codes have no assigned SQLSTATE. EXPLAIN validates signatures without evaluating conversions.

## Examples

```sql
RETURN CAST('42' AS BIGINT) AS n, CAST(FALSE AS STRING) AS b;
RETURN CAST('2024-01-02' AS DATE) AS d;
RETURN CAST(2.5 AS INTEGER) AS n;
```

The first returns `(42,'FALSE')`, the second DateOnly 2024-01-02, and the third fails COHDBG013.
`RETURN toInteger(2.5) AS n` instead returns Int64 2.

## See also

[Expressions](index.md) · [Scalar functions](../functions/scalar-functions.md) · [SQL CAST](../../../sql/language/expressions/cast.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/Internal/GraphExpressionEvaluator.Cast.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphFunctionTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlFunctionParserTests.cs`.
