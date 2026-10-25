# DatabaseProtocolExchange<TResult>

Namespace: `Assimalign.Cohesion.Database.Client`.

The abstract exchange base exposes these protected constructors:

```csharp
protected DatabaseProtocolExchange(ProtocolMessageFamily family);
protected DatabaseProtocolExchange(ProtocolMessageFamily family, bool transactionStatus);
```

The connector detects written Execute (5) and ExecutePaths (64) frames and automatically consumes
their negotiated transaction-status Ready postamble. Application exchanges using the one-argument
constructor do not opt in or consume Ready themselves. `transactionStatus: true` can still require
the postamble explicitly. Exchanges consume the full model response and use `MarkResponseComplete`
before throwing a complete statement error; successful return certifies model-response completion.
The public `Family` property and protected `ExecuteCoreAsync`/`MarkResponseComplete` contract remain.

After a complete result or statement error, the connector drains status with a five-second internal
token. Caller cancellation in that window preserves an acknowledged result, including a completed
COMMIT. Status-read failure discards the connector; an original statement exception is preserved.
See [pool reset](design.md#transaction-status-and-pool-reset).

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Client/src/DatabaseProtocolExchange.cs`.
