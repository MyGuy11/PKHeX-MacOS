using System;
using System.Buffers;

namespace PKHeX.Core;

/// <summary>
/// Logic for suggesting a moveset.
/// </summary>
public static class MoveListSuggest
{
    private static void GetSuggestedMoves(PKM pk, EvolutionHistory evoChains, MoveSourceType types, IEncounterTemplate enc, Span<ushort> moves)
    {
        if (pk is { IsEgg: true, Format: <= 5 }) // pre relearn
        {
            MoveList.GetCurrentMoves(pk, pk.Species, 0, (GameVersion)pk.Version, pk.CurrentLevel, moves);
            return;
        }

        if (types is not (MoveSourceType.None or MoveSourceType.Encounter))
        {
            GetValidMoves(pk, enc, evoChains, moves, types);
            return;
        }

        // try to give current moves
        if (enc.Generation <= 2)
        {
            var lvl = pk.Format >= 7 ? pk.Met_Level : pk.CurrentLevel;
            var ver = enc.Version;
            MoveLevelUp.GetEncounterMoves(moves, enc.Species, 0, lvl, ver);
            return;
        }

        if (pk.Species == enc.Species)
        {
            MoveLevelUp.GetEncounterMoves(moves, pk.Species, pk.Form, pk.CurrentLevel, (GameVersion)pk.Version);
            return;
        }

        GetValidMoves(pk, enc, evoChains, moves, types);
    }

    private static void GetValidMoves(PKM pk, IEncounterTemplate enc, EvolutionHistory evoChains, Span<ushort> moves, MoveSourceType types = MoveSourceType.ExternalSources)
    {
        var length = pk.MaxMoveID + 1;
        bool[] rent = ArrayPool<bool>.Shared.Rent(length);
        var span = rent.AsSpan(0, length);
        LearnPossible.Get(pk, enc, evoChains, span, types);

        var count = span[1..].Count(true);
        int remain = moves.Length;
        if (count <= remain)
            LoadAll(moves, span);
        else
            LoadSample(moves, span, count, remain);

        span.Clear();
        ArrayPool<bool>.Shared.Return(rent);
    }

    private static void LoadSample(Span<ushort> moves, ReadOnlySpan<bool> span, int count, int remain)
    {
        // Selection Sampling
        int ctr = 0;
        var rnd = Util.Rand;
        for (ushort i = 1; i < span.Length; i++)
        {
            if (!span[i])
                continue;
            if (rnd.Next(count--) >= remain)
                continue;
            moves[ctr++] = i;
            if (--remain == 0)
                break;
        }
    }

    private static void LoadAll(Span<ushort> moves, ReadOnlySpan<bool> span)
    {
        int ctr = 0;
        for (ushort i = 1; i < span.Length; i++)
        {
            if (!span[i])
                continue;
            moves[ctr++] = i;
        }
    }

    /// <summary>
    /// Gets four moves which can be learned depending on the input arguments.
    /// </summary>
    /// <param name="analysis">Parse information to generate a moveset for.</param>
    /// <param name="moves">Result storage</param>
    /// <param name="types">Allowed move sources for populating the result array</param>
    public static void GetSuggestedCurrentMoves(this LegalityAnalysis analysis, Span<ushort> moves, MoveSourceType types = MoveSourceType.All)
    {
        if (!analysis.Parsed)
            return;
        var pk = analysis.Entity;
        if (pk is { IsEgg: true, Format: >= 6 })
        {
            pk.GetRelearnMoves(moves);
            return;
        }

        if (pk.IsEgg)
            types = types.ClearNonEggSources();

        var info = analysis.Info;
        GetSuggestedMoves(pk, info.EvoChainsAllGens, types, info.EncounterOriginal, moves);
    }

    /// <summary>
    /// Gets the current <see cref="PKM.RelearnMoves"/> array of four moves that might be legal.
    /// </summary>
    /// <remarks>Use <see cref="GetSuggestedRelearnMovesFromEncounter"/> instead of calling directly; this method just puts default values in without considering the final moveset.</remarks>
    public static void GetSuggestedRelearn(this IEncounterTemplate enc, PKM pk, Span<ushort> moves)
    {
        if (LearnVerifierRelearn.ShouldNotHaveRelearnMoves(enc, pk))
            return;

        GetSuggestedRelearnInternal(enc, pk, moves);
    }

    // Invalid encounters won't be recognized as an EncounterEgg; check if it *should* be a bred egg.
    private static void GetSuggestedRelearnInternal(this IEncounterTemplate enc, PKM pk, Span<ushort> moves)
    {
        if (enc is IRelearn { Relearn: { HasMoves: true } r })
            r.CopyTo(moves);
        else if (enc is EncounterEgg or EncounterInvalid { EggEncounter: true })
            GetSuggestedRelearnEgg(enc, pk, moves);
    }

    private static void GetSuggestedRelearnEgg(IEncounterTemplate enc, PKM pk, Span<ushort> moves)
    {
        Span<ushort> current = stackalloc ushort[4];
        pk.GetRelearnMoves(current);
        Span<ushort> expected = stackalloc ushort[current.Length];
        _ = MoveBreed.GetExpectedMoves(current, enc, expected);
        expected.CopyTo(moves);
    }

    /// <summary>
    /// Gets the current <see cref="PKM.RelearnMoves"/> array of four moves that might be legal.
    /// </summary>
    public static void GetSuggestedRelearnMovesFromEncounter(this LegalityAnalysis analysis, Span<ushort> moves, IEncounterTemplate? enc = null)
    {
        var info = analysis.Info;
        enc ??= info.EncounterOriginal;
        var pk = analysis.Entity;

        if (LearnVerifierRelearn.ShouldNotHaveRelearnMoves(enc, pk))
            return;

        if (enc is EncounterEgg or EncounterInvalid {EggEncounter: true})
            enc.GetSuggestedRelearnEgg(info.Moves, pk, moves);
        else
            enc.GetSuggestedRelearnInternal(pk, moves);
    }

    private static void GetSuggestedRelearnEgg(this IEncounterTemplate enc, ReadOnlySpan<MoveResult> parse, PKM pk, Span<ushort> moves)
    {
        enc.GetEggRelearnMoves(parse, pk, moves);
        int generation = enc.Generation;
        if (generation <= 5) // gen2 does not have splitbreed, <=5 do not have relearn moves and shouldn't even be here.
            return;

        // Split-breed species like Budew & Roselia may be legal for one, and not the other.
        // If we're not a split-breed or are already legal, return.
        var split = Breeding.GetSplitBreedGeneration(generation);
        if (split?.Contains(enc.Species) != true)
            return;

        var tmp = pk.Clone();
        tmp.SetRelearnMoves(moves);
        var la = new LegalityAnalysis(tmp);
        var chk = la.Info.Moves;
        if (MoveResult.AllValid(chk))
            return;

        // Try again with the other split-breed species if possible.
        var generator = EncounterGenerator.GetGenerator(enc.Version);
        var tree = EvolutionTree.GetEvolutionTree(enc.Context);
        var chain = tree.GetValidPreEvolutions(pk, 100, skipChecks: true, stopSpecies: enc.Species);
        var other = generator.GetPossible(pk, chain, enc.Version, EncounterTypeGroup.Egg);
        foreach (var incense in other)
        {
            if (incense.Species == enc.Species)
                continue;
            incense.GetEggRelearnMoves(parse, pk, moves);
            break;
        }
    }

    private static void GetEggRelearnMoves(this IEncounterTemplate enc, ReadOnlySpan<MoveResult> parse, PKM pk, Span<ushort> moves)
    {
        // Extract a list of the moves that should end up in the relearn move list.
        LoadRelearnFlagged(moves, parse, pk);

        Span<ushort> expected = stackalloc ushort[moves.Length];
        _ = MoveBreed.GetExpectedMoves(moves, enc, expected);
        expected.CopyTo(moves);
    }

    private static void LoadRelearnFlagged(Span<ushort> moves, ReadOnlySpan<MoveResult> parse, PKM pk)
    {
        // Loads only indexes that are flagged as relearn moves
        int count = 0;
        for (int index = 0; index < parse.Length; index++)
        {
            var move = parse[index];
            if (move.ShouldBeInRelearnMoves())
                moves[count++] = pk.GetMove(index);
        }
        moves[count..].Clear();
    }
}
