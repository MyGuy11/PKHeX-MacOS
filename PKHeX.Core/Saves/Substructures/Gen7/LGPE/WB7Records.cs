using System;

namespace PKHeX.Core;

public sealed class WB7Records : SaveBlock<SAV7b>
{
    public WB7Records(SAV7b sav, int offset) : base(sav) => Offset = offset;

    private const int RecordMax = 10; // 0xE90 > (0x140 * 0xA = 0xC80), not sure what final 0x210 bytes are used for
    private const int FlagCountMax = 0x1C00; // (7168) end of the block?

    private int FlagStart => Offset + (RecordMax * WR7.Size);

    private int GetRecordOffset(int index)
    {
        if (index >= RecordMax)
            throw new ArgumentOutOfRangeException(nameof(index));

        return Offset + (index * WR7.Size);
    }

    private int GetFlagOffset(int flag)
    {
        if (flag >= FlagCountMax)
            throw new ArgumentOutOfRangeException(nameof(flag));
        return FlagStart + (flag / 8);
    }

    public WR7 GetRecord(int index)
    {
        int ofs = GetRecordOffset(index);
        byte[] data = Data.AsSpan(ofs, WR7.Size).ToArray();
        return new WR7(data);
    }

    public void SetRecord(WR7 record, int index)
    {
        int ofs = GetRecordOffset(index);
        record.Data.CopyTo(Data, ofs);
    }

    public WR7[] GetRecords()
    {
        var arr = new WR7[RecordMax];
        for (int i = 0; i < arr.Length; i++)
            arr[i] = GetRecord(i);
        return arr;
    }

    public void SetRecords(WR7[] value)
    {
        for (int i = 0; i < value.Length; i++)
            SetRecord(value[i], i);
    }

    public bool GetFlag(int flag)
    {
        int ofs = GetFlagOffset(flag);
        var mask = 1 << (flag & 7);
        return (Data[ofs] & mask) != 0;
    }

    public void SetFlag(int flag, bool value)
    {
        int ofs = GetFlagOffset(flag);
        var mask = 1 << (flag & 7);
        if (value)
            Data[ofs] |= (byte)mask;
        else
            Data[ofs] &= (byte)~mask;
    }

    public bool[] GetFlags()
    {
        var value = new bool[FlagCountMax];
        for (int i = 0; i < value.Length; i++)
            value[i] = GetFlag(i);
        return value;
    }

    public void SetFlags(ReadOnlySpan<bool> value)
    {
        for (int i = 0; i < value.Length; i++)
            SetFlag(i, value[i]);
    }
}
