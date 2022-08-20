using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using DoomSharp.Core;

namespace DoomSharp.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        protected async override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            var consoleOutputWindow = new ConsoleOutput();
            consoleOutputWindow.Show();

            RenderOptions.SetBitmapScalingMode(RenderTarget, BitmapScalingMode.NearestNeighbor);
            RenderOptions.SetEdgeMode(RenderTarget, EdgeMode.Aliased);

            await Task.Run(async () =>
            {
                await DoomGame.Instance.RunAsync();
            });
        }
    }
}
