# Certificate Authority Options Tests

This example exercises `Assimalign.Cohesion.SecretStore` through its co-located test source.

> **Status:** Partial.

The example reproduces
`cohesion/resources/SecretStore/Assimalign.Cohesion.SecretStore/tests/CertificateAuthorityOptionsTests.cs`
. It retains the test class and assertions so the setup, operation, and expected outcome stay
together. `Use` it in the source project’s test context, with its test dependencies and supporting
test objects.

## Behavior exercised

- **Case 1** — `CertificateAuthorityOptions`: defaults to standalone self-seeding.
- **Case 2** — Builder declarations: compose through the root contract.

## Source example

```csharp
using System;
using Shouldly;
using Xunit;
using Assimalign.Cohesion.SecretStore.Tests.TestObjects;

namespace Assimalign.Cohesion.SecretStore.Tests;

public sealed class CertificateAuthorityOptionsTests
{
    [Fact(DisplayName = "Cohesion Test [SecretStore] - CertificateAuthorityOptions: defaults to standalone self-seeding")]
    public void CertificateAuthorityOptions_WhenCreated_ShouldDefaultToStandaloneSelfSeeding()
    {
        // Arrange and Act
        CertificateAuthorityOptions options = new();

        // Assert
        options.CommonName.ShouldBe("Cohesion SecretStore Certificate Authority");
        options.SelfSeedWhenNoPlatform.ShouldBeTrue();
        options.PlatformEnrollmentEndpoint.ShouldBeNull();
        options.PlatformCertificate.ShouldBeNull();
        options.InitialCertificate.ShouldBeNull();
        options.InitialPrivateKey.ShouldBeNull();
    }

    [Fact(DisplayName = "Cohesion Test [SecretStore] - Builder declarations: compose through the root contract")]
    public void AddSecretAndCertificateAuthority_WithRootBuilder_ShouldCaptureDeclarativeInputs()
    {
        // Arrange
        byte[] secret = [1, 2, 3];
        byte[] platformCertificate = [4, 5, 6];
        Uri enrollmentEndpoint = new("https://platform.example.test/secrets/v1/enroll");
        RecordingSecretStoreApplicationBuilder builder = new();

        // Act
        ISecretStoreApplicationBuilder returned = builder
            .AddSecret("apps/api/client-secret", secret)
            .AddCertificateAuthority(options =>
            {
                options.CommonName = "AppA Intermediate CA";
                options.SelfSeedWhenNoPlatform = false;
                options.PlatformEnrollmentEndpoint = enrollmentEndpoint;
                options.PlatformCertificate = platformCertificate;
            });

        secret[0] = 9;

        // Assert
        returned.ShouldBeSameAs(builder);
        builder.Secrets.Count.ShouldBe(1);
        builder.Secrets[0].Path.ShouldBe("apps/api/client-secret");
        builder.Secrets[0].Value.ShouldBe([1, 2, 3]);
        CertificateAuthorityOptions options = builder.CertificateAuthority.ShouldNotBeNull();
        options.CommonName.ShouldBe("AppA Intermediate CA");
        options.SelfSeedWhenNoPlatform.ShouldBeFalse();
        options.PlatformEnrollmentEndpoint.ShouldBe(enrollmentEndpoint);
        options.PlatformCertificate.ShouldNotBeNull();
        options.PlatformCertificate.Value.ToArray().ShouldBe([4, 5, 6]);
    }
}
```

[All examples](index.md) · [Assembly overview](../index.md)

## Sources

- **Primary source** — `cohesion/resources/SecretStore/Assimalign.Cohesion.SecretStore/tests/CertificateAuthorityOptionsTests.cs`.
- **Source** — `cohesion/resources/SecretStore/Assimalign.Cohesion.SecretStore/tests/Assimalign.Cohesion.SecretStore.Tests.csproj`.
