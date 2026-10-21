# Assimalign.Cohesion.Database.Execution — Design

The model-neutral request/result vocabulary (area architecture:
`cohesion/docs/resources/Database/DESIGN.md` `3.2).
This package references `Database.Types` only. It has no direct or transitive dependency on
`Database.Language`, and it owns no planner, executor, parser or transaction pipeline.

## Request and result families

`DatabaseRequest` is a memberless abstract root. A model defines the operations it accepts:
KeyValuePair requests carry command operands; `SqlQueryRequest` and `DocumentQueryRequest`
own their typed `Statement` and `Parameters`; `GraphRequest` owns those values for the row
and path request variants. There is no shared statement property or request-kind discriminator.

`DatabaseResult` reports status, affected count and non-null diagnostics. `DatabaseResultSet` adds
columns and streaming `DatabaseRow` values. `DatabaseColumn` describes a column, including
optional `MaxLength`: the declared character count or binary byte limit, not an individual
value's size. SQL uses it with the shared `Character` identity for `CHAR(n)` metadata; its
execution and padding rules remain in the SQL model.

The owner approved all five result-family renames on 2026-10-16 (#1576, #1579).
The public names are `DatabaseResult`, `DatabaseResultSet`, `DatabaseResultStatus`,
`DatabaseRow` and `DatabaseColumn`; they all remain in this assembly. There are no aliases
or compatibility shims. Model-specific language results retain names such as `SqlQueryResult`,
`DocumentQueryResult` and `GraphPathsQueryResult`.

### Result diagnostics

```csharp
// DatabaseResult
public abstract IReadOnlyList<Diagnostic> Diagnostics { get; }
```

SQL, Documents, Graph and Key-Value Pair command and row results return an empty collection
when there is nothing to report. GraphPathsQueryResult follows the same rule for paths, as
GraphSchemaResult<T> already does for schema reads. Callers can enumerate Diagnostics or test
Count without a null check. Reading this property introduces no new exception. Blob uses its
own stream/storage operations; Cache has no implemented result surface.

The exception carriers DatabaseException and each model's remote client exception also expose
non-null diagnostics, possibly empty. This collection contract adds no wire frame or diagnostic
code. Remote result types retain their own public members; see the model client references.

## Session capabilities

The capability bases live in the root `Database` assembly, which references this package:

| Base | Capability | Models |
|---|---|---|
| `DatabaseSession` | Lifecycle, state and explicit transactions | Every model, including Blob |
| `DatabaseExecutionSession<TRequest>` | Typed `ExecuteAsync(TRequest, CancellationToken)` | Sql, Documents, Graph and KeyValuePair |
| `DatabaseLanguageSession<TRequest>` | Typed execution plus `ExecuteAsync(string, parameters, CancellationToken)` | Sql, Documents and Graph |

Both generic bases constrain `TRequest` to `DatabaseRequest`. Public execution members are
non-virtual and perform validation and tracing before calling protected abstract cores.
The language base declares its typed forwarder before its text member and forwards through
the base's public NVI. Consequently a foreign request fails compilation with the model's
request type in the diagnostic. A plain root session or Blob session has no execute member.
KeyValuePair's command parser is an internal wire entry, not a public language capability.

## Decisions and remaining work

- `Diagnostic`, `DiagnosticSeverity` and `DiagnosticLocation` moved unchanged to
  `Database.Types`. DatabaseResult.Diagnostics and DatabaseException.Diagnostics are non-null.
- There is no shared execution pipeline. The unused pipeline, transaction-scope and executor
  abstractions were deleted under #1257. Models own their execution and transaction brackets.
- There is no exception root here. Model failures use the root assembly's `DatabaseException`
  family; this lower package cannot reference that root.
- Text execution retains its existing validation, refusal and tracing order. Parsing every
  language request before entering the traced execution member is deferred to separate work.
- Streaming remains the model's responsibility: `DatabaseResultSet.GetRowsAsync` does not
  introduce shared buffering or choose a model's memory budget.

## Plan descriptions and MISSING

[DatabasePlanDescription](database-plan-description.md) materializes model plan nodes in the shared
eight-column shape without defining a shared operator algebra. Nodes retain model vocabulary;
estimated/actual rows remain NULL. EXPLAIN performs normal binding/admission without data execution.
[DatabaseRow](database-row.md) has an opt-in MISSING capability used only by Documents rows;
unsupported models throw NotSupportedException from IsMissing. OQL UPDATE descriptions use an Update
root with CollectionScan/IndexSeek access. GQL ordering/paging adds SortPage (offset/limit) and SortKey
(expression/direction/NULL placement). These preserve the existing public node/result signatures.

## AOT posture

Abstract request/result contracts, a sealed column descriptor and ordinary generic session
bases require no reflection, expression trees, runtime code generation or generic virtual method.

[Assembly overview](index.md) · [Examples](examples/index.md)

## Sources

- **Result contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Execution/src/DatabaseResult.cs`.
- **Model evidence** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlNullOrderingTests.cs`, `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentQueryTests.cs`, `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphPathsQueryTests.cs` and `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/tests/KeyValueCommandTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Execution/docs/DESIGN.md`.
