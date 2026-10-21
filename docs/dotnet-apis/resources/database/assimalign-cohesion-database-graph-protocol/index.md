# Assimalign.Cohesion.Database.Graph.Protocol

Shared Graph wire messages, codecs and transferred values for engines and remote clients.

> **Status:** Implemented.

Namespace: `Assimalign.Cohesion.Database.Graph.Protocol`. Assembly: `Assimalign.Cohesion.Database.Graph.Protocol.dll`. NuGet package: `Assimalign.Cohesion.Database.Graph.Protocol`.

This package owns 13 public types formerly shipped in `Assimalign.Cohesion.Database.Graph`.
Both engine and client consume these same contracts and the single model family instance. Update
imports and package references and recompile. Namespaces and assembly identities change without
forwarders; wire bytes, disk formats, limits and validation behavior stay the same.

## Reference pages

- **[Design](design.md)** — Contract ownership and dependencies.
- **[Examples](examples/index.md)** — Codec use without an engine reference.
- **[GraphPath](graph-path.md)** — Public wire contract.
- **[GraphProtocol](graph-protocol.md)** — Public wire contract.
- **[GraphProtocolExecuteMessage](graph-protocol-execute-message.md)** — Public wire contract.
- **[GraphProtocolMessageType](graph-protocol-message-type.md)** — Public wire contract.
- **[GraphProtocolPathMessage](graph-protocol-path-message.md)** — Public wire contract.
- **[GraphProtocolPathsCompleteMessage](graph-protocol-paths-complete-message.md)** — Public wire contract.
- **[GraphProtocolResultCompleteMessage](graph-protocol-result-complete-message.md)** — Public wire contract.
- **[GraphProtocolResultHeaderMessage](graph-protocol-result-header-message.md)** — Public wire contract.
- **[GraphValueCodec](graph-value-codec.md)** — Public wire contract.
- **[GraphNode](graph-node.md)** — Public wire contract.
- **[GraphNodeId](graph-node-id.md)** — Public wire contract.
- **[GraphRelationship](graph-relationship.md)** — Public wire contract.
- **[GraphRelationshipId](graph-relationship-id.md)** — Public wire contract.

## Project references

| Reference | Kind |
| --- | --- |
| `Assimalign.Cohesion.Database.Protocol` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Types` | `CohesionProjectReference` |

[Engine](../assimalign-cohesion-database-graph/index.md) · [Client](../assimalign-cohesion-database-graph-client/index.md) · [Database](../index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Protocol/docs/OVERVIEW.md`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Protocol/src/Assimalign.Cohesion.Database.Graph.Protocol.csproj`.
