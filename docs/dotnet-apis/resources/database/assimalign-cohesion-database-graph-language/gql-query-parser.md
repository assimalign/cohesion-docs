# GqlQueryParser

Parses the executable GQL subset through the shared lexer and analyzer pipeline.

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public sealed partial class GqlQueryParser : QueryParser
{
    public GqlQueryParser(QueryParserOptions? options = null);
    public GqlQueryParser(bool allowCustomFunctions, QueryParserOptions? options = null);
    public override QueryStatement Parse(ReadOnlySpan<char> query);
}
```

## Behavior and exceptions

Parse returns a GqlQueryStatement with diagnostics for malformed/unsupported text; concurrent
calls on the parser are serialized. The original constructor keeps standalone unknown-name
diagnostics (CG00010). allowCustomFunctions: true admits ordinary named calls for later engine
binding; it neither registers nor invokes a function. Reserved unsupported calls remain CK00001.
GraphQueryRequest.FromGql enables custom call admission automatically.

Parsed outer modes use GqlPathPattern.Mode; omitted mode is null/implicit Trail. BEGIN
[TRANSACTION] maps to GqlTransactionCommand.Start. Unsupported BEGIN/START options report
CG00002. GQL enables the shared RecognizeLeftArrow option for incoming arrows.
Request factories reject Error diagnostics with DatabaseParseException; parser stack exhaustion
CG00009 translates to CG05008 at the engine boundary. See [diagnostics](../../../../database/graph/language/diagnostics.md).

## Example

```csharp
var parser = new GqlQueryParser(allowCustomFunctions: true);
var statement = (GqlQueryStatement)parser.Parse("RETURN developer_echo(42)");
Console.WriteLine(statement.Diagnostics.Count); // 0; engine binding still required
```

## Related reference

[Assembly](index.md) · [GqlNamedFunctionCallExpression](gql-named-function-call-expression.md) · [GqlPathMode](gql-path-mode.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlQueryParser.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlNamedFunctionParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlTransactionStatementTests.cs`.
