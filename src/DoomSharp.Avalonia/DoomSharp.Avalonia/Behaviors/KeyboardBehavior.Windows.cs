using DoomSharp.Avalonia.Model;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Window = Microsoft.UI.Xaml.Window;

namespace DoomSharp.Avalonia.Behaviors;

public partial class KeyboardBehavior : PlatformBehavior<VisualElement, UIElement>
{
    protected override void OnAttachedTo(VisualElement bindable, UIElement platformView)
    {
        //We need to look for the outermost native Microsoft UI element,
        //since that is the only one with keydown and keyup events that don't get swallowed by MAUI

        //Find the first scrollviewer in the visual tree
        //We look for scrollviewer because that is a Microsoft UI control.
        //We could do grid as well but there are many grids in the visual tree and only a few scrollviewers
        DependencyObject window = GetParent<ScrollViewer>(platformView);
        var parent = VisualTreeHelper.GetParent(window);

        //Keep looking for parents until we find the outermost control
        //This is always a Microsoft UI control, not MAUI
        while (parent != null)
        {
            window = parent;
            parent = VisualTreeHelper.GetParent(window);
        }

        //cast it to a UIElement so we can get to the key events
        var root = window as UIElement;

        if (root == null)
        {
            return;
        }

        //Attach keydown and keyup events
        root.KeyDown += (sender, args) =>
        {
            KeyDown.Invoke(this, new KeyDownEventArgs(args.Key.ToString()));
        };

        root.KeyUp += (sender, args) =>
        {
            KeyUp.Invoke(this, new KeyDownEventArgs(args.Key.ToString()));
        };
    }

    public static T GetParent<T>(DependencyObject element)
    {
        if (element is T typedElement)
        {
            return typedElement;
        }

        var parent = VisualTreeHelper.GetParent(element);
        if (parent != null)
        {
            return GetParent<T>(parent);
        }

        return default;
    }
}