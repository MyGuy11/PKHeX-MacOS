using System;
using static PKHeX.Core.Species;

namespace PKHeX.Core;

/// <summary>
/// Personal Table storing <see cref="PersonalInfo4"/> used in Generation 4 games.
/// </summary>
public sealed class PersonalTable4 : IPersonalTable, IPersonalTable<PersonalInfo4>
{
    private readonly PersonalInfo4[] Table;
    private const int SIZE = PersonalInfo4.SIZE;
    private const int MaxSpecies = Legal.MaxSpeciesID_4;
    public int MaxSpeciesID => MaxSpecies;

    public PersonalTable4(ReadOnlySpan<byte> data)
    {
        Table = new PersonalInfo4[data.Length / SIZE];
        var count = data.Length / SIZE;
        for (int i = 0, ofs = 0; i < count; i++, ofs += SIZE)
        {
            var slice = data.Slice(ofs, SIZE).ToArray();
            Table[i] = new PersonalInfo4(slice);
        }
    }

    public PersonalInfo4 this[int index] => Table[(uint)index < Table.Length ? index : 0];
    public PersonalInfo4 this[ushort species, byte form] => Table[GetFormIndex(species, form)];
    public PersonalInfo4 GetFormEntry(ushort species, byte form) => Table[GetFormIndex(species, form)];

    public int GetFormIndex(ushort species, byte form)
    {
        if (species <= MaxSpecies)
            return Table[species].FormIndex(species, form);
        return 0;
    }

    public bool IsSpeciesInGame(ushort species) => species <= MaxSpecies;
    public bool IsPresentInGame(ushort species, byte form)
    {
        if (!IsSpeciesInGame(species))
            return false;
        if (form == 0)
            return true;
        if (Table[species].HasForm(form))
            return true;
        return species switch
        {
            (int)Pichu => form == 1,
            (int)Unown => form < 28,
            (int)Castform => form < 4,
            (int)Burmy => form < 3,
            (int)Mothim => form < 3,
            (int)Cherrim => form == 1,
            (int)Shellos or (int)Gastrodon => form == 1,
            (int)Arceus => form is < 18 and not 9, // Curse type does not exist legally.
            _ => false,
        };
    }

    PersonalInfo IPersonalTable.this[int index] => this[index];
    PersonalInfo IPersonalTable.this[ushort species, byte form] => this[species, form];
    PersonalInfo IPersonalTable.GetFormEntry(ushort species, byte form) => GetFormEntry(species, form);

    public void LoadTables(BinLinkerAccessor tutors)
    {
        var table = Table;
        for (ushort i = Legal.MaxSpeciesID_4; i != 0; i--)
        {
            // Set to form 0 and copy the reference to all other form entries.
            var form0 = table[i];
            form0.AddTypeTutors(tutors[i]);
            var fc = form0.FormCount;
            for (byte f = 1; f < fc; f++)
            {
                var pi = table[form0.FormIndex(i, f)];
                pi.CopyTypeTutors(form0);
            }
        }
    }
}
