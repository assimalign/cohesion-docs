# SqlDerivedTableReference

An aliased query body used as a relation in FROM, JOIN or a comma-separated item.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public sealed class SqlDerivedTableReference : SqlRelationReference
{
    public SqlQueryBodyExpression Query { get; }
    public IReadOnlyList<string> Columns { get; }
}
// Alias is inherited from SqlRelationReference.
```

## Behavior and exceptions

Construction is internal. The parser requires an alias; Columns is empty when names are inherited,
otherwise it contains ordered output renames. Planning requires a full-width list and nonblank
output names (`CS05013`), even for an empty body. Duplicate names remain ambiguous on reference
(`CS01015`). Alias hides underlying qualifiers. An independent body materializes once per statement; a correlated body evaluates for each
enclosing frame in the same statement context; projected expression types, modifiers, nullability, domains and collation provenance survive.

The accessors do not throw validation exceptions. Missing aliases or malformed lists are `CS00003`
parser diagnostics. Query nesting shares the 32-level limit (`CK00001`). Actual sibling references
receive `CK00001` naming LATERAL; enclosing-query references bind through lexical parents and execute for each outer row.
Unknown or hidden names keep `CS01015`. Outer ordering over derived outputs executes with per-key NULLS FIRST/LAST, as does ordering
inside the body. See [FROM](../../../../database/sql/language/clauses/from.md#derived-tables).

## Example

```csharp
var statement = (SqlQueryStatement)new SqlQueryParser().Parse(
    "SELECT d.a FROM (SELECT id FROM t) AS d (a);");
var select = (SqlSelectExpression)statement.SqlExpression;
var derived = (SqlDerivedTableReference)select.From[0].Table;
Console.WriteLine(derived.Alias);      // d
Console.WriteLine(derived.Columns[0]); // a
Console.WriteLine(((SqlSelectExpression)derived.Query).From.Count); // 1
```

For t containing IDs 1, 2 and 3, execution returns those IDs with output name `a`, in unspecified order.

Query also accepts [set bodies](sql-set-operation-expression.md), such as
`SELECT d.id FROM (SELECT id FROM a UNION SELECT id FROM b) d;`. Their synthesized output metadata
retains common types, modifiers and collation rather than borrowing physical source ordinals.

## Related reference

[Assembly](index.md) · [SqlRelationReference](sql-relation-reference.md) · [SqlSelectExpression](sql-select-expression.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlDerivedTableReference.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlDerivedTableParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlDerivedTableExecutionTests.cs`.
