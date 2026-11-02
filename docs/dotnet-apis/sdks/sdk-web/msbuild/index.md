# MSBuild

The Web SDK surrounds the consumer project with the base imports, its framework references, and Web defaults.

## Evaluation order

1. `Assimalign.Cohesion.Sdk.Web/Sdk/Sdk.props` sets `_CohesionResourceSdk=true` before importing the base SDK.
2. The base props run: defaults (`OutputType=Exe`), Microsoft SDK props, the version snapshot,
   common properties, settings, and framework registration.
3. Web's `Sdk/Sdk.props` adds the `Assimalign.Cohesion.App` and `Assimalign.Cohesion.App.Web`
   framework references, then imports `Targets/Sdk.Web.props`, which sets `EventSourceSupport`,
   `InterceptorsNamespaces`, and the `wwwroot/**` copy.
4. The consumer project body evaluates.
5. Web's `Sdk/Sdk.targets` imports the base `Sdk.targets`, then `Targets/Sdk.Web.targets`, which
   is empty.

- **Props** — [Every props file](props.md).
- **Targets** — [Target wiring](targets.md).
- **Tasks** — [Task ownership and tests](tasks.md).

[SDK home](../index.md)

## Sources

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Web/Sdk/Sdk.props`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Web/Sdk/Sdk.targets`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Web/Targets/Sdk.Web.props`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Web/Targets/Sdk.Web.targets`.
