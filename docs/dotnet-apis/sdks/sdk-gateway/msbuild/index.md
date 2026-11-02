# MSBuild

The Gateway SDK surrounds the consumer project with the base imports and one package reference.

## Evaluation order

1. `Assimalign.Cohesion.Sdk.Gateway/Sdk/Sdk.props` defaults `OutputType=Exe` when empty. It does not
   set the resource-SDK marker, so the base SDK adds no App framework.
2. `Assimalign.Cohesion.Sdk/Sdk/Sdk.props` runs the base props: defaults, Microsoft SDK props,
   the version snapshot, common properties, settings, and framework registration.
3. `Targets/Sdk.Gateway.props` adds the implicit `Assimalign.Cohesion.ApplicationModel.Gateway`
   package reference.
4. The consumer project body evaluates.
5. `Assimalign.Cohesion.Sdk.Gateway/Sdk/Sdk.targets` defaults an executable gateway to
   self-contained for the host runtime identifier.
6. `Assimalign.Cohesion.Sdk/Sdk/Sdk.targets` runs the base targets, including pin validation.
7. `Targets/Sdk.Gateway.targets`, which is empty, is imported last.

- **Props** — [Every props file](props.md).
- **Targets** — [Target wiring](targets.md).
- **Tasks** — [Task ownership](tasks.md).

[SDK home](../index.md)

## Sources

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Sdk/Sdk.props`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Sdk/Sdk.targets`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Targets/Sdk.Gateway.props`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Targets/Sdk.Gateway.targets`.
