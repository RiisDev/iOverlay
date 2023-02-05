using Bunifu.UI.WinForms;
using iOverlay.Properties;
using iOverlay.Utility;
using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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
        WebClient client = new WebClient();
        private Dictionary<string, Image> RankPictures = new Dictionary<string, Image>()
        {
            { "Unranked", Resources.Unranked },
            { "Iron 1", Resources.Iron_1 },
            { "Iron 2", Resources.Iron_2 },
            { "Iron 3", Resources.Iron_3 },
            { "Bronze 1", Resources.Bronze_1 },
            { "Bronze 2", Resources.Bronze_2 },
            { "Bronze 3", Resources.Bronze_3 },
            { "Silver 1", Resources.Silver_1 },
            { "Silver 2", Resources.Silver_2 },
            { "Silver 3", Resources.Silver_3 },
            { "Gold 1", Resources.Gold_1 },
            { "Gold 2", Resources.Gold_2 },
            { "Gold 3", Resources.Gold_3 },
            { "Platinum 1", Resources.Platinum_1},
            { "Platinum 2", Resources.Platinum_2},
            { "Platinum 3", Resources.Platinum_3},
            { "Diamond 1", Resources.Diamond_1},
            { "Diamond 2", Resources.Diamond_2},
            { "Diamond 3", Resources.Diamond_3},
            { "Ascendant 1", Resources.Ascendant_1},
            { "Ascendant 2", Resources.Ascendant_2},
            { "Ascendant 3", Resources.Ascendant_3},
            { "Immortal 1", Resources.Immortal_1},
            { "Immortal 2", Resources.Immortal_2},
            { "Immortal 3", Resources.Immortal_3},
            { "Radiant", Resources.Radiant}
        };

        private string GetParsedRiotName()
        {
            return $"{Properties.Settings.Default.valorantUsername.Replace(" ", "%20")}%23{Properties.Settings.Default.valorantTagLine}";
        }

        private int GetUserRR()
        {
            return int.Parse(client.DownloadString($"https://api.kyroskoh.xyz/valorant/v1/mmr/NA/{Properties.Settings.Default.valorantUsername}/{Properties.Settings.Default.valorantTagLine}?show=rronly&display=0").Replace("RR.", ""));
        }

        private void DoTheme(bool isDarkMode)
        {
            if (!isDarkMode)
            {
                BackColor = Color.White;
                foreach (Control control in Controls)
                {
                    if (control is Label)
                    {
                        control.ForeColor = Color.FromKnownColor(KnownColor.ControlText);
                    }
                }
            }
        }

        private void UpdateRR()
        {
            int CurrentRR = GetUserRR();
            int ProgressValue = rankRRProgress.Value;
            int step = CurrentRR - ProgressValue;
            rankRRProgress.Invoke(new Action(() =>
            {
                rankRRProgress.Value += step;
            }));
        }

        public ValorantWidget()
        {
            InitializeComponent();
            DoTheme(Properties.Settings.Default.valorantDarkMode);
        }

        private async void ValorantWidget_Load(object sender, EventArgs e)
        {

            MiscFunctions.CreateMovableForm(this, this);
            Region = Region.FromHrgn(MiscFunctions.CreateRoundRectRgn(0, 0, Width, Height, 25, 25));
            Opacity = .9;

            await webView.EnsureCoreWebView2Async();

            webView.CoreWebView2.NavigationCompleted += async (saef, esr) =>
            {
                string Data = await webView.CoreWebView2.ExecuteScriptAsync("document.body.innerText");
                Data = Data.Substring(1, Data.Length - 2).Replace("\\", "");
                JToken JsonData = JToken.Parse(Data);

                winPctLabel.Text = JsonData["data"]["segments"][0]["stats"]["matchesWinPct"].Value<string>("displayValue");
                KDLabel.Text = JsonData["data"]["segments"][0]["stats"]["kDRatio"].Value<string>("displayValue");
                rankNameLabel.Text = JsonData["data"]["segments"][0]["stats"]["rank"]["metadata"].Value<string>("tierName");
                rankIcon.Image = RankPictures[JsonData["data"]["segments"][0]["stats"]["rank"]["metadata"].Value<string>("tierName")];

                DoTheme(Properties.Settings.Default.valorantDarkMode);
            };

            UpdateRR();
            webView.CoreWebView2.Navigate($"https://api.tracker.gg/api/v2/valorant/standard/profile/riot/{GetParsedRiotName()}?forceCollect=true");

            await Task.Run(async () =>
            {
                for (; ; )
                {
                    await Task.Delay(60000);
                    UpdateRR();
                    Debug.WriteLine("Updating RR...");
                }
            });

            await Task.Run(async () =>
            {
                for (; ; )
                {
                    await Task.Delay(500000);
                    webView.CoreWebView2.Navigate($"https://api.tracker.gg/api/v2/valorant/standard/profile/riot/{GetParsedRiotName()}?forceCollect=true");
                    Debug.WriteLine("Rank Data RR...");
                }
            });
        }
    }
}
