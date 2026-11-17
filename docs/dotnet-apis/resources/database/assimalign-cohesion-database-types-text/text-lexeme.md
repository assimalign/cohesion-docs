# TextLexeme

Text analysis and immutable value vocabulary.

Namespace: `Assimalign.Cohesion.Database.Types.Text`.

## Signature

```csharp
public sealed class TextLexeme : IEquatable<TextLexeme>
{
    public string Text { get; }
    public IReadOnlyList<TextPosition> Positions { get; }
    public static TextLexeme Create(string text, IEnumerable<TextPosition>? positions = null);
    public bool Equals(TextLexeme? other);
    public override bool Equals(object? obj);
    public override int GetHashCode();
}
```

## Behavior and exceptions

Create preserves exact text without analysis. Null positions produce a stripped lexeme.
Positions are copied, sorted and deduplicated with highest weight, keeping the earliest 256.
Equality covers text and positions. Null text throws ArgumentNullException; empty text, null
characters or more than 2,047 UTF-8 bytes raises InvalidLexeme. Invalid scalar sequences raise
InvalidUnicode. A default position throws ArgumentOutOfRangeException. Separators are allowed.

## Related reference

[Assembly](index.md) · [Design](design.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Types.Text/src/TextLexeme.cs`.
