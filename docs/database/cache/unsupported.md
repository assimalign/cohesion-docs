# Unsupported surface

The Cache scaffold defines no executable engine or command-language contract.

> **Status:** Not yet implemented.

`Assimalign.Cohesion.Database.Cache.Language` contains a project declaration but no language
implementation. A package name alone does not establish accepted commands, grammar, or
compatibility with another cache protocol. There is no source-grounded syntax diagram or
command example to publish for this model.

| Surface | Current evidence |
|---|---|
| Engine, persistence, lifecycle | Explicitly unimplemented in the Cache design |
| Command parser and grammar | No implementation in `Cache.Language` |
| Catalog | Project scaffold |
| Client | Project scaffold |
| Storage binding | Project scaffold |
| Runtime tests | Placeholder test; no behavior contract |

The empty class and test do not specify eviction, expiration, consistency, or wire behavior.
The design defers the model and requires future work to compose the Database kernel; it does
not provide a delivery date or detailed architecture.

For an implemented byte-key surface, see [Key-Value Pair](../key-value-pair/index.md).
That model's own expiration feature is also deferred, as documented in its
[unsupported features](../key-value-pair/unsupported.md).

## See also

- **[Cache](index.md)** — package status and navigation.
- **[Database](../index.md)** — other models.

## Sources

- **Scope** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Cache/docs/OVERVIEW.md`.
- **Design** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Cache/docs/DESIGN.md`.
- **Test scaffold** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Cache/tests/UnitTest1.cs`.
- **Language scaffold** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Cache.Language/src/Assimalign.Cohesion.Database.Cache.Language.csproj`.
- **Key-Value Pair scope** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/docs/DESIGN.md`.
