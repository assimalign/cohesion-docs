# UNNEST

Expands an array into query rows, optionally preserving inputs without array elements.

## Syntax

```syntaxsql
FROM collection_name [ [ AS ] source_alias ]
[ { [ INNER ] UNNEST | LEFT [ OUTER ] UNNEST } array_expression [ [ AS ] variable ] ...n ]
```

## Remarks

Each binding evaluates in the preceding tuple's scope. INNER (the default) emits one row per
array element, in array order. MISSING, NULL, scalar, object and empty-array operands emit no rows.
LEFT emits one row with a MISSING variable for each such operand. An actual NULL array element
remains NULL. Chaining expands each preceding tuple; rows follow document-ID order then array
position at each binding. WHERE runs after expansion, COUNT(*) counts expanded rows, and paging
counts matching expanded rows.

An omitted alias defaults to the last named path step; an expression without one needs an
explicit alias. Binding names compare ordinally, with case preserved. Variables shadow document
fields. The explicit FROM alias (or unaliased collection name) and all UNNEST variables must be
unique. An unaliased collection name is reserved for duplicate checks and the synthetic star key;
it does not become an implicit path variable.

With UNNEST, SELECT * constructs an object keyed by the source alias (or collection name) and
each UNNEST variable. MISSING bindings are omitted and explicit NULL is retained. An unaliased
system source uses its full COHESION_SCHEMA name as the key. Plain star without UNNEST retains
the stored value and exact bytes. Named projections retain
[SupportsMissing/IsMissing](../../../../dotnet-apis/resources/database/assimalign-cohesion-database-execution/database-row.md).

Grouping, HAVING, aggregates, DISTINCT and ORDER BY consume expanded tuples; ORDER BY output
aliases take precedence over variable names. A document-only WHERE conjunct can still use an
index. Direct document-path sources can use a [DISTINCT array index](../statements/create-index.md#array-access-and-fetch-bounds)
when a mandatory element comparison supplies its bound. LEFT UNNEST is eligible when that
comparison rejects the synthetic MISSING row. Candidates deduplicate before content fetch;
expansion retains duplicate elements. Dependent sources do not supply array seeks.
Expansion has no row cap and can multiply memory use. [EXPLAIN](../statements/explain.md)
adds Unnest/LeftUnnest nodes with each variable name.

## Diagnostics

| Code | Meaning and trigger |
| --- | --- |
| CD00010 | Repeated FROM/UNNEST binding name, including the reserved unaliased collection name; located at the repeated/inferred path name |
| CD01023 | Direct SELECT AST repeats a binding name or supplies an empty UNNEST name |
| CD00002 | Expression has no inferable path name and no alias |
| CD05001 | META(variable) on an UNNEST variable, or aggregate in the array expression |
| CK00001 | UNNEST clause in UPDATE/DELETE; JOIN, NEST, WITHIN or FLATTEN calls |

These codes have no assigned SQLSTATE. Quantifiers may shadow outer bindings.

## Example

Execute each statement separately:

```sql
CREATE COLLECTION items;
INSERT INTO items (KEY, VALUE) VALUES
    ('a', {'tags':['blue', NULL]}), ('b', {});
SELECT META(d).id AS id, t FROM items d UNNEST d.tags t;
SELECT * FROM items d LEFT UNNEST d.tags;
```

The first query returns {"id":"a","t":"blue"} and {"id":"a","t":null}.
The second returns {"d":{"tags":["blue",null]},"tags":"blue"},
{"d":{"tags":["blue",null]},"tags":null}, then {"d":{}}. Its inferred variable is tags.

For a document with `tags:[1,1,2]`, an index
`CREATE INDEX by_tag ON items (DISTINCT ARRAY v FOR v IN tags END)` can supply candidates for
`SELECT t FROM items d UNNEST d.tags t WHERE t>=1`. The result still contains 1, 1 and 2:
distinct postings deduplicate document fetches, not expanded rows.

## See also

[FROM](from.md) · [SELECT](../statements/select.md) · [Quantified predicates](../expressions/quantified-predicates.md) · [Clauses](index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentQueryTests.ArrayIndexes.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentQueryTests.Arrays.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlArrayParserTests.cs`.
