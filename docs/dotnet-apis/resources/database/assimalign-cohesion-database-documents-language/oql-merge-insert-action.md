# OqlMergeInsertAction

The unmatched MERGE INSERT action, whose target binding is MISSING.

Namespace: `Assimalign.Cohesion.Database.Documents.Language`.

## Signature

```csharp
public sealed record OqlMergeInsertAction
{
    public OqlMergeInsertAction(OqlExpression? key, OqlExpression value,
        OqlExpression? predicate = null);
    public OqlExpression? Key { get; }
    public OqlExpression Value { get; }
    public OqlExpression? Predicate { get; }
}
```

## Behavior and exceptions

Null value throws ArgumentNullException. Lookup INSERT uses the ON KEY key and a null Key;
ANSI INSERT requires explicit KEY and VALUE. Value, Key and Predicate see the source and a
MISSING target, including target metadata. Omitted ANSI key/value semantics remain CK00001.
Construction only retains children; MERGE execution is CD05022 until OQ-M2. See
[MERGE planning](../../../../database/document/language/statements/index.md#merge-planning).

## Example

```csharp
var parsed = (OqlQueryStatement)new OqlQueryParser().Parse(
    "MERGE INTO targets t USING sources s ON KEY s.key WHEN NOT MATCHED THEN INSERT s");
Console.WriteLine(((OqlMergeExpression)parsed.OqlExpression).Insert!.Key is null); // True
```

## Related reference

[OqlMergeExpression](oql-merge-expression.md) · [Assembly](index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/OqlMergeInsertAction.cs`.
