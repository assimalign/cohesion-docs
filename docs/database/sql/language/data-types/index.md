# Data types

Maps declared SQL type names to the shared Database type identities.

> **Status:** Partial. The declared subset and its execution limits are documented below.

## Syntax

```syntaxsql
<data_type> ::= {
    BOOLEAN | BOOL | TINYINT | SMALLINT | INT2
  | INT | INTEGER | INT4 | BIGINT | INT8
  | REAL | FLOAT4 | FLOAT | FLOAT8 | DOUBLE
  | DECIMAL [ ( precision [ , scale ] ) ]
  | NUMERIC [ ( precision [ , scale ] ) ]
  | CHAR [ ( length ) ] | CHARACTER [ ( length ) ] | VARCHAR [ ( length ) ] | TEXT
  | BINARY | VARBINARY | BLOB | BYTEA
  | DATE | TIME | TIMESTAMP | DATETIME | TIMESTAMPTZ | INTERVAL
  | UUID | GUID | JSON | JSONB
}
```

## Arguments

- **`precision` and `scale`** — Decimal metadata; one argument denotes precision.
- **`length`** — String length metadata.
- **Type name** — A case-insensitive name resolved by `SqlTypeNames`.

## Remarks

| SQL names | Shared identity |
| --- | --- |
| `BOOLEAN`, `BOOL` | `Boolean` |
| `TINYINT` | `Int8` |
| `SMALLINT`, `INT2` | `Int16` |
| `INT`, `INTEGER`, `INT4` | `Int32` |
| `BIGINT`, `INT8` | `Int64` |
| `REAL`, `FLOAT4` | `Float32` |
| `FLOAT`, `FLOAT8`, `DOUBLE` | `Float64` |
| `DECIMAL`, `NUMERIC` | `Decimal` |
| `CHAR`, `CHARACTER`, `VARCHAR`, `TEXT` | `String` |
| `BINARY`, `VARBINARY`, `BLOB`, `BYTEA` | `Binary` |
| `DATE` | `Date` |
| `TIME` | `Time` |
| `TIMESTAMP`, `DATETIME` | `DateTime` |
| `TIMESTAMPTZ` | `DateTimeOffset` |
| `INTERVAL` | `TimeSpan` |
| `UUID`, `GUID` | `Guid` |
| `JSON` | `Json` |
| `JSONB` | `JsonBinary` |

Name resolution promises a shared identity, not every conversion or operation on that identity.
Planner/executor coercion is a separate concern. In particular, [CAST](../expressions/cast.md)
supports only the exact numeric, Boolean, and string conversion pairs documented on its page.
The SQL wire scalar codec does not support `Json` or `JsonBinary` components despite their
presence in the shared identity set.

`INFORMATION_SCHEMA.COLUMNS.DATA_TYPE` reports canonical names rather than the original alias.
For example, integer aliases report `INTEGER`, decimals report `NUMERIC`, and string aliases
report `CHARACTER VARYING`. These metadata strings do not extend the accepted type-name grammar.
Length, precision, and scale appear separately where the catalog retains them.

Shared identity, collation, and binary encoding come from
`Assimalign.Cohesion.Database.Types`. SQL comparison includes documented approximate-numeric
rules that need not be identical to raw physical index-key ordering.

## Examples

This declaration comes from the parser conformance corpus:

```sql
CREATE TABLE IF NOT EXISTS users (
    id BIGINT PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    balance DECIMAL(18, 4) DEFAULT 0,
    created TIMESTAMP
);
```

## See also

[Language (SQL)](../index.md) · [CAST](../expressions/cast.md) · [Literals](../expressions/literals.md) · [INFORMATION_SCHEMA](../system-views/information-schema.md)

## Sources

- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`
- **Implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlTypeNames.cs`
- **Implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlConformanceTests.cs`
- **Implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Types/docs/OVERVIEW.md`
- **Implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/docs/WIRE-PROTOCOL.md`
