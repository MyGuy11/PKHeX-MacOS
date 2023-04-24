using System;

namespace PKHeX.Core;

/// <summary>
/// <see cref="PersonalInfo"/> class with values from Generation 1 games.
/// </summary>
public sealed class PersonalInfo1 : PersonalInfo, IPersonalInfoTM
{
    public const int SIZE = 0x1C;
    private readonly byte[] Data;

    public PersonalInfo1(byte[] data) => Data = data;
    public override byte[] Write() => Data;

    public override byte Gender { get => Data[0x00]; set => Data[0x00] = value; }
    public override int HP { get => Data[0x01]; set => Data[0x01] = (byte)value; }
    public override int ATK { get => Data[0x02]; set => Data[0x02] = (byte)value; }
    public override int DEF { get => Data[0x03]; set => Data[0x03] = (byte)value; }
    public override int SPE { get => Data[0x04]; set => Data[0x04] = (byte)value; }
    public int SPC { get => Data[0x05]; set => Data[0x05] = (byte)value; }
    public override int SPA { get => SPC; set => SPC = value; }
    public override int SPD { get => SPC; set => SPC = value; }
    public override byte Type1 { get => Data[0x06]; set => Data[0x06] = value; }
    public override byte Type2 { get => Data[0x07]; set => Data[0x07] = value; }
    public override int CatchRate { get => Data[0x08]; set => Data[0x08] = (byte)value; }
    public override int BaseEXP { get => Data[0x09]; set => Data[0x09] = (byte)value; }
    public byte Move1 { get => Data[0x0F]; set => Data[0x0F] = value; }
    public byte Move2 { get => Data[0x10]; set => Data[0x10] = value; }
    public byte Move3 { get => Data[0x11]; set => Data[0x11] = value; }
    public byte Move4 { get => Data[0x12]; set => Data[0x12] = value; }
    public override byte EXPGrowth { get => Data[0x13]; set => Data[0x13] = value; }

    // EV Yields are just aliases for base stats in Gen I
    public override int EV_HP { get => HP; set { } }
    public override int EV_ATK { get => ATK; set { } }
    public override int EV_DEF { get => DEF; set { } }
    public override int EV_SPE { get => SPE; set { } }
    public int EV_SPC => SPC;
    public override int EV_SPA { get => EV_SPC; set { } }
    public override int EV_SPD { get => EV_SPC; set { } }

    // Future game values, unused
    public override int EggGroup1 { get => 0; set { } }
    public override int EggGroup2 { get => 0; set { } }
    public override int GetIndexOfAbility(int abilityID) => -1;
    public override int GetAbilityAtIndex(int abilityIndex) => -1;
    public override int AbilityCount => 0;
    public override int HatchCycles { get => 0; set { } }
    public override int BaseFriendship { get => 0; set { } }
    public override int EscapeRate { get => 0; set { } }
    public override int Color { get => 0; set { } }

    public void GetMoves(Span<ushort> value)
    {
        value[3] = Move4;
        value[2] = Move3;
        value[1] = Move2;
        value[0] = Move1;
    }

    private const int TMHM = 0x14;
    private const int CountTMHM = 50 + 5; // 50 TMs, 5 HMs
    private const int ByteCountTM = (CountTMHM + 7) / 8;

    public bool GetIsLearnTM(int index)
    {
        if ((uint)index >= CountTMHM)
            return false;
        return (Data[TMHM + (index >> 3)] & (1 << (index & 7))) != 0;
    }

    public void SetIsLearnTM(int index, bool value)
    {
        if ((uint)index >= CountTMHM)
            return;
        if (value)
            Data[TMHM + (index >> 3)] |= (byte)(1 << (index & 7));
        else
            Data[TMHM + (index >> 3)] &= (byte)~(1 << (index & 7));
    }

    public void SetAllLearnTM(Span<bool> result, ReadOnlySpan<byte> moves)
    {
        var span = Data.AsSpan(TMHM, ByteCountTM);
        for (int index = CountTMHM - 1; index >= 0; index--)
        {
            if ((span[index >> 3] & (1 << (index & 7))) != 0)
                result[moves[index]] = true;
        }
    }
}
