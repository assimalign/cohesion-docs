# DROP SEQUENCE (Cohesion SQL)

Removes one or more nonidentity sequences and their allocation state atomically.

## Syntax

```syntaxsql
DROP SEQUENCE [ IF EXISTS ] [ schema_name . ] sequence_name [ , ...n ]
    [ CASCADE | RESTRICT ] [ ; ]
```

## Remarks

RESTRICT is the default. A sequence used by a [column default](../clauses/default.md#sequence-defaults)
refuses with CS03080 (2BP01), naming the dependent default. CASCADE atomically clears dependent
defaults and drops the sequence. An identity-owned sequence refuses even with CASCADE; remove the
identity instead. The full name list validates before one catalog commit. Without IF EXISTS, any missing sequence reports CS03073 (42P01) and none are dropped.
The guard skips absent names, including a disappearance during the Exclusive wait; a replacement
identity reports CS04067 (40001).

The statement self-commits and refuses explicit transactions with CS04003 (25001). Declared
provisioning scripts refuse it with CS03074 (0A000). DROP waits for transactions using the sequence;
see [locking and dynamic names](alter-sequence.md#locking). Wait timeout uses CS04044 (55P03).

Recreation obtains a new object identity and inherits no session currval/lastval state. After
dropping the lastval sequence, lastval reports CS04069 (55000). See
[session functions](../functions/sequences.md).

## Example

Execute separately:

```sql
CREATE SEQUENCE s;
CREATE SEQUENCE other;
DROP SEQUENCE s, other RESTRICT;
SELECT SEQUENCE_NAME FROM INFORMATION_SCHEMA.SEQUENCES;
```

In an otherwise fresh database, the SELECT returns no rows.

## See also

[Statements](index.md) · [CREATE SEQUENCE](create-sequence.md) · [ALTER SEQUENCE](alter-sequence.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlSequenceSqlTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Internal/Execution/SqlPlanExecutor.Sequences.cs`.
