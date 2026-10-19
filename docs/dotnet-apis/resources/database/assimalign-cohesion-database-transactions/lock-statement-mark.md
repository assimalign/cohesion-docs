# LockStatementMark

Identifies an owner's existing lock grants before an unwritten statement acquires resources.

Namespace: `Assimalign.Cohesion.Database.Transactions`.

## Signature

```csharp
public readonly struct LockStatementMark : IEquatable<LockStatementMark>
{
    public bool Equals(LockStatementMark other);
    public override bool Equals(object? obj);
    public override int GetHashCode();
    public static bool operator ==(LockStatementMark left, LockStatementMark right);
    public static bool operator !=(LockStatementMark left, LockStatementMark right);
}
```

## Behavior and exceptions

Obtain this opaque, allocation-free value through [LockManager.MarkStatement(owner)](lock-manager.md#statement-marks).
It has no public constructor taking arguments or public grant properties. Equality compares manager
identity, transaction owner and grant boundary; equal values have equal hash codes. Two default
values compare equal. Equality operations add no validation exceptions.

Pass it to LockManager.ReleaseSince only within that transaction's lifetime, before any writes
since the mark and with no pending requests. The manager rejects default marks and marks from
another manager with ArgumentException. Earlier resources retain all modes, including upgrades
taken after the mark. To preserve a completed row wait across arbitration restarts, take a new mark
after the row grant. SQL retains those waited rows until transaction end on success; a separate
original statement mark permits cleanup of all new grants on preparation failure.

## Example

From LockStatementMarkTests.Equals_SameBoundary_ShouldHaveValueSemantics:

```csharp
using Assimalign.Cohesion.Database.Transactions;

var locks = LockManager.Create();
var owner = new TransactionSequence(1);
var first = locks.MarkStatement(owner);
var same = locks.MarkStatement(owner);
Console.WriteLine(first == same); // True: same manager, owner and boundary
locks.TryAcquire(owner, LockResource.Entry(1, 1), LockMode.Exclusive);
var later = locks.MarkStatement(owner);
Console.WriteLine(first != later); // True: a new grant advanced the boundary
locks.ReleaseAll(owner);
```

## Related reference

[Assembly](index.md) · [LockManager](lock-manager.md) · [Design](design.md#unwritten-statement-marks) · [Examples](examples/index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Transactions/src/ValueObjects/LockStatementMark.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Transactions/tests/LockStatementMarkTests.cs`.
