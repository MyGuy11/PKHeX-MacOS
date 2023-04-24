using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using static PKHeX.Core.GameVersion;
using static System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes;

namespace PKHeX.Core;

/// <summary>
/// Object representing a <see cref="PKM"/>'s data and derived properties.
/// </summary>
[DynamicallyAccessedMembers(PublicProperties | NonPublicProperties | PublicParameterlessConstructor)]
public abstract class PKM : ISpeciesForm, ITrainerID32, IGeneration, IShiny, ILangNick, IGameValueLimit, INature, IFatefulEncounter
{
    /// <summary>
    /// Valid file extensions that represent <see cref="PKM"/> data, without the leading '.'
    /// </summary>
    public static readonly string[] Extensions = EntityFileExtension.GetExtensions();
    public abstract int SIZE_PARTY { get; }
    public abstract int SIZE_STORED { get; }
    public string Extension => GetType().Name.ToLowerInvariant();
    public abstract PersonalInfo PersonalInfo { get; }
    public virtual ReadOnlySpan<ushort> ExtraBytes => Array.Empty<ushort>();

    // Internal Attributes set on creation
    public readonly byte[] Data; // Raw Storage

    protected PKM(byte[] data) => Data = data;
    protected PKM(int size) => Data = new byte[size];

    public virtual byte[] EncryptedPartyData => Encrypt().AsSpan(0, SIZE_PARTY).ToArray();
    public virtual byte[] EncryptedBoxData => Encrypt().AsSpan(0, SIZE_STORED).ToArray();
    public virtual byte[] DecryptedPartyData => Write().AsSpan(0, SIZE_PARTY).ToArray();
    public virtual byte[] DecryptedBoxData => Write().AsSpan(0, SIZE_STORED).ToArray();

    /// <summary>
    /// Rough indication if the data is junk or not.
    /// </summary>
    public abstract bool Valid { get; set; }

    // Trash Bytes
    public abstract Span<byte> Nickname_Trash { get; }
    public abstract Span<byte> OT_Trash { get; }
    public virtual Span<byte> HT_Trash => Span<byte>.Empty;

    protected abstract byte[] Encrypt();
    public abstract EntityContext Context { get; }
    public int Format => Context.Generation();
    public TrainerIDFormat TrainerIDDisplayFormat => this.GetTrainerIDFormat();

    private byte[] Write()
    {
        RefreshChecksum();
        return Data;
    }

    // Surface Properties
    public abstract ushort Species { get; set; }
    public abstract string Nickname { get; set; }
    public abstract int HeldItem { get; set; }
    public abstract int Gender { get; set; }
    public abstract int Nature { get; set; }
    public virtual int StatNature { get => Nature; set => Nature = value; }
    public abstract int Ability { get; set; }
    public abstract int CurrentFriendship { get; set; }
    public abstract byte Form { get; set; }
    public abstract bool IsEgg { get; set; }
    public abstract bool IsNicknamed { get; set; }
    public abstract uint EXP { get; set; }
    public abstract ushort TID16 { get; set; }
    public abstract ushort SID16 { get; set; }
    public abstract string OT_Name { get; set; }
    public abstract int OT_Gender { get; set; }
    public abstract int Ball { get; set; }
    public abstract int Met_Level { get; set; }

    // Aliases of ID32
    public uint TrainerTID7 { get => this.GetTrainerTID7(); set => this.SetTrainerTID7(value); }
    public uint TrainerSID7 { get => this.GetTrainerSID7(); set => this.SetTrainerSID7(value); }
    public uint DisplayTID { get => this.GetDisplayTID(); set => this.SetDisplayTID(value); }
    public uint DisplaySID { get => this.GetDisplaySID(); set => this.SetDisplaySID(value); }

    // Battle
    public abstract ushort Move1 { get; set; }
    public abstract ushort Move2 { get; set; }
    public abstract ushort Move3 { get; set; }
    public abstract ushort Move4 { get; set; }
    public abstract int Move1_PP { get; set; }
    public abstract int Move2_PP { get; set; }
    public abstract int Move3_PP { get; set; }
    public abstract int Move4_PP { get; set; }
    public abstract int Move1_PPUps { get; set; }
    public abstract int Move2_PPUps { get; set; }
    public abstract int Move3_PPUps { get; set; }
    public abstract int Move4_PPUps { get; set; }
    public abstract int EV_HP { get; set; }
    public abstract int EV_ATK { get; set; }
    public abstract int EV_DEF { get; set; }
    public abstract int EV_SPE { get; set; }
    public abstract int EV_SPA { get; set; }
    public abstract int EV_SPD { get; set; }
    public abstract int IV_HP { get; set; }
    public abstract int IV_ATK { get; set; }
    public abstract int IV_DEF { get; set; }
    public abstract int IV_SPE { get; set; }
    public abstract int IV_SPA { get; set; }
    public abstract int IV_SPD { get; set; }
    public abstract int Status_Condition { get; set; }
    public abstract int Stat_Level { get; set; }
    public abstract int Stat_HPMax { get; set; }
    public abstract int Stat_HPCurrent { get; set; }
    public abstract int Stat_ATK { get; set; }
    public abstract int Stat_DEF { get; set; }
    public abstract int Stat_SPE { get; set; }
    public abstract int Stat_SPA { get; set; }
    public abstract int Stat_SPD { get; set; }

    // Hidden Properties
    public abstract int Version { get; set; }
    public abstract uint ID32 { get; set; }
    public abstract int PKRS_Strain { get; set; }
    public abstract int PKRS_Days { get; set; }

    public abstract uint EncryptionConstant { get; set; }
    public abstract uint PID { get; set; }

    // Misc Properties
    public abstract int Language { get; set; }
    public abstract bool FatefulEncounter { get; set; }
    public abstract uint TSV { get; }
    public abstract uint PSV { get; }
    public abstract int Characteristic { get; }
    public abstract int MarkValue { get; set; }
    public abstract int Met_Location { get; set; }
    public abstract int Egg_Location { get; set; }
    public abstract int OT_Friendship { get; set; }
    public virtual bool Japanese => Language == (int)LanguageID.Japanese;
    public virtual bool Korean => Language == (int)LanguageID.Korean;

    // Future Properties
    public virtual int Met_Year { get => 0; set { } }
    public virtual int Met_Month { get => 0; set { } }
    public virtual int Met_Day { get => 0; set { } }
    public virtual string HT_Name { get => string.Empty; set { } }
    public virtual int HT_Gender { get => 0; set { } }
    public virtual int HT_Friendship { get => 0; set { } }
    public virtual byte Enjoyment { get => 0; set { } }
    public virtual byte Fullness { get => 0; set { } }
    public virtual int AbilityNumber { get => 0; set { } }

    /// <summary>
    /// The date the Pokémon was met.
    /// </summary>
    /// <returns>
    /// A DateTime representing the date the Pokémon was met.
    /// Returns null if either the <see cref="PKM"/> format does not support dates or the stored date is invalid.</returns>
    /// <remarks>
    /// Not all <see cref="PKM"/> types support the <see cref="MetDate"/> property.  In these cases, this property will return null.
    /// If null is assigned to this property, it will be cleared.
    /// </remarks>
    public DateOnly? MetDate
    {
        get
        {
            // Check to see if date is valid
            if (!DateUtil.IsDateValid(2000 + Met_Year, Met_Month, Met_Day))
                return null;
            return new DateOnly(2000 + Met_Year, Met_Month, Met_Day);
        }
        set
        {
            if (value.HasValue)
            {
                // Only update the properties if a value is provided.
                Met_Year = value.Value.Year - 2000;
                Met_Month = value.Value.Month;
                Met_Day = value.Value.Day;
            }
            else
            {
                // Clear the Met Date.
                // If code tries to access MetDate again, null will be returned.
                Met_Year = 0;
                Met_Month = 0;
                Met_Day = 0;
            }
        }
    }

    public virtual int Egg_Year { get => 0; set { } }
    public virtual int Egg_Month { get => 0; set { } }
    public virtual int Egg_Day { get => 0; set { } }

    /// <summary>
    /// The date a Pokémon was met as an egg.
    /// </summary>
    /// <returns>
    /// A DateTime representing the date the Pokémon was met as an egg.
    /// Returns null if either the <see cref="PKM"/> format does not support dates or the stored date is invalid.</returns>
    /// <remarks>
    /// Not all <see cref="PKM"/> types support the <see cref="EggMetDate"/> property.  In these cases, this property will return null.
    /// If null is assigned to this property, it will be cleared.
    /// </remarks>
    public DateOnly? EggMetDate
    {
        get
        {
            // Check to see if date is valid
            if (!DateUtil.IsDateValid(2000 + Egg_Year, Egg_Month, Egg_Day))
                return null;
            return new DateOnly(2000 + Egg_Year, Egg_Month, Egg_Day);
        }
        set
        {
            if (value.HasValue)
            {
                // Only update the properties if a value is provided.
                Egg_Year = value.Value.Year - 2000;
                Egg_Month = value.Value.Month;
                Egg_Day = value.Value.Day;
            }
            else
            {
                // Clear the Met Date.
                // If code tries to access MetDate again, null will be returned.
                Egg_Year = 0;
                Egg_Month = 0;
                Egg_Day = 0;
            }
        }
    }

    public virtual ushort RelearnMove1 { get => 0; set { } }
    public virtual ushort RelearnMove2 { get => 0; set { } }
    public virtual ushort RelearnMove3 { get => 0; set { } }
    public virtual ushort RelearnMove4 { get => 0; set { } }

    // Exposed but not Present in all
    public abstract int CurrentHandler { get; set; }

    // Maximums
    public abstract ushort MaxMoveID { get; }
    public abstract ushort MaxSpeciesID { get; }
    public abstract int MaxItemID { get; }
    public abstract int MaxAbilityID { get; }
    public abstract int MaxBallID { get; }
    public abstract int MaxGameID { get; }
    public virtual int MinGameID => 0;
    public abstract int MaxIV { get; }
    public abstract int MaxEV { get; }
    public abstract int MaxStringLengthOT { get; }
    public abstract int MaxStringLengthNickname { get; }

    // Derived
    public virtual int SpriteItem => HeldItem;
    public virtual bool IsShiny => TSV == PSV;

    public ushort ShinyXor
    {
        get
        {
            var tmp = ID32 ^ PID;
            return (ushort)(tmp ^ (tmp >> 16));
        }
    }

    public bool E => Version == (int)GameVersion.E;
    public bool FRLG => Version is (int)FR or (int)LG;
    public bool Pt => (int)GameVersion.Pt == Version;
    public bool HGSS => Version is (int)HG or (int)SS;
    public bool BW => Version is (int)B or (int)W;
    public bool B2W2 => Version is (int)B2 or (int)W2;
    public bool XY => Version is (int)X or (int)Y;
    public bool AO => Version is (int)AS or (int)OR;
    public bool SM => Version is (int)SN or (int)MN;
    public bool USUM => Version is (int)US or (int)UM;
    public bool GO => Version is (int)GameVersion.GO;
    public bool VC1 => Version is >= (int)RD and <= (int)YW;
    public bool VC2 => Version is >= (int)GD and <= (int)C;
    public bool LGPE => Version is (int)GP or (int)GE;
    public bool SWSH => Version is (int)SW or (int)SH;
    public virtual bool BDSP => Version is (int)BD or (int)SP;
    public virtual bool LA => Version is (int)PLA;
    public bool SV => Version is (int)SL or (int)VL;

    public bool GO_LGPE => GO && Met_Location == Locations.GO7;
    public bool GO_HOME => GO && Met_Location == Locations.GO8;
    public bool VC => VC1 || VC2;
    public bool GG => LGPE || GO_LGPE;
    public bool Gen9 => SV;
    public bool Gen8 => Version is >= 44 and <= 49 || GO_HOME;
    public bool Gen7 => Version is >= 30 and <= 33 || GG;
    public bool Gen6 => Version is >= 24 and <= 29;
    public bool Gen5 => Version is >= 20 and <= 23;
    public bool Gen4 => Version is (>= 7 and <= 12) and not 9;
    public bool Gen3 => Version is (>= 1 and <= 5) or 15;
    public bool Gen2 => Version == (int)GSC; // Fixed value set by the Gen2 PKM classes
    public bool Gen1 => Version == (int)RBY; // Fixed value set by the Gen1 PKM classes
    public bool GenU => Generation <= 0;

    public int Generation
    {
        get
        {
            if (Gen9) return 9;
            if (Gen8) return 8;
            if (Gen7) return 7;
            if (Gen6) return 6;
            if (Gen5) return 5;
            if (Gen4) return 4;
            if (Gen3) return 3;
            if (Gen2) return Format; // 2
            if (Gen1) return Format; // 1
            if (VC1) return 1;
            if (VC2) return 2;
            return -1;
        }
    }

    public bool PKRS_Infected { get => PKRS_Strain != 0; set => PKRS_Strain = value ? Math.Max(PKRS_Strain, 1) : 0; }

    public bool PKRS_Cured
    {
        get => PKRS_Days == 0 && PKRS_Strain > 0;
        set
        {
            PKRS_Days = value ? 0 : 1;
            PKRS_Infected = true;
        }
    }

    public int CurrentLevel { get => Experience.GetLevel(EXP, PersonalInfo.EXPGrowth); set => EXP = Experience.GetEXP(Stat_Level = value, PersonalInfo.EXPGrowth); }
    public int IVTotal => IV_HP + IV_ATK + IV_DEF + IV_SPA + IV_SPD + IV_SPE;
    public int EVTotal => EV_HP + EV_ATK + EV_DEF + EV_SPA + EV_SPD + EV_SPE;
    public int MaximumIV => Math.Max(Math.Max(Math.Max(Math.Max(Math.Max(IV_HP, IV_ATK), IV_DEF), IV_SPA), IV_SPD), IV_SPE);

    public int FlawlessIVCount
    {
        get
        {
            int max = MaxIV;
            int ctr = 0;
            if (IV_HP == max) ++ctr;
            if (IV_ATK == max) ++ctr;
            if (IV_DEF == max) ++ctr;
            if (IV_SPA == max) ++ctr;
            if (IV_SPD == max) ++ctr;
            if (IV_SPE == max) ++ctr;
            return ctr;
        }
    }

    public string FileName => $"{FileNameWithoutExtension}.{Extension}";

    public string FileNameWithoutExtension => EntityFileNamer.GetName(this);

    public int[] IVs
    {
        get => new[] { IV_HP, IV_ATK, IV_DEF, IV_SPE, IV_SPA, IV_SPD };
        set => SetIVs(value);
    }

    public void GetIVs(Span<int> value)
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

    public void GetEVs(Span<int> value)
    {
        if (value.Length != 6)
            return;
        value[0] = EV_HP;
        value[1] = EV_ATK;
        value[2] = EV_DEF;
        value[3] = EV_SPE;
        value[4] = EV_SPA;
        value[5] = EV_SPD;
    }

    public void SetEVs(ReadOnlySpan<int> value)
    {
        if (value.Length != 6)
            return;
        EV_HP = value[0];
        EV_ATK = value[1];
        EV_DEF = value[2];
        EV_SPE = value[3];
        EV_SPA = value[4];
        EV_SPD = value[5];
    }

    public int[] Stats
    {
        get => new[] { Stat_HPCurrent, Stat_ATK, Stat_DEF, Stat_SPE, Stat_SPA, Stat_SPD };
        set
        {
            if (value.Length != 6)
                return;
            Stat_HPCurrent = value[0]; Stat_ATK = value[1]; Stat_DEF = value[2];
            Stat_SPE = value[3]; Stat_SPA = value[4]; Stat_SPD = value[5];
        }
    }

    public ushort[] Moves
    {
        get => new[] { Move1, Move2, Move3, Move4 };
        set => SetMoves(value);
    }

    public void PushMove(ushort move)
    {
        if (move == 0 || move >= MaxMoveID || HasMove(move))
            return;

        var ct = MoveCount;
        if (ct == 4)
            ct = 0;
        SetMove(ct, move);
        HealPPIndex(ct);
    }

    public int MoveCount => Convert.ToInt32(Move1 != 0) + Convert.ToInt32(Move2 != 0) + Convert.ToInt32(Move3 != 0) + Convert.ToInt32(Move4 != 0);

    public void GetMoves(Span<ushort> value)
    {
        value[3] = Move4;
        value[2] = Move3;
        value[1] = Move2;
        value[0] = Move1;
    }

    public void SetMoves(Moveset value)
    {
        Move1 = value.Move1;
        Move2 = value.Move2;
        Move3 = value.Move3;
        Move4 = value.Move4;
    }

    public void SetMoves(ReadOnlySpan<ushort> value)
    {
        Move1 = value.Length > 0 ? value[0] : default;
        Move2 = value.Length > 1 ? value[1] : default;
        Move3 = value.Length > 2 ? value[2] : default;
        Move4 = value.Length > 3 ? value[3] : default;
    }

    public ushort[] RelearnMoves
    {
        get => new[] { RelearnMove1, RelearnMove2, RelearnMove3, RelearnMove4 };
        set => SetRelearnMoves(value);
    }

    public void SetRelearnMoves(Moveset value)
    {
        RelearnMove1 = value.Move1;
        RelearnMove2 = value.Move2;
        RelearnMove3 = value.Move3;
        RelearnMove4 = value.Move4;
    }

    public void SetRelearnMoves(ReadOnlySpan<ushort> value)
    {
        RelearnMove1 = value.Length > 0 ? value[0] : default;
        RelearnMove2 = value.Length > 1 ? value[1] : default;
        RelearnMove3 = value.Length > 2 ? value[2] : default;
        RelearnMove4 = value.Length > 3 ? value[3] : default;
    }

    public int PIDAbility
    {
        get
        {
            if (Generation > 5 || Format > 5)
                return -1;

            if (Version == (int) CXD)
                return PersonalInfo.GetIndexOfAbility(Ability); // Can mismatch; not tied to PID
            return (int)((Gen5 ? PID >> 16 : PID) & 1);
        }
    }

    public abstract int MarkingCount { get; }
    public abstract int GetMarking(int index);
    public abstract void SetMarking(int index, int value);

    private int HPBitValPower => ((IV_HP & 2) >> 1) | ((IV_ATK & 2) >> 0) | ((IV_DEF & 2) << 1) | ((IV_SPE & 2) << 2) | ((IV_SPA & 2) << 3) | ((IV_SPD & 2) << 4);
    public virtual int HPPower => Format < 6 ? ((40 * HPBitValPower) / 63) + 30 : 60;

    private int HPBitValType =>  ((IV_HP & 1) >> 0) | ((IV_ATK & 1) << 1) | ((IV_DEF & 1) << 2) | ((IV_SPE & 1) << 3) | ((IV_SPA & 1) << 4) | ((IV_SPD & 1) << 5);

    public virtual int HPType
    {
        get => 15 * HPBitValType / 63;
        set
        {
            var arr = HiddenPower.DefaultLowBits;
            var bits = (uint)value >= arr.Length ? 0 : arr[value];
            IV_HP = (IV_HP & ~1)   + ((bits >> 0) & 1);
            IV_ATK = (IV_ATK & ~1) + ((bits >> 1) & 1);
            IV_DEF = (IV_DEF & ~1) + ((bits >> 2) & 1);
            IV_SPE = (IV_SPE & ~1) + ((bits >> 3) & 1);
            IV_SPA = (IV_SPA & ~1) + ((bits >> 4) & 1);
            IV_SPD = (IV_SPD & ~1) + ((bits >> 5) & 1);
        }
    }

    // Misc Egg Facts
    public virtual bool WasEgg => IsEgg || Egg_Day != 0;
    public bool WasTradedEgg => Egg_Location == GetTradedEggLocation();
    public bool IsTradedEgg => Met_Location == GetTradedEggLocation();
    private int GetTradedEggLocation() => Locations.TradedEggLocation(Generation, (GameVersion)Version);

    public virtual bool IsUntraded => false;
    public virtual bool IsNative => Generation == Format;
    public bool IsOriginValid => Species <= MaxSpeciesID;

    /// <summary>
    /// Checks if the PKM has its original met location.
    /// </summary>
    /// <returns>Returns false if the Met Location has been overwritten via generational transfer.</returns>
    public virtual bool HasOriginalMetLocation => !(Format < 3 || VC || (Generation <= 4 && Format != Generation));

    /// <summary>
    /// Checks if the current <see cref="Gender"/> is valid.
    /// </summary>
    /// <returns>True if valid, False if invalid.</returns>
    public virtual bool IsGenderValid()
    {
        int gender = Gender;
        var gv = PersonalInfo.Gender;
        if (gv == PersonalInfo.RatioMagicGenderless)
            return gender == 2;
        if (gv == PersonalInfo.RatioMagicFemale)
            return gender == 1;
        if (gv == PersonalInfo.RatioMagicMale)
            return gender == 0;

        int gen = Generation;
        if (gen is not (3 or 4 or 5))
            return gender == (gender & 1);

        return gender == EntityGender.GetFromPIDAndRatio(PID, gv);
    }

    /// <summary>
    /// Updates the checksum of the <see cref="PKM"/>.
    /// </summary>
    public abstract void RefreshChecksum();

    /// <summary>
    /// Indicates if the data has a proper checksum.
    /// </summary>
    /// <remarks>Returns true for structures that do not compute or contain a checksum in the structure.</remarks>
    public abstract bool ChecksumValid { get; }

    /// <summary>
    /// Reorders moves and fixes PP if necessary.
    /// </summary>
    public void FixMoves()
    {
        ReorderMoves();

        if (Move1 == 0) Move1_PP = Move1_PPUps = 0;
        if (Move2 == 0) Move2_PP = Move2_PPUps = 0;
        if (Move3 == 0) Move3_PP = Move3_PPUps = 0;
        if (Move4 == 0) Move4_PP = Move4_PPUps = 0;
    }

    /// <summary>
    /// Reorders moves to put Empty entries last.
    /// </summary>
    private void ReorderMoves()
    {
        if (Move1 == 0 && Move2 != 0)
        {
            Move1 = Move2;
            Move1_PP = Move2_PP;
            Move1_PPUps = Move2_PPUps;
            Move2 = 0;
        }
        if (Move2 == 0 && Move3 != 0)
        {
            Move2 = Move3;
            Move2_PP = Move3_PP;
            Move2_PPUps = Move3_PPUps;
            Move3 = 0;
        }
        if (Move3 == 0 && Move4 != 0)
        {
            Move3 = Move4;
            Move3_PP = Move4_PP;
            Move3_PPUps = Move4_PPUps;
            Move4 = 0;
        }
    }

    /// <summary>
    /// Applies the desired Ability option.
    /// </summary>
    /// <param name="n">Ability Number (0/1/2)</param>
    public virtual void RefreshAbility(int n)
    {
        AbilityNumber = 1 << n;
        IPersonalAbility pi = PersonalInfo;
        if ((uint)n < pi.AbilityCount)
            Ability = pi.GetAbilityAtIndex(n);
    }

    /// <summary>
    /// Gets the IV Judge Rating value.
    /// </summary>
    /// <remarks>IV Judge scales his response 0 (worst) to 3 (best).</remarks>
    public int PotentialRating => IVTotal switch
    {
        <=  90 => 0,
        <= 120 => 1,
        <= 150 => 2,
        _      => 3,
    };

    /// <summary>
    /// Gets the current Battle Stats.
    /// </summary>
    /// <param name="p"><see cref="PersonalInfo"/> entry containing Base Stat Info</param>
    /// <returns>Battle Stats (H/A/B/S/C/D)</returns>
    public ushort[] GetStats(IBaseStat p)
    {
        ushort[] stats = new ushort[6];
        LoadStats(p, stats);
        return stats;
    }

    public virtual void LoadStats(IBaseStat p, Span<ushort> stats)
    {
        int level = CurrentLevel; // recalculate instead of checking Stat_Level
        if (this is IHyperTrain t)
            LoadStats(stats, p, t, level);
        else
            LoadStats(stats, p, level);

        // Account for nature
        NatureAmp.ModifyStatsForNature(stats, StatNature);
    }

    private void LoadStats(Span<ushort> stats, IBaseStat p, IHyperTrain t, int level)
    {
        stats[0] = (ushort)(p.HP == 1 ? 1 : (((t.HT_HP ? 31 : IV_HP) + (2 * p.HP) + (EV_HP / 4) + 100) * level / 100) + 10);
        stats[1] = (ushort)((((t.HT_ATK ? 31 : IV_ATK) + (2 * p.ATK) + (EV_ATK / 4)) * level / 100) + 5);
        stats[2] = (ushort)((((t.HT_DEF ? 31 : IV_DEF) + (2 * p.DEF) + (EV_DEF / 4)) * level / 100) + 5);
        stats[4] = (ushort)((((t.HT_SPA ? 31 : IV_SPA) + (2 * p.SPA) + (EV_SPA / 4)) * level / 100) + 5);
        stats[5] = (ushort)((((t.HT_SPD ? 31 : IV_SPD) + (2 * p.SPD) + (EV_SPD / 4)) * level / 100) + 5);
        stats[3] = (ushort)((((t.HT_SPE ? 31 : IV_SPE) + (2 * p.SPE) + (EV_SPE / 4)) * level / 100) + 5);
    }

    private void LoadStats(Span<ushort> stats, IBaseStat p, int level)
    {
        stats[0] = (ushort)(p.HP == 1 ? 1 : ((IV_HP + (2 * p.HP) + (EV_HP / 4) + 100) * level / 100) + 10);
        stats[1] = (ushort)(((IV_ATK + (2 * p.ATK) + (EV_ATK / 4)) * level / 100) + 5);
        stats[2] = (ushort)(((IV_DEF + (2 * p.DEF) + (EV_DEF / 4)) * level / 100) + 5);
        stats[4] = (ushort)(((IV_SPA + (2 * p.SPA) + (EV_SPA / 4)) * level / 100) + 5);
        stats[5] = (ushort)(((IV_SPD + (2 * p.SPD) + (EV_SPD / 4)) * level / 100) + 5);
        stats[3] = (ushort)(((IV_SPE + (2 * p.SPE) + (EV_SPE / 4)) * level / 100) + 5);
    }

    /// <summary>
    /// Applies the specified stats to the <see cref="PKM"/>.
    /// </summary>
    /// <param name="stats">Battle Stats (H/A/B/S/C/D)</param>
    public void SetStats(ReadOnlySpan<ushort> stats)
    {
        Stat_HPMax = Stat_HPCurrent = stats[0];
        Stat_ATK = stats[1];
        Stat_DEF = stats[2];
        Stat_SPE = stats[3];
        Stat_SPA = stats[4];
        Stat_SPD = stats[5];
    }

    /// <summary>
    /// Indicates if Party Stats are present. False if not initialized (from stored format).
    /// </summary>
    public bool PartyStatsPresent => Stat_HPMax != 0;

    /// <summary>
    /// Clears any status condition and refreshes the stats.
    /// </summary>
    public void ResetPartyStats()
    {
        Span<ushort> stats = stackalloc ushort[6];
        LoadStats(PersonalInfo, stats);
        SetStats(stats);
        Stat_Level = CurrentLevel;
        Status_Condition = 0;
    }

    public void Heal()
    {
        ResetPartyStats();
        HealPP();
    }

    /// <summary>
    /// Restores PP to maximum based on the current PP Ups for each move.
    /// </summary>
    public void HealPP()
    {
        Move1_PP = GetMovePP(Move1, Move1_PPUps);
        Move2_PP = GetMovePP(Move2, Move2_PPUps);
        Move3_PP = GetMovePP(Move3, Move3_PPUps);
        Move4_PP = GetMovePP(Move4, Move4_PPUps);
    }

    public int HealPPIndex(int index) => index switch
    {
        0 => Move1_PP = GetMovePP(Move1, Move1_PPUps),
        1 => Move2_PP = GetMovePP(Move2, Move2_PPUps),
        2 => Move3_PP = GetMovePP(Move3, Move3_PPUps),
        3 => Move4_PP = GetMovePP(Move4, Move4_PPUps),
        _ => throw new ArgumentOutOfRangeException(nameof(index), index, "Index must be between 0 and 3."),
    };

    /// <summary>
    /// Enforces that Party Stat values are present.
    /// </summary>
    /// <returns>True if stats were refreshed, false if stats were already present.</returns>
    public bool ForcePartyData()
    {
        if (PartyStatsPresent)
            return false;
        ResetPartyStats();
        return true;
    }

    /// <summary>
    /// Checks if the <see cref="PKM"/> can hold its <see cref="HeldItem"/>.
    /// </summary>
    /// <param name="valid">Items that the <see cref="PKM"/> can hold.</param>
    /// <returns>True/False if the <see cref="PKM"/> can hold its <see cref="HeldItem"/>.</returns>
    public virtual bool CanHoldItem(ReadOnlySpan<ushort> valid) => valid.Contains((ushort)HeldItem);

    /// <summary>
    /// Deep clones the <see cref="PKM"/> object. The clone will not have any shared resources with the source.
    /// </summary>
    /// <returns>Cloned <see cref="PKM"/> object</returns>
    public abstract PKM Clone();

    /// <summary>
    /// Sets Link Trade data for an <see cref="IsEgg"/>.
    /// </summary>
    /// <param name="day">Day the <see cref="PKM"/> was traded.</param>
    /// <param name="month">Month the <see cref="PKM"/> was traded.</param>
    /// <param name="y">Day the <see cref="PKM"/> was traded.</param>
    /// <param name="location">Link Trade location value.</param>
    protected void SetLinkTradeEgg(int day, int month, int y, int location)
    {
        Met_Day = day;
        Met_Month = month;
        Met_Year = y - 2000;
        Met_Location = location;
    }

    /// <summary>
    /// Gets the PP of a Move ID with consideration of the amount of PP Ups applied.
    /// </summary>
    /// <param name="move">Move ID</param>
    /// <param name="ppUpCount">PP Ups count</param>
    /// <returns>Current PP for the move.</returns>
    public virtual int GetMovePP(ushort move, int ppUpCount) => GetBasePP(move) * (5 + ppUpCount) / 5;

    /// <summary>
    /// Gets the base PP of a move ID depending on the <see cref="PKM"/>'s format.
    /// </summary>
    /// <param name="move">Move ID</param>
    /// <returns>Amount of PP the move has by default (no PP Ups).</returns>
    private int GetBasePP(ushort move) => MoveInfo.GetPP(Context, move);

    /// <summary>
    /// Applies a shiny <see cref="PID"/> to the <see cref="PKM"/>.
    /// </summary>
    /// <remarks>
    /// If a <see cref="PKM"/> originated in a generation prior to Generation 6, the <see cref="EncryptionConstant"/> is updated.
    /// If a <see cref="PKM"/> is in the <see cref="GBPKM"/> format, it will update the <see cref="IVs"/> instead.
    /// </remarks>
    public virtual void SetShiny()
    {
        var rnd = Util.Rand;
        do { PID = EntityPID.GetRandomPID(rnd, Species, Gender, Version, Nature, Form, PID); }
        while (!IsShiny);
        if (Format >= 6 && (Gen3 || Gen4 || Gen5))
            EncryptionConstant = PID;
    }

    /// <summary>
    /// Applies a shiny <see cref="ITrainerID32.SID16"/> to the <see cref="PKM"/>.
    /// </summary>
    public void SetShinySID(Shiny shiny = Shiny.Random)
    {
        if (IsShiny && shiny.IsValid(this))
            return;

        ushort bits = shiny switch
        {
            Shiny.AlwaysSquare => 0,
            Shiny.AlwaysStar => 1,
            _ => (ushort)Util.Rand.Next(8),
        };

        var current = ShinyXor;
        current ^= bits;
        SID16 ^= current;
    }

    /// <summary>
    /// Applies a <see cref="PID"/> to the <see cref="PKM"/> according to the specified <see cref="Gender"/>.
    /// </summary>
    /// <param name="gender"><see cref="Gender"/> to apply</param>
    /// <remarks>
    /// If a <see cref="PKM"/> originated in a generation prior to Generation 6, the <see cref="EncryptionConstant"/> is updated.
    /// </remarks>
    public void SetPIDGender(int gender)
    {
        var rnd = Util.Rand;
        do PID = EntityPID.GetRandomPID(rnd, Species, gender, Version, Nature, Form, PID);
        while (IsShiny);
        if (Format >= 6 && (Gen3 || Gen4 || Gen5))
            EncryptionConstant = PID;
    }

    /// <summary>
    /// Applies a <see cref="PID"/> to the <see cref="PKM"/> according to the specified <see cref="Gender"/>.
    /// </summary>
    /// <param name="nature"><see cref="Nature"/> to apply</param>
    /// <remarks>
    /// If a <see cref="PKM"/> originated in a generation prior to Generation 6, the <see cref="EncryptionConstant"/> is updated.
    /// </remarks>
    public void SetPIDNature(int nature)
    {
        var rnd = Util.Rand;
        do PID = EntityPID.GetRandomPID(rnd, Species, Gender, Version, nature, Form, PID);
        while (IsShiny);
        if (Format >= 6 && (Gen3 || Gen4 || Gen5))
            EncryptionConstant = PID;
    }

    /// <summary>
    /// Applies a <see cref="PID"/> to the <see cref="PKM"/> according to the specified <see cref="Form"/>.
    /// </summary>
    /// <param name="form"><see cref="Form"/> to apply</param>
    /// <remarks>
    /// This method should only be used for Unown originating in Generation 3 games.
    /// If a <see cref="PKM"/> originated in a generation prior to Generation 6, the <see cref="EncryptionConstant"/> is updated.
    /// </remarks>
    public void SetPIDUnown3(byte form)
    {
        var rnd = Util.Rand;
        do PID = rnd.Rand32(); while (EntityPID.GetUnownForm3(PID) != form);
        if (Format >= 6 && (Gen3 || Gen4 || Gen5))
            EncryptionConstant = PID;
    }

    /// <inheritdoc cref="SetRandomIVs(Span{int},int)"/>
    public void SetRandomIVs(int minFlawless = -1) => SetRandomIVs(stackalloc int[6], minFlawless);

    /// <summary>
    /// Randomizes the IVs within game constraints.
    /// </summary>
    /// <param name="ivs">Temporary variable storage</param>
    /// <param name="minFlawless">Count of flawless IVs to set. If none provided, a count will be detected.</param>
    public void SetRandomIVs(Span<int> ivs, int minFlawless = -1)
    {
        if (Version == (int)GameVersion.GO && minFlawless != 6)
        {
            SetRandomIVsGO(ivs);
            return;
        }

        var rnd = Util.Rand;
        for (int i = 0; i < 6; i++)
            ivs[i] = rnd.Next(MaxIV + 1);

        int count = minFlawless == -1 ? GetFlawlessIVCount() : minFlawless;
        if (count != 0)
        {
            for (int i = 0; i < count; i++)
                ivs[i] = MaxIV;
            rnd.Shuffle(ivs); // Randomize IV order
        }
        SetIVs(ivs);
    }

    /// <inheritdoc cref="SetRandomIVsGO(Span{int},int,int)"/>
    public void SetRandomIVsGO(int minIV = 0, int maxIV = 15) => SetRandomIVsGO(stackalloc int[6], minIV, maxIV);

    public void SetRandomIVsGO(Span<int> ivs, int minIV = 0, int maxIV = 15)
    {
        var rnd = Util.Rand;
        ivs[0] = (rnd.Next(minIV, maxIV + 1) << 1) | 1; // hp
        ivs[1] = ivs[4] = (rnd.Next(minIV, maxIV + 1) << 1) | 1; // attack
        ivs[2] = ivs[5] = (rnd.Next(minIV, maxIV + 1) << 1) | 1; // defense
        ivs[3] = rnd.Next(MaxIV + 1); // speed
        SetIVs(ivs);
    }

    /// <summary>
    /// Randomizes the IVs within game constraints.
    /// </summary>
    /// <param name="template">IV template to generate from</param>
    /// <param name="minFlawless">Count of flawless IVs to set. If none provided, a count will be detected.</param>
    /// <returns>Randomized IVs if desired.</returns>
    public void SetRandomIVsTemplate(IndividualValueSet template, int minFlawless = -1)
    {
        int count = minFlawless == -1 ? GetFlawlessIVCount() : minFlawless;
        Span<int> ivs = stackalloc int[6];
        var rnd = Util.Rand;
        do
        {
            for (int i = 0; i < 6; i++)
                ivs[i] = template[i] < 0 ? rnd.Next(MaxIV + 1) : template[i];
        } while (ivs.Count(MaxIV) < count);

        SetIVs(ivs);
    }

    /// <summary>
    /// Gets the amount of flawless IVs that the <see cref="PKM"/> should have.
    /// </summary>
    /// <returns>Count of IVs that should be max.</returns>
    public int GetFlawlessIVCount()
    {
        int gen = Generation;
        if (gen >= 6)
        {
            var species = Species;
            if (Legal.Mythicals.Contains(species))
                return 3;
            if (Legal.Legends.Contains(species))
                return 3;
            if (Legal.SubLegends.Contains(species))
                return 3;
            if (gen <= 7 && Legal.IsUltraBeast(species))
                return 3;
        }
        if (XY)
        {
            if (PersonalInfo.EggGroup1 == 15) // Undiscovered
                return 3;
            if (Met_Location == 148 && Met_Level == 30) // Friend Safari
                return 2;
        }
        if (VC)
            return Species is (int)Core.Species.Mew or (int)Core.Species.Celebi ? 5 : 3;
        if (this is IAlpha {IsAlpha: true})
            return 3;
        return 0;
    }

    /// <summary>
    /// Applies all shared properties from the current <see cref="PKM"/> to the <see cref="result"/> <see cref="PKM"/>.
    /// </summary>
    /// <param name="result"><see cref="PKM"/> that receives property values.</param>
    public void TransferPropertiesWithReflection(PKM result)
    {
        // Only transfer declared properties not defined in PKM.cs but in the actual type
        var srcType = GetType();
        var destType = result.GetType();

        static IEnumerable<Type> GetImplementingTypes(Type t)
        {
            yield return t;
            while (true)
            {
                var baseType = t.BaseType;
                if (baseType is null || baseType == typeof(PKM))
                    yield break;
                yield return t = baseType;
            }
        }

        var srcTypes = GetImplementingTypes(srcType);
        var srcProperties = srcTypes.SelectMany(ReflectUtil.GetPropertiesCanWritePublicDeclared);
        var destTypes = GetImplementingTypes(destType);
        var destProperties = destTypes.SelectMany(ReflectUtil.GetPropertiesCanWritePublicDeclared);

        // Transfer properties in the order they are defined in the destination PKM format for best conversion
        var shared = destProperties.Intersect(srcProperties);
        foreach (string property in shared)
        {
            // Setter sanity check: a derived type may not implement a setter if its parent type has one.
            if (!BatchEditing.TryGetHasProperty(result, property, out var pi))
                continue;
            if (!pi.CanWrite)
                continue;

            // Fetch the current value.
            if (!BatchEditing.TryGetHasProperty(this, property, out var src))
                continue;
            var prop = src.GetValue(this);
            if (prop is byte[] or null)
                continue; // not a valid property transfer

            // Write it to the destination.
            ReflectUtil.SetValue(pi, result, prop);
        }

        // set shared properties for the Gen1/2 base class
        if (result is GBPKM l)
            l.ImportFromFuture(this);
    }

    /// <summary>
    /// Checks if the <see cref="PKM"/> has the <see cref="move"/> in its current move list.
    /// </summary>
    public bool HasMove(ushort move) => Move1 == move || Move2 == move || Move3 == move || Move4 == move;

    public int GetMoveIndex(ushort move) => Move1 == move ? 0 : Move2 == move ? 1 : Move3 == move ? 2 : Move4 == move ? 3 : -1;

    public ushort GetMove(int index) => index switch
    {
        0 => Move1,
        1 => Move2,
        2 => Move3,
        3 => Move4,
        _ => throw new ArgumentOutOfRangeException(nameof(index), index, "Move index must be between 0 and 3."),
    };

    public ushort SetMove(int index, ushort value) => index switch
    {
        0 => Move1 = value,
        1 => Move2 = value,
        2 => Move3 = value,
        3 => Move4 = value,
        _ => throw new ArgumentOutOfRangeException(nameof(index), index, "Move index must be between 0 and 3."),
    };

    public ushort GetRelearnMove(int index) => index switch
    {
        0 => RelearnMove1,
        1 => RelearnMove2,
        2 => RelearnMove3,
        3 => RelearnMove4,
        _ => throw new ArgumentOutOfRangeException(nameof(index), index, "Move index must be between 0 and 3."),
    };

    public ushort SetRelearnMove(int index, ushort value) => index switch
    {
        0 => RelearnMove1 = value,
        1 => RelearnMove2 = value,
        2 => RelearnMove3 = value,
        3 => RelearnMove4 = value,
        _ => throw new ArgumentOutOfRangeException(nameof(index), index, "Move index must be between 0 and 3."),
    };

    /// <summary>
    /// Checks if the <see cref="PKM"/> has the <see cref="move"/> in its relearn move list.
    /// </summary>
    public bool HasRelearnMove(ushort move) => RelearnMove1 == move || RelearnMove2 == move || RelearnMove3 == move || RelearnMove4 == move;

    public void GetRelearnMoves(Span<ushort> value)
    {
        value[3] = RelearnMove4;
        value[2] = RelearnMove3;
        value[1] = RelearnMove2;
        value[0] = RelearnMove1;
    }

    /// <summary>
    /// Clears moves that a <see cref="PKM"/> may have, possibly from a future generation.
    /// </summary>
    public void ClearInvalidMoves()
    {
        uint invalid = 0;
        Span<ushort> moves = stackalloc ushort[4];
        GetMoves(moves);
        for (var i = 0; i < moves.Length; i++)
        {
            if (moves[i] <= MaxMoveID)
                continue;

            invalid++;
            moves[i] = 0;
        }
        if (invalid == 0)
            return;
        if (invalid == 4) // no moves remain
        {
            moves[0] = 1; // Pound
            Move1_PP = GetMovePP(1, Move1_PPUps);
        }

        SetMoves(moves);
        FixMoves();
    }

    /// <summary>
    /// Gets one of the <see cref="EffortValues"/> based on its index within the array.
    /// </summary>
    /// <param name="index">Index to get</param>
    public int GetEV(int index) => index switch
    {
        0 => EV_HP,
        1 => EV_ATK,
        2 => EV_DEF,
        3 => EV_SPE,
        4 => EV_SPA,
        5 => EV_SPD,
        _ => throw new ArgumentOutOfRangeException(nameof(index), index, "EV index must be between 0 and 5."),
    };

    /// <summary>
    /// Gets one of the <see cref="IVs"/> based on its index within the array.
    /// </summary>
    /// <param name="index">Index to get</param>
    public int GetIV(int index) => index switch
    {
        0 => IV_HP,
        1 => IV_ATK,
        2 => IV_DEF,
        3 => IV_SPE,
        4 => IV_SPA,
        5 => IV_SPD,
        _ => throw new ArgumentOutOfRangeException(nameof(index), index, "IV index must be between 0 and 5."),
    };
}
