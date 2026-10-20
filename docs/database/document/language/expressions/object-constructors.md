# Object constructors

Builds one JSON object from named expressions.

## Syntax

```syntaxsql
{ [ { string_literal | identifier } : expression [ , ...n ] ] }
```

## Remarks

Names are case-sensitive. MISSING members are omitted; explicit NULL remains. Repeated names resolve
to the last included value rather than creating duplicate JSON properties. A later MISSING does not
remove an earlier value. Nested objects follow the same rules. The result can be projected or used
as an INSERT VALUE. A MISSING whole INSERT value is `COHDOC008`; numeric values must fit the decimal
domain exactly (`COHDOC007`, SQLSTATE 22003).

Objects and [arrays](array-constructors.md) compose in projections (including VALUE/RAW), grouped
expressions, predicates and INSERT. SELECT constructor results may be at most 256 levels deep,
including nested stored or parameter values and the outer object for named projections.
Exceeding that limit raises `COHDOC015` at the constructor expression. INSERT retains its
128-level JSON storage limit (`COHDOC008`); recursive text parsing has a separate 128-level limit
(`OQL0005`). Computed member names and path access on constructor results remain unsupported.

Constructors use the shared [RFC 8259 UTF-8 encoder](../statements/select.md#json-bytes).
INSERT and [UPSERT](../statements/upsert.md) share the 128-level storage bound.

## Examples

For a nonempty collection items:

```sql
SELECT {'a': 1, 'a': 2, 'b': NULL, 'c': MISSING} AS o FROM items;
SELECT {'a': 1, 'a': MISSING} AS o FROM items;
```

Each source row produces `{"o":{"a":2,"b":null}}` and `{"o":{"a":1}}`, respectively.

## See also

[Expressions](index.md) · [INSERT](../statements/insert.md) · [MISSING](missing.md)

## Sources

- **Repeated names** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentInsertTests.cs`.
- **Object values** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/src/Internal/Json/DocumentJsonValue.cs`.
