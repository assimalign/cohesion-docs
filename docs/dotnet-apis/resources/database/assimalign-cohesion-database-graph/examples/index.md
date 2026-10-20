# Examples

Examples for `Assimalign.Cohesion.Database.Graph` are derived from source documentation and code.

> **Status:** Partial.

The examples below retain real usage from project documentation, templates, fixtures, or tests. Test
examples require their original test project’s dependencies and supporting objects.

- **[Graph Catalog Database Scope Tests](graph-catalog-database-scope-tests.md)** — This example exercises `Assimalign.Cohesion.Database.Graph` through its co-located test source.
- **[Graph Database Scope Tests](graph-database-scope-tests.md)** — This example exercises `Assimalign.Cohesion.Database.Graph` through its co-located test source.
- **[Insert and query a graph](insert-and-query.md)** — `Create` graph data, define a node-property index, and execute a graph query.

[Assembly overview](../index.md) · [Design](../design.md)

## Additional reference pages

- **[Examples](catalog.md)** — Reference and usage.
- **[Graph Catalog Tests](graph-catalog-tests.md)** — Reference and usage.
- **[Graph storage: create, update and snapshot reads](graph-store-tests.md)** — Reference and usage.
- **[Examples](storage.md)** — Reference and usage.

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/docs/OVERVIEW.md`.

## Conditional and composite values

[GraphValueCodec](../../assimalign-cohesion-database-graph-protocol/graph-value-codec.md#example) round-trips nested Decimal/unsigned values.
[GraphProtocol](../../assimalign-cohesion-database-graph-protocol/graph-protocol.md#example) shows the capability request;
[execute](../../assimalign-cohesion-database-graph-protocol/graph-protocol-execute-message.md#example) and [header](../../assimalign-cohesion-database-graph-protocol/graph-protocol-result-header-message.md#example)
examples show the negotiated component/column formats. These follow GraphValueCodecTests and
GraphCompositeWireTests.
