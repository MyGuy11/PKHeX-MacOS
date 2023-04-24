using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class PlayTime9 : SaveBlock<SAV9SV>
{
    public PlayTime9(SAV9SV sav, SCBlock block) : base(sav, block.Data) { }

    public int PlayedHours
    {
        get => ReadInt32LittleEndian(Data.AsSpan(Offset));
        set => WriteInt32LittleEndian(Data.AsSpan(Offset), (ushort)value);
    }

    public int PlayedMinutes
    {
        get => ReadInt32LittleEndian(Data.AsSpan(Offset + 4));
        set => WriteInt32LittleEndian(Data.AsSpan(Offset + 4), (ushort)value);
    }

    public int PlayedSeconds
    {
        get => ReadInt32LittleEndian(Data.AsSpan(Offset + 8));
        set => WriteInt32LittleEndian(Data.AsSpan(Offset + 8), (ushort)value);
    }
}
