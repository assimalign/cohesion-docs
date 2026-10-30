# SqlJsonKind

Selects the JSON kind accepted by an IS JSON predicate.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public enum SqlJsonKind { Value, Array, Object, Scalar }
```

## Behavior and exceptions

| Member | Meaning |
| --- | --- |
| Value | Any JSON value, including JSON `null`; the SQL default |
| Array | A JSON array |
| Object | A JSON object |
| Scalar | A string, number, Boolean or JSON `null` |

The enum adds no exceptions or conversion API. [SqlIsJsonExpression](sql-is-json-expression.md)
retains the parsed selection; its page covers binding refusals and NULL behavior.

## Example

```csharp
var statement = (SqlQueryStatement)new SqlQueryParser().Parse("SELECT 'null' IS JSON SCALAR;");
var node = (SqlIsJsonExpression)((SqlSelectExpression)statement.SqlExpression).Columns[0].Expression;
Console.WriteLine(node.Kind == SqlJsonKind.Scalar); // True
```

Execution returns TRUE, as checked by SqlJsonAccessTests.

## Related reference

[Assembly](index.md) · [SqlIsJsonExpression](sql-is-json-expression.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlJsonKind.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlJsonAccessTests.cs`.
