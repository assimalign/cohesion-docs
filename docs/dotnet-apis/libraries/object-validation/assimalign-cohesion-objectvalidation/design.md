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
  further item once the context holds an error, so only the first failing item is reported.
- **`ContinueThroughValidationChain` decides within one item's rule chain.** Off, the default, the
  chain stops at the first of the item's rules that fails; on, every chained rule runs.

| `ValidationMode` | `ContinueThroughValidationChain` | Reported |
| --- | --- | --- |
| `Cascade` (default) | off (default) | Every failing member, each with its first failing rule's errors |
| `Cascade` | on | Every failing rule of every member |
| `Stop` | off | The first failing member, with its first failing rule's errors |
| `Stop` | on | Every failing rule of the first failing member |

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

**Evaluation order is declaration order.** A profile's items run in the order its members are
declared (a `When` block's members where the block is declared), and each item's rules in the order
they are chained. "First" above means first in that order: with the defaults, a member whose chain
has several failing rules reports the first of them, and `Stop` reports the first-declared failing
member. `ValidationItemQueue` and `ValidationRuleQueue` are first-in, first-out, as
`IValidationRuleQueue` says: enumeration, the indexer, copies, `Peek` and `Pop` all start at the
entry pushed first. `ValidationContext<T>` keeps its errors and invocations in the order they are
added, so `ValidationResult.Errors` lists failures in declaration order. A nested profile's errors
sit in place under their parent member, and a collection's are listed rule by rule, each rule's
element by element.

The context's collections changed with the queues, and for the same reason. Had the errors stayed on
a stack, `ValidationResult.Errors` would have listed members newest first, and a nested profile's
errors, which are copied into every enclosing context, would have been reversed once per level of
nesting.

Until #1221 both queues enumerated newest first and the context held its errors on a stack. A chain
reported its last failing rule: `RuleFor(x => x.Name).NotEmpty().MinLength(3)` on `""` reported
`MinLength`'s message, not `NotEmpty`'s. `Stop` reported the last-declared failing member, and
Web.Validation's `errors` map listed members in reverse declaration order.

## A rule that throws

A rule reports a failure by adding errors to its context; an exception is a fault, not a failure. A
nested profile's rules (`ChildRules`, `UseProfile`) and a custom rule's delegate (`Custom`) run
arbitrary code, and an exception from either propagates out of `Validate` and `ValidateAsync`:
validation fails closed.

Until #1292 both rules caught every exception and returned `false`, which an item records as "rule
not invoked". A throwing nested profile or custom delegate therefore reported no error and the value
passed; Web.Validation let such a body through to the handler. A null nested object is still
skipped, explicitly: the nested rule's `object` overload returns an empty, successful context for
`null`.

The built-in rules (`NotEmpty`, `GreaterThan`, the length and pattern rules, and so on) still catch
their own exceptions and report "not invoked", which may be an intended skip for a null or
incomparable value but is not stated; #1293 decides each rule's behavior explicitly.

## Concurrency

A validator, its profiles, their items and their rules are built once and shared by every
validation that uses them: Web.Validation registers one validator per model type and every request
validates through it. Evaluation therefore keeps per-validation state in the `ValidationContext` and
in locals, never on a shared object.

- **Timing** — each validation and each rule invocation is timed from a `Stopwatch.GetTimestamp()`
  local and reported in `TimeSpan` ticks (`Stopwatch.GetElapsedTime`). Until #1291 the validator
  rented a `Stopwatch` from an unsynchronized static pool on every validation, and each item rented
  one in its constructor and restarted it on every evaluation. Concurrent validations raced on the
  pool's list, which could hand a validation `null`, and they shared one stopwatch per item. The
  pool's `ElapsedTicks` were also `Stopwatch` ticks, read as `TimeSpan` ticks, which is wrong
  wherever the timer is not 10 MHz.
- **Known shared state** — a rule's `ParentContext` is still assigned on the shared rule before it
  runs, so two concurrent validations with different options can run a nested profile with the
  other's options (#1207). Errors never cross between validations: each lands in its own context.

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
