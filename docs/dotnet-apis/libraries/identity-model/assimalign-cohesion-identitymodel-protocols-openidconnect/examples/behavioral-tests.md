# Example: Open Id Connect User Info Tests

Exercise Open Id Connect User Info behavior using the original repository tests.

[Examples](index.md) · [Assembly overview](../index.md)

## Test-project context

This is the complete `OpenIdConnectUserInfoTests.cs` listing from the package test project. Keep it
in that project when running it: the project supplies its package references, generated sources, and
any shared fixtures. The using block below makes the test-framework import explicit where the
original project supplies it globally.

## Code

```csharp
using System;
using Shouldly;
using Xunit;
using Assimalign.Cohesion.IdentityModel.Protocols.OpenIdConnect;
using Assimalign.Cohesion.IdentityModel.Protocols;
using Assimalign.Cohesion.IdentityModel;

namespace Assimalign.Cohesion.IdentityModel.Protocols.OpenIdConnect.Tests;

/// <summary>
/// Contains unit tests for the UserInfo contract.
/// </summary>
public sealed class OpenIdConnectUserInfoTests
{
    [Fact(DisplayName = "Cohesion Test [IdentityModel] - OIDC: UserInfo should carry provenance-stamped claims")]
    public void Constructor_WhenConstructed_ShouldCarryProvenanceStampedClaims()
    {
        // Arrange
        var descriptor = new OpenIdConnectUserInfoDescriptor
        {
            Subject = "24400320",
            Issuer = "https://server.example.com",
            RawDocument = "{\"sub\":\"24400320\",\"email\":\"user@example.com\"}",
        };
        descriptor.AdditionalClaims.Add(new IdentityClaim(
            IdentityClaimTypes.Email,
            "user@example.com",
            issuer: "https://server.example.com",
            provenance: new IdentityClaimProvenance(AuthenticationProtocol.OpenIdConnect, originalType: "email")));

        // Act
        var userInfo = new OpenIdConnectUserInfo(descriptor);

        // Assert
        userInfo.Subject.ShouldBe("24400320");
        userInfo.Claims.GetString(IdentityClaimTypes.Subject).ShouldBe("24400320");
        userInfo.Claims.GetString(IdentityClaimTypes.Email).ShouldBe("user@example.com");
        userInfo.RawDocument.ShouldNotBeNull();

        // The subject flows through the typed member only.
        var colliding = new OpenIdConnectUserInfoDescriptor { Subject = "24400320" };
        colliding.AdditionalClaims.Add(new IdentityClaim(IdentityClaimTypes.Subject, "other"));
        Should.Throw<IdentityModelException>(() => new OpenIdConnectUserInfo(colliding));
    }

    [Fact(DisplayName = "Cohesion Test [IdentityModel] - OIDC: UserInfo validation should enforce the subject rules")]
    public void Validate_WhenSubjectRulesFail_ShouldReport()
    {
        // A UserInfo response without sub must be diagnosable, not unconstructible.
        var missing = new OpenIdConnectUserInfo(new OpenIdConnectUserInfoDescriptor());
        missing.Validate().Errors.ShouldContain(d => d.Member == IdentityClaimTypes.Subject);

        // The sub must match the ID token's sub or the claims must not be used.
        var userInfo = new OpenIdConnectUserInfo(new OpenIdConnectUserInfoDescriptor { Subject = "24400320" });
        userInfo.Validate(expectedSubject: "24400320").Succeeded.ShouldBeTrue();
        userInfo.Validate(expectedSubject: "different-user")
            .Errors.ShouldContain(d => d.Code == ProtocolValidationCodes.SubjectMismatch);
    }
}
```

## Walkthrough

- **Covered behavior** — OIDC: UserInfo should carry provenance-stamped claims.
- **Covered behavior** — OIDC: UserInfo validation should enforce the subject rules.

The assertions define the expected result or failure boundary. Test-only doubles supply controlled
inputs; they are not additional shipped APIs.

## Sources

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols.OpenIdConnect/tests/OpenIdConnectUserInfoTests.cs`.
- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols.OpenIdConnect/tests/Assimalign.Cohesion.IdentityModel.Protocols.OpenIdConnect.Tests.csproj`.
