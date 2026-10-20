# Database ownership example

The root keeps model-independent ownership vocabulary. Schema compilation and migration types
belong to the SQL schema package; the former ProvisioningTests.cs example is replaced by the
current DatabaseObjectOwnershipTests cases.

## Example

```csharp
var refusal = new DatabaseObjectLockedException("Customers", "AppSchema", "DROP TABLE");
Console.WriteLine(refusal.ObjectName);   // Customers
Console.WriteLine(refusal.OwningSchema); // AppSchema
Console.WriteLine(refusal.Operation);    // DROP TABLE
Console.WriteLine((byte)DatabaseObjectOwner.Adhoc);  // 0
Console.WriteLine((byte)DatabaseObjectOwner.Schema); // 1
```

The refusal is a DatabaseException. Its default message directs the caller to change the owning
schema and redeploy it. The overload accepting a custom message preserves that text and rejects a
blank message with ArgumentException. Diagnostic overloads copy supplied metadata and carry the
refusal text as an Error diagnostic.

[All examples](index.md) · [Assembly overview](../index.md)

## Sources

- **Ownership cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database/tests/DatabaseObjectOwnershipTests.cs`.
