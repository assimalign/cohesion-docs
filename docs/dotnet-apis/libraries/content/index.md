# Content

Format-neutral content contracts and binary, text, document, and media format packages.

[Libraries](../index.md)

## Packages

| Package | Responsibility | Reference |
|---|---|---|
| `Assimalign.Cohesion.Content` | Defines format-neutral content identity, stream ownership, and reader and writer contracts. | [Overview](assimalign-cohesion-content/index.md) |
| `Assimalign.Cohesion.Content.Binary` | Reserves shared binary-content contracts beneath concrete binary formats. | [Overview](assimalign-cohesion-content-binary/index.md) |
| `Assimalign.Cohesion.Content.Bmff` | Contains an incomplete ISO base media file-format box model and reader and writer surface. | [Overview](assimalign-cohesion-content-bmff/index.md) |
| `Assimalign.Cohesion.Content.Ebml` | Contains the retained Extensible Binary Meta Language (EBML) document and element implementation. | [Overview](assimalign-cohesion-content-ebml/index.md) |
| `Assimalign.Cohesion.Content.Exe` | Reserves executable-content contracts over the binary-content layer. | [Overview](assimalign-cohesion-content-exe/index.md) |
| `Assimalign.Cohesion.Content.Markdown` | Parses and writes a documented CommonMark subset and renders its document model as HTML. | [Overview](assimalign-cohesion-content-markdown/index.md) |
| `Assimalign.Cohesion.Content.Media` | Reserves shared media contracts above binary content. | [Overview](assimalign-cohesion-content-media/index.md) |
| `Assimalign.Cohesion.Content.Mkv` | Reserves the Matroska content package. | [Overview](assimalign-cohesion-content-mkv/index.md) |
| `Assimalign.Cohesion.Content.Mpeg` | Reserves the MPEG content assembly. | [Overview](assimalign-cohesion-content-mpeg/index.md) |
| `Assimalign.Cohesion.Content.Pdf` | Reserves the Portable Document Format (PDF) content assembly. | [Overview](assimalign-cohesion-content-pdf/index.md) |
| `Assimalign.Cohesion.Content.Text` | Provides text encoding detection, line reading, and configurable tokenization. | [Overview](assimalign-cohesion-content-text/index.md) |
| `Assimalign.Cohesion.Content.Yaml` | Parses YAML documents and emits deterministic YAML through the content contracts. | [Overview](assimalign-cohesion-content-yaml/index.md) |

## Dependencies and delivery

The repository places this area in delivery wave 3. The wave is a dependency-ordering guide, not a
release or completeness guarantee.

Format support varies substantially: Markdown, YAML, and text have implementations, while several
binary and media packages remain scaffolds or partial implementations. The tables below report
declared project references, not intended future format relationships.

| Assembly | Declared dependencies |
|---|---|
| `Assimalign.Cohesion.Content` | No explicit Cohesion or package reference in the project file |
| `Assimalign.Cohesion.Content.Binary` | `Assimalign.Cohesion.Content` (CohesionProjectReference) |
| `Assimalign.Cohesion.Content.Bmff` | `Assimalign.Cohesion.Content.Media` (CohesionProjectReference) |
| `Assimalign.Cohesion.Content.Ebml` | No explicit Cohesion or package reference in the project file |
| `Assimalign.Cohesion.Content.Exe` | `Assimalign.Cohesion.Content.Binary` (CohesionProjectReference) |
| `Assimalign.Cohesion.Content.Markdown` | `Assimalign.Cohesion.Content` (CohesionProjectReference), `Assimalign.Cohesion.Content.Text` (CohesionProjectReference) |
| `Assimalign.Cohesion.Content.Media` | `Assimalign.Cohesion.Content.Binary` (CohesionProjectReference) |
| `Assimalign.Cohesion.Content.Mkv` | `Assimalign.Cohesion.Content.Bmff` (CohesionProjectReference) |
| `Assimalign.Cohesion.Content.Mpeg` | No explicit Cohesion or package reference in the project file |
| `Assimalign.Cohesion.Content.Pdf` | No explicit Cohesion or package reference in the project file |
| `Assimalign.Cohesion.Content.Text` | `Assimalign.Cohesion.Content` (CohesionProjectReference) |
| `Assimalign.Cohesion.Content.Yaml` | `Assimalign.Cohesion.Content` (CohesionProjectReference), `Assimalign.Cohesion.Content.Text` (CohesionProjectReference) |

## Sources

- **Source** — `cohesion/libraries/README.md`.

- **Source** — `cohesion/README.md`.

- **Source** — `cohesion/docs/programs/DELIVERY_ROADMAP.md`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Content/README.md`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content/src/Assimalign.Cohesion.Content.csproj`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content/src`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Binary/src/Assimalign.Cohesion.Content.Binary.csproj`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Binary/src`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Bmff/src/Assimalign.Cohesion.Content.Bmff.csproj`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Bmff/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Bmff/src`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Ebml/src/Assimalign.Cohesion.Content.Ebml.csproj`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Ebml/src`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Exe/src/Assimalign.Cohesion.Content.Exe.csproj`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Exe/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Exe/src`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Markdown/src/Assimalign.Cohesion.Content.Markdown.csproj`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Markdown/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Markdown/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Markdown/src`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Media/src/Assimalign.Cohesion.Content.Media.csproj`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Media/src`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Mkv/src/Assimalign.Cohesion.Content.Mkv.csproj`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Mkv/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Mkv/src`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Mpeg/src/Assimalign.Cohesion.Content.Mpeg.csproj`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Mpeg/src`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Pdf/src/Assimalign.Cohesion.Content.Pdf.csproj`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Pdf/src`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Text/src/Assimalign.Cohesion.Content.Text.csproj`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Text/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Text/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Text/src`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Yaml/src/Assimalign.Cohesion.Content.Yaml.csproj`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Yaml/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Yaml/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Yaml/src`.
