# Unsupported features

The Key-Value Pair command contract excludes query composition, administration, and several deferred engine features.

> **Status:** Partial. The six-command surface is implemented; the capabilities below are excluded or deferred.

## Grammar boundary

| Construct | Support | Boundary |
|---|---|---|
| `GET`, `PUT`, `DELETE`, `EXISTS`, `SCAN`, `KEYSPACES` | Supported | See the command reference |
| `BEGIN`, `COMMIT`, `ROLLBACK` | Not in the language | Embedded session transactions exist; wire transaction frames are deferred |
| Multi-key or atomic batch commands | Not in the language | No batch grammar |
| Database-management commands or database selectors | Not in the language | Provisioning belongs to the host-owned engine |
| Expressions, joins, or statement composition | Not in the language | Commands perform individual operations |
| Inline key/value literals | Not in the language | Data operands are named parameters |
| Comments, quoted identifiers, semicolon terminators | Not in the language | Parser splits a single command on whitespace |
| Catalog writes through `PUT KEYSPACES` or `DELETE KEYSPACES` | Not in the language | Stable read-only parse error |

The key-value parser does not use `COHDBL001` for these exclusions. It reports
`DatabaseParseException`, mapped to `ParseFailure` on the wire.

## Deferred engine capabilities

- **Expiration** — time to live (TTL) and per-entry expiration are deferred; there is no
  `ExpiresAt` surface in the current cut.
- **Named spaces** — each database has one implicit key space; `KEYSPACES` does not create a
  named-space registry.
- **Advanced indexing** — secondary value indexes and index compaction are deferred. The
  maintenance worker remains a stub for compaction.
- **Isolation** — `Serializable` is rejected; current transaction support is `Snapshot` and
  `ReadCommitted`.
- **Text collation** — configurable collation is deferred. Keys remain opaque bytes ordered by
  unsigned lexicographic comparison.
- **Model-specific replication and security packages** — their project files are scaffolds,
  without implementations in those package directories. This does not remove the server's
  shared authentication handshake.

## See also

- **[Key-Value Pair](index.md)** — implemented engine behavior.
- **[Commands](commands/index.md)** — accepted syntax.
- **[Diagnostics](diagnostics.md)** — parse and execution errors.
- **[Cache](../cache/index.md)** — separately deferred Cache model.

## Sources

- **Grammar non-goals** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/docs/COMMANDS.md`.
- **Engine non-goals** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/docs/DESIGN.md`.
- **Parser** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/src/Internal/KeyValueCommandParser.cs`.
- **Replication scaffold** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair.Replication/src/Assimalign.Cohesion.Database.KeyValuePair.Replication.csproj`.
- **Security scaffold** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair.Security/src/Assimalign.Cohesion.Database.KeyValuePair.Security.csproj`.
