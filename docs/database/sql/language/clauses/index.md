# Clauses

Reference for row sources, predicates, grouping, ordering, and value clauses.

> **Status:** Partial. Each page states the measured subset and explicit restrictions.

## Query clauses

- **[FROM](from.md)** — Bind one relation in the current database.
- **[JOIN](join.md)** — Combine exactly two stored tables with an inner join.
- **[WHERE](where.md)** — Filter input rows.
- **[GROUP BY](group-by.md)** — Group by scalar key expressions.
- **[HAVING](having.md)** — Filter aggregate results.
- **[ORDER BY](order-by.md)** — Sort by source expressions, aliases, and ordinals.
- **[LIMIT and OFFSET](limit-offset.md)** — Limit and skip returned rows.
- **[COLLATE](collate.md)** — Choose effective string comparison rules.
- **[DISTINCT](distinct.md)** — Deduplicate projected rows.

## Write and definition clauses

- **[VALUES](values.md)** — Supply scalar insert rows.
- **[SET](set.md)** — Assign update values.
- **[DEFAULT](default.md)** — Declare a literal column default.

## Scope

The [dialect hub](../index.md) describes the 33-of-49 clause measure. Recognition alone is not
execution support; use [diagnostics](../diagnostics.md) and the
[unsupported-syntax matrix](../unsupported.md) when a form is rejected.

## Sources

- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution evidence** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`

