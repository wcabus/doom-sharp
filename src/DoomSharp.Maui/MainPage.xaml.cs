using DoomSharp.Core;

namespace DoomSharp.Maui
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            await Task.Run(async () =>
            {
                await DoomGame.Instance.RunAsync(GameMode.Shareware, "DOOM1.WAD");
            });
        }
    }
}