# IIdentityHubResourceDescriptor

The `IIdentityHubResourceDescriptor` type belongs to `Assimalign.Cohesion.IdentityHub.ApplicationModel`.

> **Status:** Partial.

Extends IApplicationResourceDescriptor and IResourceCommandDescriptor. Resource exposes the typed
`IdentityHubResource`; Commands lists desired commands; AddCommand attaches a command. Both `DependsOn`
overloads return `IIdentityHubResourceDescriptor`, retaining `AddAudience`/`AddClient` through dependency
chaining. `AddIdentityHub` constructs the internal wrapper; existing assignments to
IApplicationResourceDescriptor remain valid. The interface owns no runtime host or service state.

[Assembly overview](index.md) · [Examples](examples/index.md)

## Sources

- **Primary source** — `cohesion/resources/IdentityHub/Assimalign.Cohesion.IdentityHub.ApplicationModel/docs/Assembly/Assimalign.Cohesion.IdentityHub.ApplicationModel/IIdentityHubResourceDescriptor/OVERVIEW.md`.
