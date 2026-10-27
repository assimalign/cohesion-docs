# Assimalign.Cohesion.Database.Sql.Schema design

This page describes the boundaries and implementation shape of `Assimalign.Cohesion.Database.Sql.Schema`.

> **Status:** Partial.

## SQL namespace target

Code-first table/constraint/index/domain provisioning remains pinned to dbo. The builder has no
target-namespace setting. SQL database creation seeds dbo and DROP SCHEMA dbo is CS03031.
[CREATE SCHEMA](../../../../database/sql/language/statements/create-schema.md) creates a SQL
namespace independently of a compiled schema's ownership identity; it does not redirect provisioning.
Storage format 27 retains namespaces as record kind 15, introduced in format 21; compiled schema artifacts remain version 8.

Declared scripts cannot DROP SCHEMA CASCADE over typed objects: admission reports CS03205
(no assigned SQLSTATE) before opening/writing. Declare those changes in the typed schema.

This package owns SQL schema declarations, immutable compiled relational shapes, canonical JSON,
validation, and migration planning. Feature A3 of DATABASE_MVP_FEATURES.md
(`cohesion/docs/programs/DATABASE_MVP_FEATURES.md`) moves that vocabulary out of the model-agnostic
`Database` area root.

The package directly references only `Assimalign.Cohesion.Database` and
`Assimalign.Cohesion.Database.Types`. The SDK's build task can compile a schema through this seam
without taking a dependency on the SQL engine, SQL storage, Connections.Tcp, or Hosting. The area's
root independently composes its generic child roots; this package does not invert that direction.

`SqlSchema.Compile` is the ordinary composition-root entry point: it declares and compiles a SQL
schema without making the caller repeat `EngineModel.Sql`. It delegates to the same
`SqlSchema.Create` and `SqlSchemaCompiler` path, so validation, canonical documents, and hashes
remain identical. `Create` still returns an `ISqlSchema`, built by internal implementations of
`ISqlSchemaBuilder` and the table/type/principal builder contracts, for build tooling and callers
that compile a declaration they did not author. `SqlSchemaCompiler` accepts only `EngineModel.Sql`
and lowers that retained C# declaration into `SqlCompiledSchema`. The derived type carries SQL
tables, columns, keys, indexes, constraints, types, functions, triggers, principals, grants, and
extensions. The root `CompiledSchema` carries only identity and a canonical document, with SHA-256
hashing shared across models.

Tables, indexes, and constraints in a compiled schema always report `DatabaseObjectOwner.Schema`:
applying them is code-first provisioning, and session DDL must not mutate the resulting catalog
objects. The SQL engine persists ownership and enforces the lock; this package requires no engine
implementation to express that contract. Ad-hoc catalog objects remain fully mutable through session
statements.

The removed `CompiledSchemaCollection` and collection planning do not belong to SQL. The document
model will define its own shape in its own model family. There is no collection member or
compatibility placeholder on the SQL schema.

Canonical JSON uses source-generated System.Text.Json metadata. Constructors snapshot inputs and
sort semantic sets; column order remains significant. `CanonicalDocument` and `Hash` are excluded
from their own serialization. Validation rejects malformed input, unknown JSON members, incompatible
models, duplicate declarations, missing references, and unsupported value types.

`SqlSchemaMigrationPlanner` produces deterministic dependency-ordered operations and requires the
declaration's destructive-change opt-in for data-losing steps. Unsupported metadata changes fail
explicitly. The model-independent apply contract and `SchemaMigrationResult` stay in the root.

Foreign keys and checks are executable schema metadata. The planner first creates all tables and
their indexes, then emits `AddConstraint` operations rendered as `ALTER TABLE ... ADD CONSTRAINT`.
This permits references to tables that sort later, self-references, and cycles without temporarily
disabling enforcement. `Constraint` replacement emits `DropConstraint` before removing dependent
columns or indexes, then adds the replacement after structural changes. Both constraint operations
have compensating statements, and the SQL provisioner includes their definitions when comparing or
reconstructing the live catalog after a failed apply.

The engine validates new CHECK table/schema qualifiers against the constrained table with
CS01015. Already stored checks and already-applied declared/imperative schema hashes keep
their earlier qualifier binding. Build validates other rules before I/O; provisioning checks
qualifiers for a new or changed hash after reading catalog state and before migration steps.
See [CHECK binding](../../../../database/sql/language/constraints.md#check-binding).

`CompiledSchemaConstraint.OnDelete` uses `CompiledSchemaReferentialAction.Restrict` by default and
can select `Cascade`. The default is omitted from canonical JSON, preserving hashes of existing
documents that implicitly restricted parent deletes. The retained C# `References` builder keeps that
default; its public interface is unchanged. The compiled concrete model carries the optional action
directly.

`UNIQUE` is represented by `CompiledSchemaIndex(IsUnique: true)`, consistently with the SQL
language and catalog. A unique index supplies both enforcement and lookup, so a second constraint
kind would duplicate its identity and persistence rules. Foreign keys and checks remain
`CompiledSchemaConstraint` values. `For` a check, `Expression.CanonicalText` is SQL scalar-expression
text, for example `qty > 0`; the SQL parser and evaluator validate it at provisioning. Its optional
`Columns` list is advisory and is not part of check equivalence, because table-level SQL checks
derive their dependencies from the expression. Function and trigger bodies retain their separate
compiler-produced expression representation. The frozen retained table builder has no check
declaration member; callers construct the compiled check model directly without adding an interface
member.

The package targets `net10.0`, `LangVersion=Preview`, and is AOT-compatible. The compiler reads
statically supplied expression nodes and their type metadata; it does not discover or invoke members
with reflection, compile expressions, scan assemblies, or activate code. Method signatures are
derived from the expression's argument and result nodes. Version-free type identities are parsed
from the statically supplied assembly-qualified identity string, preserving nested generic arguments
and array suffixes without reflective type discovery. Co-located Shouldly tests cover the retained
declarations, canonical documents, ownership, validation, and migration planning.

## Declared dependencies

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Database` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Types` | `CohesionProjectReference` |

[Assembly overview](index.md) · [Examples](examples/index.md)

## Current format boundary

`SqlCompiledSchema.CurrentFormat` is `cohesion/database-schema/v8`. Typed TimeSpan defaults
render exact signed total seconds plus `seconds`, replacing CLR day-dot-clock text and changing
the canonical document and hash. The renderer preserves every tick, including TimeSpan.MinValue.
Earlier artifacts, applied markers and pending targets fail with SqlSchemaValidationException,
InvalidDocument at `schema.format`, before hash comparison. Rebuild them and follow the
[catalog export/recreate procedure](../../../../database/sql/index.md#sql-format-27-and-schema-v8).

For example, the typed-default tests check `table.Default(row => row.Value, TimeSpan.FromTicks(1))`
against `table.DefaultSql(row => row.Value, "'0.0000001 seconds'")`: their v8 documents and hashes
are equal. TimeSpan.MinValue renders `'-922337203685.4775808 seconds'`; zero renders `'0 seconds'`.

Compiled schemas have no identity representation. Reconciliation of a typed schema-owned table
containing identity metadata is refused before changes with SqlSchemaMigrationException,
`CS03105` and "Nothing was changed." SQL script-created tables retain their SQL definitions.
Compiled domain declarations named serial, bigserial or smallserial fail validation with
SqlSchemaValidationErrorCode.UnsupportedType and an IDENTITY suggestion. Native SqlType.CreateDomain
throws ArgumentException; SQL CREATE DOMAIN reports CK00001, including quoted names.

The integration branch uses SQL row-storage format 27 and compiled schema version 8.
Format 17 introduced the insert-default/frozen-missing-value split (table extension 6); format 18
separately changes quoting for newly executable function names in persisted canonical SQL.
The formats preserve resolved shared type identity/modifiers across contextual multi-word and
SQL Server lexical aliases. Typed temporal, interval and binary defaults use canonical literal
text. These unreleased formats have no older-format compatibility reader; use this current
boundary when interpreting historical layout notes above.

Format 19 quotes PERCENT and TIES in persisted SQL. Format 20 adds persisted SET NULL,
SET DEFAULT and NO ACTION delete actions. Format 21 introduced namespace records of kind 15. Format 22 quotes STRING_AGG and LISTAGG
identifiers in persisted SQL. Format 23 introduced catalog views and format 24 explicit NULL
placement. Format 25 added distinct predicates, Boolean tests and LIKE ESCAPE.
Format 26 fenced temporal DEFAULT and frozen missing-value text.
Current format 27 records TYPE default assignment coercion in table extension 7.
Every earlier format is refused at open; there is no
upgrade path. Compiled schema format remains version 8.

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Schema/docs/DESIGN.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Schema/src/Assimalign.Cohesion.Database.Sql.Schema.csproj`.
