#region

using SixLabors.ImageSharp;

#endregion

namespace PicGenerator.Core.Styles;

public interface IDecoration
{
    void ProcessImage(Image image, GeneratorSettings settings);
}
