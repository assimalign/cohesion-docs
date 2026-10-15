# EXISTS

EXISTS reports whether a key has a visible entry.

> **Status:** Implemented.

## Syntax

```syntaxsql
EXISTS @key
```

## Supported clauses

`EXISTS` accepts one key parameter and no additional clauses.

## Arguments

`@key` binds a nonempty `byte[]`. The result contains one row and one Boolean column, `exists`.

## Remarks

The visibility decision uses the command's snapshot and the primary key index. An absent key
returns a row containing false; it does not return an empty result set.

## Examples

The case-insensitivity test writes the key and then executes this exact lowercase command,
with `k` bound to that key's bytes. The result is true.

```text
exists @k
```

## See also

- **[Commands](index.md)** — grammar and lexical conventions.
- **[GET](get.md)** — retrieve the entry itself.
- **[Operand types](../operand-types.md)** — parameter binding.

## Sources

- **Grammar** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/docs/COMMANDS.md`.
- **Request** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/src/KeyValueExistsRequest.cs`.
- **Example** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/tests/KeyValueCommandTests.cs`.
