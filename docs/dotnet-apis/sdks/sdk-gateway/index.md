# Assimalign.Cohesion.Sdk.Gateway

The Gateway SDK makes an executable project a Cohesion application gateway that references the gateway package.

> **Status:** Implemented. The SDK generates nothing; the gateway's model and platform are code in its `Program.cs`.

The SDK chains to the base `Assimalign.Cohesion.Sdk`, defaults the project to an executable, and
references `Assimalign.Cohesion.ApplicationModel.Gateway` at the SDK's own version. It adds no
shared framework and no platform: the project references the gateway half of each platform it
deploys through.

## Project contract

```xml
<Project Sdk="Assimalign.Cohesion.Sdk.Gateway">
  <ItemGroup>
    <!-- The platform the model runs on. -->
    <PackageReference Include="Assimalign.Cohesion.ApplicationModel.Gateway.InProcess" Version="10.0.1-preview.3" />
  </ItemGroup>
</Project>
```

The gateway's `Program.cs` declares its model and runs it on the platform it references:

```csharp
using Assimalign.Cohesion.ApplicationModel.Gateway;
using Assimalign.Cohesion.ApplicationModel.Gateway.InProcess;

await using var infrastructure = new InProcessInfrastructure();
infrastructure.AddProgram("Web", "orders-api", Orders.Api.Program.MainAsync);

ApplicationGatewayBuilder builder = ApplicationGateway.CreateBuilder("orders").UseInProcess(infrastructure);
builder.AddResource("Web", "orders-api");
await builder.Build().RunAsync();
```

The in-process platform runs each resource's program in the gateway's own process, so the gateway
references the programs it registers. An out-of-process platform, such as the Kubernetes or Docker
pair in `cohesion-platforms`, replaces the in-process package and its registration; the model does
not change. See [Platforms](../../../platforms/index.md).

```json
{
  "sdk": {
    "version": "10.0.300",
    "rollForward": "latestFeature"
  },
  "msbuild-sdks": {
    "Assimalign.Cohesion.Sdk": "10.0.1-preview.3",
    "Assimalign.Cohesion.Sdk.Gateway": "10.0.1-preview.3"
  }
}
```

## Reference

- **Overview** — [Properties and decisions](overview.md).
- **MSBuild** — [Import order](msbuild/index.md).
- **Packaging project** — [`Assimalign.Cohesion.Sdk.Gateway.Tasks`](assimalign-cohesion-sdk-gateway-tasks/index.md).
- **Gateway library** — [ApplicationModel.Gateway](../../libraries/application-model/assimalign-cohesion-applicationmodel-gateway/index.md).
- **SDK family** — [All SDKs](../index.md).

## Sources

- **Primary source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Tasks/docs/OVERVIEW.md`.
- **Design** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Tasks/docs/DESIGN.md`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Sdk/Sdk.props`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Targets/Sdk.Gateway.props`.
