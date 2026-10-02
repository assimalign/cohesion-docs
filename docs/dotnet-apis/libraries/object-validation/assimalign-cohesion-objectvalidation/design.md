# Assimalign.Cohesion.ObjectValidation design

Design decisions and ownership boundaries for `Assimalign.Cohesion.ObjectValidation`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

Profiles separate rule declaration from execution. Member selectors are inspected through resolved
metadata instead of compiling expressions, while conditional predicates are supplied as delegates.
Options control failure aggregation and throwing without changing how profiles are authored.

## Error sources

An error's `Source` names what failed. A rule's default source is its member selector's text
(`p => p.Name`), and a profile can set another through the rule's error callback
(`NotEmpty(error => error.Source = "name")`), which is kept as written.

**Nested profiles report under their parent member.** `ChildRules` and `UseProfile` validate a
member with a profile of its own type, whose selectors start from that type (`a => a.City`). The
nested rule (`ChildValidationRule`) records the parent member's selector and composes each nested
error's default source under it, so the error reads `p => p.Address.City`; two levels deep it reads
`p => p.Order.Address.City`. Only the default member-selector form is composed: a source the nested
rule set explicitly is kept as written. Before this, a nested error carried only the nested selector,
and errors on equally named members of different nested objects (`Shipping.City`, `Billing.City`)
could not be told apart. Under `RuleForEach`, nested sources name the collection member
(`p => p.Addresses.City`) without an element index: the collection item evaluates its rules per
element without passing the index to them.

Web request validation
([`Assimalign.Cohesion.Web.Validation`](../../../resources/web/assimalign-cohesion-web-validation/index.md))
reports these sources to clients as the member path after the selector parameter (`Address.City`).

## Dependency boundary

The project declares no explicit Cohesion or external package references. Shared repository build
properties still apply.

## Sources

- **Source** — `cohesion/libraries/ObjectValidation/Assimalign.Cohesion.ObjectValidation/src/Assimalign.Cohesion.ObjectValidation.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/ObjectValidation/Assimalign.Cohesion.ObjectValidation/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/ObjectValidation/Assimalign.Cohesion.ObjectValidation/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/ObjectValidation/Assimalign.Cohesion.ObjectValidation/src`.

- **Source** — `cohesion/libraries/ObjectValidation/Assimalign.Cohesion.ObjectValidation/tests/RuleTests/RuleNestedErrorSourceTests.cs`.
