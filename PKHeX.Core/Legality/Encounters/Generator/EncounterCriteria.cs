using System;
using static PKHeX.Core.AbilityPermission;

namespace PKHeX.Core;

/// <summary>
/// Object that can be fed to a <see cref="IEncounterConvertible"/> converter to ensure that the resulting <see cref="PKM"/> meets rough specifications.
/// </summary>
public sealed record EncounterCriteria
{
    /// <summary>
    /// Default criteria with no restrictions (random) for all fields.
    /// </summary>
    public static readonly EncounterCriteria Unrestricted = new();

    /// <summary> End result's gender. </summary>
    /// <remarks> Leave as -1 to not restrict gender. </remarks>
    public int Gender { get; init; } = -1;

    /// <summary> End result's ability numbers permitted. </summary>
    /// <remarks> Leave as <see cref="Any12H"/> to not restrict ability. </remarks>
    public AbilityPermission AbilityNumber { get; init; } = Any12H;

    /// <summary> End result's nature. </summary>
    /// <remarks> Leave as <see cref="Core.Nature.Random"/> to not restrict nature. </remarks>
    public Nature Nature { get; init; } = Nature.Random;

    /// <summary> End result's shininess. </summary>
    /// <remarks> Leave as <see cref="Core.Shiny.Random"/> to not restrict shininess. </remarks>
    public Shiny Shiny { get; init; }

    public int IV_HP  { get; init; } = RandomIV;
    public int IV_ATK { get; init; } = RandomIV;
    public int IV_DEF { get; init; } = RandomIV;
    public int IV_SPA { get; init; } = RandomIV;
    public int IV_SPD { get; init; } = RandomIV;
    public int IV_SPE { get; init; } = RandomIV;

    /// <summary>
    /// If the Encounter yields variable level ranges (e.g. RNG correlation), force the minimum level instead of yielding first match.
    /// </summary>
    public bool ForceMinLevelRange { get; set; }

    public sbyte TeraType { get; init; } = -1;

    // unused
    public int HPType { get; init; } = -1;

    private const int RandomIV = -1;

    /// <summary>
    /// Checks if the IVs are compatible with the encounter's defined IV restrictions.
    /// </summary>
    /// <param name="encounterIVs">Encounter template's IV restrictions. Speed is not last.</param>
    /// <param name="generation">Destination generation</param>
    /// <returns>True if compatible, false if incompatible.</returns>
    public bool IsIVsCompatible(Span<int> encounterIVs, int generation)
    {
        var IVs = encounterIVs;
        if (!ivCanMatch(IV_HP , IVs[0])) return false;
        if (!ivCanMatch(IV_ATK, IVs[1])) return false;
        if (!ivCanMatch(IV_DEF, IVs[2])) return false;
        if (!ivCanMatch(IV_SPE, IVs[3])) return false;
        if (!ivCanMatch(IV_SPA, IVs[4])) return false;
        if (!ivCanMatch(IV_SPD, IVs[5])) return false;

        bool ivCanMatch(int requestedIV, int encounterIV)
        {
            if (requestedIV >= 30 && generation >= 6) // hyper training possible
                return true;
            return encounterIV == RandomIV || requestedIV == RandomIV || requestedIV == encounterIV;
        }

        return true;
    }

    /// <inheritdoc cref="GetCriteria(IBattleTemplate, IPersonalInfo)"/>
    /// <param name="s">Template data (end result).</param>
    /// <param name="t">Personal table the end result will exist with.</param>
    public static EncounterCriteria GetCriteria(IBattleTemplate s, IPersonalTable t)
    {
        var pi = t.GetFormEntry(s.Species, s.Form);
        return GetCriteria(s, pi);
    }

    /// <summary>
    /// Creates a new <see cref="EncounterCriteria"/> by loading parameters from the provided <see cref="IBattleTemplate"/>.
    /// </summary>
    /// <param name="s">Template data (end result).</param>
    /// <param name="pi">Personal info the end result will exist with.</param>
    /// <returns>Initialized criteria data to be passed to generators.</returns>
    public static EncounterCriteria GetCriteria(IBattleTemplate s, IPersonalInfo pi) => new()
    {
        Gender = s.Gender,
        IV_HP = s.IVs[0],
        IV_ATK = s.IVs[1],
        IV_DEF = s.IVs[2],
        IV_SPE = s.IVs[3],
        IV_SPA = s.IVs[4],
        IV_SPD = s.IVs[5],
        HPType = s.HiddenPowerType,

        AbilityNumber = GetAbilityNumber(s.Ability, pi),
        Nature = NatureUtil.GetNature(s.Nature),
        Shiny = s.Shiny ? Shiny.Always : Shiny.Never,
        TeraType = (sbyte)s.TeraType,
    };

    private static AbilityPermission GetAbilityNumber(int ability, IPersonalAbility pi)
    {
        var count = pi.AbilityCount;
        if (count < 2 || pi is not IPersonalAbility12 a)
            return Any12;
        var dual = GetAbilityValueDual(ability, a);
        if (count == 2 || pi is not IPersonalAbility12H h) // prior to gen5
            return dual;
        if (ability == h.AbilityH)
            return dual == Any12 ? Any12H : OnlyHidden;
        return dual;
    }

    private static AbilityPermission GetAbilityValueDual(int ability, IPersonalAbility12 a)
    {
        if (ability == a.Ability1)
            return ability != a.Ability2 ? OnlyFirst : Any12;
        return ability == a.Ability2 ? OnlySecond : Any12;
    }

    /// <summary>
    /// Gets a random nature to generate, based off an encounter's <see cref="encValue"/>.
    /// </summary>
    public Nature GetNature(Nature encValue)
    {
        if ((uint)encValue < 25)
            return encValue;
        if (Nature != Nature.Random)
            return Nature;
        return (Nature)Util.Rand.Next(25);
    }

    /// <summary>
    /// Gets a random gender to generate, based off an encounter's <see cref="gender"/>.
    /// </summary>
    public int GetGender(int gender, IGenderDetail pkPersonalInfo)
    {
        if ((uint)gender < 3)
            return gender;
        if (!pkPersonalInfo.IsDualGender)
            return pkPersonalInfo.FixedGender();
        if (pkPersonalInfo.Genderless)
            return 2;
        if (Gender is 0 or 1)
            return Gender;
        return pkPersonalInfo.RandomGender();
    }

    /// <summary>
    /// Gets a random ability index (0/1/2) to generate, based off an encounter's <see cref="num"/>.
    /// </summary>
    public int GetAbilityFromNumber(AbilityPermission num)
    {
        if (num.IsSingleValue(out int index)) // fixed number
            return index;

        bool canBeHidden = num.CanBeHidden();
        return GetAbilityIndexPreference(canBeHidden);
    }

    private int GetAbilityIndexPreference(bool canBeHidden = false) => AbilityNumber switch
    {
        OnlyFirst => 0,
        OnlySecond => 1,
        OnlyHidden or Any12H when canBeHidden => 2, // hidden allowed
        _ => Util.Rand.Next(2),
    };

    /// <summary>
    /// Applies random IVs without any correlation.
    /// </summary>
    /// <param name="pk">Entity to mutate.</param>
    public void SetRandomIVs(PKM pk)
    {
        pk.IV_HP = IV_HP != RandomIV ? IV_HP : Util.Rand.Next(32);
        pk.IV_ATK = IV_ATK != RandomIV ? IV_ATK : Util.Rand.Next(32);
        pk.IV_DEF = IV_DEF != RandomIV ? IV_DEF : Util.Rand.Next(32);
        pk.IV_SPA = IV_SPA != RandomIV ? IV_SPA : Util.Rand.Next(32);
        pk.IV_SPD = IV_SPD != RandomIV ? IV_SPD : Util.Rand.Next(32);
        pk.IV_SPE = IV_SPE != RandomIV ? IV_SPE : Util.Rand.Next(32);
    }
}
