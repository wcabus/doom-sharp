using System;

namespace DoomSharp.Avalonia.Model;

public sealed class KeyDownEventArgs : EventArgs
{
    public string Key { get; }

    public KeyDownEventArgs(string key)
    {
        Key = key;
    }
}