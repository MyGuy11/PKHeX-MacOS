using System;
using FluentAssertions;
using Xunit;
using static PKHeX.Core.Species;
using static PKHeX.Core.Move;

namespace PKHeX.Core.Tests.Legality;

public static class TempTests
{
    // BD/SP has egg moves that cannot be obtained because no parents in the egg group can know the move.
    [Theory]
    [InlineData(Taillow, Boomburst)]
    [InlineData(Plusle, TearfulLook)] [InlineData(Minun, TearfulLook)]
    [InlineData(Luvdisc, HealPulse)]
    [InlineData(Starly, Detect)]
    [InlineData(Chatot, Boomburst)] [InlineData(Chatot, Encore)]
    [InlineData(Spiritomb, FoulPlay)]
    public static void CanLearnEggMoveBDSP(Species species, Move move)
    {
        MoveEgg.GetEggMoves(8, (ushort)species, 0, GameVersion.BD).Contains((ushort)move).Should().BeFalse();

        var pb8 = new PB8 { Species = (ushort)species };
        var encs = EncounterMovesetGenerator.GenerateEncounters(pb8, new[] { (ushort)move }, GameVersion.BD);

        encs.Should().BeEmpty("HOME supports BD/SP, but does not make any disconnected moves available.");
    }
}
