# Example: Protect a purpose-scoped payload

Protect and recover a small payload using a persisted rotating key ring.

[Examples](index.md) · [Assembly overview](../index.md)

## Code

```csharp
using System;
using System.Text;

using Assimalign.Cohesion.Security.DataProtection;

IDataProtectionProvider provider = DataProtectionProvider.Create(
    KeyRepository.CreateFileSystem("/var/lib/myapp/keys"),
    options =>
    {
        options.ApplicationDiscriminator = "myapp";
        options.KeyLifetime = TimeSpan.FromDays(90);
        options.UnprotectGracePeriod = TimeSpan.FromDays(7);
    });

IDataProtector protector = provider.CreateProtector("Cohesion.Http.Antiforgery.v1");
byte[] payload = Encoding.UTF8.GetBytes("example payload");
byte[] wire = protector.Protect(payload);
byte[] recovered = protector.Unprotect(wire);
```

## Walkthrough

Choose a writable key directory for the application deployment. The provider shares one ring across
purpose-scoped protectors, while the application discriminator separates co-located applications.
The protector authenticates the payload; invalid input raises `DataProtectionException`. The payload
initialization is supplied here to complete the overview snippet.

## Sources

- **Source** — `cohesion/libraries/Security/Assimalign.Cohesion.Security.DataProtection/docs/OVERVIEW.md`.
