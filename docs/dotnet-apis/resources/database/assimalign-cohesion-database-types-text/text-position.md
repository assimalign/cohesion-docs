# TextPosition

A positive one-based token position and weight.

Namespace: `Assimalign.Cohesion.Database.Types.Text`.

## Signature

```csharp
public readonly record struct TextPosition
{
    public int Position { get; }
    public TextWeight Weight { get; }
    public static TextPosition Create(int position, TextWeight weight = TextWeight.D);
}
```

## Behavior and exceptions

One-based position with a two-bit weight. Create clamps positive values above 16,383.
Nonpositive input or an invalid weight throws ArgumentOutOfRangeException. The default struct
is invalid when supplied to TextLexeme.Create. Record equality includes position and weight.

## Related reference

[Assembly](index.md) · [Design](design.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Types.Text/src/ValueObjects/TextPosition.cs`.
