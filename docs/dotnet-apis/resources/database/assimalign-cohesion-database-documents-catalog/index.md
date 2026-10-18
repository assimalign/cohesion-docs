# Assimalign.Cohesion.Database.Documents.Catalog

`Assimalign.Cohesion.Database.Documents.Catalog` supplies the sealed `DocumentCatalog` over the same `DocumentStorage` file set as document content.

> **Status:** Partial.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.
- **[DocumentCatalog](document-catalog.md)** — Composite build and prefix/range seek overloads.
- **[DocumentIndexMetadata](document-index-metadata.md)** — Immutable ordered paths and UNIQUE flag.
- **[DocumentConstraintViolationException](document-constraint-violation-exception.md)** — UNIQUE write/build refusals.

`Assimalign.Cohesion.Database.Documents.Catalog` supplies the sealed `DocumentCatalog` over the same `DocumentStorage` file set as document content. It stores collection
ownership, versioned document identities/content references, and secondary index definitions. Its
B+Trees come from `Database.Indexing`, and every data/index mutation joins the caller's
`ITransactionContext` through `TransactionCoordinator`.

`DocumentCatalog.Open(storage, coordinator)` opens the metadata directory and persisted physical
trees. `Read` methods accept an explicit visibility snapshot. Mutation methods neither begin nor
commit the caller's transaction. The engine takes the logical database's exclusive writer lock,
rejects write conflicts and stale index definitions, and enforces schema ownership before calling
the catalog.

`CreateIndexAsync` builds a single/composite, optionally UNIQUE index immediately. All index records
are v2; v1 is refused before recovery writes with a recreate remedy and no upgrade. `SaveDocumentAsync` and
`DeleteDocumentAsync` maintain every visible index alongside metadata writes; queries do not rebuild
indexes. The Documents engine reaches index creation and deletion through planned OQL `CREATE INDEX`
and `DROP INDEX` statements; these catalog methods remain lower-level transactional composition
seams. `SearchIndexAsync` performs a B+Tree equality or range seek and resolves only
snapshot-visible document versions, ordered by ordinal identity. See [DESIGN.md](design.md) for the
format and the precise path/scalar semantics.

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Database` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Documents.Storage` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Storage` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Transactions` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Indexing` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Types` | `CohesionProjectReference` |

[Parent: Database](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Catalog/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Catalog/src/Assimalign.Cohesion.Database.Documents.Catalog.csproj`.
