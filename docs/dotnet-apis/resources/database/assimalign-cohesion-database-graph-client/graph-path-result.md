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
never null and is empty until diagnostics arrive, when nothing was reported or when none were
negotiated. EOF verifies terminal path count and
releases the exchange. Unknown-label/type reads may return zero paths and warnings CG01010/CG01011. A
second enumeration throws InvalidOperationException. Early disposal/cancellation discards the
unfinished connection. A terminal parse/planning refusal before any path output preserves the
connection and throws GraphClientException carrying diagnostics. Transport and malformed-response
failures still discard it. This changes the QueryPathsAsync return type while preserving await
foreach.

Paths arrive in wire order through a one-slot bounded queue, with each Path decoded once.
The producer waits before reading another frame when that queue is full. A following terminal
error therefore cannot overtake a queued path. Decoded values own their storage and remain
valid after later reads or disposal; no pooled payload remains held during queue waits.
An unfinished enumeration cancels and joins the producer before returning its failed rental.
The server still materializes its path result, and each path must fit one 16 MiB payload.
See [path performance and counters](design.md#path-performance-and-diagnostics).

QueryPathsAsync also accepts DISTINCT over one bare bound entity/path, or RETURN * over one named
binding. Computed/multiple and multi-binding star shapes fail CG05001 before path output and
before duplicate-name checks; completion and reuse rules stay the same.

The supported path query tail applies DISTINCT, [ORDER BY](../../../../database/graph/language/clauses/order-by.md),
OFFSET/SKIP and LIMIT before converting projected bindings to paths. For example,
`MATCH (a:Person) RETURN a ORDER BY a.age DESC SKIP 1 LIMIT 1` returns Isolated from the
[Person fixture](../../../../database/graph/language/clauses/order-by.md#examples).

A projected binding supplied only by OPTIONAL MATCH is refused during planning with CG05001,
even when every actual row would match; a mandatory clause must bind or constrain it. For example,
`OPTIONAL MATCH (n:Person) RETURN n` is refused, while
`MATCH (n:Person) OPTIONAL MATCH (n)-[:KNOWS]->(b) RETURN n` can stream n paths.

Protocol 2.1's graph.composite-values capability changes parameter encoding for path requests,
including list-valued WHERE parameters, without changing Path payloads or this result type.
For example, MATCH (n:P) WHERE n.k IN $xs RETURN n can stream paths with a list parameter.
See [GraphConnection](graph-connection.md) for parameter limits and local capability failures.

Final aggregate projections and explicit GROUP BY are rejected with CG05001 before any Path frame;
use QueryAsync for grouped rows. A decoded refusal preserves the pooled session. Path payload and
completion diagnostics stay the same.

Read-only [composed queries](../../../../database/graph/language/clauses/next.md) can carry a mandatory
entity/path through WITH or RETURN … NEXT. Intermediate collect followed by UNWIND retains proven
entity kinds, for example `MATCH (a:P) WITH collect(a) AS ns UNWIND ns AS n RETURN n` streams one
node path per P node. Any write stage or optional-only final entity is CG05001. Final lists of
nodes/relationships cannot be streamed as row values yet (CG05001; #1662); path payloads remain unchanged.

[Quantified paths](../../../../database/graph/language/patterns.md#quantified-paths) stream complete
source-ordered trails in existing Path frames, including intermediate nodes and zero-length paths.
Stored relationship directions survive right-side index anchors. A zero-length path has one node
and no relationships. Group lists can flow through FOR/UNWIND to scalar row results; possibly
nonempty entity lists remain CG05001 on Execute even after composite negotiation.

## Example

```csharp
var result = connection.QueryPathsAsync("MATCH (n:Absent) RETURN n");
await foreach (var path in result) { /* consume paths */ }
foreach (var warning in result.Diagnostics) Console.WriteLine(warning.Code);
// With no Absent label: zero paths, then CG01010 on negotiated diagnostics.
```

[SET](../../../../database/graph/language/clauses/set.md) and
[REMOVE](../../../../database/graph/language/clauses/remove.md) are write stages and therefore
refused by QueryPathsAsync/ExecutePaths with CG05001 before mutation. Use QueryAsync for
scalar RETURN rows and their affected count.

## Related reference

[GraphConnection](graph-connection.md) · [ProtocolDiagnostic](../assimalign-cohesion-database-protocol/protocol-diagnostic.md) · [Design](design.md)

## Sources

- **Ordered handoff and retained values** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Client/tests/GraphClientFailureTests.cs` and `GraphPathAllocationTests.cs` in the same directory.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Client/src/GraphPathResult.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Client/tests/GraphWireDiagnosticTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Client/tests/GraphOptionalMatchWireTests.cs`.
