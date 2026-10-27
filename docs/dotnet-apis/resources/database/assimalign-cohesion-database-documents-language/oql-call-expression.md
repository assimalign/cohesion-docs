# OqlCallExpression

A scalar or aggregate call with its source span and ordered arguments.

Namespace: `Assimalign.Cohesion.Database.Documents.Language`.

## Signature

```csharp
public sealed class OqlCallExpression : OqlExpression
{
    public OqlCallExpression(string name, IReadOnlyList<OqlExpression> arguments,
        Location? location = null, OqlAggregateQuantifier quantifier = OqlAggregateQuantifier.None);
    public string Name { get; }
    public IReadOnlyList<OqlExpression> Arguments { get; }
    public OqlAggregateQuantifier Quantifier { get; }
}
```

## Behavior and exceptions

The constructor stores its arguments without validation; supply an uppercase Name. Text parsing
normalizes the name and checks [OqlFunctions](oql-functions.md). META uses OqlMetaExpression.
Unknown text calls report CD00008; invalid arity/star reports CD00006. Programmatic invalid calls,
incompatible literal kinds, invalid aggregate placement and grouping fail planning with CD05001.
Text/session factories reject parser errors with DatabaseParseException.

[Quantifier](oql-aggregate-quantifier.md) retains None, All or Distinct. None and All have the same
input semantics. Binding an invalid enum value, scalar quantifier or quantified star in a direct
AST reports CD01029 (no SQLSTATE). Parsed scalar/star quantifiers use CD00011 instead. All eleven
native aggregate names and single-argument registered aggregates accept ALL/DISTINCT.

Scalar calls can wrap aggregate results or appear inside aggregate arguments, but aggregates cannot
nest. Calls are not constants or index seek bounds and are refused in INSERT/UPSERT VALUES.
NULL/MISSING, lazy conditional evaluation, transaction clocks and runtime diagnostics are described
in [Scalar functions](../../../../database/document/language/functions/scalar-functions.md).

## Example

```csharp
var statement = (OqlQueryStatement)new OqlQueryParser().Parse(
    "SELECT REPLACE(name,'a','b',2) FROM items");
var call = (OqlCallExpression)((OqlSelectExpression)statement.OqlExpression)
    .Projections[0].Expression;
Console.WriteLine(call.Name); // REPLACE
Console.WriteLine(call.Arguments.Count); // 4
```

## Related reference

[Assembly](index.md) · [Design](design.md) · [OqlFunction](oql-function.md) · [OqlAggregateQuantifier](oql-aggregate-quantifier.md)

## Application function calls

OqlQueryParser(allowUnregisteredFunctions: true) admits ordinary named calls without requiring
an OqlFunctions entry. Engine request factories and session/wire execution enable this
automatically, then bind the name/arity against the executing engine's DocumentFunctionCatalog.
Standalone parsing keeps CD00008 for unknown names. An unregistered engine name still uses
CD00008 but fails during binding/execution (ExecutionFailure, no StatementParseFailed).
Native/reserved names cannot be replaced. See [DocumentFunction](../assimalign-cohesion-database-documents/document-function.md)
for callback metadata, NULL/MISSING policies and failures.

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/OqlCallExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlFunctionTests.cs`.
