# TextException

Text analysis and immutable value vocabulary.

Namespace: `Assimalign.Cohesion.Database.Types.Text`.

## Signature

```csharp
public sealed class TextException : Exception
{
    public TextErrorCode Code { get; }
}
```

## Behavior and exceptions

Independent child-root exception with no public constructor. Code identifies the Text
condition; it is not a CS/CG/CK diagnostic. No model translates these conditions yet. Messages
come from Text resources. Standard argument failures use argument exceptions instead.

## Related reference

[Assembly](index.md) · [Design](design.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Types.Text/src/Exceptions/TextException.cs`.
