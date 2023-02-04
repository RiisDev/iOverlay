using iOverlay.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace iOverlay.Widgets
{
    public partial class ValorantWidget : Form
    {

        private string GetUserRR(string Name, string Tag)
        {
            return new WebClient().DownloadString($"https://api.kyroskoh.xyz/valorant/v1/mmr/NA/{Name}/{Tag}?show=rronly&display=0").Replace("RR", "");
        }

        public ValorantWidget()
        {
            InitializeComponent();
        }

        private void ValorantWidget_Load(object sender, EventArgs e)
        {
            MiscFunctions.CreateMovableForm(this, this);
            Region = Region.FromHrgn(MiscFunctions.CreateRoundRectRgn(0, 0, Width, Height, 25, 25));
        }
    }
}
