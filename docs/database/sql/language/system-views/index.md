# System views

Reference for read-only virtual relations describing the session database's catalog.

> **Status:** Partial. Each page states the measured subset and explicit restrictions.

## Metadata families

- **[INFORMATION_SCHEMA](information-schema.md)** — Inspect stored tables, columns, and constraints.
- **[COHESION_SCHEMA](cohesion-schema.md)** — Inspect index keys and object ownership.

## Scope

The [dialect hub](../index.md) describes the 33-of-49 clause measure. Recognition alone is not
execution support; use [diagnostics](../diagnostics.md) and the
[unsupported-syntax matrix](../unsupported.md) when a form is rejected.

## Sources

- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution evidence** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`

