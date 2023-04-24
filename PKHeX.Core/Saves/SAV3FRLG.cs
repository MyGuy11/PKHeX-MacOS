using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Generation 5 <see cref="SaveFile"/> object for <see cref="GameVersion.FRLG"/>.
/// </summary>
/// <inheritdoc cref="SAV3" />
public sealed class SAV3FRLG : SAV3, IGen3Joyful, IGen3Wonder
{
    // Configuration
    protected override SAV3FRLG CloneInternal() => new(Write());
    public override GameVersion Version { get; protected set; } = GameVersion.FR; // allow mutation
    private PersonalTable3 _personal = PersonalTable.FR;
    public override PersonalTable3 Personal => _personal;

    public override int EventFlagCount => 8 * 288;
    public override int EventWorkCount => 0x100;
    protected override int DaycareSlotSize => SIZE_STORED + 0x3C; // 0x38 mail + 4 exp
    public override int DaycareSeedSize => 4; // 16bit
    protected override int EggEventFlag => 0x266;
    protected override int BadgeFlagStart => 0x820;

    public SAV3FRLG(bool japanese = false) : base(japanese) => Initialize();

    public SAV3FRLG(byte[] data) : base(data)
    {
        Initialize();

        // Fix save files that have an overflow corruption with the Pokédex.
        // Future loads of this save file will cause it to be recognized as FR/LG correctly.
        if (IsCorruptPokedexFF())
            WriteUInt32LittleEndian(Small.AsSpan(0xAC), 1);
    }

    protected override int EventFlag => 0xEE0;
    protected override int EventWork => 0x1000;

    private void Initialize()
    {
        // small
        PokeDex = 0x18;

        // large
        DaycareOffset = 0x2F80;

        // storage
        Box = 0;
    }

    public bool ResetPersonal(GameVersion g)
    {
        if (g is not (GameVersion.FR or GameVersion.LG))
            return false;
        _personal = g == GameVersion.FR ? PersonalTable.FR : PersonalTable.LG;
        Version = g;
        return true;
    }

    #region Small
    public override bool NationalDex
    {
        get => PokedexNationalMagicFRLG == PokedexNationalUnlockFRLG;
        set
        {
            PokedexNationalMagicFRLG = value ? PokedexNationalUnlockFRLG : (byte)0; // magic
            SetEventFlag(0x840, value);
            SetWork(0x4E, PokedexNationalUnlockWorkFRLG);
        }
    }

    public ushort JoyfulJumpInRow           { get => ReadUInt16LittleEndian(Small.AsSpan(0xB00)); set => WriteUInt16LittleEndian(Small.AsSpan(0xB00), Math.Min((ushort)9999, value)); }
    // u16 field2;
    public ushort JoyfulJump5InRow          { get => ReadUInt16LittleEndian(Small.AsSpan(0xB04)); set => WriteUInt16LittleEndian(Small.AsSpan(0xB04), Math.Min((ushort)9999, value)); }
    public ushort JoyfulJumpGamesMaxPlayers { get => ReadUInt16LittleEndian(Small.AsSpan(0xB06)); set => WriteUInt16LittleEndian(Small.AsSpan(0xB06), Math.Min((ushort)9999, value)); }
    // u32 field8;
    public uint   JoyfulJumpScore           { get => ReadUInt16LittleEndian(Small.AsSpan(0xB0C)); set => WriteUInt32LittleEndian(Small.AsSpan(0xB0C), Math.Min(9999, value)); }

    public uint   JoyfulBerriesScore        { get => ReadUInt16LittleEndian(Small.AsSpan(0xB10)); set => WriteUInt32LittleEndian(Small.AsSpan(0xB10), Math.Min(9999, value)); }
    public ushort JoyfulBerriesInRow        { get => ReadUInt16LittleEndian(Small.AsSpan(0xB14)); set => WriteUInt16LittleEndian(Small.AsSpan(0xB14), Math.Min((ushort)9999, value)); }
    public ushort JoyfulBerries5InRow       { get => ReadUInt16LittleEndian(Small.AsSpan(0xB16)); set => WriteUInt16LittleEndian(Small.AsSpan(0xB16), Math.Min((ushort)9999, value)); }

    public uint BerryPowder
    {
        get => ReadUInt32LittleEndian(Small.AsSpan(0xAF8)) ^ SecurityKey;
        set => WriteUInt32LittleEndian(Small.AsSpan(0xAF8), value ^ SecurityKey);
    }

    public override uint SecurityKey
    {
        get => ReadUInt32LittleEndian(Small.AsSpan(0xF20));
        set => WriteUInt32LittleEndian(Small.AsSpan(0xF20), value);
    }
    #endregion

    #region Large
    public override int PartyCount { get => Large[0x034]; protected set => Large[0x034] = (byte)value; }
    public override int GetPartyOffset(int slot) => 0x038 + (SIZE_PARTY * slot);

    public override uint Money
    {
        get => ReadUInt32LittleEndian(Large.AsSpan(0x0290)) ^ SecurityKey;
        set => WriteUInt32LittleEndian(Large.AsSpan(0x0290), value ^ SecurityKey);
    }

    public override uint Coin
    {
        get => (ushort)(ReadUInt16LittleEndian(Large.AsSpan(0x0294)) ^ SecurityKey);
        set => WriteUInt16LittleEndian(Large.AsSpan(0x0294), (ushort)(value ^ SecurityKey));
    }

    private const int OFS_PCItem = 0x0298;
    private const int OFS_PouchHeldItem = 0x0310;
    private const int OFS_PouchKeyItem = 0x03B8;
    private const int OFS_PouchBalls = 0x0430;
    private const int OFS_PouchTMHM = 0x0464;
    private const int OFS_PouchBerry = 0x054C;

    protected override InventoryPouch3[] GetItems()
    {
        const int max = 999;
        var PCItems = ArrayUtil.ConcatAll(Legal.Pouch_Items_RS, Legal.Pouch_Key_FRLG, Legal.Pouch_Ball_RS, Legal.Pouch_TMHM_RS, Legal.Pouch_Berries_RS);
        return new InventoryPouch3[]
        {
            new(InventoryType.Items, Legal.Pouch_Items_RS, max, OFS_PouchHeldItem, (OFS_PouchKeyItem - OFS_PouchHeldItem) / 4),
            new(InventoryType.KeyItems, Legal.Pouch_Key_FRLG, 1, OFS_PouchKeyItem, (OFS_PouchBalls - OFS_PouchKeyItem) / 4),
            new(InventoryType.Balls, Legal.Pouch_Ball_RS, max, OFS_PouchBalls, (OFS_PouchTMHM - OFS_PouchBalls) / 4),
            new(InventoryType.TMHMs, Legal.Pouch_TMHM_RS, max, OFS_PouchTMHM, (OFS_PouchBerry - OFS_PouchTMHM) / 4),
            new(InventoryType.Berries, Legal.Pouch_Berries_RS, 999, OFS_PouchBerry, 43),
            new(InventoryType.PCItems, PCItems, 999, OFS_PCItem, (OFS_PouchHeldItem - OFS_PCItem) / 4),
        };
    }

    protected override int SeenOffset2 => 0x5F8;
    protected override int MailOffset => 0x2CD0;

    protected override int GetDaycareEXPOffset(int slot) => GetDaycareSlotOffset(0, slot + 1) - 4; // @ end of each pk slot
    public override string GetDaycareRNGSeed(int loc) => ReadUInt16LittleEndian(Large.AsSpan(GetDaycareEXPOffset(2))).ToString("X4"); // after the 2nd slot EXP, before the step counter
    public override void SetDaycareRNGSeed(int loc, string seed) => WriteUInt16LittleEndian(Large.AsSpan(GetDaycareEXPOffset(2)), (ushort)Util.GetHexValue(seed));

    protected override int ExternalEventData => 0x30A7;

    #region eBerry
    private const int OFFSET_EBERRY = 0x30EC;
    private const int SIZE_EBERRY = 0x134;

    public override byte[] GetEReaderBerry() => Large.Slice(OFFSET_EBERRY, SIZE_EBERRY);
    public override void SetEReaderBerry(ReadOnlySpan<byte> data) => data.CopyTo(Large.AsSpan(OFFSET_EBERRY));

    public override string EBerryName => GetString(Large.AsSpan(OFFSET_EBERRY, 7));
    public override bool IsEBerryEngima => Large[OFFSET_EBERRY] is 0 or 0xFF;
    #endregion

    #region eTrainer
    public override byte[] GetEReaderTrainer() => Small.Slice(0x4A0, 0xBC);
    public override void SetEReaderTrainer(ReadOnlySpan<byte> data) => data.CopyTo(Small.AsSpan(0x4A0));
    #endregion

    public int WonderOffset => WonderNewsOffset;
    private const int WonderNewsOffset = 0x3120;
    private int WonderCardOffset => WonderNewsOffset + (Japanese ? WonderNews3.SIZE_JAP : WonderNews3.SIZE);
    private int WonderCardExtraOffset => WonderCardOffset + (Japanese ? WonderCard3.SIZE_JAP : WonderCard3.SIZE);

    public WonderNews3 WonderNews { get => new(Large.Slice(WonderNewsOffset, Japanese ? WonderNews3.SIZE_JAP : WonderNews3.SIZE)); set => SetData(Large.AsSpan(WonderOffset), value.Data); }
    public WonderCard3 WonderCard { get => new(Large.Slice(WonderCardOffset, Japanese ? WonderCard3.SIZE_JAP : WonderCard3.SIZE)); set => SetData(Large.AsSpan(WonderCardOffset), value.Data); }
    public WonderCard3Extra WonderCardExtra { get => new(Large.Slice(WonderCardExtraOffset, WonderCard3Extra.SIZE)); set => SetData(Large.AsSpan(WonderCardExtraOffset), value.Data); }
    // 0x338: 4 easy chat words
    // 0x340: news MENewsJisanStruct
    // 0x344: uint[5], uint[5] tracking?

    public override Gen3MysteryData MysteryData
    {
        get => new MysteryEvent3(Large.Slice(0x361C, MysteryEvent3.SIZE));
        set => SetData(Large.AsSpan(0x361C), value.Data);
    }

    protected override int SeenOffset3 => 0x3A18;

    public string RivalName
    {
        get => GetString(Large.AsSpan(0x3A4C, 8));
        set => SetString(Large.AsSpan(0x3A4C, 8), value, 7, StringConverterOption.ClearZero);
    }

    #endregion
}
