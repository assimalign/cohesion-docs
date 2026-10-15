# ConfigurationIniStreamProvider

Stream-backed INI configuration provider.

[Assembly overview](index.md) · [Design](design.md) · [Examples](examples/index.md)

Namespace: `Assimalign.Cohesion.Configuration.Ini`.

Assembly: `Assimalign.Cohesion.Configuration.Ini`.

## Remarks

Stream-backed INI configuration provider. Extends
`Assimalign.Cohesion.Configuration.ConfigurationProvider` directly because
there is no file system involved.

## Constructor

| Parameter | Description |
| --- | --- |
| `stream` | The INI stream to parse. Required. |
| `leaveOpen` | When `true`, the stream is left open when the provider is disposed. When `false`, the provider disposes the stream. Default `false`. |

Throws `ArgumentNullException` if `stream` is null.

## Members

| Member | Description |
| --- | --- |
| `Name` (override) | Stable provider name in the form `ConfigurationIniStreamProvider[<hash>]` where `<hash>` is the runtime identity hash of the stream. |
| `OnLoadAsync(IDictionary<Path, string?>, CancellationToken)` (override) | Rewinds the stream to position 0 if `Stream.CanSeek`, then delegates parsing to `IniConfigurationParser.ParseAsync`. |
| `OnDisposeAsync(...)` (override) | Disposes the underlying stream if `leaveOpen == false`. |

## Reload semantics

Stream providers don't watch external resources for change - there's nothing to
watch on a generic `Stream`. The `OnLoadAsync` rewind-and-reparse behavior
means a caller that explicitly triggers a load through the configuration model
will get a fresh read of the stream content.

## Sources

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.Ini/docs/Assembly/Assimalign.Cohesion.Configuration.Ini/ConfigurationIniStreamProvider/OVERVIEW.md`.
