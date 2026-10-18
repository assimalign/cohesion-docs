# META

Reads a stored source document's identity or version.

## Syntax

```syntaxsql
META ( [ iteration_variable ] ) . { id | version }
```

## Remarks

META().id returns the document's string key; META().version returns its version as a decimal value.
When supplied, the argument must be the SELECT or DELETE source alias. META accepts zero or one argument and
only id/version members, on stored collection sources. Invalid member, arity, alias or system-source
context is `COHDOC001`. The metadata values are present, not NULL/MISSING. META is also available
inside an INSERT SELECT source; it is not a VALUES source.
In [DELETE](../statements/delete.md), META predicates filter snapshot candidates and do not provide
a document-key lookup access path.

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

- **Validation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/src/Internal/DocumentPlanner.Dml.cs`.
- **Execution** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentInsertTests.cs`.
