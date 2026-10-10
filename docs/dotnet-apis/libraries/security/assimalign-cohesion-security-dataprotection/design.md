# Assimalign.Cohesion.Security.DataProtection design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Security.DataProtection`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

AES-256-GCM protects versioned payloads and HKDF-SHA256 derives purpose-specific subkeys. Lazy
rotation keeps retired keys available during a grace window. `IKeyRepository` separates persistence
from protection, allowing nodes to share a configured key directory.

The library ships no DI, logging, configuration, or hosted-service integration: a consumer wires it
at builder time and adapts the resulting `IDataProtector` to its own seam. For antiforgery that
seam is `IHttpAntiforgeryProtector`. `Http.Antiforgery` takes no dependency on this library; the
adapter ships in `Web.Antiforgery`, whose `AddAntiforgery(dataProtectionProvider)` derives a
protector for the purpose chain `("Assimalign.Cohesion.Web.Antiforgery", "v1")` and maps
`DataProtectionException` to an invalid token.

## Reloads and the unknown-key throttle

If a payload names a key not in the in-memory snapshot, the ring may reload from the repository
before failing: this is how a node picks up a key another node created after it last loaded. The key
id in a payload header comes from the client, though. Antiforgery tokens and authentication cookies
arrive from unauthenticated clients, and the ring has to resolve the id before the GCM tag can be
checked, because the tag needs the key. Until #1155, every unknown id reloaded the whole repository
under the ring's only lock, and every other protect and unprotect waited behind that read. One
request with a random key id bought a full repository read and a process-wide stall.

- **Snapshot.** The keys live in an immutable `FrozenDictionary` held in a volatile field. `Protect`
  and `Unprotect` read it without a lock. A reload or a key creation builds a new snapshot and swaps
  it in, one at a time under the reload lock, so snapshots are published in the order their reads
  ran. Protecting or unprotecting with a key the ring holds never waits on a repository read.
- **Throttle.** A miss may reload only once `DataProtectionOptions.UnknownKeyReloadInterval`
  (default 30 seconds, must be positive) has passed since the previous miss-triggered read began.
  Inside that window a miss costs one timestamp read and a second lookup in the current snapshot,
  and an id still not there is reported unknown without taking the lock. The window is measured
  from the start of the read, and a read that throws closes it too, so a failing repository is not
  read again on every miss.
- **Snapshot before window.** A reload publishes its snapshot before it closes the window, and a
  miss that finds the window closed looks its id up again after reading it. A miss whose first
  lookup ran before another miss's reload published, and that reached the window after it closed,
  therefore still finds the key that read loaded. The second lookup also sees a snapshot a
  protect-path reload published after the first one.
- **Single flight.** Misses that arrive while a reload runs wait for it on the lock, then look their
  id up in the snapshot it published. They share its result instead of reading again, so a burst of
  payloads under a freshly rotated key costs one read.
- **What single flight costs.** The window closes only when the read ends, so every miss that
  arrives during the read, invented ids included, holds its thread on the lock until the read
  finishes. Under a flood at rate *r* that parks about *r* × *d* threads once per interval, where
  *d* is the read's duration. With the file-system repository *d* is milliseconds. `Unprotect` runs
  synchronously on request threads, so a remote repository (#806) must keep its reads short or move
  the repository seam to an async read. Closing the window before the read would let those misses
  fail without the lock, but it would also reject the genuine burst under a freshly rotated key that
  arrives during the read, which is what single flight exists to serve, so it was not taken.
- **No per-id negative cache.** The throttle counts reloads, not ids. An id that is still unknown
  after a reload is reported unknown without another read until the window passes, which is all a
  negative cache would add. Against invented ids, which are new every time, a per-id cache would add
  nothing and would need its own memory bound.
- **The first miss after startup reloads at once.** The constructor's load does not open the
  window, so a node that starts just before another node rotates picks up the new key on first
  sight.
- **Monotonic time.** The window runs on `TimeProvider.GetTimestamp()`, so a wall clock stepped
  backward cannot hold it shut. Key lifetimes and grace still use `GetUtcNow()`, because they are
  persisted instants.
- **Protect-path reloads are not throttled.** `GetActiveKey` reloads only while its snapshot holds
  no active key, and the key it then finds or creates ends that, so while the repository works a
  client cannot make it repeat. It takes the reload lock, so its snapshot is ordered with the miss
  reloads, but it does not move the miss window. Rotation is exactly when misses are legitimate, and
  a protect that reloaded a moment earlier must not make another node's new key wait.
- **A failing repository makes every protect retry.** When the read, or the store of a new key,
  throws while no active key exists (an outage at key expiry, or a first start on a repository that
  cannot be written), no active key is published. Every later `Protect` then repeats the read and
  the write attempt under the reload lock until the repository recovers, and misses outside the
  window queue behind it. A client can drive that on any endpoint that hands antiforgery tokens to
  anonymous callers. `Protect` cannot succeed in that state anyway, and unprotecting with a key the
  ring holds stays lock-free. A failure backoff was not added, because it would keep every `Protect`
  failing for its length after the repository recovers.

**The propagation bound.** A key another node writes at time *t* resolves here no later than
*t* + `UnknownKeyReloadInterval`. If the last miss-triggered read started after *t*, it already
loaded the key. Otherwise it started before *t*, so its window closes before
*t* + `UnknownKeyReloadInterval`, and the payload that names the key is the miss that reloads. With
no other miss inside the preceding interval, the key resolves on first sight, as it did before the
throttle. Under a flood of invented ids, the repository is read once per interval, and a payload
under a just-rotated key can be rejected as unknown for up to one interval on nodes that did not
create the key. A shorter interval narrows that window and raises the worst-case read rate in
proportion.

## Error model

Every protection, verification, and key-lifecycle failure surfaces as `DataProtectionException`,
the area root, wrapping the underlying `CryptographicException` on authentication failure. Messages
never reveal key material or plaintext.

- **A repository read that fails during an unknown-key reload** surfaces from `Unprotect` as
  `DataProtectionException`, with the repository's exception as `InnerException`. `Unprotect` is fed
  untrusted input, its contract names only that type, and its callers (the antiforgery adapter, the
  cookie handler) catch only that type, so a token or cookie that names an invented key id is
  rejected rather than turned into an unhandled fault. Only the caller whose miss ran the read gets
  that message. Callers that waited on that read, and every miss inside the window it closes, get
  the ordinary unknown-key `DataProtectionException`.
- **A repository failure on the protect path** (`GetActiveKey`'s reload, or the store of a new key)
  still propagates unchanged from `Protect`, although `IDataProtector.Protect` documents
  `DataProtectionException` for an unreadable repository. That gap predates the throttle.

## Dependency boundary

The project declares no explicit Cohesion or external package references. Shared repository build
properties still apply.

## Sources

- **Source** — `cohesion/libraries/Security/Assimalign.Cohesion.Security.DataProtection/src/Assimalign.Cohesion.Security.DataProtection.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Security/Assimalign.Cohesion.Security.DataProtection/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Security/Assimalign.Cohesion.Security.DataProtection/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Security/README.md`.

- **Source** — `cohesion/libraries/Security/Assimalign.Cohesion.Security.DataProtection/src`.
