# DocumentDatabaseEngineOptions

Values copied when the Documents engine is created or built.

Namespace: `Assimalign.Cohesion.Database.Documents`.

## StatementMemoryBudget

```csharp
public long StatementMemoryBudget { get; set; } = 64L * 1024 * 1024;
```

This positive byte budget estimates retained RETURNING results. It covers metadata, row/value arrays
(including normalization/result copies), scalars, strings, JSON clones and preserved whole-document
bytes. A clone costs 128 bytes plus its UTF-8 length plus 12 bytes per token, including property names
and both container delimiters. A lone star retains one clone and one raw image; each is charged once.
The estimate measures JSON without allocating a UTF-16 text copy. Existing SELECT and mutation/source
staging allocations are outside this charge; it is not a managed-heap measurement.

Engine creation rejects nonpositive values with ArgumentOutOfRangeException. Exhaustion during
RETURNING raises DatabaseException with CD05018 and rolls back the writes; an explicit transaction
requires rollback after the failure. Serialization preflight also enforces 16 MiB frames (CD05016)
and 256-level result depth (CD05019) inside the transaction. These codes have no assigned SQLSTATE.

## Example

Adapted from `DocumentReturningBudgetTests.ExecuteAsync_ExactReturningBudget_ShouldCountEachOwnedRepresentationOnce`:

```csharp
using Assimalign.Cohesion.Database.Documents;

// Two 1,024-byte whole images: 208 metadata + 2 * (224 row + 1,200 clone + 1,024 raw).
var options = new DocumentDatabaseEngineOptions { StatementMemoryBudget = 5_104 };
await using var engine = DocumentDatabaseEngine.Create("budget", options);
```

The test admits both RETURNING * rows at 5,104 bytes. At 5,103 bytes it fails with CD05018 and
restores the original documents/versions for DELETE, or leaves no inserted documents for INSERT.
See [RETURNING](../../../../database/document/language/clauses/returning.md).

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/src/DocumentDatabaseEngineOptions.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/src/DocumentDatabaseEngine.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentReturningBudgetTests.cs`.
