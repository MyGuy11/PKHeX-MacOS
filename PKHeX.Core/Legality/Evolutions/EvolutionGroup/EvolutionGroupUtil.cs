using System;
using static PKHeX.Core.EntityContext;

namespace PKHeX.Core;

/// <summary>
/// Utility logic for getting an <see cref="IEvolutionGroup"/> based on the input.
/// </summary>
public static class EvolutionGroupUtil
{
    /// <summary>
    /// Gets the <see cref="IEvolutionGroup"/> for the <see cref="pk"/>.
    /// </summary>
    public static IEvolutionGroup GetCurrentGroup(PKM pk) => GetCurrentGroup(pk.Context);

    /// <summary>
    /// Gets the <see cref="IEvolutionGroup"/> for the <see cref="EntityContext"/>.
    /// </summary>
    public static IEvolutionGroup GetCurrentGroup(EntityContext context) => context switch
    {
        Gen1 => EvolutionGroup1.Instance,
        Gen2 => EvolutionGroup2.Instance,
        Gen3 => EvolutionGroup3.Instance,
        Gen4 => EvolutionGroup4.Instance,
        Gen5 => EvolutionGroup5.Instance,
        Gen6 => EvolutionGroup6.Instance,
        Gen7 => EvolutionGroup7.Instance,
        Gen8 => EvolutionGroup8.Instance,
        Gen9 => EvolutionGroup9.Instance,

        Gen7b => EvolutionGroup7b.Instance,
        Gen8a => EvolutionGroup8.Instance,
        Gen8b => EvolutionGroup8.Instance,

        _ => throw new ArgumentOutOfRangeException(nameof(context), context, null),
    };
}
