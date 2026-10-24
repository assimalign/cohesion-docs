# DatabaseProtocolExchange<TResult>

Namespace: `Assimalign.Cohesion.Database.Client`.

The abstract exchange base retains its protected family constructor and adds:

```csharp
protected DatabaseProtocolExchange(ProtocolMessageFamily family, bool transactionStatus);
```

transactionStatus marks an Execute exchange that expects the negotiated transaction-status Ready postamble. Model exchanges mark their terminal response complete before returning or throwing a complete statement error; the connector drains status afterward. The public Family property and protected ExecuteCoreAsync/MarkResponseComplete contract remain. See [pool reset](design.md#transaction-status-and-pool-reset).

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Client/src/DatabaseProtocolExchange.cs`.
