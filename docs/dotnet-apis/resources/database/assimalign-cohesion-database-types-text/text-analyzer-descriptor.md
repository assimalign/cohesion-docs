# TextAnalyzerDescriptor

The complete persisted identity of an analyzer.

Namespace: `Assimalign.Cohesion.Database.Types.Text`.

## Signature

```csharp
public sealed class TextAnalyzerDescriptor : IEquatable<TextAnalyzerDescriptor>
{
    public static TextAnalyzerDescriptor Simple { get; }
    public static TextAnalyzerDescriptor English { get; }
    public string Name { get; }
    public int Version { get; }
    public bool FoldAccents { get; }
    public static TextAnalyzerDescriptor Create(string name, bool foldAccents = false);
    public static TextAnalyzerDescriptor Create(string name, int version, bool foldAccents = false);
    public bool Equals(TextAnalyzerDescriptor? other);
    public override bool Equals(object? obj);
    public override int GetHashCode();
}
```

## Behavior and exceptions

Persist Name, Version and FoldAccents together. Names compare case-sensitively. The first
Create overload selects version 1, including for an unknown name; the second represents any int
version, including zero/negative values. Creation only rejects null names (ArgumentNullException).
TextAnalyzer.Create checks availability; no identity silently upgrades. Equality covers all three fields.

## Related reference

[Assembly](index.md) · [Design](design.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Types.Text/src/TextAnalyzerDescriptor.cs`.
