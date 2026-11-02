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
    public string? SavepointName { get; }
}
```

## Behavior and exceptions

Construction validates both enums and throws ArgumentOutOfRangeException for undefined values.
Named commands require parser construction: the public constructor has no name argument and
throws ArgumentException for Savepoint, ReleaseSavepoint or RollbackToSavepoint. SavepointName
is null for other commands and retains the decoded identifier for named controls; unquoted
names preserve case and compare exactly. ToString quotes the name and doubles embedded quotes.
Isolation applies to Begin and is ignored for other controls. Location is an optional span with
an exclusive end offset. Parsed nodes retain source Text, command and isolation.

The owning session dispatches controls before ordinary planning, in process and over the wire.
CD07002 refuses controls only without negotiated transaction-status. See [transaction behavior and diagnostics](../../../../database/document/language/statements/transactions.md).
Named controls use CK04012 outside a block, CK04013 for an unknown name and CK04009 for
incomplete undo (no SQLSTATE). An ordinary failed statement still aborts the transaction and
ROLLBACK TO cannot recover it (CD04101); whole ROLLBACK is required.

## Example

```csharp
var statement = (OqlQueryStatement)new OqlQueryParser().Parse(
    "START WORK ISOLATION LEVEL READ COMMITTED;");
var control = (OqlTransactionExpression)statement.OqlExpression;
Console.WriteLine(control.Command); // Begin
Console.WriteLine(control.Isolation); // ReadCommitted
```

```csharp
var named = (OqlTransactionExpression)((OqlQueryStatement)new OqlQueryParser().Parse(
    "ROLLBACK TRAN TO SAVEPOINT Keep;")).OqlExpression;
Console.WriteLine(named.Command);       // RollbackToSavepoint
Console.WriteLine(named.SavepointName); // Keep
```

Checked against SavepointConformanceTests; parsing changes no transaction state.

## Related reference

[Assembly](index.md) · [OqlTransactionCommand](oql-transaction-command.md) · [OqlTransactionIsolation](oql-transaction-isolation.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlTransactionParserTests.cs`.
