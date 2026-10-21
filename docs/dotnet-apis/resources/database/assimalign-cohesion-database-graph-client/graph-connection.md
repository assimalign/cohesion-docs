# GraphConnection

Namespace: `Assimalign.Cohesion.Database.Graph.Client`.

## Changed signatures

```csharp
public string Database { get; }
public bool IsOpen { get; }
public ValueTask<GraphResultSet> QueryAsync(string statement,
    IReadOnlyDictionary<string, object?>? parameters = null, CancellationToken cancellationToken = default);
public ValueTask<long> ExecuteAsync(string statement,
    IReadOnlyDictionary<string, object?>? parameters = null, CancellationToken cancellationToken = default);
public GraphPathResult QueryPathsAsync(string statement,
    IReadOnlyDictionary<string, object?>? parameters = null, CancellationToken cancellationToken = default);
public ValueTask AbortAsync();
public ValueTask DisposeAsync();
```

## Behavior and exceptions

One authenticated rental runs one active exchange. QueryAsync materializes value/SHOW/EXPLAIN rows;
ExecuteAsync returns the mutation count, including when it discards RETURN rows, or -1 for a
read-only statement. QueryPathsAsync streams one bound path,
node or relationship from read-only composition. Parameters preserve runtime scalar, list and map types and bind
case-sensitive bare names in WHERE, RETURN and maps. Missing values, collisions and invalid types reach the
same engine diagnostics (COHDBG001/014/003) as embedded requests.

RETURN accepts standalone and computed/aliased scalar expressions and DISTINCT/ALL. QueryAsync
and ExecuteAsync use wire Execute, which rejects possible entity-valued results with COHDBG001
and an ExecutePaths direction before reads, writer locks or writes, including empty matches.
EXPLAIN remains scalar for entity projections. Final column names must be unique ordinally;
duplicates fail COHDBG015 before output or mutation, including implicit/explicit collisions and
EXPLAIN (no SQLSTATE assigned). QueryPathsAsync accepts one bare entity/path binding from read-only
composition, optionally DISTINCT, or star over one named binding. Invalid computed/multiple/star shapes
use COHDBG001 before duplicate-name checks.

[SET](../../../../database/graph/language/clauses/set.md) and
[REMOVE](../../../../database/graph/language/clauses/remove.md) return updated scalar values
through QueryAsync with GraphResultSet.AffectedCount. The count includes distinct changed
entities; repeated rows do not multiply it, and pure no-ops or OPTIONAL NULL targets count zero.

LET/FILTER/FOR/UNWIND, WITH and RETURN … NEXT run as eager stages. WITH WHERE filters after
paging. Only final columns determine wire admission; intermediate collect/UNWIND preserves proven
entity kinds. Any write stage excludes QueryPathsAsync. Final node/relationship lists remain
COHDBG001 (#1662), even with graph.composite-values. See [stage rules](../../../../database/graph/language/clauses/next.md).

Protocol 2.1 negotiates [graph.composite-values](../assimalign-cohesion-database-graph-protocol/graph-protocol.md).
Lists decode as object?[]; maps as read-only ordinal dictionaries. Values permit depth 64,
65,536 total values including root and 16 MiB encoded bytes; rows also fit 16 MiB. Without the
capability, composite parameters throw GraphClientException with ExecutionFailure locally before
sending; the rental stays reusable. Possible composite results fail with COHDBG016 before reads,
locks or writes. Invalid/oversized parameter encoding throws DatabaseTypeException locally;
server parameter validation uses COHDBG003. Constructed value/row limits use COHDBG013 before
commit, rolling back statement writes. Stored pattern properties stay scalar-only.

Blank statements throw ArgumentException; disposed rentals ObjectDisposedException; overlapping
exchanges InvalidOperationException. Unfinished cancellation throws OperationCanceledException and
discards the connection. GraphClientException retains server category and diagnostics. A terminal
rejection before scalar/path output preserves reuse. Early path disposal discards it. Explicit wire
transactions are unsupported.

Path delivery decodes each frame once and uses a one-slot queue in wire order. Retained paths
remain valid after later reads and disposal. Fully consuming the stream verifies the terminal
count and joins the producer; early disposal cancels and joins it before releasing the rental.
See [path lifetime and counters](design.md#path-lifetime).

Database and the negotiated composite-value flag are captured when the wrapper receives its shared
lease. Database remains readable after disposal and IsOpen returns false. After a failed or abandoned
path stream returns that lease, subsequent queries fail as disposed while retaining operation
telemetry. AbortAsync then joins the original shared return and cannot affect a later renter.
The shared pool does not reset explicit transactions (known gap #1540): end or roll back any
explicit transaction before returning its connection. Graph wire transactions remain unsupported.

## Example

```csharp
var bindings = new Dictionary<string, object?> { ["name"] = "Ada", ["weight"] = 2.5m };
await connection.ExecuteAsync("INSERT (:P {name:$name})-[:T {weight:@weight}]->(:P)", bindings);
var rows = await connection.QueryAsync(
    "MATCH (a:P {name:@name})-[r:T]->(b) WHERE r.weight = $weight RETURN a.name, r.weight", bindings);
Console.WriteLine(rows[0][0]); // Ada; weight remains Decimal 2.5
var computed = await connection.QueryAsync("RETURN 1 + 1 AS two, $p AS p",
    new Dictionary<string, object?> { ["p"] = 5 });
Console.WriteLine(computed[0][0]); // 2L
Console.WriteLine(computed[0][1]); // 5 (Int32)
```

## Related reference

[GraphPathResult](graph-path-result.md) · [GraphResultSet](graph-result-set.md) · [Parameters](../../../../database/graph/language/expressions/parameters.md)

## Queued terminal errors

Before the next admitted operation writes a request, the shared connector checks buffered input or
a closed transport and consumes a queued terminal Error once. Its original code reaches
GraphClientException and the connector is discarded. This also applies to streaming startup;
IsOpen continues to describe physical transport health. Unexpected/malformed terminal frames use
ProtocolViolation; clean EOF without an Error remains Internal. Rent-time pings are not implemented.
A clean handshake EOF has its own message naming the startup handshake. See
[shared admission](../assimalign-cohesion-database-client/database-connection.md#queued-terminal-errors)
and [terminal delivery/TLS limits](../assimalign-cohesion-database-protocol/design.md#terminal-delivery-and-frame-writes).

## Sources

- **Terminal client conformance** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Client/tests/GraphClientTerminalErrorTests.cs`.

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Client/src/GraphConnection.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Client/tests/GraphClientWireTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphParameterTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Client/tests/GraphReturnWireTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Client/tests/GraphClientEventSourceTests.cs`.
