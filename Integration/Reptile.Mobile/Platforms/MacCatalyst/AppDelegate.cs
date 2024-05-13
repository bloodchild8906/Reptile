using Android.Runtime;

namespace Reptile;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    protected MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}