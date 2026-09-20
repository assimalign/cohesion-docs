# Protocol Frame Header Tests

This example exercises `Assimalign.Cohesion.Database.Protocol` through its co-located test source.

> **Status:** Partial.

The example reproduces
`cohesion/resources/Database/Assimalign.Cohesion.Database.Protocol/tests/ProtocolFrameHeaderTests.cs`
. It retains the test class and assertions so the setup, operation, and expected outcome stay
together. `Use` it in the source project’s test context, with its test dependencies and supporting
test objects.

## Behavior exercised

- **Case 1** — Protocol: Frame header round-trips through its encoding.
- **Case 2** — Protocol: Truncated header does not parse.
- **Case 3** — Protocol: Oversized length prefix is rejected.
- **Case 4** — Protocol: Frame exposes a matching header.

## Source example

```csharp
using System;
using Shouldly;
using Xunit;

namespace Assimalign.Cohesion.Database.Protocol.Tests;

public class ProtocolFrameHeaderTests
{
    [Fact(DisplayName = "Cohesion Test [Database] - Protocol: Frame header round-trips through its encoding")]
    public void WriteTo_ThenTryParse_ShouldRoundTrip()
    {
        // Arrange
        var header = new ProtocolFrameHeader(ProtocolMessageType.Startup, 1234);
        Span<byte> buffer = stackalloc byte[ProtocolFrameHeader.Size];

        // Act
        header.WriteTo(buffer);
        var parsed = ProtocolFrameHeader.TryParse(buffer, out var result);

        // Assert
        parsed.ShouldBeTrue();
        result.Type.ShouldBe(ProtocolMessageType.Startup);
        result.PayloadLength.ShouldBe(1234u);
    }

    [Fact(DisplayName = "Cohesion Test [Database] - Protocol: Truncated header does not parse")]
    public void TryParse_TruncatedBuffer_ShouldReturnFalse()
    {
        // Arrange
        Span<byte> buffer = stackalloc byte[ProtocolFrameHeader.Size - 1];

        // Act
        var parsed = ProtocolFrameHeader.TryParse(buffer, out _);

        // Assert
        parsed.ShouldBeFalse();
    }

    [Fact(DisplayName = "Cohesion Test [Database] - Protocol: Oversized length prefix is rejected")]
    public void TryParse_LengthAboveMaximum_ShouldReturnFalse()
    {
        // Arrange
        var header = new ProtocolFrameHeader(ProtocolMessageType.Startup, ProtocolFrameHeader.MaxPayloadLength);
        Span<byte> buffer = stackalloc byte[ProtocolFrameHeader.Size];
        header.WriteTo(buffer);
        // Bump the encoded length one past the maximum.
        buffer[3] += 1;

        // Act
        var parsed = ProtocolFrameHeader.TryParse(buffer, out _);

        // Assert
        parsed.ShouldBeFalse();
    }

    [Fact(DisplayName = "Cohesion Test [Database] - Protocol: Frame exposes a matching header")]
    public void Frame_Header_ShouldMatchPayload()
    {
        // Arrange
        var frame = new ProtocolFrame(ProtocolMessageType.Ping, new byte[42]);

        // Assert
        frame.Header.Type.ShouldBe(ProtocolMessageType.Ping);
        frame.Header.PayloadLength.ShouldBe(42u);
    }
}
```

[All examples](index.md) · [Assembly overview](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Protocol/tests/ProtocolFrameHeaderTests.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Protocol/tests/Assimalign.Cohesion.Database.Protocol.Tests.csproj`.
