# CREATE SEQUENCE (Cohesion SQL)

Creates a standalone integer sequence in the session database.

## Syntax

```syntaxsql
CREATE SEQUENCE [ IF NOT EXISTS ] [ schema_name . ] sequence_name
    [ AS { SMALLINT | INTEGER | BIGINT } ] [ sequence_option ...n ] [ ; ]

sequence_option := START [ WITH ] integer | INCREMENT [ BY ] integer
                 | MINVALUE integer | NO MINVALUE
                 | MAXVALUE integer | NO MAXVALUE
                 | CACHE integer | CYCLE | NO CYCLE
                 | OWNED BY { [ schema_name . ] table_name . column_name | NONE }
```

## Remarks

Options may appear in any order, each once. Names default to dbo and share the case-insensitive
table/view relation namespace, including quoted names. IF NOT EXISTS atomically skips an occupied
relation name before validating options, even if that relation is a table or view. It does not
replace an existing definition. DDL commits independently and is refused inside explicit
transactions with CS04003 (25001).

| Option | Default and meaning |
| --- | --- |
| AS | BIGINT; SMALLINT/INTEGER/BIGINT set bounds and metadata; value expressions still return BIGINT |
| INCREMENT | 1; a nonzero signed Int64, positive or negative |
| MINVALUE / NO MINVALUE | Ascending: 1; descending: type minimum; NO restores the directional default |
| MAXVALUE / NO MAXVALUE | Ascending: type maximum; descending: -1; NO restores the directional default |
| START | Directional starting bound: minimum for ascending, maximum for descending |
| CACHE | 1; positive metadata value; allocation still reserves up to 32 values durably through the shared allocator |
| CYCLE / NO CYCLE | NO CYCLE; cycling wraps at the directional bound |

Bounds must be ordered and fit the selected type; START must fit the bounds. Noncycling range
exhaustion is CS02002 (22003). Reservations and setval are nontransactional; rollback or recovery
may leave gaps. CACHE does not create a separate per-session value cache.

[Sequence functions](../functions/sequences.md) allocate and inspect values. Options appear in
[INFORMATION_SCHEMA.SEQUENCES](../system-views/information-schema.md#sequences).
Constant [column sequence defaults](../clauses/default.md#sequence-defaults) retain early-bound
dependencies. Compound/dynamic defaults, CHECK, domain, view and SQL-function sequence expressions
remain CS03074 (0A000). Declared provisioning scripts refuse standalone sequence DDL with the
same code; compiled schemas do not declare standalone sequences. [Identity](../clauses/identity.md)
uses this shared sequence allocator, including SERIAL aliases.

Supported column defaults are `nextval('s')`, `nextval('s'::regclass)`,
`nextval(CAST('s' AS REGCLASS))` and `NEXT VALUE FOR s`; all survive restart with their sequence
dependency intact. A domain default calling nextval is refused with CS03074 (0A000). See the
[default example](../functions/sequences.md#column-defaults).

## Ownership

OWNED BY attaches a standalone sequence to an existing column in the same SQL schema; NONE
(the default) leaves it unowned or detaches it under ALTER. Dropping the owning table/column
removes the sequence atomically. Ownership does not install a DEFAULT or identity on the column.
Invalid targets, cross-schema ownership and identity-sequence ownership changes use CS03081
(55000). External defaults depending on an owned sequence must be removed before its owner is
removed. [DROP SEQUENCE](drop-sequence.md) RESTRICT/CASCADE manages default dependencies;
identity-owned sequences cannot be dropped directly, even with CASCADE (CS03080, 2BP01).

## Diagnostics

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CS00003 | Malformed/repeated option syntax | — |
| CS03069 | Invalid type, increment, cache, bounds or start | 22023 |
| CS03070 | Occupied sequence/table/view name without the guard | 42P07 |
| CS03071 | Namespace is absent | 3F000 |
| CS03072 | Encoded definition exceeds 8,092 bytes | 54000 |
| CS03074 | Sequence DDL in a declared provisioning script or unsupported persisted sequence expression | 0A000 |
| CS04003 | Self-committing DDL inside an explicit transaction | 25001 |
| CS03081 | Invalid OWNED BY target or identity ownership change | 55000 |

## Example

Execute separately in a fresh database:

```sql
CREATE SEQUENCE s AS SMALLINT START 3 INCREMENT 2 MINVALUE 1 MAXVALUE 99 CACHE 4 CYCLE;
SELECT nextval('s'), nextval('s'), currval('s');
```

Returns BIGINT values `(3,5,5)`.

For a separate owned sequence, execute separately:

```sql
CREATE TABLE owner_table(k INT);
CREATE SEQUENCE owned_s OWNED BY owner_table.k;
ALTER SEQUENCE owned_s OWNED BY NONE;
DROP TABLE owner_table;
SELECT nextval('owned_s');
```

Returns 1 because NONE detached the sequence before table deletion. Without detachment,
dropping the owning table would also remove owned_s.

## See also

[Statements](index.md) · [ALTER SEQUENCE](alter-sequence.md) · [DROP SEQUENCE](drop-sequence.md) · [NEXT VALUE FOR](../expressions/next-value-for.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlSequenceSqlTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Internal/Execution/SqlPlanExecutor.Sequences.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Internal/Catalog/SqlSequenceOptions.cs`.
