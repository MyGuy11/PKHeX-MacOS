using System;
using System.Collections.Generic;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Generation 3 <see cref="SaveFile"/> object for <see cref="GameVersion.RS"/>.
/// </summary>
/// <inheritdoc cref="SAV3" />
public sealed class SAV3RS : SAV3, IGen3Hoenn
{
    // Configuration
    protected override SAV3RS CloneInternal() => new(Write());
    public override GameVersion Version { get => GameVersion.RS; protected set { } }
    public override PersonalTable3 Personal => PersonalTable.RS;

    public override int EventFlagCount => 8 * 288;
    public override int EventWorkCount => 0x100;
    protected override int DaycareSlotSize => SIZE_STORED;
    public override int DaycareSeedSize => 4; // 16bit
    protected override int EggEventFlag => 0x86;
    protected override int BadgeFlagStart => 0x807;

    public SAV3RS(byte[] data) : base(data) => Initialize();
    public SAV3RS(bool japanese = false) : base(japanese) => Initialize();

    protected override int EventFlag => 0x1220;
    protected override int EventWork => 0x1340;

    private void Initialize()
    {
        // small
        PokeDex = 0x18;

        // large
        DaycareOffset = 0x2F9C;

        // storage
        Box = 0;
    }

    #region Small
    public override bool NationalDex
    {
        get => PokedexNationalMagicRSE == PokedexNationalUnlockRSE;
        set
        {
            PokedexMode = value ? (byte)1 : (byte)0; // mode
            PokedexNationalMagicRSE = value ? PokedexNationalUnlockRSE : (byte)0; // magic
            SetEventFlag(0x836, value);
            SetWork(0x46, PokedexNationalUnlockWorkRSE);
        }
    }

    public override uint SecurityKey { get => 0; set { } }

    public RTC3 ClockInitial
    {
        get => new(Small.AsSpan(0x98, RTC3.Size).ToArray());
        set => SetData(Small.AsSpan(0x98), value.Data);
    }

    public RTC3 ClockElapsed
    {
        get => new(Small.AsSpan(0xA0, RTC3.Size).ToArray());
        set => SetData(Small.AsSpan(0xA0), value.Data);
    }
    #endregion

    #region Large
    public override int PartyCount { get => Large[0x234]; protected set => Large[0x234] = (byte)value; }
    public override int GetPartyOffset(int slot) => 0x238 + (SIZE_PARTY * slot);

    public override uint Money
    {
        get => ReadUInt32LittleEndian(Large.AsSpan(0x0490));
        set => WriteUInt32LittleEndian(Large.AsSpan(0x0490), value);
    }

    public override uint Coin
    {
        get => ReadUInt16LittleEndian(Large.AsSpan(0x0494));
        set => WriteUInt16LittleEndian(Large.AsSpan(0x0494), (ushort)(value));
    }

    private const int OFS_PCItem = 0x0498;
    private const int OFS_PouchHeldItem = 0x0560;
    private const int OFS_PouchKeyItem = 0x05B0;
    private const int OFS_PouchBalls = 0x0600;
    private const int OFS_PouchTMHM = 0x0640;
    private const int OFS_PouchBerry = 0x0740;

    protected override InventoryPouch3[] GetItems()
    {
        const int max = 99;
        var PCItems = ArrayUtil.ConcatAll(Legal.Pouch_Items_RS, Legal.Pouch_Key_RS, Legal.Pouch_Ball_RS, Legal.Pouch_TMHM_RS, Legal.Pouch_Berries_RS);
        return new InventoryPouch3[]
        {
            new(InventoryType.Items, Legal.Pouch_Items_RS, max, OFS_PouchHeldItem, (OFS_PouchKeyItem - OFS_PouchHeldItem) / 4),
            new(InventoryType.KeyItems, Legal.Pouch_Key_RS, 1, OFS_PouchKeyItem, (OFS_PouchBalls - OFS_PouchKeyItem) / 4),
            new(InventoryType.Balls, Legal.Pouch_Ball_RS, max, OFS_PouchBalls, (OFS_PouchTMHM - OFS_PouchBalls) / 4),
            new(InventoryType.TMHMs, Legal.Pouch_TMHM_RS, max, OFS_PouchTMHM, (OFS_PouchBerry - OFS_PouchTMHM) / 4),
            new(InventoryType.Berries, Legal.Pouch_Berries_RS, 999, OFS_PouchBerry, 46),
            new(InventoryType.PCItems, PCItems, 999, OFS_PCItem, (OFS_PouchHeldItem - OFS_PCItem) / 4),
        };
    }

    public PokeBlock3Case PokeBlocks
    {
        get => new(Large, 0x7F8);
        set => SetData(Large.AsSpan(0x7F8), value.Write());
    }

    protected override int SeenOffset2 => 0x938;

    public DecorationInventory3 Decorations => new(Large.AsSpan(0x26A0, DecorationInventory3.SIZE));

    public Swarm3 Swarm
    {
        get => new(Large.Slice(0x2AFC, Swarm3.SIZE));
        set => SetData(Large.AsSpan(0x2AFC), value.Data);
    }

    private void ClearSwarm() => Large.AsSpan(0x2AFC, Swarm3.SIZE).Clear();

    public IReadOnlyList<Swarm3> DefaultSwarms => Swarm3Details.Swarms_RS;

    public int SwarmIndex
    {
        get => Array.FindIndex(Swarm3Details.Swarms_RS, z => z.MapNum == Swarm.MapNum);
        set
        {
            var arr = DefaultSwarms;
            if ((uint)value >= arr.Count)
                ClearSwarm();
            else
                Swarm = arr[value];
        }
    }

    protected override int MailOffset => 0x2B4C;

    protected override int GetDaycareEXPOffset(int slot) => GetDaycareSlotOffset(0, 2) + (2 * 0x38) + (4 * slot); // consecutive vals, after both consecutive slots & 2 mail
    public override string GetDaycareRNGSeed(int loc) => ReadUInt16LittleEndian(Large.AsSpan(GetDaycareEXPOffset(2))).ToString("X4");
    public override void SetDaycareRNGSeed(int loc, string seed) => WriteUInt16LittleEndian(Large.AsSpan(GetDaycareEXPOffset(2)), (ushort)Util.GetHexValue(seed));

    protected override int ExternalEventData => 0x311B;

    #region eBerry
    private const int OFFSET_EBERRY = 0x3160;
    private const int SIZE_EBERRY = 0x530;

    public override byte[] GetEReaderBerry() => Large.Slice(OFFSET_EBERRY, SIZE_EBERRY);
    public override void SetEReaderBerry(ReadOnlySpan<byte> data) => data.CopyTo(Large.AsSpan(OFFSET_EBERRY));

    public override string EBerryName => GetString(Large.AsSpan(OFFSET_EBERRY, 7));
    public override bool IsEBerryEngima => Large[OFFSET_EBERRY] is 0 or 0xFF;
    #endregion

    #region eTrainer
    public override byte[] GetEReaderTrainer() => Small.Slice(0x498, 0xBC);
    public override void SetEReaderTrainer(ReadOnlySpan<byte> data) => data.CopyTo(Small.AsSpan(0x498));
    #endregion

    public override Gen3MysteryData MysteryData
    {
        get => new MysteryEvent3RS(Large.Slice(0x3690, MysteryEvent3.SIZE));
        set => SetData(Large.AsSpan(0x3690), value.Data);
    }

    public RecordMixing3Gift RecordMixingGift { get => new(Large.Slice(0x3A7C, RecordMixing3Gift.SIZE)); set => SetData(Large.AsSpan(0x3A7C), value.Data); }

    protected override int SeenOffset3 => 0x3A8C;
    #endregion
}
