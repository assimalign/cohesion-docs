# Example: Quic Connection Options Tests

Exercise Quic Connection Options behavior using the original repository tests.

[Examples](index.md) · [Assembly overview](../index.md)

## Test-project context

This is the complete `QuicConnectionOptionsTests.cs` listing from the package test project. Keep it
in that project when running it: the project supplies its package references, generated sources, and
any shared fixtures. The using block below makes the test-framework import explicit where the
original project supplies it globally.

## Code

```csharp
using System.Runtime.Versioning;
using Shouldly;
using Xunit;

namespace Assimalign.Cohesion.Connections.Quic.Tests;

[SupportedOSPlatform("windows")]
[SupportedOSPlatform("linux")]
[SupportedOSPlatform("macos")]
public class QuicConnectionOptionsTests
{
    [Fact]
    public void DefaultErrorCodes_OnListenerOptions_ShouldMatchHttp3Codes()
    {
        // Arrange / Act
        // The options default ApplicationProtocols to HTTP/3, so the error codes default to
        // the matching RFC 9114 §8.1 values.
        QuicConnectionListenerOptions options = new();

        // Assert
        options.DefaultCloseErrorCode.ShouldBe(0x100);  // H3_NO_ERROR
        options.DefaultStreamErrorCode.ShouldBe(0x10c); // H3_REQUEST_CANCELLED
    }

    [Fact]
    public void DefaultErrorCodes_OnFactoryOptions_ShouldMatchHttp3Codes()
    {
        // Arrange / Act
        // The options default ApplicationProtocols to HTTP/3, so the error codes default to
        // the matching RFC 9114 §8.1 values.
        QuicConnectionFactoryOptions options = new();

        // Assert
        options.DefaultCloseErrorCode.ShouldBe(0x100);  // H3_NO_ERROR
        options.DefaultStreamErrorCode.ShouldBe(0x10c); // H3_REQUEST_CANCELLED
    }
}
```

## Walkthrough

- **Test entry point** — `DefaultErrorCodes_OnListenerOptions_ShouldMatchHttp3Codes` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `DefaultErrorCodes_OnFactoryOptions_ShouldMatchHttp3Codes` contains the setup, invocation, and assertions for this case.

The assertions define the expected result or failure boundary. Test-only doubles supply controlled
inputs; they are not additional shipped APIs.

## Sources

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Quic/tests/QuicConnectionOptionsTests.cs`.
- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Quic/tests/Assimalign.Cohesion.Connections.Quic.Tests.csproj`.
