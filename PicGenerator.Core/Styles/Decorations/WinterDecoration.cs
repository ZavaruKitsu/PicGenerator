#region

using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

#endregion

namespace PicGenerator.Core.Styles.Decorations;

public class WinterDecoration : IDecoration
{
    static WinterDecoration()
    {
        Leaf.Mutate(context => context.Rotate(-30f));
    }

    public static Image Leaf { get; } = Image.Load(Path.Combine("Images", "Decorations", "winter.png"));


    /// <inheritdoc />
    public void ProcessImage(Image image, GeneratorSettings settings)
    {
        var size = (int)(image.Width * 1.2);
        var leaf = Leaf.Clone(x => x.Resize(size, size));

        var x = image.Width / 2 - size / 2;
        var y = (int)(-image.Width * 0.1);

        image.Mutate(context => context.DrawImage(leaf, new Point(x, y), 0.25f));
    }
}
