# DocumentProtocol

Defines the immutable Documents endpoint family, its messages and command operations.

Namespace: `Assimalign.Cohesion.Database.Documents.Protocol`.

Assembly: `Assimalign.Cohesion.Database.Documents.Protocol.dll`. NuGet package: `Assimalign.Cohesion.Database.Documents.Protocol`.

## Signature

```csharp
public static class DocumentProtocol { public static ProtocolMessageFamily Family { get; } }
public enum DocumentProtocolMessageType : byte
{
    Execute = 64, Document = 65, Complete = 66, ResultHeader = 67,
    ResultRow = 68, Command = 69, StoredDocument = 70, Collection = 71, Failure = 72
}
public enum DocumentProtocolOperation : byte
{
    Get = 1, Put = 2, Delete = 3, CreateCollection = 4,
    GetCollection = 5, DropCollection = 6, ListCollections = 7
}
```

## Behavior and exceptions

Family binds one Documents endpoint for the session lifetime. ResultRow (68) is reserved for retired
projection arrays; protocol 2.0 replaced them with Document objects so MISSING can be omitted.
Current startup negotiates shared 2.1. Documents agrees no core Diagnostics capability; Failure
remains its structured diagnostic channel. Documents advertises transaction-status. When agreed,
Ready follows Complete or Failure for Execute (64) and Command (69), with one ASCII byte:
I (Idle), T (Active) or E (Failed). Text controls use Execute; unnegotiated controls report CD07002.
Legacy response framing remains unchanged. Pool reset sends ROLLBACK with empty JSON parameters,
requires Complete (66) with two zero Int64 counts, then Ready I.

Execute results are ResultHeader, zero or more Document frames, then Complete. INSERT, UPSERT, UPDATE, DELETE and DDL can
complete without rows. Commands return StoredDocument or Collection frames as appropriate, then
Complete. Failure terminates a statement without Complete and retains session reuse. Unexpected
message order, shared Error, malformed payloads/UTF-8 or inconsistent counts are protocol
violations. Codecs throw ProtocolException for malformed inputs; the client discards partial
accumulated results on terminal Failure.

## Example

```csharp
var request = DocumentProtocolExecuteMessage.Create("SELECT name FROM items");
byte[] payload = request.Encode();
Console.WriteLine(DocumentProtocolExecuteMessage.Decode(payload).Statement);
// SELECT name FROM items
```

## Message codecs

- **[DocumentProtocolExecuteMessage](document-protocol-execute-message.md)**.
- **[DocumentProtocolResultMessage](document-protocol-result-message.md)**.
- **[DocumentProtocolCompleteMessage](document-protocol-complete-message.md)**.
- **[DocumentProtocolResultHeaderMessage](document-protocol-result-header-message.md)**.
- **[DocumentProtocolCommandMessage](document-protocol-command-message.md)**.
- **[DocumentProtocolStoredDocumentMessage](document-protocol-stored-document-message.md)**.
- **[DocumentProtocolCollectionMessage](document-protocol-collection-message.md)**.
- **[DocumentProtocolFailureMessage](document-protocol-failure-message.md)**.

## Related reference

[Server](../assimalign-cohesion-database-documents/document-database-server.md) · [Client](../assimalign-cohesion-database-documents-client/index.md) · [Shared protocol](../assimalign-cohesion-database-protocol/index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Protocol/src/DocumentProtocol.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentProtocolTests.cs`.
