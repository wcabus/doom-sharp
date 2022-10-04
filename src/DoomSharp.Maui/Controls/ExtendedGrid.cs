using DoomSharp.Maui.Model;

namespace DoomSharp.Maui.Controls;

public class ExtendedGrid : Grid
{
    public event EventHandler<KeyDownEventArgs>? KeyPressed;

    public void TriggerKeyDown(string key)
    {
        KeyPressed?.Invoke(this, new KeyDownEventArgs());
    }
}