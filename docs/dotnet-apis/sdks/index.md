# SDKs

Cohesion software development kits connect project declarations to shared frameworks and build-time generation.

## SDK family

Each resource SDK imports the base SDK; the base imports `Microsoft.NET.Sdk`. Pick the resource
area whose framework the application needs.

| SDK package | Use | Implicit frameworks |
|---|---|---|
| [`Assimalign.Cohesion.Sdk`](sdk/index.md) | Generic applications and explicit library consumers | `Assimalign.Cohesion.App` |
| [`Assimalign.Cohesion.Sdk.ApiManager`](sdk-api-manager/index.md) | ApiManager resource application | `Assimalign.Cohesion.App` + `Assimalign.Cohesion.App.ApiManager` |
| [`Assimalign.Cohesion.Sdk.ConfigurationStore`](sdk-configuration-store/index.md) | ConfigurationStore resource application | `Assimalign.Cohesion.App` + `Assimalign.Cohesion.App.ConfigurationStore` |
| [`Assimalign.Cohesion.Sdk.Database`](sdk-database/index.md) | Database resource application | `Assimalign.Cohesion.App` + `Assimalign.Cohesion.App.Database` |
| [`Assimalign.Cohesion.Sdk.EmailHub`](sdk-email-hub/index.md) | EmailHub resource application | `Assimalign.Cohesion.App` + `Assimalign.Cohesion.App.EmailHub` |
| [`Assimalign.Cohesion.Sdk.EventHub`](sdk-event-hub/index.md) | EventHub resource application | `Assimalign.Cohesion.App` + `Assimalign.Cohesion.App.EventHub` |
| [`Assimalign.Cohesion.Sdk.Gateway`](sdk-gateway/index.md) | Application gateway composition | No gateway framework; explicit in-process exception |
| [`Assimalign.Cohesion.Sdk.IdentityHub`](sdk-identity-hub/index.md) | IdentityHub resource application | `Assimalign.Cohesion.App` + `Assimalign.Cohesion.App.IdentityHub` |
| [`Assimalign.Cohesion.Sdk.IoTHub`](sdk-iot-hub/index.md) | IoTHub resource application | `Assimalign.Cohesion.App` + `Assimalign.Cohesion.App.IoTHub` |
| [`Assimalign.Cohesion.Sdk.LoadBalancer`](sdk-load-balancer/index.md) | LoadBalancer resource application | `Assimalign.Cohesion.App` + `Assimalign.Cohesion.App.LoadBalancer` |
| [`Assimalign.Cohesion.Sdk.LogSpace`](sdk-log-space/index.md) | LogSpace resource application | `Assimalign.Cohesion.App` + `Assimalign.Cohesion.App.LogSpace` |
| [`Assimalign.Cohesion.Sdk.MediaHub`](sdk-media-hub/index.md) | MediaHub resource application | `Assimalign.Cohesion.App` + `Assimalign.Cohesion.App.MediaHub` |
| [`Assimalign.Cohesion.Sdk.MessageHub`](sdk-message-hub/index.md) | MessageHub resource application | `Assimalign.Cohesion.App` + `Assimalign.Cohesion.App.MessageHub` |
| [`Assimalign.Cohesion.Sdk.NatGateway`](sdk-nat-gateway/index.md) | NatGateway resource application | `Assimalign.Cohesion.App` + `Assimalign.Cohesion.App.NatGateway` |
| [`Assimalign.Cohesion.Sdk.NotificationHub`](sdk-notification-hub/index.md) | NotificationHub resource application | `Assimalign.Cohesion.App` + `Assimalign.Cohesion.App.NotificationHub` |
| [`Assimalign.Cohesion.Sdk.Rezolvr`](sdk-rezolvr/index.md) | Rezolvr resource application | `Assimalign.Cohesion.App` + `Assimalign.Cohesion.App.Rezolvr` |
| [`Assimalign.Cohesion.Sdk.Scheduler`](sdk-scheduler/index.md) | Scheduler resource application | `Assimalign.Cohesion.App` + `Assimalign.Cohesion.App.Scheduler` |
| [`Assimalign.Cohesion.Sdk.SecretStore`](sdk-secret-store/index.md) | SecretStore resource application | `Assimalign.Cohesion.App` + `Assimalign.Cohesion.App.SecretStore` |
| [`Assimalign.Cohesion.Sdk.VpnGateway`](sdk-vpn-gateway/index.md) | VpnGateway resource application | `Assimalign.Cohesion.App` + `Assimalign.Cohesion.App.VpnGateway` |
| [`Assimalign.Cohesion.Sdk.Web`](sdk-web/index.md) | Web resource application | `Assimalign.Cohesion.App` + `Assimalign.Cohesion.App.Web` |

## Resolution and pins

A base SDK consumer can use an inline version:

```xml
<Project Sdk="Assimalign.Cohesion.Sdk/10.0.1-preview.3">
</Project>
```

For a layered SDK, pin both the chosen identity and `Assimalign.Cohesion.Sdk` in `global.json` under
`msbuild-sdks`. Nested `Import Sdk` declarations do not carry the project’s inline version. Each
area page includes a complete pin example. Versions shown here match the source repository’s
`global.json`; configured NuGet feeds must supply those packages. NuGet’s MSBuild SDK resolver
supplies the imports without a custom resolver or installer.

The base pin task walks upward from the project and checks the nearest `global.json`. All present
recognized Cohesion pins must agree by exact ordinal string equality. A non-empty string
`sdk.version` is checked for a valid numeric version of at least `10.0.300`. Missing files skip validation; consistent local versions
are valid. The target runs before `CollectPackageReferences` and `PrepareForBuild` and reports
`COHSDK002`. `CohesionSkipSdkPinCheck=true` is a tooling escape.

## Shared project defaults

| Property | Default | Effect | Defined in |
|---|---|---|---|
| `OutputType` | `Exe` | Executable output; explicitly choose Library for a base-SDK library. | `Targets/Assimalign.Cohesion.Sdk.Defaults.props` |
| `TargetFramework` | `net10.0` | Compile against the .NET 10 target framework. | `Targets/Assimalign.Cohesion.Sdk.Defaults.props` |
| `LangVersion` | `Preview` | Enable preview C# syntax. | `Targets/Assimalign.Cohesion.Sdk.Defaults.props` |
| `EnablePreviewFeatures` | `true` | Permit preview APIs; also affects language-version selection. | `Targets/Assimalign.Cohesion.Sdk.Defaults.props` |
| `ImplicitUsings` | `disable` | Require explicit using directives. | `Targets/Assimalign.Cohesion.Sdk.Defaults.props` |
| `Nullable` | `enable` | Enable nullable reference analysis. | `Targets/Assimalign.Cohesion.Sdk.Defaults.props` |
| `IsAotCompatible` | `true` | Enable ahead-of-time compatibility analysis. | `Targets/Assimalign.Cohesion.Sdk.Defaults.props` |

These defaults are conditional on empty values and run before Microsoft’s props. Command-line global
properties are honored; later consumer `Directory.Build.props` and project assignments override
ordinary defaults. Use `OutputType=Library` explicitly for base-SDK libraries. Resource executables
do not multi-target. A language-version override also needs a corresponding `EnablePreviewFeatures`
override. Gateway has stricter assignments described in its section.

## Framework delivery

The base registers every `KnownFrameworkReference`. An ordinary resource SDK adds its area
`FrameworkReference` after the base framework. These resolve to
`Assimalign.Cohesion.App[.<Area>].Ref` targeting packs and
`Assimalign.Cohesion.App[.<Area>].Runtime.<rid>` runtime packs. `CohesionAppFrameworkVersion`
defaults to the packaged `CohesionVersion`. Configure `CohesionAutoIncludeAppFramework=false`
before props inclusion to suppress implicit items; registrations remain available.

See [framework and props details](sdk/msbuild/props.md) for pack layout and import timing.

## Shared generation

`CohesionApplicationModel=enabled` turns an executable into a manifest-producing resource. The base
validates endpoints, mounts, lifecycle metadata, and references, then generates resource accessors
and control-plane registration. Area SDKs supply defaults; they do not author the application’s
`Program.cs`.

Set `CohesionAppSettingsClass` to opt into strongly typed `appsettings*.json` generation. The public
generated root has an explicit `Bind(IConfiguration)` method suitable for ahead-of-time (AOT)
compilation. The [base reference](sdk/overview.md) explains shape limits, incremental outputs, and
diagnostics.

## Gateway boundary

`Assimalign.Cohesion.Sdk.Gateway` forces an enabled executable `Composite` resource and suppresses
the implicit base framework before import. No `Assimalign.Cohesion.App.Gateway` framework exists.
Provider packages contribute `CohesionGatewayProvider` metadata; generation uses those declared
types. In-process composition explicitly adds the shipped base, Web, and Database frameworks.

Gateway remains partial: seven typed area mappings and a finite dependency bootstrap are present,
while restore-visible manifest dependency selection remains incomplete. The
[Gateway section](sdk-gateway/index.md) documents those limits and external provider integration.

## Templates and repository build

The template package supplies authored entry points and pins all twenty SDK identities. Standalone
resources explicitly disable orchestration; application and landing-zone members enable it.
Repository `build/Targets` files govern building Cohesion itself; they are not all imported into
packaged consumers. In particular, the shipped `Build.Version.props` is a static pack-time snapshot.

[.NET APIs](../index.md)

## Sources

- **Source** — `cohesion/sdks/README.md`
- **Source** — `cohesion/global.json`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Sdk/Sdk.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Sdk/Sdk.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Sdk.Resource.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Sdk.Resource.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Assimalign.Cohesion.Sdk.Defaults.props`
- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Assimalign.Cohesion.Sdk.FrameworkReference.props`
- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk/Tasks/src/Tasks/Validation/ValidateCohesionSdkPinsTask.cs`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Sdk/Sdk.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Sdk/Sdk.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Tasks/docs/OVERVIEW.md`
- **Source** — `cohesion/sdks/Directory.Build.targets`
- **Source** — `cohesion/tooling/templates/Assimalign.Cohesion.Templates/docs/OVERVIEW.md`
