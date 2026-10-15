# Parse a query

Parse query text into the language-specific statement representation.

> **Status:** Partial.

This example is adapted from the package overview with its explicit namespace import. Parsing
produces a statement and diagnostics; it does not execute a database query.

```csharp
using Assimalign.Cohesion.Database.Documents.Language;

var parser = new OqlQueryParser();
var statement = (OqlQueryStatement)parser.Parse(
    "SELECT p.name, p.orders[0].total AS total FROM people p WHERE p.age >= $minimum ORDER BY p.name");

// Check statement.Diagnostics for errors before passing its AST to planning.
var query = statement.OqlExpression;
```

[All examples](index.md) · [Assembly overview](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/docs/OVERVIEW.md`.
