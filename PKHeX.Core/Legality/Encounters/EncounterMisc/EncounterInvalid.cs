using System;

namespace PKHeX.Core;

/// <summary>
/// Invalid Encounter Data
/// </summary>
public sealed record EncounterInvalid : IEncounterable
{
    public static readonly EncounterInvalid Default = new();

    public ushort Species { get; }
    public byte Form { get; }
    public byte LevelMin { get; }
    public byte LevelMax { get; }
    public bool EggEncounter { get; }
    public int Generation { get; init; }
    public EntityContext Context { get; }
    public GameVersion Version { get; }
    public bool IsShiny => false;
    public Shiny Shiny => Shiny.Never;

    public string Name => "Invalid";
    public string LongName => "Invalid";
    public int Location => 0;
    public int EggLocation => 0;
    public AbilityPermission Ability => AbilityPermission.Any12H;
    public Ball FixedBall => Ball.None;

    private EncounterInvalid() { }

    public EncounterInvalid(PKM pk)
    {
        Species = pk.Species;
        Form = pk.Form;
        LevelMin = (byte)pk.Met_Level;
        LevelMax = (byte)pk.CurrentLevel;
        EggEncounter = pk.WasEgg;
        Generation = pk.Generation;
        Version = (GameVersion)pk.Version;
        Context = pk.Context;
    }

    public PKM ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);
    public PKM ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => throw new ArgumentException($"Cannot convert an {nameof(EncounterInvalid)} to PKM.");
}
