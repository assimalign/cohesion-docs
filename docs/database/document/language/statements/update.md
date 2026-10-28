# UPDATE

Partially updates matching documents with SET assignments and UNSET property removals.

## Syntax

```syntaxsql
UPDATE collection_name [ [ AS ] iteration_variable ]
{ SET path = expression [ , ...n ] [ UNSET path [ , ...n ] ]
  | UNSET path [ , ...n ] }
[ WHERE predicate ] [ RETURNING projection ] [ ; ]
```

## Arguments

- **`collection_name`** — One stored collection in the session database.
- **`iteration_variable`** — Optional alias for paths and META(alias).
- **`path`** — A case-sensitive [property or zero-based array path](../expressions/paths-and-arrays.md).
- **`expression`** — A nonaggregate value expression evaluated against the original document.
- **`predicate`** — An OQL filter; omission matches every document. NULL and MISSING select no
  documents; other values use [SQL++ truth conversion](../expressions/missing.md).

## Remarks

Every SET value reads the pre-update document, so assignments can swap two fields. The engine
materializes candidates through a collection scan or applicable path index seek, reapplies the
complete predicate, and builds all patched documents from the statement snapshot before writing.
Changing an indexed value cannot revisit a document. Parameters and metadata contexts are checked
even on empty input. META().id addresses the external key; META().version can filter a version
precondition. A stale version matches zero documents.

### SET and UNSET paths

SET creates missing intermediate objects when needed for a write. It never creates or expands an
array: every index requires an existing array and an in-range position. These requirements also
apply to UNSET and SET-to-MISSING beneath absent members. Traversing an existing scalar or NULL
instead of the required object or array fails the statement.

| Operation | Result |
| --- | --- |
| SET a named member to NULL | Writes JSON null |
| SET a named member to MISSING | Removes it; missing intermediate objects remain absent |
| SET an existing array element to MISSING | Writes JSON null; array length stays unchanged |
| UNSET a named member | Removes it; an absent member is a no-op |
| UNSET ending in an array index | Invalid; removal must end in a property name |

Root replacement is invalid. With an alias, `SET d[0].name = 'new'` can update an array-root
document. Duplicate paths, parent/child overlaps and SET/UNSET overlaps are rejected after alias
normalization, before reading documents.

Untouched JSON values and property names are copied byte for byte, preserving escapes, number
spellings and whitespace inside untouched values. Surviving members keep their positions; new
members append in target order. Whitespace around rewritten container delimiters and members can
be normalized. New values use JSON escaping. The patched document must fit storage's 128-level
depth; temporary expression constructors retain the query's 256-level allowance.

### Transactions and results

Content, versions and index entries change in one transaction. A failure rolls back the automatic
statement or aborts all work in the explicit transaction. An intervening committed writer can raise
DatabaseTransactionAbortedException. Older snapshots retain their view. Schema-owned collections
permit content updates; system collections are read-only.

Success returns the matched count, including matches whose content is unchanged or whose UNSET
removes nothing. Every matched document receives a higher version. Wire completion carries that
count with no Document frames when RETURNING is omitted.
[RETURNING](../clauses/returning.md) projects stored post-images and new versions in ordinal
document-ID order, with the matched count; projection failure rolls back the writes.
[EXPLAIN UPDATE](explain.md) returns an Update root and its
CollectionScan/IndexSeek child without writing.

Pending writes share [StatementMemoryBudget](../functions/aggregate-functions.md#statement-memory)
with SELECT staging and RETURNING. The complete pending buffer is charged before mutation;
exhaustion reports CD05018 (no assigned SQLSTATE) before any write. Reservations release on
success, failure and cancellation.

## Diagnostics

| Code | Meaning and trigger | SQLSTATE where supplied |
| --- | --- | --- |
| `CD00002` | Missing SET/UNSET, malformed path or assignment, qualified database target or leftover syntax | — |
| `CK00001` | USE KEYS, LIMIT, FOR or a nested SELECT | — |
| `CD05001` | Invalid metadata or aggregate context | — |
| `CD01002` | Collection does not exist | — |
| `CD04004` | Read-only system collection target | — |
| `CD01005` | Unbound parameter, including on empty input | — |
| `CD02016` | Invalid target, root replacement, UNSET ending in an index, incompatible traversal, missing array or out-of-range index | — |
| `CD01017` | Duplicate or prefix-overlapping targets after alias normalization | — |
| `CD02018` | Patched JSON is invalid or exceeds storage's 128-level depth | — |

Other expressions retain their [normal diagnostics](../diagnostics.md). A leaf-value depth failure
locates the failing target; whole-document validation uses the statement span.

## Examples

Execute each statement separately:

```sql
CREATE COLLECTION updates;
INSERT INTO updates (KEY, VALUE) VALUES
  ('a', {amount: 2, details: {score: 1}, tags: ['blue'], category: 'a'});
UPDATE updates d
SET d.amount = d.details.score, d.details.score = d.amount, d.tags[0] = MISSING
UNSET d.category, d.absent
WHERE META(d).id = 'a';
SELECT * FROM updates;
```

UPDATE affects one document. SELECT returns
`{"amount":1,"details":{"score":2},"tags":[null]}`. Both swapped values come from the original
document, and removing absent leaves no new member.

```sql
UPDATE updates SET extra.deep.value = 1;
UPDATE updates UNSET extra.deep.value;
UPDATE updates UNSET missing_array[0].name;
```

The first two statements each affect one document, leaving `extra` as `{"deep":{}}`.
The third fails with CD02016 and changes nothing because missing_array is absent.

Use [UPSERT](upsert.md) for whole-document replacement. Newly encoded SET values use the
shared [RFC 8259 UTF-8 encoder](select.md#json-bytes).

## See also

[Statements](index.md) · [DELETE](delete.md) · [WHERE](../clauses/where.md) · [META](../functions/meta.md) · [EXPLAIN](explain.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentUpdateTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/src/Internal/Json/DocumentJsonPatcher.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlDmlParserTests.cs`.
