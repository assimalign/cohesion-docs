# Canonical Identity Model Tests

This example exercises `Assimalign.Cohesion.IdentityHub.Models` through its co-located test source.

> **Status:** Partial.

The example reproduces
`cohesion/resources/IdentityHub/Assimalign.Cohesion.IdentityHub.Models/tests/CanonicalIdentityModelTests.cs`
. It retains the test class and assertions so the setup, operation, and expected outcome stay
together. `Use` it in the source project’s test context, with its test dependencies and supporting
test objects.

## Behavior exercised

- **Case 1** — `User`: retains the canonical identity subject.
- **Case 2** — `Application`: retains the canonical application subject.
- **Case 3** — `ApplicationCredential`: retains canonical credential metadata.

## Source example

```csharp
using Shouldly;
using Xunit;
using Assimalign.Cohesion.IdentityModel;

namespace Assimalign.Cohesion.IdentityHub.Models.Tests;

public sealed class CanonicalIdentityModelTests
{
    [Fact(DisplayName = "Cohesion Test [IdentityHub.Models] - User: retains the canonical identity subject")]
    public void User_WithCanonicalIdentity_ShouldRetainSameSubject()
    {
        // Arrange
        var subject = new IdentitySubject(new IdentitySubjectDescriptor
        {
            Kind = IdentityKind.User,
            Identifier = new SubjectIdentifier("user-1", SubjectIdentifierFormats.Public),
        });

        // Act
        var user = new User { Identity = subject };

        // Assert
        user.Identity.ShouldBeSameAs(subject);
        user.Kind.ShouldBe(ObjectKind.User);
    }

    [Fact(DisplayName = "Cohesion Test [IdentityHub.Models] - Application: retains the canonical application subject")]
    public void Application_WithCanonicalIdentity_ShouldRetainSameSubject()
    {
        // Arrange
        var subject = new IdentitySubject(new IdentitySubjectDescriptor
        {
            Kind = IdentityKind.Application,
            Identifier = new SubjectIdentifier("client-1", SubjectIdentifierFormats.ClientIdentifier),
        });

        // Act
        var application = new Application { Identity = subject };

        // Assert
        application.Identity.ShouldBeSameAs(subject);
        application.Kind.ShouldBe(ObjectKind.Application);
    }

    [Fact(DisplayName = "Cohesion Test [IdentityHub.Models] - ApplicationCredential: retains canonical credential metadata")]
    public void ApplicationCredential_WithCanonicalCredential_ShouldRetainSameCredential()
    {
        // Arrange
        var credential = new IdentityCredential(new IdentityCredentialDescriptor
        {
            Id = "credential-1",
            Kind = IdentityCredentialKind.Certificate,
            State = IdentityCredentialState.Active,
        });

        // Act
        var applicationCredential = new ApplicationCredential { Credential = credential };

        // Assert
        applicationCredential.Credential.ShouldBeSameAs(credential);
    }
}
```

[All examples](index.md) · [Assembly overview](../index.md)

## Sources

- **Primary source** — `cohesion/resources/IdentityHub/Assimalign.Cohesion.IdentityHub.Models/tests/CanonicalIdentityModelTests.cs`.
- **Source** — `cohesion/resources/IdentityHub/Assimalign.Cohesion.IdentityHub.Models/tests/Assimalign.Cohesion.IdentityHub.Models.Tests.csproj`.
