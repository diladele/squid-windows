/*
 * Copyright (C) 2015 Diladele B.V.
 *
 * Diladele software is distributed under GPL license.
 */

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;
using System.Windows.Forms;
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
                var startInfo = new ProcessStartInfo("net", "stop squidsrv");
                startInfo.Verb = "runas";
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                ThreadPool.QueueUserWorkItem(
                    (s) =>
                    {
                        try
                        {
                            var p = Process.Start(startInfo);
                            p.WaitForExit();
                            if (p.ExitCode != 0)
                            {
                                MessageBox.Show(
                                    "Cannot stop squid service: error code '" + p.ExitCode + "'.",
                                    "Error",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                            }
                        }
                        catch (Win32Exception e)
                        {
                            MessageBox.Show(
                                e.Message,
                                e.NativeErrorCode == Constants.OperationCancelled
                                    ? "Warning" : "Error",
                                MessageBoxButtons.OK,
                                e.NativeErrorCode == Constants.OperationCancelled
                                    ? MessageBoxIcon.Warning : MessageBoxIcon.Error);
                        }
                    },
                    null);
            }
            else
            {
                MessageBox.Show(
                    "Squid service does not exist.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        public void StartService()
        {
            if (Exists)
            {
                var startInfo = new ProcessStartInfo("net", "start squidsrv");
                startInfo.Verb = "runas";
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                ThreadPool.QueueUserWorkItem(
                    (s) =>
                    {
                        try
                        {
                            var p = Process.Start(startInfo);
                            p.WaitForExit();
                            if (p.ExitCode != 0)
                            {
                                MessageBox.Show(
                                    "Cannot start squid service: error code '" + p.ExitCode + "'.",
                                    "Error",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                            }
                        }
                        catch (Win32Exception e)
                        {
                            MessageBox.Show(
                                e.Message,
                                e.NativeErrorCode == Constants.OperationCancelled
                                    ? "Warning" : "Error",
                                MessageBoxButtons.OK,
                                e.NativeErrorCode == Constants.OperationCancelled
                                    ? MessageBoxIcon.Warning : MessageBoxIcon.Error);
                        }
                    }, null);
            }
            else
            {
                MessageBox.Show(
                    "Squid service does not exist.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
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
                foreach (var s in services)
                {
                    if (s.ServiceName == Constants.ServiceName)
                    {
                        return true;
                    }
                }

                return false;
            }
        }
    }
}
