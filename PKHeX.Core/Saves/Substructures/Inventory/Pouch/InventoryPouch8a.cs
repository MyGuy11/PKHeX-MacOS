using System;

namespace PKHeX.Core;

/// <summary>
/// Represents the data in a pouch pocket containing items of a similar type group.
/// </summary>
/// <remarks>
/// Used by <see cref="GameVersion.PLA"/>.
/// </remarks>
public sealed class InventoryPouch8a : InventoryPouch
{
    private readonly int MaxSize;

    public InventoryPouch8a(InventoryType type, ushort[] legal, int maxCount, int size, int offset = 0,
        Func<ushort, bool>? isLegal = null)
        : base(type, legal, maxCount, offset, isLegal: isLegal) => MaxSize = size;

    public override InventoryItem8a GetEmpty(int itemID = 0, int count = 0) => new() { Index = itemID, Count = count };

    public override void GetPouch(ReadOnlySpan<byte> data)
    {
        var items = new InventoryItem8a[MaxSize];

        for (int i = 0; i < items.Length; i++)
            items[i] = GetItem(data, i << 2);

        Items = items;
    }

    public static InventoryItem8a GetItem(ReadOnlySpan<byte> data, int ofs) => InventoryItem8a.Read(data[ofs..]);

    public override void SetPouch(Span<byte> data)
    {
        var items = (InventoryItem8a[])Items;
        for (var i = 0; i < items.Length; i++)
        {
            int ofs = i * 4;
            items[i].Write(data[ofs..]);
        }
    }

    public void SanitizeCounts()
    {
    }
}
