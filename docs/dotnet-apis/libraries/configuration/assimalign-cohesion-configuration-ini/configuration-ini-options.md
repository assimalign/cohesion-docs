# ConfigurationIniOptions

Options used to configure file-backed INI providers.

[Assembly overview](index.md) · [Design](design.md) · [Examples](examples/index.md)

Namespace: `Assimalign.Cohesion.Configuration.Ini`.

Assembly: `Assimalign.Cohesion.Configuration.Ini`.

## Remarks

Options used to configure file-backed INI providers. Inherits every option
from `Assimalign.Cohesion.Configuration.FileSystem.FileSystemConfigurationOptions`;
no INI-specific options are added today.

## Inherited properties

| Property | Type | Description |
| --- | --- | --- |
| `FileSystem` | `IFileSystem?` | The file system used to resolve `Path`. Required. |
| `Path` | `FileSystemPath` | The path to the INI file within the configured file system. Required (non-empty). |
| `Optional` | `bool` | When `true`, a missing file is silently ignored. Default `false`. |
| `ReloadOnChange` | `bool` | When `true`, the provider re-runs the parser when the file changes. Default `false`. |
| `ReloadDelay` | `TimeSpan` | Debounce delay between change notification and reload. Default 250 ms. Must be non-negative. |
| `OnLoadException` | `Action<ConfigurationFileLoadExceptionContext>?` | Callback invoked when load throws. Set `ctx.Ignore = true` to suppress propagation. |

## Sources

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.Ini/docs/Assembly/Assimalign.Cohesion.Configuration.Ini/ConfigurationIniOptions/OVERVIEW.md`.
