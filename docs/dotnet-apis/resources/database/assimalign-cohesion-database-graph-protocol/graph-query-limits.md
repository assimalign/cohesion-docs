# GraphQueryLimits

Namespace: `Assimalign.Cohesion.Database.Graph.Protocol`.

```csharp
public sealed class GraphQueryLimits
{
    public long? MemoryBytes { get; init; }
    public long? WorkUnits { get; init; }
}
```

Omitted members inherit engine ceilings. Supplied limits must be positive and no greater than the engine ceiling; invalid requests fail CG08021 during execution admission. The DTO stores values without constructor validation. Embedded row/path requests accept limits overloads, and GraphTraversal.Limits applies them to typed traversal. These limits are not serialized in Execute payloads. Memory exhaustion is CG05020; physical-work exhaustion is CG05004.

[Engine configuration](../assimalign-cohesion-database-graph/graph-database-engine-options.md) · [Protocol assembly](index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Protocol/src/ValueObjects/GraphQueryLimits.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/GraphDatabaseSession.cs`.
