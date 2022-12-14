using System;
using PushDemo.Services;

namespace PushDemo
{
    public static class Bootstrap
    {
        public static void Begin(Func<IDeviceInstallationService> deviceInstallationService)
        {
            ServiceContainer.Register(deviceInstallationService);
            
        }
    }
}