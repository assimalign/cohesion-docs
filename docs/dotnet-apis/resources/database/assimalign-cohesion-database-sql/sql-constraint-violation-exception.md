# SqlConstraintViolationException

Reports a named SQL integrity violation with exactly one coded Error diagnostic.

Namespace: `Assimalign.Cohesion.Database.Sql`.

## Signature

```csharp
public sealed class SqlConstraintViolationException : DatabaseException
{
    public SqlConstraintViolationException(string constraintName, string table,
        string constraintKind, object? offendingValue = null, Exception? innerException = null);
    public string ConstraintName { get; }
    public string Table { get; }
    public string ConstraintKind { get; }
    public object? OffendingValue { get; }
}
```

## Behavior and exceptions

The constructor preserves the supplied metadata and inner exception and creates one Error
diagnostic whose Message equals the exception's Message. It adds no argument-validation exception.

| ConstraintKind | Diagnostic code | SQLSTATE mapping |
| --- | --- | --- |
| `UNIQUE` or `PRIMARY KEY` | `CS03037` | 23505 |
| `CHECK` or `DOMAIN` | `CS03038` | 23514 |
| `FOREIGN KEY` | `CS03039` | 23503 |
| `NOT NULL` | `CS03024` | 23502 |
| Any other caller-supplied kind | `CS03038` | 23514 |

ConstraintName is the declared/generated constraint or index name, or the domain name. Table is
schema-qualified for row violations and empty for domain CAST/function checks that write no table.
Messages omit key values even when the caller supplies OffendingValue; that property retains the
existing safe-value disclosure policy. SQLSTATE is a documented mapping without a separate property.

Direct construction leaves the diagnostic unlocated. During execution, the session attaches an
available enclosing statement span without replacing an existing location. The located copy keeps
an existing inner cause, or the original thrown violation when there is no cause, preserving its
validator/executor stack. A rejected statement rolls back and leaves an explicit transaction usable.
Other NOT NULL assignment failures can use DatabaseException with the same CS03024 code.

Over the wire, [SqlClientException](../assimalign-cohesion-database-sql-client/sql-client-exception.md)
preserves ExecutionFailure, the code-prefixed message and negotiated diagnostics.

## Example

This follows the named primary-key case in SqlConstraintDiagnosticWireTests and
SqlLanguageConformanceTests. Execute against a fresh database session:

```csharp
await session.ExecuteAsync("CREATE TABLE t (id INT, CONSTRAINT pk_t PRIMARY KEY(id))");
await session.ExecuteAsync("INSERT INTO t VALUES (1)");
try { await session.ExecuteAsync("INSERT INTO t VALUES (1)"); }
catch (SqlConstraintViolationException failure) {
    Console.WriteLine(failure.ConstraintName);      // pk_t
    Console.WriteLine(failure.Table);               // dbo.t
    Console.WriteLine(failure.Diagnostics[0].Code); // CS03037
    Console.WriteLine(failure.Diagnostics.Count);   // 1
}
```

Only the original row remains. The message identifies `pk_t` and `dbo.t` without the key value.

## Related reference

[Assembly](index.md) · [Design](design.md#error-model) · [Constraints](../../../../database/sql/language/constraints.md) · [SQL diagnostics](../../../../database/sql/language/diagnostics.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Exceptions/SqlConstraintViolationException.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlConstraintDiagnosticTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Client/tests/SqlConstraintDiagnosticWireTests.cs`.
