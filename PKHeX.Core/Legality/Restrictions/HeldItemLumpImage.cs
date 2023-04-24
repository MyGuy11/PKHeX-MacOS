namespace PKHeX.Core;

/// <summary>
/// Lumped image type.
/// </summary>
public enum HeldItemLumpImage
{
    /// <summary>
    /// Held Item spriite is a specific held item.
    /// </summary>
    NotLump = 0,

    /// <summary>
    /// Held Item sprite should show the Technical Machine (TM) icon.
    /// </summary>
    TechnicalMachine,

    /// <summary>
    /// Held Item sprite should show the Technical Record (TR) icon.
    /// </summary>
    TechnicalRecord,
}

/// <summary>
/// Logic to check if a held item should how a lumped image sprite.
/// </summary>
public static class HeldItemLumpUtil
{
    /// <summary>
    /// Checks if the <see cref="image"/> is a lumped sprite.
    /// </summary>
    /// <param name="image">Evaluated type</param>
    /// <returns>True if the <see cref="image"/> is a lumped sprite.</returns>
    public static bool IsLump(this HeldItemLumpImage image) => image != HeldItemLumpImage.NotLump;

    /// <summary>
    /// Checks if the <see cref="item"/> should show a lumped sprite.
    /// </summary>
    /// <param name="item">Held Item index</param>
    /// <param name="context">Generation context</param>
    /// <returns>Evaluation result.</returns>
    public static HeldItemLumpImage GetIsLump(int item, EntityContext context) => context.Generation() switch
    {
        <= 4 when item is (>= 0328 and <= 0419) => HeldItemLumpImage.TechnicalMachine, // gen2/3/4 TM
        8 when item is (>= 0328 and <= 0427) => HeldItemLumpImage.TechnicalMachine, // BDSP TMs
        8 when item is (>= 1130 and <= 1229) => HeldItemLumpImage.TechnicalRecord, // Gen8 TR
        9 when item is (>= 0328 and <= 0419) // TM01-TM92
            or (>= 0618 and <= 0620) // TM93-TM95
            or (>= 0690 and <= 0693) // TM96-TM99
            or (>= 2160 and <= 2231) /* TM100-TM171 */ => HeldItemLumpImage.TechnicalMachine,
        _ => HeldItemLumpImage.NotLump,
    };
}
