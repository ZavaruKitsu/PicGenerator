#region

using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;

#endregion

// ReSharper disable PossibleLossOfFraction

namespace PicGenerator.Core.Styles.Avatars;

public class DefaultAvatarStyle : IStyle
{
    private const float Thickness = 48f;
    private const float Thickness2 = 32f;
    private const float Thickness3 = 24f;

    /// <inheritdoc />
    public void ProcessImage(string s, Image image, GeneratorSettings settings)
    {
        var radius = image.Height / 2;

        var thickness = radius / Thickness;

        if (thickness < 1f)
        {
            thickness = radius / Thickness2;
        }

        if (thickness < 0.8f)
        {
            thickness = radius / Thickness3;
        }

        var circle = new EllipsePolygon(radius, radius, radius - thickness);
        var outline = circle.GenerateOutline(thickness);

        image.Mutate(context =>
                         context.Fill(settings.PrimaryColor, circle).Fill(settings.SecondaryColor, outline));

        var font = settings.CreateFont(radius);
        var fontMeasure = TextMeasurer.Measure(s, new RendererOptions(font));

        var x = radius - fontMeasure.Width / 2f + fontMeasure.X;
        var y = radius - fontMeasure.Height / 2f + fontMeasure.Y;

        image.Mutate(context => context.DrawText(s, font, settings.TertiaryColor, new PointF(x, y)));
    }
}
