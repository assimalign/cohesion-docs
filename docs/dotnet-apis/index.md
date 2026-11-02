# .NET APIs

Browse Cohesion library, resource, and SDK reference documentation.

1. **[Libraries](libraries/index.md)** — foundation building blocks in Layer 1, with application
   composition packages also participating in Layer 2.
2. **[Resources](resources/index.md)** — service and resource platforms in Layer 3.
3. **[SDKs](sdks/index.md)** — MSBuild software development kits and the
   `Assimalign.Cohesion.App[.<Domain>]` shared frameworks.

## Reading an assembly reference

An assembly folder starts with its overview, followed by design decisions, examples, and documented
public types in alphabetical order. The overview explains purpose, scope, dependencies, and the
principal entry points. Design pages explain ownership and extension boundaries. Examples identify
their originating source or test; type pages retain member details from the assembly reference.

When a project has no per-type reference, its overview lists principal public types from source.
Scaffolds and incomplete implementations carry status callouts rather than presenting planned APIs
as available behavior.

## Package and framework names

Library and resource assemblies generally use `Assimalign.Cohesion.<Area>[.<Feature>]` names.
The framework membership manifest assigns assemblies to `Assimalign.Cohesion.App` and domain
frameworks such as `Assimalign.Cohesion.App.Web`. An assembly's name alone does not establish its
framework membership.

The base SDK chains through `Microsoft.NET.Sdk`; domain SDKs chain through the base Cohesion SDK.
The base and domain SDKs automatically add the appropriate `FrameworkReference` entries. A framework
is delivered through a `.Ref` targeting package and a `.Runtime.<rid>` runtime package, separating
compile-time reference assemblies from runtime implementations.

`Assimalign.Cohesion.Sdk.Gateway` is the exception: it references the
`Assimalign.Cohesion.ApplicationModel.Gateway` NuGet package, and there is no
`Assimalign.Cohesion.App.Gateway` framework. A gateway references the platform packages it deploys
through, and an in-process gateway references the resource programs it runs.

## Sources

- **Source** — `cohesion/sdks/README.md`.
- **Source** — `cohesion/.claude/rules/build-system.md`.
- **Source** — `cohesion/libraries/App/Assimalign.Cohesion.App.props`.
- **Source** — `cohesion/docs/programs/DELIVERY_ROADMAP.md`.
