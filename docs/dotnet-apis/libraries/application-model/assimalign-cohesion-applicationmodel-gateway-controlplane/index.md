# Assimalign.Cohesion.ApplicationModel.Gateway.ControlPlane

Exposes authenticated application discovery and resource command delivery over HTTP.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

- **[`GatewayControlPlane`](gateway-control-plane.md)** — type reference.

- **[`GatewayControlPlaneOptions`](gateway-control-plane-options.md)** — type reference.

- **[`IResourceCommandDispatcher`](i-resource-command-dispatcher.md)** — type reference.

[ApplicationModel](../index.md)

## Scope

`Assimalign.Cohesion.ApplicationModel.Gateway.ControlPlane` turns the gateway lifecycle seams into
a small NativeAOT-safe HTTP/1 server and client. The server always exposes the same
`ApplicationExportDocument` that the gateway writes to `export.json`; there is no second model or
serializer.

The server is application-scoped. That keeps model, observed state, trusted issuers, command
observations, listener lifetime, and Local discovery metadata isolated when an
`IMultiModelApplicationGateway` owns more than one application.

Requests are matched by a small internal router for the five fixed `/cohesion/v1` templates.
The package used to route with `Web.Routing` and dropped it because a library may not reference a
`resources/**` package. Matching, `404`/`405` handling, `Allow` values, and route values did not
change. DESIGN.md lists the exact rules.

The resolver client normalizes any configured gateway authority to
`/cohesion/v1/application`, sends an explicit Bearer credential, rejects credential-bearing
plaintext traffic except loopback HTTP, validates the export, and requires its
`trustKey` to match the expected trusted application.

Callers are authenticated by the built-in ES256 trusted-issuer check first and then by the
`IApplicationCallerAuthenticator`s the application registered in `builder.Providers.Callers` (for an
application-set member, in that member's `AddApplication(..., configure)` callback), so a
peer or developer can present an identity provider's credential. Discovery and command
authorization apply to the mapped `ApplicationCaller`; with nothing registered, every response is
unchanged.

See [Design](design.md) for lifecycle, authentication, and command-dispatch details.

The authenticated client also applies and deletes owned declarations through the served command
routes. `GatewayControlPlane.Configure` adapts the gateway's `CommandClients` into the server's
dispatcher seam. By default that list holds the generic `ResourceControlPlaneCommandClient`,
registered as `IGatewayResourceCommandClient.AnyKind`. A served command selects its dispatcher the
same way local delivery does: an exact manifest kind first, then the catch-all. Both paths retain
`Applied`/`Rejected` and provider detail, and a failed deletion retains the key's ownership
reservation.

The serving gateway's `IResourceTransportTrustProvider` supplies its application's TLS validator
to the required `serverCertificateValidator` parameter on dispatcher apply and delete. HTTPS
resource delivery uses the same anchors as probes and store reads; `null` keeps default trust.

## Command trust grants (O25 / item 31c)

TrustedIssuer.AllowedCommandKinds is an immutable normalized ordinal list. The original
constructor remains unrestricted. Absent or empty means every kind is allowed; a nonempty list
permits exactly those case-sensitive wire kinds. `--mode trust-add --peer peer --from peer.json --allow rezolvr.add-a-record,identityhub.add-audience` accepts repeated --allow arguments and
comma-separated values. --allow is rejected for trust-issue and other modes. The CLI forwards
--allow but still rejects --against, whose endpoint-selection contract remains item #982.

Local trusted-issuers.json and a registered trust store both persist optional
allowedCommandKinds arrays; the gateway hands the store a `TrustedIssuer` that carries the list.
Old documents remain unrestricted. The SecretStore orchestration store writes restricted grants as
{trustKey,allowedCommandKinds}; unrestricted grants retain the bare JWK protocol. Its export returns
the array to the gateway. AddTrustedIssuerAsync accepts the new collection while preserving the
existing overload and its unrestricted behavior.

The serving gateway carries the matched issuer's list onto the authenticated principal and checks
both apply and delete. A forbidden kind returns the existing 409 Rejected observation with a detail
naming the issuer and kind. It does not dispatch the command or release existing ownership.
Manifest advertisement, command-token scope and owner-equals-issuer checks remain in force.

HTTP connections are accepted with `AcceptAsync` from a typed HTTP listener over the configured TCP duplex listener. Each serving task owns and disposes its accepted connection.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.ApplicationModel`](../../application-model/assimalign-cohesion-applicationmodel/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.ApplicationModel.Gateway`](../../application-model/assimalign-cohesion-applicationmodel-gateway/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.Connections.Tcp`](../../connections/assimalign-cohesion-connections-tcp/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.Core`](../../core/assimalign-cohesion-core/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.Hosting.Resources`](../../hosting/assimalign-cohesion-hosting-resources/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.Http`](../../http/assimalign-cohesion-http/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.Http.Connections`](../../http/assimalign-cohesion-http-connections/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.IdentityModel`](../../identity-model/assimalign-cohesion-identitymodel/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.IdentityModel.Token.JsonWebToken`](../../identity-model/assimalign-cohesion-identitymodel-token-jsonwebtoken/index.md) | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `GatewayControlPlane` | `src/GatewayControlPlane.cs` |
| `GatewayControlPlaneOptions` | `src/GatewayControlPlaneOptions.cs` |
| `IResourceCommandDispatcher` | `src/IResourceCommandDispatcher.cs` |

## Sources

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway.ControlPlane/src/Assimalign.Cohesion.ApplicationModel.Gateway.ControlPlane.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway.ControlPlane/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway.ControlPlane/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/ApplicationModel/README.md`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway.ControlPlane/src`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway.ControlPlane/src/GatewayControlPlane.cs`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway.ControlPlane/src/GatewayControlPlaneOptions.cs`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway.ControlPlane/src/IResourceCommandDispatcher.cs`.
