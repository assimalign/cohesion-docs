# Assimalign.Cohesion.ApplicationModel.Definition design

Design decisions and ownership boundaries for `Assimalign.Cohesion.ApplicationModel.Definition`.

> **Status:** Implemented.

[Overview](index.md) · [Examples](examples/index.md)

## Decisions

- **D1 — Resource names are scoped per gateway** — A resource is identified by provider and name
  within the gateway whose model declares it.
- **D2 — Definitions are build-time objects** — `Build()` closes composition; the gateway and any
  parent work from the model's manifest data, never from definition objects.
- **D4 — `DependsOn` records access and order** — A dependency names the permissions the dependent
  needs and orders deployment; dependencies cannot form cycles.
- **D16 — Outputs are placeholders** — `database.Endpoint` and other outputs are placeholders until
  the platform reports their values at deployment.
- **D17 — Settings are configuration values** — `WithSetting(name, value)` delivers a configuration
  value with the deployment; a value can be a literal, an output of this gateway, or an output of its
  parent.
- **D19 — Platform extensions apply only on their platform** — `WithExtension(platform, name, value)`
  records how one platform deploys a resource (Kubernetes replicas, for example); a gateway on
  another platform ignores it. Platform packages wrap it in typed verbs.
- **D21 — Each gateway has its own model, and a child joins its parent** — There is no hierarchy
  inside one model. A child gateway names its parent with `UseParent` and sends its manifest when it
  joins.
- **D23 — The logical model references the identity only** — Definition never references the
  protocol, so the gateway's model and an instance's runtime stay independent.

## Validation

- **D11 — One validation path** — Composition checks its rules early, at each `Add` and `DependsOn`,
  so an error points at the declaration that broke it: `DependsOn` refuses a dependency that leads
  back to its own resource and names the path. `Build()` then projects the manifest and creates the
  model with `ApplicationModel.Create(manifest)`, the same call a parent makes for a joining child's
  manifest. `Create` alone checks rules that span declarations, such as a setting that uses an output
  of a gateway that is neither this one nor its parent, and copies the lists read-only.

## Sources

- **Primary source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Definition/docs/DESIGN.md`.
- **Area design** — `cohesion/docs/libraries/ApplicationModel/DESIGN.md`.
