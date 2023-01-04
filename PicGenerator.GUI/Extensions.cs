#region

using SixLabors.ImageSharp;

#endregion

namespace PicGenerator.GUI;

public static class Extensions
{
    public static Color Sharpify(this Avalonia.Media.Color color)
    {
        return Color.FromRgba(color.R, color.G, color.B, color.A);
    }
}
