# INSERT

Creates documents with explicit keys from VALUES or a SELECT snapshot.

## Syntax

```syntaxsql
INSERT INTO collection [ AS alias ] (KEY, VALUE)
    VALUES (key_expression, value_expression) [ , ...n ]
INSERT INTO collection [ AS alias ] (KEY output_name [, VALUE output_name]) select_statement
```

## Remarks

KEY must evaluate to a nonblank string. VALUE can be any valid JSON value, including NULL,
object, array or scalar, but cannot be MISSING. VALUES expressions are column-free and cannot
use aggregates or source metadata. [Object constructors](../expressions/object-constructors.md)
produce one keyed object with omitted MISSING members.

INSERT SELECT resolves the named KEY and optional VALUE against SELECT output names,
case-sensitively. With VALUE, that output is stored as the whole document. Without VALUE, the
entire projection object is stored, including the key output and excluding MISSING members.
[META().id](../functions/meta.md) obtains source keys. The complete source snapshot is materialized
and validated before writes, so newly inserted documents do not feed the source.

INSERT is insert-only; [PutAsync](../../../../dotnet-apis/resources/database/assimalign-cohesion-database-documents-client/document-connection.md)
remains replacement/upsert. A key already visible in the transaction, including a repeated key
within VALUES or a self-copy, raises DocumentDuplicateKeyException with `COHDOC012`. The statement
rolls back all its writes, versions and index changes. Explicit transaction failure follows the
Documents transaction rule. Success reports the number inserted.

| Code | Meaning |
| --- | --- |
| `COHDOC001` | Invalid VALUES context, output-name binding or aggregate/grouping context |
| `COHDOC002` | Destination collection does not exist |
| `COHDOC004` | Read-only system destination |
| `COHDOC005` | Missing parameter |
| `COHDOC007` (22003) | Number cannot fit the decimal domain exactly, including underflow or lost fractional digits |
| `COHDOC008` | Invalid/non-string/blank key, MISSING whole value or invalid JSON |
| `COHDOC012` | Duplicate document key |

RETURNING, UPSERT, UPDATE and DELETE remain outside OQL's executable statements.

## Examples

Execute each statement separately:

```sql
CREATE COLLECTION items;
CREATE COLLECTION archive;
INSERT INTO items (KEY, VALUE) VALUES ('a', {'name':'Ada'}), ('b', NULL);
INSERT INTO archive (KEY k, VALUE v)
    SELECT META(d).id AS k, d AS v FROM items d;
SELECT META().id AS id FROM archive ORDER BY META().id;
```

Each INSERT affects two documents. The final IDs are a and b. Repeating either insert raises
COHDOC012 and leaves the existing documents unchanged.

## See also

[Statements](index.md) · [SELECT](select.md) · [META](../functions/meta.md) · [EXPLAIN](explain.md)

## Sources

- **Keys, query sources and atomicity** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentInsertTests.cs`.
- **Exact numbers** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentInsertNumericTests.cs`.
- **Executor** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/src/Internal/DocumentPlanExecutor.Dml.cs`.
