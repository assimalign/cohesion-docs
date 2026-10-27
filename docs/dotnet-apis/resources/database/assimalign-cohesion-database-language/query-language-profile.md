# QueryLanguageProfile

Immutable vocabulary and clause admission for a query language.

Namespace: `Assimalign.Cohesion.Database.Language`.

## Signature

```csharp
public sealed class QueryLanguageProfile
{
    public QueryLanguageProfile(string language, string[] keywords, string[] functions,
        string[] clauses, bool isCaseSensitive = false, bool recognizeLeftArrow = false);
    public string Language { get; }
    public ReadOnlySpan<string> Keywords { get; }
    public ReadOnlySpan<string> Functions { get; }
    public IReadOnlyCollection<string> Clauses { get; }
    public bool IsCaseSensitive { get; }
    public bool RecognizeLeftArrow { get; }
    public bool Supports(string clause);
    public TokenLexerOptions ToLexerOptions();
}
```

## Behavior and exceptions

The constructor copies all vocabulary arrays and exposes read-only views. Null required
arguments or a null Supports clause throw ArgumentNullException. Matching is ordinal ignoring
case unless isCaseSensitive is true. recognizeLeftArrow defaults to false and is copied into
RecognizeLeftArrow and ToLexerOptions().RecognizeLeftArrow. GQL enables it; SQL and OQL leave
it false. <- is therefore an arrow only for an opted-in profile.

## Example

```csharp
var profile = new QueryLanguageProfile("example", [], [], ["MATCH"],
    recognizeLeftArrow: true);
Console.WriteLine(profile.RecognizeLeftArrow); // True
Console.WriteLine(profile.ToLexerOptions().RecognizeLeftArrow); // True
Console.WriteLine(profile.Supports("match")); // True
```

## Related reference

[Assembly](index.md) · [TokenLexerOptions](token-lexer-options.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Language/src/QueryLanguageProfile.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Language/tests/TokenLexerLeftArrowTests.cs`.
