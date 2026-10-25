# SELECT

SELECT projects document expressions from one collection with optional filtering, grouping, and ordering.

## Syntax

```syntaxsql
SELECT [ ALL | DISTINCT ] { projection [ , ...n ] | { VALUE | RAW } expression }
FROM collection_name [ [ AS ] iteration_variable ]
[ { [ INNER ] UNNEST | LEFT [ OUTER ] UNNEST } expression [ [ AS ] variable ] ...n ]
[ WHERE predicate ]
[ GROUP BY expression [ , ...n ] ]
[ HAVING predicate ]
[ ORDER BY expression [ ASC | DESC ] [ , ...n ] ]
[ LIMIT count [ OFFSET count ] | OFFSET count [ LIMIT count ] ]
[ ; ]

<projection> ::= { * | expression } [ AS output_name ]
```

## Supported clauses

| Clause | Support | Requirement |
|---|---|---|
| `FROM` | Supported | Required; exactly one source |
| `WHERE` | Supported | Optional document predicate |
| `GROUP BY` | Supported | Optional grouping expressions |
| `HAVING` | Supported | Optional group predicate |
| `ORDER BY` | Supported | Optional ordering expressions |
| [LIMIT/OFFSET](../clauses/limit-offset.md) | Supported | Nonnegative Int64 integer literals or parameters; either clause order |
| `ALL` / `DISTINCT` | Supported | ALL is the default; DISTINCT removes duplicate projected values |
| `VALUE` / `RAW` | Supported | One bare JSON value per row; RAW is a synonym |

## Arguments

- **`projection`** — a path, literal, parameter, arithmetic expression, aggregate, or whole-document
  star. `AS` gives an explicit output name.
- **`collection_name`** — an unqualified collection or reserved `COHESION_SCHEMA` source in the
  current database.
- **`iteration_variable`** — an optional name used to qualify document paths.

## Remarks

`SELECT *` returns a `document` column containing the complete JSON value. Objects and arrays remain
`JsonElement` values; scalars become null, Boolean, decimal, or string. Mixed-type result columns use
`DatabaseType.Null` to indicate an unknown type. Duplicate output names report `CD01003`; resolve them with aliases. Projected MISSING members
are omitted from JSON objects; explicit NULL members remain. In-process rows distinguish the two
with SupportsMissing/IsMissing; the wire sends one JSON Document per projected row.

`SELECT VALUE expression` and `SELECT RAW expression` return the expression itself, without a
member-name wrapper. They require exactly one expression without an alias or star (`CD00002` in
text; `CD05001` for an invalid direct AST). The single metadata column is named `value`.
A top-level MISSING becomes NULL before DISTINCT, ordering by that projection, and completion
counting, in process, over InMemory and over TCP. Each source row still contributes one row before
deduplication. Named projections retain their MISSING cells.

Evaluation proceeds through UNNEST expansion, WHERE, GROUP BY/HAVING, projection, DISTINCT, ORDER BY, then [LIMIT/OFFSET](../clauses/limit-offset.md). DISTINCT
compares complete projected rows using structural equality: numeric 2 and 2.0 compare equal,
arrays compare by ordered elements, and objects by ordinal keys and their values independently of
member order. It retains the first representative. Named MISSING and NULL remain distinct;
VALUE/RAW has already normalized them to NULL. Under DISTINCT every ORDER BY key must match a
projected expression after alias rewriting, otherwise `CD01014` is reported at the ordering key.
DISTINCT inside an aggregate, such as `COUNT(DISTINCT x)`, remains unsupported (`CK00001`).

Results have the same shape on every transport. A lone star without UNNEST returns the stored JSON bytes unchanged,
including whitespace and scalar roots. Named projections form one object per row; a star mixed
with other projections is a named `document` member. VALUE/RAW emits one bare JSON value. The client
sets IsDocumentQuery for lone star and VALUE/RAW even on an empty result. See
[DocumentResultSet](../../../../dotnet-apis/resources/database/assimalign-cohesion-database-documents-client/document-result-set.md).

VALUE and RAW are reserved: quote a field as `"value"` or `"raw"`, or use `d.value`/`d.raw`.
Bare reserved fields in these positions report `CD00002`. After AS, projection labels may use
reserved words. [Object](../expressions/object-constructors.md) and
[array constructors](../expressions/array-constructors.md) can shape named or bare projections.

Scans and index seeks establish ordinal document-identifier order before filtering. `ORDER BY`
preserves established order for ties. Results are materialized and own their JSON values after
execution, without retaining the transaction or borrowed storage memory.

Underlying index reads copy bounded B+Tree batches and pass cancellation at cursor open and
every move, including latch waits and page/entry reads. Each batch releases its tree latch before
document decoding. This bounds index-range buffering; SELECT still materializes its result.

With [UNNEST](../clauses/unnest.md), star returns a synthetic object keyed by the source and array
binding names, omitting MISSING bindings and keeping NULL elements. WHERE/grouping/aggregates and
paging consume expanded rows. Ordinary star without UNNEST still preserves the stored bytes.

## JSON bytes

Generated document JSON uses one RFC 8259 encoder for INSERT/UPSERT storage, constructors,
named projections and bare VALUE/RAW byte rows in process, over InMemory and over TCP.
Unicode, including supplementary characters, remains literal UTF-8; `<`, `>`, `&`, U+2028
and U+2029 remain literal too. Quotes, backslashes and U+0000–U+001F are escaped. For example,
`SELECT VALUE 'é東京<&😀' FROM items;` over one source row returns the UTF-8 JSON bytes
`"é東京<&😀"`. These bytes differ from the previous escaped serializer output.
Lone-star rows preserve the original stored JSON bytes.

Object members still omit MISSING; array elements and a bare whole MISSING become JSON null.
This encoding changes bytes without changing NULL/MISSING semantics or the wire payload layout.

## Examples

These statements are in the parser conformance corpus:

```sql
SELECT * FROM people
```

```sql
SELECT country, COUNT(*) AS total FROM people GROUP BY country HAVING COUNT(*) > 1 ORDER BY total DESC
```

[EXPLAIN](explain.md) describes a SELECT without executing it; [INSERT SELECT](insert.md) uses
its projected names to select document keys and values.

### DISTINCT and bare values

Execute each statement separately:

```sql
CREATE COLLECTION items;
INSERT INTO items (KEY, VALUE) VALUES
  ('a', {name: 'alpha', category: 'a', x: NULL}),
  ('b', {name: 'beta', category: 'a'}),
  ('c', {name: 'gamma', category: 'b'});
SELECT DISTINCT category FROM items ORDER BY category;
SELECT DISTINCT VALUE x FROM items ORDER BY x;
SELECT RAW name FROM items WHERE category = 'b';
SELECT VALUE {n: name, t: [category, {x: NULL}, MISSING]}
FROM items WHERE name = 'alpha';
```

Results are `{"category":"a"}`, `{"category":"b"}`; one bare `null`; bare `"gamma"`;
and `{"n":"alpha","t":["a",{"x":null},null]}`, respectively. Without DISTINCT, VALUE x
returns three NULL rows. `SELECT DISTINCT category FROM items ORDER BY name` fails `CD01014`.

## See also

[Statements](index.md), [FROM](../clauses/from.md), [Expressions](../expressions/index.md),
and [Aggregate functions](../functions/aggregate-functions.md).

## Probe costs and indexed intersection

Bounded snapshot-visible index probes choose among eligible accesses; equal costs retain equality-prefix, range, UNIQUE and ordinal-name ties. Comparable complete indexed predicates may intersect ordered document identities before JSON reads, then apply the full predicate. Incomplete inputs keep a single driver. No persisted statistics or ANALYZE command exists.

EXPLAIN can read index metadata for costing but executes no document statement. IndexSeek estimates identify exact visible index-entry counts or lower bounds; IndexIntersection reports the minimum input count as an upper bound. actual_rows stays NULL. See [access design](../../../../dotnet-apis/resources/database/assimalign-cohesion-database-documents/design.md#bounded-access-probes-and-intersection).

## Sources

- **Projection and transport results** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentQueryTests.Projections.cs` and `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Client/tests/DocumentProjectionWireTests.cs`.
- **Grammar** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/OqlQueryParser.Select.cs`.
- **Execution semantics** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/docs/DESIGN.md`.
- **Examples** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlQueryParserTests.cs`.
