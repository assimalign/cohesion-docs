# Expressions

Reference for scalar values, operators, conditional expressions, conversions, and subqueries.

> **Status:** Partial. Each page states the measured subset and explicit restrictions.

## Expression forms

- **[Operators and predicates](operators.md)** — Apply precedence, comparison, arithmetic, and predicates.
- **[CASE](case.md)** — Select a scalar branch.
- **[CAST](cast.md)** — Convert within the exact supported pair matrix.
- **[Subqueries](subqueries.md)** — Evaluate uncorrelated scalar, membership, and existence queries.
- **[Literals and identifiers](literals.md)** — Write values and delimited identifiers.
- **[Parameters](parameters.md)** — Bind command values by name.

## Scope

The [dialect hub](../index.md) describes the 33-of-49 clause measure. Recognition alone is not
execution support; use [diagnostics](../diagnostics.md) and the
[unsupported-syntax matrix](../unsupported.md) when a form is rejected.

## Sources

- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution evidence** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`

