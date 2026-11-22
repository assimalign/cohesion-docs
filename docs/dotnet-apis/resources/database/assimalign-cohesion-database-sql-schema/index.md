# Assimalign.Cohesion.Database.Sql.Schema

SQL's thin schema package provides one-step `SqlSchema.Compile`, lower-level `SqlSchema.Create` and `SqlSchemaCompiler`, the `ISqlSchema` declaration contracts, `SqlCompiledSchema`, its canonical serializer, and SQL migration planning.

> **Status:** Partial.

## Reference pages

- **[CompiledSchemaIndex](compiled-schema-index.md)** — Immutable key orders, predicates and v11 identity.

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

Typed column type changes use the planner's [conversion safety gate](design.md#type-migration-safety).
Only proven exact conversions bypass destructive-change opt-in. A plan containing a TYPE rewrite
uses the engine's [joint migration executor](../assimalign-cohesion-database-sql/design.md#joint-type-migrations).

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Database` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Types` | `CohesionProjectReference` |

[Parent: Database](../index.md)

## Current format boundary

v11 adds CompiledSchemaIndex.KeyOrders, retaining v10's optional Predicate and v9's
always-serialized OnUpdate metadata. The earlier v8 changed typed TimeSpan defaults to exact signed seconds;
v11 retains that rendering.
Older artifacts and recorded applied/pending documents are refused at `schema.format` before
hash comparison. See [default rendering](design.md#current-format-boundary) and the
[format-35/v11 upgrade note](../../../../database/sql/index.md#sql-format-35-and-schema-v11).

The integration branch uses SQL row-storage format 35 and compiled schema version 11.
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
Format 29 added owned-sequence metadata. Format 30 added ON UPDATE action metadata in table extension 8.
Format 31 added partial-index predicates in index extension 2. Format 32 added per-column
direction and NULL placement in mandatory index extension 3 and retained TYPE default assignment coercion.
Format 33 fenced SQL/JSON names and ordinary-call quoting. Format 34 introduced joint typed
migration intent semantics. Current format 35 fences separate physical owners for kept-table
index trees and durable per-table migration classification; format 34 and earlier are refused
before recovery writes.
Every earlier format is refused at open; there is no
upgrade path. Compiled schema format remains version 11.

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Schema/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Schema/src/Assimalign.Cohesion.Database.Sql.Schema.csproj`.
