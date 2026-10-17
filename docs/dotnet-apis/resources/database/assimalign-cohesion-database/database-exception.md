# DatabaseException

Carries database failures and their complete structured diagnostics.

Namespace: `Assimalign.Cohesion.Database`.

## Signature

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

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database/src/Exceptions/DatabaseException.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database/tests/DatabaseDiagnosticTests.cs` and `DiagnosticSortFixPassTests.cs` in the same directory.
