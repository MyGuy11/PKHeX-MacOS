using System;
using System.Diagnostics.CodeAnalysis;
using static PKHeX.Core.LearnMethod;
using static PKHeX.Core.LearnEnvironment;
using static PKHeX.Core.LearnSource3;

namespace PKHeX.Core;

/// <summary>
/// Exposes information about how moves are learned in <see cref="LG"/>.
/// </summary>
public sealed class LearnSource3LG : ILearnSource<PersonalInfo3>, IEggSource
{
    public static readonly LearnSource3LG Instance = new();
    private static readonly PersonalTable3 Personal = PersonalTable.LG;
    private static readonly Learnset[] Learnsets = Legal.LevelUpLG;
    private static readonly EggMoves6[] EggMoves = Legal.EggMovesRS; // same for all Gen3 games
    private const int MaxSpecies = Legal.MaxSpeciesID_3;
    private const LearnEnvironment Game = LG;
    private const int Generation = 3;
    private const int CountTM = 50;

    public Learnset GetLearnset(ushort species, byte form) => Learnsets[species];
    internal PersonalInfo3 this[ushort species] => Personal[species];

    public bool TryGetPersonal(ushort species, byte form, [NotNullWhen(true)] out PersonalInfo3? pi)
    {
        pi = null;
        if (species > MaxSpecies)
            return false;
        pi = Personal[species];
        return true;
    }

    public bool GetIsEggMove(ushort species, byte form, ushort move)
    {
        if (species > MaxSpecies)
            return false;
        var moves = EggMoves[species];
        return moves.GetHasEggMove(move);
    }

    public ReadOnlySpan<ushort> GetEggMoves(ushort species, byte form)
    {
        if (species > MaxSpecies)
            return ReadOnlySpan<ushort>.Empty;
        return EggMoves[species].Moves;
    }

    public MoveLearnInfo GetCanLearn(PKM pk, PersonalInfo3 pi, EvoCriteria evo, ushort move, MoveSourceType types = MoveSourceType.All, LearnOption option = LearnOption.Current)
    {
        if (types.HasFlag(MoveSourceType.LevelUp))
        {
            var learn = GetLearnset(evo.Species, evo.Form);
            var level = learn.GetLevelLearnMove(move);
            if (level != -1 && level <= evo.LevelMax)
                return new(LevelUp, Game, (byte)level);
        }

        if (types.HasFlag(MoveSourceType.Machine))
        {
            if (GetIsTM(pi, move))
                return new(TMHM, Game);
            if (pk.Format == Generation && GetIsHM(pi, move))
                return new(TMHM, Game);
        }

        if (types.HasFlag(MoveSourceType.SpecialTutor) && GetIsTutor(evo.Species, move))
            return new(Tutor, Game);

        return default;
    }

    private static bool GetIsTutor(ushort species, ushort move) => move switch
    {
        (int)Move.BlastBurn => species == (int)Species.Charizard,
        (int)Move.HydroCannon => species == (int)Species.Blastoise,
        (int)Move.FrenzyPlant => species == (int)Species.Venusaur,
        _ => false,
    };

    private static bool GetIsTM(PersonalInfo3 info, ushort move)
    {
        var index = TM_3.IndexOf(move);
        if (index == -1)
            return false;
        return info.TMHM[index];
    }

    private static bool GetIsHM(PersonalInfo3 info, ushort move)
    {
        var index = HM_3.IndexOf(move);
        if (index == -1)
            return false;
        return info.TMHM[CountTM + index];
    }

    public void GetAllMoves(Span<bool> result, PKM pk, EvoCriteria evo, MoveSourceType types = MoveSourceType.All)
    {
        if (!TryGetPersonal(evo.Species, evo.Form, out var pi))
            return;

        if (types.HasFlag(MoveSourceType.LevelUp))
        {
            var learn = GetLearnset(evo.Species, evo.Form);
            (bool hasMoves, int start, int end) = learn.GetMoveRange(evo.LevelMax);
            if (hasMoves)
            {
                var moves = learn.Moves;
                for (int i = end; i >= start; i--)
                    result[moves[i]] = true;
            }
        }

        if (types.HasFlag(MoveSourceType.Machine))
        {
            var flags = pi.TMHM;
            var moves = TM_3;
            for (int i = 0; i < moves.Length; i++)
            {
                if (flags[i])
                    result[moves[i]] = true;
            }

            if (pk.Format == 3)
            {
                moves = HM_3;
                for (int i = 0; i < moves.Length; i++)
                {
                    if (flags[CountTM + i])
                        result[moves[i]] = true;
                }
            }
        }

        if (types.HasFlag(MoveSourceType.SpecialTutor))
        {
            if (evo.Species == (int)Species.Charizard)
                result[(int)Move.BlastBurn] = true;
            else if (evo.Species == (int)Species.Blastoise)
                result[(int)Move.HydroCannon] = true;
            else if (evo.Species == (int)Species.Venusaur)
                result[(int)Move.FrenzyPlant] = true;
        }
    }
}
