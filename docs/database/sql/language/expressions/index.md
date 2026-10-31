# Expressions

Reference for scalar values, operators, conditional expressions, conversions, and subqueries.

> **Status:** Partial. Each page states the measured subset and explicit restrictions.

## Expression forms

- **[JSON access and IS JSON](json-access.md)** — Member/array extraction, kind tests and scalar conversions.
- **[NEXT VALUE FOR](next-value-for.md)** — One sequence value per row, shared by repeated standard occurrences.
- **[Operators and predicates](operators.md)** — PostgreSQL precedence, NULL-safe distinct tests, Boolean tests, LIKE/ILIKE ESCAPE, arithmetic and integer bitwise operators.
- **[CASE](case.md)** — Select a scalar branch.
- **[CAST and ::](cast.md)** — Convert within the exact supported pair matrix, including chained postfix casts.
- **[Subqueries](subqueries.md)** — Evaluate correlated and uncorrelated scalar, membership, and existence queries.
- **[Quantified comparisons](quantified-comparisons.md)** — Compare a scalar with ANY/SOME/ALL rows of a subquery.
- **[Literals and identifiers](literals.md)** — Write values and delimited identifiers.
- **[Parameters](parameters.md)** — Bind command values by name.

## Scope

The [dialect hub](../index.md) describes the 87-of-89 clause measure. Recognition alone is not
execution support; use [diagnostics](../diagnostics.md) and the
[unsupported-syntax matrix](../unsupported.md) when a form is rejected.

## Sources

- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution evidence** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`
