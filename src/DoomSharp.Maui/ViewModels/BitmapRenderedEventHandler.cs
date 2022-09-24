using SkiaSharp;

namespace DoomSharp.Maui.ViewModels
{
    public delegate void BitmapRenderedEventHandler(object? sender, BitmapRenderedEventArgs e);

    public class BitmapRenderedEventArgs : EventArgs
    {
        public BitmapRenderedEventArgs(SKBitmap bitmap)
        {
            Bitmap = bitmap;
        }

        public SKBitmap Bitmap { get; }
    }
}
