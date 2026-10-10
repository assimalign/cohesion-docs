# Cohesion Documentation

The Cohesion documentation micro-frontend package and its Viu WebAssembly preview application.

> **Status:** Partial. The package and shared-shell preview application are complete; composition by
> the `docs.assimalign.com` landing zone and its Cohesion Web host is a later phase.

The framework source lives at [assimalign/cohesion](https://github.com/assimalign/cohesion).
Content under `docs/` is the site's source of truth. `Assimalign.Cohesion.Docs` generates the catalog,
carries those pages as static package content, and contributes Cohesion routes and navigation to a
shared Assimalign frontend shell.

## Repository layout

```text
global.json                                   .NET and Viu SDK selection
NuGet.config                                  sibling package feeds and local cache
CohesionDocs.slnx                              package, tests, and preview application solution
packages/Assimalign.Cohesion.Docs/
  src/                                         host-neutral Viu micro-frontend package
  test/                                        catalog, route, navigation, and format tests
  docs/OVERVIEW.md                             package usage
  docs/DESIGN.md                               package boundaries and prefix decisions
app/Assimalign.Cohesion.Docs.App/
  Assimalign.Cohesion.Docs.App.csproj          thin shared-shell browser host
  Program.cs                                  host composition and browser startup
  LandingPage.viu                             minimal preview-only package link
  Properties/launchSettings.json               http://localhost:5180
  wwwroot/                                    bootstrap document and shared-helper import
docs/
  index.md                                    landing page and top-level navigation order
  database/                                   database guides and language references
  platforms/                                  deployment platform documentation
  dotnet-apis/                                 library, resource, and SDK references
```

## Local development

Use .NET SDK `10.0.400` or a compatible later feature band; `global.json` selects
`rollForward: latestFeature`. The application pins the Viu SDK and router packages to
`10.0.0-beta.12`, the Cohesion Viu packages to `10.0.0-beta.5`, and the shared frontend packages to
`1.0.0-preview.1`. Preview features are enabled because the Cohesion assemblies require them.

No public feed carries the `Assimalign.*` packages, so the four sibling feeds must be packed locally
first. `NuGet.config` clears inherited sources and maps each package family to its sibling feed:
`Assimalign.Viu.*` to `../viu/_out/packages`, `Assimalign.Cohesion.Viu.*` to
`../viu-platforms/_out/packages`, other `Assimalign.Cohesion.*` packages to
`../cohesion/_out/packages`, and `Assimalign.Frontend.*` to
`../../assimalign-core/aaln-core-frontend/_out/packages`. Other packages use NuGet.org. Restored
packages stay in `.nuget/packages`; no GitHub Packages source is configured.

1. **Cohesion** — `pwsh ../cohesion/installer/scripts/Install-Local.ps1` populates
   `../cohesion/_out/packages` (currently `10.0.1-preview.3.local`).
2. **Viu** — `pwsh ../viu/scripts/Install-Local.ps1 -Configuration Release` packs the
   `10.0.0-beta.12` set, including the Browser SDK and runtime pack (needs the `wasm-tools` workload).
3. **Viu platforms** — `pwsh ../viu-platforms/scripts/Install-Local.ps1 -Configuration Release`
   produces the `10.0.0-beta.5` Cohesion Viu packages.
4. **Shared frontend** — `pwsh ../../assimalign-core/aaln-core-frontend/scripts/Install-Local.ps1`
   produces `Assimalign.Frontend.MicroFrontends` and `Assimalign.Frontend.Components`
   `1.0.0-preview.1`.

Once the feeds are ready, run:

```pwsh
dotnet run --project app/Assimalign.Cohesion.Docs.App
```

Open `http://localhost:5180`. The launch profile sets both `applicationUrl` and `ASPNETCORE_URLS`
because the packaged development server gives the environment variable precedence.
`Assimalign.Cohesion.Viu.DevServer` serves the built browser assets and applies a single-page
application fallback for direct documentation route requests such as `/cohesion/database/sql`;
only the resource control plane (`/cohesion/v1` and its subtree) and the bare health probes are
reserved, never the `/cohesion` module prefix.

Run `dotnet build CohesionDocs.slnx -c Release` and
`dotnet test CohesionDocs.slnx -c Release --no-build` to verify the package and preview host.

## The `Assimalign.Cohesion.Docs` package

`CohesionDocsMicroFrontend` has descriptor ID `cohesion`, title `Cohesion`, route prefix
`/cohesion`, Cohesion brand accent, order `10`, and source repository
`https://github.com/assimalign/cohesion`. The Markdown generator builds routes beneath `/cohesion`
and asset paths beneath `_content/Assimalign.Cohesion.Docs/docs/`.

The package carries every `docs/**/*.md` file under `content/docs/` and supplies a transitive MSBuild
registration that maps restored content to `wwwroot/_content/Assimalign.Cohesion.Docs/docs/`. The
landing zone references the package, creates `CohesionDocsMicroFrontend` with its base-addressed
`HttpClient`, and adds it through `AddMicroFrontends`. The module owns its catalog, HTTP source, and
renderer, so hosts never call `AddMarkdownContent` for it.

## Routing and application design

File paths are routes beneath the module prefix: `docs/index.md` becomes `/cohesion`,
`docs/database/index.md` becomes `/cohesion/database`, and
`docs/database/sql/language/statements/select.md` becomes
`/cohesion/database/sql/language/statements/select`. The package's build generator supplies
`GeneratedMarkdownContent.Catalog`; neither the package nor the app has a manually maintained page
registry.

The application uses clean web history and `<base href="/">`. Shared micro-frontend infrastructure
qualifies fragment-only anchors with their page route, waits for asynchronously rendered headings,
and provides the layout/not-found route, cancellable page loading, table expansion, ordered
navigation, breadcrumbs, and previous/next links. The preview app only creates one shared component
factory, registers its landing component and the shared components, and composes the module.

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

The package is host-neutral and renders Markdown through the shared micro-frontend contract. The
preview application renders it in WebAssembly and its development host serves assets and fallback
routes. The dedicated `docs.assimalign.com` Cohesion Web host belongs to the landing-zone repository.

## Sources

- **Shared frontend contracts** — `aaln-core-frontend/README.md` and
  `aaln-core-frontend/libraries/Assimalign.Frontend.MicroFrontends/`.
- **Markdown integration** — `viu-platforms/platforms/cohesion/Assimalign.Cohesion.Viu.Markdown/docs/OVERVIEW.md` and `viu-platforms/platforms/cohesion/Assimalign.Cohesion.Viu.Markdown/docs/DESIGN.md`.
- **Catalog and runtime implementation** — `viu-platforms/platforms/cohesion/Assimalign.Cohesion.Viu.Markdown/shared/ContentExtraction.cs` and `viu-platforms/platforms/cohesion/Assimalign.Cohesion.Viu.Markdown/src/`.
- **Development host** — `viu-platforms/platforms/cohesion/Assimalign.Cohesion.Viu.DevServer/docs/OVERVIEW.md`.
- **Template and runtime contracts** — `viu/docs/SPECIFICATION.md`, `viu/libraries/Syntax/Assimalign.Viu.Syntax.SingleFileComponent/docs/FORMAT.md`, and `viu/libraries/Runtime/Assimalign.Viu.Reactivity/docs/OVERVIEW.md`.
- **Markdown parser** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Markdown/src/`.
- **Branding** — `branding/README.md`, `branding/svg/on-light/cohesion.svg`, and `branding/svg/on-dark/cohesion.svg`.
