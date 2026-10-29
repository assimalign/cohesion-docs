# Language (SQL)

Reference for Cohesion's declared SQL dialect and its measured execution boundaries.

> **Status:** Partial. The profile measures 80 of 82 declared clauses against the live engine.

Cohesion SQL is a bounded Structured Query Language (SQL) implementation. Support is measured using
the engine's execution-case table: every advertised clause must have a passing case covering
results, state changes, or an intended semantic error. MERGE executes ordered actions and RETURNING.
WITH RECURSIVE adds a binding-only clause: recursive planning, EXPLAIN and execution refuse with
CK00001; acyclic lists execute. Counts describe named clauses, not complete
International Organization for Standardization (ISO) SQL compatibility.

Start with [Syntax conventions](syntax-conventions.md) to read the diagrams.
Every query executes within the session's database. Accepted clauses compose only within the
restrictions on their reference pages.

## Statements

The [Statements](statements/index.md) section groups data definition language (DDL), data
manipulation language (DML), and transaction control.

- **DDL** — [CREATE SCHEMA](statements/create-schema.md), [DROP SCHEMA](statements/drop-schema.md),
  [CREATE TABLE](statements/create-table.md), [ALTER TABLE](statements/alter-table.md),
  [DROP TABLE](statements/drop-table.md), [CREATE INDEX](statements/create-index.md), and
  [DROP INDEX](statements/drop-index.md).
- **Sequences** — [CREATE](statements/create-sequence.md), [ALTER](statements/alter-sequence.md),
  [DROP](statements/drop-sequence.md), [session functions](functions/sequences.md),
  [NEXT VALUE FOR](expressions/next-value-for.md) and [DISCARD ALL](statements/discard-all.md).
- **DML** — [SELECT](statements/select.md), [INSERT](statements/insert.md),
  [UPDATE](statements/update.md), and [DELETE](statements/delete.md).
- **Table contents** — [TRUNCATE](statements/truncate.md) empties a complete table set with identity and FK policies.
- **Transactions** — [BEGIN](statements/begin-transaction.md),
  [COMMIT](statements/commit-transaction.md), and [ROLLBACK](statements/rollback-transaction.md).

[MERGE](statements/merge.md) executes ordered INSERT/UPDATE/DELETE actions and RETURNING. [EXPLAIN](statements/explain.md) describes SELECT/INSERT/UPDATE/DELETE/MERGE
without execution.

- **Views** — [CREATE [OR REPLACE] VIEW](statements/create-view.md) and [DROP VIEW](statements/drop-view.md).
- **Write results** — [INSERT/UPDATE/DELETE/MERGE RETURNING](clauses/returning.md) returns rows and affected counts.
- **Conflicting inserts** — [ON CONFLICT](clauses/on-conflict.md) skips conflicts or updates the newest eligible row with EXCLUDED.

## Clauses

The [Clauses](clauses/index.md) section describes binding and execution rules.

- **Sources and filtering** — [WITH](clauses/with.md), [FROM and derived tables](clauses/from.md), [JOIN](clauses/join.md), and
  [WHERE](clauses/where.md).
- **Grouping** — [GROUP BY, GROUPING SETS, ROLLUP and CUBE](clauses/group-by.md),
  [GROUPING masks](functions/grouping.md), [HAVING](clauses/having.md) and aggregate [FILTER](clauses/filter.md).
- **Windows** — [OVER and WINDOW](clauses/window.md), [ranking, navigation and frame values](functions/window-functions.md), with ROWS, RANGE offsets, GROUPS, EXCLUDE, NULL treatment and aggregate windows.
- **Set operations** — [UNION, INTERSECT and EXCEPT](clauses/set-operations.md), with ALL/DISTINCT in every query position.
- **Result shaping** — [ORDER BY](clauses/order-by.md), [LIMIT/OFFSET](clauses/limit-offset.md),
  [DISTINCT](clauses/distinct.md), and [COLLATE](clauses/collate.md).
- **Writes and defaults** — [VALUES](clauses/values.md), [SET](clauses/set.md), and
  [DEFAULT](clauses/default.md); [IDENTITY](clauses/identity.md) generates integer column values.

## Expressions

The [Expressions](expressions/index.md) section covers the executable scalar subset.

- **Computation** — [Operators and predicates](expressions/operators.md),
  [CASE](expressions/case.md), and [CAST](expressions/cast.md).
- **Predicates** — [NULL-safe distinct tests, Boolean tests and LIKE ESCAPE](expressions/operators.md)
  follow PostgreSQL precedence, with eager invalid-escape diagnostics.
- **Nested queries** — [Subqueries](expressions/subqueries.md) and [ANY/SOME/ALL comparisons](expressions/quantified-comparisons.md).
- **Values and names** — [Literals and identifiers](expressions/literals.md) and
  [Parameters](expressions/parameters.md).

## Functions

The [Functions](functions/index.md) section separates execution support from lexical recognition.

- **Aggregation** — [Aggregate functions](functions/aggregate-functions.md).
- **Scalar calls** — [42 T1 builtin names](functions/builtin-functions.md), with planning-time signatures,
  transaction-start clocks and bounded string results.
- **Temporal operators** — [Day-time INTERVAL arithmetic](expressions/operators.md#temporal-arithmetic).

## Types and constraints

- **[Data types](data-types/index.md)** — SQL names and shared scalar identities.
- **[Constraints](constraints.md)** — Primary keys, unique keys, foreign keys, checks, and nullability.

## System views

The [System views](system-views/index.md) section lists all supported metadata columns.

- **[INFORMATION_SCHEMA](system-views/information-schema.md)** — The standard metadata subset.
- **[COHESION_SCHEMA](system-views/cohesion-schema.md)** — Index, ownership, function and type extensions.

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
