# ROLLBACK TRANSACTION (Cohesion SQL)

Aborts the session's active explicit transaction.

> **Status:** Implemented.

## Syntax

```syntaxsql
ROLLBACK [ TRANSACTION ] [ ; ]
```

## Arguments

The optional `TRANSACTION` keyword changes no semantics; no savepoint target is accepted.

## Remarks

Rollback clears the session's active scope and undoes its row changes. Calling `ROLLBACK`
without an active transaction reports `COHSQLT002`. Disconnect also aborts an open transaction.

`ROLLBACK TO` is not implemented. It produces `SQL0003` rather than silently rolling back the
whole transaction. A rejected transaction-control statement leaves the active scope intact.
Completed DDL is self-committing and cannot be undone by a later rollback.

If client-side rollback cleanup fails, `ISqlConnection.AbortAsync` discards the rental instead
of returning an unreset session to the pool.

## Examples

Starting from the [conformance fixture](select.md#a-create-the-conformance-fixture), execute each
statement as a separate request on the same session or client connection:

```sql
BEGIN;
INSERT INTO t VALUES (4, 'new', 1);
DELETE FROM t WHERE id = 2;
ROLLBACK;
SELECT id FROM t ORDER BY id;
```

The final query returns the original identifiers `1`, `2`, and `3`.

## See also

[BEGIN](begin-transaction.md) · [COMMIT](commit-transaction.md) · [ROLLBACK](rollback-transaction.md)

[Statements](index.md) · [Diagnostics](../diagnostics.md)

## Sources

- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`
- **Transaction behavior** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlTransactionControlTests.cs`
- **Client lifetime** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Client/docs/DESIGN.md`
- **Coordinator** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Transactions/docs/OVERVIEW.md`

