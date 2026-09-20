# Assimalign.Cohesion.ObjectValidation

Runs fluent validation profiles and reports structured failures.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[ObjectValidation](../index.md)

## Scope

Profiles separate rule declaration from execution. Member selectors are inspected through resolved
metadata instead of compiling expressions, while conditional predicates are supplied as delegates.
Options control failure aggregation and throwing without changing how profiles are authored.

## Dependencies

The project file declares no explicit Cohesion project or external package references.

## Principal public types

| Type | Source file |
|---|---|
| `IValidationCondition` | `src/Abstractions/IValidationCondition.cs` |
| `IValidationCondition<T>` | `src/Abstractions/IValidationCondition.cs` |
| `IValidationContext` | `src/Abstractions/IValidationContext.cs` |
| `IValidationError` | `src/Abstractions/IValidationError.cs` |
| `IValidationItem` | `src/Abstractions/IValidationItem.cs` |
| `IValidationItem<T, TValue>` | `src/Abstractions/IValidationItem.cs` |
| `IValidationItemQueue` | `src/Abstractions/IValidationItemQueue.cs` |
| `IValidationProfile` | `src/Abstractions/IValidationProfile.cs` |
| `IValidationProfile<T>` | `src/Abstractions/IValidationProfile.cs` |
| `IValidationProfileBuilder` | `src/Abstractions/IValidationProfileBuilder.cs` |
| `IValidationRule` | `src/Abstractions/IValidationRule.cs` |
| `IValidationRule<in TValue>` | `src/Abstractions/IValidationRule.cs` |
| `ValidationContext<T>` | `src/ValidationContext.cs` |
| `ValidationError` | `src/ValidationError.cs` |
| `ValidationItemQueue` | `src/ValidationItemQueue.cs` |
| `ValidationOptions` | `src/ValidationOptions.cs` |

## Sources

- **Source** — `cohesion/libraries/ObjectValidation/Assimalign.Cohesion.ObjectValidation/src/Assimalign.Cohesion.ObjectValidation.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/ObjectValidation/Assimalign.Cohesion.ObjectValidation/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/ObjectValidation/Assimalign.Cohesion.ObjectValidation/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/ObjectValidation/Assimalign.Cohesion.ObjectValidation/src`.

- **Source** — `cohesion/libraries/ObjectValidation/Assimalign.Cohesion.ObjectValidation/src/Abstractions/IValidationCondition.cs`.

- **Source** — `cohesion/libraries/ObjectValidation/Assimalign.Cohesion.ObjectValidation/src/Abstractions/IValidationContext.cs`.

- **Source** — `cohesion/libraries/ObjectValidation/Assimalign.Cohesion.ObjectValidation/src/Abstractions/IValidationError.cs`.

- **Source** — `cohesion/libraries/ObjectValidation/Assimalign.Cohesion.ObjectValidation/src/Abstractions/IValidationItem.cs`.

- **Source** — `cohesion/libraries/ObjectValidation/Assimalign.Cohesion.ObjectValidation/src/Abstractions/IValidationItemQueue.cs`.

- **Source** — `cohesion/libraries/ObjectValidation/Assimalign.Cohesion.ObjectValidation/src/Abstractions/IValidationProfile.cs`.

- **Source** — `cohesion/libraries/ObjectValidation/Assimalign.Cohesion.ObjectValidation/src/Abstractions/IValidationProfileBuilder.cs`.

- **Source** — `cohesion/libraries/ObjectValidation/Assimalign.Cohesion.ObjectValidation/src/Abstractions/IValidationRule.cs`.

- **Source** — `cohesion/libraries/ObjectValidation/Assimalign.Cohesion.ObjectValidation/src/ValidationContext.cs`.

- **Source** — `cohesion/libraries/ObjectValidation/Assimalign.Cohesion.ObjectValidation/src/ValidationError.cs`.

- **Source** — `cohesion/libraries/ObjectValidation/Assimalign.Cohesion.ObjectValidation/src/ValidationItemQueue.cs`.

- **Source** — `cohesion/libraries/ObjectValidation/Assimalign.Cohesion.ObjectValidation/src/ValidationOptions.cs`.
