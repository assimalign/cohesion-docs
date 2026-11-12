# SELECT

SELECT projects document expressions from a collection, derived query or expression and ordered key/array bindings.

## Syntax

```syntaxsql
SELECT [ ALL | DISTINCT ] { projection [ , ...n ] | { VALUE | RAW } expression }
FROM { collection_name [ [ AS ] iteration_variable ]
     | ( SELECT ... ) [ AS ] alias | expression AS alias }
[ source_binding ...n ]
[ WHERE predicate ]
[ GROUP BY expression [ [ AS ] alias ] [ , ...n ] [ GROUP AS group_name ] ]
[ HAVING predicate ]
[ WINDOW name AS ( window_specification ) [ , ...n ] ]
[ ORDER BY expression [ ASC | DESC ] [ , ...n ] ]
[ LIMIT count [ OFFSET count ] | OFFSET count [ LIMIT count ] ]
[ ; ]

<projection> ::= { * | alias.* | expression } [ AS output_name ]
<source_binding> ::= { [ INNER ] UNNEST | LEFT [ OUTER ] UNNEST } expression [ [ AS ] variable ]
                  | [ INNER | LEFT [ OUTER ] ] { JOIN | NEST } collection_name [ [ AS ] alias ]
                    ON META(alias).id = key_expression
```

## Supported clauses

| Clause | Support | Requirement |
|---|---|---|
| `FROM` | Supported | Required initial source; optional ordered JOIN/NEST/UNNEST bindings |
| [JOIN and NEST](../clauses/join.md) | Supported | Stored right-hand collection; META(b).id = key_expression |
| `WHERE` | Supported | Optional document predicate |
| `GROUP BY` | Supported | Optional grouping expressions |
| [GROUP AS and group-term aliases](../clauses/group-by.md#group-as-and-aliases) | Supported | Named keys and post-group arrays of input binding wrappers |
| `HAVING` | Supported | Optional group predicate |
| [OVER and WINDOW](#windows) | Supported subset | Eight ranking/navigation functions; complete partitions before projection |
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
member-name wrapper. They require exactly one expression without an alias or unqualified star (`CD00002` in
text; `CD05001` for an invalid direct AST). The single metadata column is named `value`.
A top-level MISSING becomes NULL before DISTINCT, ordering by that projection, and completion
counting, in process, over InMemory and over TCP. Each source row still contributes one row before
deduplication. Named projections retain their MISSING cells.

Unless LIMIT is zero, evaluation proceeds through UNNEST expansion, WHERE, GROUP BY/HAVING,
windows, projection, DISTINCT, ORDER BY, then [LIMIT/OFFSET](../clauses/limit-offset.md). DISTINCT
compares complete projected rows using structural equality: numeric 2 and 2.0 compare equal,
arrays compare by ordered elements, and objects by ordinal keys and their values independently of
member order. It retains the first representative. Named MISSING and NULL remain distinct;
VALUE/RAW has already normalized them to NULL. Under DISTINCT every ORDER BY key must match a
projected expression after alias rewriting, otherwise `CD01014` is reported at the ordering key.
[Aggregate ALL/DISTINCT](../functions/aggregate-functions.md) instead controls inputs to one
aggregate call, such as COUNT(DISTINCT x), independently of SELECT-level deduplication.

Results have the same shape on every transport. A lone star over a stored collection without UNNEST/JOIN/NEST returns the stored JSON bytes unchanged,
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

Retained SELECT source rows, group keys/membership, accumulators, DISTINCT sets, ARRAY_AGG
inputs/results and projection/order storage share the positive StatementMemoryBudget (64 MiB
default). Exhaustion reports CD05018 before publishing any result. INSERT/UPSERT SELECT sources
and RETURNING use the same counter. See [statement memory](../functions/aggregate-functions.md#statement-memory)
for reservation lifetimes and exclusions.

Underlying index reads copy bounded B+Tree batches and pass cancellation at cursor open and
every move, including latch waits and page/entry reads. Each batch releases its tree latch before
document decoding. This bounds index-range buffering. Eligible
[array-index pages](create-index.md#array-access-and-fetch-bounds) stop content reads after
OFFSET + LIMIT residual survivors in identity order; SELECT still owns its materialized result.

With [UNNEST](../clauses/unnest.md), star returns a synthetic object keyed by the source and array
binding names, omitting MISSING bindings and keeping NULL elements. WHERE/grouping/aggregates and
paging consume expanded rows. A star over a stored collection without UNNEST/JOIN/NEST still preserves the stored bytes.

[Key JOIN and NEST](../clauses/join.md) bind documents or arrays by scalar key. Joined SELECT *
constructs an alias-keyed object; top-level qualified stars such as b.* return a binding value in
ungrouped joined projections. Collections supply implicit aliases when omitted, and inferred
UNNEST names are valid too. META(alias) retains each stored source's own identity. See the linked page for unmatched
MISSING values, NEST arrays, paging and the bounded lookup cache.

## Windows

OQL implements ROW_NUMBER, RANK, DENSE_RANK, PERCENT_RANK, CUME_DIST, NTILE, LAG
and LEAD with OVER. Use OVER (PARTITION BY ... ORDER BY ...), OVER name or a parenthesized
copy OVER (name ...). WINDOW declarations follow GROUP BY/GROUP AS/HAVING and precede final
ORDER BY/LIMIT/OFFSET. LETTING remains CK00001.

Names are ordinal and case-sensitive. Definitions can copy only earlier definitions; unknown,
self/forward references use CD01042. Empty/duplicate names use CD01041. A copy cannot supply
PARTITION BY, even when the base has none, or replace an existing ORDER BY (CD01043).
Window keys/arguments do not see projection aliases, even in final ORDER BY. Independent
subqueries, including a MERGE source, own their own window scope.

Only SELECT projections and SELECT ORDER BY admit windows at their query level. WHERE, GROUP
BY, HAVING, aggregate arguments, SATISFIES/comprehension bodies, DML actions/values and RETURNING
use CD01044, as does a window-only call without OVER. A compound's result-only ORDER BY also
refuses CD01044; wrap it in SELECT. A window in another window's arguments or keys, including
named keys, uses CD01045. Group expressions and ordinary aggregate results can supply inputs.

Partition equality matches GROUP BY: MISSING and NULL are distinct, numerics compare by value,
strings use ordinal collation, arrays/objects use existing structural order. ORDER BY defaults
to ASC, NULLS FIRST for ASC and NULLS LAST for DESC. Equal keys form peers; without ordering
the partition is one peer group. Peer and unordered navigation/ROW_NUMBER order is unspecified;
execution uses stable stage-input order. Final ORDER BY controls result order independently.

| Function | Result and defaults |
| --- | --- |
| ROW_NUMBER() | One-based Int64 row number |
| RANK(), DENSE_RANK() | One-based Int64; RANK leaves peer gaps, DENSE_RANK counts peer groups |
| PERCENT_RANK() | Decimal (rank−1)/(partition rows−1), zero for a singleton |
| CUME_DIST() | Decimal last-peer position / partition rows |
| NTILE(buckets) | Int64 bucket; larger buckets first, trailing buckets empty when buckets exceed rows |
| LAG(value [, offset [, default]]), LEAD(...) | Offset 1, fallback NULL; respect both NULL and MISSING; no SQL common-type coercion over JSON values |

NTILE accepts a positive Int64 integer literal; navigation offsets are nonnegative Int64 integer
literals, optionally unary plus. Zero reads the current row. Parameters, noninteger, NULL/MISSING,
negative and row-dependent counts/offsets retain CK00001, even on empty input. LAG/LEAD evaluate
value only at selected target rows; out-of-partition targets never evaluate value and use the
current row's explicit default. Constant offsets visit each target at most once per call.
An unaliased window projection uses the lowercase function name.

Every partition sees complete input; paging/index-order shortcuts cannot truncate it. LIMIT 0
still validates eagerly and skips execution. Equal specifications share sort/key evaluation;
independent specifications evaluate independently. Rows, results, indexes, keys and navigation
values charge StatementMemoryBudget (CD05018), with no spill. Work, including comparisons,
shares StatementWorkBudget (CD05020). Adding OVER selects nested execution for the entire
statement, so scan predicates/projections also consume work. Reservations release on every exit.
EXPLAIN includes WindowAggregate, partition/order keys and sort-sharing groups without input reads.

### OVER names and reservations

After an ordinary call's closing parenthesis, OVER starts a window before `(` or a name token
that cannot start a clause after an implicit alias; otherwise base implicit-alias rules apply.
After any of the twelve window-only function names below, OVER is always the keyword. Invalid
or missing targets use CD00002, asking for a name/specification and quoted clause-word names.
After OVER, UNNEST, NEST, JOIN, LEFT, INNER, UNION, INTERSECT, EXCEPT, RETURNING, LETTING and
WINDOW must be quoted, as must reserved clause words such as ORDER. In a parenthesized copy,
quote rows, range, groups, exclude and partition, for example OVER ("rows" ORDER BY x).
WINDOW name AS begins a declaration, including a function-name token as name. Window words
remain usable as fields/collections/aliases in window-free statements.

DocumentFunctionCollection.Add now refuses all twelve names with ArgumentException for function,
before engine build: ROW_NUMBER, RANK, DENSE_RANK, PERCENT_RANK, CUME_DIST, NTILE, LAG, LEAD,
FIRST_VALUE, LAST_VALUE, NTH_VALUE and RATIO_TO_REPORT. Earlier releases accepted these registrations.

Frames (ROWS/RANGE/GROUPS/EXCLUDE), aggregate OVER, FIRST_VALUE/LAST_VALUE/NTH_VALUE,
RATIO_TO_REPORT and RESPECT/IGNORE NULLS are not implemented yet and refuse CK00001.
No implicit frame is evaluated. A scalar such as LOWER(x) OVER () also retains CK00001.

```sql
SELECT x.n, ROW_NUMBER() OVER w AS rn, LAG(x.n) OVER w AS previous
FROM [{n:2}, {n:1}] AS x
WINDOW w AS (ORDER BY x.n)
ORDER BY x.n;
```

The ordered rows have rn 1/2; previous is NULL/1.

## Set operations

```syntaxsql
query_term { { UNION | INTERSECT | EXCEPT } [ ALL ] query_term } ...
[ ORDER BY expression [ ASC | DESC ] [ NULLS FIRST | NULLS LAST ] [ , ...n ] ]
[ LIMIT count [ OFFSET count ] | OFFSET count [ LIMIT count ] ]

query_term ::= SELECT ... | ( query )
```

UNION, INTERSECT and EXCEPT execute in every query position, with and without ALL.
UNION removes structural duplicates, retaining the first
representative; UNION ALL preserves every occurrence. Operands need not have equal arity. Operators
have equal, left-associative precedence, following Couchbase rather than ISO SQL. Use parentheses
to change grouping. Explicit DISTINCT after any set operator reports CK00001.

The final ORDER BY/LIMIT/OFFSET belongs to the compound. An operand's own ordering or paging
requires parentheses; an unparenthesized local tail followed by another operator is CD00002.
Query parentheses and operator paths each have a 128-level bound (CD00005), including compounds
inside SELECT subqueries: a leaf contributes its deepest nested compound's depth. An extra outer tail
on a lone parenthesized query that already has its own tail remains CK00001.

UNION uses SELECT DISTINCT equality: numbers compare by value, object member order does not
matter and array order does. Named MISSING members are omitted; RAW/VALUE MISSING becomes NULL.
Without ordering, left rows precede right rows; UNION keeps only previously unseen values.

For a value occurring m times on the left and n times on the right, INTERSECT ALL retains
min(m,n) occurrences and EXCEPT ALL retains max(m-n,0). The distinct forms keep the first
qualifying left representative in left order. EXCEPT ALL consumes right counts against successive
left occurrences and retains later unmatched occurrences in left order. INTERSECT/EXCEPT read
the right operand first, retaining equality keys and counts rather than every right row. Empty
inputs do not skip the other operand's evaluation, except with compound LIMIT 0.

| Operand projections | Planned result header |
| --- | --- |
| Every leaf RAW/VALUE | One `value` column |
| Same ordered named projections in every leaf | Columns with those names; compatible Int64/Decimal types widen to Decimal |
| Different names/order/arity, mixed RAW/named or wildcard documents | One `document` Json column in whole-document mode |

The shape is fixed before execution, including empty pages. Heterogeneous typed columns use the
Null descriptor. Dynamic named/value columns inspect the pre-page stream to determine types;
LIMIT 0 uses static descriptors without opening sources or invoking callbacks.

Compound ORDER BY sees result members declared by any operand; absent members evaluate to MISSING.
An enclosing alias cannot shadow a result member or fill in an absent member.
Collection aliases are out of scope; wildcard documents leave member names open. Scalar expressions
and ordinary NULL placement work; ties retain concatenation order. All-RAW/VALUE ordering reports
CD01037. Unbound members, META and star report CD01038. Subqueries in compound ordering remain
CK00001.

UNION ALL streams operand rows, while each operand retains its own blocking stages. Distinct keys,
right-side multiset keys/counters, compound sorting and kept output share StatementMemoryBudget (CD05018); row processing, equality
and sorting share StatementWorkBudget (CD05020). Fixed-type unordered pages stop when complete.
EXPLAIN describes Union/UnionAll, Intersect/IntersectAll or Except/ExceptAll and both operands
without executing them. Compounds execute in expression subqueries as arrays, derived FROM,
INSERT/UPSERT SELECT and correlated operands, including a parenthesized first term. Each operand
has its own lexical scope; caching requires every operand to be uncorrelated and cacheable.
Whole-document cells become array elements or derived rows directly, without another wrapper.
Write sources bind KEY/VALUE against static columns or declared result members; heterogeneous
rows resolve members per row, and omitted VALUE keeps the complete compound row value. All source
rows are staged and validated before mutation. See [IN grammar](../expressions/subqueries.md).

Execute this standalone expression-source query:

```sql
(SELECT x AS n FROM [3,1,2] AS x ORDER BY x LIMIT 2)
UNION ALL
(SELECT y AS n FROM [5,4] AS y ORDER BY y DESC LIMIT 1)
ORDER BY n DESC OFFSET 1 LIMIT 2;
```

It returns named rows with n = 2, then n = 1.

The first standalone query returns bare rows 1 and 2; the second returns one bare row, 1:

```sql
SELECT RAW x FROM [1,1,2,3] AS x INTERSECT ALL SELECT RAW y FROM [1,2,2] AS y;
SELECT RAW x FROM [1,1,2,3] AS x EXCEPT ALL SELECT RAW y FROM [1,2,3] AS y;
```

## Derived and expression sources

[FROM](../clauses/from.md) accepts an aliased SELECT or expression. Arrays expand in
order, objects/scalars/NULL supply one row, and MISSING supplies none. Their lone stars produce
alias-keyed objects; META returns NULL. Simple queries stream through paging; LIMIT 0 skips
execution after static validation, including sorting and grouping. These sources compose with
UNNEST and nested SELECTs. Stored collection byte-preservation rules above apply to stored sources.

## Expression subqueries

[(SELECT ...)](../expressions/subqueries.md) returns an array wherever a SELECT
expression is allowed. [EXISTS](../expressions/exists.md) tests for a nonempty array. Nested
queries share this statement's snapshot and work/memory budgets. Correlation and DML
subqueries execute; see the linked reference for capture scope and statement-start reads. Nested star results wrap nonobject roots under `document`;
a top-level lone star over a stored collection keeps the stored-value rule above.

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

- **Ranking/navigation windows** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/docs/DIALECT.md`, `tests/OqlWindowParserTests.cs` in that project, and `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentWindowTests.cs` at `2dd935103`.
- **Compound multiset/query positions** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentSetOperationTests.cs`, `DocumentCompoundRegressionTests.cs` and `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/docs/DIALECT.md`.
- **Compounds, headers and limits** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentSetOperationTests.cs` and `DocumentCompoundIntegrationTests.cs` in the same directory.
- **Compound grammar and IN wrappers** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlSetOperationParserTests.cs`.

- **Projection and transport results** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentQueryTests.Projections.cs` and `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Client/tests/DocumentProjectionWireTests.cs`.
- **Grammar** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/OqlQueryParser.Select.cs`.
- **Execution semantics** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/docs/DESIGN.md`.
- **Examples** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlQueryParserTests.cs`.
