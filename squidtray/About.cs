using System;
using System.Windows.Forms;

namespace Diladele.Squid.Tray
{
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.linkLabel1.LinkVisited = true;
            System.Diagnostics.Process.Start("http://www.squid-cache.org/");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.linkLabel1.LinkVisited = true;
            System.Diagnostics.Process.Start("http://www.diladele.com/");
        }
    }
}
