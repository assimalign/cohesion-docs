# DROP DOMAIN (Cohesion SQL)

Removes a persisted SQL domain with no remaining dependencies.

> **Status:** Implemented. Dependent definitions prevent removal.

## Syntax

```syntaxsql
DROP DOMAIN domain_name [ ; ]
```

## Arguments

**domain_name** is the unqualified name in the shared SQL/native type namespace.

## Remarks

A missing domain reports CS03301, a dependent column/definition/view CS03302, and an
engine-registered type CS03303 (0A000). CS03302 maps to 2BP01; CS03301 has no assigned
SQLSTATE. Ordinary DDL cannot
drop schema-owned definitions. IF EXISTS and CASCADE are outside this syntax. Dropping a domain
performs no scalar NULL or MISSING evaluation.

These definitions are database-local and self-committing. DDL inside an explicit transaction
reports CS04003 before publication. After CREATE or DROP commits, mandatory definition refresh
uses CancellationToken.None and returns success despite late caller cancellation or statement-timeout
expiration. A lost connection can still lose that response. Later invocations use their own
statement token. Before commit, execution can fail with CS05043 (57014) or a contended lock wait
with CS04044 (55P03); see [timeouts](../diagnostics.md#statement-and-lock-timeouts).

## Example

The definition-DDL timeout regression creates then drops an unused domain:

```sql
CREATE DOMAIN positive AS INT CHECK (VALUE > 0);
DROP DOMAIN positive;
SELECT CAST(1 AS positive);
```

CREATE and DROP succeed with affected count zero; the later CAST fails with CS02010 (42704)
because the type is absent. A column using positive would instead block the DROP with CS03302.

## See also

[CREATE DOMAIN](create-domain.md) · [Data types](../data-types/index.md) · [Diagnostics](../diagnostics.md)

## Sources

- **Grammar and contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlQueryParser.Definitions.cs` and `docs/DIALECT.md` in that project.
- **Execution** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlDefinedFunctionTests.cs`.
- **Commit boundary** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlTimeoutRegressionTests.cs`.
