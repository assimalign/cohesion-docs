# DatabaseRow

Provides typed result-cell access and an optional capability for distinguishing MISSING from NULL.

Namespace: `Assimalign.Cohesion.Database.Execution`.

## Changed members

```csharp
protected DatabaseRow(bool supportsMissing = false);
public bool SupportsMissing { get; }
public bool IsMissing(int ordinal);
protected virtual bool IsMissingCore(int ordinal);
public abstract bool IsNull(int ordinal);
public abstract object? GetValue(int ordinal);
```

## Behavior and exceptions

Documents document-projection rows and Documents.Client.DocumentRow support MISSING. Shared
plan-description rows retain the default capability. SQL, Graph and Key-Value rows do not. IsMissing
checks capability before ordinal validation. Unsupported rows throw NotSupportedException even for
an invalid ordinal; supporting rows reject negative or out-of-range ordinals with
ArgumentOutOfRangeException. A subclass opting in implements IsMissingCore.

For Documents, IsMissing is true only for absence and IsNull only for explicit NULL. GetValue
returns null for either; check the capability and IsMissing first. MISSING projections are omitted
from JSON objects on the wire. A column's MISSING cells do not erase its observed scalar type.
Top-level OQL VALUE/RAW normalizes MISSING to NULL before DISTINCT and counting: SupportsMissing
remains true, IsMissing is false and IsNull is true. GetBytes(0) for this bare NULL is UTF-8 `null`.

OQL COUNT, LENGTH, POSITION, ARRAY_LENGTH and epoch-millisecond calls expose Int64 columns with
long values, in-process and over the wire; use GetInt64 for those cells. Their ordinary NULL/MISSING
rules still apply. Mixed Int64/Decimal columns promote to Decimal. See [scalar result types](../../../../database/document/language/functions/scalar-functions.md).

## Example

With a Documents session and one stored `{}` document in items:

```csharp
await using var result = (DatabaseResultSet)await session.ExecuteAsync(
    "SELECT absent AS m, NULL AS n FROM items", cancellationToken: CancellationToken.None);
await foreach (var row in result.GetRowsAsync())
{
    Console.WriteLine(row.SupportsMissing); // True
    Console.WriteLine(row.IsMissing(0));    // True
    Console.WriteLine(row.IsNull(0));       // False
    Console.WriteLine(row.IsNull(1));       // True
}
```

Documents generated JSON bytes use the shared [RFC 8259 encoder](../../../../database/document/language/statements/select.md#json-bytes),
including bare VALUE/RAW GetBytes. Unicode stays literal UTF-8 across in-process, InMemory and TCP.

## Related reference

[Assembly](index.md) · [OQL MISSING](../../../../database/document/language/expressions/missing.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Execution/src/DatabaseRow.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentMissingTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Client/tests/DocumentMissingWireTests.cs`.
