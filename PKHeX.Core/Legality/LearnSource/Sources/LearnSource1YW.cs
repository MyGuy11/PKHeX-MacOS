using System;
using System.Diagnostics.CodeAnalysis;
using static PKHeX.Core.LearnMethod;
using static PKHeX.Core.LearnEnvironment;
using static PKHeX.Core.LearnSource1;

namespace PKHeX.Core;

/// <summary>
/// Exposes information about how moves are learned in <see cref="YW"/>.
/// </summary>
public sealed class LearnSource1YW : ILearnSource<PersonalInfo1>
{
    public static readonly LearnSource1YW Instance = new();
    private static readonly PersonalTable1 Personal = PersonalTable.Y;
    private static readonly Learnset[] Learnsets = Legal.LevelUpY;
    private const LearnEnvironment Game = YW;
    private const int MaxSpecies = Legal.MaxSpeciesID_1;

    public Learnset GetLearnset(ushort species, byte form) => Learnsets[species];

    public bool TryGetPersonal(ushort species, byte form, [NotNullWhen(true)] out PersonalInfo1? pi)
    {
        if (form is not 0 || species > MaxSpecies)
        {
            pi = null;
            return false;
        }
        pi = Personal[species];
        return true;
    }

    public MoveLearnInfo GetCanLearn(PKM pk, PersonalInfo1 pi, EvoCriteria evo, ushort move, MoveSourceType types = MoveSourceType.All, LearnOption option = LearnOption.Current)
    {
        if (move > Legal.MaxMoveID_1) // byte
            return default;

        if (types.HasFlag(MoveSourceType.Machine) && GetIsTM(pi, (byte)move))
            return new(TMHM, Game);

        if (types.HasFlag(MoveSourceType.SpecialTutor) && GetIsTutor(evo.Species, (byte)move))
            return new(Tutor, Game);

        if (types.HasFlag(MoveSourceType.LevelUp))
        {
            var learn = Learnsets[evo.Species];
            var level = learn.GetLevelLearnMove(move);
            if (level != -1 && evo.LevelMin <= level && level <= evo.LevelMax)
                return new(LevelUp, Game, (byte)level);
        }

        return default;
    }

    private static bool GetIsTutor(ushort species, byte move)
    {
        // No special tutors besides Stadium, which is GB-era only.
        if (!ParseSettings.AllowGBCartEra)
            return false;

        // Surf Pikachu via Stadium
        if (move != (int)Move.Surf)
            return false;
        return species is (int)Species.Pikachu or (int)Species.Raichu;
    }

    private static bool GetIsTM(PersonalInfo1 info, byte move)
    {
        var index = TMHM_RBY.IndexOf(move);
        if (index == -1)
            return false;
        return info.GetIsLearnTM(index);
    }

    public void GetAllMoves(Span<bool> result, PKM pk, EvoCriteria evo, MoveSourceType types = MoveSourceType.All)
    {
        if (!TryGetPersonal(evo.Species, evo.Form, out var pi))
            return;

        if (types.HasFlag(MoveSourceType.LevelUp))
        {
            var learn = GetLearnset(evo.Species, evo.Form);
            var min = ParseSettings.AllowGen1Tradeback && ParseSettings.AllowGen2MoveReminder(pk) ? 1 : evo.LevelMin;
            (bool hasMoves, int start, int end) = learn.GetMoveRange(evo.LevelMax, min);
            if (hasMoves)
            {
                var moves = learn.Moves;
                for (int i = end; i >= start; i--)
                    result[moves[i]] = true;
            }
        }

        if (types.HasFlag(MoveSourceType.Machine))
            pi.SetAllLearnTM(result, TMHM_RBY);

        if (types.HasFlag(MoveSourceType.SpecialTutor))
        {
            if (GetIsTutor(evo.Species, (int)Move.Surf))
                result[(int)Move.Surf] = true;
        }
    }

    public void GetEncounterMoves(IEncounterTemplate enc, Span<ushort> init)
    {
        var species = enc.Species;
        if (!TryGetPersonal(species, 0, out var personal))
            return;

        var learn = Learnsets[species];
        personal.GetMoves(init);
        var start = (4 - init.Count((ushort)0)) & 3;
        learn.SetEncounterMoves(enc.LevelMin, init, start);
    }
}
