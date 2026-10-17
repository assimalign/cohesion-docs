# System views

Reference for read-only virtual relations describing the session database's catalog.

> **Status:** Partial. Each page states the measured subset and explicit restrictions.

## Metadata families

- **[INFORMATION_SCHEMA](information-schema.md)** — Inspect stored tables, columns, and constraints.
- **[COHESION_SCHEMA](cohesion-schema.md)** — Inspect index keys and object ownership.

## Scope

The [dialect hub](../index.md) describes the 34-of-50 clause measure. Recognition alone is not
execution support; use [diagnostics](../diagnostics.md) and the
[unsupported-syntax matrix](../unsupported.md) when a form is rejected.

## Joining and snapshots

These read-only relations join with each other or with stored tables through the supported
two-relation [INNER JOIN](../clauses/join.md). Every system relation in a statement, including
subqueries and INSERT SELECT sources, reads one immutable catalog image. Snapshot transactions
use their BEGIN-time image; automatic and ReadCommitted statements capture it at first system
relation binding. Mutations report `COHSQLE021` (SQLSTATE 25006).

## Sources

- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution evidence** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`
