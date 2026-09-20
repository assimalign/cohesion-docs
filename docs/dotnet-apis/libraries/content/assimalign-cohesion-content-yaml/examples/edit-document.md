# Example: Edit a YAML mapping

Parse a YAML mapping, read a scalar, and add a boolean value.

[Examples](index.md) · [Assembly overview](../index.md)

## Code

```csharp
using Assimalign.Cohesion.Content.Yaml;

var document = YamlText.ParseDocument("""
    name: Cohesion
    servers:
      - url: https://api.example.com
    """);

var root = (YamlMapping)document.Root!;
var name = ((YamlScalar)root["name"]).Value;

root.Add("enabled", new YamlScalar(true));
var text = YamlText.Write(document);
```

## Walkthrough

The root is a `YamlMapping` containing scalar values and a sequence. Adding a `YamlScalar(true)`
retains a typed boolean, and `YamlText.Write` emits the updated document.

## Sources

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Yaml/docs/OVERVIEW.md`.
