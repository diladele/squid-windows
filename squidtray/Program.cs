/*
 * Copyright (C) 2015 Diladele B.V.
 *
 * Diladele software is distributed under GPL license.
 */

using System;
using System.Windows.Forms;

namespace Diladele.Squid.Tray
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                var mainForm = new Form();
                mainForm.Load += (object sender, EventArgs e) =>
                {
                    mainForm.Visible = false;
                    mainForm.ShowInTaskbar = false;
                };

                mainForm.FormClosed += (object sender, FormClosedEventArgs e) =>
                {
                    Application.Exit();
                };

                using (var applicationContext = new SquidApplicationContext(mainForm))
                {
                    Application.Run(applicationContext);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Squid Terminated Unexpectedly",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
