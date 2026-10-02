# Assimalign.Cohesion.ObjectValidation design

Design decisions and ownership boundaries for `Assimalign.Cohesion.ObjectValidation`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

Profiles separate rule declaration from execution. Member selectors are inspected through resolved
metadata instead of compiling expressions, while conditional predicates are supplied as delegates.
Options control failure aggregation and throwing without changing how profiles are authored.

## Which failures are reported

Two options decide which failures a validation reports, each at its own level:

- **`ValidationMode` decides between items**, one item per `RuleFor` or `RuleForEach` member.
  `Cascade`, the default, evaluates every item and reports each one that fails. `Stop` evaluates no
  further item once the context holds an error, so only one failing item is reported.
- **`ContinueThroughValidationChain` decides within one item's rule chain.** Off, the default, the
  chain stops once one of the item's own rules has failed; on, every chained rule runs.

| `ValidationMode` | `ContinueThroughValidationChain` | Reported |
| --- | --- | --- |
| `Cascade` (default) | off (default) | Every failing member, each with one rule's errors |
| `Cascade` | on | Every failing rule of every member |
| `Stop` | off | One failing member, with one rule's errors |
| `Stop` | on | Every failing rule of one failing member |

**An item's chain stops on its own failure only.** An item stops its chain when one of its own
rules reports an error, never because of errors that earlier items, or the caller, added to the
context; stopping across items is `Stop`'s job. Before #1206 the items checked whether the context
held any error, so with the defaults the first failing member stopped every member after it, and a
request body with three invalid members was answered with one error.

- **Collections.** A `RuleForEach` item runs each rule over every element, so the rule that fails
  reports each failing element, and the chain then stops for the whole collection.
- **Nested profiles.** `ChildRules` and `UseProfile` evaluate each nested item against a context of
  their own and apply `Stop` between nested items. To the parent member, a nested profile is one
  rule: everything it reports counts as that rule's errors.
- **`ThrowExceptionOnFailure`** changes nothing that is evaluated; the validator throws afterwards,
  when the context holds an error.

**Evaluation order is newest first, a known constraint (#1221).** The item and rule queues
enumerate from the top of their stacks, so a profile's items run from the last declared member to
the first, and a chain from its last chained rule to its first. With the defaults, a member whose
chain has two failing rules reports the later-declared rule's error, and `Stop` reports the
last-declared failing member. `IValidationRuleQueue` documents first-in, first-out order, which the
enumerators do not implement yet.

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
