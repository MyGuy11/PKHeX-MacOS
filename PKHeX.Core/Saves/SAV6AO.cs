using System;
using System.Collections.Generic;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Generation 6 <see cref="SaveFile"/> object for <see cref="GameVersion.ORAS"/>.
/// </summary>
/// <inheritdoc cref="SAV6" />
public sealed class SAV6AO : SAV6, ISaveBlock6AO, IMultiplayerSprite
{
    public SAV6AO(byte[] data) : base(data, SaveBlockAccessor6AO.BlockMetadataOffset)
    {
        Blocks = new SaveBlockAccessor6AO(this);
        Initialize();
    }

    public SAV6AO() : base(SaveUtil.SIZE_G6ORAS, SaveBlockAccessor6AO.BlockMetadataOffset)
    {
        Blocks = new SaveBlockAccessor6AO(this);
        Initialize();
        ClearBoxes();
    }

    public override PersonalTable6AO Personal => PersonalTable.AO;
    public override ReadOnlySpan<ushort> HeldItems => Legal.HeldItem_AO;
    public SaveBlockAccessor6AO Blocks { get; }
    protected override SAV6AO CloneInternal() => new((byte[])Data.Clone());
    public override ushort MaxMoveID => Legal.MaxMoveID_6_AO;
    public override int MaxItemID => Legal.MaxItemID_6_AO;
    public override int MaxAbilityID => Legal.MaxAbilityID_6_AO;

    protected override int EventWork => 0x14A00;
    protected override int EventFlag => EventWork + 0x2F0;

    private void Initialize()
    {
        PCLayout = 0x04400;
        BattleBoxOffset = 0x04A00;
        PSS = 0x05000;
        Party = 0x14200;
        PokeDex = 0x15000;
        HoF = 0x19E00;
        DaycareOffset = 0x1BC00;
        BerryField = 0x1C400;
        WondercardFlags = 0x1CC00;
        Box = 0x33000;
        JPEG = 0x67C00;

        WondercardData = WondercardFlags + 0x100;
    }

    /// <summary> Offset of the UnionPokemon block. </summary>
    public const int Fused = 0x16A00;
    /// <summary> Offset of the GtsData block. </summary>
    public const int GTS = 0x18200;
    /// <summary> Offset of the second daycare structure within the Daycare block. </summary>
    private const int Daycare2 = 0x1BC00 + 0x1F0;
    /// <summary> Offset of the Contest data block. </summary>
    public const int Contest = 0x23600;

    #region Blocks
    public override IReadOnlyList<BlockInfo> AllBlocks => Blocks.BlockInfo;
    public override MyItem Items => Blocks.Items;
    public override ItemInfo6 ItemInfo => Blocks.ItemInfo;
    public override GameTime6 GameTime => Blocks.GameTime;
    public override Situation6 Situation => Blocks.Situation;
    public override PlayTime6 Played => Blocks.Played;
    public override MyStatus6 Status => Blocks.Status;
    public override RecordBlock6 Records => Blocks.Records;
    public Puff6 Puff => Blocks.Puff;
    public OPower6 OPower => Blocks.OPower;
    public LinkBlock6 Link => Blocks.Link;
    public BoxLayout6 BoxLayout => Blocks.BoxLayout;
    public BattleBox6 BattleBox => Blocks.BattleBox;
    public MysteryBlock6 MysteryGift => Blocks.MysteryGift;
    public SuperTrainBlock SuperTrain => Blocks.SuperTrain;
    public MaisonBlock Maison => Blocks.Maison;
    public SubEventLog6 SUBE => Blocks.SUBE;
    public ConfigSave6 Config => Blocks.Config;
    public Encount6 Encount => Blocks.Encount;

    public Misc6AO Misc => Blocks.Misc;
    public Zukan6AO Zukan => Blocks.Zukan;
    public SecretBase6Block SecretBase => Blocks.SecretBase;
    #endregion

    public override GameVersion Version => Game switch
    {
        (int) GameVersion.AS => GameVersion.AS,
        (int) GameVersion.OR => GameVersion.OR,
        _ => GameVersion.Invalid,
    };

    public override bool GetCaught(ushort species) => Blocks.Zukan.GetCaught(species);
    public override bool GetSeen(ushort species) => Blocks.Zukan.GetSeen(species);
    public override void SetSeen(ushort species, bool seen) => Blocks.Zukan.SetSeen(species, seen);
    public override void SetCaught(ushort species, bool caught) => Blocks.Zukan.SetCaught(species, caught);
    protected override void SetDex(PKM pk) => Blocks.Zukan.SetDex(pk);

    public override uint Money { get => Blocks.Misc.Money; set => Blocks.Misc.Money = value; }
    public override int Vivillon { get => Blocks.Misc.Vivillon; set => Blocks.Misc.Vivillon = value; }
    public override int Badges { get => Blocks.Misc.Badges; set => Blocks.Misc.Badges = value; }
    public override int BP { get => Blocks.Misc.BP; set => Blocks.Misc.BP = value; }

    public int MultiplayerSpriteID
    {
        get => Blocks.Status.MultiplayerSpriteID_1;
        set => Blocks.Status.MultiplayerSpriteID_1 = Blocks.Status.MultiplayerSpriteID_2 = value;
    }

    // Daycare
    public override int DaycareSeedSize => 16;
    public override bool HasTwoDaycares => true;

    public override int GetDaycareSlotOffset(int loc, int slot)
    {
        int ofs = loc == 0 ? DaycareOffset : Daycare2;
        return ofs + 8 + (slot * (SIZE_STORED + 8));
    }

    public override uint? GetDaycareEXP(int loc, int slot)
    {
        int ofs = loc == 0 ? DaycareOffset : Daycare2;
        return ReadUInt32LittleEndian(Data.AsSpan(ofs + ((SIZE_STORED + 8) * slot) + 4));
    }

    public override bool? IsDaycareOccupied(int loc, int slot)
    {
        int ofs = loc == 0 ? DaycareOffset : Daycare2;
        return Data[ofs + ((SIZE_STORED + 8) * slot)] == 1;
    }

    public override string GetDaycareRNGSeed(int loc)
    {
        int ofs = loc == 0 ? DaycareOffset : Daycare2;
        return Util.GetHexStringFromBytes(Data.AsSpan(ofs + 0x1E8, DaycareSeedSize / 2));
    }

    public override bool? IsDaycareHasEgg(int loc)
    {
        int ofs = loc == 0 ? DaycareOffset : Daycare2;
        return Data[ofs + 0x1E0] == 1;
    }

    public override void SetDaycareEXP(int loc, int slot, uint EXP)
    {
        int ofs = loc == 0 ? DaycareOffset : Daycare2;
        WriteUInt32LittleEndian(Data.AsSpan(ofs + ((SIZE_STORED + 8) * slot) + 4), EXP);
    }

    public override void SetDaycareOccupied(int loc, int slot, bool occupied)
    {
        int ofs = loc == 0 ? DaycareOffset : Daycare2;
        Data[ofs + ((SIZE_STORED + 8) * slot)] = occupied ? (byte)1 : (byte)0;
    }

    public override void SetDaycareRNGSeed(int loc, string seed)
    {
        if (loc != 0)
            return;
        if (DaycareOffset < 0)
            return;
        if (seed.Length > DaycareSeedSize)
            return;

        Util.GetBytesFromHexString(seed).CopyTo(Data, DaycareOffset + 0x1E8);
    }

    public override void SetDaycareHasEgg(int loc, bool hasEgg)
    {
        int ofs = loc == 0 ? DaycareOffset : Daycare2;
        Data[ofs + 0x1E0] = hasEgg ? (byte)1 : (byte)0;
    }

    public override string JPEGTitle => !HasJPPEGData ? string.Empty : StringConverter6.GetString(Data.AsSpan(JPEG, 0x1A));
    public override byte[] GetJPEGData() => !HasJPPEGData ? Array.Empty<byte>() : Data.AsSpan(JPEG + 0x54, 0xE004).ToArray();
    private bool HasJPPEGData => Data[JPEG + 0x54] == 0xFF;

    protected override bool[] MysteryGiftReceivedFlags { get => Blocks.MysteryGift.GetReceivedFlags(); set => Blocks.MysteryGift.SetReceivedFlags(value); }
    protected override DataMysteryGift[] MysteryGiftCards { get => Blocks.MysteryGift.GetGifts(); set => Blocks.MysteryGift.SetGifts(value); }

    public override int CurrentBox { get => Blocks.BoxLayout.CurrentBox; set => Blocks.BoxLayout.CurrentBox = value; }
    protected override int GetBoxWallpaperOffset(int box) => Blocks.BoxLayout.GetBoxWallpaperOffset(box);
    public override int BoxesUnlocked { get => Blocks.BoxLayout.BoxesUnlocked; set => Blocks.BoxLayout.BoxesUnlocked = value; }
    public override byte[] BoxFlags { get => Blocks.BoxLayout.BoxFlags; set => Blocks.BoxLayout.BoxFlags = value; }

    public bool BattleBoxLocked
    {
        get => Blocks.BattleBox.Locked;
        set => Blocks.BattleBox.Locked = value;
    }
}
