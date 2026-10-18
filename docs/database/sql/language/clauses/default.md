# DEFAULT (Cohesion SQL)

Declares a column default or requests that default for an inserted or updated value.

> **Status:** Implemented for the forms below.

## Syntax

```syntaxsql
column_name data_type [ NULL | NOT NULL ] DEFAULT literal
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

`CREATE TABLE` also admits column-free scalar function defaults; they execute for each omitted
value. `ALTER TABLE ADD COLUMN` and code-first literal defaults remain literal-only. CAST,
parameters and subqueries do not become valid default declarations through these DML forms.
Added-column literal defaults resolve missing trailing fields in old rows without rewriting them.

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

## See also

[INSERT](../statements/insert.md) · [UPDATE](../statements/update.md) · [ALTER TABLE](../statements/alter-table.md) · [Diagnostics](../diagnostics.md)

## Sources

- **Execution and atomicity** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlDefaultAssignmentTests.cs`.
- **Typed declarations** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlTypedLiteralTests.cs`.
- **Default contexts** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/docs/DESIGN.md`.
