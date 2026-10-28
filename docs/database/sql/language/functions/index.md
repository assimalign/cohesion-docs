# Functions

Reference for the measured aggregate functions and executable scalar builtin functions.

> **Status:** Partial. Each page states the measured subset and explicit restrictions.

## Function groups

- **[Aggregate functions](aggregate-functions.md)** — Count, numeric, Boolean, statistical and string aggregates, with FILTER, DISTINCT/ALL and ordered inputs.
- **[Builtin functions](builtin-functions.md)** — Use 42 scalar names, planning-time signatures, transaction clocks and bounded string results.
- **[merge_action()](merge-action.md)** — Returns INSERT, UPDATE or DELETE in MERGE RETURNING.

## SQL-defined functions

[CREATE FUNCTION](../statements/create-function.md) persists an expression body;
[DROP FUNCTION](../statements/drop-function.md) removes an unused overload. Committed definition DDL
finishes mandatory refresh and reports success despite late cancellation or timeout expiration.

## Scope

The [dialect hub](../index.md) describes the 59-of-64 clause measure. Recognition alone is not
execution support; use [diagnostics](../diagnostics.md) and the
[unsupported-syntax matrix](../unsupported.md) when a form is rejected.

## Sources

- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution evidence** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`
