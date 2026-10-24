# DatabaseTransactionAbortedException

Reports a transaction the engine aborted, retaining its retryable exception kind and diagnostics.

Namespace: `Assimalign.Cohesion.Database`.

## Signature

```csharp
public class DatabaseTransactionAbortedException : DatabaseException
{
    public DatabaseTransactionAbortedException(string message);
    public DatabaseTransactionAbortedException(string message, Exception? innerException);
    public DatabaseTransactionAbortedException(string message, IEnumerable<Diagnostic> diagnostics,
        Exception? innerException = null);
}
```

## Behavior and exceptions

The engine uses this type for retryable write conflicts and confirmed aborts. Deadlock victims
retain the derived DatabaseTransactionDeadlockException. A commit whose durability is unknown
uses DatabaseTransactionCommitUnconfirmedException instead; it must not be treated as a confirmed
abort. Model engines translate the independent transaction-kernel exception at their boundary.

The diagnostics constructor copies supplied entries, stably orders located entries by Start and
places unlocated entries last. Diagnostics is read-only and never null; other constructors leave
it empty. The Message stays as supplied, without adding a code prefix, and InnerException retains
the supplied cause. A null diagnostics sequence throws ArgumentNullException.

Graph catalog snapshot conflicts use this constructor to attach CG04007 while preserving the
catalog cause and abort kind. A later refusal to use an already-aborted SQL, Key-Value or Graph
transaction is a DatabaseException carrying its model code; it need not be this subtype.
Remote failures use model client exceptions and ExecutionFailure wire responses.

## Example

This adapts DatabaseDiagnosticTests; clearing the input cannot change the exception:

```csharp
var diagnostics = new List<Diagnostic>
{
    new() { Code = "unlocated" },
    new() { Code = "late", Start = 20 },
    new() { Code = "first", Start = 1 }
};
var cause = new InvalidOperationException();
var failure = new DatabaseTransactionAbortedException("Retry the transaction.", diagnostics, cause);
diagnostics.Clear();
Console.WriteLine(string.Join(", ", failure.Diagnostics.Select(item => item.Code)));
Console.WriteLine(ReferenceEquals(failure.InnerException, cause));
```

The output is `first, late, unlocated` and `True`. Roll back and retry the complete transaction
after a confirmed retryable abort, including its earlier statements.

## Related reference

[Assembly](index.md) · [DatabaseException](database-exception.md) · [Graph diagnostics](../../../../database/graph/language/diagnostics.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database/src/Exceptions/DatabaseTransactionAbortedException.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database/tests/DatabaseDiagnosticTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphCatalogErrorTests.cs`.
