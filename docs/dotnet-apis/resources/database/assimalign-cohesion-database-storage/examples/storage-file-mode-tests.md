# Storage File Mode Tests

This example exercises `Assimalign.Cohesion.Database.Storage` through its co-located test source.

> **Status:** Partial.

The example reproduces
`cohesion/resources/Database/Assimalign.Cohesion.Database.Storage/tests/StorageFileModeTests.cs`.
It retains the test class and assertions so the setup, operation, and expected outcome stay
together. `Use` it in the source project’s test context, with its test dependencies and supporting
test objects.

## Behavior exercised

- **Case 1** — OpenMissingFile_ShouldFailWithoutCreatingIt.
- **Case 2** — CreateNewExistingFile_ShouldPreserveContentAndOpenShouldRetainCapability.
- **Case 3** — `Dispose`.

## Source example

```csharp
using System;
using System.IO;
using Shouldly;
using Xunit;

namespace Assimalign.Cohesion.Database.Storage.Tests;

public sealed class StorageFileModeTests : IDisposable
{
    private readonly string _directory = Path.Combine(Path.GetTempPath(), "cohesion-storage-modes", Guid.NewGuid().ToString("N"));

    [Fact]
    public void OpenMissingFile_ShouldFailWithoutCreatingIt()
    {
        Directory.CreateDirectory(_directory);
        string path = Path.Combine(_directory, "missing.dat");

        Should.Throw<FileNotFoundException>(() => StorageStream.FromFile(path, FileMode.Open, FileShare.Read));

        File.Exists(path).ShouldBeFalse();
    }

    [Fact]
    public void CreateNewExistingFile_ShouldPreserveContentAndOpenShouldRetainCapability()
    {
        Directory.CreateDirectory(_directory);
        string path = Path.Combine(_directory, "existing.dat");
        using (var created = StorageStream.FromFile(path, FileMode.CreateNew, FileShare.Read))
        {
            created.Write("kept"u8);
            created.FlushDurable();
        }

        Should.Throw<IOException>(() => StorageStream.FromFile(path, FileMode.CreateNew, FileShare.Read));
        File.ReadAllBytes(path).ShouldBe("kept"u8.ToArray());

        using var reopened = StorageStream.FromFile(path, FileMode.Open, FileShare.Read);
        reopened.SupportsDurableFlush.ShouldBeTrue();
        reopened.Length.ShouldBe(4);
        using var reader = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        reader.ReadByte().ShouldBe((int)'k');
    }

    public void Dispose()
    {
        if (Directory.Exists(_directory))
        {
            Directory.Delete(_directory, recursive: true);
        }
    }
}
```

[All examples](index.md) · [Assembly overview](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Storage/tests/StorageFileModeTests.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Storage/tests/Assimalign.Cohesion.Database.Storage.Tests.csproj`.
