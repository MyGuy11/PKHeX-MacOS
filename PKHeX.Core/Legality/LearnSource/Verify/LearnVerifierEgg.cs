using System;

namespace PKHeX.Core;

/// <summary>
/// Moveset verifier for entities currently existing as an egg.
/// </summary>
internal static class LearnVerifierEgg
{
    public static void Verify(Span<MoveResult> result, ReadOnlySpan<ushort> current, IEncounterTemplate enc, PKM pk)
    {
        if (enc.Generation >= 6)
            VerifyFromRelearn(result, current, enc, pk);
        else // No relearn moves available.
            VerifyPre3DS(result, current, enc);
    }

    private static void VerifyPre3DS(Span<MoveResult> result, ReadOnlySpan<ushort> current, IEncounterTemplate enc)
    {
        if (enc is EncounterEgg e)
            LearnVerifierRelearn.VerifyEggMoveset(e, result, current);
        else
            VerifyFromEncounter(result, current, enc);
    }

    private static void VerifyFromEncounter(Span<MoveResult> result, ReadOnlySpan<ushort> current, IEncounterTemplate enc)
    {
        if (enc is IMoveset { Moves: { HasMoves: true } x })
        {
            VerifyMovesInitial(result, current, x);
        }
        else
        {
            ReadOnlySpan<ushort> initial = GameData.GetLearnset(enc.Version, enc.Species, enc.Form).GetBaseEggMoves(enc.LevelMin);
            VerifyMovesInitial(result, current, initial);
        }
    }

    private static void VerifyMovesInitial(Span<MoveResult> result, ReadOnlySpan<ushort> current, Moveset initial)
    {
        // Check that the sequence of current move matches the initial move sequence.
        int i = 0;
        if (initial.Move1 != 0)
        {
            result[i] = GetMethodInitial(current[i], initial.Move1); i++;
            if (initial.Move2 != 0)
            {
                result[i] = GetMethodInitial(current[i], initial.Move2); i++;
                if (initial.Move3 != 0)
                {
                    result[i] = GetMethodInitial(current[i], initial.Move3); i++;
                    if (initial.Move4 != 0)
                    {
                        result[i] = GetMethodInitial(current[i], initial.Move4); i++;
                    }
                }
            }
        }
        for (; i < current.Length; i++)
            result[i] = current[i] == 0 ? MoveResult.Empty : MoveResult.Unobtainable(0);
    }

    private static void VerifyMovesInitial(Span<MoveResult> result, ReadOnlySpan<ushort> current, ReadOnlySpan<ushort> initial)
    {
        // Check that the sequence of current move matches the initial move sequence.
        for (int i = 0; i < initial.Length; i++)
            result[i] = GetMethodInitial(current[i], initial[i]);
        for (int i = initial.Length; i < current.Length; i++)
            result[i] = current[i] == 0 ? MoveResult.Empty : MoveResult.Unobtainable(0);
    }

    private static void VerifyFromRelearn(Span<MoveResult> result, ReadOnlySpan<ushort> current, IEncounterTemplate enc, PKM pk)
    {
        if (enc is EncounterEgg)
            VerifyMatchesRelearn(result, current, pk);
        else if (enc is IMoveset { Moves: { HasMoves: true } x })
            VerifyMovesInitial(result, current, x);
        else
            VerifyFromEncounter(result, current, enc);
    }

    private static void VerifyMatchesRelearn(Span<MoveResult> result, ReadOnlySpan<ushort> current, PKM pk)
    {
        // Check that the sequence of current move matches the relearn move sequence.
        for (int i = 0; i < result.Length; i++)
            result[i] = GetMethodRelearn(current[i], pk.GetRelearnMove(i));
    }

    private static MoveResult GetMethodInitial(ushort current, ushort initial)
    {
        if (current != initial)
            return MoveResult.Unobtainable(initial);
        if (current == 0)
            return MoveResult.Empty;
        return MoveResult.Initial;
    }

    private static MoveResult GetMethodRelearn(ushort current, ushort relearn)
    {
        if (current != relearn)
            return MoveResult.Unobtainable(relearn);
        if (current == 0)
            return MoveResult.Empty;
        return MoveResult.Relearn;
    }
}
