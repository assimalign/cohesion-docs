# GqlScalarFunction

Resolved scalar identities shared by aliases.

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public enum GqlScalarFunction
{
    ElementId, Labels, Type, Properties, Keys, StartNode, EndNode, PathLength,
    Nodes, Relationships, Lower, Upper, Trim, Substring, Replace, CharLength,
    Size, Abs, Ceil, Floor, Round, Sqrt, Log, Mod, ToString, ToInteger,
    ToFloat, ToBoolean, CurrentDate, CurrentTimestamp
}
```

## Behavior and exceptions

These are operation identities rather than written spellings. element_id/elementId share ElementId;
path_length/length share PathLength; relationships/edges share Relationships; ln/log share Log.
char_length/character_length, lower/toLower, upper/toUpper and ceil/ceiling also share identities.
The [language table](../../../../database/graph/language/functions/scalar-functions.md) specifies
arity, accepted kinds, NULLs and result types. The enum adds no methods or constructor exceptions;
invalid direct identities use plan-time COHDBG001 (no SQLSTATE).

Abs/Ceil/Floor/Round retain numeric types (unsigned inputs become Decimal); Neo4j returns Float
for ceil/floor/round. Round uses half away from zero. Sqrt/Log invalid domains use COHDBG013.
CurrentDate/CurrentTimestamp are niladic and share the transaction-start clock. ELEMENTS is not
an enum member or executable function.

## Example

```csharp
var query = ((GqlQueryStatement)new GqlQueryParser().Parse(
    "RETURN length(NULL) AS n")).GqlExpression;
var call = (GqlFunctionCallExpression)query.Projections[0].Expression;
Console.WriteLine(call.Function == GqlScalarFunction.PathLength); // True
// Execution returns NULL.
```

## Related reference

[Assembly](index.md) · [Design](design.md) · [Scalar functions](../../../../database/graph/language/functions/scalar-functions.md) · [CAST](../../../../database/graph/language/expressions/cast.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlScalarFunction.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlFunctionParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphFunctionTests.cs`.
