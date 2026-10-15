# InProcessResourceDescriptorExtensions

`InProcessResourceDescriptorExtensions.InProcess(Assembly, string)` associates a built resource descriptor with its statically linked executable assembly and absolute, resource-specific content root.

[Assembly overview](index.md) · [Design](design.md) · [Examples](examples/index.md)

Namespace: `Assimalign.Cohesion.ApplicationModel.Gateway.InProcess`.

Assembly: `Assimalign.Cohesion.ApplicationModel.Gateway.InProcess`.

## Remarks

`InProcessResourceDescriptorExtensions.InProcess(Assembly, string)` associates a built resource
descriptor with its statically linked executable assembly and absolute, resource-specific content
root. Rebinding the same descriptor to different values is rejected.

This method is SDK infrastructure and is hidden from IntelliSense. `Sdk.Gateway` emits calls only
for the enabled, composable project-reference closure and emits `DynamicDependency` metadata for the
compiler-discovered entry-point type. Hand-written callers must supply an equivalent trimming root;
the API is therefore annotated `RequiresUnreferencedCode`.

## Members

| Member | Responsibility |
|---|---|
| `InProcess(Assembly, string)` | Binds a descriptor to its statically linked executable and absolute content root; conflicting rebinding is rejected. |

## Sources

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway.InProcess/docs/Assembly/Assimalign.Cohesion.ApplicationModel.Gateway.InProcess/InProcessResourceDescriptorExtensions/OVERVIEW.md`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway.InProcess/src/Extensions/InProcessResourceDescriptorExtensions.cs`.
