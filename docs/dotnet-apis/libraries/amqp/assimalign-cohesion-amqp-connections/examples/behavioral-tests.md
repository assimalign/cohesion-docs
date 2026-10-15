# Example: Amqp Codec and Transport Capability Tests

Exercise Amqp Codec and Transport Capability behavior using the original repository tests.

[Examples](index.md) · [Assembly overview](../index.md)

## Test-project context

This is the complete `AmqpCodecTests.cs` listing from the package test project. Keep it in that
project when running it: the project supplies its package references, generated sources, and any
shared fixtures. The using block below makes the test-framework import explicit where the original
project supplies it globally.

## Code

```csharp
using System.Collections.Generic;
using System;
using Shouldly;
using Xunit;

namespace Assimalign.Cohesion.Amqp.Connections.Tests;

public class AmqpCodecTests
{
    [Fact(DisplayName = "Cohesion Test [Amqp.Connections] - FrameCodec: Should round-trip a transfer frame with an encoded AMQP message payload")]
    public void FrameCodec_OnTransferFrame_ShouldRoundTripTransferPayload()
    {
        // Arrange
        AmqpMessage message = new()
        {
            Subject = "orders.created",
            ApplicationProperties = new Dictionary<string, object?>
            {
                ["tenant"] = "blue",
                ["attempt"] = (uint) 2
            },
            DataSections = new ReadOnlyMemory<byte>[]
            {
                new ReadOnlyMemory<byte>(new byte[] { 0x10, 0x20, 0x30 })
            }
        };

        byte[] payload = AmqpMessageCodec.Encode(message);
        AmqpFrame frame = new(
            3,
            new AmqpTransferPerformative
            {
                Handle = 12,
                DeliveryId = 4,
                DeliveryTag = new byte[] { 0x01, 0x02, 0x03 },
                MessageFormat = 0
            },
            payload);

        // Act
        byte[] encodedFrame = AmqpFrameCodec.Encode(frame);
        AmqpFrame decodedFrame = AmqpFrameCodec.Decode(encodedFrame);
        AmqpMessage decodedMessage = AmqpMessageCodec.Decode(decodedFrame.Payload.Span);

        // Assert
        decodedFrame.Channel.ShouldBe((ushort) 3);
        AmqpTransferPerformative transfer = decodedFrame.Performative.ShouldBeOfType<AmqpTransferPerformative>();
        transfer.Handle.ShouldBe((uint) 12);
        transfer.DeliveryId.ShouldBe((uint) 4);
        transfer.DeliveryTag.ShouldBe(new byte[] { 0x01, 0x02, 0x03 });
        decodedMessage.Subject.ShouldBe("orders.created");
        decodedMessage.ApplicationProperties!["tenant"].ShouldBe("blue");
        decodedMessage.ApplicationProperties["attempt"].ShouldBe((uint) 2);
        decodedMessage.DataSections![0].ToArray().ShouldBe(new byte[] { 0x10, 0x20, 0x30 });
    }

    [Fact(DisplayName = "Cohesion Test [Amqp.Connections] - FrameCodec: Should round-trip SASL mechanism negotiation frames")]
    public void FrameCodec_OnSaslFrame_ShouldRoundTripMechanisms()
    {
        // Arrange
        AmqpFrame frame = new(
            0,
            new AmqpSaslMechanismsPerformative
            {
                SaslServerMechanisms = new[] { (AmqpSymbol) "ANONYMOUS", (AmqpSymbol) "PLAIN" }
            });

        // Act
        byte[] encodedFrame = AmqpFrameCodec.Encode(frame, AmqpFrameType.Sasl);
        AmqpFrame decodedFrame = AmqpFrameCodec.Decode(encodedFrame, AmqpFrameType.Sasl);

        // Assert
        AmqpSaslMechanismsPerformative mechanisms = decodedFrame.Performative.ShouldBeOfType<AmqpSaslMechanismsPerformative>();
        mechanisms.SaslServerMechanisms.Count.ShouldBe(2);
        mechanisms.SaslServerMechanisms[0].ShouldBe((AmqpSymbol) "ANONYMOUS");
        mechanisms.SaslServerMechanisms[1].ShouldBe((AmqpSymbol) "PLAIN");
    }
}
```

## Walkthrough

- **Covered behavior** — FrameCodec: Should round-trip a transfer frame with an encoded AMQP message payload.
- **Covered behavior** — FrameCodec: Should round-trip SASL mechanism negotiation frames.

The assertions define the expected result or failure boundary. Test-only doubles supply controlled
inputs; they are not additional shipped APIs.

## Transport capability tests

This is the complete `AmqpTransportCapabilityTests.cs` listing from the package test project. Keep it in that
project when running it: the project supplies its package references, generated sources, and any
shared fixtures. The using block below makes the test-framework import explicit where the original
project supplies it globally.

### Code

```csharp
using System;
using System.Net;
using System.Threading.Tasks;

using Shouldly;

using Xunit;

using Assimalign.Cohesion.Connections;

namespace Assimalign.Cohesion.Amqp.Connections.Tests;

public class AmqpTransportCapabilityTests
{
    private static readonly EndPoint _testEndPoint = new IPEndPoint(IPAddress.Loopback, 5672);

    [Fact(DisplayName = "Cohesion Test [Amqp.Connections] - Constructor: Should reject a datagram carrier listener")]
    public async Task Constructor_OnDatagramCarrierListener_ShouldThrowArgumentException()
    {
        // Arrange
        await using TestConnectionListener listener = new(
            TestConnection.DefaultCapabilities with { Delivery = ConnectionDelivery.Datagram });

        // Act
        ArgumentException exception = Should.Throw<ArgumentException>(() => new AmqpServerTransport(listener));

        // Assert
        exception.ParamName.ShouldBe("listener");
    }

    [Fact(DisplayName = "Cohesion Test [Amqp.Connections] - Constructor: Should reject an unreliable carrier listener")]
    public async Task Constructor_OnUnreliableCarrierListener_ShouldThrowArgumentException()
    {
        // Arrange
        await using TestConnectionListener listener = new(
            TestConnection.DefaultCapabilities with { IsReliable = false });

        // Act + Assert
        Should.Throw<ArgumentException>(() => new AmqpServerTransport(listener));
    }

    [Fact(DisplayName = "Cohesion Test [Amqp.Connections] - Constructor: Should reject an unordered multiplexed carrier listener")]
    public async Task Constructor_OnUnorderedMultiplexedCarrierListener_ShouldThrowArgumentException()
    {
        // Arrange
        await using TestMultiplexedConnectionListener listener = new(
            TestMultiplexedConnection.DefaultCapabilities with { IsOrdered = false });

        // Act + Assert
        Should.Throw<ArgumentException>(() => new AmqpServerTransport(listener));
    }

    [Fact(DisplayName = "Cohesion Test [Amqp.Connections] - Constructor: Should accept reliable ordered stream carrier listeners")]
    public async Task Constructor_OnReliableOrderedStreamListeners_ShouldConstructServerTransport()
    {
        // Arrange
        await using TestConnectionListener listener = new();
        await using TestMultiplexedConnectionListener multiplexedListener = new();

        // Act
        await using AmqpServerTransport singleStreamTransport = new(listener);
        await using AmqpServerTransport multiplexedTransport = new(multiplexedListener);

        // Assert
        singleStreamTransport.Connections.ShouldBeEmpty();
        multiplexedTransport.Connections.ShouldBeEmpty();
    }

    [Fact(DisplayName = "Cohesion Test [Amqp.Connections] - Constructor: Should reject a null carrier listener")]
    public void Constructor_OnNullListener_ShouldThrowArgumentNullException()
    {
        // Act + Assert
        Should.Throw<ArgumentNullException>(() => new AmqpServerTransport((IDuplexConnectionListener)null!));
        Should.Throw<ArgumentNullException>(() => new AmqpServerTransport((IMultiplexedConnectionListener)null!));
    }

    [Fact(DisplayName = "Cohesion Test [Amqp.Connections] - Constructor: Should reject a datagram carrier factory")]
    public void Constructor_OnDatagramCarrierFactory_ShouldThrowArgumentException()
    {
        // Arrange
        TestConnectionFactory factory = new(
            TestConnection.DefaultCapabilities with { Delivery = ConnectionDelivery.Datagram });

        // Act
        ArgumentException exception = Should.Throw<ArgumentException>(() => new AmqpClientTransport(factory, _testEndPoint));

        // Assert
        exception.ParamName.ShouldBe("factory");
    }

    [Fact(DisplayName = "Cohesion Test [Amqp.Connections] - Constructor: Should reject an unreliable multiplexed carrier factory")]
    public void Constructor_OnUnreliableMultiplexedCarrierFactory_ShouldThrowArgumentException()
    {
        // Arrange
        TestMultiplexedConnectionFactory factory = new(
            TestMultiplexedConnection.DefaultCapabilities with { IsReliable = false });

        // Act + Assert
        Should.Throw<ArgumentException>(() => new AmqpClientTransport(factory, _testEndPoint));
    }

    [Fact(DisplayName = "Cohesion Test [Amqp.Connections] - Constructor: Should reject an unordered carrier factory")]
    public void Constructor_OnUnorderedCarrierFactory_ShouldThrowArgumentException()
    {
        // Arrange
        TestConnectionFactory factory = new(
            TestConnection.DefaultCapabilities with { IsOrdered = false });

        // Act + Assert
        Should.Throw<ArgumentException>(() => new AmqpClientTransport(factory, _testEndPoint));
    }

    [Fact(DisplayName = "Cohesion Test [Amqp.Connections] - Constructor: Should accept reliable ordered stream carrier factories")]
    public async Task Constructor_OnReliableOrderedStreamFactories_ShouldConstructClientTransport()
    {
        // Arrange
        TestConnectionFactory factory = new();
        TestMultiplexedConnectionFactory multiplexedFactory = new();

        // Act
        await using AmqpClientTransport singleStreamTransport = new(factory, _testEndPoint);
        await using AmqpClientTransport multiplexedTransport = new(multiplexedFactory, _testEndPoint);

        // Assert
        singleStreamTransport.EndPoint.ShouldBeSameAs(_testEndPoint);
        multiplexedTransport.EndPoint.ShouldBeSameAs(_testEndPoint);
    }

    [Fact(DisplayName = "Cohesion Test [Amqp.Connections] - Constructor: Should reject null carrier factory arguments")]
    public void Constructor_OnNullFactoryArguments_ShouldThrowArgumentNullException()
    {
        // Arrange
        TestConnectionFactory factory = new();

        // Act + Assert
        Should.Throw<ArgumentNullException>(() => new AmqpClientTransport((IDuplexConnectionFactory)null!, _testEndPoint));
        Should.Throw<ArgumentNullException>(() => new AmqpClientTransport((IMultiplexedConnectionFactory)null!, _testEndPoint));
        Should.Throw<ArgumentNullException>(() => new AmqpClientTransport(factory, null!));
    }
}
```

### Walkthrough

- **Covered behavior** — Constructor: Should reject a datagram carrier listener.
- **Covered behavior** — Constructor: Should reject an unreliable carrier listener.
- **Covered behavior** — Constructor: Should reject an unordered multiplexed carrier listener.
- **Covered behavior** — Constructor: Should accept reliable ordered stream carrier listeners.
- **Covered behavior** — Constructor: Should reject a null carrier listener.
- **Covered behavior** — Constructor: Should reject a datagram carrier factory.
- **Covered behavior** — Constructor: Should reject an unreliable multiplexed carrier factory.
- **Covered behavior** — Constructor: Should reject an unordered carrier factory.
- **Covered behavior** — Constructor: Should accept reliable ordered stream carrier factories.
- **Covered behavior** — Constructor: Should reject null carrier factory arguments.

The assertions define the expected result or failure boundary. Test-only doubles supply controlled
inputs; they are not additional shipped APIs.

## Sources

- **Source** — `cohesion/libraries/Amqp/Assimalign.Cohesion.Amqp.Connections/tests/AmqpCodecTests.cs`.
- **Source** — `cohesion/libraries/Amqp/Assimalign.Cohesion.Amqp.Connections/tests/AmqpTransportCapabilityTests.cs`.
- **Source** — `cohesion/libraries/Amqp/Assimalign.Cohesion.Amqp.Connections/tests/Assimalign.Cohesion.Amqp.Connections.Tests.csproj`.
