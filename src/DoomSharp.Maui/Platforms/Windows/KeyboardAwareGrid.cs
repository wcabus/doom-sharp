using DoomSharp.Maui.Model;
using Microsoft.UI.Xaml.Input;

namespace DoomSharp.Maui.Platforms.Windows;

public class KeyboardAwareGrid : Microsoft.UI.Xaml.Controls.Grid
{
    public event EventHandler<KeyDownEventArgs>? KeyPressed;

    public KeyboardAwareGrid()
    {
        KeyDown += OnKeyDown;
    }

    private void OnKeyDown(object sender, KeyRoutedEventArgs e)
    {
        KeyPressed?.Invoke(this, new KeyDownEventArgs());
    }
}