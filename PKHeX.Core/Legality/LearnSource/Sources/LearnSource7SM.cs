using System;
using System.Diagnostics.CodeAnalysis;
using static PKHeX.Core.LearnMethod;
using static PKHeX.Core.LearnEnvironment;
using static PKHeX.Core.LearnSource7;

namespace PKHeX.Core;

/// <summary>
/// Exposes information about how moves are learned in <see cref="SM"/>.
/// </summary>
public sealed class LearnSource7SM : ILearnSource<PersonalInfo7>, IEggSource
{
    public static readonly LearnSource7SM Instance = new();
    private static readonly PersonalTable7 Personal = PersonalTable.SM;
    private static readonly Learnset[] Learnsets = Legal.LevelUpSM;
    private static readonly EggMoves7[] EggMoves = Legal.EggMovesSM;
    private const int MaxSpecies = Legal.MaxSpeciesID_7;
    private const LearnEnvironment Game = SM;
    private const int ReminderBonus = 100; // Move reminder allows re-learning ALL level up moves regardless of level.

    public Learnset GetLearnset(ushort species, byte form) => Learnsets[Personal.GetFormIndex(species, form)];

    public bool TryGetPersonal(ushort species, byte form, [NotNullWhen(true)] out PersonalInfo7? pi)
    {
        pi = null;
        if (species > MaxSpecies)
            return false;
        pi = Personal[species, form];
        return true;
    }

    public bool GetIsEggMove(ushort species, byte form, ushort move)
    {
        if (species > MaxSpecies)
            return false;
        var moves = MoveEgg.GetFormEggMoves(species, form, EggMoves).AsSpan();
        return moves.IndexOf(move) != -1;
    }

    public ReadOnlySpan<ushort> GetEggMoves(ushort species, byte form)
    {
        if (species > MaxSpecies)
            return ReadOnlySpan<ushort>.Empty;
        return MoveEgg.GetFormEggMoves(species, form, EggMoves);
    }

    public MoveLearnInfo GetCanLearn(PKM pk, PersonalInfo7 pi, EvoCriteria evo, ushort move, MoveSourceType types = MoveSourceType.All, LearnOption option = LearnOption.Current)
    {
        if (types.HasFlag(MoveSourceType.LevelUp))
        {
            var learn = GetLearnset(evo.Species, evo.Form);
            var level = learn.GetLevelLearnMove(move);
            if (level != -1) // Can relearn at any level!
                return new(LevelUp, Game, 1);
        }

        if (types.HasFlag(MoveSourceType.Machine) && pi.GetIsLearnTM(TMHM_SM.IndexOf(move)))
            return new(TMHM, Game);

        if (types.HasFlag(MoveSourceType.TypeTutor) && pi.GetIsLearnTutorType(LearnSource5.TypeTutor567.IndexOf(move)))
            return new(Tutor, Game);

        if (types.HasFlag(MoveSourceType.EnhancedTutor) && GetIsEnhancedTutor(evo, pk, move, option))
            return new(Tutor, Game);

        return default;
    }

    private static bool GetIsEnhancedTutor(EvoCriteria evo, ISpeciesForm current, ushort move, LearnOption option) => evo.Species switch
    {
        (int)Species.Pikachu or (int)Species.Raichu => move is (int)Move.VoltTackle,
        (int)Species.Necrozma => move switch
        {
            (int)Move.SunsteelStrike => (option == LearnOption.AtAnyTime || current.Form == 1), // Sun w/ Solgaleo
            (int)Move.MoongeistBeam  => (option == LearnOption.AtAnyTime || current.Form == 2), // Moon w/ Lunala
            _ => false,
        },
        (int)Species.Keldeo   => move is (int)Move.SecretSword,
        (int)Species.Meloetta => move is (int)Move.RelicSong,
        (int)Species.Rotom => move switch
        {
            (int)Move.Overheat  => option == LearnOption.AtAnyTime || current.Form == 1,
            (int)Move.HydroPump => option == LearnOption.AtAnyTime || current.Form == 2,
            (int)Move.Blizzard  => option == LearnOption.AtAnyTime || current.Form == 3,
            (int)Move.AirSlash  => option == LearnOption.AtAnyTime || current.Form == 4,
            (int)Move.LeafStorm => option == LearnOption.AtAnyTime || current.Form == 5,
            _ => false,
        },
        (int)Species.Zygarde => move is (int)Move.ExtremeSpeed or (int)Move.DragonDance or (int)Move.ThousandArrows or (int)Move.ThousandWaves or (int)Move.CoreEnforcer,
        _ => false,
    };

    public void GetAllMoves(Span<bool> result, PKM pk, EvoCriteria evo, MoveSourceType types = MoveSourceType.All)
    {
        if (!TryGetPersonal(evo.Species, evo.Form, out var pi))
            return;

        if (types.HasFlag(MoveSourceType.LevelUp))
        {
            var learn = GetLearnset(evo.Species, evo.Form);
            (bool hasMoves, int start, int end) = learn.GetMoveRange(ReminderBonus);
            if (hasMoves)
            {
                var moves = learn.Moves;
                for (int i = end; i >= start; i--)
                    result[moves[i]] = true;
            }
        }

        if (types.HasFlag(MoveSourceType.Machine))
            pi.SetAllLearnTM(result, TMHM_SM);

        if (types.HasFlag(MoveSourceType.TypeTutor))
            pi.SetAllLearnTutorType(result, LearnSource5.TypeTutor567);

        if (types.HasFlag(MoveSourceType.EnhancedTutor))
        {
            var species = evo.Species;
            if (species is (int)Species.Zygarde)
            {
                result[(int)Move.CoreEnforcer] = true;
                result[(int)Move.ExtremeSpeed] = true;
                result[(int)Move.DragonDance] = true;
                result[(int)Move.ThousandArrows] = true;
                result[(int)Move.ThousandWaves] = true;
            }

            if (species is (int)Species.Rotom && evo.Form is not 0)
                result[MoveTutor.GetRotomFormMove(evo.Form)] = true;
            else if (species is (int)Species.Pikachu or (int)Species.Raichu) // Gen7 only Volt Tackle tutor
                result[(int)Move.VoltTackle] = true;
            else if (species is (int)Species.Keldeo)
                result[(int)Move.SecretSword] = true;
            else if (species is (int)Species.Meloetta)
                result[(int)Move.RelicSong] = true;
            else if (species is (int)Species.Necrozma && pk.Form is 1) // Sun
                result[(int)Move.SunsteelStrike] = true;
            else if (species is (int)Species.Necrozma && pk.Form is 2) // Moon
                result[(int)Move.MoongeistBeam] = true;
        }
    }
}
