using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class InventoryPouch3 : InventoryPouch
{
    public uint SecurityKey { private get; set; } // = 0 // Gen3 Only

    public InventoryPouch3(InventoryType type, ushort[] legal, int maxCount, int offset, int size)
        : base(type, legal, maxCount, offset, size)
    {
    }

    public override void GetPouch(ReadOnlySpan<byte> data)
    {
        var span = data[Offset..];
        var items = new InventoryItem[PouchDataSize];
        for (int i = 0; i < items.Length; i++)
        {
            var entry = span.Slice(i * 4, 4);
            items[i] = new InventoryItem
            {
                Index = ReadUInt16LittleEndian(entry),
                Count = ReadUInt16LittleEndian(entry[2..]) ^ (ushort) SecurityKey,
            };
        }
        Items = items;
    }

    public override void SetPouch(Span<byte> data)
    {
        if (Items.Length != PouchDataSize)
            throw new ArgumentException("Item array length does not match original pouch size.");

        var span = data[Offset..];
        for (int i = 0; i < Items.Length; i++)
        {
            var item = Items[i];
            var entry = span.Slice(i * 4, 4);
            WriteUInt16LittleEndian(entry, (ushort)item.Index);
            WriteUInt16LittleEndian(entry[2..], (ushort)(item.Count ^ (int)SecurityKey));
        }
    }

    public override InventoryItem GetEmpty(int itemID = 0, int count = 0) => new() { Index = itemID, Count = count };
}
