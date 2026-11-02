# SqlQueryCommandType

Top-level command category carried by SQL query expressions.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public enum SqlQueryCommandType
{
    Unknown = 0, Select, Explain, Insert, Update, Delete, Create, Alter, Drop,
    Begin, Commit, Rollback, Truncate, CopyTo, Merge, Discard, SetTransaction,
    Savepoint, ReleaseSavepoint, RollbackToSavepoint
}
```

## Behavior and exceptions

CopyTo (13) identifies [SqlCopyToExpression](sql-copy-to-expression.md). It is inserted after
Truncate; following members have the values implied by this declaration. The streaming consumer
accepts CopyTo only (CS05095 for another statement); ordinary execution refuses COPY without its
consumer (CS05096). Reading enum members throws no exceptions.

## Example

Checked against the language parser's SqlCopyToTests:

```csharp
var statement = (SqlQueryStatement)new SqlQueryParser().Parse("COPY t TO STDOUT;");
var copy = (SqlCopyToExpression)statement.SqlExpression;
Console.WriteLine(copy.CommandType == SqlQueryCommandType.CopyTo); // True
```

## Related reference

[Assembly](index.md) · [COPY TO STDOUT](../../../../database/sql/language/statements/copy-to.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlQueryCommandType.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlCopyToTests.cs`.
