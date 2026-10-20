# CASE

Selects a value using searched predicates or a simple operand and ordered WHEN alternatives.

> **Status:** Implemented.

## Syntax

```syntaxsql
CASE WHEN predicate THEN result [ WHEN predicate THEN result ...n ] [ ELSE result ] END
CASE operand WHEN when_operand [ , ...n ] THEN result
     [ WHEN when_operand [ , ...n ] THEN result ...n ] [ ELSE result ] END

<when_operand> ::= expression | { = | <> | != | < | <= | > | >= } expression
                  | IS [ NOT ] NULL
```

## Remarks

Searched CASE selects the first TRUE predicate; FALSE and NULL do not select a branch. A
non-Boolean evaluated predicate reports COHDBG013. Simple CASE evaluates its operand once and
tests alternatives in source order. A comma list is the ISO when-operand list: any listed match
selects the shared THEN value. Plain alternatives use equality; comparison and null-test forms
use the written test. WHEN NULL never matches, including a NULL operand; use WHEN IS NULL.

Only the selected result expression evaluates. No match without ELSE returns NULL. Planning
still validates all bindings and expression shapes in skipped branches. Results retain the
selected runtime type without coercing branches to a common type; metadata is DatabaseType.Null
(unknown). Lists and maps are valid results. Equality uses [exact numeric and structural rules](list-values.md#comparison).
GQL has no separate MISSING value. Missing properties read NULL.

## Diagnostics

| Code | Meaning and trigger |
| --- | --- |
| `GQL0002` | Malformed CASE, missing WHEN/THEN/END or invalid when-operand syntax |
| `COHDBG001` | Unbound name/parameter or invalid programmatic CASE shape, including an invalid Comparison |
| `COHDBG013` | Evaluated predicate is not Boolean, value operation fails, or constructed result exceeds a value limit |
| `COHDBG016` | A possible composite wire result without negotiated graph.composite-values, even in a skipped arm |

No SQLSTATE is assigned to COHDBG016. See [value limits and wire capability](list-values.md#limits-and-wire-capability).

## Examples

Execute each standalone RETURN separately:

```sql
RETURN CASE WHEN NULL THEN 1 ELSE 2 END;
RETURN CASE WHEN TRUE THEN 2 ELSE 1/0 END;
RETURN CASE 42 WHEN 17, 42 THEN 1 ELSE 2 END;
RETURN CASE 42 WHEN < 30, >= 42 THEN 1 END;
RETURN CASE NULL WHEN IS NULL THEN 1 END;
RETURN CASE NULL WHEN NULL THEN 1 ELSE 2 END;
```

Results are 2, 2, 1, 1, 1 and 2, respectively. The skipped division never executes.

## See also

[Expressions](index.md) · [COALESCE](coalesce.md) · [NULLIF](nullif.md) · [RETURN](../clauses/return.md)

## Sources

- **Execution and results** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphValueTests.cs`.
- **Grammar** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlValueParserTests.cs`.
- **Value evaluation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/Internal/Evaluation/GraphExpressionEvaluator.Values.cs`.
