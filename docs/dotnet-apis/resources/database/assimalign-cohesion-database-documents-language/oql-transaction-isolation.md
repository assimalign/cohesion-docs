# OqlTransactionIsolation

Isolation requested by an OQL transaction start.

Namespace: `Assimalign.Cohesion.Database.Documents.Language`.

## Signature

```csharp
public enum OqlTransactionIsolation { Snapshot, ReadCommitted }
```

## Behavior and exceptions

Snapshot is the default and retains one transaction snapshot. ReadCommitted takes a fresh
committed snapshot per statement. Explicit ISOLATION LEVEL SNAPSHOT is a Cohesion extension.
The property is ignored for COMMIT/ROLLBACK. Enum access has no exceptions; undefined values
are refused by OqlTransactionExpression.

## Example

```csharp
var statement = (OqlQueryStatement)new OqlQueryParser().Parse("BEGIN");
Console.WriteLine(((OqlTransactionExpression)statement.OqlExpression).Isolation); // Snapshot
```

## Related reference

[Assembly](index.md) · [OqlTransactionExpression](oql-transaction-expression.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlTransactionParserTests.cs`.
