using iOverlay.Properties;
using iOverlay.Utility;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Net;
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

        private Tuple<string, int> GetUserRR()
        {
            Console.WriteLine($"https://api.kyroskoh.xyz/valorant/v1/mmr/NA/{Properties.Settings.Default.valorantUsername}/{Properties.Settings.Default.valorantTagLine}");
            string returnedData = client.DownloadString($"https://api.kyroskoh.xyz/valorant/v1/mmr/NA/{Properties.Settings.Default.valorantUsername}/{Properties.Settings.Default.valorantTagLine}");
            if (returnedData == null) return new Tuple<string, int>("Unranked", 0);

            int rrCount = int.Parse(returnedData.Substring(returnedData.IndexOf("-") + 1).Replace("RR", ""));
            string rankName = returnedData.Substring(0,returnedData.IndexOf("-")-1);

            return new Tuple<string, int>(rankName, rrCount);
        }

        private void DoTheme(bool isDarkMode)
        {
            BackColor = isDarkMode ? Color.Black : Color.White;

            foreach (Control control in Controls) control.ForeColor = isDarkMode ? Color.White : Color.FromKnownColor(KnownColor.ControlText);
        }


        private void UpdateRR()
        {
            Tuple<string, int> rankReturn = GetUserRR();
            int rrCount = rankReturn.Item2;
            string rankName = rankReturn.Item1;
            int step = rrCount - rankRRProgress.Value;

            rankRRProgress.Invoke((Action)(() => rankRRProgress.Value += step));
            rankNameLabel.Invoke((Action)(() => rankNameLabel.Text = rankName));
            rankNameLabel.Invoke((Action)(() => rankIcon.Image = RankPictures[rankName]));
        }

        public ValorantWidget()
        {
            InitializeComponent();
            DoTheme(Properties.Settings.Default.valorantDarkMode);
        }

        private async void ValorantWidget_Load(object sender, EventArgs e)
        {
            DoTheme(Properties.Settings.Default.valorantDarkMode);
            MiscFunctions.CreateMovableForm(this, this);
            Region = Region.FromHrgn(MiscFunctions.CreateRoundRectRgn(0, 0, Width, Height, 25, 25));
            Opacity = .9;

            await webView.EnsureCoreWebView2Async();

            webView.CoreWebView2.NavigationCompleted += async (saef, esr) =>
            {
                string bodyInnerHtml = await webView.CoreWebView2.ExecuteScriptAsync("document.body.innerText");
                bodyInnerHtml = bodyInnerHtml.Substring(1, bodyInnerHtml.Length - 2).Replace("\\", "");
                JToken jsonData = JToken.Parse(bodyInnerHtml);
                JToken userStats = jsonData["data"]["segments"][0]["stats"];

                winPctLabel.Text = userStats["matchesWinPct"].Value<string>("displayValue");
                KDLabel.Text = userStats["kDRatio"].Value<string>("displayValue");

            };

            UpdateRR();
            webView.CoreWebView2.Navigate($"https://api.tracker.gg/api/v2/valorant/standard/profile/riot/{GetParsedRiotName()}?forceCollect=true");

            await Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(60000);
                    UpdateRR();
                    Debug.WriteLine("Updating RR...");
                }
            });

            await Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(500000);
                    webView.CoreWebView2.Navigate($"https://api.tracker.gg/api/v2/valorant/standard/profile/riot/{GetParsedRiotName()}?forceCollect=true");
                    Debug.WriteLine("Rank Data RR...");
                }
            });
        }
    }
}
