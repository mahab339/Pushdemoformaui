using Foundation;
using PushDemo.iOS.Extensions;
using PushDemo.iOS.Services;
using PushDemo.Services;
using System.Diagnostics;
using UIKit;
using UserNotifications;

namespace PushDemo;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    IPushDemoNotificationActionService _notificationActionService;
    INotificationRegistrationService _notificationRegistrationService;
    IDeviceInstallationService _deviceInstallationService;

    IPushDemoNotificationActionService NotificationActionService
        => _notificationActionService ??
            (_notificationActionService =
            ServiceContainer.Resolve<IPushDemoNotificationActionService>());

    INotificationRegistrationService NotificationRegistrationService
        => _notificationRegistrationService ??
            (_notificationRegistrationService =
            ServiceContainer.Resolve<INotificationRegistrationService>());

    IDeviceInstallationService DeviceInstallationService
        => _deviceInstallationService ??
            (_deviceInstallationService =
            ServiceContainer.Resolve<IDeviceInstallationService>());

    public override bool FinishedLaunching(UIApplication app, NSDictionary options)
    {
        Bootstrap.Begin(() => new DeviceInstallationService());

        if (DeviceInstallationService.NotificationsSupported)
        {
            UNUserNotificationCenter.Current.RequestAuthorization(
                    UNAuthorizationOptions.Alert |
                    UNAuthorizationOptions.Badge |
                    UNAuthorizationOptions.Sound,
                    (approvalGranted, error) =>
                    {
                        if (approvalGranted && error == null)
                            RegisterForRemoteNotifications();
                    });
        }


        using (var userInfo = options?.ObjectForKey(
            UIApplication.LaunchOptionsRemoteNotificationKey) as NSDictionary)
            ProcessNotificationActions(userInfo);

        return base.FinishedLaunching(app, options);
    }


    [Export("application:didReceiveRemoteNotification:fetchCompletionHandler:")]
    public void DidReceiveRemoteNotification(UIKit.UIApplication application, NSDictionary userInfo, Action<UIKit.UIBackgroundFetchResult> completionHandler)
    {
        ProcessNotificationActions(userInfo);
    }

    [Export("application:didRegisterForRemoteNotificationsWithDeviceToken:")]
    public void RegisteredForRemoteNotifications(UIKit.UIApplication application, NSData deviceToken)
    {
        CompleteRegistrationAsync(deviceToken).ContinueWith((task)
                =>
        { if (task.IsFaulted) throw task.Exception; });
    }

    [Export("application:didFailToRegisterForRemoteNotificationsWithError:")]
    public void FailedToRegisterForRemoteNotifications(UIKit.UIApplication application, NSError error)
    {
        Debug.WriteLine(error.Description);
    }

    void RegisterForRemoteNotifications()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            var pushSettings = UIUserNotificationSettings.GetSettingsForTypes(
                UIUserNotificationType.Alert |
                UIUserNotificationType.Badge |
                UIUserNotificationType.Sound,
                new NSSet());

            UIApplication.SharedApplication.RegisterUserNotificationSettings(pushSettings);
            UIApplication.SharedApplication.RegisterForRemoteNotifications();
        });
    }

    Task CompleteRegistrationAsync(NSData deviceToken)
    {
        DeviceInstallationService.Token = deviceToken.ToHexString();

        return NotificationRegistrationService.RefreshRegistrationAsync();
    }

    void ProcessNotificationActions(NSDictionary userInfo)
    {
        if (userInfo == null)
            return;

        try
        {
            var actionValue = userInfo.ObjectForKey(new NSString("action")) as NSString;

            if (!string.IsNullOrWhiteSpace(actionValue?.Description))
                NotificationActionService.TriggerAction(actionValue.Description);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

}
