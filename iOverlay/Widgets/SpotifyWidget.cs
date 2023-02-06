using iOverlay.Utility;
using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json.Linq;
using System;
using System.Drawing;
using System.Net;
using System.Reflection;
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
            var backColor = darkMode ? Color.FromArgb(30, 30, 30) : Color.FromArgb(248, 248, 248);
            var foreColor = darkMode ? Color.White : Color.FromKnownColor(KnownColor.ControlText);

            BackColor = backColor;
            songNameLabel.ForeColor = foreColor;
            artistNameLabel.ForeColor = foreColor;
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

            webView.CoreWebView2.NavigationStarting += (__, args) => CurrentUrl = args.Uri;
            webView.CoreWebView2.AddWebResourceRequestedFilter("*spotify.com*", CoreWebView2WebResourceContext.All);
            webView.CoreWebView2.Navigate("https://accounts.spotify.com/en/login");

            while (SpotifyAuth == "") await Task.Delay(50);

            webView.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;

            await webView.CoreWebView2.ExecuteScriptAsync(@"
const getArtist = () => {
  const artistsRaw = document.querySelectorAll('[data-testid=""context-item-info-artist""]');
  return Array.from(artistsRaw).map(a => a.innerHTML).join(', ');
};

setInterval(() => {
  const message = [
    getArtist(),
    document.querySelector('[data-testid=""context-item-link""]').innerHTML,
    document.querySelector('[data-testid=""cover-art-image""]').src,
    document.querySelector('[data-testid=""playback-duration""]').innerHTML,
    document.querySelector('[data-testid=""playback-position""]').innerHTML,
    document.querySelector('[data-testid=""progress-bar""]').style.cssText
  ].join('|');
  window.chrome.webview.postMessage(message);
}, 0);
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
                        catch (TargetInvocationException exception)
                        {
                            if (exception.InnerException.Message.Contains("401"))
                            {
                                string oldAuth = SpotifyAuth;
                                webView.CoreWebView2.Navigate("https://accounts.spotify.com/en/login");

                                while (oldAuth == SpotifyAuth) await Task.Delay(25);
                            }
                            else if (exception.InnerException.Message.Contains("429"))
                            {
                                await Task.Delay(30000);
                                Client.DownloadStringAsync(new Uri("https://api.spotify.com/v1/me/player"));
                            }
                        }
                        catch { }
                    };

                    Client.DownloadStringAsync(new Uri("https://api.spotify.com/v1/me/player"));
                }
            });
        }

        private void CoreWebView2_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            var data = e.TryGetWebMessageAsString().Split('|');
            if (data[1] != songNameLabel.Text)
            {
                artistNameLabel.Text = data[0];
                songNameLabel.Text = data[1];
                albumArt.Load(data[2]);
            }
        }
    }
}
