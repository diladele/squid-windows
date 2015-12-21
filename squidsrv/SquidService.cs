/*
 * Copyright (C) 2015 Diladele B.V.
 *
 * Diladele Squid Installer software is distributed under GPL license.
 */

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.ServiceProcess;
using Diladele.Squid.Tray;

namespace Diladele.Squid.Service
{
    public partial class SquidService : ServiceBase
    {
        private Process squid;
        private System.Threading.Timer timer;
        private System.Threading.Timer updateTimer;
        private readonly object locker;

        public SquidService()
        {
            InitializeComponent();

            if (!EventLog.SourceExists("Squid Service Source"))
            {
                EventLog.CreateEventSource("Squid Service Source", "Squid Service Log");
            }

            this.eventLog.Source = "Squid Service Source";
            this.eventLog.Log = "Squid Service Log";
            this.locker = new object();
        }

        protected override void OnStart(string[] args)
        {
            this.eventLog.WriteEntry("Squid is starting...", EventLogEntryType.Information);

            StartSquidProcess();

            this.timer = new System.Threading.Timer(this.OnTimer, null, TimeSpan.Zero, TimeSpan.FromSeconds(20));
            this.updateTimer = new System.Threading.Timer(this.OnUpdateTimer, null, TimeSpan.Zero, TimeSpan.FromHours(6));
        }

        protected override void OnStop()
        {
            this.eventLog.WriteEntry("Squid is stopping...", EventLogEntryType.Information);

            if (timer != null)
            {
                timer.Dispose();
                timer = null;
            }

            if (updateTimer != null)
            {
                updateTimer.Dispose();
                updateTimer = null;
            }

            lock(this.locker)
            {
                this.Kill(this.squid);
                this.squid = null;
            }

            var processes = Process.GetProcessesByName("squid");
            foreach (var p in processes)
            {
                this.Kill(p);
            }

            this.eventLog.WriteEntry("Squid stopped.", EventLogEntryType.Information);
        }
        private void OnTimer(object state)
        {
            lock (this.locker)
            {
                var processes = Process.GetProcessesByName("squid");
                if (processes == null || processes.Length == 0)
                {
                    eventLog.WriteEntry("Cannot find a squid process. Trying to start it...", EventLogEntryType.Information);
                    this.squid = null;
                    StartSquidProcess();
                }
            }
        }

        private void OnUpdateTimer(object state)
        {
            lock (this.updateTimer)
            {
                try
                {
                    var remoteVersionFile = PredefinedPaths.InstallationFolder + @"\var\log\squid.version";

                    var req = (HttpWebRequest)WebRequest.Create("https://defs.diladele.com/squid/version/windows");
                    req.UserAgent = "Squid3.5.12/" + Environment.OSVersion.VersionString + "/x64 (win;0-0-0-0)";
                    req.Headers.Add("Authorization", "Token 0000000000000000");

                    WebResponse resp = req.GetResponse();
                    StreamReader sr = new StreamReader(resp.GetResponseStream());
                    var result = sr.ReadToEnd().Trim();

                    using (StreamWriter file = new StreamWriter(remoteVersionFile))
                    {
                        file.WriteLine(result);
                    }
                }
                catch (Exception e)
                {
                    eventLog.WriteEntry(e.Message, EventLogEntryType.Error);
                }
            }
        }

        private void StartSquidProcess()
        {
            lock (this.locker)
            {
                this.squid = new Process();
                this.squid.StartInfo.FileName = PredefinedPaths.InstallationFolder + @"\bin\squid.exe";
                this.squid.StartInfo.CreateNoWindow = true;

                this.squid.Start();
                this.eventLog.WriteEntry(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Squid started: process id '{0}'.",
                        this.squid.Id));
            }
        }

        private void Kill(Process p)
        {
            try
            {
                if (!p.HasExited)
                {
                    p.Kill();
                }
            }
            catch (Exception)
            {
                this.eventLog.WriteEntry(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Could not terminate squid process '{0}'.",
                        p.Id),
                    EventLogEntryType.Warning);
            }
        }
    }
}
