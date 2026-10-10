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
Members are evaluated in the order they are declared and each member's rules in the order they are
chained. With the defaults, every failing member is reported, each with the errors of its first
failing rule: `ValidationMode.Stop` stops at the first failing member, and
`ContinueThroughValidationChain` runs every rule of a member. See
[which failures are reported](design.md#which-failures-are-reported).

The pattern rules bound what a request body can cost. `EmailAddress` runs in linear time and fails
an address over the RFC 5321 sizes. `Matches` runs the caller's pattern in linear time when the
non-backtracking engine supports it. Otherwise each match gets one second, per element under
`RuleForEach`, so N strings can cost N seconds. A match that runs out fails the rule, and an invalid
pattern throws where the profile declares it. See
[pattern rules on untrusted input](design.md#pattern-rules-on-untrusted-input).

A nested profile's rule or a custom rule that throws faults the validation: the exception propagates
out of `Validate` and `ValidateAsync` rather than letting the value pass (see
[a rule that throws](design.md#a-rule-that-throws)). A validator is shared by every validation that
uses it, so evaluation keeps per-validation state in the validation's context, with one known
exception tracked as #1207 (see [concurrency](design.md#concurrency)).
`ValidationResult.ValidationElapsedTicks` and `ValidationInvocation.ElapsedTicks` are `TimeSpan`
ticks.

An error raised inside a nested profile (`ChildRules`, `UseProfile`) carries a source composed under
its parent member — `order => order.Shipping.City` rather than `a => a.City` — so errors on equally
named members of different nested objects (`Shipping.City`, `Billing.City`) stay distinguishable. A
source a rule set explicitly is kept as written. See the [design](design.md#error-sources).

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

- **Source** — `cohesion/libraries/ObjectValidation/Assimalign.Cohesion.ObjectValidation/src/ValidationResult.cs`.

- **Source** — `cohesion/libraries/ObjectValidation/Assimalign.Cohesion.ObjectValidation/src/ValidationInvocation.cs`.
