# QueryAnalyzerContext

Namespace: `Assimalign.Cohesion.Database.Language`.

```csharp
public sealed class QueryAnalyzerContext
{
    public QueryStatement Statement { get; }
}
```

The parser supplies the parsed statement to a QueryAnalyzer. The context has no public constructor and is now sealed. Analyzer execution/cancellation and diagnostic collection keep their existing contracts.

[Language assembly](index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Language/src/QueryAnalyzerContext.cs`.
