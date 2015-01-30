/*
 * Copyright (C) 2015 Diladele B.V.
 *
 * Diladele software is distributed under GPL license.
 */

using System;
using System.Windows.Forms;

namespace Diladele.Squid.Tray
{
    public partial class Help : Form
    {
        public Help()
        {
            InitializeComponent();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.linkLabel1.LinkVisited = true;
            System.Diagnostics.Process.Start("http://www.squid-cache.org/Doc/");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
