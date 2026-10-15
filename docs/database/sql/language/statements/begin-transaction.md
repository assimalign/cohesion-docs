# BEGIN TRANSACTION (Cohesion SQL)

Starts the session's explicit transaction.

> **Status:** Implemented.

## Syntax

```syntaxsql
BEGIN [ TRANSACTION ] [ ; ]
```

## Arguments

The optional `TRANSACTION` keyword changes no semantics. There is no transaction name or
isolation-level argument in this syntax.

## Remarks

The session uses snapshot isolation for SQL `BEGIN`. Later statements on the same session join
that transaction; its own writes are visible to it and remain hidden from other sessions until commit.
A second `BEGIN` reports `COHSQLT001` and preserves the original active scope.

Data definition language (DDL) is rejected while the explicit transaction is active with
`COHSQLT003`. Statements outside an explicit transaction auto-commit. Session disposal or wire
disconnect rolls back an unfinished transaction. `TRANSACTION` alone is not a statement.

The coordinator also has an API for selecting an isolation level; SQL isolation-level syntax,
nested transactions, and savepoints are outside this language subset.

## Examples

Starting from the [conformance fixture](select.md#a-create-the-conformance-fixture), execute each
statement as a separate request on the same session or client connection:

```sql
BEGIN TRANSACTION;
INSERT INTO t VALUES (4, 'new', 1);
COMMIT TRANSACTION;
```

The inserted row becomes visible to an observing session after commit.

## See also

[BEGIN](begin-transaction.md) · [COMMIT](commit-transaction.md) · [ROLLBACK](rollback-transaction.md)

[Statements](index.md) · [Diagnostics](../diagnostics.md)

## Sources

- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`
- **Transaction behavior** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlTransactionControlTests.cs`
- **Client lifetime** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Client/docs/DESIGN.md`
- **Coordinator** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Transactions/docs/OVERVIEW.md`

