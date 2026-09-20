# Unsupported language features

OQL reserves vocabulary beyond its executable subset and rejects those constructs with explicit diagnostics.

> **Status:** Partial. Recognition of a keyword or function does not imply execution support.

## Language matrix

| Construct | Support | Boundary |
|---|---|---|
| `DEFINE` | Recognized, not supported (`COHDBL001`) | Named query definitions |
| `ELEMENT` | Recognized, not supported (`COHDBL001`) | Singleton extraction |
| `FLATTEN` | Recognized, not supported (`COHDBL001`) | Collection expansion |
| Nested `SELECT` | Recognized, not supported (`COHDBL001`) | Reported as `SUBQUERY` |
| `DISTINCT`, `ALL` | Recognized, not supported (`COHDBL001`) | Projection and aggregate modifiers |
| `IN`, `EXISTS`, `LIKE`, `BETWEEN` | Recognized, not supported (`COHDBL001`) | Additional predicates |
| `FOR`, `SOME`, `ANY` | Recognized, not supported (`COHDBL001`) | Quantifiers |
| `STRUCT`, `LIST`, `SET`, `BAG`, `ARRAY`, `COLLECTION` | Recognized, not supported (`COHDBL001`) | Collection constructors |
| `FIRST`, `LAST`, `UNIQUE`, `LISTTOSET`, `TYPEOF`, `UNDEFINED` | Recognized, not supported (`COHDBL001`) | Reserved operations and values |
| `ABS`, unknown function calls | Recognized, not supported (`COHDBL001`) | Only the five documented aggregates execute |
| `INSERT`, `UPDATE`, `DELETE` | Recognized, not supported (`COHDBL001`) | Use collection API mutations |
| `CREATE DATABASE`, `DROP DATABASE`, `USE`, `ALTER` | Recognized, not supported (`COHDBL001`) | No server or database administration language |
| `BEGIN`, `COMMIT`, `ROLLBACK` | Recognized, not supported (`COHDBL001`) | Transactions use engine/session APIs |
| `WITH`, `JOIN`, `LIMIT`, `OFFSET`, `UNION`, `INTERSECT`, `EXCEPT` | Recognized, not supported (`COHDBL001`) | No corresponding query clauses |
| General database-qualified source | Not in the language | `other.people` produces `OQL0002` |
| Multiple statements per parse | Not in the language | Produces `OQL0002` |
| Negative, fractional, or oversized array subscript | Not in the language | Produces `OQL0002` |
| `?` parameter marker | Not in the language | Produces `OQL0002` |

Non-OQL command names are checked in command or clause positions; they are not universally reserved
document property names. Quoting and properties after dots preserve ordinary field access.

## Engine limits

Queries materialize input and results, and whole JSON values remain in managed memory. There is no
external sort or query-cost statistics model. Configurable document collations are deferred;
strings remain ordinal and case-sensitive. Document protocol server/client, replication, security,
hosting wiring, ApplicationModel integration, and compiled-schema provisioning are outside the
current engine implementation.

## See also

[Language (OQL)](index.md), [Diagnostics](diagnostics.md), and [Document](../index.md).

## Sources

- **Reserved vocabulary** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/OqlLanguageProfile.cs`.
- **Capability validation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/OqlQueryParser.cs`.
- **Rejected forms** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlQueryParserTests.cs`.
- **Engine limits** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/docs/DESIGN.md`.
