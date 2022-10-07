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

        protected override void OnStart()
        {
            Microsoft.Maui.Handlers.WindowHandler.Mapper.AppendToMapping("KeyboardSupport", (handler, view) =>
            {
#if WINDOWS
                handler.PlatformView.Content.KeyDown += (sender, args) =>
                {

                };
#endif
            });
            base.OnStart();
        }
    }
}