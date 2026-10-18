# GraphPathResult

A single-use asynchronous path stream with negotiated completion diagnostics.

Namespace: `Assimalign.Cohesion.Database.Graph.Client`.

## Signature

```csharp
public sealed class GraphPathResult : IAsyncEnumerable<GraphPath>
{
    public IReadOnlyList<ProtocolDiagnostic> Diagnostics { get; }
    public IAsyncEnumerator<GraphPath> GetAsyncEnumerator(CancellationToken cancellationToken = default);
}
// GraphConnection.QueryPathsAsync returns GraphPathResult.
```

## Behavior and exceptions

Construction is internal. Enumerate once to EOF before reading final Diagnostics. The collection is
empty until diagnostics arrive or when none were negotiated. EOF verifies terminal path count and
releases the exchange. Unknown-label/type reads may return zero paths and warnings COHDBG010/011. A
second enumeration throws InvalidOperationException. Early disposal/cancellation discards the
unfinished connection. A terminal parse/planning refusal before any path output preserves the
connection and throws GraphClientException carrying diagnostics. Transport and malformed-response
failures still discard it. This changes the QueryPathsAsync return type while preserving await
foreach.

QueryPathsAsync also accepts DISTINCT over one bare bound entity/path, or RETURN * over one named
binding. Computed/multiple and multi-binding star shapes fail COHDBG001 before path output and
before duplicate-name checks; completion and reuse rules stay the same.

A projected binding supplied only by OPTIONAL MATCH is refused during planning with COHDBG001,
even when every actual row would match; a mandatory clause must bind or constrain it. For example,
`OPTIONAL MATCH (n:Person) RETURN n` is refused, while
`MATCH (n:Person) OPTIONAL MATCH (n)-[:KNOWS]->(b) RETURN n` can stream n paths.

Protocol 2.1's graph.composite-values capability changes parameter encoding for path requests,
including list-valued WHERE parameters, without changing Path payloads or this result type.
For example, MATCH (n:P) WHERE n.k IN $xs RETURN n can stream paths with a list parameter.
See [GraphConnection](graph-connection.md) for parameter limits and local capability failures.

## Example

```csharp
var result = connection.QueryPathsAsync("MATCH (n:Absent) RETURN n");
await foreach (var path in result) { /* consume paths */ }
foreach (var warning in result.Diagnostics) Console.WriteLine(warning.Code);
// With no Absent label: zero paths, then COHDBG010 on negotiated diagnostics.
```

## Related reference

[GraphConnection](graph-connection.md) · [ProtocolDiagnostic](../assimalign-cohesion-database-protocol/protocol-diagnostic.md) · [Design](design.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Client/src/GraphPathResult.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Client/tests/GraphWireDiagnosticTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Client/tests/GraphOptionalMatchWireTests.cs`.
