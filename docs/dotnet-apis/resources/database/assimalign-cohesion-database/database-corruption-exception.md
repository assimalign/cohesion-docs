# DatabaseCorruptionException

Reports detected storage or attached-index corruption at an engine boundary with database and page context.

Namespace: `Assimalign.Cohesion.Database`.

## Signature

```csharp
public sealed class DatabaseCorruptionException : DatabaseException
{
    public string Code { get; }
    public DatabaseName Database { get; }
    public PageId? PageId { get; }
    public static DatabaseCorruptionException Create(DatabaseName database,
        StorageCorruptionException cause, IEnumerable<Diagnostic>? additionalDiagnostics = null);
    public static DatabaseCorruptionException Create(DatabaseName database,
        IndexCorruptionException cause, IEnumerable<Diagnostic>? additionalDiagnostics = null);
}
```

DatabaseName is in this namespace, PageId and StorageCorruptionException in
`Assimalign.Cohesion.Database.Storage`, and Diagnostic in `Assimalign.Cohesion.Database.Types`.
IndexCorruptionException is in `Assimalign.Cohesion.Database.Indexing`.

## Behavior and exceptions

There is no public constructor. Create retains cause as InnerException, copies its optional PageId
and names the database and, when known, the page in Message. The message advises restoration from
a verified backup. Code is always `COHDBS003`; SQL and Key-Value Pair map it to SQLSTATE XX001.
The embedded Message has ordinary explanatory text; wire writers add the diagnostic prefix.
Diagnostics contains an
Error with this code plus supplied additional diagnostics, using DatabaseException's snapshot and
location ordering. Create throws ArgumentException for an empty database name and
ArgumentNullException for a null cause.

The index overload retains the IndexCorruptionException and its page, and adds COHDBI002
before supplied model diagnostics. Thus attached-index corruption produces COHDBS003,
COHDBI002 and the model code: COHSQLE041 (SQL), COHDBK005 (Key-Value Pair), COHDBD005
(Documents), COHDBG006 (Graph) or COHDBB006 (Blob). COHDBI002 has no assigned SQLSTATE here.

SQL, Key-Value Pair, Documents, Graph and Blob translate detected storage corruption on reads,
statements and transaction operations. Root create/open lifecycle translation also finds corruption
inside coded wrappers and aggregate branches after model cleanup, for synchronous and deferred
failures. An existing DatabaseCorruptionException is retained. Lifecycle translation preserves
existing model diagnostics and removes a duplicate common code before adding COHDBS003.
Graph, Documents and Blob refused opens also retain COHDBG018, COHDBD003 and COHDBB004,
respectively; Graph record-format refusal retains COHDBG017 too.

A damaged read fails instead of returning an absent row, key, document, node or blob. Legitimately
reclaimed records keep their absence behavior. Remote callers catch their model client exception;
the existing execution-failure path carries COHDBS003, with structured diagnostics where supported.

## Example

This uses the page and wrapped-cause values from the root corruption tests:

```csharp
using System;
using Assimalign.Cohesion.Database;
using Assimalign.Cohesion.Database.Storage;

var cause = new StorageCorruptionException((PageId)9L, "Page 9 failed checksum verification.");
var failure = DatabaseCorruptionException.Create("broken", cause);
Console.WriteLine(failure.Code); // COHDBS003
Console.WriteLine(failure.Database); // broken
Console.WriteLine((long)failure.PageId!.Value); // 9
Console.WriteLine(ReferenceEquals(failure.InnerException, cause)); // True
```

The model IndexFailureTranslationTests use the same attached-index cause and page:

```csharp
var indexCause = new IndexCorruptionException("index", 42, 0);
var indexFailure = DatabaseCorruptionException.Create("broken", indexCause);
Console.WriteLine(indexFailure.Diagnostics[0].Code); // COHDBS003
Console.WriteLine(indexFailure.Diagnostics[1].Code); // COHDBI002
Console.WriteLine((long)indexFailure.PageId!.Value); // 42
Console.WriteLine(ReferenceEquals(indexFailure.InnerException, indexCause)); // True
```

Import Assimalign.Cohesion.Database.Indexing for the index example. Engine translation supplies
the additional model diagnostic; the direct factory call above supplies none.

Catch the same type around `await engine.OpenDatabaseAsync("broken")` to inspect a detected
integrity failure. Other lifecycle failures retain their own exception types.

## Related reference

[Assembly](index.md) · [DatabaseException](database-exception.md) · [StorageCorruptionException](../assimalign-cohesion-database-storage/storage-corruption-exception.md) · [Recovery](../../../../database/overview.md#database-file-sets-and-lifecycle)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/IndexFailureTranslationTests.cs` and each model's corresponding test.

- `cohesion/resources/Database/Assimalign.Cohesion.Database/src/Exceptions/DatabaseCorruptionException.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database/src/DatabaseEngine.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database/tests/DatabaseCorruptionTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/StorageCorruptionTests.cs`.
