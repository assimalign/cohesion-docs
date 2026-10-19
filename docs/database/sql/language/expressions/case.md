# CASE (Cohesion SQL)

Returns a value from the first matching branch of a simple or searched CASE expression.

> **Status:** Partial. This page describes the executable subset and its boundaries.

## Syntax

```syntaxsql
CASE input_expression
    WHEN comparison_expression THEN result_expression
    [ WHEN comparison_expression THEN result_expression ...n ]
    [ ELSE result_expression ]
END

CASE
    WHEN search_condition THEN result_expression
    [ WHEN search_condition THEN result_expression ...n ]
    [ ELSE result_expression ]
END
```

## Arguments

- **`input_expression`** — The value compared against each simple-form branch.
- **`search_condition`** — A searched-form predicate.
- **`result_expression`** — The value of the selected branch or fallback.

## Remarks

Simple form chooses the first equal non-null comparison. Searched form chooses the first condition
evaluating to `TRUE`. An unmatched expression returns its `ELSE` value, or `NULL` when no
`ELSE` exists. Branches remain limited to the executable scalar subset.

The measured contract covers multiple branches, row expressions, and implicit null results.
In grouped projections, numeric `CASE` alternatives use a common numeric result type; incompatible
nonnumeric alternatives are planning errors.

Length, precision, scale and domain metadata survive only when every result alternative has
the same type, domain and modifiers. A missing ELSE or an unconstrained/different alternative
clears that metadata. [Persisted views](../statements/create-view.md) freeze this output contract
for INFORMATION_SCHEMA.COLUMNS and compatible replacement checks.

## Examples

Use the [conformance fixture](../statements/select.md#a-create-the-conformance-fixture).

```sql
SELECT CASE WHEN age > 40 THEN 'senior' ELSE 'junior' END,
       CASE id WHEN 1 THEN 'first' ELSE 'later' END
FROM t ORDER BY id;
```

The fixture produces `(junior, first)`, `(senior, later)`, and `(senior, later)`.

## See also

[Expressions](index.md) · [SELECT](../statements/select.md) · [Data types](../data-types/index.md)

## Sources

- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`
- **Implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Internal/SqlExpressionEvaluator.cs`

