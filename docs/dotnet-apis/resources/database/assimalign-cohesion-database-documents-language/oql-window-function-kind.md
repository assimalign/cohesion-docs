# OqlWindowFunctionKind

The executable ranking and navigation function identity exposed by OqlFunction.WindowKind.

Namespace: `Assimalign.Cohesion.Database.Documents.Language`.

## Signature

```csharp
public enum OqlWindowFunctionKind
{
    None, RowNumber, Rank, DenseRank, PercentRank, CumeDist, Ntile, Lag, Lead
}
```

## Members

| Value | Function / meaning |
| --- | --- |
| None (0) | Scalar or ordinary aggregate call |
| RowNumber (1) | ROW_NUMBER: one-based Int64 row number |
| Rank (2) | RANK: first peer's one-based Int64 position |
| DenseRank (3) | DENSE_RANK: one-based Int64 peer-group number |
| PercentRank (4) | PERCENT_RANK: Decimal (rank−1)/(rows−1), zero for a singleton |
| CumeDist (5) | CUME_DIST: Decimal last-peer position / partition rows |
| Ntile (6) | NTILE: one-based Int64 balanced bucket number |
| Lag (7) | LAG: preceding row's JSON value |
| Lead (8) | LEAD: following row's JSON value |

These eight require OVER. Deferred FIRST_VALUE, LAST_VALUE, NTH_VALUE and RATIO_TO_REPORT have
no executable enum member. See [functions and defaults](../../../../database/document/language/statements/select.md#windows).

## Example

```csharp
Console.WriteLine(OqlFunctions.Find("rank")!.WindowKind); // Rank
Console.WriteLine(OqlFunctions.Find("lower")!.WindowKind); // None
```

## Related reference

[OqlFunction](oql-function.md) · [OqlWindowExpression](oql-window-expression.md) · [Assembly](index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/OqlWindowFunctionKind.cs`.
