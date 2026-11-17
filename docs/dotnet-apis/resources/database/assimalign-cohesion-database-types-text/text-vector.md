# TextVector

An immutable sorted tsvector-style value.

Namespace: `Assimalign.Cohesion.Database.Types.Text`.

## Signature

```csharp
public sealed class TextVector : IEquatable<TextVector>
{
    public const int MaxLexemeBytes = 2047;
    public const int MaxPosition = 16383;
    public const int MaxPositionsPerLexeme = 256;
    public const int MaxPayloadBytes = (1 << 20) - 1;
    public static TextVector Empty { get; }
    public IReadOnlyList<TextLexeme> Lexemes { get; }
    public static TextVector Create(IEnumerable<TextLexeme> lexemes);
    public bool Equals(TextVector? other);
    public override bool Equals(object? obj);
    public override int GetHashCode();
}
```

## Behavior and exceptions

Immutable tsvector-style value. Create snapshots explicit lexemes without analysis, merges
duplicate text and sorts by Unicode scalar/UTF-8 byte order. Positions sort, deduplicate with
highest weight and retain the earliest 256 (analysis retains 255). Equality is structural.
A null sequence or element throws ArgumentNullException. Payload above 1,048,575 bytes raises
TextException with VectorTooLarge. No public payload-size property, index or wire format is implied.

## Related reference

[Assembly](index.md) · [Design](design.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Types.Text/src/TextVector.cs`.
