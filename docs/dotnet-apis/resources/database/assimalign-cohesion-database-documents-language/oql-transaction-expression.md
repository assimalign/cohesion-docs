# OqlTransactionExpression

A session-owned OQL transaction control AST.

Namespace: `Assimalign.Cohesion.Database.Documents.Language`.

## Signature

```csharp
public sealed class OqlTransactionExpression : OqlExpression
{
    public OqlTransactionExpression(OqlTransactionCommand command,
        OqlTransactionIsolation isolation = OqlTransactionIsolation.Snapshot,
        Location? location = null);
    public OqlTransactionCommand Command { get; }
    public OqlTransactionIsolation Isolation { get; }
}
```

## Behavior and exceptions

Construction validates both enums and throws ArgumentOutOfRangeException for undefined values.
Isolation applies to Begin and is ignored for Commit/Rollback. Location is an optional span with
an exclusive end offset. Parsed nodes retain source Text, command and isolation.

The owning in-process session dispatches controls before ordinary planning. Documents servers
refuse them with CD07002 in this batch. See [transaction behavior and diagnostics](../../../../database/document/language/statements/transactions.md).

## Example

```csharp
var statement = (OqlQueryStatement)new OqlQueryParser().Parse(
    "START WORK ISOLATION LEVEL READ COMMITTED;");
var control = (OqlTransactionExpression)statement.OqlExpression;
Console.WriteLine(control.Command); // Begin
Console.WriteLine(control.Isolation); // ReadCommitted
```

## Related reference

[Assembly](index.md) · [OqlTransactionCommand](oql-transaction-command.md) · [OqlTransactionIsolation](oql-transaction-isolation.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlTransactionParserTests.cs`.
