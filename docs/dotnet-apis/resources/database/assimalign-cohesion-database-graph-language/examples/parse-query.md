# Parse a query

Parse query text into the language-specific statement representation.

> **Status:** Partial.

This example is adapted from the package overview with its explicit namespace import. Parsing
produces a statement and diagnostics; it does not execute a database query.

```csharp
using Assimalign.Cohesion.Database.Graph.Language;

var parser = new GqlQueryParser();
var statement = (GqlQueryStatement)parser.Parse(
    "MATCH (a:Person {name: 'Alice'})-[r:KNOWS]->(b) RETURN b.name");
// Inspect statement.Diagnostics before consuming statement.GqlExpression.
```

[All examples](index.md) · [Assembly overview](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/docs/OVERVIEW.md`.
