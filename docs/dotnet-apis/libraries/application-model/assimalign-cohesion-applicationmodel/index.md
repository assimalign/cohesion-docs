# Assimalign.Cohesion.ApplicationModel

The resource identity every plane of the application model shares: provider and resource names, their ids, and platform names.

> **Status:** Implemented.

## Reference pages

- **[Design](design.md)** — Placement rule, naming, and dependency boundaries.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.

A resource takes three forms: its definition, which a gateway's model declares; its deployment, a
gateway's view of it across its instances; and each instance, one running copy. This assembly holds
the names all three share, so the logical model, the gateway, and every instance carry the same
identity without loading each other's code.

## Public types

| Type | Role |
|---|---|
| `IApplicationResource` | The base of every form of a resource: `Provider`, `Name`, and the `Id` derived from them. |
| `ProviderName`, `ResourceName` | What a resource is (its provider, such as `Web`) and which one it is (`orders-api`). |
| `ProviderId`, `ResourceId` | Version 5 UUIDs derived from the names. |
| `PlatformName` | A platform, such as `inprocess` or `kubernetes`: what a gateway deploys onto and which client an instance reaches its gateway through. |

## Usage

```csharp
using Assimalign.Cohesion.ApplicationModel;

ResourceId id = ResourceId.FromName("Web", "orders-api");
PlatformName platform = "kubernetes";
```

## Dependencies

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Core` | `CohesionProjectReference` |

The hyphenated-words name grammar in `shared/Names/` is compiled into this assembly, Protocol, and
Definition as shared source; there is no public helper.

[Parent: ApplicationModel](../index.md)

## Sources

- **Primary source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel/docs/OVERVIEW.md`.
- **Design** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel/docs/DESIGN.md`.
- **Project** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel/src/Assimalign.Cohesion.ApplicationModel.csproj`.
