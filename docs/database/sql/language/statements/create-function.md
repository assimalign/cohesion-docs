# CREATE FUNCTION (Cohesion SQL)

Persists a database-local scalar function with one SQL expression body.

> **Status:** Partial. Expression bodies execute; procedural bodies and CREATE OR REPLACE are excluded.

## Syntax

```syntaxsql
CREATE FUNCTION function_name ( [ parameter_name type_name [ , ...n ] ] )
RETURNS type_name
[ { IMMUTABLE | STABLE | VOLATILE } ]
[ { STRICT | CALLED ON NULL INPUT } ]
RETURN expression [ ; ]
```

## Arguments

- **function_name** — An unqualified name in the shared SQL/native function namespace.
- **parameter_name, type_name** — Named parameters and their types; DROP identifies the overload by parameter types.
- **expression** — One scalar expression over parameters and compatible function calls.

## Remarks

Defaults are VOLATILE and CALLED ON NULL INPUT. STRICT returns NULL without evaluating the body
when an argument is NULL, after domain argument coercion/checks. Domain NOT NULL and check rules
also apply to function results. SQL has no MISSING value. The invocation's effective collation
flows into parameters and the body.

Tables, subqueries, aggregates, external parameters and recursion are refused. A function cannot
call a more volatile function; CHECKs admit only Immutable calls. Invalid definitions use
CS03301 (no SQLSTATE); duplicate signatures use CS03065 (42723), and missing/unresolved
implementations use CS01009 (42883). Unknown types use CS02010 (42704); incompatible calls use
CS01006 (42883). Domain check failures use CS03038 (23514). There is no LANGUAGE clause,
qualified function name or replacement form. Persisted call identities retain their resolved overload.

These definitions are database-local and self-committing. DDL inside an explicit transaction
reports CS04003 before publication. After CREATE or DROP commits, mandatory definition refresh
uses CancellationToken.None and returns success despite late caller cancellation or statement-timeout
expiration. A lost connection can still lose that response. Later invocations use their own
statement token. Before commit, execution can fail with CS05043 (57014) or a contended lock wait
with CS04044 (55P03); see [timeouts](../diagnostics.md#statement-and-lock-timeouts).

## Example

Execute each statement separately in a fresh database, as in SqlTimeoutRegressionTests:

```sql
CREATE FUNCTION plus_one(x INT) RETURNS INT RETURN x + 1;
SELECT plus_one(41);
```

CREATE succeeds with affected count zero; SELECT returns 42. With IMMUTABLE STRICT appended before
RETURN, calling plus_one(NULL) returns NULL without running its body.

## See also

[DROP FUNCTION](drop-function.md) · [Functions](../functions/index.md) · [Diagnostics](../diagnostics.md)

## Sources

- **Grammar and contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlQueryParser.Definitions.cs` and `docs/DIALECT.md` in that project.
- **Execution** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlDefinedFunctionTests.cs`.
- **Commit boundary** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlTimeoutRegressionTests.cs`.
