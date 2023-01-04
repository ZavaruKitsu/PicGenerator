#region

using System.Collections.Generic;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

#endregion

namespace PicGenerator.Core;

public sealed class PictureGenerator
{
    public PictureGenerator()
    {
        Settings = new GeneratorSettings();
    }

    public PictureGenerator(GeneratorSettings settings)
    {
        Settings = settings;
    }

    public GeneratorSettings Settings { get; }

    public IEnumerable<Stream> CreateAvatar(string s)
    {
        for (var i = 0; i < Settings.Resolutions.Count; i++)
        {
            var resolution = Settings.Resolutions[i];
            var stream = new MemoryStream();

            var (width, height) = ParseResolution(resolution);

            var image = new Image<Rgba64>(width, height);

            // $$$ean$$$ $$$merti

            Settings.Style?.ProcessImage(s, image, Settings);

            foreach (var decoration in Settings.Decorations)
            {
                decoration?.ProcessImage(image, Settings);
            }

            image.Save(stream, Settings.Encoder);
            stream.Position = 0;

            yield return stream;
        }
    }

    private (int width, int height) ParseResolution(string resolution)
    {
        var split = resolution.Split('x');

        var width = int.Parse(split[0]);
        var height = int.Parse(split[1]);

        return (width, height);
    }
}
