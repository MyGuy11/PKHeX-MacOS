using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Xunit;

namespace PKHeX.Core.Tests.Legality;

public class LegalityData
{
    [Fact]
    public void EvolutionsOrdered() // feebas, see issue #2394
    {
        var trees = typeof(EvolutionTree).GetFields(BindingFlags.Static | BindingFlags.NonPublic);
        var fEntries = typeof(EvolutionTree).GetFields(BindingFlags.NonPublic | BindingFlags.Instance).First(z => z.Name == "Entries");
        foreach (var t in trees)
        {
            var gen = Convert.ToInt32(t.Name[7].ToString());
            if (gen <= 4)
                continue;

            if (t.GetValue(typeof(EvolutionTree)) is not EvolutionTree fTree)
                throw new ArgumentException(nameof(fTree));
            if (fEntries.GetValue(fTree) is not IReadOnlyList<EvolutionMethod[]> entries)
                throw new ArgumentException(nameof(entries));
            var feebas = entries[(int)Species.Feebas];
            if (feebas.Length == 0)
                continue;

            var t1 = feebas[0].Method;
            var t2 = feebas[1].Method;

            t1.IsLevelUpRequired().Should().BeFalse();
            t2.IsLevelUpRequired().Should().BeTrue();
        }
    }

    [Fact]
    public void EvolutionsOrderedSV()
    {
        // SV Crabrawler added a second, UseItem evolution method. Need to be sure it's before the more restrictive level-up method.
        var tree = EvolutionTree.Evolves9;
        var fEntries = typeof(EvolutionTree).GetFields(BindingFlags.NonPublic | BindingFlags.Instance).First(z => z.Name == "Entries");
        if (fEntries.GetValue(tree) is not IReadOnlyList<EvolutionMethod[]> entries)
            throw new ArgumentException(nameof(entries));
        var crab = entries[(int)Species.Crabrawler];

        var t1 = crab[0].Method;
        var t2 = crab[1].Method;

        t1.IsLevelUpRequired().Should().BeFalse();
        t2.IsLevelUpRequired().Should().BeTrue();
    }
}
