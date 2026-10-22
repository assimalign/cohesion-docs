# CREATE DOMAIN (Cohesion SQL)

Persists a database-local domain over a built-in scalar type.

> **Status:** Partial. Literal defaults, NOT NULL and one immutable CHECK execute.

## Syntax

```syntaxsql
CREATE DOMAIN domain_name AS base_type
[ DEFAULT literal ] [ NOT NULL ]
[ [ CONSTRAINT constraint_name ] CHECK ( search_condition ) ] [ ; ]
```

## Arguments

- **domain_name, base_type** — An unqualified domain name and built-in base type; domain-over-domain is excluded.
- **literal** — The default inherited by columns unless they specify their own default.
- **search_condition** — A Boolean immutable expression using VALUE for the assigned value.

## Remarks

Domain bounds, NOT NULL and checks apply to writes, CAST and function domain arguments/results.
CHECK rejects FALSE and accepts TRUE or UNKNOWN. A nullable domain therefore accepts NULL when
its check yields UNKNOWN; NOT NULL rejects it. A column's explicit DEFAULT NULL overrides default
inheritance. SQL has no MISSING value.

Invalid, duplicate or volatility-incompatible domain definitions use COHSQLD001 (no SQLSTATE).
The SQL binder rejects an existing domain name before catalog publication; low-level catalog
non-function definition collisions instead use COHSQLE050 (42710).
Non-Boolean CHECK uses COHSQLE003 (42804); a valid check rejecting a value uses COHSQLE038 (23514).
SERIAL/BIGSERIAL/SMALLSERIAL domain names, including quoted forms, use COHDBL001; use identity
columns for generation. Domain defaults are literal-only. See [types](../data-types/index.md) and
[constraints](../constraints.md) for assignment rules.

These definitions are database-local and self-committing. DDL inside an explicit transaction
reports COHSQLT003 before publication. After CREATE or DROP commits, mandatory definition refresh
uses CancellationToken.None and returns success despite late caller cancellation or statement-timeout
expiration. A lost connection can still lose that response. Later invocations use their own
statement token. Before commit, execution can fail with COHSQLE043 (57014) or a contended lock wait
with COHSQLE044 (55P03); see [timeouts](../diagnostics.md#statement-and-lock-timeouts).

## Example

Execute separately in a fresh database, following SqlTimeoutRegressionTests:

```sql
CREATE DOMAIN positive AS INT CHECK (VALUE > 0);
CREATE TABLE uses_domain (v positive);
INSERT INTO uses_domain VALUES (1), (NULL);
SELECT v FROM uses_domain;
```

CREATE DOMAIN succeeds with affected count zero. SELECT returns 1 and NULL in unspecified order.
A later INSERT of 0 fails with COHSQLE038 (23514); the two rows remain.

## See also

[DROP DOMAIN](drop-domain.md) · [Data types](../data-types/index.md) · [CAST](../expressions/cast.md)

## Sources

- **Grammar and contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlQueryParser.Definitions.cs` and `docs/DIALECT.md` in that project.
- **Execution** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlDefinedFunctionTests.cs`.
- **Commit boundary** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlTimeoutRegressionTests.cs`.
