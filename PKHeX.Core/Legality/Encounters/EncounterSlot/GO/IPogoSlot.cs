namespace PKHeX.Core;

/// <summary>
/// Stores details about <see cref="GameVersion.GO"/> encounters relevant for legality.
/// </summary>
public interface IPogoSlot
{
    /// <summary> Start date the encounter became available. If zero, no date specified (unbounded start). </summary>
    int StartDate { get; }

    /// <summary> Last day the encounter was available. If zero, no date specified (unbounded finish). </summary>
    /// <remarks> If there is no end date (yet), we'll try to clamp to a date in the near-future to prevent it from being open-ended. </remarks>
    int EndDate { get; }

    /// <summary> Possibility of shiny for the encounter. </summary>
    Shiny Shiny { get; }

    /// <summary> Method the Pok�mon may be encountered with. </summary>
    PogoType Type { get; }

    /// <summary> Gender the Pok�mon may be encountered with. </summary>
    Gender Gender { get; }
}
