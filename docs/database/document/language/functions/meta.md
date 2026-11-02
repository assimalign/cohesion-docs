# META

Reads a stored source document's identity or version.

## Syntax

```syntaxsql
META ( [ iteration_variable ] ) . { id | version }
```

## Remarks

META().id returns the document's string key; META().version returns its version as a decimal value.
Without joins, the argument must be the SELECT or DELETE source alias. META accepts zero or one argument and
only id/version members. Invalid member, arity, alias or system-source
context is `CD05001`. Stored-source metadata values are present, not NULL/MISSING. META is also available
inside an INSERT SELECT source; it is not a VALUES source.
In [DELETE](../statements/delete.md), META predicates filter snapshot candidates and do not provide
a document-key lookup access path.

META on an UNNEST or quantified variable reports CD05001, including when a quantifier shadows
the FROM alias. META() and META(source_alias) still refer to the stored source document after
expansion.

On [derived and expression sources](../clauses/from.md), META().id/version and
META(source_alias).id/version return NULL. These rows have no stored identity. An inner stored
SELECT can explicitly project its metadata as ordinary data. Existing argument and grouping
restrictions still apply.

## Joined sources

In a [JOIN/NEST](../clauses/join.md) SELECT, specify META(alias): META() is ambiguous (CD01035).
Each stored source retains its own id/version, even for stored NULL content. Unmatched LEFT JOIN
metadata is MISSING. NEST arrays and derived/expression sources have no single identity, so their
metadata is NULL; an unknown-key LEFT NEST binding is MISSING. Local aliases replace captured
metadata receivers in declaration order. UNNEST/quantifier/comprehension receivers remain invalid;
joined validation uses CD01035.

## Examples

After `INSERT INTO items (KEY, VALUE) VALUES ('a', {'name':'Ada'})`:

```sql
SELECT META(d).id AS key FROM items d;
```

The projected row is `{"key":"a"}`. Use KEY k with an output alias k to copy that key in
[INSERT SELECT](../statements/insert.md).

## See also

[Functions](index.md) · [SELECT](../statements/select.md) · [INSERT](../statements/insert.md)

## Sources

- **Validation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/src/Internal/Planning/DocumentPlanner.Dml.cs`.
- **Execution** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentInsertTests.cs`.
