using Bunifu.UI.WinForms.BunifuButton;
using iOverlay.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace iOverlay.Widgets
{
    public partial class Settings : Form
    {

        private void HidePages()
        {
            foreach (Panel tabPage in tabPages.Controls) tabPage.Visible = false;
        }

        public Settings()
        {
            InitializeComponent();
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            HidePages();
            MiscFunctions.CreateMovableForm(this, controlPanel);

            (tabList.Controls[0] as BunifuButton2).PerformClick();

            valorantUserName.Text = Properties.Settings.Default.valorantUsername;
            valorantTagLine.Text = Properties.Settings.Default.valorantTagLine;
            valorantDarkModeToggle.Checked = Properties.Settings.Default.valorantDarkMode;
            showWinPctAndKD.Checked = Properties.Settings.Default.valorantShowPct;

            spotifyDarkModeToggle.Checked = Properties.Settings.Default.spotifyDarkMode;
        }

        private void applySpotifySettings_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.spotifyDarkMode = spotifyDarkModeToggle.Checked;
            Properties.Settings.Default.Save();
        }

        private void applyValorantSettings_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.valorantUsername = valorantUserName.Text;
            Properties.Settings.Default.valorantTagLine = valorantTagLine.Text;
            Properties.Settings.Default.valorantDarkMode = valorantDarkModeToggle.Checked;
            Properties.Settings.Default.valorantShowPct = showWinPctAndKD.Checked;
            Properties.Settings.Default.Save();
        }

        private void bunifuButton21_Click(object sender, EventArgs e)
        {
            HidePages();
            spotifySettingsPage.Visible = true;
        }

        private void valorantPageButton_Click(object sender, EventArgs e)
        {
            HidePages();
            valorantSettingsPage.Visible = true;
        }
    }
}
