#if WINDOWS
using DoomSharp.Core.Graphics;
using Microsoft.Maui.Controls.PlatformConfiguration.GTKSpecific;
using Microsoft.Maui.Handlers;
using PlatformView = Microsoft.UI.Xaml.Controls.Grid;
#elif IOS || MACCATALYST || ANDROID
using PlatformView = Microsoft.Maui.Controls.Grid;
#elif (NETSTANDARD || !PLATFORM) || (NET6_0 && !IOS && !ANDROID)
using PlatformView = System.Object;
#endif
using DoomSharp.Maui.Controls;

namespace DoomSharp.Maui.Handlers;

public partial class ExtendedGridHandler
{
    #if WINDOWS
    public static IPropertyMapper<ExtendedGrid, ExtendedGridHandler> PropertyMapper = new PropertyMapper<ExtendedGrid, ExtendedGridHandler>(ViewMapper);

    public ExtendedGridHandler() : base(PropertyMapper)
    {
    }
#endif
}