using System;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace PKHeX.Drawing.OSX;

/// <summary>
/// Utility class for manipulating <see cref="Color"/> values.
/// </summary>
public static class ColorUtil
{
    private const byte MaxStat = 180; // shift the green cap down
    private const byte MinStat = 0;

    public static Color ColorBaseStat(int stat)
    {
        float x = (100f * stat) / MaxStat;
        if (x > 100)
            x = 100;
        double red = 255f * (x > 50 ? 1 - (2 * (x - 50) / 100.0) : 1.0);
        double green = 255f * (x > 50 ? 1.0 : 2 * x / 100.0);

        return Blend(Color.FromRgb((byte)red, (byte)green, 0), Color.White, 0.4);
    }

    public static Color ColorBaseStatTotal(int tot)
    {
        const byte sumShiftDown = 175;
        const float sumDivisor = 3f;
        var sumToSingle = Math.Max(MinStat, tot - sumShiftDown) / sumDivisor;
        return ColorBaseStat((int)sumToSingle);
    }

    public static Color Blend(Color color, Color backColor, double amount)
    {
        Argb32 pixel = color.ToPixel<Argb32>();
        Argb32 backPixel = backColor.ToPixel<Argb32>();
        byte r = (byte)((pixel.R * amount) + (backPixel.R * (1 - amount)));
        byte g = (byte)((pixel.G * amount) + (backPixel.G * (1 - amount)));
        byte b = (byte)((pixel.B * amount) + (backPixel.B * (1 - amount)));
        return Color.FromRgb(r, g, b);
    }
}
