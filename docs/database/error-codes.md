# Database error codes

A runtime diagnostic identity has two uppercase letters and five decimal digits. Branch on the
complete code, such as `CS05067`, rather than a prefix, numeric suffix, message or SQLSTATE.
Identities are append-only; retired identities remain reserved and `00000` is unassigned.

| Prefix | Owner | Authoritative per-model registry |
| --- | --- | --- |
| CK | Shared kernel and infrastructure | [Kernel](https://github.com/assimalign/cohesion/blob/79944702b/resources/Database/Assimalign.Cohesion.Database/docs/ERROR-CODES.md#kernel-migration) |
| CS | SQL | [SQL](https://github.com/assimalign/cohesion/blob/79944702b/resources/Database/Assimalign.Cohesion.Database/docs/ERROR-CODES.md#sql-migration) |
| CD | Documents | [Documents](https://github.com/assimalign/cohesion/blob/79944702b/resources/Database/Assimalign.Cohesion.Database/docs/ERROR-CODES.md#documents-migration) |
| CB | Blob | [Blob](https://github.com/assimalign/cohesion/blob/79944702b/resources/Database/Assimalign.Cohesion.Database/docs/ERROR-CODES.md#blob-migration) |
| CG | Graph | [Graph](https://github.com/assimalign/cohesion/blob/79944702b/resources/Database/Assimalign.Cohesion.Database/docs/ERROR-CODES.md#graph-migration) |
| CV | KeyValuePair | [KeyValuePair](https://github.com/assimalign/cohesion/blob/79944702b/resources/Database/Assimalign.Cohesion.Database/docs/ERROR-CODES.md#keyvaluepair-migration) |
| CC | Cache (reserved) | [Registry](https://github.com/assimalign/cohesion/blob/79944702b/resources/Database/Assimalign.Cohesion.Database/docs/ERROR-CODES.md) |

The suffix allocates two category digits and three condition digits within these ranges.
Categories help organize the registry; callers still match the complete identity.

| Suffix range | Category |
| --- | --- |
| 00001–00100 | Language and syntax |
| 00101–00999 | Reserved |
| 01000–01999 | Binding and name resolution |
| 02000–02999 | Values, types and encoding |
| 03000–03999 | Schema, catalog and constraints |
| 04000–04999 | Transactions, locking and transaction state |
| 05000–05999 | Execution, capabilities and budgets |
| 06000–06999 | Storage, durability and format |
| 07000–07999 | Protocol, transport, authentication and sessions |
| 08000–08999 | Configuration and argument validation |
| 09000–09999 | Hosting, pooling and lifecycle |
| 10000–99999 | Reserved |

`Diagnostic.Code` carries the identity separately from its unprefixed `Message`. Legacy
message-only failures carry one `CODE: message` prefix; this includes the SQL declared-script
`CS03201`–`CS03205` refusals. Wire Error text also composes one prefix for legacy readers, while
structured diagnostic messages remain unprefixed. Do not parse a human-readable message when a
diagnostic carrier is available.

The migration preserves SQLSTATEs, numeric wire error kinds, frame layouts, protocol versions
and disk formats. Standard .NET argument guards retain their exception contracts. Build and SDK
diagnostics, including `COHDBSDK…`, `COHDBCLIENT001`, `COHRES…`, `COHAM001` and `COHSDK002`,
are independent analyzer identities and remain unchanged.

Use the model references for current conditions: [SQL](sql/language/diagnostics.md),
[Documents](document/language/diagnostics.md), [Graph](graph/language/diagnostics.md),
[Key-Value Pair](key-value-pair/diagnostics.md), [Blob](blob/operations.md) and [Cache](cache/index.md).
The source registry also lists [newly coded conditions](https://github.com/assimalign/cohesion/blob/79944702b/resources/Database/Assimalign.Cohesion.Database/docs/ERROR-CODES.md#rules-audit-condition-additions).

The omitted SQL semicolon is informational `CS00009`; it does not refuse parsing or execution.
`CS00100` is retired and remains reserved. Recognized unsupported clauses use `CK00001`.
Hard SQL deadlocks carry `CS04065`, mapped to SQLSTATE `40P01` (`deadlock_detected`). These
SQLSTATE mappings are documented code mappings, not separate wire fields.

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database/docs/ERROR-CODES.md` at `79944702b`.
