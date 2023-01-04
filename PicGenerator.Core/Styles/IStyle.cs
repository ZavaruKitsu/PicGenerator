#region

using SixLabors.ImageSharp;

#endregion

namespace PicGenerator.Core.Styles;

public interface IStyle
{
    void ProcessImage(string s, Image image, GeneratorSettings settings);
}
