# DROP CONSTRAINT

Drops one named rule, preserving other constraints and declared property metadata.

> **Status:** Implemented.

## Syntax

```syntaxsql
DROP CONSTRAINT constraint_name [ IF EXISTS ]
```

## Remarks

Names use the database-wide, ordinal, case-sensitive schema namespace. IF EXISTS suppresses
an absent constraint. A real drop affects 1; a conditional no-op affects 0. Dropping the last
rule retains discovered label/type/property tokens. A schema-owned rule is protected.
The [autocommit policy and DDL limits](create-index.md#transactions-and-limits) apply even
to conditional no-ops: CG04025 aborts an explicit transaction.

An absent unguarded drop uses CG01021; invalid limits use CG08021 and exhausted work/memory
budgets use CG05004/CG05020. These have no assigned SQLSTATE. See [Diagnostics](../diagnostics.md).

## Example

Execute separately in a fresh database:

```sql
CREATE CONSTRAINT required FOR (n:Fresh) REQUIRE n.p IS NOT NULL
DROP CONSTRAINT required
DROP CONSTRAINT required IF EXISTS
INSERT (:Fresh)
```

The DDL affected counts are 1, 1, 0. The insert succeeds: p is known but no longer required.

## See also

[CREATE CONSTRAINT](create-constraint.md) · [SHOW](show.md) · [Statements](index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphSchemaDdlTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/GraphSchema.Ddl.cs`.
