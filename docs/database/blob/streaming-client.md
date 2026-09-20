# Blob streaming client

The typed Blob client streams content and metadata over a leased database connection.

> **Status:** Implemented.

`Assimalign.Cohesion.Database.Blob.Client` supplies `BlobClient`, `IBlobClient`, and
`IBlobConnection`. `BlobClient.Create` combines `DatabaseConnectionSettings` with an application
supplied `IConnectionFactory`. The shared `Assimalign.Cohesion.Database.Client` package owns
dialing, authentication, framing, and pooling. The Blob package owns its typed exchanges.

## Example

This is the client overview's operation sequence, wrapped in a method with explicit inputs and
usings. The supplied settings select an existing database at a Blob endpoint; the `images`
container must already exist. The caller supplies and owns the source and destination streams.

```csharp
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Assimalign.Cohesion.Connections;
using Assimalign.Cohesion.Database.Blob;
using Assimalign.Cohesion.Database.Blob.Client;
using Assimalign.Cohesion.Database.Client;

public static class BlobStreamingExample
{
    public static async Task RunAsync(
        DatabaseConnectionSettings settings,
        IConnectionFactory connectionFactory,
        Stream source,
        Stream destination,
        CancellationToken cancellationToken)
    {
        await using var client = BlobClient.Create(new BlobClientOptions
        {
            Settings = settings,
            ConnectionFactory = connectionFactory,
        });
        await using var connection = await client.ConnectAsync(cancellationToken);

        long committed = await connection.UploadAsync("images", "cover", source,
            contentType: "image/png", cancellationToken: cancellationToken);
        Console.WriteLine(committed);

        await using (Stream download = await connection.DownloadAsync(
            "images", "cover", cancellationToken))
        {
            await download.CopyToAsync(destination, cancellationToken);
        }

        BlobProperties? properties = await connection.GetPropertiesAsync(
            "images", "cover", cancellationToken);
        Console.WriteLine(properties);

        await foreach (BlobProperties item in connection.GetBlobsAsync(
            "images", "covers/", cancellationToken))
        {
            Console.WriteLine(item.Name);
        }

        bool deleted = await connection.DeleteAsync("images", "cover", cancellationToken);
        Console.WriteLine(deleted);
    }
}
```

## Upload contract

`UploadAsync` reads from the source's current position and leaves it open. The source need not
seek or expose a length. `length: -1`, the default, means unknown remaining length. A
nonnegative declared length must match exactly. The default content type is
`application/octet-stream`, and `overwrite` defaults to true. Set `overwrite: false` to reject
an existing object.

The returned `long` is the committed content length. Success means the server acknowledged
atomic publication after commit. A failure before commit leaves no partial object. A lost
acknowledgement after commit leaves an uncertain outcome, so an automatic retry is not an
exactly-once operation.

## Download contract

`DownloadAsync` returns a nonseekable, read-only `Stream` after validating transfer metadata.
Read it to end of file (EOF), then dispose it. Both synchronous and asynchronous reads work.
The implementation uses a bounded queue of 64 kibibyte (KiB) chunks; neither endpoint requires
an object-sized buffer. The server must use file-backed storage for content larger than memory.

The token supplied to `DownloadAsync` remains active after the method returns. Canceling a
`ReadAsync` token cancels the whole download. Early disposal aborts an unfinished exchange and
closes its connection. Errors after startup surface from reads and remain failures on subsequent
reads; they cannot turn into successful EOF. Discard any partial content if a later read fails.

## Connection lifetime and metadata

One `IBlobConnection` admits one active exchange. A live download or listing holds that exchange
until completion or disposal. Dispose returned streams and enumerators before another operation.
Dispose connections before the client; healthy leases return to the shared pool. Disposing a
connection cancels and waits for its active exchange.

`GetPropertiesAsync` returns `BlobProperties?`, with null for an absent object. `DeleteAsync`
returns false when the object is absent. `GetBlobsAsync` returns an asynchronous metadata
enumeration with an optional ordinal prefix; null selects all objects.

## Diagnostics

`BlobClientException.Code` preserves the server's `ProtocolErrorCode`. Local malformed or
truncated frames map to `ProtocolViolation`; transport failures map to `Internal`. Every failed
Blob exchange closes its connection, including rejected metadata operations. Rent a fresh
connection after failure.

Argument validation and lifetime errors are ordinary .NET exceptions: the connection contract
documents `ArgumentNullException`, `ArgumentException`, `InvalidOperationException`,
`ObjectDisposedException`, and `OperationCanceledException` for their applicable cases.

## Verification in the source tree

The client tests cover nonseekable sources, bounded reads, empty objects, ordered prefixes,
property lookup, deletion, and mismatched lengths. The process fixture round-trips 256 mebibytes
(MiB) plus 123 bytes through the server and client with a 64 MiB managed heap limit, validating
the length, Secure Hash Algorithm 256-bit (SHA-256) digest, and bounded source reads. This is a
measured test scenario, not a maximum size.

## See also

- **Blob** — [Engine overview](index.md).
- **Operations** — [Publication and response semantics](operations.md).
- **Wire protocol** — [Transfer sequencing](wire-protocol.md).
- **Database** — [Database documentation](../index.md).

## Sources

- **Client overview and example** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Blob.Client/docs/OVERVIEW.md`.
- **Client options** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Blob.Client/src/BlobClientOptions.cs`.
- **Connection contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Blob.Client/src/Abstractions/IBlobConnection.cs`.
- **Behavior tests** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Blob.Client/tests/BlobClientTests.cs`.
- **Measured fixture** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Blob.Client/README.md`.
