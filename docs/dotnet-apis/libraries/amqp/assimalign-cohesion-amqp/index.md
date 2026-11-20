# Assimalign.Cohesion.Amqp

The AMQP 1.0 model: connection, session, link and delivery contracts, messages, delivery states, termini, values, SASL mechanisms and transactions.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Amqp](../index.md)

## Scope

The AMQP 1.0 model: the contracts an application programs against and the values it exchanges.
It holds the connection, session and link contracts (`IAmqpConnection`, `IAmqpSession`,
`IAmqpSender`, `IAmqpReceiver`, `IAmqpDelivery`), the message format (`AmqpMessage` and its
sections), delivery states and outcomes, termini, the Part 1 values that have no CLR equivalent,
SASL mechanisms, and the transaction controller and coordinator of Part 4.

It implements no protocol. The engine that speaks AMQP over a carrier is
`Assimalign.Cohesion.Amqp.Connections`, whose `AmqpConnectionListener` and
`AmqpConnectionFactory` produce `IAmqpConnection` instances. A broker, a client, or a test double
programs against this package alone.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Core`](../../core/assimalign-cohesion-core/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.Connections`](../../connections/assimalign-cohesion-connections/index.md) | `CohesionProjectReference` |

- `Assimalign.Cohesion.Connections`: `IAmqpConnection` is an `IConnection`, and SASL mechanisms read the carrier connection's features (its TLS facts, for EXTERNAL).

`Assimalign.Cohesion.Amqp.Connections` references this package; this package never references it.

## Key types

| Area | Types |
|---|---|
| Connection, session, link | `IAmqpConnection`, `IAmqpSession`, `IAmqpLink`, `IAmqpSender`, `IAmqpReceiver`, `IAmqpLinkRequest` (an incoming attach to accept or refuse), `IAmqpDelivery` |
| Options | `AmqpSessionOptions`, `AmqpLinkOptions` with `AmqpSenderOptions` and `AmqpReceiverOptions`, `AmqpDeliveryOptions` |
| Link resumption | `AmqpUnsettledReconciliation` (`IAmqpLink.UnsettledReconciliation`), `AmqpDeliveryOptions.Resume`, `IAmqpSender.AbortDeliveryAsync`, `IAmqpDelivery.IsResumed` and `IsAborted` |
| Messages | `AmqpMessage`; its sections `AmqpHeader`, `AmqpProperties`, the annotation and property maps, and one body: `AmqpDataBody`, `AmqpSequenceBody` or `AmqpValueBody` |
| Delivery states | `AmqpDeliveryState`: `AmqpReceived`, `AmqpTransactionalState`, and the outcomes `AmqpAccepted`, `AmqpRejected`, `AmqpReleased`, `AmqpModified` and `AmqpDeclared` |
| Termini | `AmqpSource`, `AmqpTarget` and `AmqpCoordinator` (both `IAmqpTarget`), `AmqpTerminusDurability`, `AmqpTerminusExpiryPolicy`, `AmqpDistributionMode`, `AmqpLifetimePolicy`, `AmqpNodeProperties` |
| Values | `AmqpSymbol`, `AmqpDeliveryTag`, `AmqpDecimal32`, `AmqpDecimal64`, `AmqpDecimal128`, `AmqpDescribedValue`, `AmqpArray` with `AmqpPrimitiveType` |
| SASL | `IAmqpSaslClientMechanism`, `IAmqpSaslServerMechanism` and their exchanges; PLAIN, ANONYMOUS and EXTERNAL on both sides; `IAmqpSaslConnectionInfo`, read as `connection.AmqpSasl` |
| Transactions | `AmqpTransactionController` (`IAmqpTransactionController`), `AmqpTransactionCoordinator` (`IAmqpTransactionCoordinator`) over an `IAmqpTransactionHandler`, `AmqpDeclare`, `AmqpDischarge`, `AmqpTransactionCapability` |
| Errors | `AmqpException` with `AmqpErrorCode`; `AmqpError` and the standard conditions in `AmqpErrorCondition` |

## Usage

A client sends a message and waits for its outcome. The connection comes from
`AmqpConnectionFactory` in `Assimalign.Cohesion.Amqp.Connections`:

```csharp
using Assimalign.Cohesion.Amqp;
using Assimalign.Cohesion.Amqp.Connections;

AmqpConnectionFactory factory = AmqpConnectionFactory.Create(tcpFactory, new AmqpConnectionFactoryOptions
{
    SaslMechanism = AmqpSaslPlainClientMechanism.Create("user", password)
});

await using IAmqpConnection connection = await factory.ConnectAsync(endPoint, cancellationToken);
IAmqpSession session = await connection.BeginSessionAsync(cancellationToken: cancellationToken);
IAmqpSender sender = await session.AttachSenderAsync(new AmqpSenderOptions
{
    Target = new AmqpTarget { Address = "orders" }
}, cancellationToken);

AmqpDeliveryState? outcome = await sender.SendAsync(new AmqpMessage
{
    Body = new AmqpValueBody("order 42")
}, cancellationToken: cancellationToken);
```

A server accepts what a peer attaches and settles each delivery:

```csharp
await using IAmqpConnection connection = await listener.AcceptAsync(cancellationToken);
IAmqpSession session = await connection.AcceptSessionAsync(cancellationToken);
IAmqpLinkRequest request = await session.AcceptLinkAsync(cancellationToken);

if (request.Target is not AmqpTarget { Address: "orders" })
{
    await request.RejectAsync(new AmqpError(AmqpErrorCondition.NotFound), cancellationToken);
    return;
}

IAmqpReceiver receiver = await request.AcceptReceiverAsync(cancellationToken: cancellationToken);
IAmqpDelivery delivery = await receiver.ReceiveAsync(cancellationToken);
AmqpMessage message = delivery.GetMessage();
await delivery.AcceptAsync(cancellationToken);
```

Work done under a transaction is declared and discharged through a controller:

```csharp
await using AmqpTransactionController controller = await AmqpTransactionController.AttachAsync(session, cancellationToken: cancellationToken);
ReadOnlyMemory<byte> transactionId = await controller.DeclareAsync(cancellationToken);

await sender.SendAsync(message, new AmqpDeliveryOptions
{
    State = new AmqpTransactionalState(transactionId)
}, cancellationToken);

await controller.DischargeAsync(transactionId, cancellationToken: cancellationToken);
```

A broker runs the other end with `AmqpTransactionCoordinator.AcceptAsync(request, handler)` and its
own `IAmqpTransactionHandler`. The [design](design.md) records what each side guarantees.

## Source layout

```
src/
├── Abstractions/      connection, session, link, delivery, SASL and transaction contracts
├── Exceptions/        AmqpException and AmqpErrorCode
├── Extensions/        connection.AmqpSasl
├── Internal/          the built-in SASL exchanges
├── ValueObjects/      AmqpSymbol, AmqpDeliveryTag and the decimal types
├── Properties/        AssemblyInfo.cs and ErrorMessages.resx
└── *.cs               messages, delivery states, termini, options, link resumption, SASL mechanisms, transactions
```

## Principal public types

| Type | Source |
|---|---|
| `AmqpAccepted` | `src/AmqpOutcome.Accepted.cs` |
| `AmqpArray` | `src/AmqpArray.cs` |
| `AmqpCoordinator` | `src/AmqpCoordinator.cs` |
| `AmqpDataBody` | `src/AmqpMessageBody.Data.cs` |
| `AmqpDecimal128` | `src/ValueObjects/AmqpDecimal128.cs` |
| `AmqpDecimal32` | `src/ValueObjects/AmqpDecimal32.cs` |
| `AmqpDecimal64` | `src/ValueObjects/AmqpDecimal64.cs` |
| `AmqpDeclare` | `src/AmqpDeclare.cs` |
| `AmqpDeclared` | `src/AmqpOutcome.Declared.cs` |
| `AmqpDeliveryOptions` | `src/AmqpDeliveryOptions.cs` |
| `AmqpDeliveryState` | `src/AmqpDeliveryState.cs` |
| `AmqpDeliveryTag` | `src/ValueObjects/AmqpDeliveryTag.cs` |
| `AmqpDescribedValue` | `src/AmqpDescribedValue.cs` |
| `AmqpDischarge` | `src/AmqpDischarge.cs` |
| `AmqpDistributionMode` | `src/AmqpDistributionMode.cs` |
| `AmqpError` | `src/AmqpError.cs` |
| `AmqpErrorCode` | `src/Exceptions/AmqpErrorCode.cs` |
| `AmqpErrorCondition` | `src/AmqpErrorCondition.cs` |
| `AmqpException` | `src/Exceptions/AmqpException.cs` |
| `AmqpHeader` | `src/AmqpHeader.cs` |
| `AmqpLifetimePolicy` | `src/AmqpLifetimePolicy.cs` |
| `AmqpLinkOptions` | `src/AmqpLinkOptions.cs` |
| `AmqpMessage` | `src/AmqpMessage.cs` |
| `AmqpMessageBody` | `src/AmqpMessageBody.cs` |
| `AmqpModified` | `src/AmqpOutcome.Modified.cs` |
| `AmqpNodeProperties` | `src/AmqpNodeProperties.cs` |
| `AmqpOutcome` | `src/AmqpOutcome.cs` |
| `AmqpPrimitiveType` | `src/AmqpPrimitiveType.cs` |
| `AmqpProperties` | `src/AmqpProperties.cs` |
| `AmqpReceived` | `src/AmqpDeliveryState.Received.cs` |
| `AmqpReceiverOptions` | `src/AmqpReceiverOptions.cs` |
| `AmqpReceiverSettleMode` | `src/AmqpReceiverSettleMode.cs` |
| `AmqpRejected` | `src/AmqpOutcome.Rejected.cs` |
| `AmqpReleased` | `src/AmqpOutcome.Released.cs` |
| `AmqpRole` | `src/AmqpRole.cs` |
| `AmqpSaslAnonymousClientMechanism` | `src/AmqpSaslAnonymousClientMechanism.cs` |
| `AmqpSaslAnonymousServerMechanism` | `src/AmqpSaslAnonymousServerMechanism.cs` |
| `AmqpSaslCode` | `src/AmqpSaslCode.cs` |
| `AmqpSaslConnectionFeatureExtensions` | `src/Extensions/AmqpSaslConnectionFeatureExtensions.cs` |
| `AmqpSaslExternalClientMechanism` | `src/AmqpSaslExternalClientMechanism.cs` |
| `AmqpSaslExternalServerMechanism` | `src/AmqpSaslExternalServerMechanism.cs` |
| `AmqpSaslPlainClientMechanism` | `src/AmqpSaslPlainClientMechanism.cs` |
| `AmqpSaslPlainCredentials` | `src/AmqpSaslPlainCredentials.cs` |
| `AmqpSaslPlainServerMechanism` | `src/AmqpSaslPlainServerMechanism.cs` |
| `AmqpSaslServerStep` | `src/AmqpSaslServerStep.cs` |
| `AmqpSenderOptions` | `src/AmqpSenderOptions.cs` |
| `AmqpSenderSettleMode` | `src/AmqpSenderSettleMode.cs` |
| `AmqpSequenceBody` | `src/AmqpMessageBody.Sequence.cs` |
| `AmqpSessionOptions` | `src/AmqpSessionOptions.cs` |
| `AmqpSource` | `src/AmqpSource.cs` |
| `AmqpSymbol` | `src/ValueObjects/AmqpSymbol.cs` |
| `AmqpTarget` | `src/AmqpTarget.cs` |
| `AmqpTerminusDurability` | `src/AmqpTerminusDurability.cs` |
| `AmqpTerminusExpiryPolicy` | `src/AmqpTerminusExpiryPolicy.cs` |
| `AmqpTransactionCapability` | `src/AmqpTransactionCapability.cs` |
| `AmqpTransactionController` | `src/AmqpTransactionController.cs` |
| `AmqpTransactionCoordinator` | `src/AmqpTransactionCoordinator.cs` |
| `AmqpTransactionalState` | `src/AmqpDeliveryState.Transactional.cs` |
| `AmqpUnsettledReconciliation` | `src/AmqpUnsettledReconciliation.cs` |
| `AmqpValueBody` | `src/AmqpMessageBody.Value.cs` |
| `IAmqpConnection` | `src/Abstractions/IAmqpConnection.cs` |
| `IAmqpDelivery` | `src/Abstractions/IAmqpDelivery.cs` |
| `IAmqpLink` | `src/Abstractions/IAmqpLink.cs` |
| `IAmqpLinkRequest` | `src/Abstractions/IAmqpLinkRequest.cs` |
| `IAmqpReceiver` | `src/Abstractions/IAmqpReceiver.cs` |
| `IAmqpSaslClientExchange` | `src/Abstractions/IAmqpSaslClientExchange.cs` |
| `IAmqpSaslClientMechanism` | `src/Abstractions/IAmqpSaslClientMechanism.cs` |
| `IAmqpSaslConnectionInfo` | `src/Abstractions/IAmqpSaslConnectionInfo.cs` |
| `IAmqpSaslServerExchange` | `src/Abstractions/IAmqpSaslServerExchange.cs` |
| `IAmqpSaslServerMechanism` | `src/Abstractions/IAmqpSaslServerMechanism.cs` |
| `IAmqpSender` | `src/Abstractions/IAmqpSender.cs` |
| `IAmqpSession` | `src/Abstractions/IAmqpSession.cs` |
| `IAmqpTarget` | `src/Abstractions/IAmqpTarget.cs` |
| `IAmqpTransactionController` | `src/Abstractions/IAmqpTransactionController.cs` |
| `IAmqpTransactionCoordinator` | `src/Abstractions/IAmqpTransactionCoordinator.cs` |
| `IAmqpTransactionHandler` | `src/Abstractions/IAmqpTransactionHandler.cs` |

## Sources

- **Source** — `cohesion/libraries/Amqp/Assimalign.Cohesion.Amqp/src/Assimalign.Cohesion.Amqp.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Amqp/Assimalign.Cohesion.Amqp/README.md`.

- **Source** — `cohesion/libraries/Amqp/Assimalign.Cohesion.Amqp/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Amqp/Assimalign.Cohesion.Amqp/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Amqp/Assimalign.Cohesion.Amqp/src`.
