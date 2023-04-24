using System;
using System.Collections.Generic;

namespace PKHeX.Core;

public sealed class EncounterGeneratorGO : IEncounterGenerator
{
    public static readonly EncounterGeneratorGO Instance = new();

    public IEnumerable<IEncounterable> GetEncounters(PKM pk, EvoCriteria[] chain, LegalInfo info)
    {
        var loc = pk.Met_Location;
        if (loc == Locations.GO7)
            return EncounterGenerator7GO.Instance.GetEncounters(pk, chain, info);
        if (loc == Locations.GO8)
            return EncounterGenerator8GO.Instance.GetEncounters(pk, chain, info);
        return Array.Empty<IEncounterable>();
    }

    public IEnumerable<IEncounterable> GetPossible(PKM pk, EvoCriteria[] chain, GameVersion game, EncounterTypeGroup groups)
    {
        var lgpe = EncounterGenerator7GO.Instance.GetPossible(pk, chain, game, groups);
        foreach (var enc in lgpe)
            yield return enc;

        var home = EncounterGenerator8GO.Instance.GetPossible(pk, chain, game, groups);
        foreach (var enc in home)
            yield return enc;
    }
}
