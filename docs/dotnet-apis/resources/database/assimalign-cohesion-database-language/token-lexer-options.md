# TokenLexerOptions

Configures lexical vocabulary and GQL left-arrow recognition.

Namespace: `Assimalign.Cohesion.Database.Language`.

## Signature

```csharp
public ref struct TokenLexerOptions
{
    public required ReadOnlySpan<string> Keywords { get; set; }
    public ReadOnlySpan<string> Functions { get; set; }
    public bool IsCaseSensitive { get; set; }
    public bool RecognizeLeftArrow { get; set; }
}
```

## Behavior and exceptions

RecognizeLeftArrow defaults to false. SQL/OQL tokenize a<-1 as a < -1 and a<--1 as less-than
followed by a -- comment, following PostgreSQL. GQL opts in so <- remains its incoming arrow.
The Boolean option does not change other token recognition or add validation exceptions.

## Example

```csharp
var options = new TokenLexerOptions { Keywords = [], RecognizeLeftArrow = false };
var lexer = new TokenLexer("a<-1", options);
foreach (var token in lexer) { Console.WriteLine(token.Type); }
// Identifier, LessThan, Minus, Integer.
```

## Related reference

[Assembly](index.md) · [QueryLanguageProfile](query-language-profile.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Language/src/Lexer/TokenLexerOptions.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Language/tests/TokenLexerLeftArrowTests.cs`.
