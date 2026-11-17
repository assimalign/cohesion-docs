# TextAnalyzer

A deterministic immutable built-in analyzer.

Namespace: `Assimalign.Cohesion.Database.Types.Text`.

## Signature

```csharp
public sealed class TextAnalyzer
{
    public static TextAnalyzer Simple { get; }
    public static TextAnalyzer English { get; }
    public TextAnalyzerDescriptor Descriptor { get; }
    public static TextAnalyzer Create(TextAnalyzerDescriptor descriptor);
    public TextVector Analyze(string text, TextWeight weight = TextWeight.D);
}
```

## Behavior and exceptions

Immutable built-in analyzer. Simple and English are version 1 with accents preserved.
Create resolves the exact descriptor; unknown names and unavailable versions raise TextException
with UnknownAnalyzer or AnalyzerVersionMismatch. A null descriptor or text throws
ArgumentNullException; an invalid weight throws ArgumentOutOfRangeException. Analyze accepts
empty text, tokenizes/folds deterministically and returns an immutable vector. Invalid Unicode
or excessive payload raises TextException. Stop words leave gaps; see the design for byte cutoffs.

## Related reference

[Assembly](index.md) · [Design](design.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Types.Text/src/TextAnalyzer.cs`.
