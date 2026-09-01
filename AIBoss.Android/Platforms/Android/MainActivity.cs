using Android.App;
using Android.Content.PM;
using Android.OS;
using Microsoft.Maui;

namespace AIBoss;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, Exported = true,
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
[IntentFilter(
    new[] { Android.Content.Intent.ActionMain },
    Categories = new[]
    {
        Android.Content.Intent.CategoryLauncher,
        Android.Content.Intent.CategoryLeanbackLauncher
    })]
public class MainActivity : MauiAppCompatActivity
{
}
