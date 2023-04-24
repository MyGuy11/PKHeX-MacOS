using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class TeamIndexes9 : SaveBlock<SAV9SV>, ITeamIndexSet
{
    private const int TeamCount = 6;
    private const int NONE_SELECTED = -1;
    public readonly int[] TeamSlots = new int[TeamCount * 6];

    public TeamIndexes9(SAV9SV sav, SCBlock block) : base(sav, block.Data) { }

    public void LoadBattleTeams()
    {
        if (!SAV.State.Exportable)
        {
            ClearBattleTeams();
            return;
        }

        for (int i = 0; i < TeamCount * 6; i++)
        {
            short val = ReadInt16LittleEndian(Data.AsSpan(i * 2));
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
    }

    public void UnlockAllTeams()
    {
        for (int i = 0; i < TeamCount; i++)
            SetIsTeamLocked(i, false);
    }

    public void SaveBattleTeams()
    {
        var span = Data.AsSpan();
        for (int i = 0; i < TeamCount * 6; i++)
        {
            int index = TeamSlots[i];
            if (index < 0)
            {
                WriteInt16LittleEndian(span[(i * 2)..], (short)index);
                continue;
            }

            SAV.GetBoxSlotFromIndex(index, out var box, out var slot);
            index = (box << 8) | slot;
            WriteInt16LittleEndian(span[(i * 2)..], (short)index);
        }
    }

    public bool GetIsTeamLocked(int team) => true;
    public void SetIsTeamLocked(int team, bool value) { }
}
