# HealthChecksBuilder used by Database hosting

Namespace: `Assimalign.Cohesion.Web.Health`. Assembly: `Assimalign.Cohesion.Web.Health`.

```csharp
public sealed class HealthChecksBuilder : IHealthChecksBuilder
{
    public HealthChecksBuilder();
    public IReadOnlyCollection<HealthCheckRegistration> Registrations { get; }
    public IHealthChecksBuilder Add(HealthCheckRegistration registration);
    public IHealthCheckService Build();
}
```

Database's admin endpoint now constructs this public sealed builder directly. The removed HealthChecks.CreateBuilder factory is replaced by new HealthChecksBuilder(). Add rejects null registration with ArgumentNullException and duplicate names under ordinal case-insensitive comparison with InvalidOperationException. Build creates an immutable service snapshot; later registrations do not mutate that service. Existing IHealthChecksBuilder extensions/adapters remain usable.

[Database hosting](index.md) · [Hosting design](design.md)

## Sources

- `cohesion/resources/Web/Assimalign.Cohesion.Web.Health/src/HealthChecksBuilder.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Hosting/src/Internal/DatabaseAdminEndpointService.cs`.
