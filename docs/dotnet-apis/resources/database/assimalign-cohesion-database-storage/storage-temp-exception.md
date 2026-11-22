# StorageTempException

A coded refusal or integrity failure in disposable query storage.

Namespace: `Assimalign.Cohesion.Database.Storage`.

## Signature

```csharp
public sealed class StorageTempException : StorageException
{
    public const string EngineQuotaCode = "CK05001";
    public const string StatementQuotaCode = "CK05002";
    public const string FreeSpaceCode = "CK05003";
    public const string DamageCode = "CK05004";
    public const string UnavailableCode = "CK05005";
    public const string RecordTooLargeCode = "CK05006";
    public string Code { get; }
}
```

## Behavior and exceptions

Code is the stable identity, separate from the unprefixed message. There is no public constructor.

| Code | Condition |
| --- | --- |
| CK05001 | Engine physical-byte quota exceeded |
| CK05002 | Statement physical-byte quota exceeded |
| CK05003 | Next page crosses the free-space floor |
| CK05004 | Page/record validation failed, or the reader previously failed |
| CK05005 | Temporary storage unavailable, including lease, volume and file I/O failures |
| CK05006 | Encoded sort record exceeds its declared maximum |

Unavailable I/O retains its cause as InnerException. Failed-reader retries perform no I/O and
emit no further damage event/counter. These temporary failures do not fence durable storage
offline. No query operator uses the substrate yet.

## Related reference

[Space](storage-temp-space.md) · [Storage events](design.md#storage-eventsource) · [Error codes](../../../../database/error-codes.md#temporary-storage)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Storage/src/Exceptions/StorageTempException.cs`.

