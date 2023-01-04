#region

using ReactiveUI;

#endregion

namespace PicGenerator.GUI.Models;

public class Resolution : ReactiveObject
{
    private bool _isChecked;

    public Resolution(string resolution)
    {
        ResolutionText = resolution;
    }

    public string ResolutionText { get; set; }

    public bool IsChecked
    {
        get => _isChecked;
        set => this.RaiseAndSetIfChanged(ref _isChecked, value);
    }
}
