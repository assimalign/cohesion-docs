# OqlFunctions

The single executable function table shared by the OQL parser, profile and engine.

Namespace: `Assimalign.Cohesion.Database.Documents.Language`.

## Signature

```csharp
public static class OqlFunctions
{
    public static IReadOnlyList<OqlFunction> All { get; }
    public static OqlFunction? Find(string name);
}
```

## Behavior and exceptions

All is immutable and ordered by declaration: META, eleven native aggregate names and 34 SQL++ scalars.
The aggregates are COUNT, SUM, AVG, MIN, MAX, ARRAY_AGG, BOOL_AND, BOOL_OR, EVERY, STDDEV and
VARIANCE. EVERY aliases BOOL_AND; quantified EVERY retains its separate expression node.
Find compares names with OrdinalIgnoreCase and returns null for an unknown name; it declares no
lookup exception. It does not register user functions. Function names remain usable as fields.
The parser emits CD00008 for unknown calls and CD00006 for invalid arity/star arguments.
Deferred calls such as REGEXP_LIKE retain CK00001.

## Example

```csharp
Console.WriteLine(OqlFunctions.All.Count); // 46
Console.WriteLine(OqlFunctions.Find("count")!.IsAggregate); // True
Console.WriteLine(OqlFunctions.Find("replace")!.MaximumArguments); // 4
Console.WriteLine(OqlFunctions.Find("LOWER2") is null); // True
```

## Related reference

[OqlFunction](oql-function.md) · [OqlCallExpression](oql-call-expression.md) · [Assembly](index.md) · [Design](design.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/OqlFunctions.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlFunctionTests.cs`.
