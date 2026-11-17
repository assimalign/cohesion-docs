# TextErrorCode

Text analysis and immutable value vocabulary.

Namespace: `Assimalign.Cohesion.Database.Types.Text`.

## Signature

```csharp
public enum TextErrorCode
{
    UnknownAnalyzer, AnalyzerVersionMismatch, InvalidUnicode, InvalidLexeme, VectorTooLarge
}
```

## Behavior and exceptions

These values are independent Text conditions, outside model diagnostic identities.

| Value | Condition |
| --- | --- |
| UnknownAnalyzer | Exact case-sensitive analyzer name is unavailable |
| AnalyzerVersionMismatch | Requested version is unavailable |
| InvalidUnicode | Invalid Unicode scalar sequence |
| InvalidLexeme | Explicit lexeme is empty, contains a null or exceeds 2,047 UTF-8 bytes |
| VectorTooLarge | Lexeme/position payload exceeds 1,048,575 bytes |

No model translation exists yet; see the area [registry](../../../../database/error-codes.md#text-analysis-conditions).

## Related reference

[Assembly](index.md) · [Design](design.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Types.Text/src/Exceptions/TextErrorCode.cs`.
