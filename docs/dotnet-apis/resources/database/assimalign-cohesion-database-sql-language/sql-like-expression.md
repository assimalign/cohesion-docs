# SqlLikeExpression

Parsed LIKE/ILIKE pattern, optionally negated, with an optional ESCAPE operand.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public sealed class SqlLikeExpression : SqlExpression
{
    public SqlExpression Operand { get; }
    public SqlExpression Pattern { get; }
    public SqlExpression? Escape { get; }
    public bool IsNegated { get; }
    public bool IsCaseInsensitive { get; }
}
```

## Behavior and exceptions

The parser owns construction. Escape is null when ESCAPE is omitted, selecting backslash;
an explicit SQL NULL is a SqlLiteralExpression, while an empty string disables escaping.
Pattern and Escape can be arbitrary supported scalar trees, including parameters and
concatenations. Depth is one plus the maximum child depth, including Escape. The canonical
renderer retains explicit ESCAPE, the ILIKE spelling when IsCaseInsensitive is true, and
conservative parentheses around concatenated operands. IsNegated controls NOT independently.

Properties add no validation exceptions. Parse returns diagnostics; request/session factories
reject malformed text with DatabaseParseException. A missing ESCAPE operand uses CS00003.
Execution raises DatabaseException/CS02034 (22025) for a multi-scalar escape or trailing
pattern escape. Fixed and supplied-parameter patterns validate during binding, including empty
inputs and NULL matched values; dynamic operands validate on evaluation. Valid NULL operands
give UNKNOWN. Wildcards consume original Unicode scalars under every collation. See
[LIKE and ESCAPE](../../../../database/sql/language/expressions/operators.md#like-and-escape).

Ordinary invariant matches use linear work in input length. Pathological invariant matching raises
DatabaseException with located CS05040 when its per-match work budget is exhausted; no SQLSTATE
is assigned. The linked language reference gives the exact budget and charging rules. LIKE,
ILIKE and their NOT forms fail without a Boolean result. The executing statement token reaches row-time
matching, including cached views and stored CHECKs; cancellation takes precedence over refusal.

## Example

```csharp
var statement = (SqlQueryStatement)new SqlQueryParser().Parse(
    "SELECT name ILIKE 'a!%' ESCAPE '!' FROM t;");
var node = (SqlLikeExpression)((SqlSelectExpression)statement.SqlExpression).Columns[0].Expression;
Console.WriteLine(node.Escape is SqlLiteralExpression); // True
Console.WriteLine(node.IsNegated); // False
Console.WriteLine(node.IsCaseInsensitive); // True
Console.WriteLine(SqlExpressionRenderer.Render(node)); // name ILIKE 'a!%' ESCAPE '!'
```

For name equal to `A%`, execution returns TRUE; `abc` returns FALSE. Case matching follows
[the effective collation](../../../../database/sql/language/expressions/operators.md#ilike-and-not-ilike).

## Related reference

[Assembly](index.md) · [SqlDistinctExpression](sql-distinct-expression.md) · [Diagnostics](../../../../database/sql/language/diagnostics.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/Expressions/SqlLikeExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlPredicateParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlPredicateExecutionTests.cs`.
