# GqlProjection

A projected value expression with an optional result-column alias.

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public sealed record GqlProjection(GqlExpression Expression, string? Alias = null);
```

## Behavior and exceptions

This replaces the former variable/property tuple constructor. Construct a
[GqlVariableExpression](gql-variable-expression.md) for a bare binding, GqlPropertyExpression for
a property, or any executable value expression, including CASE/COALESCE/NULLIF and list/map constructors. RETURN shares WHERE's evaluator. The record
stores values without validating them; the planner rejects a missing expression with COHDBG001.

Alias names the column exactly. Without an alias, parsed expressions use normalized source text;
source-free expressions use rendered syntax, including parentheses around binary expressions.
Names compare ordinally and case-sensitively. Duplicate implicit/explicit names report COHDBG015
at the second expression before reads or mutations, even for empty matches or EXPLAIN; no SQLSTATE
is assigned. Aliases do not bind neighboring expressions.

Embedded row requests can return entities/paths. Wire Execute accepts scalar and negotiated composite kinds
before reads, writer locks or writes; COHDBG001 directs entity results to ExecutePaths. A path
request requires exactly one bare binding; invalid shapes retain COHDBG001 before duplicate-name
validation. Evaluated value/access/limit errors use COHDBG013; path property access uses COHDBG003.
Possible composite results need graph.composite-values (COHDBG016 before reads/writes without it).
See [list values and limits](../../../../database/graph/language/expressions/list-values.md).

## Example

```csharp
var expression = new GqlBinaryExpression(
    new GqlLiteralExpression(1L), "+", new GqlLiteralExpression(1L));
var query = new GqlQueryExpression([], null, [], [], false,
    [new GqlProjection(expression)]);
await using var result = (DatabaseResultSet)await session.ExecuteAsync(
    new GraphQueryRequest(new GqlQueryStatement(query)), CancellationToken.None);
// One row: 2L. The source-free column name is "(1 + 1)".
```

The example uses an open GraphDatabaseSession and the Graph, Graph.Language and Execution
namespaces. `new GqlProjection(expression, "two")` names the same output `two`.

## Related reference

- **Assembly** — [`Assimalign.Cohesion.Database.Graph.Language`](index.md).
- **Design** — [Lifetime and implementation decisions](design.md).
- **Examples** — [Source-backed usage](examples/index.md).
- **Query shape** — [GqlQueryExpression](gql-query-expression.md).
- **Language** — [RETURN](../../../../database/graph/language/clauses/return.md).

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/docs/Assembly/Assimalign.Cohesion.Database.Graph.Language/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlProjection.cs`.
- **Execution example** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphReturnTests.cs`.
