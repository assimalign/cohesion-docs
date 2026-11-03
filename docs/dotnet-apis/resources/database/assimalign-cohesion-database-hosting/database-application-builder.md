# DatabaseApplicationBuilder

Collects engine intent and hosting registrations for one application Build attempt.

Namespace: `Assimalign.Cohesion.Database.Hosting`.

## Signature

The HTTP health member is:

```csharp
public sealed class DatabaseApplicationBuilder : IDatabaseApplicationBuilder
{
    public DatabaseApplicationBuilder AddHealthEndpoint(Uri endpoint);
}
```

Other composition members and one-shot Build behavior are described in the [assembly](index.md)
and [design](design.md).

## AddHealthEndpoint

Configures one plain-HTTP health listener and returns this builder. The last call replaces the
previous URI. Registration validates the endpoint and bind host immediately: a literal IP address
or localhost is required. HTTPS has no certificate source and is refused. Null throws
ArgumentNullException; an invalid endpoint URI throws ArgumentException. Unsupported scheme/host,
or registration after Build begins, throws InvalidOperationException.

When COHESION_GATEWAY is nonempty, Build reads the current AppEnvironment `http` assignment. A
plain-HTTP assignment with a literal IP or localhost wins over explicit health. Replacing a
different explicit URI emits Hosting event 19, HealthEndpointOverridden. An unsupported scheme or
DNS bind host emits informational event 18, HealthEndpointAssignmentSkipped, and keeps explicit
health or composes no listener. Missing/malformed assignments are unset. An unmanaged application
uses only this explicit member, even if platform endpoint values are present.

The listener starts before model servers and stops after they drain. Start awaits binding;
socket failure reports CK07102 in DatabaseException.Diagnostics, retaining the Web startup and
socket cause chain and rolling back prior services. Cancellation propagates unchanged.

Bare `/healthz`, `/readyz` and `/livez` accept remote probes. Namespaced `/cohesion/v1` health aliases
accept loopback transport peers only; forwarded headers do not grant locality. Other namespaced
paths remain forbidden. Readiness requires the outer host to be Started after all model servers
confirm acceptance. See [health composition](design.md#platform-endpoints-and-http-health).

## Example

For an unmanaged application, following DatabaseHealthEndpointTests:

```csharp
var builder = DatabaseApplication.CreateBuilder();
builder.AddHealthEndpoint(new Uri("http://127.0.0.1:5741"));
await using var application = builder.Build();
await application.RunAsync();
```

Import `Assimalign.Cohesion.Database.Hosting` and `Assimalign.Cohesion.Hosting` for the host APIs.
While started, probes use `http://127.0.0.1:5741/readyz`.

## Related reference

[Assembly](index.md) · [Server options](../assimalign-cohesion-database/database-server-options.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Hosting/src/DatabaseApplicationBuilder.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Hosting/src/Internal/DatabaseAdminEndpointService.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Hosting/tests/DatabaseHealthEndpointTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Hosting/tests/DatabaseHostingEventSourceTests.PlatformEndpoints.cs`.
