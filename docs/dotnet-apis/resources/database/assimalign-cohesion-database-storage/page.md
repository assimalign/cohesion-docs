# Page

Overlays a fixed-size page buffer owned by the caller or the storage pool.

Namespace: `Assimalign.Cohesion.Database.Storage.Units`.

## Signature

The caller-memory and span members are:

```csharp
public readonly unsafe struct Page
{
    public const int Size = 8192;
    public const int HeaderSize = 96;
    public Page(byte* pointer);
    public Page(byte* pointer, int bufferLength);
    public int OverflowSize { get; set; }
    public Span<byte> AsSpan();
    public Span<byte> AsBodySpan();
}
```

## Behavior and exceptions

The caller keeps the pointer valid for the page's lifetime. Both constructors throw
ArgumentNullException for null. The length-taking overload requires at least Size bytes, throwing
ArgumentOutOfRangeException before reading any header if shorter; extra bytes are ignored. The
pointer-only overload assumes a full 8 KiB buffer.

Ordinary pages expose 8,192 total bytes and 8,096 body bytes, ignoring OverflowSize. With
PageFlags.Overflow, the reserved layout exposes HeaderSize + OverflowSize bytes and OverflowSize
body bytes. Both accessors read/validate the size once before creating the span. Outside 0..8,096,
they throw StorageCorruptionException with the header page id. Pool loads use the physical page id.
A default Page throws InvalidOperationException on either accessor before dereferencing. This is
bounded single-buffer geometry; multi-page overflow storage remains unimplemented.

## Example

From PageTests.AsSpan_ValidOverflowHeader_ShouldReturnBoundedSpans:

```csharp
unsafe {
    var bytes = new byte[Page.Size + 16];
    fixed (byte* pointer = bytes) {
        var page = new Page(pointer, bytes.Length);
        page.Flags = PageFlags.Overflow;
        page.OverflowSize = 17;
        Console.WriteLine(page.AsSpan().Length);     // 113
        Console.WriteLine(page.AsBodySpan().Length); // 17
        page.AsBodySpan().Fill(0x11); // extra bytes beyond Page.Size are untouched
    }
}
```

## Related reference

[Assembly](index.md) · [Page design](design.md#the-page-model) · [Corruption](storage-corruption-exception.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Storage/src/Units/Page.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Storage/tests/PageTests.cs`.
