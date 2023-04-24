using static PKHeX.Core.LegalityCheckStrings;
using static PKHeX.Core.RibbonIndex;

namespace PKHeX.Core;

/// <summary>
/// Verifies the <see cref="RibbonIndex"/> values for markings.
/// </summary>
public sealed class MarkVerifier : Verifier
{
    protected override CheckIdentifier Identifier => CheckIdentifier.RibbonMark;

    public override void Verify(LegalityAnalysis data)
    {
        var pk = data.Entity;
        if (pk is not IRibbonIndex m)
            return;

        if (!MarkRules.IsEncounterMarkAllowed(data)) // Shedinja doesn't copy Ribbons or Marks
            VerifyNoMarksPresent(data, m);
        else
            VerifyMarksPresent(data, m);

        VerifyAffixedRibbonMark(data, m);

        // temp logic to catch this case; in the future we will need more robust checks for encounters
        if (data.EncounterMatch is WC9 { RibbonMarkCharismatic: true} && pk is IRibbonSetMark8 { RibbonMarkCharismatic: false})
            data.AddLine(GetInvalid(string.Format(LRibbonMarkingFInvalid_0, GetRibbonNameSafe(MarkCharismatic))));
    }

    private void VerifyNoMarksPresent(LegalityAnalysis data, IRibbonIndex m)
    {
        for (var x = MarkLunchtime; x <= MarkSlump; x++)
        {
            if (m.GetRibbon((int)x))
                data.AddLine(GetInvalid(string.Format(LRibbonMarkingFInvalid_0, GetRibbonNameSafe(x))));
        }
    }

    private void VerifyMarksPresent(LegalityAnalysis data, IRibbonIndex m)
    {
        bool hasOne = false;
        for (var mark = MarkLunchtime; mark <= MarkSlump; mark++)
        {
            bool has = m.GetRibbon((int)mark);
            if (!has)
                continue;

            if (hasOne)
            {
                data.AddLine(GetInvalid(string.Format(LRibbonMarkingFInvalid_0, GetRibbonNameSafe(mark))));
                return;
            }

            bool result = MarkRules.IsEncounterMarkValid(mark, data.Entity, data.EncounterMatch);
            if (!result)
            {
                data.AddLine(GetInvalid(string.Format(LRibbonMarkingFInvalid_0, GetRibbonNameSafe(mark))));
                return;
            }

            hasOne = true;
        }
    }

    private static string GetRibbonNameSafe(RibbonIndex index)
    {
        if (index >= MAX_COUNT)
            return index.ToString();
        var expect = $"Ribbon{index}";
        return RibbonStrings.GetName(expect);
    }

    private void VerifyAffixedRibbonMark(LegalityAnalysis data, IRibbonIndex m)
    {
        if (m is not IRibbonSetAffixed a)
            return;

        var affixValue = a.AffixedRibbon;
        if (affixValue == -1) // None
            return;

        var affix = (RibbonIndex)affixValue;
        var max = MarkRules.GetMaxAffixValue(data.Entity.Format);
        if (affix > max)
        {
            data.AddLine(GetInvalid(string.Format(LRibbonMarkingAffixedF_0, GetRibbonNameSafe(affix))));
            return;
        }

        if (m is not PKM pk)
            return;

        if (MarkRules.IsEncounterMarkLost(data))
        {
            VerifyShedinjaAffixed(data, affix, pk, m);
            return;
        }
        EnsureHasRibbon(data, m, affix);
    }

    private void VerifyShedinjaAffixed(LegalityAnalysis data, RibbonIndex affix, PKM pk, IRibbonIndex r)
    {
        // Does not copy ribbons or marks, but retains the Affixed Ribbon value.
        // Try re-verifying to see if it could have had the Ribbon/Mark.

        var enc = data.EncounterOriginal;
        if (affix.IsEncounterMark8())
        {
            if (!MarkRules.IsEncounterMarkValid(affix, pk, enc))
                data.AddLine(GetInvalid(string.Format(LRibbonMarkingAffixedF_0, GetRibbonNameSafe(affix))));
            return;
        }

        if (enc.Generation <= 4 && (pk.Ball != (int)Ball.Poke || IsMoveSetEvolvedShedinja(pk)))
        {
            // Evolved in a prior generation.
            EnsureHasRibbon(data, r, affix);
            return;
        }

        var clone = pk.Clone();
        clone.Species = (int) Species.Nincada;
        var args = new RibbonVerifierArguments(clone, enc, data.Info.EvoChainsAllGens);
        affix.Fix(args, true);
        var name = GetRibbonNameSafe(affix);
        bool invalid = RibbonVerifier.IsValidExtra(affix, args);
        var severity = invalid ? Severity.Invalid : Severity.Fishy;
        data.AddLine(Get(string.Format(LRibbonMarkingAffixedF_0, name), severity));
    }

    private static bool IsMoveSetEvolvedShedinja(PKM pk)
    {
        // Check for gen3/4 exclusive moves that are Ninjask glitch only.
        if (pk.HasMove((int) Move.Screech))
            return true;
        if (pk.HasMove((int) Move.SwordsDance))
            return true;
        if (pk.HasMove((int) Move.Slash))
            return true;
        if (pk.HasMove((int) Move.BatonPass))
            return true;
        return pk.HasMove((int)Move.Agility) && pk is PK8 pk8 && !pk8.GetMoveRecordFlag(12); // TR12 (Agility)
    }

    private void EnsureHasRibbon(LegalityAnalysis data, IRibbonIndex m, RibbonIndex affix)
    {
        var hasRibbon = m.GetRibbonIndex(affix);
        if (!hasRibbon)
            data.AddLine(GetInvalid(string.Format(LRibbonMarkingAffixedF_0, GetRibbonNameSafe(affix))));
    }
}
