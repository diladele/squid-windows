using System;
using System.ServiceProcess;
using Microsoft.Win32;

namespace Diladele.Squid.Tray
{
    internal sealed class ServiceManager : IDisposable
    {
        private ServiceController controller;

        public ServiceManager()
        {
            this.controller = new ServiceController(Constants.ServiceName);
        }

        public void StopService()
        {
            if (Exists && controller.CanStop)
            {
                controller.Stop();
            }
        }

        public void StartService()
        {
            if (Exists)
            {
                controller.Start();
            }
        }

        public ServiceControllerStatus GetStatus()
        {
            if (!Exists)
            {
                return ServiceControllerStatus.Stopped;
            }

            controller.Refresh();
            return controller.Status;
        }

        public bool NotAvailable
        {
            get
            {
                if (!Exists)
                {
                    return true;
                }

                const string basepathStr = @"System\CurrentControlSet\services\";
                string subKeyStr = basepathStr + Constants.ServiceName;

                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(subKeyStr))
                {
                    return (int)key.GetValue("Start") == 4;
                }
            }
        }

        public void Dispose()
        {
            if (controller != null)
            {
                controller.Dispose();
                controller = null;
            }
        }

        private bool Exists
        {
            get
            {
                ServiceController[] services = ServiceController.GetServices();
                foreach(var s in services)
                {
                    if(s.ServiceName == Constants.ServiceName)
                    {
                        return true;
                    }
                }

                return false;
            }
        }
    }
}
