# CREATE INDEX (Cohesion SQL)

Creates a table-scoped secondary index over a plain column list, optionally restricted by a predicate.

> **Status:** Partial. The documented subset is measured against the live SQL engine.

## Syntax

```syntaxsql
CREATE [ UNIQUE ] INDEX [ IF NOT EXISTS ] index_name
ON [ schema_name . ] table_name ( <index_key> [ , ...n ] )
[ WHERE predicate ]
[ ; ]

<index_key> ::= column_name [ ASC | DESC ] [ NULLS { FIRST | LAST } ]
```

## Arguments

- **`index_name`** — The index identity, scoped to its table.
- **`column_name`** — A key column; list order defines composite-key order.
- **`UNIQUE`** — Rejects duplicate encoded keys among indexed rows. Partial unique indexes keep NULL keys distinct.
- **`IF NOT EXISTS`** — Allows an index with that name to exist.
- **`predicate`** — An immutable, row-local Boolean expression. Only TRUE rows enter the index;
  FALSE and UNKNOWN rows have no entry.

## Remarks

Each plain column key accepts ASC/DESC and independent NULL placement, including on partial
indexes. Defaults follow PostgreSQL: ASC NULLS LAST and DESC NULLS FIRST. Canonical DDL omits
ASC and each direction's default NULL placement. PRIMARY KEY and UNIQUE constraint lists
reject these options with `CS00003`; expression keys and INCLUDE remain excluded.
Index construction and writes use the shared index kernel.
An applicable secondary-index prefix can assist an inner join; a partial index additionally
requires a predicate proof. SELECT, UPDATE and DELETE use [eligible leading key bounds](select.md#index-access);
[EXPLAIN](explain.md) reports the selected seek.

Unconditional unique indexes treat `NULL` as an equal key. String keys use the column's effective collation;
`invariant` is scan-only and cannot back an index or index-backed constraint. Predicates whose
collation differs from the index use a scan.

Duplicate keys during a unique-index build or a later write report `CS03037` (SQLSTATE
23505). The constraint exception carries one Error diagnostic naming the index and table
without key values; a failed build leaves the index unpublished. See [Diagnostics](../diagnostics.md).

Index DDL is self-committing and is rejected inside an explicit transaction with `CS04003`.
An existing index name reports CS03049 (42P07); a conflicting constraint name reports
CS03050 (42710). CREATE INDEX under `invariant` uses CS05013 (no assigned SQLSTATE);
index-backed constraint admission instead uses CS05057 (0A000).
Schema-owned object admission can report CS03066 (42501). Execution/DDL failures retain the
statement span and expose the code separately from Message.

### Key directions and NULL placement

Equality-prefix seeks and ranges honour each key's direction. Bounds come from =, <, <=, >,
>= and BETWEEN; descending ranges reverse their physical endpoints. NULL placement is
independent of value direction. Comparison ranges never visit the NULL block, including
one-sided ranges; NULL equality prefixes retain the complete matching suffix range.
LIKE remains a residual after an equality-prefix seek.

Matching ORDER BY keys can use forward index order or reverse every direction and NULL
placement together for a backward scan. NULL placement is irrelevant for keys proven unable
to vary between NULL and non-NULL, including NOT NULL columns and equality prefixes.
See [ORDER BY](../clauses/order-by.md#index-order) for matching rules, costing and the difference
between query and index defaults.
Index direction does not change uniqueness or partial-index NULL rules. Floating index keys
canonicalize signed zeros together and place all NaNs below negative infinity; stored row
values keep their IEEE representation. Floating seek eligibility remains conservative.

[INDEXES metadata](../system-views/cohesion-schema.md) exposes nullable DIRECTION and
NULL_PLACEMENT per key. Catalog format 32 and compiled schema v11 refuse earlier formats;
follow the [export and rebuild procedure](../../index.md#sql-format-32-and-schema-v11).
Invalid key-order widths/values or directed primary-key metadata use `CS03085` (42P17).
Malformed stored vectors use CS06059; unsupported extensions use CS06060; an older SQL
record-space format uses CS06041.

### Partial indexes

The predicate may use the table's columns, immutable scalar functions and built-in casts. It
cannot contain subqueries, aggregates, windows, parameters, sequences, or Stable/Volatile
functions (`CS05091`, SQLSTATE 42P17). Explicit and implicit conversions must be provably
immutable. Reachable application casts are refused because their registrations have no
immutability metadata; domain checks must be immutable. Every CASE/COALESCE/NULLIF alternative
is checked, including an unreachable branch. Unknown columns retain `CS01015`; a non-Boolean
predicate uses `CS02003` (42804).

Rows enter or leave the index whenever a write changes predicate membership. This applies to
INSERT VALUES/SELECT, UPDATE and DELETE (including joined forms), MERGE, ON CONFLICT actions,
referential actions, table rewrites, identity backfills and TRUNCATE. A partial unique index
checks uniqueness only within its TRUE subset; any NULL key component keeps that key distinct.
An update that brings a duplicate non-NULL key into the subset fails with `CS03037` (23505).

Untargeted [ON CONFLICT DO NOTHING](../clauses/on-conflict.md) considers each partial unique
index only when the proposed row belongs to it, following PostgreSQL.
Targeted `ON CONFLICT (cols) WHERE predicate` can infer partial unique indexes when its
predicate proves theirs; a missing or unproven inference predicate uses `CS03035` (42P10).
ON CONSTRAINT cannot name a partial index. Partial unique indexes cannot supply a foreign-key target.

Dropping a key or predicate column drops its index. Changing a predicate column's type,
including widening, refuses with `CS03083` (0A000): drop and recreate the index around the
change. Table and column renames carry into the stored predicate text. Predicate function and
domain dependencies also participate in DROP restrictions.

If a required function registration disappears or changes incompatibly at restart, or a cast
or domain no longer proves immutable, the database opens with an unresolved predicate. Reads
and DROP INDEX work, but every write to that table fails with `CS01009` (42883), even with
empty input. Restoring the required immutable definition or dropping the index restores writes.

Catalog metadata and [INFORMATION_SCHEMA.INDEXES](../system-views/information-schema.md)
expose canonical predicate text. SQL catalog format 32 and compiled schema v11 persist it and
refuse earlier formats. Invalid predicate/dependency metadata uses `CS03084` (42P17); a
programmatically supplied primary-key predicate uses `CS03082` (42P16).

The planner may use a partial index when mandatory query conjuncts prove its predicate. It
matches bound column names and aliases, parentheses, literal signs and standard literal casts:

- Identical immutable row expressions prove themselves, except general expressions involving OR or NOT.
  [JSON extraction and IS JSON](../expressions/json-access.md#partial-indexes) match exact
  conjuncts, including the IS JSON kind and negation.
- Comparisons of the same column and constant type, with the same resolved collation, use
  equality and ordered bounds: `x > 10` implies `x > 5`; `x = 7` implies `x >= 7`;
  `x >= 7` does not imply `x > 7`.
- A strict column/constant comparison with a non-NULL constant implies `column IS NOT NULL`.
- A Boolean column and its `= TRUE` form are interchangeable.

Identical-expression matching preserves constant representations, including decimal scale,
floating sign bits and timestamp offsets. Comparison bounds compare values. Floating-point
bound deductions, contradiction reasoning and cross-column/transitive facts supply no proof.
Parameters supply no proof, including after folding through immutable calls, casts or signs.
OR, NOT and volatile calls supply no proof.

Proofs come only from the indexed input's own WHERE conjuncts and INNER JOIN ON conjuncts
referencing that input alone. An ON term must reference at least one of its columns;
constant-only ON terms, outer-join predicates, nullable outer-join inputs and other scopes
supply no proof. The complete query predicate remains a residual filter.

A proven index can provide equality seeks, ranges, correlated join probes or an unbounded
subset scan. This includes UPDATE FROM, DELETE USING and MERGE inputs where row preservation
permits it. Partial uniqueness never establishes unconditional DISTINCT or join uniqueness.
[EXPLAIN](explain.md) reports `partial_predicate` and `implied_by` for selected partial indexes.

### Building over existing data

The build holds the table's Exclusive lock through construction, validation and publication;
concurrent writers wait. It includes stored historical versions so older snapshots can use the
index, but checks uniqueness against current live keys even if a writer committed after the
DDL captured its initial snapshot. Deleted history does not cause false duplicate failures.

Unique builds construct the whole tree, then compare adjacent live keys in bounded cursor
batches, retaining one previous key. Duplicate-detection memory is independent of row count;
the build uses the existing buffer-pool and spillable pre-image budgets separately. The extra
tree pass delays duplicate detection until construction finishes. Inline builds, deferred
provisioning builds and PRIMARY KEY/UNIQUE backing indexes use this validation.

An inline CREATE UNIQUE INDEX reports the smallest duplicated live key in encoded index order,
rather than the first duplicate in storage order. Its in-process exception's safe scalar
OffendingValue, when available, follows that key; messages and wire diagnostics omit key values.
A duplicate, cancellation or storage failure before publication cleans up the unpublished tree
and registration. After fixing the cause, the same index name can be retried.

Unique and nonunique builds enforce the same **1,024-byte encoded key limit** as later writes.
The count includes NULL markers, component delimiters and collation transforms. A key of exactly
1,024 bytes is accepted; a larger key reports `CS05042` / SQLSTATE 54000, leaves the index
unpublished and cleans up the unpublished tree. The table and connection remain usable. Historical
versions are included in construction. See [size limits](../diagnostics.md#row-and-index-key-limits).
Because construction precedes duplicate validation, CS05042 can take precedence over a
CS03037 duplicate that occurs earlier in storage order.

## Examples

Execute separately in a fresh database:

```sql
CREATE TABLE directed_keys (id INT, k INT, active BOOLEAN);
INSERT INTO directed_keys VALUES (1,NULL,TRUE),(2,7,TRUE),(3,11,TRUE),(4,12,FALSE);
CREATE INDEX ix_directed ON directed_keys(k DESC NULLS LAST) WHERE active;
SELECT id FROM directed_keys WHERE active AND k > 5 ORDER BY k DESC NULLS LAST;
SELECT DIRECTION, NULL_PLACEMENT FROM COHESION_SCHEMA.INDEXES
WHERE TABLE_NAME = 'directed_keys' AND INDEX_NAME = 'ix_directed';
```

The first SELECT returns `3`, `2`; this index can supply its order. The metadata row is `('DESC', 'LAST')`.

Execute separately in a fresh database:

```sql
CREATE TABLE filtered_keys (id INT, x INT);
INSERT INTO filtered_keys VALUES (1,NULL),(2,7),(3,11);
CREATE INDEX ix_positive ON filtered_keys(x) WHERE x > 5;
EXPLAIN SELECT id FROM filtered_keys WHERE x > 10;
SELECT id FROM filtered_keys WHERE x > 10;
```

The query returns `3`. The eligible partial index's EXPLAIN detail includes `partial_predicate`
and `implied_by`, identifying the predicate and the query conjunct proving it.

Execute separately in a fresh database:

```sql
CREATE TABLE subset_keys (id INT PRIMARY KEY, k INT, active BOOLEAN);
INSERT INTO subset_keys VALUES (1,7,TRUE),(2,7,FALSE),(3,NULL,TRUE),(4,NULL,TRUE);
CREATE UNIQUE INDEX uq_active ON subset_keys(k) WHERE active;
INSERT INTO subset_keys VALUES (5,7,TRUE) ON CONFLICT DO NOTHING;
UPDATE subset_keys SET active=FALSE WHERE id=1;
UPDATE subset_keys SET active=TRUE WHERE id=2;
SELECT id FROM subset_keys WHERE active ORDER BY id;
```

The insert skips its conflict. SELECT returns `2`, `3`, `4`; the updates move the non-NULL
key between members, and both NULL keys coexist.

Using the [conformance fixture](select.md#a-create-the-conformance-fixture):

```sql
CREATE UNIQUE INDEX ix_name ON t(name);
SELECT id FROM t WHERE name = 'Ada';
```

A subsequent insert of another `Ada` name reports CS03037 (23505) without inserting a row.

In an empty database, bind `v` to `new string('x', 2000)` and execute separately:

```sql
CREATE TABLE wide_keys (id INT, v TEXT);
INSERT INTO wide_keys VALUES (1, @v);
CREATE INDEX ix_v ON wide_keys(v);
SELECT id FROM wide_keys;
```

CREATE INDEX fails with CS05042; SELECT returns `1`, and no index is published.

Execute separately in a fresh database:

```sql
CREATE TABLE duplicate_keys (a INT, b INT);
INSERT INTO duplicate_keys VALUES (1, NULL), (2, NULL), (1, NULL);
CREATE UNIQUE INDEX ix_ab ON duplicate_keys(a, b);
SELECT a, b FROM duplicate_keys ORDER BY a;
```

The build fails with CS03037 (23505) because the two (1, NULL) keys compare equal. No index
is published; SELECT returns (1, NULL), (1, NULL), (2, NULL). SQL has no MISSING key value.

## See also

[DROP INDEX](drop-index.md) · [COLLATE](../clauses/collate.md) · [JOIN](../clauses/join.md)

[Statements](index.md) · [Language (SQL)](../index.md)

## Sources

- **Directions and NULL ranges** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlDirectedIndexTests.cs`, `SqlDirectedRangeNullTests.cs` and `Catalog/SqlCatalogKeyOrderCorruptionTests.cs` in the same test tree.

- **Implication and inference** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlPartialIndexImplicationTests.cs` and `SqlPartialIndexReviewFixTests.cs` in the same directory.

- **Partial predicates, maintenance and arbitration** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlPartialIndexTests.cs`, `SqlPartialIndexDurabilityTests.cs` and `SqlPartialIndexSecondPassTests.cs` in the same directory.
- **Bounded builds, history and failure cleanup** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlDdlMemoryTests.cs` and `SqlUniqueIndexBuildTests.cs` in the same directory.
- **Conformance evidence** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlProgramLimitConformanceTests.cs`.
- **Conformance evidence** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlProgramLimitReviewTests.cs`.

- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`
- **Implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlQueryParser.Ddl.cs`
- **Duplicate keys** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlCollationIndexTests.cs`.

