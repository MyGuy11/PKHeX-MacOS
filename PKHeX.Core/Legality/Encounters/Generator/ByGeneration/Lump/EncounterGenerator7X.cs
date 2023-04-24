using System.Collections.Generic;

namespace PKHeX.Core;

public sealed class EncounterGenerator7X : IEncounterGenerator
{
    public static readonly EncounterGenerator7X Instance = new();

    public IEnumerable<IEncounterable> GetPossible(PKM pk, EvoCriteria[] chain, GameVersion game, EncounterTypeGroup groups) => pk.Version switch
    {
        (int)GameVersion.GO => EncounterGenerator7GO.Instance.GetPossible(pk, chain, game, groups),
        > (int)GameVersion.GO => EncounterGenerator7GG.Instance.GetPossible(pk, chain, game, groups),
        _ => EncounterGenerator7.Instance.GetPossible(pk, chain, game, groups),
    };

    public IEnumerable<IEncounterable> GetEncounters(PKM pk, LegalInfo info)
    {
        var chain = EncounterOrigin.GetOriginChain(pk);
        return GetEncounters(pk, chain, info);
    }

    public IEnumerable<IEncounterable> GetEncounters(PKM pk, EvoCriteria[] chain, LegalInfo info) => pk.Version switch
    {
        (int)GameVersion.GO => EncounterGenerator7GO.Instance.GetEncounters(pk, chain, info),
        > (int)GameVersion.GO => EncounterGenerator7GG.Instance.GetEncounters(pk, chain, info),
        _ => EncounterGenerator7.Instance.GetEncounters(pk, chain, info),
    };
}
