# FROM

FROM supplies rows from a collection, a derived SELECT or an expression, including a captured outer field.

## Syntax

```syntaxsql
FROM { collection_name [ [ AS ] iteration_variable ]
     | ( SELECT ... ) [ AS ] alias
     | expression AS alias }
[ { [ INNER ] UNNEST | LEFT [ OUTER ] UNNEST } expression [ [ AS ] variable ] ...n ]

<collection_name> ::= { identifier | COHESION_SCHEMA . identifier }
```

## Supported clauses

| Construct | Support | Meaning |
|---|---|---|
| One collection | Supported | Stored or system source |
| Derived SELECT | Supported | Required alias; AS optional |
| Expression | Supported | AS required before an explicit alias |
| `AS iteration_variable` or `iteration_variable` | Supported | Optional path qualifier |
| `COHESION_SCHEMA` source | Supported | Reserved current-database metadata collection |
| `JOIN` | Recognized, not supported (`CK00001`) | No multi-source query |

## Arguments

- **`identifier`** — a case-preserving collection name, optionally double quoted.
- **`iteration_variable`** — an optional identifier prefix for paths such as `p.name`.

## Remarks

An ordinary collection is unqualified. `FROM other.people` is invalid; a quoted name is one opaque
identifier. Only the reserved `COHESION_SCHEMA` namespace allows qualification. The engine resolves
the following read-only sources from the statement's catalog snapshot:

| Source | Document fields |
|---|---|
| `COHESION_SCHEMA.INDEXES` | `COLLECTION_CATALOG`, `COLLECTION_NAME`, `INDEX_NAME`, `PATH`, `PATHS`, `IS_UNIQUE` |
| `COHESION_SCHEMA.OBJECT_OWNERSHIP` | `COLLECTION_CATALOG`, `COLLECTION_NAME`, `OBJECT_TYPE`, `OBJECT_NAME`, `OWNER`, `OWNING_SCHEMA` |

`INDEXES` has one document per visible index. `OBJECT_OWNERSHIP` has one per visible collection;
index ownership follows the collection rather than an independent index owner. For scalar-only
indexes, `PATHS` is the ordered path array and `PATH` its leading element. For array indexes,
PATHS renders ordered scalar paths and normalized DISTINCT ARRAY keys; PATH
is the leading rendered key. Array keys use canonical variable v and a quoted source root.
The typed metadata API still exposes scalar-only Paths/Path; array-aware .NET consumers use
[DocumentIndexMetadata.Keys](../../../../dotnet-apis/resources/database/assimalign-cohesion-database-documents/document-index-metadata.md).
`IS_UNIQUE` is the actual flag.
See [composite/UNIQUE indexes](../statements/create-index.md).
`OWNER` is `Adhoc` or `Schema`; `OWNING_SCHEMA` is a schema name or null. Source names are
case-insensitive, including fully quoted names, while JSON field names remain case-sensitive.

[UNNEST and LEFT UNNEST](unnest.md) can follow any source. They expand arrays
before WHERE; a missing alias defaults to the final named path step. Bindings must be unique,
including the reserved unaliased collection name (CD00010 in text, CD01023 in a direct AST).
Inside a correlated subquery, the collection name reaches an unaliased outer row. Own-row
paths retain the [known scope inconsistency](../expressions/subqueries.md#correlation-and-scope).

## Derived and expression rows

A derived SELECT requires an alias. Named projections supply objects, omitting MISSING members;
RAW/VALUE supplies each projected value directly, normalizing MISSING to NULL. The source alias
names the complete row; unqualified fields address its object members. A generic expression can
be an array, parameter, constructor, function or operator. An ordinary bare identifier still
names a collection. An unnamed expression needs an implicit field name; literals, arrays and
parameters require `AS alias`. The source expression has no implicit input-row scope.

| Expression value | Input rows |
| --- | --- |
| Array | One per element in array order; an empty array supplies none |
| Object | One containing the complete object |
| Number, string or Boolean | One containing that scalar |
| NULL | One NULL row |
| MISSING | No rows |

`SELECT *` wraps these rows in an object keyed by the source alias, plus any UNNEST bindings.
For example, `SELECT * FROM [1,NULL,{n:2}] AS v` returns `{"v":1}`, `{"v":null}` and
`{"v":{"n":2}}`. Stored collection stars keep their existing shape. META().id/version and
META(alias).id/version return NULL on derived and expression rows, which have no stored identity.
Metadata explicitly projected by an inner stored SELECT remains ordinary data.

WHERE, GROUP BY, GROUP AS, HAVING, aggregates, DISTINCT, ORDER BY, paging, UNNEST and further
derived nesting compose with these sources. All levels share the statement snapshot, earlier
transaction writes, parameters, function catalog, cancellation and memory budget.

## Evaluation

An expression source evaluates once when reached, per outer row for correlated sources.
For example, `SELECT RAW (SELECT RAW i FROM d.items AS i) FROM [{items:[1,2]}] AS d`
returns one bare array `[1,2]`. NULL supplies one NULL row and MISSING supplies none. Simple derived SELECTs stream rows without
materializing an expression-subquery array. An outer LIMIT stops pulling matching inner rows
and disposes the cursor; on this nonblocking path paging precedes projection and dynamic column
types describe the selected page. Grouping, DISTINCT and ordering retain their required rows
under the shared CD05018 budget. Outer ORDER BY sorts its inputs.

After eager static validation, LIMIT 0 returns no rows with planned column names and types,
before opening any stored, system, expression or derived source or executing blocking stages.
It runs no source expression, predicate, projection or callback. An inner SELECT's own LIMIT 0
has the same behavior. Missing collections, parameters and invalid names still fail validation.
EXPLAIN describes DerivedSource/ExpressionSource and nested plans without executing them.

Correlated sources and derived/expression INSERT/UPSERT SELECT sources execute. JOIN/NEST
(including comma joins) remain CK00001. See [expression subqueries](../expressions/subqueries.md)
for captures, shared work limits and statement-start mutation reads.

## Diagnostics

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CD00015 | Derived SELECT has no alias | — |
| CD01033 | Expression source has neither an explicit alias nor an implicit field name | — |
| CD01034 | Free variable in a source expression with no input-row scope | — |
| CK00001 | JOIN/NEST | — |
| CD05015 | Serialized projection exceeds 256 levels, including the star wrapper | — |
| CD05018 | Shared statement memory budget exhausted | — |
| CD05020 | Shared reached-expression work budget exhausted | — |

## Examples

The parser corpus supplies the ordinary collection query; the engine design supplies introspection:

```sql
SELECT p.name FROM people AS p WHERE p.age >= $age
```

```sql
SELECT INDEX_NAME, PATH FROM COHESION_SCHEMA.INDEXES
WHERE COLLECTION_NAME = 'items' ORDER BY INDEX_NAME
```

Execute each statement separately:

```sql
SELECT RAW x FROM [{tags:[1,2]},{tags:[3]}] AS d
UNNEST d.tags AS x ORDER BY x DESC LIMIT 2;
SELECT SUM(d.n) AS total FROM (SELECT v AS n FROM [1,2,3] AS v) d;
```

Returns bare rows `3`, `2`, then `{"total":6}`, respectively.

## See also

[Clauses](index.md), [SELECT](../statements/select.md), and [WHERE](where.md).

## Sources

- **Scope and grammar** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/docs/DESIGN.md`.
- **System collections** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/docs/DESIGN.md`.
- **Example** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlQueryParserTests.cs`.
- **Source rows and streaming** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentSourceTests.cs` and `DocumentLimitZeroTests.cs`.
