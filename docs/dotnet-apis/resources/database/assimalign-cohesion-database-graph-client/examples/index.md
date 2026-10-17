# Examples

Examples for `Assimalign.Cohesion.Database.Graph.Client` are derived from source documentation and code.

> **Status:** Partial.

The examples below retain real usage from project documentation, templates, fixtures, or tests. Test
examples require their original test project’s dependencies and supporting objects.

- **[Graph Client Failure Tests](graph-client-failure-tests.md)** — This example exercises `Assimalign.Cohesion.Database.Graph.Client` through its co-located test source.
- **[Graph Client Wire Tests](graph-client-wire-tests.md)** — This example exercises `Assimalign.Cohesion.Database.Graph.Client` through its co-located test source.
- **[Graph Path Wire Tests](graph-path-wire-tests.md)** — This example exercises `Assimalign.Cohesion.Database.Graph.Client` through its co-located test source.

[Assembly overview](../index.md) · [Design](../design.md)

## T1 API examples

[GraphConnection](../graph-connection.md) demonstrates typed scalar parameters and
[GraphPathResult](../graph-path-result.md) demonstrates completion warnings after EOF.
Pre-output terminal refusals retain the session; early disposal still discards it.

Computed/standalone RETURN and parameter runtime types appear in the
[GraphConnection example](../graph-connection.md#example). [RETURN](../../../../../database/graph/language/clauses/return.md#examples)
and [INSERT](../../../../../database/graph/language/statements/insert.md#examples) show DISTINCT,
star, duplicate-name diagnostics and the pre-write scalar gate, checked against GraphReturnWireTests.

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Client/docs/OVERVIEW.md`.
