# SqlJsonFunctionExpression

Parsed SQL/JSON path function and its result/behavior clauses.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public sealed class SqlJsonFunctionExpression : SqlExpression
{
    public SqlJsonFunctionKind Kind { get; }
    public SqlExpression Context { get; }
    public SqlExpression Path { get; }
    public string? Returning { get; }
    public DatabaseTypeInfo? ReturningType { get; }
    public SqlJsonWrapper Wrapper { get; }
    public bool OmitQuotes { get; }
    public SqlJsonBehavior OnEmpty { get; }
    public SqlExpression? EmptyDefault { get; }
    public SqlJsonBehavior OnError { get; }
    public SqlExpression? ErrorDefault { get; }
    public IEnumerable<SqlExpression> GetOperands();
}
```

## Behavior and exceptions

The parser owns construction. Returning is the explicit type spelling, or null for the function's
default; ReturningType is populated for a resolved built-in declaration. Context and Path preserve
their expression nodes. GetOperands yields context, path, then present empty/error defaults.
Depth is one plus the deepest operand. Wrapper, OmitQuotes and behavior properties record the
effective defaults. Canonical rendering spells out wrapper, quote and behavior policies.

Properties and operand enumeration add no validation exceptions. SqlQueryRequest.FromSql throws
DatabaseParseException for parser errors. Execution uses DatabaseException diagnostics: CS01068
(42804) for a nonconstant/nonparameter path, CS01069 (42804) for invalid signatures, CS02081
(42601/0A000) for unsupported paths/PASSING, CS02082 (2203A) for strict structure, CS02083 (2203F)
for ERROR ON EMPTY, CS02084 (22003) for context numeric bounds, CS05093 (22034/2203B) for result
cardinality/kind and CS05094 (54000) for path/work/memory limits. The
[language reference](../../../../database/sql/language/functions/json-path.md) defines which errors
reach ON ERROR and the SQL NULL rules.

## Example

Checked against SqlJsonFunctionParserTests and SqlJsonPathTests:

```csharp
var statement = (SqlQueryStatement)new SqlQueryParser().Parse(
    "SELECT JSON_QUERY('[1,2]', '$[*]' RETURNING TEXT WITH WRAPPER);");
var node = (SqlJsonFunctionExpression)((SqlSelectExpression)statement.SqlExpression).Columns[0].Expression;
Console.WriteLine(node.Kind);    // Query
Console.WriteLine(node.Wrapper); // Unconditional
Console.WriteLine(SqlExpressionRenderer.Render(node));
// JSON_QUERY('[1,2]', '$[*]' RETURNING TEXT WITH UNCONDITIONAL ARRAY WRAPPER KEEP QUOTES NULL ON EMPTY NULL ON ERROR)
```

Execution returns TEXT `[1, 2]`.

## Related reference

[Assembly](index.md) · [SqlJsonFunctionKind](sql-json-function-kind.md) · [SqlJsonWrapper](sql-json-wrapper.md) · [SqlJsonBehavior](sql-json-behavior.md) · [SqlClauses](sql-clauses.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/Expressions/SqlJsonFunctionExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlJsonFunctionParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlJsonPathTests.cs`.
