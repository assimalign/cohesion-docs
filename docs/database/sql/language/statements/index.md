# Statements

Reference for the supported data definition, data manipulation, and transaction-control statements.

> **Status:** Partial. Each page states the measured subset and explicit restrictions.

## Data definition language

- **[CREATE SCHEMA](create-schema.md)** — Persist a SQL namespace independently of tables.
- **[DROP SCHEMA](drop-schema.md)** — RESTRICT by default or atomically CASCADE dependent objects.

- **[CREATE TABLE](create-table.md)** — Declare columns, defaults, collation, and constraints.
- **[ALTER TABLE](alter-table.md)** — Add/drop columns and constraints, including literal-default backfill.
- **[DROP TABLE](drop-table.md)** — Remove an ad-hoc stored table.
- **[TRUNCATE](truncate.md)** — Empty a table set with FK closure, identity policies and old-snapshot visibility.
- **[CREATE INDEX](create-index.md)** — Create a plain-column secondary index.
- **[DROP INDEX](drop-index.md)** — Drop an index using its required table qualifier.

- **[CREATE VIEW](create-view.md)** — Persist a bound read-only query; optionally replace compatible outputs.
- **[DROP VIEW](drop-view.md)** — Remove a view with RESTRICT or dependent-view CASCADE.

- **[CREATE FUNCTION](create-function.md)** — Persist a scalar SQL expression body.
- **[DROP FUNCTION](drop-function.md)** — Remove an unused SQL overload by signature.
- **[CREATE DOMAIN](create-domain.md)** — Persist a scalar domain with default/null/check rules.
- **[DROP DOMAIN](drop-domain.md)** — Remove an unused SQL domain.

## Data manipulation language

- **[SELECT](select.md)** — Read and compose the measured relational query subset.
- **[INSERT](insert.md)** — Insert scalar rows or query results.
- **[UPDATE](update.md)** — Assign column values to selected rows.
- **[DELETE](delete.md)** — Remove selected rows.

- **[EXPLAIN](explain.md)** — Describe a chosen plan; ANALYZE executes SELECT for candidate-work counts.

## Transaction control

- **[BEGIN TRANSACTION](begin-transaction.md)** — Begin a session-scoped snapshot transaction.
- **[COMMIT TRANSACTION](commit-transaction.md)** — Publish its changes.
- **[ROLLBACK TRANSACTION](rollback-transaction.md)** — Abort its changes.

## Scope

The [dialect hub](../index.md) describes the 55-of-61 clause measure. Recognition alone is not
execution support; use [diagnostics](../diagnostics.md) and the
[unsupported-syntax matrix](../unsupported.md) when a form is rejected.

## Sources

- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution evidence** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`
