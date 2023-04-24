using System;

namespace PKHeX.Core;

/// <summary> Generation 4 <see cref="PKM"/> format. </summary>
public abstract class G4PKM : PKM,
    IRibbonSetEvent3, IRibbonSetEvent4, IRibbonSetUnique3, IRibbonSetUnique4, IRibbonSetCommon3, IRibbonSetCommon4, IRibbonSetRibbons, IContestStats, IGroundTile
{
    protected G4PKM(byte[] data) : base(data) { }
    protected G4PKM(int size) : base(size) { }

    // Maximums
    public sealed override ushort MaxMoveID => Legal.MaxMoveID_4;
    public sealed override ushort MaxSpeciesID => Legal.MaxSpeciesID_4;
    public sealed override int MaxAbilityID => Legal.MaxAbilityID_4;
    public sealed override int MaxItemID => Legal.MaxItemID_4_HGSS;
    public sealed override int MaxBallID => Legal.MaxBallID_4;
    public sealed override int MaxGameID => Legal.MaxGameID_4;
    public sealed override int MaxIV => 31;
    public sealed override int MaxEV => 255;
    public sealed override int MaxStringLengthOT => 7;
    public sealed override int MaxStringLengthNickname => 10;

    public sealed override uint PSV => (((PID >> 16) ^ (PID & 0xFFFF)) >> 3);
    public sealed override uint TSV => (uint)(TID16 ^ SID16) >> 3;

    protected bool PtHGSS => Pt || HGSS;

    public sealed override int Characteristic
    {
        get
        {
            int pm6 = (int)(EncryptionConstant % 6); // PID
            int maxIV = MaximumIV;
            int pm6stat = 0;
            for (int i = 0; i < 6; i++)
            {
                pm6stat = (pm6 + i) % 6;
                if (GetIV(pm6stat) == maxIV)
                    break;
            }
            return (pm6stat * 5) + (maxIV % 5);
        }
    }

    public abstract ushort Sanity { get; set; }
    public abstract ushort Checksum { get; set; }
    public sealed override void RefreshChecksum() => Checksum = CalculateChecksum();
    public sealed override bool ChecksumValid => CalculateChecksum() == Checksum;
    public override bool Valid { get => Sanity == 0 && ChecksumValid; set { if (!value) return; Sanity = 0; RefreshChecksum(); } }
    protected virtual ushort CalculateChecksum() => PokeCrypto.GetCHK(Data.AsSpan()[8..PokeCrypto.SIZE_4STORED]);

    // Trash Bytes
    public sealed override Span<byte> Nickname_Trash => Data.AsSpan(0x48, 22);
    public sealed override Span<byte> OT_Trash => Data.AsSpan(0x68, 16);

    // Future Attributes
    public sealed override uint EncryptionConstant { get => PID; set { } }
    public sealed override int Nature { get => (int)(PID % 25); set { } }
    public sealed override int CurrentFriendship { get => OT_Friendship; set => OT_Friendship = value; }
    public sealed override int CurrentHandler { get => 0; set { } }
    public sealed override int AbilityNumber { get => 1 << PIDAbility; set { } }

    public abstract int ShinyLeaf { get; set; }

    #region Ribbons
    public abstract bool RibbonEarth { get; set; }
    public abstract bool RibbonNational { get; set; }
    public abstract bool RibbonCountry { get; set; }
    public abstract bool RibbonChampionBattle { get; set; }
    public abstract bool RibbonChampionRegional { get; set; }
    public abstract bool RibbonChampionNational { get; set; }
    public abstract bool RibbonClassic { get; set; }
    public abstract bool RibbonWishing { get; set; }
    public abstract bool RibbonPremier { get; set; }
    public abstract bool RibbonEvent { get; set; }
    public abstract bool RibbonBirthday { get; set; }
    public abstract bool RibbonSpecial { get; set; }
    public abstract bool RibbonWorld { get; set; }
    public abstract bool RibbonChampionWorld { get; set; }
    public abstract bool RibbonSouvenir { get; set; }
    public abstract bool RibbonWinning { get; set; }
    public abstract bool RibbonVictory { get; set; }
    public abstract bool RibbonAbility { get; set; }
    public abstract bool RibbonAbilityGreat { get; set; }
    public abstract bool RibbonAbilityDouble { get; set; }
    public abstract bool RibbonAbilityMulti { get; set; }
    public abstract bool RibbonAbilityPair { get; set; }
    public abstract bool RibbonAbilityWorld { get; set; }
    public abstract bool RibbonG3Cool { get; set; }
    public abstract bool RibbonG3CoolSuper { get; set; }
    public abstract bool RibbonG3CoolHyper { get; set; }
    public abstract bool RibbonG3CoolMaster { get; set; }
    public abstract bool RibbonG3Beauty { get; set; }
    public abstract bool RibbonG3BeautySuper { get; set; }
    public abstract bool RibbonG3BeautyHyper { get; set; }
    public abstract bool RibbonG3BeautyMaster { get; set; }
    public abstract bool RibbonG3Cute { get; set; }
    public abstract bool RibbonG3CuteSuper { get; set; }
    public abstract bool RibbonG3CuteHyper { get; set; }
    public abstract bool RibbonG3CuteMaster { get; set; }
    public abstract bool RibbonG3Smart { get; set; }
    public abstract bool RibbonG3SmartSuper { get; set; }
    public abstract bool RibbonG3SmartHyper { get; set; }
    public abstract bool RibbonG3SmartMaster { get; set; }
    public abstract bool RibbonG3Tough { get; set; }
    public abstract bool RibbonG3ToughSuper { get; set; }
    public abstract bool RibbonG3ToughHyper { get; set; }
    public abstract bool RibbonG3ToughMaster { get; set; }
    public abstract bool RibbonG4Cool { get; set; }
    public abstract bool RibbonG4CoolGreat { get; set; }
    public abstract bool RibbonG4CoolUltra { get; set; }
    public abstract bool RibbonG4CoolMaster { get; set; }
    public abstract bool RibbonG4Beauty { get; set; }
    public abstract bool RibbonG4BeautyGreat { get; set; }
    public abstract bool RibbonG4BeautyUltra { get; set; }
    public abstract bool RibbonG4BeautyMaster { get; set; }
    public abstract bool RibbonG4Cute { get; set; }
    public abstract bool RibbonG4CuteGreat { get; set; }
    public abstract bool RibbonG4CuteUltra { get; set; }
    public abstract bool RibbonG4CuteMaster { get; set; }
    public abstract bool RibbonG4Smart { get; set; }
    public abstract bool RibbonG4SmartGreat { get; set; }
    public abstract bool RibbonG4SmartUltra { get; set; }
    public abstract bool RibbonG4SmartMaster { get; set; }
    public abstract bool RibbonG4Tough { get; set; }
    public abstract bool RibbonG4ToughGreat { get; set; }
    public abstract bool RibbonG4ToughUltra { get; set; }
    public abstract bool RibbonG4ToughMaster { get; set; }
    public abstract bool RibbonChampionG3 { get; set; }
    public abstract bool RibbonArtist { get; set; }
    public abstract bool RibbonEffort { get; set; }
    public abstract bool RibbonChampionSinnoh { get; set; }
    public abstract bool RibbonAlert { get; set; }
    public abstract bool RibbonShock { get; set; }
    public abstract bool RibbonDowncast { get; set; }
    public abstract bool RibbonCareless { get; set; }
    public abstract bool RibbonRelax { get; set; }
    public abstract bool RibbonSnooze { get; set; }
    public abstract bool RibbonSmile { get; set; }
    public abstract bool RibbonGorgeous { get; set; }
    public abstract bool RibbonRoyal { get; set; }
    public abstract bool RibbonGorgeousRoyal { get; set; }
    public abstract bool RibbonFootprint { get; set; }
    public abstract bool RibbonRecord { get; set; }
    public abstract bool RibbonLegend { get; set; }
    public abstract int RibbonCount { get; }

    // Unused
    public abstract bool RIB3_4 { get; set; }
    public abstract bool RIB3_5 { get; set; }
    public abstract bool RIB3_6 { get; set; }
    public abstract bool RIB3_7 { get; set; }
    public abstract bool RIBA_4 { get; set; }
    public abstract bool RIBA_5 { get; set; }
    public abstract bool RIBA_6 { get; set; }
    public abstract bool RIBA_7 { get; set; }
    public abstract bool RIBB_0 { get; set; }
    public abstract bool RIBB_1 { get; set; }
    public abstract bool RIBB_2 { get; set; }
    public abstract bool RIBB_3 { get; set; }
    public abstract bool RIBB_4 { get; set; }
    public abstract bool RIBB_5 { get; set; }
    public abstract bool RIBB_6 { get; set; }
    public abstract bool RIBB_7 { get; set; }
    #endregion

    public abstract byte CNT_Cool { get; set; }
    public abstract byte CNT_Beauty { get; set; }
    public abstract byte CNT_Cute { get; set; }
    public abstract byte CNT_Smart { get; set; }
    public abstract byte CNT_Tough { get; set; }
    public abstract byte CNT_Sheen { get; set; }

    public abstract GroundTileType GroundTile { get; set; }
    public abstract byte BallDPPt { get; set; }
    public abstract byte BallHGSS { get; set; }
    public abstract byte PokeathlonStat { get; set; }
    public override int MarkingCount => 6;

    public override int GetMarking(int index)
    {
        if ((uint)index >= MarkingCount)
            throw new ArgumentOutOfRangeException(nameof(index));
        return (MarkValue >> index) & 1;
    }

    public override void SetMarking(int index, int value)
    {
        if ((uint)index >= MarkingCount)
            throw new ArgumentOutOfRangeException(nameof(index));
        MarkValue = (MarkValue & ~(1 << index)) | ((value & 1) << index);
    }

    public abstract ushort Egg_LocationDP { get; set; }
    public abstract ushort Egg_LocationExtended { get; set; }
    public abstract ushort Met_LocationDP { get; set; }
    public abstract ushort Met_LocationExtended { get; set; }

    public sealed override int Egg_Location
    {
        get
        {
            ushort hgssloc = Egg_LocationExtended;
            if (hgssloc != 0)
                return hgssloc;
            return Egg_LocationDP;
        }
        set
        {
            if (value == 0)
            {
                Egg_LocationDP = Egg_LocationExtended = 0;
            }
            else if (Locations.IsPtHGSSLocation(value) || Locations.IsPtHGSSLocationEgg(value))
            {
                // Met location not in DP, set to Faraway Place
                Egg_LocationDP = Locations.Faraway4;
                Egg_LocationExtended = (ushort)value;
            }
            else
            {
                int pthgss = PtHGSS ? value : 0; // only set to PtHGSS loc if encountered in game
                Egg_LocationDP = (ushort)value;
                Egg_LocationExtended = (ushort)pthgss;
            }
        }
    }

    public sealed override int Met_Location
    {
        get
        {
            ushort hgssloc = Met_LocationExtended;
            if (hgssloc != 0)
                return hgssloc;
            return Met_LocationDP;
        }
        set
        {
            if (value == 0)
            {
                Met_LocationDP = Met_LocationExtended = 0;
            }
            else if (Locations.IsPtHGSSLocation(value) || Locations.IsPtHGSSLocationEgg(value))
            {
                // Met location not in DP, set to Faraway Place
                Met_LocationDP = Locations.Faraway4;
                Met_LocationExtended = (ushort)value;
            }
            else
            {
                int pthgss = PtHGSS ? value : 0; // only set to PtHGSS loc if encountered in game
                Met_LocationDP = (ushort)value;
                Met_LocationExtended = (ushort)pthgss;
            }
        }
    }

    public sealed override int Ball
    {
        // HG/SS added new ball IDs mid-generation, and the previous Gen4 games couldn't handle invalid ball values.
        // Pokémon obtained in HG/SS have the HG/SS ball value set (@0x86) to the capture ball.
        // However, this info is not set in event gift data!
        // Event gift data contains a pre-formatted PK4 template, which is slightly mutated.
        // No HG/SS ball values were used in these event gifts, and no HG/SS ball values are set (0).

        // To get the display ball (assume HG/SS +), return the higher of the two values.
        get => Math.Max(BallHGSS, BallDPPt);
        set
        {
            static byte Clamp(int value, Ball max) => (uint)value <= (uint)max ? (byte)value : (byte)Core.Ball.Poke;

            // Ball to display in DPPt
            BallDPPt = Clamp(value, Core.Ball.Cherish);

            // Only set the HG/SS value if it originated in HG/SS and was not an event.
            if (!HGSS || FatefulEncounter)
                BallHGSS = 0;
            else
                BallHGSS = Clamp(value, Core.Ball.Sport);
        }
    }

    // Synthetic Trading Logic
    public bool Trade(string SAV_Trainer, uint savID32, int SAV_GENDER, int Day = 1, int Month = 1, int Year = 2009)
    {
        // Eggs do not have any modifications done if they are traded
        if (IsEgg && !(SAV_Trainer == OT_Name && savID32 == ID32 && SAV_GENDER == OT_Gender))
        {
            SetLinkTradeEgg(Day, Month, Year, Locations.LinkTrade4);
            return true;
        }
        return false;
    }

    // Enforce DP content only (no PtHGSS)
    protected void StripPtHGSSContent(PKM pk)
    {
        if (Form != 0 && !PersonalTable.DP[Species].HasForms && Species != 201)
            pk.Form = 0;
        if (HeldItem > Legal.MaxItemID_4_DP)
            pk.HeldItem = 0;
    }

    protected T ConvertTo<T>() where T : G4PKM, new()
    {
        var pk = new T
        {
            PID = PID,
            Species = Species,
            HeldItem = HeldItem,
            TID16 = TID16,
            SID16 = SID16,
            EXP = EXP,
            OT_Friendship = OT_Friendship,
            Ability = Ability,
            Language = Language,

            IsEgg = IsEgg,
            IsNicknamed = IsNicknamed,
            OT_Gender = OT_Gender,

            IV_HP = IV_HP,
            IV_ATK = IV_ATK,
            IV_DEF = IV_DEF,
            IV_SPE = IV_SPE,
            IV_SPA = IV_SPA,
            IV_SPD = IV_SPD,
            EV_HP = EV_HP,
            EV_ATK = EV_ATK,
            EV_DEF = EV_DEF,
            EV_SPE = EV_SPE,
            EV_SPA = EV_SPA,
            EV_SPD = EV_SPD,
            CNT_Cool = CNT_Cool,
            CNT_Beauty = CNT_Beauty,
            CNT_Cute = CNT_Cute,
            CNT_Smart = CNT_Smart,
            CNT_Tough = CNT_Tough,
            CNT_Sheen = CNT_Sheen,

            Move1 = Move1,
            Move2 = Move2,
            Move3 = Move3,
            Move4 = Move4,
            Move1_PP = Move1_PP,
            Move2_PP = Move2_PP,
            Move3_PP = Move3_PP,
            Move4_PP = Move4_PP,
            Move1_PPUps = Move1_PPUps,
            Move2_PPUps = Move2_PPUps,
            Move3_PPUps = Move3_PPUps,
            Move4_PPUps = Move4_PPUps,

            Gender = Gender,
            Form = Form,
            ShinyLeaf = ShinyLeaf,
            Version = Version,
            PKRS_Days = PKRS_Days,
            PKRS_Strain = PKRS_Strain,
            BallDPPt = BallDPPt,
            BallHGSS = BallHGSS,
            GroundTile = GroundTile,
            PokeathlonStat = PokeathlonStat,
            FatefulEncounter = FatefulEncounter,

            Met_Level = Met_Level,
            Met_Location = Met_Location,
            Met_Year = Met_Year,
            Met_Month = Met_Month,
            Met_Day = Met_Day,

            Egg_Location = Egg_Location,
            Egg_Year = Egg_Year,
            Egg_Month = Egg_Month,
            Egg_Day = Egg_Day,

            #region Ribbons
            RibbonChampionSinnoh = RibbonChampionSinnoh,
            RibbonAbility = RibbonAbility,
            RibbonAbilityGreat = RibbonAbilityGreat,
            RibbonAbilityDouble = RibbonAbilityDouble,
            RibbonAbilityMulti = RibbonAbilityMulti,
            RibbonAbilityPair = RibbonAbilityPair,
            RibbonAbilityWorld = RibbonAbilityWorld,
            RibbonAlert = RibbonAlert,
            RibbonShock = RibbonShock,
            RibbonDowncast = RibbonDowncast,
            RibbonCareless = RibbonCareless,
            RibbonRelax = RibbonRelax,
            RibbonSnooze = RibbonSnooze,
            RibbonSmile = RibbonSmile,
            RibbonGorgeous = RibbonGorgeous,
            RibbonRoyal = RibbonRoyal,
            RibbonGorgeousRoyal = RibbonGorgeousRoyal,
            RibbonFootprint = RibbonFootprint,
            RibbonRecord = RibbonRecord,
            RibbonEvent = RibbonEvent,
            RibbonLegend = RibbonLegend,
            RibbonChampionWorld = RibbonChampionWorld,
            RibbonBirthday = RibbonBirthday,
            RibbonSpecial = RibbonSpecial,
            RibbonSouvenir = RibbonSouvenir,
            RibbonWishing = RibbonWishing,
            RibbonClassic = RibbonClassic,
            RibbonPremier = RibbonPremier,
            RibbonG3Cool = RibbonG3Cool,
            RibbonG3CoolSuper = RibbonG3CoolSuper,
            RibbonG3CoolHyper = RibbonG3CoolHyper,
            RibbonG3CoolMaster = RibbonG3CoolMaster,
            RibbonG3Beauty = RibbonG3Beauty,
            RibbonG3BeautySuper = RibbonG3BeautySuper,
            RibbonG3BeautyHyper = RibbonG3BeautyHyper,
            RibbonG3BeautyMaster = RibbonG3BeautyMaster,
            RibbonG3Cute = RibbonG3Cute,
            RibbonG3CuteSuper = RibbonG3CuteSuper,
            RibbonG3CuteHyper = RibbonG3CuteHyper,
            RibbonG3CuteMaster = RibbonG3CuteMaster,
            RibbonG3Smart = RibbonG3Smart,
            RibbonG3SmartSuper = RibbonG3SmartSuper,
            RibbonG3SmartHyper = RibbonG3SmartHyper,
            RibbonG3SmartMaster = RibbonG3SmartMaster,
            RibbonG3Tough = RibbonG3Tough,
            RibbonG3ToughSuper = RibbonG3ToughSuper,
            RibbonG3ToughHyper = RibbonG3ToughHyper,
            RibbonG3ToughMaster = RibbonG3ToughMaster,
            RibbonChampionG3 = RibbonChampionG3,
            RibbonWinning = RibbonWinning,
            RibbonVictory = RibbonVictory,
            RibbonArtist = RibbonArtist,
            RibbonEffort = RibbonEffort,
            RibbonChampionBattle = RibbonChampionBattle,
            RibbonChampionRegional = RibbonChampionRegional,
            RibbonChampionNational = RibbonChampionNational,
            RibbonCountry = RibbonCountry,
            RibbonNational = RibbonNational,
            RibbonEarth = RibbonEarth,
            RibbonWorld = RibbonWorld,
            RibbonG4Cool = RibbonG4Cool,
            RibbonG4CoolGreat = RibbonG4CoolGreat,
            RibbonG4CoolUltra = RibbonG4CoolUltra,
            RibbonG4CoolMaster = RibbonG4CoolMaster,
            RibbonG4Beauty = RibbonG4Beauty,
            RibbonG4BeautyGreat = RibbonG4BeautyGreat,
            RibbonG4BeautyUltra = RibbonG4BeautyUltra,
            RibbonG4BeautyMaster = RibbonG4BeautyMaster,
            RibbonG4Cute = RibbonG4Cute,
            RibbonG4CuteGreat = RibbonG4CuteGreat,
            RibbonG4CuteUltra = RibbonG4CuteUltra,
            RibbonG4CuteMaster = RibbonG4CuteMaster,
            RibbonG4Smart = RibbonG4Smart,
            RibbonG4SmartGreat = RibbonG4SmartGreat,
            RibbonG4SmartUltra = RibbonG4SmartUltra,
            RibbonG4SmartMaster = RibbonG4SmartMaster,
            RibbonG4Tough = RibbonG4Tough,
            RibbonG4ToughGreat = RibbonG4ToughGreat,
            RibbonG4ToughUltra = RibbonG4ToughUltra,
            RibbonG4ToughMaster = RibbonG4ToughMaster,
            RIB3_4 = RIB3_4,
            RIB3_5 = RIB3_5,
            RIB3_6 = RIB3_6,
            RIB3_7 = RIB3_7,
            RIBA_4 = RIBA_4,
            RIBA_5 = RIBA_5,
            RIBA_6 = RIBA_6,
            RIBA_7 = RIBA_7,
            RIBB_0 = RIBB_0,
            RIBB_1 = RIBB_1,
            RIBB_2 = RIBB_2,
            RIBB_3 = RIBB_3,
            RIBB_4 = RIBB_4,
            RIBB_5 = RIBB_5,
            RIBB_6 = RIBB_6,
            RIBB_7 = RIBB_7,
            #endregion
        };

        // Transfer Trash Bytes
        for (int i = 0; i < 11; i++) // Nickname
        {
            pk.Data[0x48 + (2 * i)] = Data[0x48 + (2 * i) + 1];
            pk.Data[0x48 + (2 * i) + 1] = Data[0x48 + (2 * i)];
        }
        for (int i = 0; i < 8; i++) // OT_Name
        {
            pk.Data[0x68 + (2 * i)] = Data[0x68 + (2 * i) + 1];
            pk.Data[0x68 + (2 * i) + 1] = Data[0x68 + (2 * i)];
        }
        return pk;
    }
}
