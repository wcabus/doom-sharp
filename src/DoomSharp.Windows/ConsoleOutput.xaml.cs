using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
            box!.CaretIndex = box.Text.Length;
        }
    }
}
