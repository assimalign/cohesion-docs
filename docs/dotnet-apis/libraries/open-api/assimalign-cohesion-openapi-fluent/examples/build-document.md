# Example: Build an OpenAPI document

Author a version-targeted OpenAPI document with a server and operation.

[Examples](index.md) · [Assembly overview](../index.md)

## Code

```csharp
using Assimalign.Cohesion.OpenApi;
using Assimalign.Cohesion.OpenApi.Fluent;

var document = OpenApiDocumentBuilder.Create(OpenApiSpecVersion.V3_1, "Petstore", "1.0.0")
    .Info(i => i.Description("A sample API").License("MIT"))
    .Server("https://api.example.com")
    .Path("/pets", path => path
        .Operation(OperationType.Get, op => op
            .OperationId("listPets")
            .Response("200", r => r.Description("A list of pets"))))
    .Build();
```

## Walkthrough

The builder fixes the target specification version at creation. Nested callbacks attach metadata, a
server, and a path operation; `Build()` returns the ordinary model for later serialization or
validation.

## Sources

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Fluent/docs/OVERVIEW.md`.
