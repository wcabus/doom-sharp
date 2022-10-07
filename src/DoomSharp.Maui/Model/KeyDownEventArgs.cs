namespace DoomSharp.Maui.Model;

public sealed class KeyDownEventArgs : EventArgs
{
    public string Key { get; }

    public KeyDownEventArgs(string key)
    {
        Key = key;
    }
}