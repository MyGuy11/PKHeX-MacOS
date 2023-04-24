namespace PKHeX.Core;

/// <summary>
/// Interface for Accessing named blocks within a Generation 9 save file.
/// </summary>
public interface ISaveBlock9Main
{
    Box8 BoxInfo { get; }
    Party9 PartyInfo { get; }
    MyItem9 Items { get; }
    MyStatus9 MyStatus { get; }
    BoxLayout9 BoxLayout { get; }
    PlayTime9 Played { get; }
    Zukan9 Zukan { get; }
    ConfigSave9 Config { get; }
    TeamIndexes9 TeamIndexes { get; }
    Epoch1970Value LastSaved { get; }
    PlayerFashion9 PlayerFashion { get; }
    PlayerAppearance9 PlayerAppearance { get; }
    RaidSpawnList9 Raid { get; }
}
