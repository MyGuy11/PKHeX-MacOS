using System;
using System.Runtime.InteropServices;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;

namespace PKHeX.Drawing.OSX;

public static class ImageUtil
{
    public static Bitmap LayerImage(Image baseLayer, Image overLayer, int x, int y, double transparency)
    {
        overLayer = ChangeOpacity(overLayer, transparency);
        return LayerImage(baseLayer, overLayer, x, y);
    }

    public static Bitmap LayerImage(Image baseLayer, Image overLayer, int x, int y)
    {
        Bitmap img = new(baseLayer);
        using Graphics gr = Graphics.FromImage(img);
        gr.DrawImage(overLayer, x, y, overLayer.Width, overLayer.Height);
        return img;
    }
}
