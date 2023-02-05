using iOverlay.Utility;
using iOverlay.Widgets;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace iOverlay
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            MiscFunctions.CreateMovableForm(this, controlPanel);
            Region = Region.FromHrgn(MiscFunctions.CreateRoundRectRgn(0, 0, Width, Height, 10, 10));
        }

        private void WidgetMouseEnter(object sender, EventArgs e)
        {
            if (sender is Panel)
            {
                (sender as Control).BackColor = Color.FromArgb(45, 45, 45);
            }
            else
            {
                (sender as Control).Parent.BackColor = Color.FromArgb(45, 45, 45);
            }
        }

        private void WidgetMouseLeave(object sender, EventArgs e)
        {
            if (sender is Panel)
            {
                (sender as Panel).BackColor = Color.FromArgb(35, 35, 35);
            }
            else
            {
                (sender as Control).Parent.BackColor = Color.FromArgb(35, 35, 35);
            }
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Save();
            Environment.Exit(0);
        }

        private void minimizeButton_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        // Settings Handler
        private void label4_Click(object sender, EventArgs e)
        {
            Settings settings = new Settings();
            settings.ShowDialog();
        }

        // Valorant Handler
        private void label3_Click(object sender, EventArgs e)
        {
            ValorantWidget valorantWidget = new ValorantWidget();
            valorantWidget.Show();
        }

        // Spotify Handler
        private void label2_Click(object sender, EventArgs e)
        {
            SpotifyWidget spotifyWidget = new SpotifyWidget();
            spotifyWidget.Show();
        }

    }
}
