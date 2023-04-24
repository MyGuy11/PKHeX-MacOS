namespace PKHeX.Core;

/// <summary>
/// Interface that exposes an indication of the Pokémon's Tera Type.
/// </summary>
public interface ITeraType : ITeraTypeReadOnly
{
    /// <summary> Tera Type value the entity is originally encountered with. </summary>
    MoveType TeraTypeOriginal { get; set; }
    /// <summary> If the type was modified, this value will indicate accordingly. </summary>
    MoveType TeraTypeOverride { get; set; }
}

public interface ITeraTypeReadOnly
{
    /// <summary> Elemental type the Pokémon's Tera Type is. </summary>
    MoveType TeraType { get; }
}

/// <summary>
/// Logic for interacting with Tera Types.
/// </summary>
public static class TeraTypeUtil
{
    /// <summary>
    /// Magic value to indicate that a Tera Type has not been overriden from the original value.
    /// </summary>
    public const byte OverrideNone = 19;

    /// <summary>
    /// Max amount of Tera Types possible. Range is [0,17].
    /// </summary>
    public const byte MaxType = 17;

    /// <summary>
    /// For out of range values, we fall back to this Tera Type.
    /// </summary>
    private const MoveType Fallback = MoveType.Normal;

    /// <summary>
    /// Calculates the effective Tera Type based on the inputs.
    /// </summary>
    /// <param name="t">Entity to calculate for.</param>
    public static MoveType GetTeraType(this ITeraType t)
    {
        return GetTeraType((byte)t.TeraTypeOriginal, (byte)t.TeraTypeOverride);
    }

    /// <summary>
    /// Calculates the effective Tera Type based on the inputs.
    /// </summary>
    /// <param name="original">Unmodified Tera Type value initially encountered with.</param>
    /// <param name="override">If the type was modified, this value will indicate accordingly.</param>
    public static MoveType GetTeraType(byte original, byte @override)
    {
        if (@override <= MaxType)
            return (MoveType)@override;
        if (@override != OverrideNone)
            return Fallback; // 18 or out of range.

        if (original <= MaxType)
            return (MoveType)original;
        return Fallback; // out of range.
    }

    /// <summary>
    /// Applies a new Tera Type value to the entity.
    /// </summary>
    /// <param name="t">Entity to set the value to.</param>
    /// <param name="type">Value to update with.</param>
    public static void SetTeraType(this ITeraType t, MoveType type)
    {
        if ((byte)type > MaxType)
            type = Fallback;

        var original = t.TeraTypeOriginal;
        if (original == type)
            t.TeraTypeOverride = (MoveType)OverrideNone;
        else
            t.TeraTypeOverride = type;
    }

    /// <summary>
    /// Applies a new Tera Type value to the entity.
    /// </summary>
    /// <param name="t">Entity to set the value to.</param>
    /// <param name="type">Value to update with.</param>
    public static void SetTeraType(this ITeraType t, byte type) => t.SetTeraType((MoveType)type);
}
