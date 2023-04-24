using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class Hall4
{
    private const int SIZE = 0xBA0;
    private const int SIZE_FOOTER = 0x10;
    private const int SIZE_BLOCK = 0x1000;

    private readonly byte[] Raw;
    private readonly int Offset;
    private Span<byte> Data => Raw.AsSpan(Offset, SIZE_BLOCK);
    public Hall4(byte[] data, int offset) => (Raw, Offset) = (data, offset);

    private const int SIZE_ARRAY = 0x3DE; // 493+(egg, bad-egg) species

    // Structure:
    // u32 key
    // u16[495] single records
    // u16[495] double records
    // u16[495] multi records
    // u16 alignment
    // extdata 0x10 byte footer

    public uint Key { get => ReadUInt32LittleEndian(Data); set => WriteUInt32LittleEndian(Data, value); }

    public ushort GetCount(int battleType, ushort species)
    {
        var offset = GetRecordOffset(battleType, species);
        return ReadUInt16LittleEndian(Data[offset..]);
    }

    public void SetCount(int battleType, ushort species, ushort value)
    {
        var offset = GetRecordOffset(battleType, species);
        WriteUInt16LittleEndian(Data[offset..], value);
    }

    private static int GetRecordOffset(int battleType, ushort species)
    {
        if (species > Legal.MaxSpeciesID_4)
            throw new ArgumentOutOfRangeException(nameof(species));
        if ((uint)battleType > 2)
            throw new ArgumentOutOfRangeException(nameof(battleType));

        return sizeof(uint) + (battleType * SIZE_ARRAY) + (species * sizeof(ushort));
    }

    #region Footer
    public bool SizeValid => BlockSize == SIZE;
    public bool ChecksumValid => Checksum == GetChecksum();
    public bool IsValid => SizeValid && ChecksumValid;

    public uint  Magic     { get => ReadUInt32LittleEndian(Footer); set => WriteUInt32LittleEndian(Footer, value); }
    public uint  Revision  { get => ReadUInt32LittleEndian(Footer[0x4..]); set => WriteUInt32LittleEndian(Footer[0x4..], value); }
    public int   BlockSize { get => ReadInt32LittleEndian (Footer[0x8..]); set => WriteInt32LittleEndian (Footer[0x8..], value); }
    public ushort BlockID  { get => ReadUInt16LittleEndian(Footer[0xC..]); set => WriteUInt16LittleEndian(Footer[0xC..], value); }
    public ushort Checksum { get => ReadUInt16LittleEndian(Footer[0xE..]); set => WriteUInt16LittleEndian(Footer[0xE..], value); }

    private ReadOnlySpan<byte> GetRegion() => Data[..(SIZE + SIZE_FOOTER)];
    private Span<byte> Footer => Data.Slice(SIZE, SIZE_FOOTER);
    private ushort GetChecksum() => Checksums.CRC16_CCITT(GetRegion()[..^2]);
    public void RefreshChecksum() => Checksum = GetChecksum();
    #endregion
}
