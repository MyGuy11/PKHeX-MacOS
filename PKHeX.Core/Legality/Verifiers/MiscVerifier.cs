using System;
using System.Collections.Generic;
using System.Text;
using static PKHeX.Core.LegalityCheckStrings;
using static PKHeX.Core.CheckIdentifier;

namespace PKHeX.Core;

/// <summary>
/// Verifies miscellaneous data including <see cref="IFatefulEncounter.FatefulEncounter"/> and minor values.
/// </summary>
public sealed class MiscVerifier : Verifier
{
    protected override CheckIdentifier Identifier => Misc;

    public override void Verify(LegalityAnalysis data)
    {
        var pk = data.Entity;
        if (pk.IsEgg)
        {
            VerifyMiscEggCommon(data);

            if (pk is IContestStatsReadOnly s && s.HasContestStats())
                data.AddLine(GetInvalid(LEggContest, Egg));

            switch (pk)
            {
                case PK5 pk5 when pk5.PokeStarFame != 0 && pk5.IsEgg:
                    data.AddLine(GetInvalid(LEggShinyPokeStar, Egg));
                    break;
                case PK4 pk4 when pk4.ShinyLeaf != 0:
                    data.AddLine(GetInvalid(LEggShinyLeaf, Egg));
                    break;
                case PK4 pk4 when pk4.PokeathlonStat != 0:
                    data.AddLine(GetInvalid(LEggPokeathlon, Egg));
                    break;
                case PK3 when pk.Language != 1:  // All Eggs are Japanese and flagged specially for localized string
                    data.AddLine(GetInvalid(string.Format(LOTLanguage, LanguageID.Japanese, (LanguageID)pk.Language), Egg));
                    break;
            }

            if (pk is IHomeTrack {Tracker: not 0})
                data.AddLine(GetInvalid(LTransferTrackerShouldBeZero));
        }
        else
        {
            VerifyMiscMovePP(data);
        }

        switch (pk)
        {
            case PK7 {ResortEventStatus: >= ResortEventState.MAX}:
                data.AddLine(GetInvalid(LTransferBad));
                break;
            case PB7 pb7:
                VerifyBelugaStats(data, pb7);
                break;
            case PK8 pk8:
                VerifySWSHStats(data, pk8);
                break;
            case PB8 pb8:
                VerifyBDSPStats(data, pb8);
                break;
            case PA8 pa8:
                VerifyPLAStats(data, pa8);
                break;
            case PK9 pk9:
                VerifySVStats(data, pk9);
                break;
        }

        if (pk.Format >= 6)
            VerifyFullness(data, pk);

        var enc = data.EncounterMatch;
        if (enc is IEncounterServerDate { IsDateRestricted: true } serverGift)
        {
            var date = new DateOnly(pk.Met_Year + 2000, pk.Met_Month, pk.Met_Day);

            // HOME Gifts for Sinnoh/Hisui starters were forced JPN until May 20, 2022 (UTC).
            if (enc is WB8 { CardID: 9015 or 9016 or 9017 } or WA8 { CardID: 9018 or 9019 or 9020 })
            {
                if (date < new DateOnly(2022, 5, 20) && pk.Language != (int)LanguageID.Japanese)
                    data.AddLine(GetInvalid(LDateOutsideDistributionWindow));
            }

            var result = serverGift.IsValidDate(date);
            if (result == EncounterServerDateCheck.Invalid)
                data.AddLine(GetInvalid(LDateOutsideDistributionWindow));
        }
        else if (enc is IOverworldCorrelation8 z)
        {
            var match = z.IsOverworldCorrelationCorrect(pk);
            var req = z.GetRequirement(pk);
            if (match)
            {
                var seed = Overworld8RNG.GetOriginalSeed(pk);
                data.Info.PIDIV = new PIDIV(PIDType.Overworld8, seed);
            }

            bool valid = req switch
            {
                OverworldCorrelation8Requirement.MustHave => match,
                OverworldCorrelation8Requirement.MustNotHave => !match,
                _ => true,
            };

            if (!valid)
                data.AddLine(GetInvalid(LPIDTypeMismatch));
        }
        else if (enc is IStaticCorrelation8b s8b)
        {
            var match = s8b.IsStaticCorrelationCorrect(pk);
            var req = s8b.GetRequirement(pk);
            if (match)
                data.Info.PIDIV = new PIDIV(PIDType.Roaming8b, pk.EncryptionConstant);

            bool valid = req switch
            {
                StaticCorrelation8bRequirement.MustHave => match,
                StaticCorrelation8bRequirement.MustNotHave => !match,
                _ => true,
            };

            if (!valid)
                data.AddLine(GetInvalid(LPIDTypeMismatch));
        }
        else if (enc is IMasteryInitialMoveShop8 m)
        {
            if (!m.IsForcedMasteryCorrect(pk))
                data.AddLine(GetInvalid(LEncMasteryInitial));
        }

        VerifyMiscFatefulEncounter(data);
        VerifyMiscPokerus(data);
    }

    private void VerifySVStats(LegalityAnalysis data, PK9 pk9)
    {
        VerifyStatNature(data, pk9);

        if (!pk9.IsBattleVersionValid(data.Info.EvoChainsAllGens))
            data.AddLine(GetInvalid(LStatBattleVersionInvalid));

        var enc = data.EncounterOriginal;
        if (CheckHeightWeightOdds(enc) && pk9 is { HeightScalar: 0, WeightScalar: 0 } && ParseSettings.ZeroHeightWeight != Severity.Valid)
            data.AddLine(Get(LStatInvalidHeightWeight, ParseSettings.ZeroHeightWeight, Encounter));

        if (enc is EncounterEgg g && UnreleasedSV.Contains(g.Species | g.Form << 11))
            data.AddLine(GetInvalid(LTransferBad));

        if (!IsObedienceLevelValid(pk9, pk9.Obedience_Level, pk9.Met_Level))
            data.AddLine(GetInvalid(LTransferObedienceLevel));

        if (pk9.Tracker != 0)
            data.AddLine(GetInvalid(LTransferTrackerShouldBeZero));

        if (enc is EncounterEgg && !Tera9RNG.IsMatchTeraTypePersonalEgg(enc.Species, enc.Form, (byte)pk9.TeraTypeOriginal))
            data.AddLine(GetInvalid(LTeraTypeMismatch));
        if (pk9.IsEgg && pk9.TeraTypeOverride != (MoveType)TeraTypeUtil.OverrideNone)
            data.AddLine(GetInvalid(LTeraTypeIncorrect));

        if (enc is ITeraRaid9)
        {
            var seed = Tera9RNG.GetOriginalSeed(pk9);
            data.Info.PIDIV = new PIDIV(PIDType.Tera9, seed);
        }

        VerifyTechRecordSV(data, pk9);
    }

    private static bool IsObedienceLevelValid(PKM pk, byte current, int expectObey)
    {
        if (current > pk.CurrentLevel)
            return false;
        if (!pk.IsUntraded)
            return current >= expectObey;
        return current == expectObey;
    }

    private static readonly HashSet<int> UnreleasedSV = new()
    {
        (int)Species.Diglett | (1 << 11), // Diglett-1
        (int)Species.Meowth | (1 << 11), // Meowth-1
        (int)Species.Growlithe | (1 << 11), // Growlithe-1
        (int)Species.Slowpoke | (1 << 11), // Slowpoke-1
        (int)Species.Grimer | (1 << 11), // Grimer-1
        (int)Species.Voltorb | (1 << 11), // Voltorb-1
        (int)Species.Tauros, // Tauros-0
        (int)Species.Cyndaquil, // Cyndaquil
        (int)Species.Qwilfish | (1 << 11), // Qwilfish-1
        (int)Species.Sneasel | (1 << 11), // Sneasel-1
        (int)Species.Basculin | (2 << 11), // Basculin-2
        (int)Species.Chespin, // Chespin
        (int)Species.Fennekin, // Fennekin
        (int)Species.Carbink, // Carbink
        (int)Species.Grookey, // Grookey
        (int)Species.Sobble, // Sobble

        // Silly workaround for evolution chain reversal not being iteratively implemented -- block cross-gen evolution cases
        (int)Species.Raichu | (1 << 11), // Raichu-1
        (int)Species.Typhlosion | (1 << 11), // Typhlosion-1
        (int)Species.Samurott | (1 << 11), // Samurott-1
        (int)Species.Lilligant | (1 << 11), // Lilligant-1
        (int)Species.Braviary | (1 << 11), // Braviary-1
        (int)Species.Sliggoo | (1 << 11), // Sliggoo-1
        (int)Species.Avalugg | (1 << 11), // Avalugg-1
        (int)Species.Decidueye | (1 << 11), // Decidueye-1
        (int)Species.Basculegion, // Basculegion
        (int)Species.Basculegion | (1 << 11), // Basculegion-1

        (int)Species.Wyrdeer,
        (int)Species.Kleavor,
        (int)Species.Ursaluna,
        (int)Species.Overqwil,
    };

    private void VerifyMiscPokerus(LegalityAnalysis data)
    {
        var pk = data.Entity;
        if (pk.Format == 1)
            return;

        var strain = pk.PKRS_Strain;
        var days = pk.PKRS_Days;
        bool strainValid = Pokerus.IsStrainValid(pk, strain, days);
        if (!strainValid)
            data.AddLine(GetInvalid(string.Format(LPokerusStrainUnobtainable_0, strain)));

        bool daysValid = Pokerus.IsDurationValid(strain, days, out var max);
        if (!daysValid)
            data.AddLine(GetInvalid(string.Format(LPokerusDaysTooHigh_0, max)));
    }

    public void VerifyMiscG1(LegalityAnalysis data)
    {
        var pk = data.Entity;
        if (pk.IsEgg)
            VerifyMiscEggCommon(data);

        if (pk is not PK1 pk1)
        {
            if (pk is ICaughtData2 { CaughtData: not 0 } t)
            {
                var time = t.Met_TimeOfDay;
                bool valid = data.EncounterOriginal is EncounterTrade2 ? time == 0 : time is 1 or 2 or 3;
                if (!valid)
                    data.AddLine(new CheckResult(Severity.Invalid, LMetDetailTimeOfDay, Encounter));
            }
            return;
        }

        VerifyMiscG1Types(data, pk1);
        VerifyMiscG1CatchRate(data, pk1);
    }

    private void VerifyMiscG1Types(LegalityAnalysis data, PK1 pk1)
    {
        var Type_A = pk1.Type1;
        var Type_B = pk1.Type2;
        var species = pk1.Species;
        if (species == (int)Species.Porygon)
        {
            // Can have any type combination of any species by using Conversion.
            if (!GBRestrictions.TypeIDExists(Type_A))
            {
                data.AddLine(GetInvalid(LG1TypePorygonFail1));
            }
            if (!GBRestrictions.TypeIDExists(Type_B))
            {
                data.AddLine(GetInvalid(LG1TypePorygonFail2));
            }
            else // Both types exist, ensure a Gen1 species has this combination
            {
                var TypesAB_Match = PersonalTable.RB.IsValidTypeCombination(Type_A, Type_B);
                var result = TypesAB_Match ? GetValid(LG1TypeMatchPorygon) : GetInvalid(LG1TypePorygonFail);
                data.AddLine(result);
            }
        }
        else // Types must match species types
        {
            var pi = PersonalTable.RB[species];
            var Type_A_Match = Type_A == pi.Type1;
            var Type_B_Match = Type_B == pi.Type2;

            var first = Type_A_Match ? GetValid(LG1TypeMatch1) : GetInvalid(LG1Type1Fail);
            var second = Type_B_Match || (ParseSettings.AllowGBCartEra && ((species is (int)Species.Magnemite or (int)Species.Magneton) && Type_B == 9)) // Steel Magnemite via Stadium2
                ? GetValid(LG1TypeMatch2) : GetInvalid(LG1Type2Fail);
            data.AddLine(first);
            data.AddLine(second);
        }
    }

    private void VerifyMiscG1CatchRate(LegalityAnalysis data, PK1 pk1)
    {
        var catch_rate = pk1.Catch_Rate;
        var tradeback = GBRestrictions.IsTimeCapsuleTransferred(pk1, data.Info.Moves, data.EncounterMatch);
        var result = tradeback is TimeCapsuleEvaluation.NotTransferred or TimeCapsuleEvaluation.BadCatchRate
            ? GetWasNotTradeback(tradeback)
            : GetWasTradeback(tradeback);
        data.AddLine(result);

        CheckResult GetWasTradeback(TimeCapsuleEvaluation timeCapsuleEvalution)
        {
            if (PK1.IsCatchRateHeldItem(catch_rate))
                return GetValid(LG1CatchRateMatchTradeback);
            if (timeCapsuleEvalution == TimeCapsuleEvaluation.BadCatchRate)
                return GetInvalid(LG1CatchRateItem);

            return GetWasNotTradeback(timeCapsuleEvalution);
        }

        CheckResult GetWasNotTradeback(TimeCapsuleEvaluation timeCapsuleEvalution)
        {
            if (Array.Exists(data.Info.Moves, z => z.Generation == 2))
                return GetInvalid(LG1CatchRateItem);
            var e = data.EncounterMatch;
            if (e is EncounterStatic1E {Version: GameVersion.Stadium} or EncounterTrade1)
                return GetValid(LG1CatchRateMatchPrevious); // Encounters detected by the catch rate, cant be invalid if match this encounters

            ushort species = pk1.Species;
            if (GBRestrictions.Species_NotAvailable_CatchRate.Contains((byte)species) && catch_rate == PersonalTable.RB[species].CatchRate)
            {
                if (species != (int) Species.Dragonite || catch_rate != 45 || !e.Version.Contains(GameVersion.YW))
                    return GetInvalid(LG1CatchRateEvo);
            }
            if (!GBRestrictions.RateMatchesEncounter(e.Species, e.Version, catch_rate))
                return GetInvalid(timeCapsuleEvalution == TimeCapsuleEvaluation.Transferred12 ? LG1CatchRateChain : LG1CatchRateNone);
            return GetValid(LG1CatchRateMatchPrevious);
        }
    }

    private static void VerifyMiscFatefulEncounter(LegalityAnalysis data)
    {
        var pk = data.Entity;
        var enc = data.EncounterMatch;
        switch (enc)
        {
            case WC3 {FatefulEncounter: true} w:
                if (w.IsEgg)
                {
                    // Eggs hatched in RS clear the obedience flag!
                    // Hatching in Gen3 doesn't change the origin version.
                    if (pk.Format != 3)
                        return; // possible hatched in either game, don't bother checking
                    if (pk.Met_Location <= 087) // hatched in RS or Emerald
                        return; // possible hatched in either game, don't bother checking
                    // else, ensure fateful is active (via below)
                }
                VerifyFatefulIngameActive(data);
                VerifyWC3Shiny(data, w);
                return;
            case WC3 w:
                if (w.Version == GameVersion.XD)
                    return; // Can have either state
                VerifyWC3Shiny(data, w);
                break;
            case MysteryGift g: // WC3 handled above
                VerifyReceivability(data, g);
                VerifyFatefulMysteryGift(data, g);
                return;
            case IFatefulEncounterReadOnly {FatefulEncounter: true}: // ingame fateful
                VerifyFatefulIngameActive(data);
                return;
        }
        if (pk.FatefulEncounter)
            data.AddLine(GetInvalid(LFatefulInvalid, Fateful));
    }

    private static void VerifyMiscMovePP(LegalityAnalysis data)
    {
        var pk = data.Entity;

        if (!Legal.IsPPUpAvailable(pk)) // No PP Ups for format
        {
            if (pk.Move1_PPUps is not 0)
                data.AddLine(GetInvalid(string.Format(LMovePPUpsTooHigh_0, 1), CurrentMove));
            if (pk.Move2_PPUps is not 0)
                data.AddLine(GetInvalid(string.Format(LMovePPUpsTooHigh_0, 2), CurrentMove));
            if (pk.Move3_PPUps is not 0)
                data.AddLine(GetInvalid(string.Format(LMovePPUpsTooHigh_0, 3), CurrentMove));
            if (pk.Move4_PPUps is not 0)
                data.AddLine(GetInvalid(string.Format(LMovePPUpsTooHigh_0, 4), CurrentMove));
        }
        else // Check specific move indexes
        {
            if (!Legal.IsPPUpAvailable(pk.Move1) && pk.Move1_PPUps is not 0)
                data.AddLine(GetInvalid(string.Format(LMovePPUpsTooHigh_0, 1), CurrentMove));
            if (!Legal.IsPPUpAvailable(pk.Move2) && pk.Move2_PPUps is not 0)
                data.AddLine(GetInvalid(string.Format(LMovePPUpsTooHigh_0, 2), CurrentMove));
            if (!Legal.IsPPUpAvailable(pk.Move3) && pk.Move3_PPUps is not 0)
                data.AddLine(GetInvalid(string.Format(LMovePPUpsTooHigh_0, 3), CurrentMove));
            if (!Legal.IsPPUpAvailable(pk.Move4) && pk.Move4_PPUps is not 0)
                data.AddLine(GetInvalid(string.Format(LMovePPUpsTooHigh_0, 4), CurrentMove));
        }

        if (pk.Move1_PP > pk.GetMovePP(pk.Move1, pk.Move1_PPUps))
            data.AddLine(GetInvalid(string.Format(LMovePPTooHigh_0, 1), CurrentMove));
        if (pk.Move2_PP > pk.GetMovePP(pk.Move2, pk.Move2_PPUps))
            data.AddLine(GetInvalid(string.Format(LMovePPTooHigh_0, 2), CurrentMove));
        if (pk.Move3_PP > pk.GetMovePP(pk.Move3, pk.Move3_PPUps))
            data.AddLine(GetInvalid(string.Format(LMovePPTooHigh_0, 3), CurrentMove));
        if (pk.Move4_PP > pk.GetMovePP(pk.Move4, pk.Move4_PPUps))
            data.AddLine(GetInvalid(string.Format(LMovePPTooHigh_0, 4), CurrentMove));
    }

    private static void VerifyMiscEggCommon(LegalityAnalysis data)
    {
        var pk = data.Entity;
        if (pk.Move1_PPUps > 0 || pk.Move2_PPUps > 0 || pk.Move3_PPUps > 0 || pk.Move4_PPUps > 0)
            data.AddLine(GetInvalid(LEggPPUp, Egg));
        if (!IsZeroMovePP(pk))
            data.AddLine(GetInvalid(LEggPP, Egg));

        var enc = data.EncounterMatch;
        if (!EggStateLegality.GetIsEggHatchCyclesValid(pk, enc))
            data.AddLine(GetInvalid(LEggHatchCycles, Egg));

        if (pk.Format >= 6 && enc is EncounterEgg && !MovesMatchRelearn(pk))
        {
            const int moveCount = 4;
            var sb = new StringBuilder(64);
            for (int i = 0; i < moveCount; i++)
            {
                var move = pk.GetRelearnMove(i);
                var name = ParseSettings.GetMoveName(move);
                sb.Append(name);
                if (i != moveCount - 1)
                    sb.Append(", ");
            }
            var msg = string.Format(LMoveFExpect_0, sb);
            data.AddLine(GetInvalid(msg, Egg));
        }

        if (pk is ITechRecord record)
        {
            if (record.GetMoveRecordFlagAny())
                data.AddLine(GetInvalid(LEggRelearnFlags, Egg));
            if (pk.StatNature != pk.Nature)
                data.AddLine(GetInvalid(LEggNature, Egg));
        }
    }

    private static bool IsZeroMovePP(PKM pk)
    {
        if (pk.Move1_PP != pk.GetMovePP(pk.Move1, 0))
            return false;
        if (pk.Move2_PP != pk.GetMovePP(pk.Move2, 0))
            return false;
        if (pk.Move3_PP != pk.GetMovePP(pk.Move3, 0))
            return false;
        if (pk.Move4_PP != pk.GetMovePP(pk.Move4, 0))
            return false;
        return true;
    }

    private static bool MovesMatchRelearn(PKM pk)
    {
        if (pk.Move1 != pk.RelearnMove1)
            return false;
        if (pk.Move2 != pk.RelearnMove2)
            return false;
        if (pk.Move3 != pk.RelearnMove3)
            return false;
        if (pk.Move4 != pk.RelearnMove4)
            return false;
        return true;
    }

    private static void VerifyFatefulMysteryGift(LegalityAnalysis data, MysteryGift g)
    {
        var pk = data.Entity;
        if (g is PGF {IsShiny: true})
        {
            var Info = data.Info;
            Info.PIDIV = MethodFinder.Analyze(pk);
            if (Info.PIDIV.Type != PIDType.G5MGShiny && pk.Egg_Location != Locations.LinkTrade5)
                data.AddLine(GetInvalid(LPIDTypeMismatch, PID));
        }

        bool shouldHave = g.FatefulEncounter;
        var result = pk.FatefulEncounter == shouldHave
            ? GetValid(LFatefulMystery, Fateful)
            : GetInvalid(LFatefulMysteryMissing, Fateful);
        data.AddLine(result);
    }

    private static void VerifyReceivability(LegalityAnalysis data, MysteryGift g)
    {
        var pk = data.Entity;
        switch (g)
        {
            case WC6 wc6 when !wc6.CanBeReceivedByVersion(pk.Version) && !pk.WasTradedEgg:
            case WC7 wc7 when !wc7.CanBeReceivedByVersion(pk.Version) && !pk.WasTradedEgg:
            case WC8 wc8 when !wc8.CanBeReceivedByVersion(pk.Version):
            case WB8 wb8 when !wb8.CanBeReceivedByVersion(pk.Version, pk):
            case WA8 wa8 when !wa8.CanBeReceivedByVersion(pk.Version, pk):
                data.AddLine(GetInvalid(LEncGiftVersionNotDistributed, GameOrigin));
                return;
            case WC6 wc6 when wc6.RestrictLanguage != 0 && pk.Language != wc6.RestrictLanguage:
                data.AddLine(GetInvalid(string.Format(LOTLanguage, wc6.RestrictLanguage, pk.Language), CheckIdentifier.Language));
                return;
            case WC7 wc7 when wc7.RestrictLanguage != 0 && pk.Language != wc7.RestrictLanguage:
                data.AddLine(GetInvalid(string.Format(LOTLanguage, wc7.RestrictLanguage, pk.Language), CheckIdentifier.Language));
                return;
        }
    }

    private static void VerifyWC3Shiny(LegalityAnalysis data, WC3 g3)
    {
        // check for shiny locked gifts
        if (!g3.Shiny.IsValid(data.Entity))
            data.AddLine(GetInvalid(LEncGiftShinyMismatch, Fateful));
    }

    private static void VerifyFatefulIngameActive(LegalityAnalysis data)
    {
        var pk = data.Entity;
        var result = pk.FatefulEncounter
            ? GetValid(LFateful, Fateful)
            : GetInvalid(LFatefulMissing, Fateful);
        data.AddLine(result);
    }

    public void VerifyVersionEvolution(LegalityAnalysis data)
    {
        var pk = data.Entity;
        if (pk.Format < 7 || data.EncounterMatch.Species == pk.Species)
            return;

        // No point using the evolution tree. Just handle certain species.
        switch (pk.Species)
        {
            case (int)Species.Lycanroc when pk.Format == 7 && ((pk.Form == 0 && Moon()) || (pk.Form == 1 && Sun())):
            case (int)Species.Solgaleo when Moon():
            case (int)Species.Lunala when Sun():
                bool Sun() => (pk.Version & 1) == 0;
                bool Moon() => (pk.Version & 1) == 1;
                if (pk.IsUntraded)
                    data.AddLine(GetInvalid(LEvoTradeRequired, Evolution));
                break;
        }
    }

    private static void VerifyFullness(LegalityAnalysis data, PKM pk)
    {
        if (pk.IsEgg)
        {
            if (pk.Fullness != 0)
                data.AddLine(GetInvalid(string.Format(LMemoryStatFullness, "0"), Encounter));
            if (pk.Enjoyment != 0)
                data.AddLine(GetInvalid(string.Format(LMemoryStatEnjoyment, "0"), Encounter));
            return;
        }

        if (pk.Format >= 8)
        {
            if (pk.Fullness > 245) // Exiting camp is -10
                data.AddLine(GetInvalid(string.Format(LMemoryStatFullness, "<=245"), Encounter));
            else if (pk.Fullness is not 0 && pk is not PK8)
                data.AddLine(GetInvalid(string.Format(LMemoryStatFullness, "0"), Encounter));

            if (pk.Enjoyment != 0)
                data.AddLine(GetInvalid(string.Format(LMemoryStatEnjoyment, "0"), Encounter));
            return;
        }

        if (pk.Format != 6 || !pk.IsUntraded || pk.XY)
            return;

        // OR/AS PK6
        if (pk.Fullness == 0)
            return;
        if (pk.Species != data.EncounterMatch.Species)
            return; // evolved

        if (Unfeedable.Contains(pk.Species))
            data.AddLine(GetInvalid(string.Format(LMemoryStatFullness, "0"), Encounter));
    }

    private static readonly HashSet<ushort> Unfeedable = new()
    {
        (int)Species.Metapod,
        (int)Species.Kakuna,
        (int)Species.Pineco,
        (int)Species.Silcoon,
        (int)Species.Cascoon,
        (int)Species.Shedinja,
        (int)Species.Spewpa,
    };

    private static void VerifyBelugaStats(LegalityAnalysis data, PB7 pb7)
    {
        VerifyAbsoluteSizes(data, pb7);
        if (pb7.Stat_CP != pb7.CalcCP && !IsStarterLGPE(pb7))
            data.AddLine(GetInvalid(LStatIncorrectCP, Encounter));
    }

    private static void VerifyAbsoluteSizes(LegalityAnalysis data, IScaledSizeValue obj)
    {
        // ReSharper disable once CompareOfFloatsByEqualityOperator -- THESE MUST MATCH EXACTLY
        if (obj.HeightAbsolute != obj.CalcHeightAbsolute)
            data.AddLine(GetInvalid(LStatIncorrectHeight, Encounter));
        // ReSharper disable once CompareOfFloatsByEqualityOperator -- THESE MUST MATCH EXACTLY
        if (obj.WeightAbsolute != obj.CalcWeightAbsolute)
            data.AddLine(GetInvalid(LStatIncorrectWeight, Encounter));
    }

    private static bool IsStarterLGPE(ISpeciesForm pk) => pk.Species switch
    {
        (int)Species.Pikachu when pk.Form == 8 => true,
        (int)Species.Eevee   when pk.Form == 1 => true,
        _ => false,
    };

    private void VerifySWSHStats(LegalityAnalysis data, PK8 pk8)
    {
        var social = pk8.Sociability;
        if (pk8.IsEgg)
        {
            if (social != 0)
                data.AddLine(GetInvalid(LMemorySocialZero, Encounter));
        }
        else if (social > byte.MaxValue)
        {
            data.AddLine(GetInvalid(string.Format(LMemorySocialTooHigh_0, byte.MaxValue), Encounter));
        }

        VerifyStatNature(data, pk8);

        if (!pk8.IsBattleVersionValid(data.Info.EvoChainsAllGens))
            data.AddLine(GetInvalid(LStatBattleVersionInvalid));

        var enc = data.EncounterMatch;
        bool originGMax = enc is IGigantamaxReadOnly {CanGigantamax: true};
        if (originGMax != pk8.CanGigantamax)
        {
            bool ok = !pk8.IsEgg && pk8.CanToggleGigantamax(pk8.Species, pk8.Form, enc.Species, enc.Form);
            var chk = ok ? GetValid(LStatGigantamaxValid) : GetInvalid(LStatGigantamaxInvalid);
            data.AddLine(chk);
        }

        if (pk8.DynamaxLevel != 0)
        {
            if (!pk8.CanHaveDynamaxLevel(pk8) || pk8.DynamaxLevel > 10)
                data.AddLine(GetInvalid(LStatDynamaxInvalid));
        }

        if (CheckHeightWeightOdds(data.EncounterMatch) && pk8 is { HeightScalar: 0, WeightScalar: 0 } && ParseSettings.ZeroHeightWeight != Severity.Valid)
            data.AddLine(Get(LStatInvalidHeightWeight, ParseSettings.ZeroHeightWeight, Encounter));

        VerifyTechRecordSWSH(data, pk8);
    }

    private void VerifyPLAStats(LegalityAnalysis data, PA8 pa8)
    {
        VerifyAbsoluteSizes(data, pa8);
        if (!data.Info.EvoChainsAllGens.HasVisitedSWSH)
        {
            var affix = pa8.AffixedRibbon;
            if (affix != -1) // None
                data.AddLine(GetInvalid(string.Format(LRibbonMarkingAffixedF_0, affix)));
        }

        var social = pa8.Sociability;
        if (social != 0)
            data.AddLine(GetInvalid(LMemorySocialZero, Encounter));

        VerifyStatNature(data, pa8);

        if (!pa8.IsBattleVersionValid(data.Info.EvoChainsAllGens))
            data.AddLine(GetInvalid(LStatBattleVersionInvalid));

        if (pa8.CanGigantamax)
            data.AddLine(GetInvalid(LStatGigantamaxInvalid));

        if (pa8.DynamaxLevel != 0)
            data.AddLine(GetInvalid(LStatDynamaxInvalid));

        if (pa8.GetMoveRecordFlagAny() && !pa8.IsEgg) // already checked for eggs
            data.AddLine(GetInvalid(LEggRelearnFlags));

        if (CheckHeightWeightOdds(data.EncounterMatch) && pa8 is { HeightScalar: 0, WeightScalar: 0 } && ParseSettings.ZeroHeightWeight != Severity.Valid)
            data.AddLine(Get(LStatInvalidHeightWeight, ParseSettings.ZeroHeightWeight, Encounter));

        VerifyTechRecordSWSH(data, pa8);
    }

    private void VerifyBDSPStats(LegalityAnalysis data, PB8 pb8)
    {
        if (!data.Info.EvoChainsAllGens.HasVisitedSWSH)
        {
            var affix = pb8.AffixedRibbon;
            if (affix != -1) // None
                data.AddLine(GetInvalid(string.Format(LRibbonMarkingAffixedF_0, affix)));
        }

        var social = pb8.Sociability;
        if (social != 0)
            data.AddLine(GetInvalid(LMemorySocialZero, Encounter));

        if (pb8.IsDprIllegal)
            data.AddLine(GetInvalid(LTransferFlagIllegal));
        if (pb8.Species is (int)Species.Spinda or (int)Species.Nincada && !pb8.BDSP)
            data.AddLine(GetInvalid(LTransferNotPossible));
        if (pb8.Species is (int)Species.Spinda && pb8.Tracker != 0)
            data.AddLine(GetInvalid(LTransferTrackerShouldBeZero));

        VerifyStatNature(data, pb8);

        if (!pb8.IsBattleVersionValid(data.Info.EvoChainsAllGens))
            data.AddLine(GetInvalid(LStatBattleVersionInvalid));

        if (pb8.CanGigantamax)
            data.AddLine(GetInvalid(LStatGigantamaxInvalid));

        if (pb8.DynamaxLevel != 0)
            data.AddLine(GetInvalid(LStatDynamaxInvalid));

        if (pb8.GetMoveRecordFlagAny() && !pb8.IsEgg) // already checked for eggs
            data.AddLine(GetInvalid(LEggRelearnFlags));

        if (CheckHeightWeightOdds(data.EncounterMatch) && pb8 is { HeightScalar: 0, WeightScalar: 0 } && ParseSettings.ZeroHeightWeight != Severity.Valid)
            data.AddLine(Get(LStatInvalidHeightWeight, ParseSettings.ZeroHeightWeight, Encounter));

        VerifyTechRecordSWSH(data, pb8);
    }

    private static bool CheckHeightWeightOdds(IEncounterTemplate enc)
    {
        if (enc.Generation < 8)
            return false;
        if (enc is WC8 { IsHOMEGift: true })
            return false;
        return true;
    }

    private void VerifyStatNature(LegalityAnalysis data, PKM pk)
    {
        var sn = pk.StatNature;
        if (sn == pk.Nature)
            return;
        // Only allow Serious nature (12); disallow all other neutral natures.
        if (sn != 12 && (sn > 24 || sn % 6 == 0))
            data.AddLine(GetInvalid(LStatNatureInvalid));
    }

    private void VerifyTechRecordSWSH<T>(LegalityAnalysis data, T pk) where T : PKM, ITechRecord
    {
        string GetMoveName(int index) => ParseSettings.MoveStrings[pk.Permit.RecordPermitIndexes[index]];
        var evos = data.Info.EvoChainsAllGens.Gen8;
        if (evos.Length == 0)
        {
            var count = pk.Permit.RecordCountUsed;
            for (int i = 0; i < count; i++)
            {
                if (!pk.GetMoveRecordFlag(i))
                    continue;
                data.AddLine(GetInvalid(string.Format(LMoveSourceTR, GetMoveName(i))));
            }
        }
        else
        {
            static PersonalInfo8SWSH GetPersonal(EvoCriteria evo) => PersonalTable.SWSH.GetFormEntry(evo.Species, evo.Form);
            PersonalInfo8SWSH? pi = null;
            var count = pk.Permit.RecordCountUsed;
            for (int i = 0; i < count; i++)
            {
                if (!pk.GetMoveRecordFlag(i))
                    continue;
                if ((pi ??= GetPersonal(evos[0])).GetIsLearnTR(i))
                    continue;

                // Calyrex-0 can have TR flags for Calyrex-1/2 after it has force unlearned them.
                // Re-fusing can be reacquire the move via relearner, rather than needing another TR.
                // Calyrex-0 cannot reacquire the move via relearner, even though the TR is checked off in the TR list.
                if (pk.Species == (int)Species.Calyrex)
                {
                    var form = pk.Form;
                    // Check if another alt form can learn the TR
                    if ((form != 1 && CanLearnTR((int)Species.Calyrex, 1, i)) || (form != 2 && CanLearnTR((int)Species.Calyrex, 2, i)))
                        continue;
                }

                data.AddLine(GetInvalid(string.Format(LMoveSourceTR, GetMoveName(i))));
            }
        }
    }

    private static bool CanLearnTR(ushort species, byte form, int tr)
    {
        var pi = PersonalTable.SWSH.GetFormEntry(species, form);
        return pi.GetIsLearnTR(tr);
    }

    private void VerifyTechRecordSV(LegalityAnalysis data, PK9 pk)
    {
        string GetMoveName(int index) => ParseSettings.MoveStrings[pk.Permit.RecordPermitIndexes[index]];
        var evos = data.Info.EvoChainsAllGens.Gen9;
        if (evos.Length == 0)
        {
            int count = pk.Permit.RecordCountUsed;
            for (int i = 0; i < count; i++)
            {
                if (!pk.GetMoveRecordFlag(i))
                    continue;
                data.AddLine(GetInvalid(string.Format(LMoveSourceTR, GetMoveName(i))));
            }
        }
        else
        {
            static PersonalInfo9SV GetPersonal(EvoCriteria evo) => PersonalTable.SV.GetFormEntry(evo.Species, evo.Form);
            PersonalInfo9SV? pi = null;
            int count = pk.Permit.RecordCountUsed;
            for (int i = 0; i < count; i++)
            {
                if (!pk.GetMoveRecordFlag(i))
                    continue;
                if ((pi ??= GetPersonal(evos[0])).GetIsLearnTM(i))
                    continue;

                // Zoroark-0 cannot learn Encore via TM, but the pre-evolution Zorua-0 can via TM.
                // Double check if any pre-evolutions can learn the TM.
                bool preEvoHas = false;
                for (int p = 1; p < evos.Length; p++)
                {
                    if (!GetPersonal(evos[p]).GetIsLearnTM(i))
                        continue;
                    preEvoHas = true;
                    break;
                }
                if (!preEvoHas)
                    data.AddLine(GetInvalid(string.Format(LMoveSourceTR, GetMoveName(i))));
            }
        }
    }
}
