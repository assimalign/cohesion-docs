# DROP INDEX

Drops a named graph index and its physical tree, retaining discovered label/property tokens.

> **Status:** Implemented.

## Syntax

```syntaxsql
DROP INDEX index_name [ IF EXISTS ]
```

## Remarks

Names use the database-wide, ordinal, case-sensitive schema namespace. IF EXISTS suppresses
an absent index. A real drop affects 1; a conditional no-op affects 0. Dropping a schema-owned
index is refused. The [autocommit policy and DDL limits](create-index.md#transactions-and-limits)
apply even to conditional no-ops: CG04025 aborts an explicit transaction.

An absent unguarded drop uses CG01021; invalid limits use CG08021 and exhausted work/memory
budgets use CG05004/CG05020. These have no assigned SQLSTATE. See [Diagnostics](../diagnostics.md).

## Example

Execute separately in a fresh database:

```sql
CREATE INDEX ix FOR (n:Fresh) ON (n.p)
DROP INDEX ix
DROP INDEX ix IF EXISTS
SHOW INDEXES
```

Affected counts are 1, 1, 0. SHOW returns no rows; Fresh and p remain known tokens.

## See also

[CREATE INDEX](create-index.md) · [SHOW](show.md) · [Statements](index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphSchemaDdlTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/GraphSchema.Ddl.cs`.
