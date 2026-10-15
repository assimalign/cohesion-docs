# Example: Data Protection Behavioral Tests

Exercise file-system key persistence and purpose-scoped data protection using the original repository tests.

[Examples](index.md) · [Assembly overview](../index.md)

## Test-project context

These are complete test-file listings from the package test project. Keep them in that project
when running them: the project supplies its package references, generated sources, and any shared
fixtures. The using blocks make the test-framework import explicit where the original project
supplies it globally.

## Code

### FileSystemKeyRepositoryTests.cs

```csharp
using System;
using System.IO;
using System.Text;

using Shouldly;

using Xunit;

namespace Assimalign.Cohesion.Security.DataProtection.Tests;

public class FileSystemKeyRepositoryTests
{
    private static readonly byte[] _sample = Encoding.UTF8.GetBytes("filesystem-sample");

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
            .Create(FileSystemKeyRepository.Create(temp.Path), o => o.ApplicationDiscriminator = "app")
            .CreateProtector("purpose");
        byte[] protectedData = nodeA.Protect(_sample);

        // "Node B" is a fresh provider over the same directory — as after a restart or on a
        // second instance. It must read Node A's key and unprotect the payload.
        IDataProtector nodeB = DataProtectionProvider
            .Create(FileSystemKeyRepository.Create(temp.Path), o => o.ApplicationDiscriminator = "app")
            .CreateProtector("purpose");

        nodeB.Unprotect(protectedData).ShouldBe(_sample);
    }

    [Fact(DisplayName = "Cohesion Test [Security.DataProtection] - FileRepository: Should write one document per key")]
    public void Protect_ShouldWriteAKeyDocumentToDisk()
    {
        using TempDirectory temp = new();
        FileSystemKeyRepository repository = FileSystemKeyRepository.Create(temp.Path);

        DataProtectionProvider.Create(repository).CreateProtector("purpose").Protect(_sample);

        Directory.GetFiles(temp.Path, "*.key").Length.ShouldBe(1);
        repository.GetAllKeys().Count.ShouldBe(1);
    }

    [Fact(DisplayName = "Cohesion Test [Security.DataProtection] - FileRepository: Should report no keys for an empty directory")]
    public void GetAllKeys_OnEmptyDirectory_ShouldBeEmpty()
    {
        using TempDirectory temp = new();

        FileSystemKeyRepository.Create(temp.Path).GetAllKeys().ShouldBeEmpty();
    }

    [Fact(DisplayName = "Cohesion Test [Security.DataProtection] - FileRepository: Should replace an existing document by name")]
    public void StoreKey_OnSameName_ShouldOverwrite()
    {
        using TempDirectory temp = new();
        FileSystemKeyRepository repository = FileSystemKeyRepository.Create(temp.Path);

        repository.StoreKey(new KeyDocument("dup", Encoding.UTF8.GetBytes("first")));
        repository.StoreKey(new KeyDocument("dup", Encoding.UTF8.GetBytes("second")));

        repository.GetAllKeys().Count.ShouldBe(1);
        Encoding.UTF8.GetString(repository.GetAllKeys()[0].Content.Span).ShouldBe("second");
    }
}
```

### DataProtectorTests.cs

```csharp
using System;
using System.Text;

using Shouldly;

using Xunit;

namespace Assimalign.Cohesion.Security.DataProtection.Tests;

public class DataProtectorTests
{
    private static readonly byte[] _sample = Encoding.UTF8.GetBytes("cohesion-data-protection-sample-payload");

    private static DataProtectionProvider CreateProvider(string discriminator = "app")
    {
        return DataProtectionProvider.Create(
            new InMemoryKeyRepository(),
            options => options.ApplicationDiscriminator = discriminator);
    }

    [Fact(DisplayName = "Cohesion Test [Security.DataProtection] - Protect: Should round-trip a payload")]
    public void Protect_ThenUnprotect_ShouldReturnOriginalPlaintext()
    {
        IDataProtector protector = CreateProvider().CreateProtector("purpose");

        byte[] protectedData = protector.Protect(_sample);
        byte[] recovered = protector.Unprotect(protectedData);

        recovered.ShouldBe(_sample);
    }

    [Fact(DisplayName = "Cohesion Test [Security.DataProtection] - Protect: Should round-trip an empty payload")]
    public void Protect_OnEmptyPlaintext_ShouldRoundTrip()
    {
        IDataProtector protector = CreateProvider().CreateProtector("purpose");

        byte[] recovered = protector.Unprotect(protector.Protect(ReadOnlySpan<byte>.Empty));

        recovered.ShouldBeEmpty();
    }

    [Fact(DisplayName = "Cohesion Test [Security.DataProtection] - Protect: Should produce a fresh ciphertext each call")]
    public void Protect_OnRepeatedCalls_ShouldProduceDistinctPayloads()
    {
        IDataProtector protector = CreateProvider().CreateProtector("purpose");

        byte[] first = protector.Protect(_sample);
        byte[] second = protector.Protect(_sample);

        // Random nonce per call ⇒ distinct payloads that both still decrypt.
        first.ShouldNotBe(second);
        protector.Unprotect(first).ShouldBe(_sample);
        protector.Unprotect(second).ShouldBe(_sample);
    }

    [Fact(DisplayName = "Cohesion Test [Security.DataProtection] - Protect: Should embed a versioned key-id header")]
    public void Protect_ShouldEmitVersionAndKeyIdHeader()
    {
        IDataProtector protector = CreateProvider().CreateProtector("purpose");

        byte[] protectedData = protector.Protect(_sample);

        // [version:1][keyId:16][nonce:12][ciphertext][tag:16]
        protectedData.Length.ShouldBeGreaterThan(45);
        protectedData[0].ShouldBe((byte)0x01);
        // The key id is a non-zero GUID in bytes 1..17.
        Guid keyId = new(protectedData.AsSpan(1, 16));
        keyId.ShouldNotBe(Guid.Empty);
    }

    [Fact(DisplayName = "Cohesion Test [Security.DataProtection] - Unprotect: Should reject a tampered payload")]
    public void Unprotect_OnTamperedCiphertext_ShouldThrow()
    {
        IDataProtector protector = CreateProvider().CreateProtector("purpose");
        byte[] protectedData = protector.Protect(_sample);
        protectedData[^1] ^= 0xFF; // flip a tag byte

        Should.Throw<DataProtectionException>(() => protector.Unprotect(protectedData));
    }

    [Fact(DisplayName = "Cohesion Test [Security.DataProtection] - Unprotect: Should reject a malformed payload")]
    public void Unprotect_OnMalformedPayload_ShouldThrow()
    {
        IDataProtector protector = CreateProvider().CreateProtector("purpose");

        Should.Throw<DataProtectionException>(() => protector.Unprotect(new byte[] { 1, 2, 3 }));
    }

    [Fact(DisplayName = "Cohesion Test [Security.DataProtection] - Purpose: Should isolate different purposes")]
    public void Unprotect_OnDifferentPurpose_ShouldThrow()
    {
        IDataProtectionProvider provider = CreateProvider();
        IDataProtector purposeA = provider.CreateProtector("purpose-a");
        IDataProtector purposeB = provider.CreateProtector("purpose-b");

        byte[] protectedData = purposeA.Protect(_sample);

        Should.Throw<DataProtectionException>(() => purposeB.Unprotect(protectedData));
        purposeA.Unprotect(protectedData).ShouldBe(_sample);
    }

    [Fact(DisplayName = "Cohesion Test [Security.DataProtection] - Purpose: Should isolate different applications sharing a key ring")]
    public void Unprotect_OnDifferentDiscriminator_ShouldThrow()
    {
        // Both apps share the same repository (and therefore the same ring key), but their
        // discriminators fold into the subkey derivation, so payloads do not cross-validate.
        InMemoryKeyRepository shared = new();
        IDataProtector appOne = DataProtectionProvider
            .Create(shared, o => o.ApplicationDiscriminator = "app-one")
            .CreateProtector("purpose");
        IDataProtector appTwo = DataProtectionProvider
            .Create(shared, o => o.ApplicationDiscriminator = "app-two")
            .CreateProtector("purpose");

        byte[] protectedData = appOne.Protect(_sample);

        Should.Throw<DataProtectionException>(() => appTwo.Unprotect(protectedData));
    }

    [Fact(DisplayName = "Cohesion Test [Security.DataProtection] - Purpose: Chained and params derivation should agree")]
    public void CreateProtector_ChainedAndParams_ShouldDeriveTheSameSubkey()
    {
        IDataProtectionProvider provider = CreateProvider();
        IDataProtector chained = provider.CreateProtector("outer").CreateProtector("inner");
        IDataProtector viaParams = provider.CreateProtector("outer", "inner");

        byte[] protectedData = chained.Protect(_sample);

        viaParams.Unprotect(protectedData).ShouldBe(_sample);
    }

    [Fact(DisplayName = "Cohesion Test [Security.DataProtection] - Purpose: Sub-purpose should isolate from its parent")]
    public void CreateProtector_SubPurpose_ShouldNotUnprotectParentPayload()
    {
        IDataProtectionProvider provider = CreateProvider();
        IDataProtector parent = provider.CreateProtector("outer");
        IDataProtector child = parent.CreateProtector("inner");

        byte[] parentData = parent.Protect(_sample);

        Should.Throw<DataProtectionException>(() => child.Unprotect(parentData));
    }
}
```

## Walkthrough

### FileSystemKeyRepositoryTests

- **Covered behavior** — FileRepository: Should persist keys across provider instances.
- **Covered behavior** — FileRepository: Should write one document per key.
- **Covered behavior** — FileRepository: Should report no keys for an empty directory.
- **Covered behavior** — FileRepository: Should replace an existing document by name.

### DataProtectorTests

- **Covered behavior** — Protect: Should round-trip a payload.
- **Covered behavior** — Protect: Should round-trip an empty payload.
- **Covered behavior** — Protect: Should produce a fresh ciphertext each call.
- **Covered behavior** — Protect: Should embed a versioned key-id header.
- **Covered behavior** — Unprotect: Should reject a tampered payload.
- **Covered behavior** — Unprotect: Should reject a malformed payload.
- **Covered behavior** — Purpose: Should isolate different purposes.
- **Covered behavior** — Purpose: Should isolate different applications sharing a key ring.
- **Covered behavior** — Purpose: Chained and params derivation should agree.
- **Covered behavior** — Purpose: Sub-purpose should isolate from its parent.

The assertions define the expected result or failure boundary. Test-only doubles supply controlled
inputs; they are not additional shipped APIs.

## Sources

- **Source** — `cohesion/libraries/Security/Assimalign.Cohesion.Security.DataProtection/tests/FileSystemKeyRepositoryTests.cs`.
- **Source** — `cohesion/libraries/Security/Assimalign.Cohesion.Security.DataProtection/tests/DataProtectorTests.cs`.
- **Source** — `cohesion/libraries/Security/Assimalign.Cohesion.Security.DataProtection/tests/Assimalign.Cohesion.Security.DataProtection.Tests.csproj`.
