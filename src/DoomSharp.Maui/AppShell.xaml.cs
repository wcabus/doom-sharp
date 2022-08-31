using DoomSharp.Core;
using DoomSharp.Core.Data;
using DoomSharp.Maui.Data;
using DoomSharp.Maui.ViewModels;

namespace DoomSharp.Maui
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            WadFileCollection.Init(new WadStreamProvider());
            DoomGame.SetConsole(ConsoleViewModel.Instance);
            DoomGame.SetOutputRenderer(MainViewModel.Instance);
        }
    }
}