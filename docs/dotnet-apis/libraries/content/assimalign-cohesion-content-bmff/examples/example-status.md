# Example: Availability

Current example availability for `Assimalign.Cohesion.Content.Bmff`.

> **Status:** Not yet implemented.

[Examples](index.md) · [Assembly overview](../index.md)

Explicit box factories separate container structure from codecs. Many box and visitor operations
remain unimplemented, and the reader's unknown-box error is not a stable malformed-input contract.
Disposing the default reader closes its stream; the media fixture test requires a developer-local
file.

An executable usage example is not supplied for this surface. The closest checked-in test is
`cohesion/libraries/Content/Assimalign.Cohesion.Content.Bmff/tests/UnitTest1.cs`. Read it as
evidence of the current scaffold or neighboring behavior, not as proof of a completed
implementation.

## Sources

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Bmff/src/Assimalign.Cohesion.Content.Bmff.csproj`.
- **Source** — `cohesion/libraries/Directory.Build.props`.
- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Bmff/docs/DESIGN.md`.
- **Source** — `cohesion/libraries/Content/README.md`.
- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Bmff/src`.
- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Bmff/tests/UnitTest1.cs`.
