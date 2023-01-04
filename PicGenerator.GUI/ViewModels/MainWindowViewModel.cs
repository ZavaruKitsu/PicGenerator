#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using DynamicData;
using DynamicData.Binding;
using PicGenerator.Core;
using PicGenerator.Core.Styles;
using PicGenerator.GUI.Models;
using ReactiveUI;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.Extensions;
using SixLabors.ImageSharp.Formats.Png;

#endregion

namespace PicGenerator.GUI.ViewModels;

public class MainWindowViewModel : ViewModelBase, IValidatableViewModel
{
    private string _chars = "AZ";

    private IObservable<int> _resolutionsChecked;

    private TypeCutter _selectedDecoration;
    private TypeCutter _selectedStyle;

    public MainWindowViewModel()
    {
        Resolutions.AddRange(new List<string>
        {
            "16x16", "32x32", "48x48", "64x64", "128x128", "256x256", "512x512", "1024x1024"
        }.Select(x => new Resolution(x)));

        SelectedStyle = Styles.First();

        ResolutionsChecked = Resolutions.ToObservableChangeSet()
                                        .AutoRefresh(x => x.IsChecked)
                                        .ToCollection()
                                        .Select(y => y.Count(z => z.IsChecked));

        this.ValidationRule(viewModel => viewModel.Chars,
                            chars => !string.IsNullOrWhiteSpace(chars) && chars.Length == 2,
                            "Enter valid chars");

        this.ValidationRule(viewModel => viewModel.SelectedStyle, style => style != null, "Select style");

        this.ValidationRule(viewModel => viewModel.ResolutionsChecked,
                            ResolutionsChecked, state => state > 0, _ => "Select at least one resolution");

        UpdateCommand = ReactiveCommand.Create(UpdateImage, ValidationContext.Valid);
        UpdateCommand.ThrownExceptions.Select(x => x.Message).Subscribe(Console.WriteLine);

        var change1 = this.WhenAny(viewModel => viewModel.Chars, viewModel => viewModel.PrimaryColor,
                                   viewModel => viewModel.SecondaryColor, viewModel => viewModel.TertiaryColor,
                                   viewModel => viewModel.SelectedStyle, viewModel => viewModel.SelectedDecoration,
                                   (change, observedChange, arg3, arg4, arg5, arg6) => Unit.Default)!;
        var change2 = this.WhenAnyObservable(viewModel => viewModel.ResolutionsChecked).Select(x => Unit.Default)!;

        var list = new List<IObservable<Unit>> { change1, change2 };
        list.Merge().InvokeCommand(UpdateCommand);

        SaveCommand = ReactiveCommand.CreateFromTask(SaveImages, ValidationContext.Valid);
    }

    public IObservable<int> ResolutionsChecked
    {
        get => _resolutionsChecked;
        set => this.RaiseAndSetIfChanged(ref _resolutionsChecked, value);
    }

    public string Chars
    {
        get => _chars;
        set => this.RaiseAndSetIfChanged(ref _chars, value);
    }

    public ReactiveCommand<Unit, Unit> UpdateCommand { get; set; }

    public List<TypeCutter> Styles { get; set; } = typeof(IStyle).Assembly.ExportedTypes
                                                                 .Where(x => x.IsAssignableTo(typeof(IStyle)) &&
                                                                             !x.IsAbstract)
                                                                 .Select(x => new TypeCutter(x)).ToList();

    public List<TypeCutter> Decorations { get; set; } = typeof(IDecoration).Assembly.ExportedTypes
                                                                           .Where(x =>
                                                                               x.IsAssignableTo(typeof(
                                                                                   IDecoration)) &&
                                                                               !x.IsAbstract)
                                                                           .Select(x => new TypeCutter(x)).ToList();

    public ObservableCollectionExtended<Resolution> Resolutions { get; set; } = new();

    public TypeCutter? SelectedDecoration
    {
        get => _selectedDecoration;
        set => this.RaiseAndSetIfChanged(ref _selectedDecoration, value);
    }

    public TypeCutter? SelectedStyle
    {
        get => _selectedStyle;
        set => this.RaiseAndSetIfChanged(ref _selectedStyle, value);
    }

    public AvaloniaList<Bitmap> Images { get; set; } = new();

    public ReactiveCommand<Unit, Unit> SaveCommand { get; set; }

    /// <inheritdoc />
    public ValidationContext ValidationContext { get; } = new();

    private async Task SaveImages()
    {
        var dialog = new SaveFileDialog { DefaultExtension = "png", InitialFileName = Chars };

        var current = (Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)
            .MainWindow;
        var res = await dialog.ShowAsync(current);

        if (res == null)
        {
            return;
        }

        var dir = Path.GetDirectoryName(res);

        if (dir == null)
        {
            return;
        }

        Parallel.ForEach(Images, image =>
        {
            var name = $"{Chars}_x{image.Size.Width}.png";
            var resPath = Path.Combine(dir, name);
            image.Save(resPath);
        });

        if (OperatingSystem.IsWindows())
        {
            Process.Start("explorer.exe",
                          $"/select, \"{Path.Combine(dir, $"{Chars}_x{Images.First().Size.Width}.png")}\"");
        }
    }

    private void UpdateImage()
    {
        var settings = new GeneratorSettings();
        var generator = new PictureGenerator(settings);

        if (SelectedStyle != null)
        {
            settings.Style = (IStyle)Activator.CreateInstance(SelectedStyle.Type)!;
        }

        if (SelectedDecoration != null && SelectedDecoration.Type is not null)
        {
            settings.Decorations.Add((IDecoration)Activator.CreateInstance(SelectedDecoration.Type)!);
        }

        settings.PrimaryColor = PrimaryColor.Sharpify();
        settings.SecondaryColor = SecondaryColor.Sharpify();
        settings.TertiaryColor = TertiaryColor.Sharpify();

        settings.Font = "Comfortaa";
        settings.Resolutions.AddRange(Resolutions.Where(x => x.IsChecked).Select(x => x.ResolutionText));

        settings.Encoder = new PngEncoder();

        var res = generator.CreateAvatar(Chars);
        var bitmaps = res.Select(x => new Bitmap(x));

        Images.Clear();
        Images.AddRange(bitmaps);
    }

    #region Colors

    private Color _primaryColor = Color.Parse("#ffffff");
    private Color _secondaryColor = Color.Parse("#000000");
    private Color _tertiaryColor = Color.Parse("#000000");

    #endregion

    #region Colors

    public Color PrimaryColor
    {
        get => _primaryColor;
        set => this.RaiseAndSetIfChanged(ref _primaryColor, value);
    }

    public Color SecondaryColor
    {
        get => _secondaryColor;
        set => this.RaiseAndSetIfChanged(ref _secondaryColor, value);
    }

    public Color TertiaryColor
    {
        get => _tertiaryColor;
        set => this.RaiseAndSetIfChanged(ref _tertiaryColor, value);
    }

    #endregion
}
