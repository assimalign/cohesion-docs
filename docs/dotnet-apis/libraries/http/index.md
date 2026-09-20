# Http

Hypertext Transfer Protocol (HTTP) contracts, connection handling, and optional exchange features.

[Libraries](../index.md)

## Packages

| Package | Responsibility | Reference |
|---|---|---|
| `Assimalign.Cohesion.Http` | Defines Hypertext Transfer Protocol (HTTP) messages, fields, features, and exchange contracts. | [Overview](assimalign-cohesion-http/index.md) |
| `Assimalign.Cohesion.Http.Antiforgery` | Protects HTTP requests with paired antiforgery cookie and request tokens. | [Overview](assimalign-cohesion-http-antiforgery/index.md) |
| `Assimalign.Cohesion.Http.ClientFactory` | Creates named HTTP clients while pooling and rotating their message handlers. | [Overview](assimalign-cohesion-http-clientfactory/index.md) |
| `Assimalign.Cohesion.Http.Connections` | Carries HTTP exchanges over Cohesion stream and multiplexed connections. | [Overview](assimalign-cohesion-http-connections/index.md) |
| `Assimalign.Cohesion.Http.Cookies` | Adds typed request and response cookies to the HTTP feature model. | [Overview](assimalign-cohesion-http-cookies/index.md) |
| `Assimalign.Cohesion.Http.DigestFields` | Parses and verifies HTTP integrity digest fields. | [Overview](assimalign-cohesion-http-digestfields/index.md) |
| `Assimalign.Cohesion.Http.ExtendedConnect` | Exposes extended CONNECT exchanges through an optional HTTP feature. | [Overview](assimalign-cohesion-http-extendedconnect/index.md) |
| `Assimalign.Cohesion.Http.Forms` | Parses URL-encoded and multipart form bodies into typed collections. | [Overview](assimalign-cohesion-http-forms/index.md) |
| `Assimalign.Cohesion.Http.Forwarded` | Defines effective request identity after a trusted proxy has forwarded an exchange. | [Overview](assimalign-cohesion-http-forwarded/index.md) |
| `Assimalign.Cohesion.Http.InterimResponses` | Sends interim HTTP responses through the exchange interceptor seam. | [Overview](assimalign-cohesion-http-interimresponses/index.md) |
| `Assimalign.Cohesion.Http.ProtocolUpgrade` | Models HTTP/1.1 upgrades and CONNECT tunnels. | [Overview](assimalign-cohesion-http-protocolupgrade/index.md) |
| `Assimalign.Cohesion.Http.RequestLimits` | Exposes a per-request view of the maximum request body size. | [Overview](assimalign-cohesion-http-requestlimits/index.md) |
| `Assimalign.Cohesion.Http.ServerSentEvents` | Formats Server-Sent Events and writes them through HTTP response streaming. | [Overview](assimalign-cohesion-http-serversentevents/index.md) |
| `Assimalign.Cohesion.Http.Sessions` | Defines per-exchange binary session state and typed convenience access. | [Overview](assimalign-cohesion-http-sessions/index.md) |
| `Assimalign.Cohesion.Http.Streaming` | Writes HTTP response bodies incrementally through an optional feature. | [Overview](assimalign-cohesion-http-streaming/index.md) |

## Dependencies and delivery

The repository places this area in delivery wave 3. The wave is a dependency-ordering guide, not a
release or completeness guarantee.

| Assembly | Declared dependencies |
|---|---|
| `Assimalign.Cohesion.Http` | `Assimalign.Cohesion.Core` (CohesionProjectReference) |
| `Assimalign.Cohesion.Http.Antiforgery` | `Assimalign.Cohesion.Http` (CohesionProjectReference), `Assimalign.Cohesion.Http.Cookies` (CohesionProjectReference), `Assimalign.Cohesion.Http.Forms` (CohesionProjectReference) |
| `Assimalign.Cohesion.Http.ClientFactory` | `Assimalign.Cohesion.Http` (CohesionProjectReference) |
| `Assimalign.Cohesion.Http.Connections` | `Assimalign.Cohesion.Connections` (CohesionProjectReference), `Assimalign.Cohesion.Http` (CohesionProjectReference) |
| `Assimalign.Cohesion.Http.Cookies` | `Assimalign.Cohesion.Http` (CohesionProjectReference) |
| `Assimalign.Cohesion.Http.DigestFields` | `Assimalign.Cohesion.Http` (CohesionProjectReference) |
| `Assimalign.Cohesion.Http.ExtendedConnect` | `Assimalign.Cohesion.Http` (CohesionProjectReference) |
| `Assimalign.Cohesion.Http.Forms` | `Assimalign.Cohesion.Http` (CohesionProjectReference) |
| `Assimalign.Cohesion.Http.Forwarded` | `Assimalign.Cohesion.Http` (CohesionProjectReference) |
| `Assimalign.Cohesion.Http.InterimResponses` | `Assimalign.Cohesion.Http` (CohesionProjectReference) |
| `Assimalign.Cohesion.Http.ProtocolUpgrade` | `Assimalign.Cohesion.Http` (CohesionProjectReference), `Assimalign.Cohesion.Http.Cookies` (CohesionProjectReference) |
| `Assimalign.Cohesion.Http.RequestLimits` | `Assimalign.Cohesion.Http` (CohesionProjectReference) |
| `Assimalign.Cohesion.Http.ServerSentEvents` | `Assimalign.Cohesion.Http` (CohesionProjectReference), `Assimalign.Cohesion.Http.Streaming` (CohesionProjectReference) |
| `Assimalign.Cohesion.Http.Sessions` | `Assimalign.Cohesion.Http` (CohesionProjectReference) |
| `Assimalign.Cohesion.Http.Streaming` | `Assimalign.Cohesion.Http` (CohesionProjectReference) |

## Sources

- **Source** — `cohesion/libraries/README.md`.

- **Source** — `cohesion/README.md`.

- **Source** — `cohesion/docs/programs/DELIVERY_ROADMAP.md`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Http/README.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/Assimalign.Cohesion.Http.csproj`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Antiforgery/src/Assimalign.Cohesion.Http.Antiforgery.csproj`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Antiforgery/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Antiforgery/src`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ClientFactory/src/Assimalign.Cohesion.Http.ClientFactory.csproj`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ClientFactory/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ClientFactory/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ClientFactory/src`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Connections/src/Assimalign.Cohesion.Http.Connections.csproj`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Connections/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Connections/src`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Cookies/src/Assimalign.Cohesion.Http.Cookies.csproj`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Cookies/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Cookies/src`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.DigestFields/src/Assimalign.Cohesion.Http.DigestFields.csproj`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.DigestFields/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.DigestFields/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.DigestFields/src`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ExtendedConnect/src/Assimalign.Cohesion.Http.ExtendedConnect.csproj`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ExtendedConnect/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ExtendedConnect/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ExtendedConnect/src`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Forms/src/Assimalign.Cohesion.Http.Forms.csproj`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Forms/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Forms/src`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Forwarded/src/Assimalign.Cohesion.Http.Forwarded.csproj`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Forwarded/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Forwarded/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Forwarded/src`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.InterimResponses/src/Assimalign.Cohesion.Http.InterimResponses.csproj`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.InterimResponses/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.InterimResponses/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.InterimResponses/src`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ProtocolUpgrade/src/Assimalign.Cohesion.Http.ProtocolUpgrade.csproj`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ProtocolUpgrade/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ProtocolUpgrade/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ProtocolUpgrade/src`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.RequestLimits/src/Assimalign.Cohesion.Http.RequestLimits.csproj`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.RequestLimits/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.RequestLimits/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.RequestLimits/src`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ServerSentEvents/src/Assimalign.Cohesion.Http.ServerSentEvents.csproj`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ServerSentEvents/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ServerSentEvents/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ServerSentEvents/src`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Sessions/src/Assimalign.Cohesion.Http.Sessions.csproj`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Sessions/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Sessions/src`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Streaming/src/Assimalign.Cohesion.Http.Streaming.csproj`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Streaming/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Streaming/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Streaming/src`.
