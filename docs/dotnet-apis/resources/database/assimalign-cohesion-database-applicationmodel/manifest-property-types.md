# Manifest property types used by Database

The shared ApplicationModel manifest DTOs now use checked value objects at these public init properties:

| DTO property | Public type |
| --- | --- |
| ResourceManifestControlPlane.Path | ResourceHttpPath |
| ResourceManifestProbe.Http | ResourceHttpPath? |
| ResourceManifestMount.Source | ResourceMountSource? |

The DTOs are in Assimalign.Cohesion.ApplicationModel. Their existing implicit string conversions validate values; default/deserialized values are revalidated at manifest boundaries. JSON retains strings and the existing schema. Consumers use Value at string-only boundaries; nullable members require unwrapping the nullable value and then its Value.

```csharp
string controlPath = manifest.ControlPlane.Path.Value;
string? readinessPath = manifest.Probes.Readiness?.Http?.Value;
string? mountSource = manifest.Mounts[0].Source?.Value;
```

Database planning, local preparation and in-process hosting use these checked values without a Database runtime dependency in ApplicationModel.

[Database ApplicationModel](index.md) · [Design](design.md)

## Sources

- `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel/src/Manifests/ResourceManifestControlPlane.cs`.
- `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel/src/Manifests/ResourceManifestProbe.cs`.
- `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel/src/Manifests/ResourceManifestMount.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.ApplicationModel/docs/DESIGN.md`.
