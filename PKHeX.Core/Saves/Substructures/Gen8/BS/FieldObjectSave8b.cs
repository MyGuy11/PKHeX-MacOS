﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Stores 1000 field objects to spawn into the map.
/// </summary>
/// <remarks>size: 0x109A0 (1000 * 4*17)</remarks>
[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class FieldObjectSave8b : SaveBlock<SAV8BS>
{
    private const int COUNT_OBJECTS = 1_000;

    public FieldObjectSave8b(SAV8BS sav, int offset) : base(sav) => Offset = offset;

    public FieldObject8b[] AllObjects
    {
        get => GetObjects();
        set => SetObjects(value);
    }

    private FieldObject8b[] GetObjects()
    {
        var result = new FieldObject8b[COUNT_OBJECTS];
        for (int i = 0; i < result.Length; i++)
            result[i] = new FieldObject8b(Data, Offset + (i * FieldObject8b.SIZE));
        return result;
    }

    private static void SetObjects(IReadOnlyList<FieldObject8b> value)
    {
        if (value.Count != COUNT_OBJECTS)
            throw new ArgumentOutOfRangeException(nameof(value.Count));
        // data is already hard-referencing the original byte array. This is mostly a hack for Property Grid displays.
    }
}

[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class FieldObject8b
{
    public const int SIZE = 4 * 17;

    private readonly byte[] Data = new byte[SIZE];

    public override string ToString() => $"{NameHash:X8} @ ({GridX:000},{GridY:000}) - {(Active ? "✓" : "✕")}";

    public FieldObject8b(byte[] data, int offset)
    {
        data.AsSpan(offset, SIZE).CopyTo(Data);
    }

    public byte Count // cnt
    {
        get => Data[0] ;
        set => Data[0] = value;
    }

    public int NameHash { get => ReadInt32LittleEndian(Data.AsSpan(0x04)); set => WriteInt32LittleEndian(Data.AsSpan(0x04), value); }
    public int GridX    { get => ReadInt32LittleEndian(Data.AsSpan(0x08)); set => WriteInt32LittleEndian(Data.AsSpan(0x08), value); }
    public int GridY    { get => ReadInt32LittleEndian(Data.AsSpan(0x0C)); set => WriteInt32LittleEndian(Data.AsSpan(0x0C), value); }
    public int Height   { get => ReadInt32LittleEndian(Data.AsSpan(0x10)); set => WriteInt32LittleEndian(Data.AsSpan(0x10), value); }
    public int Angle    { get => ReadInt32LittleEndian(Data.AsSpan(0x14)); set => WriteInt32LittleEndian(Data.AsSpan(0x14), value); }
    public bool Active  { get => ReadInt32LittleEndian(Data.AsSpan(0x18)) == 1; set => WriteUInt32LittleEndian(Data.AsSpan(0x18), value ? 1u : 0u); }
    public int MoveCode { get => ReadInt32LittleEndian(Data.AsSpan(0x1C)); set => WriteInt32LittleEndian(Data.AsSpan(0x1C), value); }
    public int DirHead  { get => ReadInt32LittleEndian(Data.AsSpan(0x20)); set => WriteInt32LittleEndian(Data.AsSpan(0x20), value); }
    public int MvParam0 { get => ReadInt32LittleEndian(Data.AsSpan(0x24)); set => WriteInt32LittleEndian(Data.AsSpan(0x24), value); }
    public int MvParam1 { get => ReadInt32LittleEndian(Data.AsSpan(0x28)); set => WriteInt32LittleEndian(Data.AsSpan(0x28), value); }
    public int MvParam2 { get => ReadInt32LittleEndian(Data.AsSpan(0x2C)); set => WriteInt32LittleEndian(Data.AsSpan(0x2C), value); }
    public int LimitX   { get => ReadInt32LittleEndian(Data.AsSpan(0x30)); set => WriteInt32LittleEndian(Data.AsSpan(0x30), value); }
    public int LimitZ   { get => ReadInt32LittleEndian(Data.AsSpan(0x34)); set => WriteInt32LittleEndian(Data.AsSpan(0x34), value); }
    public int EvType   { get => ReadInt32LittleEndian(Data.AsSpan(0x38)); set => WriteInt32LittleEndian(Data.AsSpan(0x38), value); }
    public int MvOldDir { get => ReadInt32LittleEndian(Data.AsSpan(0x3C)); set => WriteInt32LittleEndian(Data.AsSpan(0x3C), value); }
    public int MvDir    { get => ReadInt32LittleEndian(Data.AsSpan(0x40)); set => WriteInt32LittleEndian(Data.AsSpan(0x40), value); }
}
