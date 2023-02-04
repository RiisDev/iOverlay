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
    public partial class SpotifyWidget : Form
    {
        public SpotifyWidget()
        {
            InitializeComponent();
        }

        private void ApplyTheme(bool darkMode)
        {
            if (darkMode)
            {
                BackColor = Color.FromArgb(30, 30, 30);
                songNameLabel.ForeColor = Color.White;
                artistNameLabel.ForeColor = Color.White;
            }
            else
            {
                BackColor = Color.FromArgb(248, 248, 248);
                songNameLabel.ForeColor = Color.FromKnownColor(KnownColor.ControlText);
                artistNameLabel.ForeColor = Color.FromKnownColor(KnownColor.ControlText);
            }
        }

        private void DoPlayPauseToggle()
        {
            if (albumArt.Text == "\xE768")
            {
                albumArt.Text = "\xE769";
            }
            else
            {
                albumArt.Text = "\xE768";
            }
        }

        private void SpotifyWidget_Load(object sender, EventArgs e)
        {
            ApplyTheme(darkMode: true);
            Region = Region.FromHrgn(MiscFunctions.CreateRoundRectRgn(0, 0, Width, Height, 25, 25));
            MiscFunctions.CreateMovableForm(this, this);
            Opacity = .8;

        }
    }
}
