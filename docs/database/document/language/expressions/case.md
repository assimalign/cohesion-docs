# CASE

Chooses the first matching branch and evaluates only its result.

## Syntax

```syntaxsql
CASE [ operand ] WHEN expression THEN result [ WHEN expression THEN result ...n ]
    [ ELSE result ] END
```

## Remarks

Searched CASE omits operand and converts each WHEN using the [SQL++ truth rule](missing.md).
Simple CASE compares operand and WHEN values structurally. NULL and MISSING never match,
including themselves. An unmatched CASE returns ELSE or NULL when ELSE is absent; a selected
result may be MISSING. Only the selected result evaluates, but all branches undergo parameter,
numeric, META, aggregate and grouping validation before execution. Grouping errors are `COHDOC001`,
missing parameters `COHDOC005` and lossy numbers `COHDOC007` (22003).

CASE/WHEN/THEN/ELSE/END are reserved. Existing fields work quoted or after a dot, such as d.end.

## Examples

For items containing `{"amount":2}`:

```sql
SELECT CASE amount WHEN 2 THEN 'two' ELSE 'other' END AS kind,
       CASE WHEN 1 THEN 'yes' ELSE 'no' END AS truth,
       CASE MISSING WHEN 1 THEN 2 END AS absent FROM items;
```

The result is `{"kind":"two","truth":"yes","absent":null}`. CASE on MISSING falls through.

## See also

[Expressions](index.md) · [Operators](operators.md) · [MISSING](missing.md)

## Sources

- **Execution** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentQueryTests.Predicates.cs`.
- **Branch evaluation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/src/Internal/Evaluation/DocumentExpressionEvaluator.Predicates.cs`.
