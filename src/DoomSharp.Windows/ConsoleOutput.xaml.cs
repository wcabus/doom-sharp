using System.Windows;
using System.Windows.Controls;

namespace DoomSharp.Windows
{
    /// <summary>
    /// Interaction logic for ConsoleOutput.xaml
    /// </summary>
    public partial class ConsoleOutput : Window
    {
        public ConsoleOutput()
        {
            InitializeComponent();
        }

        private void OnConsoleOutputChanged(object sender, TextChangedEventArgs e)
        {
            var box = e.Source as TextBox;
            box!.ScrollToEnd();
        }
    }
}
