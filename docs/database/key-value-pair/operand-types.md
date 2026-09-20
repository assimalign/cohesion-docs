# Operand types

Key-Value Pair commands bind data through named parameters with fixed runtime types.

> **Status:** Implemented.

The text token `@name` resolves against the bare name `name` in the execute call's parameter
map. The grammar admits no inline key, value, string, or binary literals. A `LIMIT` count is
the only literal operand.

## Binding reference

| Operand | Required value | Wire component |
|---|---|---|
| Key, value, start, end, prefix | `byte[]` | `Binary` |
| Expected etag | `long` or `int` | `Int64` or `Int32`, accepted as an integer |
| Limit parameter | Nonnegative `int` or `long`, at most `int.MaxValue` | `Int32` or `Int64` |
| Limit literal | Nonnegative integer fitting `int` | Part of the statement text |

The text parser specifically accepts `byte[]`; a string or boxed `ReadOnlyMemory<byte>` does
not satisfy a byte operand. The typed request constructors separately accept
`ReadOnlyMemory<byte>`. Point-operation keys cannot be empty. A value may be an empty byte array.

## Errors

A missing binding, a non-`@` data operand, null, or a wrong operand type raises
`DatabaseParseException`. Request-constructor argument errors are also converted to parse
errors at the text boundary. No execution occurs for these failures.

The corpus verifies that `GET @unbound` without a parameter map fails, and that binding `k`
to the string `not-bytes` for `GET @k` fails. Encode application text into bytes before binding;
the engine does not choose an encoding or apply text collation to those bytes.

## See also

- **[Key-Value Pair](index.md)** — engine behavior.
- **[Commands](commands/index.md)** — grammar and examples.
- **[Wire protocol](wire-protocol.md)** — scalar components.
- **[Diagnostics](diagnostics.md)** — parse and execution errors.

## Sources

- **Grammar** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/docs/COMMANDS.md`.
- **Parser** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/src/Internal/KeyValueCommandParser.cs`.
- **Request validation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/src/KeyValuePutRequest.cs`.
- **Conformance** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/tests/KeyValueCommandTests.cs`.
