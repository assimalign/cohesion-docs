# CALL

Invokes an application scalar registered on the receiving engine.

> **Status:** Implemented in process and over InMemory/TCP.

## Syntax

```text
CALL function_name [ @argument ... ]
```

## Arguments

- **function_name** — an ASCII letter/underscore followed by ASCII letters, digits or underscores; matched ordinally ignoring case.
- **@argument** — a named parameter containing a native primitive value or NULL. Zero arguments is allowed.

## Remarks

The host registers [KeyValueFunction](../../../dotnet-apis/resources/database/assimalign-cohesion-database-keyvaluepair/key-value-function.md)
on its engine builder before build. CALL has no inline data literals, expression evaluation,
comments, quoting, semicolon or composition. It returns one nullable column named value, of
the function's declared primitive type, one row and affected count -1. There is no MISSING value.

Overloads match argument count and exact non-NULL primitive types. Int32 and Int64 differ;
there is no numeric widening or SQL cast. NULL is compatible with every parameter type, so it
can make a call ambiguous. Resolution occurs before strict short-circuiting.
ReturnsNullOnNullInput returns NULL without invoking the body when any input is NULL;
CalledOnNullInput invokes it once. Other valid calls invoke the body exactly once, with no
optimizer folding or determinism promise. There are no built-in functions or aggregates.

Callbacks are trusted, stateless or thread-safe application code. They must not reenter an
engine; same-session commands during CALL report CK07003 before execution. External effects
cannot be rolled back. Ordinary callback failures leave an explicit transaction active.
I/O/timeouts thrown by callbacks use CV05003 and retain their cause, rather than becoming
engine storage failures. Actual statement cancellation and existing DatabaseException keep
their identity. Completed execution failures leave the wire session usable.
Arguments and binary buffers are detached; binary results are caller-owned.

## Diagnostics

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CV00019 | CALL has no function name; parse failure | — |
| CV01018 | Name absent from the engine catalog | — |
| CV01019 | No overload accepts the argument count | — |
| CV01020 | No overload accepts the exact non-NULL types | — |
| CV01021 | More than one overload accepts NULL-compatible inputs | — |
| CV02016 | Result is unsupported or violates the declared primitive type | — |
| CV05003 | Application callback failed; original cause retained | — |
| CK07003 | Another command or reentry attempted while CALL owns the session | — |

## Example

The tests register the callback before building the engine:

```csharp
var builder = KeyValueDatabaseEngine.CreateBuilder("typed-functions");
builder.Functions.Add(KeyValueFunction.Create<long, long>("f1", value => value));
await using var engine = await builder.BuildAsync(CancellationToken.None);
```

On a session of that engine, bind arg to CLR 4L and send:

```text
CALL f1 @arg
```

The result is one row with Int64 value 4. A CLR Int32 value 4 reports CV01020. NULL returns
one NULL cell without invoking this strict callback. The typed embedded equivalent is
KeyValueFunctionRequest("f1", new object?[] { 4L }); the client equivalent is:

```csharp
object? value = await connection.InvokeFunctionAsync("f1", new object?[] { 4L }); // 4L
```

## See also

[Commands](index.md) · [Diagnostics](../diagnostics.md) · [Operand types](../operand-types.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/tests/KeyValueFunctionTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/docs/COMMANDS.md`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair.Client/tests/KeyValueFunctionClientTests.cs`.
