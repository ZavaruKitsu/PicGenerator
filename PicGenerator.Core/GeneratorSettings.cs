#region

using System.Collections.Generic;
using System.IO;
using PicGenerator.Core.Styles;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;

#endregion

namespace PicGenerator.Core;

public sealed class GeneratorSettings
{
    private readonly FontCollection _fontCollection;

    public GeneratorSettings()
    {
        _fontCollection = new FontCollection();
        _fontCollection.Install(Path.Combine("Fonts", "Comfortaa-Regular.ttf"));
    }

    public List<string> Resolutions { get; } = new();

    public string Font { get; set; }

    public IStyle? Style { get; set; }

    public List<IDecoration?> Decorations { get; } = new();

    public Color PrimaryColor { get; set; } = Color.White;
    public Color SecondaryColor { get; set; } = Color.Black;
    public Color TertiaryColor { get; set; } = Color.Black;

    public IImageEncoder Encoder { get; set; }

    internal Font CreateFont(int size)
    {
        return _fontCollection.CreateFont(Font, size);
    }

    public void SetEncoderByExt(string ext)
    {
        Encoder = ext switch
                  {
                      "png" => new PngEncoder(),
                      "gif" => new GifEncoder(),
                      "bmp" => new BmpEncoder(),
                      "jpg" => new JpegEncoder(),
                      _     => Encoder
                  };
    }
}
