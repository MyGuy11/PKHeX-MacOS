using System;
using System.Diagnostics;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class HallFame3Entry
{
    private readonly byte[] Parent;
    private readonly bool Japanese;
    private readonly int Offset;

    private const int Count = 6;
    public const int SIZE = Count * HallFame3PKM.SIZE;

    public HallFame3Entry(byte[] data, int offset, bool japanese)
    {
        Parent = data;
        Japanese = japanese;
        Offset = offset;
    }

    private int GetMemberOffset(int index) => Offset + (index * HallFame3PKM.SIZE);
    private HallFame3PKM GetMember(int index) => new(Parent, GetMemberOffset(index), Japanese);

    public HallFame3PKM[] Team
    {
        get
        {
            var team = new HallFame3PKM[6];
            for (int i = 0; i < Count; i++)
                team[i] = GetMember(i);
            return team;
        }
    }

    private const int MaxEntries = 50;
    private const int MaxLength = MaxEntries * SIZE;

    public static HallFame3Entry[] GetEntries(SAV3 sav)
    {
        byte[] data = sav.GetHallOfFameData();
        Debug.Assert(data.Length > MaxLength);
        bool Japanese = sav.Japanese;

        var entries = new HallFame3Entry[MaxEntries];
        for (int i = 0; i < entries.Length; i++)
            entries[i] = new HallFame3Entry(data, SIZE, Japanese);
        return entries;
    }

    public static void SetEntries(SAV3 sav, HallFame3Entry[] entries)
    {
        byte[] data = entries[0].Team[0].Data;
        sav.SetHallOfFameData(data);
    }
}

public sealed class HallFame3PKM : ISpeciesForm
{
    public const int SIZE = 20;

    public HallFame3PKM(byte[] data, int offset, bool jp)
    {
        Data = data;
        Offset = offset;
        Japanese = jp;
    }

    public readonly byte[] Data;
    private readonly int Offset;
    private readonly bool Japanese;

    public int TID16 { get => ReadUInt16LittleEndian(Data.AsSpan(0 + Offset)); set => WriteUInt16LittleEndian(Data.AsSpan(0 + Offset), (ushort)value); }
    public int SID16 { get => ReadUInt16LittleEndian(Data.AsSpan(2 + Offset)); set => WriteUInt16LittleEndian(Data.AsSpan(2 + Offset), (ushort)value); }
    public uint PID { get => ReadUInt32LittleEndian(Data.AsSpan(4 + Offset)); set => WriteUInt32LittleEndian(Data.AsSpan(4 + Offset), value); }
    private int SpecLevel { get => ReadUInt16LittleEndian(Data.AsSpan(8 + Offset)); set => WriteUInt16LittleEndian(Data.AsSpan(8 + Offset), (ushort)value); }

    private Span<byte> Nickname_Trash => Data.AsSpan(10 + Offset, 10);
    public string Nickname { get => StringConverter3.GetString(Nickname_Trash, Japanese); set => StringConverter3.SetString(Nickname_Trash, value, 10, Japanese, StringConverterOption.ClearZero); }

    public ushort Species { get => (ushort)(SpecLevel & 0x1FF); set => SpecLevel = (SpecLevel & 0xFE00) | value; }
    public byte Form => 0; // no forms; derive Unown's from PID else use the Version for Deoxys.
    public int Level { get => SpecLevel >> 9; set => SpecLevel = (SpecLevel & 0x1FF) | (value << 9); }
}
