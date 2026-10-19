# SqlSetOperationExpression

A binary UNION, INTERSECT or EXCEPT query body.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public sealed class SqlSetOperationExpression : SqlQueryBodyExpression
{
    public SqlQueryBodyExpression Left { get; }
    public SqlQueryBodyExpression Right { get; }
    public SqlSetOperator Operator { get; }
    public bool IsAll { get; }
}
```

## Behavior and exceptions

Construction is internal to the parser. Left supplies output names. IsAll is false for omitted
or explicit DISTINCT. INTERSECT binds tighter than UNION/EXCEPT, which associate left to right.
Inherited OrderBy/Limit/Offset describe the combined result. Canonical rendering preserves the
query tree and normalizes redundant parentheses, including IN ((SELECT …)) membership.

Accessors do not validate. Planning checks equal widths (COHSQLE013), common types (COHSQLE003,
42804) and output-name/ordinal ordering (COHSQLE018; ambiguous names COHSQLE019) before reads.
CORRESPONDING is COHDBL001. SqlQueryRequest.FromSql rejects parser errors with DatabaseParseException.
The executor materializes operands; compatible UNION chains flatten to an N-way plan. EXPLAIN
describes set operators and conversion/output metadata without executing them.

## Example

```csharp
var statement = (SqlQueryStatement)new SqlQueryParser().Parse(
    "SELECT id FROM a UNION ALL SELECT id FROM b INTERSECT SELECT id FROM c;");
var union = (SqlSetOperationExpression)statement.SqlExpression;
Console.WriteLine(union.Operator); // Union
Console.WriteLine(union.IsAll); // True
Console.WriteLine(((SqlSetOperationExpression)union.Right).Operator); // Intersect
```

## Related reference

[SqlSetOperator](sql-set-operator.md) · [SqlQueryBodyExpression](sql-query-body-expression.md) · [Assembly](index.md) · [Set operations](../../../../database/sql/language/clauses/set-operations.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/Expressions/SqlSetOperationExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlSetOperationParserTests.cs`.
