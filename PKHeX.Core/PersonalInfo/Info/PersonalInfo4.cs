using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// <see cref="PersonalInfo"/> class with values from Generation 4 games.
/// </summary>
public sealed class PersonalInfo4 : PersonalInfo, IPersonalAbility12, IPersonalInfoTM, IPersonalInfoTutorType
{
    public const int SIZE = 0x2C;
    private readonly byte[] Data;

    public PersonalInfo4(byte[] data) => Data = data;
    public override byte[] Write() => Data;

    public override int HP { get => Data[0x00]; set => Data[0x00] = (byte)value; }
    public override int ATK { get => Data[0x01]; set => Data[0x01] = (byte)value; }
    public override int DEF { get => Data[0x02]; set => Data[0x02] = (byte)value; }
    public override int SPE { get => Data[0x03]; set => Data[0x03] = (byte)value; }
    public override int SPA { get => Data[0x04]; set => Data[0x04] = (byte)value; }
    public override int SPD { get => Data[0x05]; set => Data[0x05] = (byte)value; }
    public override byte Type1 { get => Data[0x06]; set => Data[0x06] = value; }
    public override byte Type2 { get => Data[0x07]; set => Data[0x07] = value; }
    public override int CatchRate { get => Data[0x08]; set => Data[0x08] = (byte)value; }
    public override int BaseEXP { get => Data[0x09]; set => Data[0x09] = (byte)value; }
    private int EVYield { get => ReadUInt16LittleEndian(Data.AsSpan(0x0A)); set => WriteUInt16LittleEndian(Data.AsSpan(0x0A), (ushort)value); }
    public override int EV_HP { get => (EVYield >> 0) & 0x3; set => EVYield = (EVYield & ~(0x3 << 0)) | ((value & 0x3) << 0); }
    public override int EV_ATK { get => (EVYield >> 2) & 0x3; set => EVYield = (EVYield & ~(0x3 << 2)) | ((value & 0x3) << 2); }
    public override int EV_DEF { get => (EVYield >> 4) & 0x3; set => EVYield = (EVYield & ~(0x3 << 4)) | ((value & 0x3) << 4); }
    public override int EV_SPE { get => (EVYield >> 6) & 0x3; set => EVYield = (EVYield & ~(0x3 << 6)) | ((value & 0x3) << 6); }
    public override int EV_SPA { get => (EVYield >> 8) & 0x3; set => EVYield = (EVYield & ~(0x3 << 8)) | ((value & 0x3) << 8); }
    public override int EV_SPD { get => (EVYield >> 10) & 0x3; set => EVYield = (EVYield & ~(0x3 << 10)) | ((value & 0x3) << 10); }
    public int Item1 { get => ReadInt16LittleEndian(Data.AsSpan(0xC)); set => WriteInt16LittleEndian(Data.AsSpan(0xC), (short)value); }
    public int Item2 { get => ReadInt16LittleEndian(Data.AsSpan(0xE)); set => WriteInt16LittleEndian(Data.AsSpan(0xE), (short)value); }
    public override byte Gender { get => Data[0x10]; set => Data[0x10] = value; }
    public override int HatchCycles { get => Data[0x11]; set => Data[0x11] = (byte)value; }
    public override int BaseFriendship { get => Data[0x12]; set => Data[0x12] = (byte)value; }
    public override byte EXPGrowth { get => Data[0x13]; set => Data[0x13] = value; }
    public override int EggGroup1 { get => Data[0x14]; set => Data[0x14] = (byte)value; }
    public override int EggGroup2 { get => Data[0x15]; set => Data[0x15] = (byte)value; }
    public int Ability1 { get => Data[0x16]; set => Data[0x16] = (byte)value; }
    public int Ability2 { get => Data[0x17]; set => Data[0x17] = (byte)value; }
    public override int EscapeRate { get => Data[0x18]; set => Data[0x18] = (byte)value; }
    public override int Color { get => Data[0x19] & 0x7F; set => Data[0x19] = (byte)((Data[0x19] & 0x80) | value); }
    public bool NoFlip { get => Data[0x19] >> 7 == 1; set => Data[0x19] = (byte)(Color | (value ? 0x80 : 0)); }

    public override int AbilityCount => 2;
    public override int GetIndexOfAbility(int abilityID) => abilityID == Ability1 ? 0 : abilityID == Ability2 ? 1 : -1;
    public override int GetAbilityAtIndex(int abilityIndex) => abilityIndex switch
    {
        0 => Ability1,
        1 => Ability2,
        _ => throw new ArgumentOutOfRangeException(nameof(abilityIndex), abilityIndex, null),
    };
    public int GetAbility(bool second) => second && HasSecondAbility ? Ability2 : Ability1;

    public bool HasSecondAbility => Ability1 != Ability2;

    // Manually added attributes
    public override byte FormCount { get => Data[0x29]; set {} }
    public override int FormStatsIndex { get => ReadUInt16LittleEndian(Data.AsSpan(0x2A)); set {} }

    public void AddTypeTutors(ReadOnlySpan<byte> data) => TypeTutors = FlagUtil.GitBitFlagArray(data);
    public void CopyTypeTutors(PersonalInfo4 other) => TypeTutors = other.TypeTutors;

    /// <summary>
    /// Grass-Fire-Water-Etc typed learn compatibility flags for individual moves.
    /// </summary>
    public bool[] TypeTutors { get; private set; } = Array.Empty<bool>();

    private const int TMHM = 0x1C;
    private const int CountTM = 92;
    private const int CountHM = 8;
    private const int CountTMHM = CountTM + CountHM;
    private const int ByteCountTM = (CountTMHM + 7) / 8;
    private const int CountTutor = 0x34;

    public bool GetIsLearnTM(int index)
    {
        if ((uint)index >= CountTM)
            return false;
        return (Data[TMHM + (index >> 3)] & (1 << (index & 7))) != 0;
    }

    public void SetIsLearnTM(int index, bool value)
    {
        if ((uint)index >= CountTM)
            return;
        if (value)
            Data[TMHM + (index >> 3)] |= (byte)(1 << (index & 7));
        else
            Data[TMHM + (index >> 3)] &= (byte)~(1 << (index & 7));
    }

    public void SetAllLearnTM(Span<bool> result, ReadOnlySpan<ushort> moves)
    {
        var span = Data.AsSpan(TMHM, ByteCountTM);
        for (int index = CountTM - 1; index >= 0; index--)
        {
            if ((span[index >> 3] & (1 << (index & 7))) != 0)
                result[moves[index]] = true;
        }
    }

    public bool GetIsLearnTutorType(int index)
    {
        if ((uint)index >= CountTutor)
            return false;
        return TypeTutors[index];
    }

    public void SetIsLearnTutorType(int index, bool value)
    {
        if ((uint)index >= CountTutor)
            throw new ArgumentOutOfRangeException(nameof(index), index, null);
        TypeTutors[index] = value;
    }

    public void SetAllLearnTutorType(Span<bool> result, ReadOnlySpan<ushort> moves)
    {
        for (int index = CountTutor - 1; index >= 0; index--)
        {
            if (TypeTutors[index])
                result[moves[index]] = true;
        }
    }

    public bool GetIsLearnHM(int index)
    {
        if ((uint)index >= CountHM)
            return false;
        index += CountTM;
        return (Data[TMHM + (index >> 3)] & (1 << (index & 7))) != 0;
    }

    public void SetAllLearnHM(Span<bool> result, ReadOnlySpan<ushort> moves)
    {
        var span = Data.AsSpan(TMHM, ByteCountTM);
        for (int index = CountTM + CountHM - 1; index >= CountTM; index--)
        {
            if ((span[index >> 3] & (1 << (index & 7))) != 0)
                result[moves[index - CountTM]] = true;
        }
    }
}
