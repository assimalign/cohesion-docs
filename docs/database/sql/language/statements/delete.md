# DELETE (Cohesion SQL)

Removes rows selected from a stored table.

> **Status:** Partial. The documented subset is measured against the live SQL engine.

## Syntax

```syntaxsql
[ WITH common_table_expression [ , ...n ] ]
DELETE FROM [ schema_name . ] table_name
[ WHERE search_condition ]
[ RETURNING { * | expression [ [ AS ] column_alias ] } [ , ...n ] ]
[ ; ]
```

## Arguments

- **`table_name`** — The stored table from which rows are removed.
- **`search_condition`** — An optional predicate; omission targets all rows.

## Remarks

[WITH](../clauses/with.md) binds non-recursive query definitions and materializes reachable
definitions once under the statement snapshot, before any write. Targets remain stored tables.

Qualified references use the target name, or its alias when supplied. An explicit alias hides
the base name; hidden or unknown qualifiers report located `COHSQLE015` before any write.

Only rows for which `WHERE` is `TRUE` are selected. The statement reports an affected count.
Foreign keys default to `ON DELETE NO ACTION`; a declared `ON DELETE CASCADE` removes dependent
rows. Nullable foreign-key values are allowed.

Correlated and uncorrelated scalar, IN/NOT IN and EXISTS/NOT EXISTS [subqueries](../expressions/subqueries.md#update-and-delete-snapshots)
execute in WHERE. Independent children materialize once; correlated children evaluate for each
candidate row before any deletion. Both use pre-statement values, including self-reads; the session's earlier uncommitted changes are visible. Rollback restores rows and
cascades together. [RETURNING](../clauses/returning.md) projects old values of directly targeted
rows and excludes cascade/child-action rows. Deleting rows does not remove the
table definition; [DROP TABLE](drop-table.md) is the separate schema operation.

[SET NULL and SET DEFAULT](../constraints.md) update surviving children through cascade chains.
Defaults use the delete transaction clock; child validation and index changes are atomic with deletion.

## Examples

Starting from the [conformance fixture](select.md#a-create-the-conformance-fixture):

```sql
DELETE FROM t WHERE id = 2;
SELECT id FROM t ORDER BY id;
```

The delete affects one row; the query returns `1` and `3`.

## See also

[WHERE](../clauses/where.md) · [Constraints](../constraints.md) · [ROLLBACK](rollback-transaction.md)

[Statements](index.md) · [Language (SQL)](../index.md)

## Sources

- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`
