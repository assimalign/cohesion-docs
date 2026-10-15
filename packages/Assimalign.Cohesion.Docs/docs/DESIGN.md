# Assimalign.Cohesion.Docs design

## Boundaries

The package contains no application shell, browser startup code, or custom CSS. The shared
`Assimalign.Frontend.MicroFrontends` package supplies documentation rendering and navigation, while
`Assimalign.Frontend.Components` belongs to the composing host. This keeps the package usable by the
preview app and the `docs.assimalign.com` landing zone without embedding either host.

## Route and asset prefixes

The Markdown generator uses `ViuMarkdownRouteBase=/cohesion`, so catalog routes are already
`/cohesion`, `/cohesion/database`, and deeper descendants. `DocumentationMicroFrontend` recognizes
routes already beneath the module descriptor's `/cohesion` prefix and converts them to child route
paths; it does not add the prefix a second time. Setting the generator route base to `/` would leave
navigation and rendered Markdown links unprefixed even though route records were composed beneath
the module.

The independent asset prefix is `_content/Assimalign.Cohesion.Docs/docs/`. It is passed to the HTTP
content source and matches the static paths produced by the package's transitive MSBuild content
registration. Assets are not embedded in the WebAssembly assembly.

## Content contract

The repository-level `README.md` defines page headings, descriptions, status callouts, navigation
ordering, supported Markdown, pipe tables, and house style. The generated catalog preserves those
rules. Tests compare the catalog count to the physical Markdown tree, validate all asset and route
prefixes, pin root ordering and contextual navigation, and report every invalid status callout with
its source path and line number.
