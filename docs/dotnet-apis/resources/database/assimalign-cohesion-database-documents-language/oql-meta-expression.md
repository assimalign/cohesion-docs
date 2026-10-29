# OqlMetaExpression

Namespace: `Assimalign.Cohesion.Database.Documents.Language`.

## Signature

```csharp
public sealed class OqlMetaExpression : OqlExpression
{
    public OqlMetaExpression(IReadOnlyList<OqlExpression> arguments, string? member, Location? location = null);
    public IReadOnlyList<OqlExpression> Arguments { get; }
    public string? Member { get; }
}
```

## Behavior and exceptions

Arguments retain an optional source alias; Member selects id or version. The planner validates
arity, member and stored-source context with CD05001. id evaluates as string and version as
decimal. Bare META and system-source metadata do not execute.
In joined SELECTs, an explicit receiver is required (CD01035 for ambiguous/invalid metadata).
Each stored source retains its identity; unmatched JOIN metadata is MISSING. NEST arrays and
derived/expression rows yield NULL metadata; an unknown-key LEFT NEST yields MISSING.

These AST constructors retain child references and do not execute a query. Parser-produced lists are
read-only snapshots. Always inspect statement diagnostics before planning; execution performs
semantic validation and uses DatabaseException.Diagnostics for coded failures.

## Example

```csharp
var node = new OqlMetaExpression([], "id");
Console.WriteLine(node.Member); // id
```

## Related reference

[Assembly](index.md) · [Design](design.md) · [Language reference](../../../../database/document/language/functions/meta.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/OqlMetaExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlDmlParserTests.cs`.
