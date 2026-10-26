# OqlDeleteExpression

A database-local DELETE target, optional source alias and predicate.

Namespace: `Assimalign.Cohesion.Database.Documents.Language`.

## Signature

```csharp
public sealed class OqlDeleteExpression : OqlExpression
{
    public OqlDeleteExpression(string collection, string? alias, OqlExpression? predicate,
        Location? location = null, OqlReturningExpression? returning = null);
    public string Collection { get; }
    public string? Alias { get; }
    public OqlExpression? Predicate { get; }
    public OqlReturningExpression? Returning { get; }
}
```

## Behavior and exceptions

Construction retains arguments without executing or validating them. A null Predicate means all
documents; otherwise SQL++ truth conversion selects matches. The planner validates metadata,
parameters and nonaggregate filter expressions before mutation, including empty input. Execution
materializes identities from the statement snapshot, reapplies the full filter after any index seek,
and deletes atomically with index maintenance. Collection/index definitions survive.

Invalid metadata/aggregate context raises DatabaseException `CD05001`, absent collection
`CD01002`, system target `CD04004`, and unbound parameter `CD01005`. Predicate value errors
retain their codes, such as `CD02006` (22012). A later conflict rolls back prior deletions and
raises DatabaseTransactionAbortedException; a failed explicit transaction loses all its work.
Malformed text is returned as parser diagnostics; requests reject it with DatabaseParseException.

[OqlExplainExpression](oql-explain-expression.md) can wrap DELETE, producing a Delete root and
CollectionScan/IndexSeek child without deletion. OqlClauses.Delete is the `DELETE` capability name.

Returning optionally retains an [affected-image projection](oql-returning-expression.md).
DELETE projects pre-images and old metadata; null retains count-only execution.

## Example

For the engine test fixture with stored keys a, b, c, d and object:

```csharp
var statement = (OqlQueryStatement)new OqlQueryParser().Parse(
    "DELETE FROM items d WHERE META(d).id = $id");
var delete = (OqlDeleteExpression)statement.OqlExpression;
Console.WriteLine(delete.Collection); // items
Console.WriteLine(delete.Alias);      // d
var request = new DocumentQueryRequest(statement,
    new Dictionary<string, object?> { ["id"] = "b" });
var result = await session.ExecuteAsync(request, CancellationToken.None);
Console.WriteLine(result.AffectedCount); // 1; a, c, d and object remain
```

META uses the external key; the object document's content field `id: 'b'` does not select it.

## Related reference

[Assembly](index.md) · [Design](design.md) · [DELETE](../../../../database/document/language/statements/delete.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/OqlDeleteExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlDmlParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentDeleteTests.cs`.
