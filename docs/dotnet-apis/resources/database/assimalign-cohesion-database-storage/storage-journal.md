# StorageJournal

The sealed write-ahead journal assigns ordered LSNs, buffers frames and confirms written/durable prefixes.

Namespace: `Assimalign.Cohesion.Database.Storage`.

## Signatures

```csharp
public sealed class StorageJournal : IAsyncDisposable, IDisposable
{
    public static StorageJournal Create(Stream stream, bool leaveOpen = false);
    public static StorageJournal Create(StorageStream stream, bool leaveOpen = false);
    public static StorageJournal Create(IFileSystemFileHandle handle, bool leaveOpen = false);
    public long LastLsn { get; }
    public long WrittenLsn { get; }
    public long DurableLsn { get; }
    public long Length { get; }
    public long AppendBegin(long transactionSequence);
    public long AppendOperation(long transactionSequence, ReadOnlySpan<byte> payload);
    public long AppendCommit(long transactionSequence);
    public long AppendRollback(long transactionSequence);
    public long AppendPageImage(long transactionSequence, PageId pageId,
        JournalRecordType type, ReadOnlySpan<byte> image);
    public long Checkpoint(ReadOnlySpan<long> activeTransactions);
    public void EnsureDurable(long lsn);
    public void Flush(bool forceDurable = false);
    public IReadOnlyList<JournalRecord> ReadAll();
    public IEnumerable<JournalRecord> ReadSequential();
    public void Dispose();
    public ValueTask DisposeAsync();
}
```

Create requires a readable, writable, seekable stream. A raw Stream supplies no durable-flush
contract; StorageStream and IFileSystemFileHandle preserve their explicit durability support.
leaveOpen keeps the supplied medium open after journal disposal. The type has no public constructor.

## Appends and durability

Append methods return the assigned LSN. LastLsn includes buffered records; WrittenLsn covers
records handed to the medium, which may still have its own buffering; DurableLsn covers the prefix
confirmed by a durable flush. Length includes buffered frames since the last truncation.
AppendPageImage accepts an 8 KiB page and FullPageImage or CommittedPageImage, encoding byte runs.

The journal detaches a buffered prefix under its append lock, then releases that lock during the
device write and fsync. Concurrent appends fill a replacement buffer. A full replacement waits for
the medium outside the append lock. Two retained buffers are reused, each growing from 64 KiB to
at most 1 MiB; oversized frames use a pooled direct buffer. LSN order still matches physical order.

EnsureDurable drains and durably flushes when its LSN is not already covered. Waiters recheck the
durable prefix after acquiring the medium, allowing one fsync to cover several waiters. A force
confirms only its written prefix; appends made during that I/O need a later force. Flush drains
and performs an ordinary flush by default, or a durable flush when forceDurable is true.
Checkpoint and disposal wait for the medium; checkpoint pauses appends, drains and truncates
before writing its checkpoint record. Disposal closes appends and drains the remaining frames once.

A failed journal write or fsync latches StorageOfflineException before releasing the medium.
Later appends, flushes and checkpoints refuse work; no later prefix is acknowledged. A previously
durable LSN can still be confirmed. Public reads on an offline journal return only what the medium
holds and do not drain pending buffers. Reopen and recovery determine which records survived.

## Sequential reads and payload ownership

ReadSequential returns verifiable records in append order and stops at the first torn/corrupt frame.
Each public JournalRecord.Payload is an owned copy: it stays valid after MoveNext and enumerator
disposal, with no pooled-memory return required from the caller. ReadAll materializes records with
the same ownership guarantee. Both readers drain pending appends before reading when online.

Enumeration holds a synchronous medium lock and pauses appends until it completes or is disposed.
Consume it on one thread without awaiting or checkpointing. An append on that same thread while
enumeration is active throws InvalidOperationException. Other appenders resume after the enumeration
releases the medium. Early break, consumer exception or consumer cancellation must dispose the
enumerator; foreach handles this and restores the underlying stream position. ReadSequential has
no cancellation-token parameter.

Storage recovery and consistency checks use an internal borrowed reader whose payload expires at
the next MoveNext or disposal. That reader is not public. The [recovery design](design.md#empty-pages-and-streaming-journal-recovery)
describes one-pass initialization and the single immediate replay that can reuse verified checksums.
Public reads and later analyses verify checksums afresh.

## Exceptions

| Exception | Trigger |
| --- | --- |
| ArgumentNullException | Create receives a null stream or handle |
| ArgumentException | Create stream lacks read/write/seek support, or AppendPageImage is not given an 8 KiB page |
| ArgumentOutOfRangeException | AppendPageImage type is neither FullPageImage nor CommittedPageImage |
| InvalidOperationException | Append from inside an active journal enumeration on the same thread |
| JournalException | An operation/checkpoint frame would exceed the maximum frame size |
| ObjectDisposedException | Operation after journal disposal; also a borrowed medium already closed at the final disposal drain |
| NotSupportedException | Durable flush requested from a medium without that capability; this is not an fsync failure |
| StorageOfflineException | A journal write/flush/truncation failed, or a later write is refused by the offline latch |
| StorageFormatException | A checksum-verified frame has an unsupported version; it is refused rather than treated as a torn tail |

Closing a borrowed stream while the journal is live is a write/flush failure and takes it offline.
Only the final disposal drain can propagate ObjectDisposedException for that closed medium without
latching offline. Dispose releases its medium even when the final drain fails; repeated calls do nothing.

## Example

This adapts the payload-retention and reentrant-append cases in JournalTests:

```csharp
using System;
using System.IO;
using Assimalign.Cohesion.Database.Storage;

using var journal = StorageJournal.Create(new MemoryStream());
journal.AppendOperation(1, new byte[] { 4, 5, 6 });
journal.AppendCommit(1);
ReadOnlyMemory<byte> saved;
using (var records = journal.ReadSequential().GetEnumerator())
{
    records.MoveNext();
    saved = records.Current.Payload;
    records.MoveNext();
    try { journal.AppendBegin(2); }
    catch (InvalidOperationException) { Console.WriteLine("Enumeration active"); }
}
Console.WriteLine(journal.AppendBegin(2)); // 3
Console.WriteLine(saved.Span[0]);          // 4, still owned after MoveNext/disposal
```

MemoryStream permits ordinary reads/flushes but supplies no durable-flush guarantee.
JournalConcurrencyTests uses simulated durable handles to check appends during stalled I/O,
prefix confirmation, shared flushes and failure fencing.

## Related reference

[Assembly](index.md) · [Design](design.md) · [TransactionCoordinator](../assimalign-cohesion-database-transactions/transaction-coordinator.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Storage/src/Journal/StorageJournal.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Storage/tests/JournalTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Storage/tests/JournalConcurrencyTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Storage/tests/RecoveryAnalysisTests.cs`.
