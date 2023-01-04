#region

using System.Diagnostics;
using System.IO;
using System.Linq;
using PicGenerator.Core;
using PicGenerator.Core.Styles.Avatars;

#endregion

namespace PicGenerator.CLI;

internal class Program
{
    private static void Main(string[] args)
    {
        var generator = new PictureGenerator
        {
            Settings =
            {
                Resolutions =
                {
                    "256x256"
                },
                Font = "Comfortaa",
                Style = new DefaultAvatarStyle()
            }
        };

        generator.Settings.SetEncoderByExt("png");

        var stream = generator.CreateAvatar("RG").First();

        var f = File.Open("res.png", FileMode.Create);
        stream.CopyTo(f);
        stream.Dispose();
        f.Close();

        Process.Start("cmd", new[] { "/c", "start", "res.png" });
    }
}
