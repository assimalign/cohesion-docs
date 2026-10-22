# Cohesion Documentation

The Cohesion documentation micro-frontend package, its Viu WebAssembly client, and its Cohesion Web preview host.

> **Status:** Partial. The package, shared-shell client, and standalone Web preview host are complete;
> composition by the `docs.assimalign.com` landing zone is a later phase.

The framework source lives at [assimalign/cohesion](https://github.com/assimalign/cohesion).
Content under `docs/` is the site's source of truth. `Assimalign.Cohesion.Docs` generates the catalog,
carries those pages as static package content, and contributes Cohesion routes and navigation to a
shared Assimalign frontend shell.

## Repository layout

```text
global.json                                   .NET, Viu, and Cohesion SDK selection
NuGet.config                                  GitHub Packages feed and local cache
Directory.Build.props                         Viu, Cohesion Viu, and shared frontend package pins
CohesionDocs.slnx                              package, tests, Client, and Web solution
packages/Assimalign.Cohesion.Docs/
  src/                                         host-neutral Viu micro-frontend package
  test/                                        catalog, route, navigation, and format tests
  docs/OVERVIEW.md                             package usage
  docs/DESIGN.md                               package boundaries and prefix decisions
app/Assimalign.Cohesion.Docs.Client/
  Assimalign.Cohesion.Docs.Client.csproj        Viu Browser client
  Program.cs                                  components, routing, and browser startup
  LandingPage.viu                              minimal preview-only package link
  wwwroot/                                    bootstrap document and shared-helper import
app/Assimalign.Cohesion.Docs.Web/
  Assimalign.Cohesion.Docs.Web.csproj           Cohesion Web host with marked client ProjectReference
  Program.cs                                  client middleware and host lifetime
  appsettings.Local.json                      local HTTP endpoint at http://localhost:5000
  Properties/launchSettings.json               selects COHESION_ENVIRONMENT=Local
docs/
  index.md                                    landing page and top-level navigation order
  database/                                   database guides and language references
  platforms/                                  deployment platform documentation
  dotnet-apis/                                 library, resource, and SDK references
```

## Version line

- Viu SDK and router: `10.0.0-preview.0.ci.7`
- Cohesion SDKs: `10.0.0-preview.0.ci.70`
- `Assimalign.Cohesion.Viu.*`: `10.0.0-preview.0.ci.12`
- `Assimalign.Frontend.*`: `1.0.0-preview.1.ci.2`

## Local development

Use .NET SDK `10.0.400` or a compatible later feature band; `global.json` selects
`rollForward: latestFeature`. Preview features are enabled because the Cohesion assemblies require
them.

Every `Assimalign.*` package restores from the Assimalign GitHub Packages feed
(`https://nuget.pkg.github.com/assimalign/index.json`); no sibling checkout has to be packed first.
The SDK and package pins move together so the runtime and hosting builds stay compatible:

- the `Assimalign.Viu.Sdk` entry in `global.json` selects the Viu SDK, and `Directory.Build.props`
  reads it as `ViuVersion` for every Viu package reference (keep `Assimalign.Viu.Sdk.Browser` equal);
- the `Assimalign.Cohesion.Sdk` and `Assimalign.Cohesion.Sdk.Web` entries in `global.json` select
  the Cohesion framework build required by the Web host and Cohesion Viu packages;
- `CohesionViuVersion` in `Directory.Build.props` selects the viu-platforms build;
- `FrontendVersion` in `Directory.Build.props` selects the aaln-core-frontend build, published by
  that repository's Azure pipeline as `1.0.0-preview.1.ci.<run>`.

CI pins are temporary: viu and viu-platforms keep only their newest 20 `main` builds. GitHub Packages
requires a personal access token (classic) with `read:packages`, and the shared frontend packages
are private to the `assimalign` organization. Never write the token to `NuGet.config`; supply it
from the environment:

```pwsh
$env:NuGetPackageSourceCredentials_assimalign = "Username=<github-user>;Password=$(gh auth token);ValidAuthenticationTypes=Basic"
```

Then run:

```pwsh
dotnet run --project app/Assimalign.Cohesion.Docs.Web
```

Open `http://localhost:5000`. The Web launch profile selects `COHESION_ENVIRONMENT=Local`, and
`appsettings.Local.json` configures the HTTP endpoint. The Web project builds the Browser client
through a normal `ProjectReference` marked `ViuClient="true"`. `AddViuClient()` discovers the generated
`viu-client.json` build output; `UseViuApplication()` serves the client's asset manifests and applies
the SPA fallback for direct documentation routes such as `/cohesion/database/sql`. No manually
configured client output paths or separate development-server package are needed.

Publishing the Web project also publishes the trimmed Browser client and bundles its `wwwroot`:

```pwsh
dotnet publish app/Assimalign.Cohesion.Docs.Web -c Release
```

Only the resource control plane (`/cohesion/v1` and its subtree) and the bare health probes are
reserved, never the `/cohesion` module prefix. To add HTTP APIs, register `builder.Services.AddRouting()`
and call `application.UseRouting()` before `UseViuApplication()`. Selected endpoints take precedence
over client content without an API branch.

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
`HttpClient`, and adds it through `builder.Services.AddMicroFrontends`. The module owns its catalog,
HTTP source, and renderer, so hosts never call `AddMarkdownContent` for it.

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
navigation, breadcrumbs, and previous/next links. The preview app registers everything in
`builder.Services`: its own components (`AddComponents`), the shared components
(`AddFrontendComponents`), its landing route (`AddRouting`), and the module (`AddMicroFrontends`),
then installs the router with `application.UseRouter()` after building.

Use the parameterless `AddComponents()` to register the compile-time aggregate of the application
and its referenced component libraries. It can be combined with `AddFrontendComponents()` without
registering a catalog twice. Keep generated catalog aggregation enabled (the SDK default); explicit
`GeneratedViuComponents.Register` callbacks bypass that deduplication.

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
Client project renders it in WebAssembly. The Web project owns the HTTP pipeline and process lifetime,
serves the client and documentation assets, and can compose API middleware. The production
`docs.assimalign.com` landing zone remains a separate composing host.

## Sources

- **Shared frontend contracts** — `aaln-core-frontend/README.md` and
  `aaln-core-frontend/libraries/Assimalign.Frontend.MicroFrontends/`.
- **Markdown integration** — `viu-platforms/platforms/cohesion/Assimalign.Cohesion.Viu.Markdown/docs/OVERVIEW.md` and `viu-platforms/platforms/cohesion/Assimalign.Cohesion.Viu.Markdown/docs/DESIGN.md`.
- **Catalog and runtime implementation** — `viu-platforms/platforms/cohesion/Assimalign.Cohesion.Viu.Markdown/shared/ContentExtraction.cs` and `viu-platforms/platforms/cohesion/Assimalign.Cohesion.Viu.Markdown/src/`.
- **Web host** — `viu-platforms/platforms/cohesion/Assimalign.Cohesion.Viu.Server/docs/OVERVIEW.md`.
- **Template and runtime contracts** — `viu/docs/SPECIFICATION.md`, `viu/libraries/Syntax/Assimalign.Viu.Syntax.SingleFileComponent/docs/FORMAT.md`, and `viu/libraries/Runtime/Assimalign.Viu.Reactivity/docs/OVERVIEW.md`.
- **Markdown parser** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Markdown/src/`.
- **Branding** — `branding/README.md`, `branding/svg/on-light/cohesion.svg`, and `branding/svg/on-dark/cohesion.svg`.
