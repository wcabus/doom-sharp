using System.Windows;
using DoomSharp.Core;
using DoomSharp.Core.Data;
using DoomSharp.Core.Internals;
using DoomSharp.Windows.Data;
using DoomSharp.Windows.ViewModels;

namespace DoomSharp.Windows
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly SoundDriver _soundDriver = new();

        public App()
        {
            Exit += App_Exit;

            DoomGame.SetConsole(ConsoleViewModel.Instance);
            DoomGame.SetOutputRenderer(MainViewModel.Instance);
            DoomGame.SetSoundDriver(_soundDriver);
            WadFileCollection.Init(new WadStreamProvider());
        }

        private void App_Exit(object sender, ExitEventArgs e)
        {
            _soundDriver.Dispose();
        }
    }
}
