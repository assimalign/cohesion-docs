# Assimalign.Cohesion.Database.Execution — Overview

The model-neutral request/result vocabulary of the Cohesion Data Platform:
`DatabaseRequest`, `DatabaseResult`, `DatabaseResultSet`, `DatabaseResultStatus`,
`DatabaseRow` and `DatabaseColumn`.

## Scope

- **Requests:** `DatabaseRequest` has no members. Model requests own their inputs. Language
  requests own a typed `Statement` and `Parameters`; key-value requests carry command operands.
- **Results:** `DatabaseResult` reports status, affected count and diagnostics.
  `DatabaseResultSet` streams `DatabaseRow` values described by `DatabaseColumn` metadata.
  Each model supplies the result implementations it needs.

Planning, parsing, execution and transaction brackets belong to model engines. Blob sessions
operate through their own storage API and have no execute member.

## Dependencies

Only `Database.Types`, which owns the shared diagnostic types as well as value types.
There is no query-language dependency, direct or transitive. The root `Database` assembly
references this package and owns `DatabaseExecutionSession<TRequest>` and
`DatabaseLanguageSession<TRequest>`; this package does not reference the root.

## Usage

```csharp
// session is a SqlDatabaseSession; only SqlQueryRequest or SQL text can be executed here.
DatabaseResult result = await session.ExecuteAsync(SqlQueryRequest.FromSql("SELECT id FROM t"));
if (result is DatabaseResultSet rows)
{
    await using var resultLifetime = rows;
    await foreach (DatabaseRow row in rows.GetRowsAsync())
    {
        // Consume the model's row values.
    }
}
```

Sql, Documents and Graph offer typed and text execution. KeyValuePair offers typed execution
only; its server uses an internal command-text parser. A variable typed as `DatabaseSession`
offers lifecycle, state and transactions. Use the concrete model session to execute its requests.

See [Design](design.md) for the decisions and deferred result-contract work.

[Examples](examples/index.md) · [Parent: Database](../index.md)

- **[DatabasePlanDescription](database-plan-description.md)** — Public contract and usage.

- **[DatabasePlanNode](database-plan-node.md)** — Public contract and usage.

- **[DatabaseRow](database-row.md)** — Public contract and usage.

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Execution/docs/OVERVIEW.md`.
