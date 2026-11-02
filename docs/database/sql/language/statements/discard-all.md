# DISCARD ALL (Cohesion SQL)

Clears retained SQL session state outside an explicit transaction.

## Syntax

```syntaxsql
DISCARD ALL [ ; ]
```

## Remarks

DISCARD ALL clears sequence currval/lastval observations and restores statement/lock timeout
values configured before the session's first statement. These original host reset values survive
user overrides and repeated discards. It does not rewind a sequence allocator. Inside an explicit
transaction it reports CS04070 (25001).

After each negotiated exchange, the Ready postamble reports transaction state and a second byte
for retained session state. SQL sets it for retained sequence entries or timeout settings different
from the original reset values. On pool return the client first rolls back an active/failed
transaction, then sends DISCARD ALL through Execute only if the latest Ready still reports retained
state. A clean SELECT 1 or idle return adds no reset exchange; a user DISCARD avoids a second reset.
Graph and Documents report no retained state requiring this family reset. See
[connection pooling](../../../../dotnet-apis/resources/database/assimalign-cohesion-database-client/design.md)
and [ProtocolTransactionStatusMessage](../../../../dotnet-apis/resources/database/assimalign-cohesion-database-protocol/protocol-transaction-status-message.md).

## Example

Execute separately in one session:

```sql
CREATE SEQUENCE s;
SELECT nextval('s');
DISCARD ALL;
SELECT currval('s');
```

The first SELECT returns `1`; the last reports CS04068 (55000). The next nextval would return `2`.

## See also

[Statements](index.md) · [Sequence functions](../functions/sequences.md) · [Transactions](begin-transaction.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlSequenceSecondPassTests.Reset.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlSequenceThirdPassTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Client/tests/SqlRetainedStateResetTests.cs`.
