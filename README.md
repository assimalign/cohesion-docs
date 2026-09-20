# Cohesion Documentation

Markdown documentation and a Viu WebAssembly application for the Cohesion framework.

> **Status:** Partial. The browser application builds, runs on the packaged Cohesion development
> server, and renders every page in the tree. A Web server rendering host is a later phase.

The framework source lives at [assimalign/cohesion](https://github.com/assimalign/cohesion).
Content under `docs/` is the site's source of truth. The application uses
`Assimalign.Cohesion.Viu.Markdown` to generate a catalog during compilation, fetch Markdown assets,
and render Viu nodes in the browser. It follows the sibling `viu-docs` application's composition.

## Repository layout

```text
global.json                                   .NET and Viu SDK selection
NuGet.config                                  sibling package feeds and local cache
CohesionDocs.slnx                              application solution
app/Assimalign.Cohesion.Docs.App/
  Assimalign.Cohesion.Docs.App.csproj           browser project and linked content assets
  Program.cs                                  hosting, catalog, router, and rendering policy
  CohesionDocsComponentCatalog.cs              explicit component registrations
  DocsRoutes.cs                               page routes beneath AppShell
  DocsPageView.cs                              cancellable Markdown page loading
  PipeTables.cs                               pipe-table extraction and node expansion
  NavigationTree.cs                           ordered rows, breadcrumbs, previous/next links
  Components/AppShell.viu                      responsive application layout
  FragmentScrolling.cs                        JavaScript heading-wait import
  Properties/launchSettings.json               http://localhost:5180
  wwwroot/                                    bootstrap, CSS, and Cohesion SVG marks
  checks/                                     bootstrap helper tests requiring only Node.js
docs/
  index.md                                    landing page and top-level navigation order
  database/                                   database guides and language references
  platforms/                                  deployment platform documentation
  dotnet-apis/                                 library, resource, and SDK references
```

## Local development

Use .NET SDK `10.0.400` or a compatible later feature band; `global.json` selects
`rollForward: latestFeature`. The application pins the Viu SDK and router packages to
`10.0.0-beta.12`, and the Cohesion Viu Markdown, Hosting, Hosting.Browser, and DevServer packages to
`10.0.0-beta.4`. Preview features are enabled because the Cohesion assemblies require them.

No public feed carries the `Assimalign.*` packages, so the three sibling feeds must be packed
locally first. `NuGet.config` clears inherited sources and maps each package family to its sibling
feed (`Assimalign.Viu.*` → `../viu/_out/packages`, `Assimalign.Cohesion.Viu.*` →
`../viu-platforms/_out/packages`, everything else `Assimalign.Cohesion.*` →
`../cohesion/_out/packages`). Other packages use NuGet.org. Restored packages stay in
`.nuget/packages`; no GitHub Packages source is configured.

1. **Cohesion** — `pwsh ../cohesion/installer/scripts/Install-Local.ps1` populates
   `../cohesion/_out/packages` (currently `10.0.1-preview.3.local`).
2. **Viu** — `pwsh ../viu/scripts/Install-Local.ps1 -Configuration Release` packs the
   `10.0.0-beta.12` set, including the Browser SDK and runtime pack (needs the `wasm-tools` workload).
3. **Viu platforms** — the repository pins Cohesion `10.0.0-beta.1`, which only exists on the
   authenticated GitHub Packages feed. To build against the local Cohesion feed instead, pack from a
   copy of the sources (kept at `../viu-platforms/_out/src`, which is gitignored) with four
   adjustments: `ServiceProviderBuilder.Services` became `Container` in Cohesion, the DevServer
   project's inline SDK pin and `global.json` must name the local Cohesion SDK version, the Server
   project needs `<OutputType>Library</OutputType>` because the newer Web SDK defaults to `Exe`, and
   `Assimalign.Cohesion.Viu.Markdown/build/Assimalign.Cohesion.Viu.Markdown.targets` must compute its
   content root with `$([MSBuild]::NormalizePath(...))` instead of the two-argument
   `Path.GetFullPath`, which does not exist on the .NET Framework MSBuild that Visual Studio uses
   (otherwise Visual Studio fails to load the project with `MSB4186`). Then
   pack `Hosting`, `Markdown`, `Hosting.Browser`, `Server`, and `DevServer` with
   `-p:CohesionVersion=10.0.1-preview.3.local -p:PackageOutputPath=<viu-platforms>\_out\packages`
   and a NuGet configuration that maps `Assimalign.Cohesion.*` to the Cohesion feed.

Once the feeds are ready, run:

```pwsh
dotnet run --project app/Assimalign.Cohesion.Docs.App
```

Open `http://localhost:5180`. The launch profile sets both `applicationUrl` and `ASPNETCORE_URLS`
because the packaged development server gives the environment variable precedence.
`Assimalign.Cohesion.Viu.DevServer` selects the Viu SDK's custom-server integration and serves the
manifest-described assets with a single-page application fallback for direct route requests.

The heading-wait helper can be checked independently, without .NET or package restore:

```sh
node --test app/Assimalign.Cohesion.Docs.App/checks/fragment-scrolling.test.mjs
```

## Routing and application design

File paths are routes: `docs/index.md` becomes `/`, `docs/database/index.md` becomes `/database`,
and `docs/database/sql/language/statements/select.md` becomes
`/database/sql/language/statements/select`. The build generator supplies
`GeneratedMarkdownContent.Catalog`; the app has no manually maintained page registry.

The application uses clean web history and `<base href="/">`. The Markdown `LinkResolver` qualifies
fragment-only anchors with their page route so the base element does not send them to `/`.
Relative `.md` links use `RouterLink`. `ScrollBehavior` waits for destination headings through
`globalThis.cohesionDocs.waitForHeading`, then delegates scrolling to the router. The helper bounds
each wait to five seconds and cancels an earlier wait on subsequent navigation. There is no
hash-bookmark migration or click interception. New page navigation scrolls to the top when no
heading or saved position applies.

`DocsRoutes` retains the layout and not-found behavior of `MarkdownRoutes`, selecting `DocsPageView`
for page content. `DocsPageView` preserves the package view's cancellation, request-version guard,
loading and error states, update lifecycle, unmount cleanup, and server-prefetch seam.
`MarkdownComponents.Register` remains part of the explicit component factory registration.

`NavigationTree` interleaves pages and child sections using their shared `Order` values. Each
section uses its `index.md` as its landing link, without listing that page twice. The tree is
flattened into `NavigationRow` records, then visibility and active state are derived from the
expanded-section `Reference<T>` and `Router.CurrentRoute`. Route changes expand the current page's
section chain. Section toggle buttons remain independent of landing-page links. Missing section
indexes still have a toggle and label, with no invented route.

`AppShell` keeps `<RouterView :depth="1" />` and the reference app's typed-method workaround for
assimalign/viu#366: `GetRows()` supplies the loop's concrete element type. A nested `v-if` runs inside
the `v-for` scope. Breadcrumbs follow the page's section chain; previous/next links follow
`MarkdownContentCatalog.Pages`. The page's H1 is rendered only by Markdown.

## Page format

Each content page starts with an H1, one description sentence, and an optional status blockquote.
There is no YAML frontmatter.

```text
# Page title

One sentence describing the page, with only optional inline `code` markup.

> **Status:** Implemented.
```

- **Line 1** — the page's single H1 supplies its title; use H2 or deeper for subsequent headings.
- **Line 3** — one plain description sentence supplies catalog metadata.
- **Line 5** — an optional status blockquote starts exactly with `> **Status:**`.
- **Paths** — filenames and folders use lowercase kebab-case because they define routes.
- **Section titles** — a folder's `index.md` H1 supplies its navigation title.
- **Navigation order** — relative `.md` links in `index.md` order direct pages and child sections;
  unlinked entries follow in ordinal path order. Link every direct child page and one existing page
  in each child folder, in the intended order.
- **Internal links** — use relative `.md` destinations, optionally with heading fragments, and
  link only to existing pages. Heading identifiers are deterministic lowercase slugs.
- **Sources** — end each content page with a short `## Sources` section listing repository-prefixed
  source paths, such as `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`.

Use exactly these status callouts, optionally followed by clarifying prose on the same blockquote:

| Callout | Meaning |
|---|---|
| `> **Status:** Implemented.` | The documented behavior is implemented. |
| `> **Status:** Partial.` | Only part of the described surface is implemented; name the gaps. |
| `> **Status:** Not yet implemented.` | A marker or declaration exists without the described behavior. |
| `> **Status:** Planned — not present in the codebase.` | The planned surface is absent from source. |

## Markdown and tables

The underlying `Assimalign.Cohesion.Content.Markdown` parser implements a CommonMark subset and
has no table node. This application adds ordinary GitHub Flavored Markdown pipe tables through
`PipeTables`; table support is an application extension.

```text
| Syntax | Support | Notes |
|:---|:---:|---:|
| `SELECT` | Implemented | [Reference](select.md) |
```

Tables need a header and delimiter row with matching cell counts. Use one row per line, optional
outer pipes, `\|` for a literal pipe, and `:---`, `:---:`, or `---:` for left, center, or right
alignment. Cells support inline code, emphasis, and links. Missing body cells become empty cells;
extra body cells are omitted. Use prose outside tables for multi-paragraph explanations.

`PipeTables.Extract` skips fenced code and replaces table blocks with `gfm-table` fenced markers.
That info string is reserved for this extension. `PipeTables.Expand` replaces the rendered markers
with immutable `table`, `thead`, `tbody`, `tr`, `th`, and `td` nodes. Cell content goes through the
same `MarkdownText.Parse` and `MarkdownVirtualNodeRenderer` as page content, retaining internal link
resolution. Wide tables live in keyboard-focusable `.table-scroll` containers.

- **Supported authoring** — use ATX headings, paragraphs, blockquotes, ordered and bullet lists,
  fenced code, thematic breaks, emphasis, strong text, code spans, links, and images.
- **Raw HTML** — renders literally; do not use it for page formatting.
- **Unsupported forms** — avoid setext headings, indented code blocks, reference links, footnotes,
  task lists, nested tables, and multiline table cells.
- **Code languages** — label fences with `csharp`, `sql`, `syntaxsql`, `viu`, `xml`, `json`, `sh`,
  `pwsh`, `text`, or `mermaid`. Mermaid is displayed as code, not rendered as a diagram.
- **C# samples** — show complete `using` blocks; implicit usings are disabled. Put `System.*`
  before third-party and `Assimalign.*` namespaces, and verify APIs against their source.
- **House style** — wrap prose near 100 characters, put identifiers in inline code, and use bold
  lead-ins followed by an em dash in bulleted lists. Use tables for mappings and matrices.

## Hosting boundary

The current application renders Markdown in WebAssembly. Its development host serves assets and
fallback routes. A dedicated Cohesion Web host with request-time page rendering is a later phase;
the app does not currently implement server-rendered documentation responses.

## Sources

- **Reference application** — `viu-docs/README.md` and `viu-docs/app/Assimalign.Viu.Docs.App/`.
- **Markdown integration** — `viu-platforms/platforms/cohesion/Assimalign.Cohesion.Viu.Markdown/docs/OVERVIEW.md` and `viu-platforms/platforms/cohesion/Assimalign.Cohesion.Viu.Markdown/docs/DESIGN.md`.
- **Catalog and runtime implementation** — `viu-platforms/platforms/cohesion/Assimalign.Cohesion.Viu.Markdown/shared/ContentExtraction.cs` and `viu-platforms/platforms/cohesion/Assimalign.Cohesion.Viu.Markdown/src/`.
- **Development host** — `viu-platforms/platforms/cohesion/Assimalign.Cohesion.Viu.DevServer/docs/OVERVIEW.md`.
- **Template and runtime contracts** — `viu/docs/SPECIFICATION.md`, `viu/libraries/Syntax/Assimalign.Viu.Syntax.SingleFileComponent/docs/FORMAT.md`, and `viu/libraries/Runtime/Assimalign.Viu.Reactivity/docs/OVERVIEW.md`.
- **Markdown parser** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Markdown/src/`.
- **Branding** — `branding/README.md`, `branding/svg/on-light/cohesion.svg`, and `branding/svg/on-dark/cohesion.svg`.
