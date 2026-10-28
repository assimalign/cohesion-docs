# CREATE INDEX

CREATE INDEX builds a named index over one to 32 ordered keys in a collection, including at most
one DISTINCT array projection.

## Syntax

```syntaxsql
CREATE [ UNIQUE ] INDEX [ IF NOT EXISTS ] index_name
ON collection_name ( index_key [ , ...n ] ) [ ; ]

index_key ::= document_path [ INCLUDE MISSING ]
    | DISTINCT ARRAY variable [ . element_path ] FOR variable IN document_path
        [ WHEN condition ] END
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
arrays < objects. Numbers 1 and 1.00 have the same key. Without INCLUDE MISSING, leading MISSING omits the entry; trailing
MISSING and explicit NULL have different component tags. Array/object tags in nonunique indexes
are lossy, so a seek never replaces predicate evaluation. Encoded keys must fit 1,024 bytes.

UNIQUE keys containing any NULL or MISSING never conflict. An indexed array/object component in a
UNIQUE key is refused. Constraints are immediate per row, so a multirow key swap may fail. Failure
rolls back the statement; an explicit transaction must be rolled back before the session is reused.

### INCLUDE MISSING

The suffix is allowed only on the leading scalar key, inside the parentheses:
`CREATE INDEX ix ON items (score INCLUDE MISSING, category)`. It includes documents whose
leading path is absent. Later keys and a suffix after the closing parenthesis use CD00002.
A leading array key with IncludeMissing in a direct AST/catalog request uses CD03025.
INCLUDE remains contextual. A later array key still emits no posting for an empty or filtered array.

COHESION_SCHEMA.INDEXES exposes INCLUDE_MISSING as a Boolean. PATH and PATHS[0] append
` INCLUDE MISSING` to the leading scalar key; other PATHS entries retain their key rendering.
For an index on `(v INCLUDE MISSING)`, PATH is `v INCLUDE MISSING`, PATHS is
`["v INCLUDE MISSING"]`, and INCLUDE_MISSING is true. The rendered list can be reused in DDL.
This uses the existing v3 flag and does not change the format. See
[EXPLAIN eligibility](explain.md#index-ordering-eligibility): execution still sorts on this branch.

### Array keys and WHEN

One key may be `DISTINCT ARRAY v.p FOR v IN arr [WHEN condition] END`; scalar keys may
precede or follow it. The source is a document path. The projection is an element-relative
path rooted in the FOR variable, or the variable itself. Computed sources/projections are refused.

WHEN accepts `=`, `!=`, `<>`, `<`, `<=`, `>` and `>=` between an element path and a NULL,
Boolean, Decimal or String literal, plus IS [NOT] NULL, IS [NOT] MISSING, AND, OR and NOT.
Comparisons may reverse their operands. Parameters, functions, IN and other expressions are
excluded. Text numbers bind exactly to Decimal; unsupported or out-of-range constants are
refused. WHEN is limited to 32 levels and 128 predicate nodes; IS NOT tests count as NOT plus
the test. All WHEN conditions apply to the same element; only TRUE retains it.

Each document version posts distinct qualifying scalar projections combined with its scalar
keys. Numeric scales deduplicate. NULL projections post; MISSING, array and object projections
do not. Empty/filtered arrays produce no postings. A missing source gives a MISSING component,
omitted when leading; a present non-array source gives NULL. Array indexes cannot be UNIQUE.
See the [.NET key API](../../../../dotnet-apis/resources/database/assimalign-cohesion-database-documents/document-index-key.md).

### Array access and fetch bounds

Top-level conjunctive ANY/SOME predicates can seek matching source/projection paths regardless
of variable spelling. Separate quantifiers keep separate witnesses. Direct document-path
[UNNEST](../clauses/unnest.md) can seek too, including LEFT UNNEST when a mandatory element
comparison rejects its synthetic MISSING row. Dependent sources, EVERY, NOT ANY, OR-only element
predicates and presence tests alone do not supply array seeks. A scalar prefix alone cannot
make an array index complete for empty/filtered arrays.

A filtered index is eligible only when the matching element predicate implies every WHEN
condition through matching normalized conjuncts. Reversed comparisons and negated presence
tests normalize; broader inequality inference does not occur. `x.stock > 1` does not establish
an index WHEN `v.stock > 0`. A separate ANY witness cannot establish another ANY's WHEN.

Candidates deduplicate by document identity before content fetch. The complete predicate is
always reapplied, rejecting cross-element false positives. UNNEST then expands the original
array: `[1,1,2]` still produces three rows.

For ungrouped, non-DISTINCT queries with fixed projection types, unordered pages or pages ordered
solely by META().id (also through a projection alias, ASC or DESC) stop after OFFSET + LIMIT
residual survivors. This bounds content fetches, not candidate metadata or probes. A single-field
predicate fully covered by the key, or a matching WHEN that covers the additional element
condition, avoids cross-element rejections. The tested fully indexed category/price pages fetch
at most OFFSET + LIMIT documents; the matching stock WHEN also meets the fetch bound for the
same-element price/stock shape. The tested half-qualifying residual shape stays within
`2 * (OFFSET + LIMIT)` fetches; arbitrary residuals have no such ratio. An unmatched multi-field
predicate remains correct but has no fetch bound: the unfiltered price index in the tests fetches
1,001 documents to return one same-element price/stock match; the matching stock WHEN fetches one.
Other orderings and projections needing type inference retain materialization.

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
| `CD00002` | Empty/duplicate paths, expressions, wildcards, ASC/DESC, partial WHERE, malformed INCLUDE MISSING, or the suffix on a later key/outside parentheses | — |
| `CD01028` | Projection or WHEN path is not rooted in its element variable | — |
| `CD02027` | WHEN constant is not a supported scalar literal, including a parameter or invalid Decimal | — |
| `CD03025` | Multiple array keys or a UNIQUE array index; typed IncludeMissing with a leading array key | — |
| `CD03027` | Source, projection or WHEN uses an excluded expression | — |
| `CD03028` | WHEN exceeds 32 levels or 128 predicate nodes | — |
| `CK00001` | ALL ARRAY, WITHIN or multiple comprehension bindings; UNIQUE in other reserved constructs | — |

Array-key refusals carry the offending source span and occur before catalog mutation.

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

### Array index example

Execute separately in a fresh database:

```sql
CREATE COLLECTION products;
INSERT INTO products (KEY, VALUE) VALUES
    ('cross', {variants:[{price:20,stock:0},{price:5,stock:3}]}),
    ('good', {variants:[{price:20,stock:3},{price:20,stock:3}]});
CREATE INDEX prices ON products
    (DISTINCT ARRAY v.price FOR v IN variants WHEN v.stock > 0 END);
SELECT META().id AS id FROM products
WHERE ANY x IN variants SATISFIES x.price > 10 AND 0 < x.stock END;
SELECT PATH, PATHS FROM COHESION_SCHEMA.INDEXES;
DROP INDEX prices ON products;
```

The first SELECT returns only `{"id":"good"}`. The metadata row has PATH
`DISTINCT ARRAY v.price FOR v IN "variants" WHEN v.stock > 0 END` and PATHS containing that
one string. PATHS renders every key in order; PATH renders the leading key. Array rendering
uses canonical variable v and quotes the source root and path segments that need quoting.
DROP removes scalar and array indexes with the same syntax and returns zero affected documents.

## See also

[Statements](index.md) · [DROP INDEX](drop-index.md) · [Paths and arrays](../expressions/paths-and-arrays.md) · [Diagnostics](../diagnostics.md)

## Explicit transactions

OQL index DDL inside an explicit transaction reports CD04105 before mutation, including
conditional no-ops, and faults the transaction. Run it in an automatic statement transaction;
[ROLLBACK](transactions.md) recovers a faulted session.

## Sources

- **Array DDL, residuals and fetch counts** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentQueryTests.ArrayIndexes.cs`.
- **Composite execution** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentQueryTests.CompositeIndexes.cs`.
- **Grammar** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/OqlQueryParser.Ddl.cs`.
- **INCLUDE MISSING and rendering** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentOrderedAccessTests.cs`.
- **Index behavior** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/docs/DESIGN.md`.
- **Examples** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlIndexDdlParserTests.cs`.
