# Assimalign.Cohesion.Database.Sql.Schema

SQL's thin schema package provides one-step `SqlSchema.Compile`, lower-level `SqlSchema.Create` and `SqlSchemaCompiler`, the `ISqlSchema` declaration contracts, `SqlCompiledSchema`, its canonical serializer, and SQL migration planning.

> **Status:** Partial.

## Reference pages

- **[CompiledSchemaConstraint](compiled-schema-constraint.md)** — OnUpdate metadata, defaults and migration identity.
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

## Current format boundary

v8 changes typed TimeSpan defaults to exact signed seconds, changing document identity.
Older artifacts and recorded applied/pending documents are refused at `schema.format` before
hash comparison. See [default rendering](design.md#current-format-boundary) and the
[format-29/v8 upgrade note](../../../../database/sql/index.md#sql-format-29-and-schema-v8).

The integration branch uses SQL row-storage format 29 and compiled schema version 8.
Format 17 introduced the insert-default/frozen-missing-value split (table extension 6); format 18
separately changes quoting for newly executable function names in persisted canonical SQL.
The formats preserve resolved shared type identity/modifiers across contextual multi-word and
SQL Server lexical aliases. Typed temporal, interval and binary defaults use canonical literal
text. These unreleased formats have no older-format compatibility reader; use this current
boundary when interpreting historical layout notes above.

Format 19 quotes PERCENT and TIES in persisted SQL. Format 20 adds persisted SET NULL,
SET DEFAULT and NO ACTION delete actions. Format 21 introduced namespace records of kind 15. Format 22 quotes STRING_AGG and LISTAGG
identifiers in persisted SQL. Format 23 introduced catalog views and format 24 explicit NULL
placement. Format 25 added distinct predicates, Boolean tests and LIKE ESCAPE.
Format 26 fenced temporal DEFAULT and frozen missing-value text.
Current format 29 adds owned-sequence metadata and retains TYPE default assignment coercion
in table extension 7.
Every earlier format is refused at open; there is no
upgrade path. Compiled schema format remains version 8.

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Schema/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Schema/src/Assimalign.Cohesion.Database.Sql.Schema.csproj`.
