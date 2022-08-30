using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using DoomSharp.Core;
using DoomSharp.Core.Input;
using DoomSharp.Windows.ViewModels;

namespace DoomSharp.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private double _lastMouseX;
        private double _lastMouseY;
        private bool _mouseMoved = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            var consoleOutputWindow = new ConsoleOutput();
            consoleOutputWindow.Show();

            RenderOptions.SetBitmapScalingMode(RenderTarget, BitmapScalingMode.NearestNeighbor);
            RenderOptions.SetEdgeMode(RenderTarget, EdgeMode.Aliased); 
            
            Task.Run(() => DoomGame.Instance.Run());
        }

        private void HandleKeyDown(object sender, KeyEventArgs e)
        {
            MainViewModel.Instance.AddEvent(new InputEvent(EventType.KeyDown, TranslateKey(e), 0, 0));
        }

        private void HandleKeyUp(object sender, KeyEventArgs e)
        {
            MainViewModel.Instance.AddEvent(new InputEvent(EventType.KeyUp, TranslateKey(e), 0, 0));
        }

        private void HandleMouseMove(object sender, MouseEventArgs e)
        {
            var clickState = e.LeftButton == MouseButtonState.Pressed ? 1 : 0;
            clickState += e.RightButton == MouseButtonState.Pressed ? 2 : 0;
            clickState += e.MiddleButton == MouseButtonState.Pressed ? 4 : 0;

            var position = e.GetPosition(this);
            var moveX = (int)(position.X - _lastMouseX) << 2;
            var moveY = (int)(_lastMouseY - position.Y) << 2;

            if (moveX > 0 || moveY > 0)
            {
                _lastMouseX = position.X;
                _lastMouseY = position.Y;
                
                MainViewModel.Instance.AddEvent(new InputEvent(EventType.Mouse, clickState, moveX, moveY));
            }
        }

        private void HandleMouseDown(object sender, MouseButtonEventArgs e)
        {
            var clickState = e.LeftButton == MouseButtonState.Pressed ? 1 : 0;
            clickState += e.RightButton == MouseButtonState.Pressed ? 2 : 0;
            clickState += e.MiddleButton == MouseButtonState.Pressed ? 4 : 0;
            
            MainViewModel.Instance.AddEvent(new InputEvent(EventType.Mouse, clickState, 0, 0));
        }

        private void HandleMouseUp(object sender, MouseButtonEventArgs e)
        {
            var clickState = e.LeftButton == MouseButtonState.Pressed ? 1 : 0;
            clickState += e.RightButton == MouseButtonState.Pressed ? 2 : 0;
            clickState += e.MiddleButton == MouseButtonState.Pressed ? 4 : 0;

            MainViewModel.Instance.AddEvent(new InputEvent(EventType.Mouse, clickState, 0, 0));
        }

        private int TranslateKey(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Right:
                    return (int)Keys.RightArrow;
                case Key.Left:
                    return (int)Keys.LeftArrow;
                case Key.Up:
                    return (int)Keys.UpArrow;
                case Key.Down:
                    return (int)Keys.DownArrow;
                case Key.Escape:
                    return (int)Keys.Escape;
                case Key.Enter:
                    return (int)Keys.Enter;
                case Key.Tab:
                    return (int)Keys.Tab;

                case Key.F1:
                    return (int)Keys.F1;
                case Key.F2:
                    return (int)Keys.F2;
                case Key.F3:
                    return (int)Keys.F3;
                case Key.F4:
                    return (int)Keys.F4;
                case Key.F5:
                    return (int)Keys.F5;
                case Key.F6:
                    return (int)Keys.F6;
                case Key.F7:
                    return (int)Keys.F7;
                case Key.F8:
                    return (int)Keys.F8;
                case Key.F9:
                    return (int)Keys.F9;
                case Key.F10:
                    return (int)Keys.F10;
                case Key.F11:
                    return (int)Keys.F11;
                case Key.F12:
                    return (int)Keys.F12;

                case Key.Back:
                    return (int)Keys.Backspace;
                case Key.Pause:
                    return (int)Keys.Pause;

                case Key.OemPlus:
                    return (int)Keys.Equals;
                case Key.OemMinus:
                    return (int)Keys.Minus;

                case Key.RightShift:
                    return (int)Keys.RShift;
                case Key.RightCtrl:
                    return (int)Keys.RCtrl;
                case Key.RightAlt:
                    return (int)Keys.RAlt;
                case Key.LeftAlt:
                    return (int)Keys.LAlt;

                case Key.Space:
                    return 32;

                case >= Key.A and <= Key.Z:
                    return 'A' + (e.Key - Key.A);

                default:
                    return (int)e.Key;
            }
        }
    }
}
