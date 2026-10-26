# OqlReturningExpression

An affected-document projection attached to INSERT, UPSERT, UPDATE or DELETE.

Namespace: `Assimalign.Cohesion.Database.Documents.Language`.

## Signature

```csharp
public sealed class OqlReturningExpression : OqlExpression
{
    public OqlReturningExpression(IReadOnlyList<OqlProjection> projections,
        bool isValue = false, Location? location = null);
    public IReadOnlyList<OqlProjection> Projections { get; }
    public bool IsValue { get; }
}
```

## Behavior and exceptions

Construction retains child references without validation or execution. Projections stores ordered
expressions/output names; IsValue selects one unwrapped RAW/VALUE expression. Location is the
complete clause span. The engine validates projections even for an empty candidate set; an empty
direct-AST output name fails with CD01026 before mutation, and duplicate names use CD01003.

INSERT/UPSERT/UPDATE project stored post-images; DELETE projects pre-images. Projection failures
roll back the statement and fault an explicit transaction. Retained-result budget exhaustion uses
CD05018; serialized depth above 256 including a named wrapper uses CD05019. These codes have no
SQLSTATE. See [RETURNING](../../../../database/document/language/clauses/returning.md) for syntax,
MISSING rules, frame limits and client compatibility.

## Example

Adapted from DocumentReturningTests' direct AST construction:

```csharp
var returning = new OqlReturningExpression(
    [new OqlProjection(new OqlLiteralExpression(1), "one")]);
var delete = new OqlDeleteExpression("items", null, null, returning: returning);
Console.WriteLine(delete.Returning!.Projections.Count); // 1
Console.WriteLine(delete.Returning.IsValue);            // False
```

Executing this DELETE returns `{"one":1}` per deleted document with its affected count.

## Related reference

[Assembly](index.md) · [Design](design.md) · [OqlDeleteExpression](oql-delete-expression.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/OqlReturningExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentReturningTests.cs`.
