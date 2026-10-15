# Containers

Containers supplies shared image-index, artifact-store, and registry infrastructure for platform gateways.

> **Status:** Partial. Shared artifact machinery is implemented; general Kubernetes registry reachability remains separate work.

## Target and package

`Assimalign.Cohesion.ApplicationModel.Gateway.Containers` supports Docker and Kubernetes gateways.
It has no platform compiler and cannot be selected through `CohesionGateways`. Selecting Docker
or Kubernetes brings this shared dependency into the gateway.

The package owns image location, identity validation, indexing, and serving. It uses the generic
ApplicationModel and gateway contracts. Platform observation uses Cohesion's public
`InMemoryResourceStateManager`, not a duplicate Containers-owned state manager.

## Image contracts

| Artifact | Schema and purpose |
|---|---|
| `image.json` | `cohesion/image/v1`, one resource image identity. |
| `application.images.json` | `cohesion/images/v1`, resource-index entries gathered for an application. |
| Verified disk store | Content-addressed Open Container Initiative (OCI) and Docker-save image data. |
| Embedded registry | Pull-only OCI Distribution Registry API v2 over a bounded loopback HTTP/1.1 listener. |

An entry carries an authority-free repository, immutable digest, and lowercase OCI platform.
`ArtifactRef.Self` resolves only the current resource's entry. Missing entries, duplicate resource
names, tag-only identities, and digest mismatches fail before platform contact.

An omitted or null `registry` is late-bound to the selected target. A concrete registry authority
is pinned and cannot be replaced by a target override. An optional `archive` path is relative to
the index, cannot escape its directory, and is omitted when no archive is available.

The disk store verifies blobs before placing them under SHA-256 addresses. The loopback registry
supports `GET` and `HEAD` for stored manifests and repository-reachable blobs; it is not a push
registry. Its listener uses the base class library rather than importing a resource Web runtime.

## Local development and build ownership

The Cohesion SDK produces and gathers indexes. Gateways consume them and locate, verify, load,
or pull existing images during gathering. They do not compile application source or build images.

[Docker](../docker/index.md) uses the selected compatible engine, including Podman locally.
[Kubernetes](../kubernetes/index.md) can load an advertised archive into a selected Kind cluster.
Containers does not infer a registry address reachable from arbitrary Kubernetes nodes.

Return to [Platforms](../index.md).

## Sources

- **Area contract** — `cohesion-platforms/platforms/Containers/README.md`.
- **Image schema** — `cohesion-platforms/platforms/Containers/Assimalign.Cohesion.ApplicationModel.Gateway.Containers/docs/IMAGE_INDEX.md`.
- **Program boundaries** — `cohesion-platforms/docs/PLATFORMS_PROGRAM_PLAN.md`.
- **Dependency rules** — `cohesion-platforms/.claude/rules/platform-areas.md`.
- **Provider selection** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Targets/Sdk.Gateway.props`.
