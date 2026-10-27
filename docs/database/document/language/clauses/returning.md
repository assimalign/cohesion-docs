# RETURNING

Returns one projected row per document affected by INSERT, UPSERT, UPDATE or DELETE.

## Syntax

```syntaxsql
RETURNING expression [ AS alias ] [ , ...n ]
RETURNING RAW expression
RETURNING VALUE expression
```

Append the clause after VALUES or the final SELECT source on INSERT/UPSERT, or after the optional
WHERE on UPDATE/DELETE. A lone `RETURNING *` returns the complete image with its stored UTF-8
bytes. RAW and VALUE are synonyms for one unwrapped expression, without alias or star.

## Remarks

| Statement | Image and metadata returned | Row order |
| --- | --- | --- |
| INSERT / UPSERT | Stored post-image and newly allocated durable version | VALUES order or final SELECT source order |
| UPDATE | Stored post-image and new version | Ordinal document-ID order |
| DELETE | Pre-image and old version | Ordinal document-ID order |

The target alias, or collection name without an alias, binds the image. META().id and
META(binding).version address that image's metadata. INSERT/UPSERT SELECT source bindings do
not escape into RETURNING. Scalar functions, parameters, predicates, CASE, constructors and
[ARRAY/FIRST comprehensions](../expressions/comprehensions.md) work;
aggregates and subqueries are excluded. Names and types follow [SELECT](../statements/select.md).
Named MISSING members are omitted; explicit NULL remains JSON null. RAW/VALUE converts top-level
MISSING to JSON null. Constructors omit MISSING object members and replace MISSING array elements
with null. ELEMENT is not a synonym for VALUE.

There is no separate RETURNING DISTINCT, sort or paging. Zero matches returns an empty result with
projection metadata and affected count 0. Parameter and expression-context validation still runs.
Without RETURNING, writes retain count-only completion. EXPLAIN validates the clause and describes
Returning/ReturningValue without reading documents, invoking projection callbacks or writing.

### Atomicity and limits

The affected count accompanies the rows and equals their count. Projection, serialization preflight
and writes complete inside the same transaction. A failed projection or cancellation rolls back the
statement; an explicit transaction follows the normal whole-transaction abort-until-rollback rule.
No partial result reaches the caller. Returned JSON owns its data.

Serialization preflight enforces the 16 MiB frame limit (CD05016) and maximum depth of 256 levels
(CD05019), counting the wrapper object for named projections, before commit. Constructors retain
their own depth diagnostic, CD05015. Storage JSON retains its separate 128-level limit.

[DocumentDatabaseEngineOptions.StatementMemoryBudget](../../../../dotnet-apis/resources/database/assimalign-cohesion-database-documents/document-database-engine-options.md)
defaults to 64 MiB and must be positive. It estimates retained RETURNING metadata, rows, scalar
values, JSON clones and preserved whole-image bytes. Each clone costs 128 bytes plus UTF-8 length
plus 12 bytes per JSON token. A lone star charges one clone and one raw image, once per
representation. This estimate covers RETURNING retention; existing SELECT and mutation/source
staging allocations are outside it. Exhaustion fails with CD05018 and rolls back the writes.

### Client compatibility

There is no RETURNING capability flag or negotiation. Updated clients accept rows with an equal,
nonnegative affected count. An older client rejects this completion with a protocol violation and
closes the connection **after an autocommit write has committed**. Retrying reapplies the write:
`SET n = n + 10` can leave 11 from an original 1, then 21 on retry. In an explicit transaction,
the closed session rolls back the uncommitted work. Use an updated client for RETURNING; the older
client's error does not mean the autocommit write rolled back. See
[client compatibility](../../../../dotnet-apis/resources/database/assimalign-cohesion-database-documents-client/design.md#returning-and-client-compatibility).

## Diagnostics

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CD01003 | Duplicate output name; use distinct aliases | — |
| CD01026 | Empty RETURNING output name in a directly constructed AST, before mutation | — |
| CD00002 | Malformed projection or empty quoted output label in text | — |
| CD05001 | Invalid expression context, including aggregates | — |
| CD01005 | Unbound parameter, including on empty input | — |
| CD05015 | Array/object constructor exceeds the 256-level result limit, at its expression | — |
| CD05016 | Serialized result row or header exceeds the 16 MiB frame limit | — |
| CD05018 | Retained result exceeds StatementMemoryBudget, at the clause | — |
| CD05019 | Serialized value exceeds 256 levels, including the named wrapper, at the clause | — |
| CK00001 | Unsupported projection form, such as a subquery or ELEMENT call | — |

Other expressions retain their [normal diagnostic codes](../diagnostics.md).

## Example

Execute separately in a fresh database; the image, metadata and rollback cases are covered by
DocumentReturningTests and DocumentReturningWireTests.

```sql
CREATE COLLECTION items;
INSERT INTO items (KEY, VALUE) VALUES ('a', {n:1}), ('b', {n:2})
RETURNING META().id AS id, n, absent;
UPDATE items SET n = n + 10 RETURNING n;
DELETE FROM items WHERE n = 11 RETURNING RAW n;
UPSERT INTO items (KEY, VALUE) VALUES ('b', {n:20}) RETURNING VALUE MISSING;
```

INSERT affects 2 and returns `{"id":"a","n":1}`, `{"id":"b","n":2}`; absent is omitted.
UPDATE affects 2 and returns `{"n":11}`, `{"n":12}`. DELETE affects 1 and returns bare `11`,
the pre-image value. UPSERT affects 1 and returns bare `null`.

```sql
UPDATE items SET n = 99 RETURNING 1 / 0 AS broken;
SELECT n FROM items;
```

UPDATE fails with CD02006 (SQLSTATE 22012). In an automatic session the SELECT still returns
`{"n":20}`: the write rolled back.

## See also

[Clauses](index.md) · [INSERT](../statements/insert.md) · [UPSERT](../statements/upsert.md) · [UPDATE](../statements/update.md) · [DELETE](../statements/delete.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentReturningTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentReturningBudgetTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Client/tests/DocumentReturningWireTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/src/Internal/Execution/DocumentPlanExecutor.Returning.cs`.
