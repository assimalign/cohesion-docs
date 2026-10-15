# Assimalign.Cohesion.App runtime pack

This project produces the RID-specific runtime pack for the Cohesion hosting
kernel. It is a packaging shell: the policy lives in
`libraries/App/Assimalign.Cohesion.App.props`, and the collection mechanics live in
`libraries/App/Assimalign.Cohesion.App.targets`.

The base `Assimalign.Cohesion.Sdk` does not reference this framework implicitly.
Resource-area SDKs reference App together with `App.<Area>`; a direct base-SDK
executable adds its own explicit `FrameworkReference`.

## Derived kernel

`@(CohesionAppKernelRoot)` is the only curated input. It names Hosting and its
Health, Resources, and Telemetry siblings; the CommandLine,
EnvironmentVariables, FileSystem, and Json configuration providers;
Logging.Console and Logging.EventSource; OpenTelemetry; DependencyInjection; FileSystem.Physical;
and Connections with its StreamAbort, StreamLimits, and Tls capability packages. Connections belongs in the kernel because generated resource
accessors and every area hosting module depend on it.
The runtime project references those roots, and the pack target derives their
transitive `Assimalign.Cohesion.*` project-reference closure before writing
`RuntimeList.xml` and `FrameworkList.xml`. The App umbrella assembly is added to
that derived set.

The framework tests independently walk the same `libraries/**` and
`resources/**` project index. They enforce both the App kernel closure and each
area framework's coverage, including whether a dependency is public or a
runtime-only implementation detail.

Libraries outside the kernel are ordinary NuGet packages. Adding one requires a
consumer package reference; it must not be appended directly to App's assembly
list.

## Examples

- **[Framework manifest tests](examples/framework-manifest-tests.md)** — Verify the seventeen kernel roots and their closure, every area framework’s coverage, and the location of each area list.
- **[Web EventSource conformance tests](examples/web-event-source-conformance-tests.md)** — Inspect every App.Web member and check strict manifests, assembly ownership, documented sources, and deliberately uninstrumented Web assemblies.

[App](../index.md) · [All examples](examples/index.md)

## Sources

- **Primary source** — `cohesion/libraries/App/Assimalign.Cohesion.App.Runtime/README.md`.
- **Kernel membership** — `cohesion/libraries/App/Assimalign.Cohesion.App.props`.
- **Build logic** — `cohesion/libraries/App/Assimalign.Cohesion.App.targets`.
