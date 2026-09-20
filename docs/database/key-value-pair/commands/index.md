# Commands

The Key-Value Pair command grammar supplies six operations through the session text execution surface.

> **Status:** Implemented.

This grammar has no expressions or composition. Every data operand is a named parameter;
only a `LIMIT` count may appear as a literal. The wire `Execute` message carries the command
and its named tuple-codec parameters.

## Command reference

| Command | Support | Result |
|---|---|---|
| [GET](get.md) | Supported | Zero or one entry |
| [PUT](put.md) | Supported | One applied/etag outcome |
| [DELETE](delete.md) | Supported | Affected count |
| [EXISTS](exists.md) | Supported | One Boolean row |
| [SCAN](scan.md) | Supported | Entries in ascending key order |
| [KEYSPACES](keyspaces.md) | Supported | One metadata row for the implicit key space |

## Syntax conventions

Uppercase words are keywords; `snake_case` names identify operands. Square brackets denote
optional syntax. Braces enclose a required choice and `|` separates alternatives. These notation
characters are not command text. The `@` prefix is literal: `@key` references the parameter
whose map key is `key`.

Keywords are case-insensitive and tokens separate on whitespace. There is no identifier or
string-literal quoting syntax and no comment syntax. Commands are single whitespace-tokenized
statements without semicolon terminators. Parameter lookup uses the supplied map's bare names.

## See also

- **[Key-Value Pair](../index.md)** — engine scope and transaction behavior.
- **[Operand types](../operand-types.md)** — required parameter values.
- **[Unsupported features](../unsupported.md)** — excluded forms.

## Sources

- **Grammar** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/docs/COMMANDS.md`.
- **Parser** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/src/Internal/KeyValueCommandParser.cs`.
- **Conformance** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/tests/KeyValueCommandTests.cs`.
