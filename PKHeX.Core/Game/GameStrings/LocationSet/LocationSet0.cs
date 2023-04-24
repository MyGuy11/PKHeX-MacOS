using System;
using System.Collections.Generic;

namespace PKHeX.Core;

public sealed record LocationSet0(string[] Met0) : ILocationSet
{
    public ReadOnlySpan<string> GetLocationNames(int bankID) => bankID switch
    {
        0 => Met0,
        _ => Array.Empty<string>(),
    };

    public string GetLocationName(int locationID)
    {
        if ((uint)locationID >= Met0.Length)
            return string.Empty;
        return Met0[locationID];
    }

    public IEnumerable<(int Bank, string[] Names)> GetAll()
    {
        yield return (0, Met0);
    }
}
