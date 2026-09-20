# Example: File System Key Repository Tests

Exercise File System Key Repository behavior using the original repository tests.

[Examples](index.md) · [Assembly overview](../index.md)

## Test-project context

This is the complete `FileSystemKeyRepositoryTests.cs` listing from the package test project. Keep
it in that project when running it: the project supplies its package references, generated sources,
and any shared fixtures. The using block below makes the test-framework import explicit where the
original project supplies it globally.

## Code

```csharp
using System.IO;
using System.Text;
using System;
using Shouldly;
using Xunit;

namespace Assimalign.Cohesion.Security.DataProtection.Tests;

public class FileSystemKeyRepositoryTests
{
    private static readonly byte[] Sample = Encoding.UTF8.GetBytes("filesystem-sample");

    /// <summary>A unique temp directory that is removed when the test finishes.</summary>
    private sealed class TempDirectory : IDisposable
    {
        public TempDirectory()
        {
            Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "cohesion-dp-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(Path);
        }

        public string Path { get; }

        public void Dispose()
        {
            try
            {
                Directory.Delete(Path, recursive: true);
            }
            catch (IOException)
            {
                // Best-effort cleanup.
            }
        }
    }

    [Fact(DisplayName = "Cohesion Test [Security.DataProtection] - FileRepository: Should persist keys across provider instances")]
    public void Protect_ThenUnprotectFromNewProvider_ShouldRoundTripAcrossRestart()
    {
        using TempDirectory temp = new();

        // "Node A" protects and persists a key to the shared directory.
        IDataProtector nodeA = DataProtectionProvider
            .Create(KeyRepository.CreateFileSystem(temp.Path), o => o.ApplicationDiscriminator = "app")
            .CreateProtector("purpose");
        byte[] protectedData = nodeA.Protect(Sample);

        // "Node B" is a fresh provider over the same directory — as after a restart or on a
        // second instance. It must read Node A's key and unprotect the payload.
        IDataProtector nodeB = DataProtectionProvider
            .Create(KeyRepository.CreateFileSystem(temp.Path), o => o.ApplicationDiscriminator = "app")
            .CreateProtector("purpose");

        nodeB.Unprotect(protectedData).ShouldBe(Sample);
    }

    [Fact(DisplayName = "Cohesion Test [Security.DataProtection] - FileRepository: Should write one document per key")]
    public void Protect_ShouldWriteAKeyDocumentToDisk()
    {
        using TempDirectory temp = new();
        IKeyRepository repository = KeyRepository.CreateFileSystem(temp.Path);

        DataProtectionProvider.Create(repository).CreateProtector("purpose").Protect(Sample);

        Directory.GetFiles(temp.Path, "*.key").Length.ShouldBe(1);
        repository.GetAllKeys().Count.ShouldBe(1);
    }

    [Fact(DisplayName = "Cohesion Test [Security.DataProtection] - FileRepository: Should report no keys for an empty directory")]
    public void GetAllKeys_OnEmptyDirectory_ShouldBeEmpty()
    {
        using TempDirectory temp = new();

        KeyRepository.CreateFileSystem(temp.Path).GetAllKeys().ShouldBeEmpty();
    }

    [Fact(DisplayName = "Cohesion Test [Security.DataProtection] - FileRepository: Should replace an existing document by name")]
    public void StoreKey_OnSameName_ShouldOverwrite()
    {
        using TempDirectory temp = new();
        IKeyRepository repository = KeyRepository.CreateFileSystem(temp.Path);

        repository.StoreKey(new KeyDocument("dup", Encoding.UTF8.GetBytes("first")));
        repository.StoreKey(new KeyDocument("dup", Encoding.UTF8.GetBytes("second")));

        repository.GetAllKeys().Count.ShouldBe(1);
        Encoding.UTF8.GetString(repository.GetAllKeys()[0].Content.Span).ShouldBe("second");
    }
}
```

## Walkthrough

- **Covered behavior** — FileRepository: Should persist keys across provider instances.
- **Covered behavior** — FileRepository: Should write one document per key.
- **Covered behavior** — FileRepository: Should report no keys for an empty directory.
- **Covered behavior** — FileRepository: Should replace an existing document by name.

The assertions define the expected result or failure boundary. Test-only doubles supply controlled
inputs; they are not additional shipped APIs.

## Sources

- **Source** — `cohesion/libraries/Security/Assimalign.Cohesion.Security.DataProtection/tests/FileSystemKeyRepositoryTests.cs`.
- **Source** — `cohesion/libraries/Security/Assimalign.Cohesion.Security.DataProtection/tests/Assimalign.Cohesion.Security.DataProtection.Tests.csproj`.
