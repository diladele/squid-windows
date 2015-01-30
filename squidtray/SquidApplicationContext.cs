/*
 * Copyright (C) 2015 Diladele B.V.
 *
 * Diladele software is distributed under GPL license.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.ServiceProcess;
using System.Threading;
using System.Windows.Forms;

namespace Diladele.Squid.Tray
{
    /// <summary>
    /// Tray app.
    /// </summary>
    public class SquidApplicationContext : ApplicationContext
    {
        private readonly ServiceManager squidManager;

        private IContainer components;
        private NotifyIcon notifyIcon;
        private Dictionary<string, ToolStripMenuItem> items;

        private About about;
        private Help help;
        private System.Threading.Timer updateTimer;

        public SquidApplicationContext(Form f) : base(f)
        {
            squidManager = new ServiceManager();
            items = new Dictionary<string, ToolStripMenuItem>();

            InitializeContext();
        }

        private void InitializeContext()
        {
            var bmp = new Bitmap(Properties.Resources.SquidIcon);
            var handle = bmp.GetHicon();
            components = new Container();
            notifyIcon = new NotifyIcon(components)
            {
                ContextMenuStrip = new ContextMenuStrip(),
                Icon = Icon.FromHandle(handle),
                Text = "Squid for Windows",
                Visible = true
            };

            notifyIcon.ContextMenuStrip.Opening += ContextMenuStrip_Opening;
            notifyIcon.MouseUp += notifyIcon_MouseUp;
            notifyIcon.BalloonTipClicked += notifyIcon_BalloonTipClicked;

            updateTimer = new System.Threading.Timer(CheckUpdate, null, TimeSpan.Zero, TimeSpan.FromHours(24));
        }

        private void ContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            e.Cancel = false;

            notifyIcon.ContextMenuStrip.Items.Clear();
            items.Clear();

            var item = new ToolStripMenuItem("O&pen Squid Configuration");
            item.Click += OnOpenSquidConfig;
            notifyIcon.ContextMenuStrip.Items.Add(item);

            item = new ToolStripMenuItem("&Open Squid Folder");
            item.Click += OnOpenSquidFolder;
            notifyIcon.ContextMenuStrip.Items.Add(item);

            notifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());

            item = new ToolStripMenuItem("&Start Squid Service");
            item.Click += OnStartSquid;
            var status = squidManager.GetStatus();
            if (status == ServiceControllerStatus.Running ||
                status == ServiceControllerStatus.StartPending ||
                squidManager.NotAvailable)
            {
                item.Enabled = false;
            }
            notifyIcon.ContextMenuStrip.Items.Add(item);
            items.Add("start", item);

            item = new ToolStripMenuItem("S&top Squid Service");
            item.Click += OnStopSquid;
            if (status == ServiceControllerStatus.Stopped ||
                status == ServiceControllerStatus.StopPending ||
                squidManager.NotAvailable)
            {
                item.Enabled = false;
            }
            notifyIcon.ContextMenuStrip.Items.Add(item);
            items.Add("stop", item);
            
            notifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());

            item = new ToolStripMenuItem("&Help");
            item.Click += OnHelp;
            notifyIcon.ContextMenuStrip.Items.Add(item);

            item = new ToolStripMenuItem("&About");
            item.Click += OnAbout;
            notifyIcon.ContextMenuStrip.Items.Add(item);

            notifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());

            item = new ToolStripMenuItem("&Exit");
            item.Click += OnExit;
            notifyIcon.ContextMenuStrip.Items.Add(item);
        }

        private void OnStartSquid(object sender, EventArgs e)
        {
            squidManager.StartService();
            items["start"].Enabled = false;
            if (!squidManager.NotAvailable)
            {
                items["stop"].Enabled = true;
            }
        }

        private void OnStopSquid(object sender, EventArgs e)
        {
            squidManager.StopService();
            items["stop"].Enabled = false;
            if (!squidManager.NotAvailable)
            {
                items["start"].Enabled = true;
            }
        }

        private void OnOpenSquidConfig(object sender, EventArgs notUsed)
        {
            if (PredefinedPaths.InstallationFolder != string.Empty)
            {
                var startInfo = new ProcessStartInfo("notepad.exe", PredefinedPaths.InstallationFolder + "\\etc\\squid\\squid.conf");
                startInfo.Verb = "runas";
                ThreadPool.QueueUserWorkItem(
                    (s) =>
                    {
                        try
                        {
                            Process.Start(startInfo);
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
        }

        private void OnOpenSquidFolder(object sender, EventArgs e)
        {
            if (PredefinedPaths.InstallationFolder != string.Empty)
            {
                Process.Start(PredefinedPaths.InstallationFolder);
            }
        }

        private void OnHelp(object sender, EventArgs e)
        {
            if (help == null)
            {
                help = new Help();
                help.FormClosed += OnHelpClosed;
                help.Show();
            }
            else
            {
                about.Activate();
            }
        }

        private void OnAbout(object sender, EventArgs e)
        {
            if (about == null)
            {
                about = new About();
                about.FormClosed += OnAboutClosed;
                about.Show();
            }
            else
            {
                about.Activate(); 
            }
        }

        internal void OnExit(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void OnAboutClosed(object sender, EventArgs e) 
        {
            this.about = null;
        }

        private void OnHelpClosed(object sender, EventArgs e)
        {
            this.help = null;
        }

        private void notifyIcon_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                MethodInfo mi = typeof(NotifyIcon).GetMethod("ShowContextMenu", BindingFlags.Instance | BindingFlags.NonPublic);
                mi.Invoke(notifyIcon, null);
            }
        }

        [DataContract]
        private class UpdaterProductInfo
        {
            [DataMember(Name = "current")]
            public string Version { get; set; }
        }

        [DataContract]
        private sealed class Settings
        {
            [DataMember(Name = "version")]
            public string Version { get; set; }
        }

        [DataContract]
        private sealed class ProductInfo
        {
            [DataMember(Name = "settings")]
            public Settings Settings { get; set; }
        }

        private void CheckUpdate(object state)
        {
            try
            {
                var currentVersionFile = PredefinedPaths.InstallationFolder + @"\bin\settings.json";
                var remoteVersionFile = PredefinedPaths.InstallationFolder + @"\var\log\squid.version";

                if (!File.Exists(currentVersionFile) || !File.Exists(remoteVersionFile))
                {
                    return;
                }

                ProductInfo current;
                using (var tmp = new FileStream(currentVersionFile, FileMode.Open, FileAccess.Read))
                {
                    current = (ProductInfo)new DataContractJsonSerializer(typeof(ProductInfo)).ReadObject(tmp);
                }

                UpdaterProductInfo remote;
                using (var tmp = new FileStream(remoteVersionFile, FileMode.Open, FileAccess.Read))
                {
                    remote = (UpdaterProductInfo)new DataContractJsonSerializer(typeof(UpdaterProductInfo)).ReadObject(tmp);
                }

                if (current.Settings.Version!= remote.Version)
                {
                    this.notifyIcon.ShowBalloonTip(
                        (int)TimeSpan.FromSeconds(15).TotalMilliseconds,
                        "Squid For Windows Update Available!",
                        "New version of the product '" + remote.Version + "' is now available. Please visit www.diladele.com",
                        ToolTipIcon.Info);
                }
            }
            catch (Exception)
            {
            }
        }

        void notifyIcon_BalloonTipClicked(object sender, EventArgs e)
        {
            try
            {
                Process.Start("iexplore.exe", "www.diladele.com");
            }
            catch (Exception)
            {
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
        }

        protected override void ExitThreadCore()
        {
            if (about != null)
            {
                about.Close();
                about = null;
            }

            notifyIcon.Visible = false;
            base.ExitThreadCore();
        }
    }
}
