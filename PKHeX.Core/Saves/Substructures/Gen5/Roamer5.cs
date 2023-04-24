using System;
using System.ComponentModel;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Gen5 structure
/// </summary>
/// <remarks>size 0x18</remarks>
[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class Roamer5
{
    public const int SIZE = 0x18;
    public readonly byte[] Raw;
    private readonly int Offset;

    public Roamer5(byte[] raw, int offset) => (Raw, Offset) = (raw, offset);
    private Span<byte> Data => Raw.AsSpan(Offset, SIZE);

    public ushort LastLocation   { get => ReadUInt16LittleEndian(Data); set => WriteUInt16LittleEndian(Data, value); }
    public ushort CaptureUnknown { get => ReadUInt16LittleEndian(Data[0x02..]); set => WriteUInt16LittleEndian(Data[0x02..], value); }
    public ushort Location       { get => ReadUInt16LittleEndian(Data[0x04..]); set => WriteUInt16LittleEndian(Data[0x04..], value); }
    public ushort Nature         { get => ReadUInt16LittleEndian(Data[0x06..]); set => WriteUInt16LittleEndian(Data[0x06..], value); }
    public uint IV32             { get => ReadUInt32LittleEndian(Data[0x08..]); set => WriteUInt32LittleEndian(Data[0x08..], value); }
    public uint PID              { get => ReadUInt32LittleEndian(Data[0x0C..]); set => WriteUInt32LittleEndian(Data[0x0C..], value); }
    public ushort Species        { get => ReadUInt16LittleEndian(Data[0x10..]); set => WriteUInt16LittleEndian(Data[0x10..], value); }
    public ushort Stat_HPCurrent { get => ReadUInt16LittleEndian(Data[0x12..]); set => WriteUInt16LittleEndian(Data[0x12..], value); }
    public byte Level  { get => Data[0x14]; set => Data[0x14] = value; }
    public byte Status { get => Data[0x15]; set => Data[0x15] = value; }
    public bool Active { get => Data[0x16] != 0; set => Data[0x16] = (byte)(value ? 1 : 0); }
    public byte Unk17  { get => Data[0x17]; set => Data[0x17] = value; }

    // Derived Properties
    private int IV_HP  { get => (int)(IV32 >> 00) & 0x1F; set => IV32 = (uint)((IV32 & ~(0x1F << 00)) | (uint)((value > 31 ? 31 : value) << 00)); }
    private int IV_ATK { get => (int)(IV32 >> 05) & 0x1F; set => IV32 = (uint)((IV32 & ~(0x1F << 05)) | (uint)((value > 31 ? 31 : value) << 05)); }
    private int IV_DEF { get => (int)(IV32 >> 10) & 0x1F; set => IV32 = (uint)((IV32 & ~(0x1F << 10)) | (uint)((value > 31 ? 31 : value) << 10)); }
    private int IV_SPE { get => (int)(IV32 >> 15) & 0x1F; set => IV32 = (uint)((IV32 & ~(0x1F << 15)) | (uint)((value > 31 ? 31 : value) << 15)); }
    private int IV_SPA { get => (int)(IV32 >> 20) & 0x1F; set => IV32 = (uint)((IV32 & ~(0x1F << 20)) | (uint)((value > 31 ? 31 : value) << 20)); }
    private int IV_SPD { get => (int)(IV32 >> 25) & 0x1F; set => IV32 = (uint)((IV32 & ~(0x1F << 25)) | (uint)((value > 31 ? 31 : value) << 25)); }

    /// <summary>
    /// Roamer's IVs.
    /// </summary>
    public int[] IVs
    {
        get => new[] { IV_HP, IV_ATK, IV_DEF, IV_SPE, IV_SPA, IV_SPD };
        set => SetIVs(value);
    }

    public void SetIVs(ReadOnlySpan<int> value)
    {
        if (value.Length != 6)
            return;
        IV_HP = value[0];
        IV_ATK = value[1];
        IV_DEF = value[2];
        IV_SPE = value[3];
        IV_SPA = value[4];
        IV_SPD = value[5];
    }

    public void Clear() => Data.Clear();
}
