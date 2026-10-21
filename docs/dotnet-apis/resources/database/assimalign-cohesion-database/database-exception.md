# DatabaseException

Carries database failures and their complete structured diagnostics.

Namespace: `Assimalign.Cohesion.Database`.

## Signature

This is an engine-side exception. The six shared/model client exceptions now derive directly from
System.Exception; catching DatabaseException no longer handles remote failures. Catch the exception
for the client API in use. See [remote error handling](../../../../database/model-project-migration.md#remote-failures).

```csharp
public class DatabaseException : Exception
{
    public DatabaseException();
    public DatabaseException(string message);
    public DatabaseException(string message, Exception? innerException);
    public DatabaseException(string message, IEnumerable<Diagnostic> diagnostics,
        Exception? innerException = null);
    public IReadOnlyList<Diagnostic> Diagnostics { get; }
}
```

## Behavior and exceptions

Diagnostics is never null. Constructors without diagnostics return an empty collection. The
structured constructor snapshots supplied entries, ordering located entries by Start before
unlocated ones; ties retain input order. Code, message, severity and available source spans stay
separate. Locations use zero-based UTF-16 offsets with exclusive End and one-based Line. Passing a
null diagnostic sequence throws ArgumentNullException.

Query parse/binder/planner rejections use this carrier. Servers compose CODE: message from the first
Error entry; SQL and GQL negotiate structured wire diagnostics, while Documents uses its Failure
frame. DatabaseParseException retains complete parser diagnostics through the same base.

Engine failure codes belong in Diagnostics, including translated Storage, Transactions and
Indexing failures. Translation searches inner exceptions and aggregate branches, preserves an
existing specific diagnosis and gives corruption precedence over ordinary I/O. Storage-failure
and corruption Message text is explanatory; wire formatting supplies a missing code prefix.
SQL/Key-Value Pair aborted-transaction refusals currently also retain a prefix in their embedded
Message. Inspect Diagnostics for the code instead of parsing Message.

[DatabaseCorruptionException](database-corruption-exception.md) specializes this carrier for
detected storage damage: COHDBS003, database name, optional page and the original storage cause.
It applies to create/open and model operations; an affected read fails instead of omitting data.

## Example

```csharp
var failure = new DatabaseException("Rejected", [
    new Diagnostic { Code = "later", Message = "No span", Severity = DiagnosticSeverity.Error },
    new Diagnostic { Code = "first", Message = "At column", Severity = DiagnosticSeverity.Error,
        Start = 7, End = 8, Line = 1 }
]);
Console.WriteLine(failure.Diagnostics[0].Code); // first
```

## Related reference

[Assembly](index.md) · [SQL codes](../../../../database/sql/language/diagnostics.md) · [OQL codes](../../../../database/document/language/diagnostics.md) · [GQL codes](../../../../database/graph/language/diagnostics.md)

DatabaseNotFoundException also preserves structured diagnostics from an inner DatabaseException.
Known incomplete file sets therefore keep absence semantics for provisioning while retaining their
model code. See [file-set diagnostics](../../../../database/overview.md#database-file-sets-and-lifecycle).

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database/src/Exceptions/DatabaseException.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database/tests/DatabaseDiagnosticTests.cs` and `DiagnosticSortFixPassTests.cs` in the same directory.
