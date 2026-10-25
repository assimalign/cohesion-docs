# GqlCastType

Supported explicit scalar conversion targets.

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public enum GqlCastType
{
    Int32, Int64, Float32, Float64, Decimal, String, Boolean, Date, Timestamp
}
```

## Behavior and exceptions

The enum identifies resolved storage types, independently of accepted textual aliases. INTEGER/INT
maps to Int32, BIGINT/INT64 to Int64, REAL to Float32, DOUBLE/FLOAT/FLOAT64 to Float64,
DECIMAL/NUMERIC to Decimal, STRING/CHAR/VARCHAR to String, BOOL/BOOLEAN to Boolean, DATE to
Date, and ZONED DATETIME/TIMESTAMP WITH TIME ZONE to Timestamp. String targets are unbounded;
Timestamp returns DateTimeOffset.

The enum adds no methods or constructor exceptions. Text INT32, SMALLINT and FLOAT32 are
recognized unsupported (CK00001), despite enum identities Int32/Float32. Type modifiers and
other excluded target families also use CK00001; unknown names use CG00002. Invalid direct AST
targets use CG05001. These codes have no assigned SQLSTATE. See the language CAST page.

## Example

```csharp
var query = ((GqlQueryStatement)new GqlQueryParser().Parse(
    "RETURN CAST('2024-01-02' AS DATE) AS d")).GqlExpression;
Console.WriteLine(((GqlCastExpression)query.Projections[0].Expression).Target); // Date
// Execution returns DateOnly 2024-01-02.
```

## Related reference

[Assembly](index.md) · [Design](design.md) · [Scalar functions](../../../../database/graph/language/functions/scalar-functions.md) · [CAST](../../../../database/graph/language/expressions/cast.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlCastType.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlFunctionParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphFunctionTests.cs`.
