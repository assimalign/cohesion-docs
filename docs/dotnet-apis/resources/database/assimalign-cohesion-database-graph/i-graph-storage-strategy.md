# IGraphStorageStrategy (removed)

> **Status:** Removed from the public API.

The former storage strategy interface has no public replacement. GraphStorageStrategy is an internal
engine implementation detail. Configure file-backed storage through GraphDatabaseEngineOptions.RootPath;
omitting RootPath uses in-memory storage.

[Graph API](index.md) · [Design](design.md)

## Sources

- **Public storage options** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/GraphDatabaseEngineOptions.cs`.
- **Internal boundary** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/Internal/Storage/GraphStorageStrategy.cs`.
