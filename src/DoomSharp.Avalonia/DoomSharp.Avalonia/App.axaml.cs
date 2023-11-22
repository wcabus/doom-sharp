using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using DoomSharp.Avalonia.Data;
using DoomSharp.Avalonia.ViewModels;
using DoomSharp.Avalonia.Views;
using DoomSharp.Core.Data;
using DoomSharp.Core;

namespace DoomSharp.Avalonia
{
    public partial class App : Application
    {
        public static ViewModelLocator Locator { get; set; } = new();

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow();
            }
            else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
            {
                singleViewPlatform.MainView = new MainView();
            }


            WadFileCollection.Init(new WadStreamProvider());
            DoomGame.SetOutputRenderer(MainViewModel.Instance);
            base.OnFrameworkInitializationCompleted();
        }
    }
}