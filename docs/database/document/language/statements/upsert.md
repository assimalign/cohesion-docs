# UPSERT

Inserts absent keys and wholly replaces documents whose keys already exist.

## Syntax

```syntaxsql
UPSERT INTO collection [ AS alias ] (KEY, VALUE)
    VALUES (key_expression, value_expression) [ , ...n ] [ RETURNING projection ]
UPSERT INTO collection [ AS alias ] (KEY output_name [, VALUE output_name]) select_statement
    [ RETURNING projection ]
```

## Arguments

- **`collection`** — One stored collection in the session database. Schema-owned content is writable; system sources are read-only.
- **`key_expression`** — A nonblank string identity, compared case-sensitively.
- **`value_expression`** — The complete replacement JSON value: object, array, scalar or NULL.
- **`output_name`** — A case-sensitive SELECT output name supplying KEY or VALUE.

## Remarks

UPSERT replaces the entire JSON value. Members absent from the replacement disappear. Every input
pair counts once and receives a fresh version, including an unchanged value. Content, versions and
secondary indexes change in the same transaction; replaced index entries disappear and new entries
participate immediately. Older snapshots retain their visible versions.

VALUES expressions have no document, metadata or aggregate scope. SELECT materializes its complete
source before writes, including a source that is also the destination. With VALUE, that named output
is stored as the whole document. Without VALUE, the complete projected row is stored, including the
key output and omitting MISSING members. A MISSING or NULL key is invalid. A whole MISSING VALUE
is invalid; explicit NULL is a valid document. Object members omit MISSING, while array elements
convert it to JSON null. These rules are shared with [INSERT](insert.md).

Repeating a key in one statement fails with DocumentDuplicateKeyException (CD03012), even for
UPSERT and even when the key already exists. Its message is `Document key 'a' appears more than
once in one statement.` There is no last-pair-wins rule. All writes and index changes roll back.
An automatic session remains reusable after failure; failure in an explicit transaction aborts
all its work and requires rollback. Success returns the number of written pairs. Without RETURNING
there are no result rows. [RETURNING](../clauses/returning.md) projects each stored post-image and
new version in VALUES or final SELECT source order; projection failure rolls back the writes.

UPSERT has no statement version precondition. Use PutAsync(expectedVersion) or UPDATE with
META().version for a version check. OPTIONS remains CK00001. JSON storage depth is
128, and numbers must fit decimal exactly. Generated JSON follows the shared
[RFC 8259 UTF-8 encoding](select.md#json-bytes) in process, over InMemory and over TCP.

## Diagnostics

| Code | Meaning and trigger | SQLSTATE where supplied |
| --- | --- | --- |
| `CD00002` | Missing KEY/VALUE header, malformed pair/binding, qualified destination or trailing syntax | — |
| `CK00001` | OPTIONS or a nested SELECT in VALUES | — |
| `CD05001` | VALUES reads paths/metadata/aggregates, or KEY/VALUE binding names no SELECT output | — |
| `CD01002` | Destination collection does not exist | — |
| `CD04004` | Read-only system destination | — |
| `CD01005` | Missing parameter, including an empty source or EXPLAIN | — |
| `CD02007` | Number loses digits, underflows or exceeds decimal range | 22003 |
| `CD02008` | Non-string/blank/NULL/MISSING key, whole MISSING value, invalid JSON or storage depth beyond 128 | — |
| `CD03012` | A key occurs more than once in this statement, in VALUES or SELECT | — |

Validation failures name UPSERT, for example `UPSERT KEY must evaluate to a nonblank string.`
Other expressions retain their [normal diagnostics](../diagnostics.md).

## Examples

Execute separately in a fresh database:

```sql
CREATE COLLECTION items;
INSERT INTO items (KEY, VALUE) VALUES ('a', {name:'old', amount:2});
UPSERT INTO items (KEY, VALUE) VALUES ('a', {name:'new'}), ('z', NULL);
SELECT META(d).id AS id, d.name AS name, d.amount AS amount FROM items d ORDER BY id;
```

UPSERT affects two documents. Results are `{"id":"a","name":"new"}` and `{"id":"z"}`;
amount is MISSING, and both name and amount are MISSING on the NULL-root document.

```sql
CREATE COLLECTION archive;
UPSERT INTO archive (KEY k, VALUE v) SELECT META(d).id AS k, d AS v FROM items d;
UPSERT INTO archive (KEY k) SELECT META(d).id AS k, d.name AS name FROM items d;
```

Each affects two documents. The second replaces a with `{"k":"a","name":"new"}` and z with
`{"k":"z"}`. Repeating a key within either source fails with CD03012.

## See also

[Statements](index.md) · [INSERT](insert.md) · [UPDATE](update.md) · [EXPLAIN](explain.md) · [META](../functions/meta.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentUpsertTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlDmlParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Client/tests/DocumentClientWireTests.cs`.
