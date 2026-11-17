# TextWeight

Text analysis and immutable value vocabulary.

Namespace: `Assimalign.Cohesion.Database.Types.Text`.

## Signature

```csharp
public enum TextWeight : byte { D = 0, C = 1, B = 2, A = 3 }
```

## Behavior and exceptions

D is the default/lowest weight; A is highest. Duplicate positions retain the highest weight,
including at count and terminal-position cutoffs. Reading an enum value adds no exceptions;
TextPosition.Create and TextAnalyzer.Analyze reject values outside this range.

## Related reference

[Assembly](index.md) · [Design](design.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Types.Text/src/ValueObjects/TextWeight.cs`.
