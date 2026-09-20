# Parse a query

Parse query text into the language-specific statement representation.

> **Status:** Partial.

This example is adapted from the package overview with its explicit namespace import. Parsing
produces a statement and diagnostics; it does not execute a database query.

```csharp
using Assimalign.Cohesion.Database.Sql.Language;

var parser = new SqlQueryParser();
var statement = (SqlQueryStatement)parser.Parse("SELECT id FROM users WHERE age >= 21;");

var select = (SqlSelectExpression)statement.SqlExpression;
```

[All examples](index.md) · [Assembly overview](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/OVERVIEW.md`.
