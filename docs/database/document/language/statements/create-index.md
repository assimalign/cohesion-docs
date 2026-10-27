# CREATE INDEX

CREATE INDEX builds a named index over one to 32 ordered document paths in a collection.

## Syntax

```syntaxsql
CREATE [ UNIQUE ] INDEX [ IF NOT EXISTS ] index_name
ON collection_name ( document_path [ , ...n ] ) [ ; ]
```

## Arguments

- **index_name** — the index identifier, preserving case.
- **collection_name** — a collection in the session database.
- **document_path** — a distinct path relative to each document. Nested members, nonnegative
  Int32 array subscripts and bracket-string names, including a root `['x.y']`, are supported.
- **UNIQUE** — requires distinct valued scalar key tuples; a Cohesion extension in SQL spelling.
- **IF NOT EXISTS** — preserves an existing same-named definition regardless of its paths or UNIQUE flag.

## Remarks

Creation populates existing documents in the statement transaction. INSERT, UPSERT, UPDATE, DELETE
and collection CRUD maintain every index atomically. The database writer lock serializes writes;
schema-owned collections reject index creation and system collections are read-only. Successful
DDL returns Success and zero affected documents. Snapshot conflicts are checked before no-ops.

Composite access uses a leading equality prefix and at most one range on the next path. A
predicate only on a non-leading path scans. Bounded probes rank access costs first; ties use the longest equality prefix, then a
range, then UNIQUE, then ordinal index name. Comparable complete indexed inputs may intersect
identities before JSON reads. The full predicate always filters seek candidates.

Keys collate as Couchbase does: MISSING < NULL < FALSE < TRUE < numbers < ordinal strings <
arrays < objects. Numbers 1 and 1.00 have the same key. Leading MISSING omits the entry; trailing
MISSING and explicit NULL have different component tags. Array/object tags in nonunique indexes
are lossy, so a seek never replaces predicate evaluation. Encoded keys must fit 1,024 bytes.

UNIQUE keys containing any NULL or MISSING never conflict. An indexed array/object component in a
UNIQUE key is refused. Constraints are immediate per row, so a multirow key swap may fail. Failure
rolls back the statement; an explicit transaction must be rolled back before the session is reused.

The [.NET catalog API](../../../../dotnet-apis/resources/database/assimalign-cohesion-database-documents/document-index-key.md)
supports DISTINCT array-element postings in this step. OQL ALL/DISTINCT ARRAY keys remain
CK00001; OQL array-index DDL arrives in batch 6.

All indexes use Documents.Catalog v3 definition and physical-registration records. V1/v2 indexes are
refused at open before recovery writes, with found/supported versions and a recreate remedy.
Recreate the database with this engine; there is no upgrade path or legacy reader.

## Diagnostics

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| `CD03024` | Existing index name without IF NOT EXISTS | — |
| `CD03026` | Encoded index definition exceeds 8,092 bytes; before posting extraction/tree creation | — |
| `CD02026` | Composite encoded key exceeds 1,024 bytes during build or mutation | — |
| `CD03020` | Duplicate valued scalar UNIQUE key during a write or build; names index and collection without key data | 23505 |
| `CD02021` | Array/object component in a UNIQUE key; names index and path | — |
| `CD00009` | More than 32 paths; located at the 33rd path | — |
| `CD03022` | Direct AST/catalog request has fewer than one or more than 32 paths | — |
| `CD00002` | Empty/duplicate paths, expressions, wildcards, ASC/DESC, partial WHERE or INCLUDE MISSING | — |
| `CK00001` | ALL/DISTINCT ARRAY index keys; UNIQUE in other reserved constructs remains unsupported | — |

CD03020/CD02021 are carried by
[DocumentConstraintViolationException](../../../../dotnet-apis/resources/database/assimalign-cohesion-database-documents/document-constraint-violation-exception.md).
Wire failures use ExecutionFailure (6) and preserve the connection.
CD03024/CD02026 use DatabaseException.Diagnostics with the original DocumentCatalogException;
an explicit transaction is aborted until rollback. No SQLSTATE is assigned to either code.

## Examples

Execute separately:

```sql
CREATE COLLECTION items;
CREATE UNIQUE INDEX uq ON items (a, b);
INSERT INTO items (KEY, VALUE) VALUES ('one', {a: 1, b: 2.0});
INSERT INTO items (KEY, VALUE) VALUES ('two', {a: 1.00, b: 2});
```

The second INSERT fails CD03020 (23505). Only one remains. Two documents with
`{a:1,b:NULL}` can coexist, as can two with `{a:1}`. On this collection:

```sql
CREATE UNIQUE INDEX IF NOT EXISTS uq ON items (other);
SELECT PATH, PATHS, IS_UNIQUE FROM COHESION_SCHEMA.INDEXES;
```

Creation is a no-op; the query returns `{"PATH":"a","PATHS":["a","b"],"IS_UNIQUE":true}`.
`SELECT * FROM items WHERE a = 1 AND b >= 2` can seek uq and returns the one document.

After the example's `uq` exists, `CREATE INDEX uq ON items (other);` fails CD03024;
`CREATE INDEX IF NOT EXISTS uq ON items (other);` remains a no-op. Indexing a document whose
encoded composite key exceeds 1,024 bytes instead fails CD02026 and publishes no index.

## See also

[Statements](index.md) · [DROP INDEX](drop-index.md) · [Paths and arrays](../expressions/paths-and-arrays.md) · [Diagnostics](../diagnostics.md)

## Explicit transactions

OQL index DDL inside an explicit transaction reports CD04105 before mutation, including
conditional no-ops, and faults the transaction. Run it in an automatic statement transaction;
[ROLLBACK](transactions.md) recovers a faulted session.

## Sources

- **Composite execution** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentQueryTests.CompositeIndexes.cs`.
- **Grammar** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/OqlQueryParser.Ddl.cs`.
- **Index behavior** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/docs/DESIGN.md`.
- **Examples** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlIndexDdlParserTests.cs`.
