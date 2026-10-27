# JOIN (Cohesion SQL)

Combines stored tables, registered system relations and derived queries with INNER, LEFT, RIGHT, FULL, CROSS, and comma joins.

> **Status:** Partial. The documented subset executes; restrictions are listed below.

## Syntax

```syntaxsql
FROM <table_source> [ <join> ...n ] [ , <table_source> [ <join> ...n ] ...n ]

<table_source> ::= [ schema_name . ] table_name [ [ AS ] table_alias ]
                 | ( select_statement ) [ AS ] table_alias [ ( column_name [ , ...n ] ) ]
                 | ( <table_source> <join> [ <join> ...n ] )
                   [ [ AS ] table_alias [ ( column_name [ , ...n ] ) ] ]
<join> ::= { [ INNER ] JOIN | LEFT [ OUTER ] JOIN | RIGHT [ OUTER ] JOIN | FULL [ OUTER ] JOIN } <table_source>
           { ON search_condition | USING ( column_name [ , ...n ] ) }
         | CROSS JOIN <table_source>
```

## Arguments

- **`table_source`** — A stored table, [CTE](with.md), INFORMATION_SCHEMA/COHESION_SCHEMA relation or [derived table](from.md#derived-tables).
- **`search_condition`** — The predicate evaluated for each candidate pair.
- **`column_name`** — A name occurring exactly once in each input to that USING step.

## Remarks

Only `TRUE` matches ON or USING's implicit equalities; `FALSE` and `UNKNOWN` do not.
LEFT emits one row with NULL in every right column when no candidate matches. A predicate in ON
can preserve unmatched left rows; the same predicate in WHERE can remove them. `COUNT(*)` counts
the extended row, while `COUNT(right.column)` ignores its NULL. SQL has no MISSING value.
CROSS and comma joins produce Cartesian products; an empty input makes the product
empty while retaining column metadata.

RIGHT preserves every written right row and fills the entire written left subtree with NULL
when no pair matches. ON/WHERE and COUNT follow the corresponding LEFT rules with sides reversed.
SELECT * retains written left-then-right column order; qualified references keep their written
inputs. Left-side columns become nullable in result metadata, including declared NOT NULL columns;
right-side columns retain their input nullability. Implementation normalization does not reorder
the visible columns.

FULL [OUTER] JOIN returns matching pairs plus every unmatched row from both sides, filling the
other subtree's columns with NULL. Duplicates retain their multiplicity; NULL equality keys never
match each other. A row counts as matched only when the complete ON condition is TRUE. WHERE
filters after both sides' NULL extension. Both qualified inputs become nullable, even for declared
NOT NULL columns. An empty side still returns all rows from the other side.

All inputs share the statement's multi-version concurrency control (MVCC) snapshot. Multiplicity
is preserved. Grouping/HAVING, DISTINCT, ordering, pagination and subqueries compose with the joins.
Row order requires ORDER BY. System relations share one captured catalog image.

JOIN binds more tightly than comma. In `FROM a, b JOIN c ON ...`, ON sees b and c, but cannot
see a or a later input. Joins associate to the left within each comma item. Explicit aliases hide
base names and distinguish repeated tables. Unqualified names shared by inputs remain ambiguous
unless USING merges them. These checks apply before rows are read, even on empty inputs.

Parenthesized joins preserve explicit association. Unaliased groups expose their component names;
an aliased group hides them behind its own name, like a derived SELECT *, with an optional complete
column-alias list. Groups may start with an aliased derived SELECT and may nest. CREATE VIEW and
reopen retain that association. RIGHT, FULL and grouped trees keep predicates at their written level.

ON requires a [Boolean search condition](where.md#boolean-search-conditions). A non-Boolean
condition such as `ON t.age` fails before reads with CS02003 (42804); untyped NULL is
UNKNOWN and matches no pair. LEFT then applies its ordinary NULL extension.

### USING columns

USING compares each named pair with ordinary equality and collation precedence; NULL never
matches NULL. It exposes one merged column per name for unqualified lookup and `*`. Wildcard
order is the USING list, remaining left columns, then remaining right columns. Later joins apply
the rule to the accumulated left output; comma items concatenate their visible columns.

Merged columns use PostgreSQL's common-type selection through implicit casts within a type
category: INT/DECIMAL becomes DECIMAL and INT/BIGINT becomes BIGINT in either input order.
Arithmetic and function overloads see that common type. LEFT uses the coerced left value and its
nullability; INNER prefers an unchanged input of the common type when available. Identical domains
retain identity; mixed domain/base or differing domains resolve through base types. Length,
precision and scale survive only when both types and modifiers agree. CHAR(3)/VARCHAR(10), for
example, clears the merged length. Qualified `left.id` and `right.id` retain their original types
and modifiers; the right column becomes nullable under LEFT.

RIGHT uses the coerced right value and its collation/nullability for each merged USING column,
including the right spelling under case-insensitive equality. Merged keys still come first in USING
list order, followed by remaining left and right columns. Qualified references continue to expose
the original physical inputs; the left input becomes nullable under RIGHT.

FULL merges each USING name with COALESCE of the two common-typed inputs. The left value wins
on a match, including its spelling under case-insensitive equality; an unmatched right row supplies
its right value. Merged text uses the equality collation. A merged column is nonnullable only when
both original inputs were nonnullable. Both qualified physical columns become nullable. Written
USING order, common types, identical domain identity and modifier rules remain the same. Computed
merged keys can feed later joins, including parenthesized FULL subtrees.

[ORDER BY](order-by.md) aliases after `*` and numeric ordinals count the visible merged columns.
Qualified wildcards such as `t.*` remain excluded.

### Access paths

Ungrouped INNER/LEFT execution is left-deep; grouped and FULL joins retain their normalized tree.
Only the first INNER pair may swap inputs; bounded probe costs can choose its driver while complete unique-key seeks retain their cheap path. Later steps may seek into the added input,
and LEFT always drives from its preserved left side.
Mandatory equalities can probe an index's leading key prefix under the existing type/collation
rules; the full predicate still filters candidates. Otherwise the added input buffers once.
System inputs have no index paths or object locks. Computed merged keys do not gain index probes.
Both inputs can use selective equality/range restrictions before constructing pairs, including
filtering scanned inputs. Compatible mandatory equijoin equalities propagate exact constants across
INNER inputs and from the preserved to the nullable input of LEFT. Floating keys and incompatible
text types or collations do not infer constants. Complete ON and WHERE predicates still run;
duplicates and mixed numeric comparisons keep their ordinary results. A dropped or replaced index,
including a same-name index with different keys, falls back to a residual-filtered scan.
[EXPLAIN](../statements/explain.md) describes the driving input and each step's scan, seek or hash access.
Bounded probe costs can select among eligible accesses and first-pair directions; a complete unique
lookup retains its cheap path.

### Predicate safety

Only mandatory AND terms whose entire evaluation is proven unable to raise an error can filter
an input early. Eligible forms include comparisons (including IS [NOT] DISTINCT FROM), IS [NOT]
NULL, Boolean tests, BETWEEN, IN and LIKE over columns, constants and supplied parameters with
compatible types and fixed collations. Text comparisons and LIKE require an index-backed
collation; LIKE also requires a validated constant/parameter pattern and escape. Text predicates
over derived outputs and predicates over unknown column types remain at their original level.

Standard identity and widening numeric casts qualify only without target length, precision or
scale modifiers. Narrowing, text-to-number, domain and application casts remain residual.
Text LENGTH/CHAR_LENGTH/CHARACTER_LENGTH and the six captured transaction-clock forms are the only
admitted calls; Immutable or Stable metadata alone does not prove safety. The clock forms use the
same captured instant for seek bounds and runtime filters.

Arithmetic, other unproven calls or scalar forms, row-dependent LIKE patterns/escapes, subqueries,
outer references and deferred failures remain at their original join level. Thus an invalid cast
or zero divisor on a row discarded by the join or an earlier ON/WHERE term raises no new error.
Nullable-side WHERE predicates stay after LEFT's NULL extension; nullable-side ON predicates may
filter that input early. A preserved-side ON predicate cannot remove its unmatched left row.

| Form | Support | Boundary |
| --- | --- | --- |
| INNER, LEFT [OUTER] or RIGHT [OUTER] JOIN | Supported (measured) | Exactly one ON or USING per step; any number of steps |
| FULL [OUTER] JOIN | Supported (measured) | ON/USING must provide a safe mandatory hash equality; complete ON evaluation must be proven non-throwing |
| CROSS JOIN and comma sources | Supported (measured) | Products; CROSS accepts neither ON nor USING |
| NATURAL, LATERAL | Recognized, not supported (`CK00001`) | Outside this join subset |
| Qualified wildcard | Recognized, not supported (`CK00001`) | List qualified columns explicitly |

## Diagnostics

| Code | Meaning and trigger |
| --- | --- |
| `CS02003` | Non-Boolean ON condition or logical operand; SQLSTATE 42804 |
| `CS01015` | Unknown/hidden/out-of-scope qualifier, ambiguous column, missing/repeated/ambiguous USING name, or no common USING type; rejected during planning, including empty inputs |
| `CS01019` | Duplicate table binding or alias |
| `CK00001` | Unsupported join/wildcard, or INNER/LEFT/RIGHT/FULL without ON or USING |
| `CS05069` | Direct RIGHT, FULL or unaliased parenthesized source in UPDATE FROM / DELETE USING; refused before reads/writes, no SQLSTATE |
| `CS05070` | FULL condition lacks a safe mandatory hash equality or complete ON evaluation is not proven non-throwing; before either input is read, no SQLSTATE |
| `CS05067` | Concurrently retained hash builds exceed StatementMemoryBudget; SQLSTATE 53200 |
| `CS00003` | Malformed join/USING list, both ON and USING, or CROSS with either condition |

The source assigns no SQLSTATE to the other join diagnostics in this table.

## Examples

These examples use the [conformance fixture](../statements/select.md#a-create-the-conformance-fixture).

```sql
CREATE TABLE profiles (user_id INT REFERENCES t(id), email TEXT);
INSERT INTO profiles VALUES (1, 'ada@example.test'), (3, 'alan@example.test');
SELECT t.name, p.email
FROM t INNER JOIN profiles p ON t.id = p.user_id
ORDER BY t.id;
```

The result contains Ada's and Alan's matching email addresses.

Reversing the written inputs and restricting the primary key still returns Ada's one match:

```sql
SELECT t.name, p.email FROM profiles p JOIN t ON p.user_id = t.id WHERE t.id = 1;
SELECT t.id FROM t LEFT JOIN profiles p ON t.id = p.user_id WHERE p.user_id IS NULL;
```

The results are `(Ada, ada@example.test)` and `2`, respectively. The second WHERE remains after
NULL extension and selects the unmatched row.

```sql
SELECT s.TABLE_NAME, c.COLUMN_NAME
FROM INFORMATION_SCHEMA.TABLES s
JOIN INFORMATION_SCHEMA.COLUMNS c ON s.TABLE_NAME = c.TABLE_NAME
WHERE s.TABLE_NAME = 'profiles' ORDER BY c.ORDINAL_POSITION;
```

In this fixture the result is `(profiles, user_id)`, `(profiles, email)`.

### LEFT, USING and computed output

Execute each statement separately:

```sql
CREATE TABLE a (id INT NOT NULL);
CREATE TABLE b (id DECIMAL NOT NULL);
CREATE TABLE c (id INT NOT NULL, enabled BOOLEAN);
INSERT INTO a VALUES (1), (2), (3);
INSERT INTO b VALUES (1), (2), (3);
INSERT INTO c VALUES (1, TRUE), (2, FALSE);
SELECT a.id, c.id
FROM a JOIN b ON a.id = b.id
LEFT OUTER JOIN c ON b.id = c.id AND c.enabled = TRUE
ORDER BY a.id;
SELECT *, id / 2 AS ratio FROM a LEFT JOIN b USING (id) ORDER BY ratio;
SELECT COUNT(*) FROM a CROSS JOIN b;
SELECT COUNT(*) FROM a, b;
```

The first query returns `(1, 1)`, `(2, NULL)`, `(3, NULL)`. Moving `c.enabled = TRUE`
to WHERE leaves only `(1, 1)`. The USING query has columns `id`, `ratio`, both Decimal,
and returns `(1, 0.5)`, `(2, 1)`, `(3, 1.5)`. Each product count is `9`.

### RIGHT, USING and groups

Execute separately in a fresh database, adapted from SqlRightJoinTests:

```sql
CREATE TABLE a (left_value TEXT NOT NULL, id INT NOT NULL);
CREATE TABLE b (id INT NOT NULL, right_value TEXT NOT NULL);
INSERT INTO a VALUES ('left', 1);
INSERT INTO b VALUES (1, 'one'), (2, 'two');
SELECT * FROM a RIGHT OUTER JOIN b ON a.id = b.id ORDER BY 3;
SELECT * FROM a RIGHT JOIN b USING (id) ORDER BY id;
SELECT d.id, b.id FROM ((SELECT id FROM a) d RIGHT JOIN b ON d.id = b.id) ORDER BY b.id;
```

The first result's columns are left_value, id, id, right_value, with rows
`('left',1,1,'one')`, `(NULL,NULL,2,'two')`; only the first two columns become nullable.
USING returns columns id, left_value, right_value and rows `(1,'left','one')`, `(2,NULL,'two')`.
The derived-first group returns `(1,1)`, `(NULL,2)`.

Direct RIGHT, FULL or unaliased grouped sources in joined DML fail CS05069. A derived SELECT containing
RIGHT or FULL executes; see [UPDATE FROM](../statements/update.md#from-sources) and
[DELETE USING](../statements/delete.md#using-sources).

EXPLAIN identifies `normalized RIGHT to LEFT` and the chosen hash, indexed or buffered path.
JoinStep nesting follows physical association; both EXPLAIN and EXPLAIN ANALYZE emit the complete
physical outer subtree before its correlated inner IndexSeek, including a derived outer query.
See [EXPLAIN](../statements/explain.md#right-and-grouped-joins).

### FULL, USING and groups

Execute separately in a fresh database, adapted from SqlFullJoinTests:

```sql
CREATE TABLE a (id INT NOT NULL);
CREATE TABLE b (id BIGINT NOT NULL);
CREATE TABLE c (id DECIMAL NOT NULL);
INSERT INTO a VALUES (1);
INSERT INTO b VALUES (2);
INSERT INTO c VALUES (2), (3);
SELECT id, a.id AS aid, b.id AS bid, c.id AS cid
FROM a FULL JOIN (b FULL JOIN c USING (id)) USING (id)
ORDER BY id;
```

The rows are `(1,1,NULL,NULL)`, `(2,NULL,2,2)`, `(3,NULL,NULL,3)`. The merged id is Decimal
and nonnullable; aid, bid and cid are nullable and keep their original physical types.
`SELECT COUNT(*) FROM a FULL JOIN b USING (id)` returns 2. Replacing USING with `ON a.id < b.id`
reports CS05070, even when either table is empty.

## See also

[Clauses](index.md) · [SELECT](../statements/select.md) · [Unsupported syntax](../unsupported.md)

## Hash joins and memory

Eligible mandatory equijoins can choose `HashJoin` by comparing bounded build/probe costs with
nested-loop work. Only the first INNER pair may reverse to build the smaller estimated input;
LEFT preserves its written outer side. Small exact indexed inputs and complete unique probes
retain index nested loops. When both inputs lack countable indexes, eligible equijoins select
hash joins regardless of actual size, even when empty or tiny; both estimates use 4,096 rows.

For INNER/LEFT/RIGHT, unsupported key shapes or predicates whose movement could change error
evaluation retain the loop/probe path. An ON condition referencing any text column (String, Json or Character) from a
derived relation prevents hash selection, including references only in residual terms. This
preserves deferred collation/Unicode errors that skipping nonmatching key pairs could hide.

The hash path retains duplicates and evaluates the complete ON/WHERE residuals. NULL keys do not match; LEFT still emits unmatched outer rows. Type and collation equality rules apply to hashes and collisions.

FULL always builds a hash table from the written right child, even when an index exists. It retains
duplicate and NULL-key build rows independently and tracks a match only after complete ON succeeds.
After probing/null-extending left rows, it emits unmatched build rows once. Binding requires a
mandatory equality with supported hash keys and proves the whole ON condition non-throwing.
Inequality-only or OR-only conditions, arithmetic, unproven calls, subqueries, CHAR-only keys and
derived text without a stored-codec Unicode guarantee fail CS05070. There is no FULL loop fallback.
Proven identity/widening casts and merged USING COALESCE keys can qualify.

FULL match flags, NULL-key rows and overlapping growth arrays share the existing hash-build budget,
including nested builds. Cancellation, timeout, refusal and success release all reservations.
EXPLAIN reports FullHashJoin and `unmatched=both` in JoinStep detail; ANALYZE counts candidate
pairs, excluding separately emitted unmatched rows.

SqlDatabaseEngineOptions.StatementMemoryBudget is a positive byte budget for concurrently retained hash builds in one statement, including nested queries. Reservations account for retained rows, table/array storage and overlapping arrays during growth; all exits release them. Exhaustion reports CS05067 (SQLSTATE 53200) with an unprefixed diagnostic message. There is no partitioning or spill. [CTAS](../statements/create-table.md#create-table-as) also charges retained source results and copy buffers; other result materialization and transient planning remain outside this accounting. See [performance and configuration](../../performance.md).

## Sources

- **FULL semantics, refusal, metadata and budgets** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlFullJoinTests.cs`.

- **Boolean conditions** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlBooleanPredicateTests.cs`.


- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`
- **Implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlJoinExecutionTests.cs`
- **Implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlJoinIndexSemanticsTests.cs`
- **Predicate safety** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlJoinPredicateSafetyTests.cs`
- **N-way, LEFT, USING and output types** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlNWayJoinTests.cs`
- **Syntax boundaries** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlJoinShapeTests.cs`
- **RIGHT and groups** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlRightJoinTests.cs`
- **Physical plan order** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlRightJoinExplainTests.cs`
