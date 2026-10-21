# Examples

Examples for `Assimalign.Cohesion.Database.Sql` are derived from source documentation and code.

> **Status:** Partial.

The examples below retain real usage from project documentation, templates, fixtures, or tests. Test
examples require their original test project’s dependencies and supporting objects.

- **[Sql Schema Expression Tests](sql-schema-expression-tests.md)** — This example exercises `Assimalign.Cohesion.Database.Sql` through its co-located test source.
- **[Sql Database Scope Tests](sql-database-scope-tests.md)** — This example exercises `Assimalign.Cohesion.Database.Sql` through its co-located test source.
- **[Create a table and insert a row](create-table-and-insert.md)** — An embedded SQL engine creates a database, opens a session, and executes parameterized SQL.

[Assembly overview](../index.md) · [Design](../design.md)

[Builtin scalar examples](../../../../../database/sql/language/functions/builtin-functions.md#examples-and-signatures)
cover every T1 name and result. [Transaction clock examples](../../../../../database/sql/language/functions/builtin-functions.md#clock-temporal-and-uuid-functions)
show agreement across requests. [SqlFunctionCollection](../sql-function-collection.md#example)
registers an application function; [SqlFunctionContext](../sql-function-context.md#example) demonstrates
the public collation/cancellation contract and an application function that reads TransactionTimestamp,
matching NOW(). [ALTER TABLE](../../../../../database/sql/language/statements/alter-table.md#examples)
covers renames, defaults and nullability.

[ON CONFLICT](../../../../../database/sql/language/clauses/on-conflict.md#examples) covers skipped,
updated and inserted rows with RETURNING, query sources and DEFAULT VALUES, checked against
SqlUpsertExecutionTests/SqlUpsertWireTests. [Final-row foreign keys](../../../../../database/sql/language/constraints.md#update-final-row-examples)
shows self-reference moves and rejected dangling references from SqlUpdateForeignKeyTests.

## Additional reference pages

- **[Catalog storage and collation](catalog-storage-and-collation.md)** — Reference and usage.
- **[Examples](catalog.md)** — Reference and usage.
- **[Sql Catalog Collation Tests](sql-catalog-collation-tests.md)** — Reference and usage.
- **[Examples](storage.md)** — Reference and usage.

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/docs/OVERVIEW.md`.
