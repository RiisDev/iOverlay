using iOverlay.Utility;
using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace iOverlay.Widgets
{
    public partial class SpotifyWidget : Form
    {
        private string SpotifyAuth = "";
        private string CurrentUrl = "";

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

        private async void SpotifyWidget_Load(object sender, EventArgs e)
        {
            ApplyTheme(darkMode: Properties.Settings.Default.spotifyDarkMode);
            Region = Region.FromHrgn(MiscFunctions.CreateRoundRectRgn(0, 0, Width, Height, 25, 25));
            MiscFunctions.CreateMovableForm(this, this);
            Opacity = .8;

            await webView.EnsureCoreWebView2Async();

            webView.CoreWebView2.NavigationCompleted += async (saef, esr) =>
            {
                string WebPage = await webView.CoreWebView2.ExecuteScriptAsync("document.documentElement.outerHTML;");

                if (WebPage.ToLower().Contains("web-player-link"))
                {
                    Size = new Size(365, 70);
                    Region = Region.FromHrgn(MiscFunctions.CreateRoundRectRgn(0, 0, Width, Height, 25, 25));
                    webView.CoreWebView2.Navigate("https://open.spotify.com/");
                }
                else if (WebPage.Contains("Email address or username"))
                {
                    Size = new Size(365, 630);
                    Region = Region.FromHrgn(MiscFunctions.CreateRoundRectRgn(0, 0, Width, Height, 25, 25));
                }
            };
            webView.CoreWebView2.WebResourceRequested += (__, ResourceArgs) =>
            {
                if (ResourceArgs.Request.Headers.Contains("authorization"))
                {
                    Size = new Size(365, 70);
                    Region = Region.FromHrgn(MiscFunctions.CreateRoundRectRgn(0, 0, Width, Height, 25, 25));
                    SpotifyAuth = ResourceArgs.Request.Headers.GetHeader("Authorization");
                }
            };

            webView.CoreWebView2.NavigationStarting += (__, WebViewArgs) => { CurrentUrl = WebViewArgs.Uri; };
            webView.CoreWebView2.AddWebResourceRequestedFilter("*spotify.com*", CoreWebView2WebResourceContext.All);
            webView.CoreWebView2.Navigate("https://accounts.spotify.com/en/login");

            while (SpotifyAuth == "") await Task.Delay(50);

            webView.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;

            await webView.CoreWebView2.ExecuteScriptAsync(@"
function GetArtist() {
    let Artists = """";
    var ArtistsRaw = document.querySelectorAll('[data-testid=""context-item-info-artist""]');
    for (var i = 0, len = ArtistsRaw.length; i<len;i++)
    {
        Artists+= "", "" + ArtistsRaw[i].innerHTML;
    }
    Artists = Artists.substring(2, Artists.length);
    return Artists
}; 
setInterval(function() {
    window.chrome.webview.postMessage( GetArtist() + ""|"" + document.querySelectorAll('[data-testid=""context-item-link""]')[0].innerHTML + ""|"" + document.querySelectorAll('[data-testid=""cover-art-image""]')[0].src + ""|"" + document.querySelectorAll('[data-testid=""playback-duration""]')[0].innerHTML + ""|"" + document.querySelectorAll('[data-testid=""playback-position""]')[0].innerHTML + ""|"" + document.querySelectorAll('[data-testid=""progress-bar""]')[0].style.cssText)
}, 0)
");

            await Task.Run(() =>
            {
                using (WebClient Client = new WebClient())
                {
                    Client.Headers.Add(HttpRequestHeader.Authorization, SpotifyAuth);
                    Client.Headers.Add(HttpRequestHeader.ContentType, "application/json");

                    Client.DownloadStringCompleted += async (__, DownloadStringArgs) =>
                    {
                        try
                        {
                            JToken returnedData = JToken.Parse(DownloadStringArgs.Result);
                            double songTimeStamp = returnedData.Value<double>("progress_ms");
                            double songDuration = returnedData["item"].Value<double>("duration_ms");

                            int Percent = (int)Math.Floor((songTimeStamp / songDuration) * 100);

                            bunifuProgressBar1.Invoke((Action)(() =>
                            {
                                bunifuProgressBar1.Value = Percent;
                            }));

                            Client.DownloadStringAsync(new Uri("https://api.spotify.com/v1/me/player"));
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                            await Task.Delay(60000);
                            Client.DownloadStringAsync(new Uri("https://api.spotify.com/v1/me/player"));
                        }
                    };

                    Client.DownloadStringAsync(new Uri("https://api.spotify.com/v1/me/player"));
                }
            });
        }

        private void CoreWebView2_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            List<string> Data = e.TryGetWebMessageAsString().Split('|').ToList();

            if (Data[1] != songNameLabel.Text)
            {
                artistNameLabel.Text = Data[0];
                songNameLabel.Text = Data[1];
                albumArt.Load(Data[2]);
            }
        }
    }
}
