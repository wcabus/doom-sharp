using DoomSharp.Maui.ViewModels;

namespace DoomSharp.Maui
{
    public partial class App : Application
    {
        public static ViewModelLocator Locator { get; } = new();

        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }
    }
}