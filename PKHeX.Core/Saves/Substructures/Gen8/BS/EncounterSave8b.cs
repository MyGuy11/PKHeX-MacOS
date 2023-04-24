﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Encounter Save Data
/// </summary>
/// <remarks>size 0x188, struct_name ENC_SV_DATA</remarks>
[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class EncounterSave8b : SaveBlock<SAV8BS>
{
    public const int COUNT_HONEYTREE = 21;
    private const int SIZE_HillBackData = 0x8;
    private const int SIZE_HoneyTree = 8 + 4 + (COUNT_HONEYTREE * HoneyTree8b.SIZE); // 0x108
    private const int SIZE_Roamer = 0x20;
    private const int SIZE_Closing = 10 + 2; // 2 padding alignment

    private const int OFS_HillBackData = 0xC;
    private const int OFS_HoneyTree = OFS_HillBackData + SIZE_HillBackData; // 0x14
    private const int OFS_SWAY = OFS_HoneyTree + SIZE_HoneyTree; // 0x11C
    private const int OFS_ZONEHISTORY = OFS_SWAY + (4 * 6); // 0x134
    private const int OFS_ROAM1 = OFS_ZONEHISTORY + 4 + 4; // 0x13C
    private const int OFS_ROAM2 = OFS_ROAM1 + SIZE_Roamer; // 0x15C
    private const int OFS_CLOSING = OFS_ROAM2 + SIZE_Roamer; // 0x17C
    private const int SIZE = OFS_CLOSING + SIZE_Closing; // 0x188

    public EncounterSave8b(SAV8BS sav, int offset) : base(sav) => Offset = offset;

    public void Clear() => Data.AsSpan(Offset, SIZE).Clear();

    public int EncounterWalkCount
    {
        get => ReadInt32LittleEndian(Data.AsSpan(Offset + 0x00));
        set => WriteInt32LittleEndian(Data.AsSpan(Offset + 0x00), value);
    }

    public uint SafariRandSeed
    {
        get => ReadUInt32LittleEndian(Data.AsSpan(Offset + 0x04));
        set => WriteUInt32LittleEndian(Data.AsSpan(Offset + 0x04), value);
    }

    public uint GenerateRandSeed
    {
        get => ReadUInt32LittleEndian(Data.AsSpan(Offset + 0x08));
        set => WriteUInt32LittleEndian(Data.AsSpan(Offset + 0x08), value);
    }

    // HILL_BACK_DATA
    public bool HillTalkFlag
    {
        get => ReadUInt32LittleEndian(Data.AsSpan(Offset + OFS_HillBackData + 0x00)) == 1;
        set => WriteUInt32LittleEndian(Data.AsSpan(Offset + OFS_HillBackData + 0x00), value ? 1u : 0u);
    }
    public ushort HillEncTblIdx1
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(Offset + OFS_HillBackData + 0x04));
        set => WriteUInt16LittleEndian(Data.AsSpan(Offset + OFS_HillBackData + 0x04), value);
    }
    public ushort HillEncTblIdx2
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(Offset + OFS_HillBackData + 0x06));
        set => WriteUInt16LittleEndian(Data.AsSpan(Offset + OFS_HillBackData + 0x06), value);
    }

    // HONEY_TREE
    public long HoneyLastUpdateMinutes
    {
        get => ReadInt64LittleEndian(Data.AsSpan(Offset + OFS_HoneyTree + 0x00));
        set => WriteInt64LittleEndian(Data.AsSpan(Offset + OFS_HoneyTree + 0x00), value);
    }
    public byte HoneyTreeNo
    {
        get => Data[Offset + OFS_HoneyTree + 0x08];
        set => Data[Offset + OFS_HoneyTree + 0x08] = value;
    }

    public HoneyTree8b[] HoneyTrees
    {
        get => GetTrees();
        set => SetTrees(value);
    }

    private HoneyTree8b[] GetTrees()
    {
        var result = new HoneyTree8b[COUNT_HONEYTREE];
        for (int i = 0; i < result.Length; i++)
            result[i] = new HoneyTree8b(Data, Offset + OFS_HoneyTree + 0xC + (i * HoneyTree8b.SIZE));
        return result;
    }

    private static void SetTrees(IReadOnlyList<HoneyTree8b> value)
    {
        if (value.Count != COUNT_HONEYTREE)
            throw new ArgumentOutOfRangeException(nameof(value.Count));
        // data is already hard-referencing the original byte array. This is mostly a hack for Property Grid displays.
    }

    public uint Radar1Species { get => ReadUInt32LittleEndian(Data.AsSpan(Offset + OFS_SWAY + 0x00)); set => WriteUInt32LittleEndian(Data.AsSpan(Offset + OFS_SWAY + 0x00), value); }
    public uint Radar1Chain   { get => ReadUInt32LittleEndian(Data.AsSpan(Offset + OFS_SWAY + 0x04)); set => WriteUInt32LittleEndian(Data.AsSpan(Offset + OFS_SWAY + 0x04), value); }
    public uint Radar2Species { get => ReadUInt32LittleEndian(Data.AsSpan(Offset + OFS_SWAY + 0x08)); set => WriteUInt32LittleEndian(Data.AsSpan(Offset + OFS_SWAY + 0x08), value); }
    public uint Radar2Chain   { get => ReadUInt32LittleEndian(Data.AsSpan(Offset + OFS_SWAY + 0x0C)); set => WriteUInt32LittleEndian(Data.AsSpan(Offset + OFS_SWAY + 0x0C), value); }
    public uint Radar3Species { get => ReadUInt32LittleEndian(Data.AsSpan(Offset + OFS_SWAY + 0x10)); set => WriteUInt32LittleEndian(Data.AsSpan(Offset + OFS_SWAY + 0x10), value); }
    public uint Radar3Chain   { get => ReadUInt32LittleEndian(Data.AsSpan(Offset + OFS_SWAY + 0x14)); set => WriteUInt32LittleEndian(Data.AsSpan(Offset + OFS_SWAY + 0x14), value); }

    public int BeforeZone { get => ReadInt32LittleEndian(Data.AsSpan(Offset + OFS_ZONEHISTORY + 0x00)); set => WriteInt32LittleEndian(Data.AsSpan(Offset + OFS_ZONEHISTORY + 0x00), value); }
    public int OldZone    { get => ReadInt32LittleEndian(Data.AsSpan(Offset + OFS_ZONEHISTORY + 0x04)); set => WriteInt32LittleEndian(Data.AsSpan(Offset + OFS_ZONEHISTORY + 0x04), value); }

    // Mesprit
    public int   Roamer1ZoneID  { get =>  ReadInt32LittleEndian(Data.AsSpan(Offset + OFS_ROAM1 + 0x00)); set => WriteInt32LittleEndian(Data.AsSpan(Offset + OFS_ROAM1 + 0x00), value); }
    public ulong Roamer1Seed    { get => ReadUInt64LittleEndian(Data.AsSpan(Offset + OFS_ROAM1 + 0x04)); set => WriteUInt64LittleEndian(Data.AsSpan(Offset + OFS_ROAM1 + 0x04), value); }
    public uint  Roamer1Species { get => ReadUInt32LittleEndian(Data.AsSpan(Offset + OFS_ROAM1 + 0x0C)); set => WriteUInt32LittleEndian(Data.AsSpan(Offset + OFS_ROAM1 + 0x0C), value); }
    public uint  Roamer1HP      { get => ReadUInt32LittleEndian(Data.AsSpan(Offset + OFS_ROAM1 + 0x10)); set => WriteUInt32LittleEndian(Data.AsSpan(Offset + OFS_ROAM1 + 0x10), value); }
    public byte  Roamer1Level   { get => Data[Offset + OFS_ROAM1 + 0x14]; set => Data[Offset + OFS_ROAM1 + 0x14] = value; }
    public uint  Roamer1Status  { get => ReadUInt32LittleEndian(Data.AsSpan(Offset + OFS_ROAM1 + 0x18)); set => WriteUInt32LittleEndian(Data.AsSpan(Offset + OFS_ROAM1 + 0x18), value); }
    public byte  Roamer1Encount { get => Data[Offset + OFS_ROAM1 + 0x1C]; set => Data[Offset + OFS_ROAM1 + 0x1C] = value; }

    // Cresselia
    public int   Roamer2ZoneID  { get =>  ReadInt32LittleEndian(Data.AsSpan(Offset + OFS_ROAM2 + 0x00)); set => WriteInt32LittleEndian(Data.AsSpan(Offset + OFS_ROAM2 + 0x00), value); }
    public ulong Roamer2Seed    { get => ReadUInt64LittleEndian(Data.AsSpan(Offset + OFS_ROAM2 + 0x04)); set => WriteUInt64LittleEndian(Data.AsSpan(Offset + OFS_ROAM2 + 0x04), value); }
    public uint  Roamer2Species { get => ReadUInt32LittleEndian(Data.AsSpan(Offset + OFS_ROAM2 + 0x0C)); set => WriteUInt32LittleEndian(Data.AsSpan(Offset + OFS_ROAM2 + 0x0C), value); }
    public uint  Roamer2HP      { get => ReadUInt32LittleEndian(Data.AsSpan(Offset + OFS_ROAM2 + 0x10)); set => WriteUInt32LittleEndian(Data.AsSpan(Offset + OFS_ROAM2 + 0x10), value); }
    public byte  Roamer2Level   { get => Data[Offset + OFS_ROAM2 + 0x14]; set => Data[Offset + OFS_ROAM2 + 0x14] = value; }
    public uint  Roamer2Status  { get => ReadUInt32LittleEndian(Data.AsSpan(Offset + OFS_ROAM2 + 0x18)); set => WriteUInt32LittleEndian(Data.AsSpan(Offset + OFS_ROAM2 + 0x18), value); }
    public byte  Roamer2Encount { get => Data[Offset + OFS_ROAM2 + 0x1C]; set => Data[Offset + OFS_ROAM2 + 0x1C] = value; }

    public bool GenerateValid
    {
        get => ReadUInt32LittleEndian(Data.AsSpan(Offset + OFS_CLOSING + 0)) == 1;
        set => WriteUInt32LittleEndian(Data.AsSpan(Offset + OFS_CLOSING + 0), value ? 1u : 0u);
    }
    public short SprayCount
    {
        get => ReadInt16LittleEndian(Data.AsSpan(Offset + OFS_CLOSING + 4));
        set => WriteInt16LittleEndian(Data.AsSpan(Offset + OFS_CLOSING + 4), value);
    }
    public byte SprayType       { get => Data[Offset + OFS_CLOSING + 6]; set => Data[Offset + OFS_CLOSING + 6] = value; }
    public byte VsSeekerCharge  { get => Data[Offset + OFS_CLOSING + 7]; set => Data[Offset + OFS_CLOSING + 7] = value; } // max 100
    public byte PokeRadarCharge { get => Data[Offset + OFS_CLOSING + 8]; set => Data[Offset + OFS_CLOSING + 8] = value; } // max 50
    public byte FluteType       { get => Data[Offset + OFS_CLOSING + 9]; set => Data[Offset + OFS_CLOSING + 9] = value; } // vidro
}

[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class HoneyTree8b
{
    public const int SIZE = 0xC;

    private readonly int Offset;
    private readonly byte[] Data;

    public HoneyTree8b(byte[] data, int offset)
    {
        Data = data;
        Offset = offset;
    }

    public bool Spreaded
    {
        get => ReadUInt32LittleEndian(Data.AsSpan(Offset + 0x00)) == 1;
        set => WriteUInt32LittleEndian(Data.AsSpan(Offset + 0x00), value ? 1u : 0u);
    }
    public int Minutes
    {
        get => ReadInt32LittleEndian(Data.AsSpan(Offset + 0x04));
        set => WriteInt32LittleEndian(Data.AsSpan(Offset + 0x04), value);
    }
    public byte TblMonsNo
    {
        get => Data[Offset + 0x08];
        set => Data[Offset + 0x08] = value;
    }
    public byte RareLv
    {
        get => Data[Offset + 0x09];
        set => Data[Offset + 0x09] = value;
    }
    public byte SwayLv
    {
        get => Data[Offset + 0x0A];
        set => Data[Offset + 0x0A] = value;
    }
    // 0xB alignment
}
