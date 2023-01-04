#region

using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

#endregion

namespace PicGenerator.Core.Styles.Decorations;

public class NewYearDecoration : IDecoration
{
    static NewYearDecoration()
    {
        Hat.Mutate(context => context.Rotate(-30f));
    }

    public static Image Hat { get; } = Image.Load(Path.Combine("Images", "Decorations", "new_year.png"));

    /// <inheritdoc />
    public void ProcessImage(Image image, GeneratorSettings settings)
    {
        var size = (int)(image.Width * 1.3);
        var hat = Hat.Clone(x => x.Resize(size, size));

        var x = (int)(image.Width / 2 - size / 2 + image.Width * 0.1);
        var y = -(int)(size * 0.35);

        image.Mutate(context => context.DrawImage(hat, new Point(x, y), 1));
    }
}
