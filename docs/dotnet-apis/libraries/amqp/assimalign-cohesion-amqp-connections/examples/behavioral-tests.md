# Example: AMQP Codec and Carrier Capability Tests

Exercise AMQP codec and carrier capability behavior using the original repository tests.

[Examples](index.md) · [Assembly overview](../index.md)

## Test-project context

This is the complete `AmqpCodecTests.cs` listing from the package test project. Keep it in that
project when running it: the project supplies its package references, generated sources, and any
shared fixtures.

## Code

```csharp
using System;
using System.Collections.Generic;

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

## Carrier capability tests

This is the complete `AmqpCarrierCapabilityTests.cs` listing from the package test project. It
covers the capability gate `AmqpConnectionListener.Create` and `AmqpConnectionFactory.Create` apply
to their carrier: a reliable, ordered byte stream, single-stream or multiplexed. Keep it in that
project when running it: `TestConnectionListener`, `TestConnectionFactory` and their multiplexed
counterparts are the project's test doubles.

### Code

```csharp
using System;
using System.Threading.Tasks;

using Shouldly;

using Xunit;

using Assimalign.Cohesion.Connections;

namespace Assimalign.Cohesion.Amqp.Connections.Tests;

public class AmqpCarrierCapabilityTests
{
    [Fact(DisplayName = "Cohesion Test [Amqp.Connections] - Create: Should reject a datagram carrier listener")]
    public void Create_OnDatagramCarrierListener_ShouldThrowArgumentException()
    {
        // Arrange
        TestConnectionListener listener = new(TestConnection.DefaultCapabilities with { Delivery = ConnectionDelivery.Datagram });

        // Act
        ArgumentException exception = Should.Throw<ArgumentException>(() => AmqpConnectionListener.Create(listener));

        // Assert
        exception.ParamName.ShouldBe("listener");
        listener.IsDisposed.ShouldBeFalse();
    }

    [Fact(DisplayName = "Cohesion Test [Amqp.Connections] - Create: Should reject an unreliable carrier listener")]
    public void Create_OnUnreliableCarrierListener_ShouldThrowArgumentException()
    {
        // Arrange
        TestConnectionListener listener = new(TestConnection.DefaultCapabilities with { IsReliable = false });

        // Act
        ArgumentException exception = Should.Throw<ArgumentException>(() => AmqpConnectionListener.Create(listener));

        // Assert
        exception.ParamName.ShouldBe("listener");
    }

    [Fact(DisplayName = "Cohesion Test [Amqp.Connections] - Create: Should reject an unordered multiplexed carrier listener")]
    public void Create_OnUnorderedMultiplexedCarrierListener_ShouldThrowArgumentException()
    {
        // Arrange
        TestMultiplexedConnectionListener listener = new(TestMultiplexedConnection.DefaultCapabilities with { IsOrdered = false });

        // Act
        ArgumentException exception = Should.Throw<ArgumentException>(() => AmqpConnectionListener.Create(listener));

        // Assert
        exception.ParamName.ShouldBe("listener");
    }

    [Fact(DisplayName = "Cohesion Test [Amqp.Connections] - Create: Should accept reliable, ordered stream carrier listeners of either kind")]
    public async Task Create_OnReliableOrderedStreamListeners_ShouldCreateListener()
    {
        // Arrange
        TestConnectionListener listener = new();
        TestMultiplexedConnectionListener multiplexedListener = new();

        // Act
        await using AmqpConnectionListener amqp = AmqpConnectionListener.Create(listener);
        await using AmqpConnectionListener multiplexedAmqp = AmqpConnectionListener.Create(multiplexedListener);

        // Assert
        amqp.Connections.ShouldBeEmpty();
        multiplexedAmqp.Connections.ShouldBeEmpty();
    }

    [Fact(DisplayName = "Cohesion Test [Amqp.Connections] - Create: Should reject a null carrier listener")]
    public void Create_OnNullListener_ShouldThrowArgumentNullException()
    {
        // Act + Assert
        Should.Throw<ArgumentNullException>(() => AmqpConnectionListener.Create((IDuplexConnectionListener)null!)).ParamName.ShouldBe("listener");
        Should.Throw<ArgumentNullException>(() => AmqpConnectionListener.Create((IMultiplexedConnectionListener)null!)).ParamName.ShouldBe("listener");
    }

    [Fact(DisplayName = "Cohesion Test [Amqp.Connections] - Create: Should reject a datagram carrier factory")]
    public void Create_OnDatagramCarrierFactory_ShouldThrowArgumentException()
    {
        // Arrange
        TestConnectionFactory factory = new(TestConnection.DefaultCapabilities with { Delivery = ConnectionDelivery.Datagram });

        // Act
        ArgumentException exception = Should.Throw<ArgumentException>(() => AmqpConnectionFactory.Create(factory));

        // Assert
        exception.ParamName.ShouldBe("factory");
    }

    [Fact(DisplayName = "Cohesion Test [Amqp.Connections] - Create: Should reject an unreliable multiplexed carrier factory")]
    public void Create_OnUnreliableMultiplexedCarrierFactory_ShouldThrowArgumentException()
    {
        // Arrange
        TestMultiplexedConnectionFactory factory = new(TestMultiplexedConnection.DefaultCapabilities with { IsReliable = false });

        // Act
        ArgumentException exception = Should.Throw<ArgumentException>(() => AmqpConnectionFactory.Create(factory));

        // Assert
        exception.ParamName.ShouldBe("factory");
    }

    [Fact(DisplayName = "Cohesion Test [Amqp.Connections] - Create: Should reject an unordered carrier factory")]
    public void Create_OnUnorderedCarrierFactory_ShouldThrowArgumentException()
    {
        // Arrange
        TestConnectionFactory factory = new(TestConnection.DefaultCapabilities with { IsOrdered = false });

        // Act
        ArgumentException exception = Should.Throw<ArgumentException>(() => AmqpConnectionFactory.Create(factory));

        // Assert
        exception.ParamName.ShouldBe("factory");
    }

    [Fact(DisplayName = "Cohesion Test [Amqp.Connections] - Create: Should accept reliable, ordered stream carrier factories of either kind")]
    public void Create_OnReliableOrderedStreamFactories_ShouldCreateFactory()
    {
        // Arrange
        TestConnectionFactory factory = new();
        TestMultiplexedConnectionFactory multiplexedFactory = new();

        // Act
        AmqpConnectionFactory amqp = AmqpConnectionFactory.Create(factory);
        AmqpConnectionFactory multiplexedAmqp = AmqpConnectionFactory.Create(multiplexedFactory);

        // Assert
        amqp.Capabilities.IsReliable.ShouldBeTrue();
        multiplexedAmqp.Capabilities.IsOrdered.ShouldBeTrue();
    }

    [Fact(DisplayName = "Cohesion Test [Amqp.Connections] - Create: Should reject a null carrier factory")]
    public void Create_OnNullFactory_ShouldThrowArgumentNullException()
    {
        // Act + Assert
        Should.Throw<ArgumentNullException>(() => AmqpConnectionFactory.Create((IDuplexConnectionFactory)null!)).ParamName.ShouldBe("factory");
        Should.Throw<ArgumentNullException>(() => AmqpConnectionFactory.Create((IMultiplexedConnectionFactory)null!)).ParamName.ShouldBe("factory");
    }
}
```

### Walkthrough

- **Covered behavior** — Create: Should reject a datagram carrier listener.
- **Covered behavior** — Create: Should reject an unreliable carrier listener.
- **Covered behavior** — Create: Should reject an unordered multiplexed carrier listener.
- **Covered behavior** — Create: Should accept reliable, ordered stream carrier listeners of either kind.
- **Covered behavior** — Create: Should reject a null carrier listener.
- **Covered behavior** — Create: Should reject a datagram carrier factory.
- **Covered behavior** — Create: Should reject an unreliable multiplexed carrier factory.
- **Covered behavior** — Create: Should reject an unordered carrier factory.
- **Covered behavior** — Create: Should accept reliable, ordered stream carrier factories of either kind.
- **Covered behavior** — Create: Should reject a null carrier factory.

The assertions define the expected result or failure boundary. Test-only doubles supply controlled
inputs; they are not additional shipped APIs.

## Sources

- **Source** — `cohesion/libraries/Amqp/Assimalign.Cohesion.Amqp.Connections/tests/AmqpCodecTests.cs`.
- **Source** — `cohesion/libraries/Amqp/Assimalign.Cohesion.Amqp.Connections/tests/AmqpCarrierCapabilityTests.cs`.
- **Source** — `cohesion/libraries/Amqp/Assimalign.Cohesion.Amqp.Connections/tests/Assimalign.Cohesion.Amqp.Connections.Tests.csproj`.
