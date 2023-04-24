using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class BoxLayout7 : SaveBlock<SAV7>, IBoxDetailName, IBoxDetailWallpaper, ITeamIndexSet
{
    private const int BoxCount = 32;

    private const int BattleBoxFlags      = 0x4C4;
    private const int PCBackgrounds       = 0x5C0;
    private const int PCFlags             = 0x5E0;
    private const int Unlocked            = 0x5E1;
    private const int LastViewedBoxOffset = 0x5E3;

    private const int StringMaxLength = SAV6.LongStringLength / 2;

    private const int TeamCount = 6;
    private const int NONE_SELECTED = -1;
    public readonly int[] TeamSlots = new int[TeamCount * 6];

    public BoxLayout7(SAV7SM sav, int offset) : base(sav) => Offset = offset;
    public BoxLayout7(SAV7USUM sav, int offset) : base(sav) => Offset = offset;

    public int GetBoxWallpaperOffset(int box) => Offset + PCBackgrounds + box;

    public int GetBoxWallpaper(int box)
    {
        if ((uint)box > SAV.BoxCount)
            return 0;
        return Data[GetBoxWallpaperOffset(box)];
    }

    public void SetBoxWallpaper(int box, int value)
    {
        if ((uint)box > SAV.BoxCount)
            return;
        Data[GetBoxWallpaperOffset(box)] = (byte)value;
    }

    private Span<byte> GetBoxNameSpan(int box) => Data.AsSpan(GetBoxNameOffset(box), SAV6.LongStringLength);
    private int GetBoxNameOffset(int box) => Offset + (SAV6.LongStringLength * box);
    public string GetBoxName(int box) => SAV.GetString(GetBoxNameSpan(box));
    public void SetBoxName(int box, ReadOnlySpan<char> value) => SAV.SetString(GetBoxNameSpan(box), value, StringMaxLength, StringConverterOption.ClearZero);

    public byte[] BoxFlags
    {
        get => new[] { Data[Offset + PCFlags] }; // bits for wallpaper unlocks
        set
        {
            if (value.Length != 1)
                return;
            Data[Offset + PCFlags] = value[0];
        }
    }

    public int BoxesUnlocked
    {
        get => Data[Offset + Unlocked];
        set
        {
            if (value > BoxCount)
                value = BoxCount;
            Data[Offset + Unlocked] = (byte)value;
        }
    }

    public int CurrentBox { get => Data[Offset + LastViewedBoxOffset]; set => Data[Offset + LastViewedBoxOffset] = (byte)value; }

    public void LoadBattleTeams()
    {
        for (int i = 0; i < TeamCount * 6; i++)
        {
            short val = ReadInt16LittleEndian(Data.AsSpan(Offset + BattleBoxFlags + (i * 2)));
            if (val < 0)
            {
                TeamSlots[i] = NONE_SELECTED;
                continue;
            }

            int box = val >> 8;
            int slot = val & 0xFF;
            int index = (SAV.BoxSlotCount * box) + slot;
            TeamSlots[i] = index & 0xFFFF;
        }
    }

    public void ClearBattleTeams()
    {
        for (int i = 0; i < TeamSlots.Length; i++)
            TeamSlots[i] = NONE_SELECTED;
        for (int i = 0; i < TeamCount; i++)
            SetIsTeamLocked(i, false);
    }

    public void SaveBattleTeams()
    {
        var span = Data.AsSpan(Offset + BattleBoxFlags);
        for (int i = 0; i < TeamCount * 6; i++)
        {
            int index = TeamSlots[i];
            if (index < 0)
            {
                WriteInt16LittleEndian(span[(i*2)..], (short)index);
                continue;
            }

            SAV.GetBoxSlotFromIndex(index, out var box, out var slot);
            index = (box << 8) | slot;
            WriteInt16LittleEndian(span[(i * 2)..], (short)index);
        }
    }

    public void UnlockAllTeams()
    {
        for (int i = 0; i < TeamCount; i++)
            SetIsTeamLocked(i, false);
    }

    public bool GetIsTeamLocked(int team) => Data[Offset + PCBackgrounds - TeamCount - team] == 1;
    public void SetIsTeamLocked(int team, bool value) => Data[Offset + PCBackgrounds - TeamCount - team] = value ? (byte)1 : (byte)0;

    public string this[int i]
    {
        get => GetBoxName(i);
        set => SetBoxName(i, value);
    }
}
