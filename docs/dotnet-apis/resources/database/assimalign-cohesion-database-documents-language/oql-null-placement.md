# OqlNullPlacement

The written nullish placement of an OQL ordering term.

Namespace: `Assimalign.Cohesion.Database.Documents.Language`.

## Signature

```csharp
public enum OqlNullPlacement
{
    Default,
    First,
    Last
}
```

## Behavior and exceptions

Default resolves to First for ASC and Last for DESC. First/Last places the entire MISSING/NULL
block; ASC orders MISSING before NULL, DESC orders NULL before MISSING. Reading a value throws
no exception. Planning an OqlOrdering with an invalid enum value raises DatabaseException with
CD01032, without an assigned SQLSTATE. See [ORDER BY](../../../../database/document/language/clauses/order-by.md).

## Example

```csharp
var select = (OqlSelectExpression)((OqlQueryStatement)new OqlQueryParser().Parse(
    "SELECT v FROM c ORDER BY v DESC NULLS FIRST")).OqlExpression;
Console.WriteLine(select.OrderBy[0].NullPlacement); // First
Console.WriteLine(select.OrderBy[0].Descending);    // True
```

## Related reference

[Assembly](index.md) · [OqlSelectExpression](oql-select-expression.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/OqlNullPlacement.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlOrderingParserTests.cs`.
