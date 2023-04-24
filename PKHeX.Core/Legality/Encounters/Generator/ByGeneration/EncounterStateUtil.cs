namespace PKHeX.Core;

/// <summary>
/// Utility logic for checking encounter state.
/// </summary>
public static class EncounterStateUtil
{
    /// <summary>
    /// Checks if the input <see cref="pk"/> could have been a wild encounter.
    /// </summary>
    /// <param name="pk">Pokémon to check.</param>
    /// <returns>True if the <see cref="pk"/> could have been a wild encounter, false otherwise.</returns>
    public static bool CanBeWildEncounter(PKM pk)
    {
        if (pk.IsEgg)
            return false;
        if (IsMetAsEgg(pk))
            return false;
        return true;
    }

    /// <summary>
    /// Checks if the input <see cref="pk"/> was met as an egg.
    /// </summary>
    /// <param name="pk">Pokémon to check.</param>
    /// <returns>True if the <see cref="pk"/> was met as an egg, false otherwise.</returns>
    /// <remarks>Only applicable for Generation 4 origins and above.</remarks>
    public static bool IsMetAsEgg(PKM pk) => pk switch
    {
        // This all could be simplified to just checking Egg_Day != 0 without type checks.
        // Leaving like this to indicate how Egg_Location is not a true indicator due to quirks from BD/SP.

        PA8 or PK8 => pk.Egg_Location is not 0 || pk is { BDSP: true, Egg_Day: not 0 },
        PB8 pb8 => pb8.Egg_Location is not Locations.Default8bNone,
        _ => pk.Egg_Location is not 0,
    };
}
