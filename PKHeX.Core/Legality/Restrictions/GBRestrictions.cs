using System;
using System.Collections.Generic;

using static PKHeX.Core.Legal;
using static PKHeX.Core.GameVersion;
using static PKHeX.Core.Species;
using static PKHeX.Core.PotentialGBOrigin;
using static PKHeX.Core.TimeCapsuleEvaluation;

namespace PKHeX.Core;

/// <summary>
/// Miscellaneous GB Era restriction logic for legality checking
/// </summary>
internal static class GBRestrictions
{
    private static readonly HashSet<byte> Stadium_GiftSpecies = new()
    {
        (int)Bulbasaur,
        (int)Charmander,
        (int)Squirtle,
        (int)Psyduck,
        (int)Hitmonlee,
        (int)Hitmonchan,
        (int)Eevee,
        (int)Omanyte,
        (int)Kabuto,
    };

    /// <summary>
    /// Checks if the type matches any of the type IDs extracted from the Personal Table used for R/G/B/Y games.
    /// </summary>
    /// <remarks>Valid values: 0, 1, 2, 3, 4, 5, 7, 8, 20, 21, 22, 23, 24, 25, 26</remarks>
    internal static bool TypeIDExists(byte type) => type < 32 && (0b111111100000000000110111111 & (1 << type)) != 0;

    /// <summary>
    /// Species that have a catch rate value that is different from their pre-evolutions, and cannot be obtained directly.
    /// </summary>
    internal static readonly HashSet<byte> Species_NotAvailable_CatchRate = new()
    {
        (int)Butterfree,
        (int)Pidgeot,
        (int)Nidoqueen,
        (int)Nidoking,
        (int)Ninetales,
        (int)Vileplume,
        (int)Persian,
        (int)Arcanine,
        (int)Poliwrath,
        (int)Alakazam,
        (int)Machamp,
        (int)Victreebel,
        (int)Rapidash,
        (int)Cloyster,
        (int)Exeggutor,
        (int)Starmie,
        (int)Dragonite,
    };

    internal static bool IsTradeEvolution1(ushort species) => species is (int)Kadabra or (int)Machoke or (int)Graveler or (int)Haunter;

    public static bool RateMatchesEncounter(ushort species, GameVersion version, byte rate)
    {
        if (version.Contains(YW))
        {
            if (rate == PersonalTable.Y[species].CatchRate)
                return true;
            if (version == YW) // no RB
                return false;
        }
        return rate == PersonalTable.RB[species].CatchRate;
    }

    private static bool GetCatchRateMatchesPreEvolution(PK1 pk, byte catch_rate)
    {
        // For species catch rate, discard any species that has no valid encounters and a different catch rate than their pre-evolutions
        var table = EvolutionTree.Evolves1;
        var chain = table.GetValidPreEvolutions(pk, levelMax: (byte)pk.CurrentLevel);
        foreach (var entry in chain)
        {
            var s = entry.Species;
            if (Species_NotAvailable_CatchRate.Contains((byte)s))
                continue;
            if (catch_rate == PersonalTable.RB[s].CatchRate || catch_rate == PersonalTable.Y[s].CatchRate)
                return true;
        }

        // Krabby encounter trade special catch rate
        ushort species = pk.Species;
        if (catch_rate == 204 && (species is (int)Krabby or (int)Kingler))
            return true;

        if (catch_rate is (167 or 168) && Stadium_GiftSpecies.Contains((byte)species))
            return true;

        return false;
    }

    /// <summary>
    /// Checks if the <see cref="pk"/> can inhabit <see cref="Gen1"></see>
    /// </summary>
    /// <param name="pk">Data to check</param>
    /// <returns>true if can inhabit, false if not.</returns>
    private static bool CanInhabitGen1(this PKM pk)
    {
        // Korean Gen2 games can't trade-back because there are no Gen1 Korean games released
        if (pk.Korean || pk.IsEgg)
            return false;

        // Gen2 format with met data can't receive Gen1 moves, unless Stadium 2 is used (Oak's PC).
        // If you put a Pokemon in the N64 box, the met info is retained, even if you switch over to a Gen I game to teach it TMs
        // You can use rare candies from within the lab, so level-up moves from RBY context can be learned this way as well
        // Stadium 2 is GB Cart Era only (not 3DS Virtual Console).
        if (pk is ICaughtData2 {CaughtData: not 0} && !ParseSettings.AllowGBCartEra)
            return false;

        // Sanity check species, if it could have existed as a pre-evolution.
        ushort species = pk.Species;
        if (species <= MaxSpeciesID_1)
            return true;
        return EvolutionLegality.FutureEvolutionsGen1.Contains(species);
    }

    /// <summary>
    /// Gets the Tradeback status depending on various values.
    /// </summary>
    /// <param name="pk">Pokémon to guess the tradeback status from.</param>
    internal static PotentialGBOrigin GetTradebackStatusInitial(PKM pk)
    {
        if (pk is PK1 pk1)
            return GetTradebackStatusRBY(pk1);

        if (pk.Format == 2 || pk.VC2) // Check for impossible tradeback scenarios
            return !pk.CanInhabitGen1() ? Gen2Only : Either;

        // VC2 is released, we can assume it will be TradebackType.Any.
        // Is impossible to differentiate a VC1 pokemon traded to Gen7 after VC2 is available.
        // Met Date cannot be used definitively as the player can change their system clock.
        return Either;
    }

    /// <summary>
    /// Gets the Tradeback status depending on the <see cref="PK1.Catch_Rate"/>
    /// </summary>
    /// <param name="pk">Pokémon to guess the tradeback status from.</param>
    private static PotentialGBOrigin GetTradebackStatusRBY(PK1 pk)
    {
        if (!ParseSettings.AllowGen1Tradeback)
            return Gen1Only;

        // Detect tradeback status by comparing the catch rate(Gen1)/held item(Gen2) to the species in the pk's evolution chain.
        var catch_rate = pk.Catch_Rate;
        if (catch_rate == 0)
            return Either;

        bool matchAny = GetCatchRateMatchesPreEvolution(pk, catch_rate);
        if (!matchAny)
            return Either;

        if (IsTradebackCatchRate(catch_rate))
            return Either;

        return Gen1Only;
    }

    public static TimeCapsuleEvaluation IsTimeCapsuleTransferred(PKM pk, ReadOnlySpan<MoveResult> moves, IEncounterTemplate enc)
    {
        foreach (var z in moves)
        {
            if (z.Generation == enc.Generation || z.Generation > 2)
                continue;
            if (pk is PK1 {Catch_Rate: not 0} g1 && !IsTradebackCatchRate(g1.Catch_Rate))
                return BadCatchRate;
            return enc.Generation == 2 ? Transferred21 : Transferred12;
        }

        if (pk is not GBPKM gb)
        {
            return enc.Generation switch
            {
                2 when pk.VC2 => Transferred21,
                1 when pk.VC1 => Transferred12,
                _ => NotTransferred,
            };
        }

        if (gb is ICaughtData2 pk2)
        {
            if (enc.Generation == 1)
                return Transferred12;
            if (pk2.CaughtData != 0)
                return NotTransferred;
            if (enc.Version == C)
                return Transferred21;
            return Indeterminate;
        }

        if (gb is PK1 pk1)
        {
            var rate = pk1.Catch_Rate;
            if (rate == 0)
                return Transferred12;

            bool isTradebackItem = IsTradebackCatchRate(rate);
            if (IsCatchRateMatchEncounter(enc, pk1))
                return isTradebackItem ? Indeterminate : NotTransferred;
            return isTradebackItem ? Transferred12 : BadCatchRate;
        }
        return Indeterminate;
    }

    private static bool IsCatchRateMatchEncounter(IEncounterTemplate enc, PK1 pk1) => enc switch
    {
        EncounterStatic1 s when s.GetMatchRating(pk1) != EncounterMatchRating.PartialMatch => true,
        EncounterTrade1 => true,
        _ => RateMatchesEncounter(enc.Species, enc.Version, pk1.Catch_Rate),
    };

    public static bool IsTradebackCatchRate(byte rate) => Array.IndexOf(HeldItems_GSC, rate) != -1;
}

/// <summary>
/// Guess of the origin of a GB Pokémon.
/// </summary>
public enum PotentialGBOrigin
{
    /// <summary>
    /// Pokémon is possible from either generation.
    /// </summary>
    Either,

    /// <summary>
    /// Pokémon is only possible in Generation 1.
    /// </summary>
    Gen1Only,

    /// <summary>
    /// Pokémon is only possible in Generation 2.
    /// </summary>
    Gen2Only,
}

/// <summary>
/// Indicates if the entity has been transferred between Generation 1-2 games via the Time Capsule.
/// </summary>
public enum TimeCapsuleEvaluation
{
    /// <summary>
    /// Transferring via Time Capsule cannot be inferred.
    /// </summary>
    Indeterminate,

    /// <summary>
    /// Indicates that the entity was transferred from Generation 2 to Generation 1.
    /// </summary>
    Transferred21,

    /// <summary>
    /// Indicates that the entity was transferred from Generation 1 to Generation 2, but the catch rate is not a valid tradeback item.
    /// </summary>
    Transferred12,

    /// <summary>
    /// Was not transferred via the Time Capsule.
    /// </summary>
    NotTransferred,

    /// <summary>
    /// Has a catch rate that does not match a held item or the original catch rate value for any progenitor species.
    /// </summary>
    BadCatchRate,
}

/// <summary>
/// Extension methods for <see cref="TimeCapsuleEvaluation"/>.
/// </summary>
public static class TimeCapsuleEvlautationExtensions
{
    /// <summary>
    /// Indicates if the <see cref="eval"/> definitely transferred via Time Capsule.
    /// </summary>
    public static bool WasTimeCapsuleTransferred(this TimeCapsuleEvaluation eval) => eval is not (Indeterminate or NotTransferred or BadCatchRate);
}
