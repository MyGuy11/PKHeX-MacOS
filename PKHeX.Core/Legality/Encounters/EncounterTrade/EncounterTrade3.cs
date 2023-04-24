using System;

namespace PKHeX.Core;

/// <summary>
/// Generation 3 Trade Encounter
/// </summary>
/// <inheritdoc cref="EncounterTrade"/>
public sealed record EncounterTrade3 : EncounterTrade, IContestStatsReadOnly
{
    public override int Generation => 3;
    public override EntityContext Context => EntityContext.Gen3;
    public override int Location => Locations.LinkTrade3NPC;

    /// <summary>
    /// Fixed <see cref="PKM.PID"/> value the encounter must have.
    /// </summary>
    public readonly uint PID;

    public override Shiny Shiny => Shiny.FixedValue;

    public byte CNT_Cool   { get; private init; }
    public byte CNT_Beauty { get; private init; }
    public byte CNT_Cute   { get; private init; }
    public byte CNT_Smart  { get; private init; }
    public byte CNT_Tough  { get; private init; }
    public byte CNT_Sheen  { get; private init; }

    public ReadOnlySpan<byte> Contest
    {
        init
        {
            CNT_Cool   = value[0];
            CNT_Beauty = value[1];
            CNT_Cute   = value[2];
            CNT_Smart  = value[3];
            CNT_Tough  = value[4];
            CNT_Sheen  = value[5];
        }
    }

    public EncounterTrade3(GameVersion game, uint pid, ushort species, byte level) : base(game)
    {
        PID = pid;
        Species = species;
        Level = level;
    }

    public override bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (!base.IsMatchExact(pk, evo))
            return false;

        if (pk is IContestStatsReadOnly s && s.IsContestBelow(this))
            return false;

        return true;
    }

    protected override void ApplyDetails(ITrainerInfo sav, EncounterCriteria criteria, PKM pk)
    {
        base.ApplyDetails(sav, criteria, pk);
        var pk3 = (PK3) pk;

        // Italian LG Jynx untranslated from English name
        if (Species == (int)Core.Species.Jynx && pk3 is { Version: (int)GameVersion.LG, Language: (int)LanguageID.Italian })
        {
            pk3.OT_Name = GetOT((int)LanguageID.English);
            pk3.SetNickname(GetNickname((int)LanguageID.English));
        }

        this.CopyContestStatsTo((PK3)pk);
    }

    protected override void SetPINGA(PKM pk, EncounterCriteria criteria)
    {
        var pi = pk.PersonalInfo;
        int gender = criteria.GetGender(EntityGender.GetFromPID(Species, PID), pi);
        int nature = (int)criteria.GetNature(Nature);
        int ability = criteria.GetAbilityFromNumber(Ability);

        pk.PID = PID;
        pk.Nature = nature;
        pk.Gender = gender;
        pk.RefreshAbility(ability);

        SetIVs(pk);
    }

    protected override bool IsMatchNatureGenderShiny(PKM pk)
    {
        return PID == pk.EncryptionConstant;
    }
}
