using DoomSharp.Maui.Controls;
using Microsoft.Maui.Handlers;
using Microsoft.UI.Xaml.Input;
using Grid = Microsoft.UI.Xaml.Controls.Grid;

namespace DoomSharp.Maui.Handlers;

public partial class ExtendedGridHandler : ViewHandler<ExtendedGrid, Grid>
{
    public ExtendedGridHandler(IPropertyMapper mapper, CommandMapper? commandMapper = null) : base(mapper, commandMapper)
    {
    }

    protected override Grid CreatePlatformView()
    {
        return new Grid();
    }

    //protected override void ConnectHandler(Grid nativeView)
    //{
    //    base.ConnectHandler(nativeView);
    //    nativeView.KeyDown += OnKeyDown;
    //}

    //protected override void DisconnectHandler(Grid nativeView)
    //{
    //    base.DisconnectHandler(nativeView);
    //    nativeView.KeyDown -= OnKeyDown;
    //}

    //private void OnKeyDown(object sender, KeyRoutedEventArgs e)
    //{
    //    VirtualView.TriggerKeyDown(e.Key.ToString());
    //}
}