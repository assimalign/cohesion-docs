# Language (SQL)

Reference for Cohesion's declared SQL dialect and its measured execution boundaries.

> **Status:** Partial. Phase 22 measures 33 of 49 declared clauses against the live engine.

Cohesion SQL is a bounded Structured Query Language (SQL) implementation. Support is measured using
the engine's execution-case table: every advertised clause must have a passing case covering
results, state changes, or an intended semantic error. Counts describe named clauses, not complete
International Organization for Standardization (ISO) SQL compatibility.

Start with [Syntax conventions](syntax-conventions.md) to read the diagrams.
Every query executes within the session's database. Accepted clauses compose only within the
restrictions on their reference pages.

## Statements

The [Statements](statements/index.md) section groups data definition language (DDL), data
manipulation language (DML), and transaction control.

- **DDL** — [CREATE TABLE](statements/create-table.md), [ALTER TABLE](statements/alter-table.md),
  [DROP TABLE](statements/drop-table.md), [CREATE INDEX](statements/create-index.md), and
  [DROP INDEX](statements/drop-index.md).
- **DML** — [SELECT](statements/select.md), [INSERT](statements/insert.md),
  [UPDATE](statements/update.md), and [DELETE](statements/delete.md).
- **Transactions** — [BEGIN](statements/begin-transaction.md),
  [COMMIT](statements/commit-transaction.md), and [ROLLBACK](statements/rollback-transaction.md).

## Clauses

The [Clauses](clauses/index.md) section describes binding and execution rules.

- **Sources and filtering** — [FROM](clauses/from.md), [JOIN](clauses/join.md), and
  [WHERE](clauses/where.md).
- **Grouping** — [GROUP BY](clauses/group-by.md) and [HAVING](clauses/having.md).
- **Result shaping** — [ORDER BY](clauses/order-by.md), [LIMIT/OFFSET](clauses/limit-offset.md),
  [DISTINCT](clauses/distinct.md), and [COLLATE](clauses/collate.md).
- **Writes and defaults** — [VALUES](clauses/values.md), [SET](clauses/set.md), and
  [DEFAULT](clauses/default.md).

## Expressions

The [Expressions](expressions/index.md) section covers the executable scalar subset.

- **Computation** — [Operators and predicates](expressions/operators.md),
  [CASE](expressions/case.md), and [CAST](expressions/cast.md).
- **Nested queries** — [Subqueries](expressions/subqueries.md).
- **Values and names** — [Literals and identifiers](expressions/literals.md) and
  [Parameters](expressions/parameters.md).

## Functions

The [Functions](functions/index.md) section separates execution support from lexical recognition.

- **Aggregation** — [Aggregate functions](functions/aggregate-functions.md).
- **Scalar calls** — [Builtin functions](functions/builtin-functions.md).

## Types and constraints

- **[Data types](data-types/index.md)** — SQL names and shared scalar identities.
- **[Constraints](constraints.md)** — Primary keys, unique keys, foreign keys, checks, and nullability.

## System views

The [System views](system-views/index.md) section lists all supported metadata columns.

- **[INFORMATION_SCHEMA](system-views/information-schema.md)** — The standard metadata subset.
- **[COHESION_SCHEMA](system-views/cohesion-schema.md)** — Index and ownership extensions.

## Errors and exclusions

- **[Diagnostics](diagnostics.md)** — Language codes, transaction diagnostics, and wire errors.
- **[Unsupported and reserved syntax](unsupported.md)** — The complete documented exclusion matrix.

## Engine context

[SQL](../index.md) describes the engine, transport, catalog, and client packages.
The [SELECT examples](statements/select.md#examples) include the shared fixture and a complete
typed-client program.

## Sources

- **Declared contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Profile and vocabulary** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlLanguageProfile.cs`
- **Parser architecture** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/OVERVIEW.md`
- **Execution evidence** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`

