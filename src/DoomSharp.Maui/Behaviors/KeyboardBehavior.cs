using System.Runtime.Versioning;
using DoomSharp.Maui.Model;

namespace DoomSharp.Maui.Behaviors;

/// <summary>
/// <see cref="PlatformBehavior{TView,TPlatformView}"/> that enables hardware keyboard presses
/// </summary>
[UnsupportedOSPlatform("Android"), UnsupportedOSPlatform("iOS")]
public partial class KeyboardBehavior
{
    public event EventHandler<KeyDownEventArgs> KeyDown;
    public event EventHandler<KeyDownEventArgs> KeyUp;
}