# Security Headers Feature Tests

This example exercises `Assimalign.Cohesion.Web.SecurityHeaders` through its co-located test source.

> **Status:** Partial.

The example reproduces
`cohesion/resources/Web/Assimalign.Cohesion.Web.SecurityHeaders/tests/SecurityHeadersFeatureTests.cs`.
It retains the test class and assertions so the setup, operation, and expected outcome stay
together. Use it in the source project’s test context, with its test dependencies and supporting
test objects.

## Behavior exercised

- **Case 1** — Feature: The nonce should be 128 bits of base64 and stable once read.
- **Case 2** — Feature: Concurrent first reads should agree on one nonce.
- **Case 3** — Feature: Separate exchanges should get distinct nonces.

## Source example

```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shouldly;
using Xunit;
using Assimalign.Cohesion.Web.SecurityHeaders.Internal;

namespace Assimalign.Cohesion.Web.SecurityHeaders.Tests;

/// <summary>
/// The default nonce feature: 128 bits of CSPRNG output in base64, fixed for the exchange once read, and
/// distinct across exchanges.
/// </summary>
public class SecurityHeadersFeatureTests
{
    [Fact(DisplayName = "Cohesion Test [Web.SecurityHeaders] - Feature: The nonce should be 128 bits of base64 and stable once read")]
    public void Nonce_ReadTwice_ShouldReturnTheSameBase64Value()
    {
        // Arrange
        SecurityHeadersFeature feature = new();

        // Act
        string first = feature.Nonce;
        string second = feature.Nonce;

        // Assert
        second.ShouldBeSameAs(first);
        Convert.FromBase64String(first).Length.ShouldBe(16);
        SecurityHeadersGrammar.IsBase64Value(first).ShouldBeTrue();
        feature.Name.ShouldBe(nameof(ISecurityHeadersFeature));
    }

    [Fact(DisplayName = "Cohesion Test [Web.SecurityHeaders] - Feature: Concurrent first reads should agree on one nonce")]
    public async Task Nonce_ConcurrentFirstReads_ShouldAgree()
    {
        // Arrange
        SecurityHeadersFeature feature = new();
        Task<string>[] reads = new Task<string>[16];

        // Act
        for (int index = 0; index < reads.Length; index++)
        {
            reads[index] = Task.Run(() => feature.Nonce);
        }

        string[] nonces = await Task.WhenAll(reads);

        // Assert
        new HashSet<string>(nonces).Count.ShouldBe(1);
    }

    [Fact(DisplayName = "Cohesion Test [Web.SecurityHeaders] - Feature: Separate exchanges should get distinct nonces")]
    public void Nonce_SeparateFeatures_ShouldDiffer()
    {
        // Arrange
        HashSet<string> nonces = new();

        // Act
        for (int index = 0; index < 64; index++)
        {
            nonces.Add(new SecurityHeadersFeature().Nonce);
        }

        // Assert
        nonces.Count.ShouldBe(64);
    }
}
```

[All examples](index.md) · [Assembly overview](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.SecurityHeaders/tests/SecurityHeadersFeatureTests.cs`.
- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.SecurityHeaders/tests/Assimalign.Cohesion.Web.SecurityHeaders.Tests.csproj`.
