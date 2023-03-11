using Android.App;
using Android.Content.PM;
using Android.OS;
using DoomSharp.Core;
using DoomSharp.Core.Internals;

namespace DoomSharp.Maui
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ScreenOrientation = ScreenOrientation.Landscape, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            Java.Lang.JavaSystem.LoadLibrary("fmod");
            Org.Fmod.FMOD.Init(this);

            DoomGame.SetSoundDriver(new SoundDriver());

            base.OnCreate(savedInstanceState);
        }
    }
}