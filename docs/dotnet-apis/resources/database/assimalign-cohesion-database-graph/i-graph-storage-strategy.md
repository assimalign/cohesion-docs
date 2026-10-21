# IGraphStorageStrategy (removed)

The former interface is removed from the public API.

> **Status:** Implemented. Configure storage through engine options.

The former storage strategy interface has no public replacement. GraphStorageStrategy is an internal
engine implementation detail. Configure file-backed storage through GraphDatabaseEngineOptions.RootPath;
omitting RootPath uses in-memory storage.

[Graph API](index.md) · [Design](design.md)

## Sources

- **Public storage options** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/GraphDatabaseEngineOptions.cs`.
- **Internal boundary** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/Internal/Engine/Storage/GraphStorageStrategy.cs`.
