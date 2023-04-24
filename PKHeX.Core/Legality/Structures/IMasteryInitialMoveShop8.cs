using System;

namespace PKHeX.Core;

/// <summary>
/// Exposes permissions about the Move Shop
/// </summary>
public interface IMasteryInitialMoveShop8
{
    (Learnset Learn, Learnset Mastery) GetLevelUpInfo();
    void LoadInitialMoveset(PA8 pa8, Span<ushort> moves, Learnset learn, int level);
    bool IsForcedMasteryCorrect(PKM pk);
    void SetInitialMastery(PKM pk)
    {
        if (pk is PA8 pa8)
        {
            Span<ushort> moves = stackalloc ushort[4];
            var level = pa8.Met_Level;
            var (learn, mastery) = GetLevelUpInfo();
            LoadInitialMoveset(pa8, moves, learn, level);
            pa8.SetEncounterMasteryFlags(moves, mastery, level);
        }
    }
}
