using DoomSharp.Maui.Controls;
using DoomSharp.Maui.Handlers;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace DoomSharp.Maui
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseSkiaSharp()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })
                .ConfigureMauiHandlers(handlers =>
                {
                    handlers.AddHandler(typeof(ExtendedGrid), typeof(ExtendedGridHandler));
                });

            return builder.Build();
        }
    }
}