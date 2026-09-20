# Assimalign.Cohesion.Database.Sql.Schema

SQL's thin schema package provides one-step `SqlSchema.Compile`, lower-level `SqlSchema.Create` and `SqlSchemaCompiler`, the `ISqlSchema` declaration contracts, `SqlCompiledSchema`, its canonical serializer, and SQL migration planning.

> **Status:** Partial.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.

SQL's thin schema package provides one-step `SqlSchema.Compile`, lower-level `SqlSchema.Create` and
`SqlSchemaCompiler`, the `ISqlSchema*` declaration contracts, `SqlCompiledSchema`, its canonical
serializer, and SQL migration planning. Both SDK build tooling and the SQL engine consume it without
moving relational vocabulary into the `Database` area root.

Compiled tables, indexes, and constraints are schema-owned. The SQL catalog persists that ownership
and the engine protects those objects from session DDL.

See [DESIGN.md](design.md) for boundaries, canonicalization, and AOT decisions.

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Database` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Types` | `CohesionProjectReference` |

[Parent: Database](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Schema/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Schema/src/Assimalign.Cohesion.Database.Sql.Schema.csproj`.
