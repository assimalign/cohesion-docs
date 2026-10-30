# SqlIsJsonExpression

Parsed IS [NOT] JSON kind test.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public sealed class SqlIsJsonExpression : SqlExpression
{
    public SqlExpression Operand { get; }
    public SqlJsonKind Kind { get; }
    public bool IsNegated { get; }
}
```

## Behavior and exceptions

The parser owns construction. Kind is [Value](sql-json-kind.md) when omitted in SQL;
IsNegated records NOT. Depth is one plus Operand.Depth. Canonical rendering writes the kind,
including VALUE. Execution accepts character, JSON and JSONB values and returns nullable BOOLEAN:
malformed text is FALSE, SQL NULL stays NULL, and JSON `null` satisfies SCALAR.

Properties add no validation exceptions. Parser diagnostics report malformed syntax;
SqlQueryRequest.FromSql throws DatabaseParseException for parser errors. WITH/WITHOUT UNIQUE
KEYS is CK00001. Invalid operand signatures fail binding with DatabaseException/CS01067 (42883);
incompatible declared CASE/COALESCE alternatives use CS02080 (42804), before reading rows.

## Example

Checked against SqlJsonParserTests and SqlJsonAccessTests:

```csharp
var statement = (SqlQueryStatement)new SqlQueryParser().Parse(
    "SELECT 'null' IS NOT JSON ARRAY;");
var node = (SqlIsJsonExpression)((SqlSelectExpression)statement.SqlExpression).Columns[0].Expression;
Console.WriteLine(node.Kind);      // Array
Console.WriteLine(node.IsNegated); // True
```

Execution returns TRUE.

## Related reference

[Assembly](index.md) · [SqlJsonKind](sql-json-kind.md) · [JSON language reference](../../../../database/sql/language/expressions/json-access.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/Expressions/SqlIsJsonExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlJsonParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlJsonAccessTests.cs`.
