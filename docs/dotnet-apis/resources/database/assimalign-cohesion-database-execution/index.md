# Assimalign.Cohesion.Database.Execution

The execution package defines query requests, results, contexts, pipelines, and transaction scopes.

> **Status:** Partial.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.

The model-agnostic execution substrate of the Cohesion Data Platform: the request/result families
every engine speaks (`QueryRequest`, `QueryResult`, `QueryResultSet`, `QueryRow`, `QueryColumn`
), the per-execution context (`QueryExecutionContext`), the composable pipeline (`IQueryPipeline`,
`IQueryPipelineStage`, `QueryPipelineBuilder`), and the transaction boundary seam
(`IQueryTransactionScope`) whose semantics the pipeline enforces.

## Scope

- **Request/result contracts** — abstract families model engines subclass; plus the
  concrete `QueryStatementResult` for non-row-returning statements.
- **Execution context** — request, transaction scope, request-abort token,
  diagnostics accumulation, and an item bag for engine state between stages.
- **Pipeline** — middleware-shaped stages composed around a terminal executor;
  stages observe, wrap, or short-circuit execution (retry, tracing, timeouts,
  plan caching).
- **Transaction boundaries** — implicit (auto-commit) scopes commit on success and
  roll back on failure/exception/cancellation, with errors propagating after the
  rollback; explicit scopes belong to their session.

## Dependencies

`Database.Language` (statements, diagnostics) and `Database.Types`. Deliberately **below** the area
contract root — the root's session surface (`IDatabaseSession.ExecuteAsync`) is typed in this
project's terms, so nothing here may reference root types (transaction identity, engine contracts).
Engines adapt their transaction manager to `IQueryTransactionScope`.

## Usage

See the [source-backed usage examples](examples/index.md).

See [DESIGN.md](design.md) for the boundary rules and the decisions behind them.

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Database.Language` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Types` | `CohesionProjectReference` |

[Parent: Database](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Execution/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Execution/src/Assimalign.Cohesion.Database.Execution.csproj`.
