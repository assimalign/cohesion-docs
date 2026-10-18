# Clauses

Reference for row sources, predicates, grouping, ordering, and value clauses.

> **Status:** Partial. Each page states the measured subset and explicit restrictions.

## Query clauses

- **[WITH](with.md)** — Define ordered, non-recursive query relations for one statement.

- **[FROM](from.md)** — Bind stored, system and derived relations in the current database.
- **[JOIN](join.md)** — N-way INNER/LEFT/CROSS joins, USING columns, and comma products.
- **[WHERE](where.md)** — Filter input rows.
- **[GROUP BY](group-by.md)** — Group by scalar key expressions.
- **[HAVING](having.md)** — Filter aggregate results.
- **[ORDER BY](order-by.md)** — Sort by source expressions, aliases, and ordinals.
- **[LIMIT, OFFSET, FETCH and TOP](limit-offset.md)** — Limit and skip returned rows.
- **[COLLATE](collate.md)** — Choose effective string comparison rules.
- **[DISTINCT](distinct.md)** — Deduplicate projected rows.
- **[UNION, INTERSECT and EXCEPT](set-operations.md)** — Combine query bodies with ALL/DISTINCT.

## Write and definition clauses

- **[VALUES](values.md)** — Supply scalar insert rows.
- **[SET](set.md)** — Assign update values.
- **[DEFAULT](default.md)** — Declare or request a column default.
- **[IDENTITY](identity.md)** — Generate integer values with durable, noncycling reservations.

## Scope

The [dialect hub](../index.md) describes the 47-of-56 clause measure. Recognition alone is not
execution support; use [diagnostics](../diagnostics.md) and the
[unsupported-syntax matrix](../unsupported.md) when a form is rejected.

## Sources

- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution evidence** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`
