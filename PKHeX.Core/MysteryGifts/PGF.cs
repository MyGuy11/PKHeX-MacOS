using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Generation 5 Mystery Gift Template File
/// </summary>
public sealed class PGF : DataMysteryGift, IRibbonSetEvent3, IRibbonSetEvent4, ILangNick, IContestStats, INature
{
    public const int Size = 0xCC;
    public const int SizeFull = 0x2D0;
    public override int Generation => 5;
    public override EntityContext Context => EntityContext.Gen5;
    public override bool FatefulEncounter => true;

    public PGF() : this(new byte[Size]) { }
    public PGF(byte[] data) : base(data) { }

    public override uint ID32 { get => ReadUInt32LittleEndian(Data.AsSpan(0x00)); set => WriteUInt32LittleEndian(Data.AsSpan(0x00), value); }
    public override ushort TID16 { get => ReadUInt16LittleEndian(Data.AsSpan(0x00)); set => WriteUInt16LittleEndian(Data.AsSpan(0x00), value); }
    public override ushort SID16 { get => ReadUInt16LittleEndian(Data.AsSpan(0x02)); set => WriteUInt16LittleEndian(Data.AsSpan(0x02), value); }
    public int OriginGame { get => Data[0x04]; set => Data[0x04] = (byte)value; }
    // Unused 0x05 0x06, 0x07
    public uint PID { get => ReadUInt32LittleEndian(Data.AsSpan(0x08)); set => WriteUInt32LittleEndian(Data.AsSpan(0x08), value); }

    private byte RIB0 { get => Data[0x0C]; set => Data[0x0C] = value; }
    private byte RIB1 { get => Data[0x0D]; set => Data[0x0D] = value; }
    public bool RibbonCountry          { get => (RIB0 & (1 << 0)) == 1 << 0; set => RIB0 = (byte)((RIB0 & ~(1 << 0)) | (value ? 1 << 0 : 0)); }
    public bool RibbonNational         { get => (RIB0 & (1 << 1)) == 1 << 1; set => RIB0 = (byte)((RIB0 & ~(1 << 1)) | (value ? 1 << 1 : 0)); }
    public bool RibbonEarth            { get => (RIB0 & (1 << 2)) == 1 << 2; set => RIB0 = (byte)((RIB0 & ~(1 << 2)) | (value ? 1 << 2 : 0)); }
    public bool RibbonWorld            { get => (RIB0 & (1 << 3)) == 1 << 3; set => RIB0 = (byte)((RIB0 & ~(1 << 3)) | (value ? 1 << 3 : 0)); }
    public bool RibbonClassic          { get => (RIB0 & (1 << 4)) == 1 << 4; set => RIB0 = (byte)((RIB0 & ~(1 << 4)) | (value ? 1 << 4 : 0)); }
    public bool RibbonPremier          { get => (RIB0 & (1 << 5)) == 1 << 5; set => RIB0 = (byte)((RIB0 & ~(1 << 5)) | (value ? 1 << 5 : 0)); }
    public bool RibbonEvent            { get => (RIB0 & (1 << 6)) == 1 << 6; set => RIB0 = (byte)((RIB0 & ~(1 << 6)) | (value ? 1 << 6 : 0)); }
    public bool RibbonBirthday         { get => (RIB0 & (1 << 7)) == 1 << 7; set => RIB0 = (byte)((RIB0 & ~(1 << 7)) | (value ? 1 << 7 : 0)); }
    public bool RibbonSpecial          { get => (RIB1 & (1 << 0)) == 1 << 0; set => RIB1 = (byte)((RIB1 & ~(1 << 0)) | (value ? 1 << 0 : 0)); }
    public bool RibbonSouvenir         { get => (RIB1 & (1 << 1)) == 1 << 1; set => RIB1 = (byte)((RIB1 & ~(1 << 1)) | (value ? 1 << 1 : 0)); }
    public bool RibbonWishing          { get => (RIB1 & (1 << 2)) == 1 << 2; set => RIB1 = (byte)((RIB1 & ~(1 << 2)) | (value ? 1 << 2 : 0)); }
    public bool RibbonChampionBattle   { get => (RIB1 & (1 << 3)) == 1 << 3; set => RIB1 = (byte)((RIB1 & ~(1 << 3)) | (value ? 1 << 3 : 0)); }
    public bool RibbonChampionRegional { get => (RIB1 & (1 << 4)) == 1 << 4; set => RIB1 = (byte)((RIB1 & ~(1 << 4)) | (value ? 1 << 4 : 0)); }
    public bool RibbonChampionNational { get => (RIB1 & (1 << 5)) == 1 << 5; set => RIB1 = (byte)((RIB1 & ~(1 << 5)) | (value ? 1 << 5 : 0)); }
    public bool RibbonChampionWorld    { get => (RIB1 & (1 << 6)) == 1 << 6; set => RIB1 = (byte)((RIB1 & ~(1 << 6)) | (value ? 1 << 6 : 0)); }
    public bool RIB1_7                 { get => (RIB1 & (1 << 7)) == 1 << 7; set => RIB1 = (byte)((RIB1 & ~(1 << 7)) | (value ? 1 << 7 : 0)); }

    public override int Ball { get => Data[0x0E]; set => Data[0x0E] = (byte)value; }
    public override int HeldItem { get => ReadUInt16LittleEndian(Data.AsSpan(0x10)); set => WriteUInt16LittleEndian(Data.AsSpan(0x10), (ushort)value); }
    public ushort Move1 { get => ReadUInt16LittleEndian(Data.AsSpan(0x12)); set => WriteUInt16LittleEndian(Data.AsSpan(0x12), value); }
    public ushort Move2 { get => ReadUInt16LittleEndian(Data.AsSpan(0x14)); set => WriteUInt16LittleEndian(Data.AsSpan(0x14), value); }
    public ushort Move3 { get => ReadUInt16LittleEndian(Data.AsSpan(0x16)); set => WriteUInt16LittleEndian(Data.AsSpan(0x16), value); }
    public ushort Move4 { get => ReadUInt16LittleEndian(Data.AsSpan(0x18)); set => WriteUInt16LittleEndian(Data.AsSpan(0x18), value); }
    public override ushort Species { get => ReadUInt16LittleEndian(Data.AsSpan(0x1A)); set => WriteUInt16LittleEndian(Data.AsSpan(0x1A), value); }
    public override byte Form { get => Data[0x1C]; set => Data[0x1C] = value; }
    public int Language { get => Data[0x1D]; set => Data[0x1D] = (byte)value; }

    public string Nickname
    {
        get => StringConverter5.GetString(Data.AsSpan(0x1E, 11 * 2));
        set => StringConverter5.SetString(Data.AsSpan(0x1E, 11 * 2), value, 11, StringConverterOption.ClearFF);
    }

    public int Nature { get => (sbyte)Data[0x34]; set => Data[0x34] = (byte)value; }
    public override int Gender { get => Data[0x35]; set => Data[0x35] = (byte)value; }
    public override int AbilityType { get => Data[0x36]; set => Data[0x36] = (byte)value; }
    public int PIDType { get => Data[0x37]; set => Data[0x37] = (byte)value; }
    public override int EggLocation { get => ReadUInt16LittleEndian(Data.AsSpan(0x38)); set => WriteUInt16LittleEndian(Data.AsSpan(0x38), (ushort)value); }
    public ushort MetLocation { get => ReadUInt16LittleEndian(Data.AsSpan(0x3A)); set => WriteUInt16LittleEndian(Data.AsSpan(0x3A), value); }
    public int MetLevel { get => Data[0x3C]; set => Data[0x3C] = (byte)value; }
    public byte CNT_Cool   { get => Data[0x3D]; set => Data[0x3D] = value; }
    public byte CNT_Beauty { get => Data[0x3E]; set => Data[0x3E] = value; }
    public byte CNT_Cute   { get => Data[0x3F]; set => Data[0x3F] = value; }
    public byte CNT_Smart  { get => Data[0x40]; set => Data[0x40] = value; }
    public byte CNT_Tough  { get => Data[0x41]; set => Data[0x41] = value; }
    public byte CNT_Sheen  { get => Data[0x42]; set => Data[0x42] = value; }
    public int IV_HP { get => Data[0x43]; set => Data[0x43] = (byte)value; }
    public int IV_ATK { get => Data[0x44]; set => Data[0x44] = (byte)value; }
    public int IV_DEF { get => Data[0x45]; set => Data[0x45] = (byte)value; }
    public int IV_SPE { get => Data[0x46]; set => Data[0x46] = (byte)value; }
    public int IV_SPA { get => Data[0x47]; set => Data[0x47] = (byte)value; }
    public int IV_SPD { get => Data[0x48]; set => Data[0x48] = (byte)value; }
    // Unused 0x49
    public override string OT_Name
    {
        get => StringConverter5.GetString(Data.AsSpan(0x4A, 8 * 2));
        set => StringConverter5.SetString(Data.AsSpan(0x4A, 8 * 2), value, 8, StringConverterOption.ClearFF);
    }

    public int OTGender { get => Data[0x5A]; set => Data[0x5A] = (byte)value; }
    public override byte Level { get => Data[0x5B]; set => Data[0x5C] = value; }
    public override bool IsEgg { get => Data[0x5C] == 1; set => Data[0x5C] = value ? (byte)1 : (byte)0; }
    // Unused 0x5D 0x5E 0x5F
    public override string CardTitle
    {
        get => StringConverter5.GetString(Data.AsSpan(0x60, 37 * 2));
        set => StringConverter5.SetString(Data.AsSpan(0x60, 37 * 2), value, 36, StringConverterOption.ClearZero);
    }

    // Card Attributes
    public override int ItemID { get => ReadUInt16LittleEndian(Data.AsSpan(0x00)); set => WriteUInt16LittleEndian(Data.AsSpan(0x00), (ushort)value); }

    private ushort Year { get => ReadUInt16LittleEndian(Data.AsSpan(0xAE)); set => WriteUInt16LittleEndian(Data.AsSpan(0xAE), value); }
    private byte Month { get => Data[0xAD]; set => Data[0xAD] = value; }
    private byte Day { get => Data[0xAC]; set => Data[0xAC] = value; }

    /// <summary>
    /// Gets or sets the date of the card.
    /// </summary>
    public DateOnly? Date
    {
        get
        {
            // Check to see if date is valid
            if (!DateUtil.IsDateValid(Year, Month, Day))
                return null;

            return new DateOnly(Year, Month, Day);
        }
        set
        {
            if (value.HasValue)
            {
                // Only update the properties if a value is provided.
                Year = (ushort)value.Value.Year;
                Month = (byte)value.Value.Month;
                Day = (byte)value.Value.Day;
            }
            else
            {
                // Clear the Met Date.
                // If code tries to access MetDate again, null will be returned.
                Year = 0;
                Month = 0;
                Day = 0;
            }
        }
    }

    public override int CardID
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(0xB0));
        set => WriteUInt16LittleEndian(Data.AsSpan(0xB0), (ushort)value);
    }

    public int CardLocation { get => Data[0xB2]; set => Data[0xB2] = (byte)value; }
    public int CardType { get => Data[0xB3]; set => Data[0xB3] = (byte)value; }
    public override bool GiftUsed { get => Data[0xB4] >> 1 > 0; set => Data[0xB4] = (byte)((Data[0xB4] & ~2) | (value ? 2 : 0)); }
    public bool MultiObtain { get => Data[0xB4] == 1; set => Data[0xB4] = value ? (byte)1 : (byte)0; }

    // Meta Accessible Properties
    public override int[] IVs
    {
        get => new[] { IV_HP, IV_ATK, IV_DEF, IV_SPE, IV_SPA, IV_SPD };
        set
        {
            if (value.Length != 6) return;
            IV_HP = value[0]; IV_ATK = value[1]; IV_DEF = value[2];
            IV_SPE = value[3]; IV_SPA = value[4]; IV_SPD = value[5];
        }
    }

    public override void GetIVs(Span<int> value)
    {
        if (value.Length != 6)
            return;
        value[0] = IV_HP;
        value[1] = IV_ATK;
        value[2] = IV_DEF;
        value[3] = IV_SPE;
        value[4] = IV_SPA;
        value[5] = IV_SPD;
    }

    public bool IsNicknamed => Nickname.Length > 0;
    public override bool IsShiny => PIDType == 2;
    public override int Location { get => MetLocation; set => MetLocation = (ushort)value; }
    public override Moveset Moves => new(Move1, Move2, Move3, Move4);
    public override bool IsEntity { get => CardType == 1; set { if (value) CardType = 1; } }
    public override bool IsItem { get => CardType == 2; set { if (value) CardType = 2; } }
    public bool IsPower { get => CardType == 3; set { if (value) CardType = 3; } }

    public override PK5 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        if (!IsEntity)
            throw new ArgumentException(nameof(IsEntity));

        var rnd = Util.Rand;

        var dt = DateTime.Now;
        if (Day == 0)
        {
            Day = (byte)dt.Day;
            Month = (byte)dt.Month;
            Year = (byte)dt.Year;
        }

        int currentLevel = Level > 0 ? Level : 1 + rnd.Next(100);
        var pi = PersonalTable.B2W2.GetFormEntry(Species, Form);
        PK5 pk = new()
        {
            Species = Species,
            HeldItem = HeldItem,
            Met_Level = currentLevel,
            Nature = Nature != -1 ? Nature : rnd.Next(25),
            Form = Form,
            Version = OriginGame == 0 ? tr.Game : OriginGame,
            Language = Language == 0 ? tr.Language : Language,
            Ball = Ball,
            Move1 = Move1,
            Move2 = Move2,
            Move3 = Move3,
            Move4 = Move4,
            Met_Location = MetLocation,
            MetDate = Date,
            Egg_Location = EggLocation,
            CNT_Cool = CNT_Cool,
            CNT_Beauty = CNT_Beauty,
            CNT_Cute = CNT_Cute,
            CNT_Smart = CNT_Smart,
            CNT_Tough = CNT_Tough,
            CNT_Sheen = CNT_Sheen,

            EXP = Experience.GetEXP(currentLevel, pi.EXPGrowth),

            // Ribbons
            RibbonCountry = RibbonCountry,
            RibbonNational = RibbonNational,
            RibbonEarth = RibbonEarth,
            RibbonWorld = RibbonWorld,
            RibbonClassic = RibbonClassic,
            RibbonPremier = RibbonPremier,
            RibbonEvent = RibbonEvent,
            RibbonBirthday = RibbonBirthday,

            RibbonSpecial = RibbonSpecial,
            RibbonSouvenir = RibbonSouvenir,
            RibbonWishing = RibbonWishing,
            RibbonChampionBattle = RibbonChampionBattle,
            RibbonChampionRegional = RibbonChampionRegional,
            RibbonChampionNational = RibbonChampionNational,
            RibbonChampionWorld = RibbonChampionWorld,

            FatefulEncounter = true,
        };
        if (tr.Generation > 5 && OriginGame == 0) // Gen6+, give random gen5 game
            pk.Version = (int)GameVersion.W + rnd.Next(4);

        if (Move1 == 0) // No moves defined
        {
            Span<ushort> moves = stackalloc ushort[4];
            MoveLevelUp.GetEncounterMoves(moves, Species, Form, Level, (GameVersion)pk.Version);
            pk.SetMoves(moves);
        }

        pk.SetMaximumPPCurrent();

        if (IsEgg) // User's
        {
            pk.TID16 = tr.TID16;
            pk.SID16 = tr.SID16;
            pk.OT_Name = tr.OT;
            pk.OT_Gender = tr.Gender;
        }
        else // Hardcoded
        {
            pk.TID16 = TID16;
            pk.SID16 = SID16;
            pk.OT_Name = OT_Name;
            pk.OT_Gender = (OTGender == 3 ? tr.Gender : OTGender) & 1; // some events have variable gender based on receiving SaveFile
        }

        pk.IsNicknamed = IsNicknamed;
        pk.Nickname = IsNicknamed ? Nickname : SpeciesName.GetSpeciesNameGeneration(Species, pk.Language, Generation);

        SetPINGA(pk, criteria);

        if (IsEgg)
            SetEggMetDetails(pk);

        pk.CurrentFriendship = pk.IsEgg ? pi.HatchCycles : pi.BaseFriendship;

        pk.RefreshChecksum();
        return pk;
    }

    private void SetEggMetDetails(PK5 pk)
    {
        pk.IsEgg = true;
        pk.EggMetDate = Date;
        pk.Nickname = SpeciesName.GetEggName(pk.Language, Generation);
        pk.IsNicknamed = true;
    }

    private void SetPINGA(PK5 pk, EncounterCriteria criteria)
    {
        var pi = PersonalTable.B2W2.GetFormEntry(Species, Form);
        pk.Nature = (int)criteria.GetNature((Nature)Nature);
        pk.Gender = pi.Genderless ? 2 : Gender != 2 ? Gender : criteria.GetGender(-1, pi);
        var av = GetAbilityIndex(criteria);
        SetPID(pk, av);
        pk.RefreshAbility(av);
        SetIVs(pk);
    }

    private int GetAbilityIndex(EncounterCriteria criteria) => AbilityType switch
    {
        00 or 01 or 02 => AbilityType, // Fixed 0/1/2
        03 or 04 => criteria.GetAbilityFromNumber(Ability), // 0/1 or 0/1/H
        _ => throw new ArgumentOutOfRangeException(nameof(AbilityType)),
    };

    public override AbilityPermission Ability => AbilityType switch
    {
        0 => AbilityPermission.OnlyFirst,
        1 => AbilityPermission.OnlySecond,
        2 => AbilityPermission.OnlyHidden,
        3 => AbilityPermission.Any12,
        _ => AbilityPermission.Any12H,
    };

    private void SetPID(PK5 pk, int av)
    {
        if (PID != 0)
        {
            pk.PID = PID;
            return;
        }

        var rnd = Util.Rand;
        pk.PID = rnd.Rand32();
        // Force Gender
        do { pk.PID = (pk.PID & 0xFFFFFF00) | (uint)rnd.Next(0x100); }
        while (!pk.IsGenderValid());

        if (PIDType == 2) // Always
        {
            var gb = (byte)pk.PID;
            pk.PID = PIDGenerator.GetMG5ShinyPID(gb, (uint)av, pk.TID16, pk.SID16);
        }
        else if (PIDType != 1) // Force Not Shiny
        {
            if (pk.IsShiny)
                pk.PID ^= 0x10000000;
        }

        if (av == 1)
            pk.PID |= 0x10000;
        else
            pk.PID &= 0xFFFEFFFF;
    }

    public override Shiny Shiny => PIDType switch
    {
        0 when !IsEgg => Shiny.Never,
        2 when !IsEgg => Shiny.Always,
        _ => Shiny.Random, // 1
    };

    private void SetIVs(PK5 pk)
    {
        Span<int> finalIVs = stackalloc int[6];
        GetIVs(finalIVs);
        var rnd = Util.Rand;
        for (int i = 0; i < finalIVs.Length; i++)
            finalIVs[i] = finalIVs[i] == 0xFF ? rnd.Next(32) : finalIVs[i];
        pk.SetIVs(finalIVs);
    }

    public override bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (!IsEgg)
        {
            if (SID16 != pk.SID16) return false;
            if (TID16 != pk.TID16) return false;
            if (OT_Name != pk.OT_Name) return false;
            if (OTGender < 3 && OTGender != pk.OT_Gender) return false;
            if (PID != 0 && pk.PID != PID) return false;
            if (PIDType == 0 && pk.IsShiny) return false;
            if (PIDType == 2 && !pk.IsShiny) return false;
            if (OriginGame != 0 && OriginGame != pk.Version) return false;
            if (Language != 0 && Language != pk.Language) return false;

            if (!IsMatchEggLocation(pk)) return false;
            if (MetLocation != pk.Met_Location) return false;
        }
        else
        {
            if (EggLocation != pk.Egg_Location) // traded
            {
                if (pk.Egg_Location != Locations.LinkTrade5)
                    return false;
            }
            else if (PIDType == 0 && pk.IsShiny)
            {
                return false; // can't be traded away for un-shiny
            }

            if (pk is { IsEgg: true, IsNative: false })
                return false;
        }

        if (Form != evo.Form && !FormInfo.IsFormChangeable(Species, Form, pk.Form, Context, pk.Context))
            return false;

        if (Level != pk.Met_Level) return false;
        if (Ball != pk.Ball) return false;
        if (Nature != -1 && pk.Nature != Nature)
            return false;
        if (Gender != 2 && Gender != pk.Gender) return false;

        if (pk is IContestStatsReadOnly s && s.IsContestBelow(this))
            return false;

        return true;
    }

    protected override bool IsMatchDeferred(PKM pk) => Species != pk.Species;
    protected override bool IsMatchPartial(PKM pk) => !CanBeReceivedByVersion(pk.Version);

    public bool CanBeReceivedByVersion(int game) => OriginGame == 0 || OriginGame == game;
}
