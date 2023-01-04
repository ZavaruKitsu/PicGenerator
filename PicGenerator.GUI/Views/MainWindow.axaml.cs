#region

using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using PicGenerator.GUI.ViewModels;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;

#endregion

namespace PicGenerator.GUI.Views;

public class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    public MainWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
        this.WhenActivated(disposables =>
        {
            // ReSharper disable once ConvertToLambdaExpressionWhenPossible
            this.BindValidation(ViewModel, view => view.StatusBar.Text, new ValidationFormatter())
                .DisposeWith(disposables);
        });
    }

    public TextBlock StatusBar => this.FindControl<TextBlock>("StatusBar");

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
