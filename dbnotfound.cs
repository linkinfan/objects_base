using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BestWorker
{
    public partial class dbnotfound : Form
    {
        public string text = "";
        public dbnotfound()
        {
            InitializeComponent();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK) 
            {
                Properties.Settings.Default.server = openFileDialog1.FileName.Replace("\\","\\\\");
                Properties.Settings.Default.Save();
                Properties.Settings.Default.Upgrade();
                Application.Restart();
            }
        }

        private void dbnotfound_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}
