# SqlCopyOptions

Resolved text/CSV export options. Encoding is always UTF-8.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public sealed class SqlCopyOptions
{
    public bool IsCsv { get; }
    public char Delimiter { get; }
    public string NullString { get; }
    public bool Header { get; }
    public char Quote { get; }
    public char Escape { get; }
    public bool ForceQuoteAll { get; }
    public IReadOnlyList<string> ForceQuote { get; }
}
```

## Behavior and exceptions

The parser constructs Options on [SqlCopyToExpression](sql-copy-to-expression.md). Text defaults
are IsCsv=false, tab Delimiter, `\N` NullString and Header=false. CSV defaults are comma Delimiter,
empty NullString, double-quote Quote and matching Escape. ForceQuoteAll=false and an empty copied,
read-only ForceQuote list mean no forced quoting. Quote/escape controls apply only to CSV.

Reading properties throws no exceptions. Invalid, duplicate or incompatible SQL options produce
CS01070 diagnostics; malformed option syntax uses CS00003. Binding reports CS01071 for repeated
or out-of-export FORCE_QUOTE columns. The [options table](../../../../database/sql/language/statements/copy-to.md#options)
defines character, encoding and NULL-marker validation.

## Example

From the default assertions in language SqlCopyToTests:

```csharp
var statement = (SqlQueryStatement)new SqlQueryParser().Parse("COPY t TO STDOUT (FORMAT csv);");
var options = ((SqlCopyToExpression)statement.SqlExpression).Options;
Console.WriteLine(options.Delimiter == ','); // True
Console.WriteLine(options.NullString == ""); // True
Console.WriteLine(options.Quote == '"');     // True
```

## Related reference

[Assembly](index.md) · [COPY TO STDOUT](../../../../database/sql/language/statements/copy-to.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlCopyOptions.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlCopyToTests.cs`.
