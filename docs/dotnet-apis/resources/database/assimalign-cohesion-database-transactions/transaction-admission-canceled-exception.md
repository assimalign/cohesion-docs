# TransactionAdmissionCanceledException

A journal checkpoint admission wait was canceled, or admission refused while retaining metadata.

Namespace: `Assimalign.Cohesion.Database.Transactions`.

## Signature

```csharp
public sealed class TransactionAdmissionCanceledException : OperationCanceledException
{
}
```

## Behavior

The kernel creates this exception; it exposes no public constructor. Caller cancellation retains
its CancellationToken. Models distinguish that cancellation from statement-deadline expiry or a
non-waiting refusal. SQL translates the latter conditions to CK04014 / SQLSTATE 57014 and keeps
the statement atomic. See [interlock admission](transaction-checkpoint-interlock.md#journal-admission).

## Related reference

[Assembly](index.md) · [Design](design.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Transactions/src/Exceptions/TransactionAdmissionCanceledException.cs`.
