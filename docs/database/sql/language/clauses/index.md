# Clauses

Reference for row sources, predicates, grouping, ordering, and value clauses.

> **Status:** Partial. Each page states the measured subset and explicit restrictions.

## Query clauses

- **[WITH](with.md)** — Define query relations and demand-driven recursive UNION ALL/DISTINCT fixpoints.

- **[FROM](from.md)** — Bind stored, system and derived relations in the current database.
- **[JOIN](join.md)** — N-way INNER/LEFT/CROSS joins, USING columns, and comma products.
- **[WHERE](where.md)** — Filter input rows.
- **[GROUP BY](group-by.md)** — Scalar keys, empty sets, GROUPING SETS, ROLLUP and CUBE.
- **[HAVING](having.md)** — Filter aggregate results.
- **[FILTER](filter.md)** — Select rows for one aggregate before evaluating its arguments.
- **[OVER and WINDOW](window.md)** — Ordered partitions, ROWS/RANGE frames, named inheritance and window placement.
- **[ORDER BY](order-by.md)** — Sort by source expressions, aliases, and ordinals.
- **[LIMIT, OFFSET, FETCH and TOP](limit-offset.md)** — Limit and skip returned rows.
- **[COLLATE](collate.md)** — Choose effective string comparison rules.
- **[DISTINCT](distinct.md)** — Deduplicate projected rows.
- **[UNION, INTERSECT and EXCEPT](set-operations.md)** — Combine query bodies with ALL/DISTINCT.

## Write and definition clauses

- **[VALUES](values.md)** — Supply scalar insert rows.
- **[ON CONFLICT](on-conflict.md)** — Skip unique conflicts or update with target and EXCLUDED values.
- **[SET](set.md)** — Assign update values.
- **[DEFAULT](default.md)** — Declare or request a column default.
- **[IDENTITY](identity.md)** — Generate integer values with owned sequences, overrides and ALTER/backfill.

- **[RETURNING](returning.md)** — Return each directly written row's scalar projection.

## Scope

The [dialect hub](../index.md) describes the 83-of-85 clause measure. Recognition alone is not
execution support; use [diagnostics](../diagnostics.md) and the
[unsupported-syntax matrix](../unsupported.md) when a form is rejected.

## Sources

- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution evidence** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`
