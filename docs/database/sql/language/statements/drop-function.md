# DROP FUNCTION (Cohesion SQL)

Removes a persisted SQL function overload by its parameter signature.

> **Status:** Implemented. Dependent definitions prevent removal.

## Syntax

```syntaxsql
DROP FUNCTION function_name ( [ type_name [ , ...n ] ] ) [ ; ]
```

## Arguments

The unqualified function name and normalized parameter types identify the overload. Type aliases
and function-name casing do not change that identity.

## Remarks

A missing overload reports COHSQLE009 (42883), an existing dependency (including a view)
COHSQLD002 (2BP01), and an engine-registered overload COHSQLD003 (0A000). Another native
overload with the same name does not make an absent signature native. Ordinary DDL cannot drop
schema-owned definitions. IF EXISTS, CASCADE and qualified function names are outside this syntax.
No scalar NULL or MISSING evaluation occurs.

These definitions are database-local and self-committing. DDL inside an explicit transaction
reports COHSQLT003 before publication. After CREATE or DROP commits, mandatory definition refresh
uses CancellationToken.None and returns success despite late caller cancellation or statement-timeout
expiration. A lost connection can still lose that response. Later invocations use their own
statement token. Before commit, execution can fail with COHSQLE043 (57014) or a contended lock wait
with COHSQLE044 (55P03); see [timeouts](../diagnostics.md#statement-and-lock-timeouts).

## Example

From the definition-DDL timeout regression, execute separately:

```sql
CREATE FUNCTION plus_one(x INT) RETURNS INT RETURN x + 1;
DROP FUNCTION plus_one(INT);
SELECT plus_one(41);
```

CREATE and DROP succeed with affected count zero. The later SELECT fails with COHSQLE016 because
the function is absent.

## See also

[CREATE FUNCTION](create-function.md) · [Functions](../functions/index.md) · [Diagnostics](../diagnostics.md)

## Sources

- **Grammar and contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlQueryParser.Definitions.cs` and `docs/DIALECT.md` in that project.
- **Execution** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlDefinedFunctionTests.cs`.
- **Commit boundary** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlTimeoutRegressionTests.cs`.
