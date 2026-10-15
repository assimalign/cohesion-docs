# Logger

Reusable abstract base class for `ILogger` implementations.

[Assembly overview](index.md) · [Design](design.md) · [Examples](examples/index.md)

Namespace: `Assimalign.Cohesion.Logging`.

Assembly: `Assimalign.Cohesion.Logging`.

## Remarks

Reusable abstract base class for `ILogger` implementations. Folds the per-call boilerplate
(null guards, level short-circuit) into non-virtual methods so derived classes only implement
the actual write and scope-creation work.

## Constructor

Throws `ArgumentException` when `category` is null or empty.

## Members

| Member | Description |
| --- | --- |
| `string Category { get; }` | The category the logger was created for. |
| `virtual bool IsEnabled(LogLevel level)` | Defaults to "any level except `None`". Override to factor in provider disposal or external state. |
| `void Log(ILoggerEntry entry)` | Non-virtual. Null-checks, calls `IsEnabled`, and dispatches to `WriteCore`. |
| `IScopedLogger BeginScope(ILoggerEntry entry)` | Non-virtual. Null-checks and dispatches to `BeginScopeCore`. |
| `protected abstract void WriteCore(ILoggerEntry entry)` | Derived classes implement the actual write. |
| `protected abstract IScopedLogger BeginScopeCore(ILoggerEntry entry)` | Derived classes implement scope creation. Derived classes that want the seed entry to appear in their sink output MUST call `WriteCore` explicitly before constructing the scope. |

## Devirtualization

`Log` and `BeginScope` are non-virtual; callers holding a strongly typed `Logger` (or a
sealed derived) reference pay a single virtual dispatch to `WriteCore` / `BeginScopeCore`
instead of two through the interface. Concrete implementations should be `sealed`.

## Sources

- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging/docs/Assembly/Assimalign.Cohesion.Logging/Logger/OVERVIEW.md`.
