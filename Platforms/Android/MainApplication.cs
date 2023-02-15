using Android.App;
using Android.Runtime;
using PushDemo.Droid.Services;

namespace PushDemo;

[Application]
public class MainApplication : MauiApplication
{
	public MainApplication(IntPtr handle, JniHandleOwnership ownership)
		: base(handle, ownership)
	{
        Bootstrap.Begin(() => new DeviceInstallationService());
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
