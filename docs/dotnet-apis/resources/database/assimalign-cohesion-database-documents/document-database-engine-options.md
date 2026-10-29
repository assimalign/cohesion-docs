# DocumentDatabaseEngineOptions

Values copied when the Documents engine is created or built.

Namespace: `Assimalign.Cohesion.Database.Documents`.

## StatementMemoryBudget

```csharp
public long StatementMemoryBudget { get; set; } = 64L * 1024 * 1024;
```

This positive byte budget estimates retained statement memory. SELECT source rows (including JSON,
identifiers, raw star bytes and expanded bindings), catalog/sort staging, group keys/membership,
native accumulator state, DISTINCT sets, ARRAY_AGG inputs/results and projection/order storage
share it with GROUP AS wrappers/arrays, DML pending buffers and RETURNING. INSERT/UPSERT SELECT
sources and RETURNING use the same operation counter.
RETURNING covers metadata, row/value arrays (including normalization/result copies), scalars,
strings, JSON clones and preserved whole-document
bytes. A clone costs 128 bytes plus its UTF-8 length plus 12 bytes per token, including property names
and both container delimiters. A lone star retains one clone and one raw image; each is charged once.
The estimate measures JSON without allocating a UTF-16 text copy. Input and owned ARRAY_AGG result
representations are charged separately while both live. Reservations release with their owning
source/group, scratch, pending-buffer or result lifetime and on every failure/cancellation exit.
Application callback-owned state is excluded; this is not a managed-heap measurement.

Engine creation rejects nonpositive values with ArgumentOutOfRangeException. Exhaustion during
SELECT, aggregate evaluation or RETURNING raises DatabaseException with CD05018 before result
publication and rolls back statement writes; an explicit transaction requires rollback after the
failure. DML pending buffers are completely staged before mutation, so pending-buffer exhaustion
refuses before the first write. Serialization preflight also enforces 16 MiB frames (CD05016)
and 256-level result depth (CD05019) inside the transaction. These codes have no assigned SQLSTATE.

## StatementWorkBudget

```csharp
public long StatementWorkBudget { get; set; } = 10_000_000;
```

The positive count bounds reached expression evaluations in statements containing subqueries or
expression sources. All depths and cache hits share it; unreached branches spend no work. Engine
creation throws ArgumentOutOfRangeException with ParamName StatementWorkBudget for a nonpositive value.
The engine copies the option at construction. Exhaustion raises DatabaseException with CD05020
(no assigned SQLSTATE), before the next evaluation, and rolls back statement writes.

Adapted from `DocumentCorrelatedSubqueryTests.Create_WorkBudget_ShouldValidateAndSnapshotOptionAsync`:

```csharp
var options = new DocumentDatabaseEngineOptions { StatementWorkBudget = 16 };
await using var engine = DocumentDatabaseEngine.Create("frozen-work", options);
options.StatementWorkBudget = 32;
```

The engine continues using the copied limit of 16 evaluations.

Cached arrays have statement lifetime; uncached nested scratch releases with the consuming
expression/row. Retained projections and keys own separate charged values under
StatementMemoryBudget. See [subquery evaluation](../../../../database/document/language/expressions/subqueries.md#evaluation).

## Example

Adapted from `DocumentReturningBudgetTests.ExecuteAsync_ExactReturningBudget_ShouldCountEachOwnedRepresentationOnce`:

```csharp
using Assimalign.Cohesion.Database.Documents;

// DELETE RETURNING *: two 1,024-byte images with five-character identifiers.
// 208 metadata + 2 * (224 row + 1,200 clone + 1,024 raw + 106 pending identity).
var options = new DocumentDatabaseEngineOptions { StatementMemoryBudget = 5_316 };
await using var engine = DocumentDatabaseEngine.Create("budget", options);
```

The DELETE test admits both RETURNING * rows at 5,316 bytes. At 5,315 bytes it fails with CD05018
and restores the original documents and versions. INSERT/UPSERT and UPDATE also charge pending
encoded or patched bytes, so their limits differ.
See [RETURNING](../../../../database/document/language/clauses/returning.md).

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentCorrelatedSubqueryTests.cs`.

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/src/DocumentDatabaseEngineOptions.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/src/DocumentDatabaseEngine.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentReturningBudgetTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentStatementMemoryTests.cs` and `DocumentQueryTests.Aggregates.cs`.
