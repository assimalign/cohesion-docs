# Assimalign.Cohesion.Database.Types

The shared scalar type system of the Cohesion Data Platform: type identity (`DatabaseType`, `DatabaseTypeInfo`), explicit string collation (`Collation`), and order-preserving binary key encodings (`DatabaseKeyWriter` / `DatabaseKeyReader`).

> **Status:** Partial.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.

The shared scalar type system of the Cohesion Data Platform: type identity (`DatabaseType`,
`DatabaseTypeInfo`), explicit string collation (`Collation`), and order-preserving binary key
encodings (`DatabaseKeyWriter` / `DatabaseKeyReader`). Anything that ends up inside an index key or
a stored, comparable value goes through this project so ordering is consistent across every database
model.

## Scope

- **Type identity** — the `DatabaseType` scalar set (boolean, integers, floats,
  decimal, string, binary, date/time family, GUID, JSON) and `DatabaseTypeInfo`
  constraints (length/precision/scale).
- **`Collation`** — explicit, named, persisted-by-id string ordering rules:
  `Collation.Binary` (code-point order), `CaseInsensitive` (Unicode simple case fold),
  and `CaseAccentInsensitive` (canonical decomposition, mark removal, case fold).
  Their Unicode 17.0 transforms are pinned and independent of runtime globalization.
  Legacy `Collation.Invariant` retains linguistic scan comparison but is explicitly
  **not index-backed** and rejects invariant-globalization operation.
- **`Key` encodings** — `DatabaseKeyWriter` builds self-describing composite keys whose
  unsigned byte-wise comparison equals component-by-component value comparison;
  `DatabaseKeyReader` decodes them back (folding collations return canonical text;
  original spelling stays in value storage). `Database.Indexing`'s `IndexKey`
  and every model's key convention consume these.
- **Boxed-value bridge** — `DatabaseValueCodec` maps boxed runtime values onto the
  same component encoding (dispatch by runtime type, read back boxed). The wire
  protocol's parameter and result-row payloads go through it on both the server and
  client sides.

## Dependencies

None — a leaf kernel project. Consumers: `Database.Indexing` (key composition), `Database.Execution`
(value typing), per-model catalogs and planners.

## Usage

See the [source-backed usage examples](examples/index.md).

See [DESIGN.md](design.md) for the encoding rules and the decisions behind them.

[Parent: Database](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Types/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Types/src/Assimalign.Cohesion.Database.Types.csproj`.
