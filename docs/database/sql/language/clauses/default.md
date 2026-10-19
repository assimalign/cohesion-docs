# DEFAULT (Cohesion SQL)

Declares a column default or requests that default for an inserted or updated value.

> **Status:** Implemented for the forms below.

## Syntax

```syntaxsql
column_name data_type [ NULL | NOT NULL ] DEFAULT default_expression
INSERT INTO table_name [ ( column_name [ , ...n ] ) ]
    VALUES ( { expression | DEFAULT } [ , ...n ] ) [ , ...n ]
INSERT INTO table_name [ ( column_name [ , ...n ] ) ] DEFAULT VALUES
UPDATE table_name SET column_name = DEFAULT [ WHERE predicate ]
```

## Remarks

A whole-item `DEFAULT` follows the same path as omitting its destination column. It uses the
column's declared default, or NULL if no default exists and the column allows NULL.
`DEFAULT VALUES` inserts exactly one row, even with an optional column list. Explicit NULL
requests NULL and does not request a default. Typed temporal, interval, binary and national
string [literals](../expressions/literals.md) work in literal declarations.

CREATE TABLE, ADD COLUMN and ALTER COLUMN SET DEFAULT admit column-free scalar expressions,
including arithmetic, CAST and Immutable, Stable or Volatile calls. Columns, parameters and
aggregates fail with `COHSQLE013`; subqueries fail with `COHDBL001`. Result types must satisfy
INSERT assignment compatibility before publication. SQL has NULL values, with no MISSING scalar.

The top-level declaration grammar admits comparisons and IS [NOT] DISTINCT FROM, retaining
following column constraints: `flag BOOLEAN DEFAULT 1 IS DISTINCT FROM 2 NOT NULL`.
Use parentheses for other predicate forms, such as `flag BOOLEAN DEFAULT (NULL IS UNKNOWN)`;
top-level NOT/AND/OR, truth tests and LIKE/IN are left outside the default expression.

Immutable defaults are evaluated and coerced once at declaration. Stable and Volatile calls run
per assigned row, using the transaction-start clock, for omitted INSERT values, INSERT SELECT,
explicit DEFAULT, UPDATE SET DEFAULT and ON DELETE SET DEFAULT. UUID defaults generate a value
for each assignment. Opening a table or rebinding its definitions never evaluates an expression
default. If a registered immutable implementation later fails or returns an invalid value,
reads and writes with explicit values still work; only writes needing that default fail.

ADD COLUMN freezes a nonvolatile default once into an independent missing-value literal for old
rows. A Stable clock default therefore gives old rows the addition's timestamp and later inserts
their own transaction timestamp. A Volatile default requires an empty table; live rows cause
`COHSQLE027`, naming the column, before publication. SET/DROP DEFAULT never changes the frozen
value. Domain, typed DefaultSql and SDK defaults remain literal-only.

Declaration checks assignment types and bounds. A NULL-producing NOT NULL default may be declared;
actual assignments enforce NOT NULL (`COHSQLE024`, SQLSTATE 23502). Immutable and frozen Stable
floating defaults must be finite (`COHSQLE002`, 22003). Other evaluation and conversion failures
retain their [normal codes](../diagnostics.md). DROP DEFAULT re-inherits a domain default;
SET DEFAULT NULL preserves an explicit NULL override when hiding one.

[Identity columns](identity.md) exclude a declared DEFAULT, but omission or a whole INSERT DEFAULT
item allocates a value. BY DEFAULT identity also generates for UPDATE SET id = DEFAULT; ALWAYS
rejects all assignments to the identity column in UPDATE, including DEFAULT (`COHSQLE025`).

A bare DEFAULT in SELECT, a predicate, INSERT SELECT, arithmetic, a function argument, or a
parenthesized scalar expression is `SQL0003`. Quoted or qualified columns named `default`
remain ordinary column references. A NOT NULL failure is `COHSQLE024` (SQLSTATE 23502), naming
the column with no execution-time source span. The failed statement changes no rows and preserves
earlier work in an explicit transaction.

## Examples

Execute each statement separately:

```sql
CREATE TABLE defaults (id INT DEFAULT 7 NOT NULL, label TEXT DEFAULT 'seed', note INT);
INSERT INTO defaults DEFAULT VALUES;
UPDATE defaults SET label = DEFAULT;
SELECT id, label, note FROM defaults;
```

The insert and update each affect one row. SELECT returns `(7, 'seed', NULL)`.
`INSERT INTO defaults VALUES (DEFAULT, DEFAULT, DEFAULT)` produces the same row.

```sql
CREATE TABLE expr_defaults (id INT, n INT DEFAULT 1 + 2, label TEXT DEFAULT UPPER('x'),
    c INT DEFAULT CAST('1' AS INT), uid UUID DEFAULT GEN_RANDOM_UUID());
INSERT INTO expr_defaults (id) VALUES (1), (2);
SELECT id, n, label, c FROM expr_defaults ORDER BY id;
```

The result is `(1, 3, 'X', 1)`, `(2, 3, 'X', 1)`; the two stored UUIDs differ.

A Stable default shares the transaction clock across row assignments. Execute separately:

```sql
CREATE TABLE clock_defaults (id INT, stamp TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP);
BEGIN;
INSERT INTO clock_defaults (id) VALUES (1), (2);
SELECT id, stamp = CURRENT_TIMESTAMP AS same_instant FROM clock_defaults ORDER BY id;
COMMIT;
```

SELECT returns `(1, TRUE)`, `(2, TRUE)`.

```sql
CREATE TABLE fast_default (id INT);
INSERT INTO fast_default VALUES (1);
ALTER TABLE fast_default ADD stamp TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP;
ALTER TABLE fast_default ALTER stamp DROP DEFAULT;
INSERT INTO fast_default (id) VALUES (2);
SELECT id, stamp IS NULL AS absent FROM fast_default ORDER BY id;
ALTER TABLE fast_default ADD uid UUID DEFAULT GEN_RANDOM_UUID();
```

SELECT returns `(1, FALSE)`, `(2, TRUE)`: the old row retains its frozen timestamp after DROP
DEFAULT. The final addition fails COHSQLE027 because rows exist, leaving the schema unchanged.

## See also

[INSERT](../statements/insert.md) · [UPDATE](../statements/update.md) · [ALTER TABLE](../statements/alter-table.md) · [Diagnostics](../diagnostics.md)

## Sources

- **Expression defaults** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlExpressionDefaultTests.cs`.
- **Execution and atomicity** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlDefaultAssignmentTests.cs`.
- **Typed declarations** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlTypedLiteralTests.cs`.
- **Default contexts** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/docs/DESIGN.md`.
