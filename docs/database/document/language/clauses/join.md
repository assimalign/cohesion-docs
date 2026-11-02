# JOIN and NEST

Looks up one stored document by key for each preceding input row.

> **Status:** Implemented. Key joins and NEST complete the OQL T2 lane.

## Syntax

```syntaxsql
FROM source AS a
{ [ INNER | LEFT [ OUTER ] ] { JOIN | NEST } collection [ AS ] b
  ON META(b).id = key_expression } [ ...n ]
```

## Arguments

- **`source`** — A stored/system collection, derived SELECT or expression source.
- **`collection`** — A stored collection in the session database.
- **`b`** — Required explicit alias for the right-hand source.
- **`key_expression`** — A string, NULL or MISSING evaluated over preceding bindings and outer captures.

## Remarks

INNER is the default. LEFT OUTER JOIN and LEFT JOIN are equivalent; NEST also accepts INNER and
LEFT OUTER. Bindings execute left to right and may interleave with [UNNEST](unnest.md).
ON must be exactly `META(b).id = key_expression`, with optional parentheses. Reversed equality,
general predicates, ON KEYS, comma joins and derived/expression right-hand sources remain CK00001.
UPDATE/DELETE cannot introduce joins; INSERT/UPSERT SELECT and expression subqueries can use them.

Unqualified key fields address the initial FROM value. Keys may use functions, parameters,
subqueries and earlier aliases. A self/forward reference uses CD01035; qualify an initial field
when its name collides with a self or later alias. Key comparison is ordinal. Arrays and other
non-string, non-NULL/MISSING values use CD02028; this subset does not accept key lists.

Every FROM and UNNEST source in **every SELECT scope of a statement containing JOIN or NEST**
requires an explicit alias, including nested scopes without joins. Inferred path aliases do not
satisfy this rule. AS is optional for collection and UNNEST aliases; expression FROM keeps its
existing AS requirement. Text uses CD00016; an invalid direct AST uses CD01035. DML target alias
rules are unchanged. This restriction tracks
[assimalign/cohesion#1767](https://github.com/assimalign/cohesion/issues/1767).
Aliases are ordinal and unique within their SELECT.

### Unmatched rows and metadata

INNER JOIN drops an input with no match; LEFT JOIN preserves it with a MISSING right binding.
A found document whose content is NULL is still a match and retains its stored metadata.
NEST binds an array of matched document values. A scalar key can match at most one document:

| Key | INNER NEST | LEFT NEST |
| --- | --- | --- |
| Existing string | One row with a one-element array | One row with a one-element array |
| Absent string | No row | One row with `[]` |
| NULL or MISSING | No row | One row with a MISSING binding |

[META(alias).id/version](../functions/meta.md) reads each stored source's own identity, independently
of JSON fields. Unmatched JOIN metadata is MISSING. Derived/expression sources and NEST arrays
have no single document identity, so META returns NULL; an unknown-key LEFT NEST binding has
MISSING metadata. META() is ambiguous in a joined SELECT (CD01035). UNNEST, quantifier and
comprehension variables are invalid META receivers, including names shadowing captured sources.

### Projection and evaluation

SELECT * constructs an object keyed by the explicit FROM alias and each JOIN/NEST/UNNEST alias.
MISSING members are omitted; NULL members remain. A top-level qualified star, such as `b.*`,
returns that binding's value in an ungrouped joined SELECT. As the sole projection it uses the
bare document result mode. Qualified stars outside joined queries retain their existing refusal.

Grouping, GROUP AS, HAVING, DISTINCT, ordering, paging and correlated subqueries compose with
joins. Equal sort keys retain source order. Nonblocking joins stop after the selected paging
window, before projection; dynamic headers describe that window. LIMIT 0 and EXPLAIN perform
static validation without document lookups.

### Lookup cache and limits

All lookups share the current operation snapshot and its own transaction writes. A statement
caches hits and misses by collection identity and ordinal key across aliases, chains and
subqueries. The least-recently-used cache is bounded to one eighth of StatementMemoryBudget:
8 MiB within the default 64 MiB grant. Eviction releases the entry and its NEST array; oversized
entries bypass caching. An evicted key is looked up again in the same snapshot.

Cache keys, entries, content and arrays share the statement memory grant (CD05018). Current
cursor rows and rows retained for sorting/grouping keep independent reservations. Lookup attempts,
cache hits and NEST array creation share StatementWorkBudget (CD05020). Completion, failure and
cancellation release reservations. There is no new wire or disk format.

EXPLAIN shows KeyJoin, LeftKeyJoin, KeyNest or LeftKeyNest with collection, alias, snapshot and
cache policy. Joined ordering retains a sort; index candidates report `key_join_requires_sort`.

## Diagnostics

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CD00016 | SELECT source without an explicit alias in text containing JOIN/NEST | — |
| CD01035 | Unbound/self/forward source, missing direct-AST alias, ambiguous/invalid joined META receiver | — |
| CD02028 | Key is neither string, NULL nor MISSING; arrays are unsupported | — |
| CD00010 | Duplicate parsed binding name | — |
| CD01023 | Duplicate/empty direct-AST binding name | — |
| CD05018 | Shared retained statement memory exhausted | — |
| CD05020 | Shared expression/lookup work exhausted, including cache hits | — |
| CK00001 | Unsupported join predicate, right-hand source or mutation join | — |

The registry assigns no SQLSTATE to these conditions.

## Examples

Execute each statement separately in a fresh database:

```sql
CREATE COLLECTION items;
INSERT INTO items (KEY, VALUE) VALUES ('a', {n:1}), ('b', {n:2}), ('c', {n:3});
SELECT META(a).id AS l, META(b).id AS r
FROM items AS a JOIN items AS b ON META(b).id = META(a).id ORDER BY l;
SELECT * FROM [1] AS a LEFT JOIN items AS b ON META(b).id = 'absent';
SELECT RAW b.* FROM items AS a JOIN items AS b ON META(b).id = META(a).id ORDER BY a.n;
SELECT b FROM [1] AS a LEFT NEST items AS b ON META(b).id = 'absent';
SELECT b IS MISSING AS m FROM [1] AS a LEFT NEST items AS b ON META(b).id = NULL;
```

The queries return `{"l":"a","r":"a"}`, `{"l":"b","r":"b"}`, `{"l":"c","r":"c"}`;
then `{"a":1}`; then bare documents `{"n":1}`, `{"n":2}`, `{"n":3}`;
then `{"b":[]}`; then `{"m":true}`.

## See also

[FROM](from.md) · [UNNEST](unnest.md) · [SELECT](../statements/select.md) · [EXPLAIN](../statements/explain.md) · [Diagnostics](../diagnostics.md)

## Sources

- **Grammar** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlKeyJoinParserTests.cs`.
- **Execution** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentKeyJoinTests.cs`.
- **Cache and snapshot** — `DocumentKeyJoinCacheTests.cs` and `DocumentKeyJoinSnapshotTests.cs` in the same directory.
- **Wire** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Client/tests/DocumentKeyJoinWireTests.cs`.
