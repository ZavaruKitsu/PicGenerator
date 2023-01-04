#region

using System;
using System.Text;

#endregion

namespace PicGenerator.GUI.Models;

public class TypeCutter
{
    public TypeCutter(Type? type)
    {
        if (type is null)
        {
            Name = "<no>";
            Type = null;
        }

        var name = type!.Name;
        name = name.Replace("Decoration", "");
        name = name.Replace("Style", "");

        var sb = new StringBuilder();

        for (var i = 0; i < name.Length; i++)
        {
            if (char.IsUpper(name[i]) && i != 0)
            {
                sb.Append(' ');
                sb.Append(char.ToLower(name[i]));
            }
            else
            {
                sb.Append(name[i]);
            }
        }

        Name = sb.ToString();
        Type = type;
    }

    public string Name { get; }
    public Type? Type { get; }
}
