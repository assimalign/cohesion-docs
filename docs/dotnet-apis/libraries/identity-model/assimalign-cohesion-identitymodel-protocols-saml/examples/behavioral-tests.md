# Example: Saml Bindings Tests

Exercise Saml Bindings behavior using the original repository tests.

[Examples](index.md) · [Assembly overview](../index.md)

## Test-project context

This is the complete `SamlBindingsTests.cs` listing from the package test project. Keep it in that
project when running it: the project supplies its package references, generated sources, and any
shared fixtures. The using block below makes the test-framework import explicit where the original
project supplies it globally.

## Code

```csharp
using Shouldly;
using Xunit;
using Assimalign.Cohesion.IdentityModel.Protocols;

namespace Assimalign.Cohesion.IdentityModel.Protocols.Saml.Tests;

/// <summary>
/// Verifies the single place SAML binding URIs map onto the family's transport-shaped
/// <see cref="ProtocolBinding" /> vocabulary.
/// </summary>
public sealed class SamlBindingsTests
{
    [Theory(DisplayName = "Cohesion Test [IdentityModel] - SAML: Known binding URIs should map to the neutral vocabulary")]
    [InlineData(SamlBindings.HttpRedirect)]
    [InlineData(SamlBindings.HttpPost)]
    [InlineData(SamlBindings.HttpArtifact)]
    [InlineData(SamlBindings.Soap)]
    [InlineData(SamlBindings.Paos)]
    public void ToProtocolBinding_WhenKnown_ShouldNotBeUnknown(string bindingUri)
    {
        SamlBindings.ToProtocolBinding(bindingUri).ShouldNotBe(ProtocolBinding.Unknown);
    }

    [Fact(DisplayName = "Cohesion Test [IdentityModel] - SAML: Redirect and POST should map to their transport shapes")]
    public void ToProtocolBinding_WhenRedirectOrPost_ShouldMapToTransport()
    {
        SamlBindings.ToProtocolBinding(SamlBindings.HttpRedirect).ShouldBe(ProtocolBinding.HttpRedirect);
        SamlBindings.ToProtocolBinding(SamlBindings.HttpPost).ShouldBe(ProtocolBinding.HttpPost);
    }

    [Fact(DisplayName = "Cohesion Test [IdentityModel] - SAML: SOAP and PAOS should both map to SOAP")]
    public void ToProtocolBinding_WhenSoapFamily_ShouldMapToSoap()
    {
        SamlBindings.ToProtocolBinding(SamlBindings.Soap).ShouldBe(ProtocolBinding.Soap);
        SamlBindings.ToProtocolBinding(SamlBindings.Paos).ShouldBe(ProtocolBinding.Soap);
    }

    [Fact(DisplayName = "Cohesion Test [IdentityModel] - SAML: An unrecognized binding URI should map to Unknown")]
    public void ToProtocolBinding_WhenUnrecognized_ShouldMapToUnknown()
    {
        SamlBindings.ToProtocolBinding("urn:example:binding:made-up").ShouldBe(ProtocolBinding.Unknown);
    }
}
```

## Walkthrough

- **Covered behavior** — SAML: Known binding URIs should map to the neutral vocabulary.
- **Covered behavior** — SAML: Redirect and POST should map to their transport shapes.
- **Covered behavior** — SAML: SOAP and PAOS should both map to SOAP.
- **Covered behavior** — SAML: An unrecognized binding URI should map to Unknown.

The assertions define the expected result or failure boundary. Test-only doubles supply controlled
inputs; they are not additional shipped APIs.

## Sources

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols.Saml/tests/SamlBindingsTests.cs`.
- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols.Saml/tests/Assimalign.Cohesion.IdentityModel.Protocols.Saml.Tests.csproj`.
