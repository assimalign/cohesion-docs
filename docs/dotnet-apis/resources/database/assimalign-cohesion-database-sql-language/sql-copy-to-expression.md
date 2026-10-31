# SqlCopyToExpression

Parsed table export to the caller's output stream.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public sealed class SqlCopyToExpression : SqlQueryExpression
{
    public SqlTableReference Table { get; }
    public IReadOnlyList<string> Columns { get; }
    public SqlCopyOptions Options { get; }
}
```

## Behavior and exceptions

Construction is internal to the parser. Table is the physical source; Columns is a copied,
read-only ordered list, with an empty list selecting every live column. Options contains resolved
defaults. CommandType is [SqlQueryCommandType.CopyTo](sql-query-command-type.md), value 13.
Properties add no validation exceptions. Parser diagnostics include CS00003 for malformed/legacy
options, CS01070 for invalid options and CK00001 for excluded destinations/formats. Binding uses
CS01071 for duplicate columns or invalid FORCE_QUOTE membership, and CS01015 for missing output
columns. See the [statement reference](../../../../database/sql/language/statements/copy-to.md).

## Example

Checked against SqlCopyToTests in the language project:

```csharp
var statement = (SqlQueryStatement)new SqlQueryParser().Parse(
    "COPY t(a,b) TO STDOUT (FORMAT csv, HEADER);");
var copy = (SqlCopyToExpression)statement.SqlExpression;
Console.WriteLine(copy.Columns.Count); // 2
Console.WriteLine(copy.Options.IsCsv);  // True
Console.WriteLine(copy.Options.Header); // True
```

Use [CopyToAsync](../assimalign-cohesion-database-sql/sql-database-session.md#copytoasync) to execute.

## Related reference

[Assembly](index.md) · [SqlCopyOptions](sql-copy-options.md) · [SqlExpressionRenderer](sql-expression-renderer.md) · [SqlClauses](sql-clauses.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/Expressions/SqlCopyToExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlCopyToTests.cs`.
