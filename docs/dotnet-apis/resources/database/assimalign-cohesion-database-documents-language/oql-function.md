# OqlFunction

Immutable metadata for one executable OQL scalar, aggregate or META function.

Namespace: `Assimalign.Cohesion.Database.Documents.Language`.

## Signature

```csharp
public sealed class OqlFunction
{
    public string Name { get; }
    public int MinimumArguments { get; }
    public int MaximumArguments { get; }
    public DatabaseType ResultType { get; }
    public bool IsAggregate { get; }
    public bool AcceptsArity(int count);
    public bool AcceptsArgument(int index, JsonValueKind kind);
}
```

## Behavior and exceptions

Construction is internal; obtain definitions from [OqlFunctions](oql-functions.md). Name is invariant
uppercase. MaximumArguments is Int32.MaxValue for variadic calls. ResultType Null denotes an
argument-dependent result. COUNT, LENGTH and POSITION use Int64; numeric scalar calculations use Decimal.

AcceptsArity includes COUNT's star as one argument. AcceptsArgument checks a known JSON kind;
NULL/MISSING propagation belongs to the evaluator. A negative index throws ArgumentOutOfRangeException.
It does not validate arity; variadic positions repeat the last declared kind. The planner rejects
incompatible literals with COHDOC001; wrong-kind stored inputs produce NULL.

## Example

```csharp
var length = OqlFunctions.Find("length")!;
Console.WriteLine(length.ResultType); // Int64
Console.WriteLine(length.AcceptsArity(1)); // True
Console.WriteLine(length.AcceptsArgument(0, System.Text.Json.JsonValueKind.String)); // True
Console.WriteLine(length.AcceptsArgument(0, System.Text.Json.JsonValueKind.Number)); // False
```

## Related reference

[Assembly](index.md) · [Design](design.md) · [Scalar functions](../../../../database/document/language/functions/scalar-functions.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/OqlFunction.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlFunctionTests.cs`.
