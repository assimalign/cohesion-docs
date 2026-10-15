# Statements

Reference for the supported data definition, data manipulation, and transaction-control statements.

> **Status:** Partial. Each page states the measured subset and explicit restrictions.

## Data definition language

- **[CREATE TABLE](create-table.md)** — Declare columns, defaults, collation, and constraints.
- **[ALTER TABLE](alter-table.md)** — Add/drop columns and constraints, including literal-default backfill.
- **[DROP TABLE](drop-table.md)** — Remove an ad-hoc stored table.
- **[CREATE INDEX](create-index.md)** — Create a plain-column secondary index.
- **[DROP INDEX](drop-index.md)** — Drop an index using its required table qualifier.

## Data manipulation language

- **[SELECT](select.md)** — Read and compose the measured relational query subset.
- **[INSERT](insert.md)** — Insert scalar rows or query results.
- **[UPDATE](update.md)** — Assign column values to selected rows.
- **[DELETE](delete.md)** — Remove selected rows.

## Transaction control

- **[BEGIN TRANSACTION](begin-transaction.md)** — Begin a session-scoped snapshot transaction.
- **[COMMIT TRANSACTION](commit-transaction.md)** — Publish its changes.
- **[ROLLBACK TRANSACTION](rollback-transaction.md)** — Abort its changes.

## Scope

The [dialect hub](../index.md) describes the 33-of-49 clause measure. Recognition alone is not
execution support; use [diagnostics](../diagnostics.md) and the
[unsupported-syntax matrix](../unsupported.md) when a form is rejected.

## Sources

- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution evidence** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`

