#region

using SixLabors.ImageSharp;

#endregion

namespace PicGenerator.Core.Styles.Decorations;

public class EmptyDecoration : IDecoration
{
    /// <inheritdoc />
    public void ProcessImage(Image image, GeneratorSettings settings)
    {
        // ignored
    }
}
