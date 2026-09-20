# DEFAULT (Cohesion SQL)

Declares the literal value used when an insert omits a column.

> **Status:** Partial. The documented subset executes; restrictions are listed below.

## Syntax

```syntaxsql
column_name data_type [ NULL | NOT NULL ] DEFAULT literal
```

## Arguments

- **`literal`** — A supported literal convertible to the declared storage type.

## Remarks

Only literal defaults execute. Parenthesized arithmetic, calls, parameters, `CAST`, and other
expressions are rejected during planning before mutation. The diagnostic names the column and says
only literal `DEFAULT` values are supported.

Bounds include string length and decimal precision/scale. Strings are not truncated and decimals
are not rounded. Approximate floating defaults use normal IEEE conversion rounding, but nonfinite
results and nonzero underflow to zero are rejected.

An omitted column uses the default; explicit `NULL` follows nullability instead. For an added
column, missing trailing fields in old rows resolve from the persisted default without rewriting
those rows. See [ALTER TABLE](../statements/alter-table.md) for empty-table and snapshot details.

## Examples

```sql
CREATE TABLE created (id INT PRIMARY KEY, label TEXT DEFAULT 'new');
INSERT INTO created (id) VALUES (1);
SELECT label FROM created;
```

The result is `new`.

## See also

[Clauses](index.md) · [SELECT](../statements/select.md) · [Unsupported syntax](../unsupported.md)

## Sources

- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`
- **Implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlAddColumnDefaultTypeTests.cs`

