# OqlMergeExpression

A Couchbase lookup or key-equality ANSI MERGE tree with ordered optional actions.

Namespace: `Assimalign.Cohesion.Database.Documents.Language`.

## Signature

```csharp
public sealed class OqlMergeExpression : OqlExpression
{
    public OqlMergeExpression(string collection, string? alias, OqlMergeSource source,
        OqlExpression on, bool isLookup, bool isPrimary = false,
        OqlMergeUpdateAction? update = null, OqlMergeDeleteAction? delete = null,
        OqlMergeInsertAction? insert = null, OqlReturningExpression? returning = null,
        Location? location = null);
    public string Collection { get; }
    public string? Alias { get; }
    public OqlMergeSource Source { get; }
    public OqlExpression On { get; }
    public bool IsLookup { get; }
    public bool IsPrimary { get; }
    public OqlMergeUpdateAction? Update { get; }
    public OqlMergeDeleteAction? Delete { get; }
    public OqlMergeInsertAction? Insert { get; }
    public OqlReturningExpression? Returning { get; }
    public override string ToString();
}
```

## Behavior and exceptions

Null collection, source or on throws ArgumentNullException. Null Alias uses the target's implicit
name. IsLookup selects ON KEY; IsPrimary retains ON PRIMARY KEY. Actions occur in UPDATE,
DELETE, INSERT order, each at most once, with at least one required by planning. Invalid direct
source/form/action shapes use CD01039; unavailable bindings/metadata use CD01040.

ToString renders canonical text that reparses to the same tree shape. Unsupported AST shape,
rendering complexity or execution-stack exhaustion throws InvalidOperationException. Complete
trees and canonical parse cost are bounded to 128 levels; text overflow uses CD00005.

MERGE parses, renders, binds, plans and explains. Execution refuses one located CD05022 before
source reads or writes until OQ-M2; OqlLanguageProfile.Instance.Supports("MERGE") remains false.
See [forms, scopes and conservative refusals](../../../../database/document/language/statements/index.md#merge-planning).

## Example

```csharp
var statement = (OqlQueryStatement)new OqlQueryParser().Parse(
    "MERGE INTO targets t USING sources s ON KEY s.key WHEN MATCHED THEN DELETE");
var merge = (OqlMergeExpression)statement.OqlExpression;
Console.WriteLine(merge.IsLookup);       // True
Console.WriteLine(merge.Delete is not null); // True
Console.WriteLine(merge.ToString());
```

## Related reference

[Assembly](index.md) · [Source](oql-merge-source.md) · [Update](oql-merge-update-action.md) · [Delete](oql-merge-delete-action.md) · [Insert](oql-merge-insert-action.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/OqlMergeExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlMergeParserTests.cs`.
