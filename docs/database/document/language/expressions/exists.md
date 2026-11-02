# EXISTS

EXISTS tests whether an array contains any elements.

## Syntax

```syntaxsql
EXISTS expression
NOT EXISTS expression
```

## Behavior

| Operand | EXISTS | NOT EXISTS |
| --- | --- | --- |
| Nonempty array, including `[NULL]` | TRUE | FALSE |
| Empty array | FALSE | TRUE |
| MISSING | MISSING | MISSING |
| NULL, scalar or object | NULL | NULL |

NOT EXISTS uses ordinary negation. EXISTS accepts [expression subqueries](subqueries.md)
as well as other expressions. It binds below IS and above IN and comparisons; see the full
[precedence table](operators.md#precedence). The retired ODMG binding quantifier remains
CK00001 (no assigned SQLSTATE).

## Example

With items containing `{n:1}`, `{n:2}` and `{n:3}`:

```sql
SELECT EXISTS (SELECT RAW n FROM items WHERE n > 10) AS any,
       NOT EXISTS [] AS empty, EXISTS [NULL] AS present FROM items LIMIT 1;
```

Returns `{"any":false,"empty":true,"present":true}`.

## See also

[Expressions](index.md) · [MISSING](missing.md) · [Subquery evaluation and diagnostics](subqueries.md#evaluation)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentSubqueryTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/src/Internal/Evaluation/DocumentExpressionEvaluator.cs`.
