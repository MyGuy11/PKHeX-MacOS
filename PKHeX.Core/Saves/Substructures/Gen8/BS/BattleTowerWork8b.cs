using System;
using System.Collections.Generic;
using System.ComponentModel;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Stores progress within the Battle Tower for the four battle modes.
/// </summary>
/// <remarks>size: 0x1B8</remarks>
[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class BattleTowerWork8b : SaveBlock<SAV8BS>
{
    private const int OFS_ClassData = 20;
    private const int COUNT_CLASSDATA = 4;

    public BattleTowerWork8b(SAV8BS sav, int offset) : base(sav) => Offset = offset;

    // Structure:
    // uint max_master_rank;
    // int play_mode;
    // int old_playmode;
    // uint btl_point;
    // uint day_challeng_cnt;
    // BTLTOWER_CLASSDATA[4] class_data;
    // uint challenge_cnt;
    public int MasterRankMax { get => ReadInt32LittleEndian(Data.AsSpan(Offset + 0x0)); set => WriteInt32LittleEndian(Data.AsSpan(Offset + 0x0), value); } // max_master_rank
    public int PlayMode      { get => ReadInt32LittleEndian(Data.AsSpan(Offset + 0x4)); set => WriteInt32LittleEndian(Data.AsSpan(Offset + 0x4), value); }// play_mode
    public int PlayModeOld   { get => ReadInt32LittleEndian(Data.AsSpan(Offset + 0x8)); set => WriteInt32LittleEndian(Data.AsSpan(Offset + 0x8), value); } // old_playmode
    public uint BP           { get => ReadUInt32LittleEndian(Data.AsSpan(Offset + 0xC)); set => WriteUInt32LittleEndian(Data.AsSpan(Offset + 0xC), value); } // btl_point

    public uint ChallengeCount { get => ReadUInt32LittleEndian(Data.AsSpan(Offset + 0x1B4)); set => WriteUInt32LittleEndian(Data.AsSpan(Offset + 0x1B4), value); } // challenge_cnt

    public BattleTowerClassData8b[] Records
    {
        get => GetRecords();
        set => SetRecords(value);
    }

    private BattleTowerClassData8b[] GetRecords()
    {
        var result = new BattleTowerClassData8b[COUNT_CLASSDATA];
        for (int i = 0; i < result.Length; i++)
            result[i] = new BattleTowerClassData8b(Data, Offset + OFS_ClassData + (i * BattleTowerClassData8b.SIZE));
        return result;
    }

    private static void SetRecords(IReadOnlyList<BattleTowerClassData8b> value)
    {
        if (value.Count != COUNT_CLASSDATA)
            throw new ArgumentException($"Expected {COUNT_CLASSDATA} items, received {value.Count}.", nameof(value));
        // data is already hard-referencing the original byte array. This is mostly a hack for Property Grid displays.
    }
}

[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class BattleTowerClassData8b
{
    public const int SIZE = 0x68;

    private readonly int Offset;
    private readonly byte[] Data;

    public BattleTowerClassData8b(byte[] data, int offset)
    {
        Data = data;
        Offset = offset;
    }

    public override string ToString() => $"Rank: {Rank}, Streak: {RenshouCount} (Max {RenshouCountOld}), Wins: {TotalWins}|{TotalWinsLoop}|{TotalWinsLose}";

    public byte Cleared
    {
        get => Data[Offset + 0x00];
        set => Data[Offset]= value;
    }
    public bool Suspended
    {
        get => ReadInt32LittleEndian(Data.AsSpan(Offset + 0x04)) == 1;
        set => WriteUInt32LittleEndian(Data.AsSpan(Offset + 0x04), value ? 1u : 0u);
    }
    public ulong BattlePlaySeed  { get => ReadUInt64LittleEndian(Data.AsSpan(Offset + 0x08)); set => WriteUInt64LittleEndian(Data.AsSpan(Offset + 0x08), value); }
    public uint Rank             { get => ReadUInt32LittleEndian(Data.AsSpan(Offset + 0x10)); set => WriteUInt32LittleEndian(Data.AsSpan(Offset + 0x10), value); }
    public uint RankDownLose     { get => ReadUInt32LittleEndian(Data.AsSpan(Offset + 0x14)); set => WriteUInt32LittleEndian(Data.AsSpan(Offset + 0x14), value); }

    public ulong TrainerSeed1 { get => ReadUInt64LittleEndian(Data.AsSpan(Offset + 0x1C)); set => WriteUInt64LittleEndian(Data.AsSpan(Offset + 0x1C), value); }
    public ulong TrainerSeed2 { get => ReadUInt64LittleEndian(Data.AsSpan(Offset + 0x24)); set => WriteUInt64LittleEndian(Data.AsSpan(Offset + 0x24), value); }
    public ulong TrainerSeed3 { get => ReadUInt64LittleEndian(Data.AsSpan(Offset + 0x2C)); set => WriteUInt64LittleEndian(Data.AsSpan(Offset + 0x2C), value); }
    public ulong TrainerSeed4 { get => ReadUInt64LittleEndian(Data.AsSpan(Offset + 0x34)); set => WriteUInt64LittleEndian(Data.AsSpan(Offset + 0x34), value); }
    public ulong TrainerSeed5 { get => ReadUInt64LittleEndian(Data.AsSpan(Offset + 0x3C)); set => WriteUInt64LittleEndian(Data.AsSpan(Offset + 0x3C), value); }
    public ulong TrainerSeed6 { get => ReadUInt64LittleEndian(Data.AsSpan(Offset + 0x44)); set => WriteUInt64LittleEndian(Data.AsSpan(Offset + 0x44), value); }
    public ulong TrainerSeed7 { get => ReadUInt64LittleEndian(Data.AsSpan(Offset + 0x4C)); set => WriteUInt64LittleEndian(Data.AsSpan(Offset + 0x4C), value); }

    public uint TotalWins        { get => ReadUInt32LittleEndian(Data.AsSpan(Offset + 0x54)); set => WriteUInt32LittleEndian(Data.AsSpan(Offset + 0x54), value); }
    public uint TotalWinsLoop    { get => ReadUInt32LittleEndian(Data.AsSpan(Offset + 0x58)); set => WriteUInt32LittleEndian(Data.AsSpan(Offset + 0x58), value); }
    public uint TotalWinsLose    { get => ReadUInt32LittleEndian(Data.AsSpan(Offset + 0x5C)); set => WriteUInt32LittleEndian(Data.AsSpan(Offset + 0x5C), value); }
    public uint RenshouCountOld  { get => ReadUInt32LittleEndian(Data.AsSpan(Offset + 0x60)); set => WriteUInt32LittleEndian(Data.AsSpan(Offset + 0x60), value); }
    public uint RenshouCount     { get => ReadUInt32LittleEndian(Data.AsSpan(Offset + 0x64)); set => WriteUInt32LittleEndian(Data.AsSpan(Offset + 0x64), value); }
}
