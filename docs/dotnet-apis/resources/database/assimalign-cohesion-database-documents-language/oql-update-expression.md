# OqlUpdateExpression

A collection-local partial UPDATE with SET assignments, UNSET paths and an optional predicate.

Namespace: `Assimalign.Cohesion.Database.Documents.Language`.

## Signature

```csharp
public sealed class OqlUpdateExpression : OqlExpression
{
    public OqlUpdateExpression(string collection, string? alias,
        IReadOnlyList<OqlUpdateAssignment> assignments, IReadOnlyList<OqlPathExpression> unset,
        OqlExpression? predicate, Location? location = null);
    public string Collection { get; }
    public string? Alias { get; }
    public IReadOnlyList<OqlUpdateAssignment> Assignments { get; }
    public IReadOnlyList<OqlPathExpression> Unset { get; }
    public OqlExpression? Predicate { get; }
}
```

## Behavior and exceptions

Construction retains the supplied arguments without validation or execution. Parser-created lists
are read-only. A null Predicate matches all documents. Every [assignment](oql-update-assignment.md)
reads the original document; UNSET removes properties. OqlClauses.Update and OqlClauses.Unset are
the profile names. [OqlExplainExpression](oql-explain-expression.md) can wrap this node.

Execution freezes candidates and values from the statement snapshot, then updates content, versions
and indexes atomically. AffectedCount counts matched documents, including unchanged matches; there
are no result rows. Each match receives a higher version. Untouched JSON fragments retain their
original bytes; whitespace around rewritten container members can change.

Invalid targets or incompatible paths raise DatabaseException with CD02016, overlapping targets
CD01017, and invalid/deep stored JSON CD02018. No SQLSTATE is assigned to these codes.
Absent collections use CD01002, system targets CD04004 and missing parameters CD01005.
Malformed text produces parser diagnostics and requests reject it with DatabaseParseException.
A concurrent committed writer can raise DatabaseTransactionAbortedException. Failure rolls back
the statement or aborts the entire explicit transaction.

## Example

With the DocumentUpdateTests fixture's alpha document (amount 2, details.score 1):

```csharp
var statement = (OqlQueryStatement)new OqlQueryParser().Parse(
    "UPDATE items d SET d.amount = d.details.score, d.details.score = d.amount " +
    "UNSET d.category WHERE name = 'alpha'");
var update = (OqlUpdateExpression)statement.OqlExpression;
Console.WriteLine(update.Collection);        // items
Console.WriteLine(update.Assignments.Count); // 2
Console.WriteLine(update.Unset.Count);       // 1
var result = await session.ExecuteAsync(new DocumentQueryRequest(statement), CancellationToken.None);
Console.WriteLine(result.AffectedCount);     // 1; amount 1, details.score 2, category absent
```

## Related reference

[Assembly](index.md) · [Design](design.md) · [UPDATE](../../../../database/document/language/statements/update.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/OqlUpdateExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentUpdateTests.cs`.
