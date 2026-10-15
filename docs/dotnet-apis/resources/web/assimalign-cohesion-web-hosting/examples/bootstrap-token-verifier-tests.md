# Bootstrap Token Verifier Tests

This example exercises `Assimalign.Cohesion.Web.Hosting` through its co-located test source.

> **Status:** Partial.

The example reproduces
`cohesion/resources/Web/Assimalign.Cohesion.Web.Hosting/tests/BootstrapTokenVerifierTests.cs`.
It retains the test class and assertions so the setup, operation, and expected outcome stay
together. Use it in the source project’s test context, with its test dependencies and supporting
test objects.

## Behavior exercised

- **Case 1** — BootstrapTokenVerifier: Null and blank audiences remain forbidden.

## Source example

```csharp
using System;

using Assimalign.Cohesion.Hosting.Resources;
using Assimalign.Cohesion.Web.Hosting.Internal;

using Shouldly;

using Xunit;

namespace Assimalign.Cohesion.Web.Hosting.Tests;

public sealed class BootstrapTokenVerifierTests
{
    [Theory(DisplayName = "Cohesion Test [Web.Hosting] - BootstrapTokenVerifier: Null and blank audiences remain forbidden")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    public void Validate_WhenExpectedAudienceIsNullOrBlank_ReturnsForbidden(string? expectedAudience)
    {
        // Arrange: pin the audience behavior that predates the factory conversion.
        using var identity = new TestBootstrapIdentity("application", "gateway");
        DateTimeOffset now = DateTimeOffset.UtcNow;
        string compactToken = identity.Issue("resource", issuedAt: now);
        var context = new ResourceContext(
            applicationName: identity.Issuer,
            resourceName: "resource",
            environmentName: "Testing",
            gatewayName: identity.Subject,
            contentRootPath: null,
            endpoints: null,
            mounts: null,
            settings: null,
            references: null,
            bootstrapCredential: ReadOnlyMemory<byte>.Empty,
            applicationTrustKey: identity.PublicKey,
            ambientValues: null);
        var verifier = new BootstrapTokenVerifier(context);

        // Act
        ControlPlaneCredentialVerification result = verifier.Validate(compactToken, expectedAudience!, now);

        // Assert
        result.Verification.Status.ShouldBe(ResourceCredentialStatus.Forbidden);
        result.Reason.ShouldBe(CredentialRejectionReason.AudienceMismatch);
    }
}
```

[All examples](index.md) · [Assembly overview](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Hosting/tests/BootstrapTokenVerifierTests.cs`.
- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Hosting/tests/Assimalign.Cohesion.Web.Hosting.Tests.csproj`.
