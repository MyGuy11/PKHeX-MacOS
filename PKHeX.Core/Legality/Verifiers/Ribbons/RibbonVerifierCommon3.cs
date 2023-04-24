using static PKHeX.Core.RibbonIndex;

namespace PKHeX.Core;

/// <summary>
/// Parsing logic for <see cref="IRibbonSetCommon3"/>.
/// </summary>
public static class RibbonVerifierCommon3
{
    public static void Parse(this IRibbonSetCommon3 r, RibbonVerifierArguments args, ref RibbonResultList list)
    {
        var evos = args.History;
        var pk = args.Entity;
        if (r.RibbonChampionG3 && !evos.HasVisitedGen3)
            list.Add(ChampionG3);
        if (r.RibbonArtist && !evos.HasVisitedGen3)
            list.Add(Artist);
        if (r.RibbonEffort && !RibbonRules.IsRibbonValidEffort(pk, evos, args.Encounter.Generation))
            list.Add(Effort);
    }

    public static void ParseEgg(this IRibbonSetCommon3 r, ref RibbonResultList list)
    {
        if (r.RibbonChampionG3)
            list.Add(ChampionG3);
        if (r.RibbonArtist)
            list.Add(Artist);
        if (r.RibbonEffort)
            list.Add(Effort);
    }
}
