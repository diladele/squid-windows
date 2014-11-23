using System;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.ServiceProcess;
using System.Diagnostics;
using System.Collections.Generic;

namespace Diladele.Squid.Tray
{
    /// <summary>
    /// Tray app.
    /// </summary>
    public class SquidApplicationContext : ApplicationContext
    {
        private IContainer components;
        private NotifyIcon notifyIcon;
        private ServiceManager squidManager;
        private Dictionary<string, ToolStripMenuItem> items;

        private About about;
        private Help help;


        public SquidApplicationContext()
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
                Text = "Squid Server",
                Visible = true
            };

            notifyIcon.ContextMenuStrip.Opening += ContextMenuStrip_Opening;
            notifyIcon.MouseUp += notifyIcon_MouseUp;
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

        private void OnOpenSquidConfig(object sender, EventArgs e)
        {
            if (PredefinedPaths.InstallationFolder != string.Empty)
            {
                Process.Start("notepad.exe", PredefinedPaths.InstallationFolder + "\\etc\\squid\\squid.conf");
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

        private void OnExit(object sender, EventArgs e)
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
