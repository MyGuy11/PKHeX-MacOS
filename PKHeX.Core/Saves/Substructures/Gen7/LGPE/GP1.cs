using System;
using System.Collections.Generic;
using System.Text;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Go Park Entity transferred from <see cref="GameVersion.GO"/> to <see cref="GameVersion.GG"/>.
/// </summary>
public sealed class GP1 : IEncounterInfo, IFixedAbilityNumber, IScaledSizeReadOnly
{
    public const int SIZE = 0x1B0;
    public readonly byte[] Data;

    public GameVersion Version => GameVersion.GO;
    public bool EggEncounter => false;
    public byte LevelMin => Level;
    public byte LevelMax => Level;
    public int Generation => 7;
    public EntityContext Context => EntityContext.Gen7b;
    public AbilityPermission Ability => AbilityPermission.Any12;
    public PKM ConvertToPKM(ITrainerInfo tr) => ConvertToPB7(tr);
    public PKM ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPB7(tr, criteria);

    public GP1(byte[] data) => Data = data;
    public GP1() : this(new byte[SIZE]) => InitializeBlank(Data);
    public void WriteTo(byte[] data, int offset) => Data.CopyTo(data, offset);

    public static GP1 FromData(byte[] data, int offset)
    {
        var span = data.AsSpan(offset);
        return FromData(span);
    }

    private static GP1 FromData(ReadOnlySpan<byte> span)
    {
        var result = new GP1();
        span[..SIZE].CopyTo(result.Data);
        return result;
    }

    /// <summary>
    /// First 0x20 bytes of an empty <see cref="GP1"/>, all other bytes are 0.
    /// </summary>
    private static ReadOnlySpan<byte> Blank20 => new byte[]
    {
        0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x80, 0x3F, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x3F,
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x3F, 0x00, 0x00, 0x80, 0x3F, 0x00, 0x00, 0x80, 0x3F,
        0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x85, 0xEC, 0x33, 0x01,
    };

    public static void InitializeBlank(Span<byte> data) => Blank20.CopyTo(data);

    public string Username1 => Util.TrimFromZero(Encoding.ASCII.GetString(Data.AsSpan(0x00, 0x10)));
    public string Username2 => Util.TrimFromZero(Encoding.ASCII.GetString(Data.AsSpan(0x10, 0x10)));

    public ushort Species => ReadUInt16LittleEndian(Data.AsSpan(0x28)); // s32, just read as u16
    public int CP => ReadInt32LittleEndian(Data.AsSpan(0x2C));
    public float LevelF => ReadSingleLittleEndian(Data.AsSpan(0x30));
    public byte Level => Math.Max((byte)1, (byte)Math.Round(LevelF));
    public int Stat_HP => ReadInt32LittleEndian(Data.AsSpan(0x34));
    // geolocation data 0x38-0x47?
    public float HeightF => ReadSingleLittleEndian(Data.AsSpan(0x48));
    public float WeightF => ReadSingleLittleEndian(Data.AsSpan(0x4C));

    public byte HeightScalar
    {
        get
        {
            var height = HeightF * 100f;
            var pi = PersonalTable.GG.GetFormEntry(Species, Form);
            var avgHeight = pi.Height;
            return PB7.GetHeightScalar(height, avgHeight);
        }
    }

    public byte WeightScalar
    {
        get
        {
            var height = HeightF * 100f;
            var weight = WeightF * 10f;
            var pi = PersonalTable.GG.GetFormEntry(Species, Form);
            var avgHeight = pi.Height;
            var avgWeight = pi.Weight;
            return PB7.GetWeightScalar(height, weight, avgHeight, avgWeight);
        }
    }

    public int IV_HP => ReadInt32LittleEndian(Data.AsSpan(0x50));
    public int IV_ATK => ReadInt32LittleEndian(Data.AsSpan(0x54));
    public int IV_DEF => ReadInt32LittleEndian(Data.AsSpan(0x58));
    public int Date => ReadInt32LittleEndian(Data.AsSpan(0x5C)); // ####.##.## YYYY.MM.DD
    public int Year => Date / 1_00_00;
    public int Month => (Date / 1_00) % 1_00;
    public int Day => Date % 1_00;

    public int Gender => Data[0x70] - 1; // M=1, F=2, G=3 ;; shift down by 1.

    public byte Form => Data[0x72];
    public bool IsShiny => Data[0x73] == 1;

    // https://bulbapedia.bulbagarden.net/wiki/List_of_moves_in_Pok%C3%A9mon_GO
    public int Move1 => ReadInt32LittleEndian(Data.AsSpan(0x74)); // uses Go Indexes
    public int Move2 => ReadInt32LittleEndian(Data.AsSpan(0x78)); // uses Go Indexes

    public string GeoCityName => Util.TrimFromZero(Encoding.ASCII.GetString(Data, 0x7C, 0x60)); // dunno length

    public string Nickname => Util.TrimFromZero(Encoding.ASCII.GetString(Data, 0x12D, 0x20)); // dunno length

    public static readonly IReadOnlyList<string> Genders = GameInfo.GenderSymbolASCII;
    public string GenderString => (uint) Gender >= Genders.Count ? string.Empty : Genders[Gender];
    public string ShinyString => IsShiny ? "★ " : string.Empty;
    public string FormString => Form != 0 ? $"-{Form}" : string.Empty;
    private string NickStr => string.IsNullOrWhiteSpace(Nickname) ? SpeciesName.GetSpeciesNameGeneration(Species, (int)LanguageID.English, 7) : Nickname;
    public string FileName => $"{FileNameWithoutExtension}.gp1";

    public string FileNameWithoutExtension
    {
        get
        {
            string form = Form > 0 ? $"-{Form:00}" : string.Empty;
            string star = IsShiny ? " ★" : string.Empty;
            return $"{Species:000}{form}{star} - {NickStr} - Lv. {Level:00} - {IV_HP:00}.{IV_ATK:00}.{IV_DEF:00} - CP {CP:0000} (Moves {Move1:000}, {Move2:000})";
        }
    }

    public string GeoTime => $"Captured in {GeoCityName} by {Username1} on {Year}/{Month:00}/{Day:00}";
    public string StatMove => $"{IV_HP:00}/{IV_ATK:00}/{IV_DEF:00}, CP {CP:0000} (Moves {Move1:000}, {Move2:000})";
    public string Dump(IReadOnlyList<string> speciesNames, int index) => $"{index:000} {Nickname} ({speciesNames[Species]}{FormString} {ShinyString}[{GenderString}]) @ Lv. {Level:00} - {StatMove}, {GeoTime}.";

    /// <summary>
    /// GO Park transfers start with 2 AVs for all stats.
    /// </summary>
    public const byte InitialAV = 2;

    public PB7 ConvertToPB7(ITrainerInfo sav) => ConvertToPB7(sav, EncounterCriteria.Unrestricted);

    public PB7 ConvertToPB7(ITrainerInfo sav, EncounterCriteria criteria)
    {
        var rnd = Util.Rand;
        var pk = new PB7
        {
            EncryptionConstant = rnd.Rand32(),
            PID = rnd.Rand32(),
            Version = (int) GameVersion.GO,
            Species = Species,
            Form = Form,
            Met_Location = 50, // Go complex
            Met_Year = Year - 2000,
            Met_Month = Month,
            Met_Day = Day,
            CurrentLevel = Level,
            Met_Level = Level,
            TID16 = sav.TID16,
            SID16 = sav.SID16,
            OT_Name = sav.OT,
            Ball = 4,
            Language = sav.Language,
        };

        var nick = Nickname;
        if (!string.IsNullOrWhiteSpace(nick))
        {
            pk.Nickname = nick;
            pk.IsNicknamed = true;
        }
        else
        {
            pk.Nickname = SpeciesName.GetSpeciesNameGeneration(Species, sav.Language, 7);
        }

        pk.IV_DEF = pk.IV_SPD = (IV_DEF * 2) + 1;
        pk.IV_ATK = pk.IV_SPA = (IV_ATK * 2) + 1;
        pk.IV_HP = (IV_HP * 2) + 1;
        pk.IV_SPE = Util.Rand.Next(32);

        var pi = pk.PersonalInfo;
        pk.Gender = criteria.GetGender(Gender, pi);
        pk.Nature = (int)criteria.GetNature(Nature.Random);
        pk.RefreshAbility(criteria.GetAbilityFromNumber(Ability));

        bool isShiny = pk.IsShiny;
        if (IsShiny && !isShiny) // Force Square
        {
            var low = pk.PID & 0xFFFF;
            pk.PID = ((low ^ sav.TID16 ^ sav.SID16) << 16) | low;
        }
        else if (isShiny)
        {
            pk.PID ^= 0x1000_0000;
        }

        Span<ushort> moves = stackalloc ushort[4];
        MoveLevelUp.GetEncounterMoves(moves, pk, pk.CurrentLevel, GameVersion.GO);
        pk.SetMoves(moves);
        pk.SetMaximumPPCurrent(moves);
        pk.OT_Friendship = pk.PersonalInfo.BaseFriendship;

        pk.HeightScalar = HeightScalar;
        pk.WeightScalar = WeightScalar;

        pk.AwakeningSetAllTo(InitialAV); // 2
        pk.ResetCalculatedValues();

        return pk;
    }
}
