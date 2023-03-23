using iOverlay.Utility;
using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace iOverlay.Widgets
{
    public partial class SpotifyWidget : Form
    {
        private string cachedSongName = "";
        private string songName = "";
        private string _spotifyAuth = "";
        private bool _refreshed = false;
        private bool _401Running = false;
        private int songLabelIndex = 0;

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

        private void SpotifyTimestampHandler()
        {
            using (WebClient client = new WebClient())
            {
                client.Headers.Add(HttpRequestHeader.Authorization, _spotifyAuth);
                client.Headers.Add(HttpRequestHeader.ContentType, "application/json");

                client.DownloadStringCompleted += async (__, downloadStringArgs) =>
                {
                    try
                    {
                        JToken returnedData = JToken.Parse(downloadStringArgs.Result);

                        double songTimeStamp = returnedData.Value<double>("progress_ms");
                        double songDuration = returnedData["item"].Value<double>("duration_ms");
                        songName = returnedData["item"].Value<string>("name") + " ";
                        string artistName = returnedData["item"]["artists"][0].Value<string>("name");
                        string albumArtData = returnedData["item"]["album"]["images"][0].Value<string>("url");

                        int percent = (int)Math.Floor((songTimeStamp / songDuration) * 100);

                        bunifuProgressBar1.Invoke((Action)(() => bunifuProgressBar1.Value = percent));


                        if (songName != cachedSongName)
                        {
                            songNameLabel.Invoke((Action)(() => songNameLabel.Text = songName));
                            artistNameLabel.Invoke((Action)(() => artistNameLabel.Text = artistName));
                            albumArt.Invoke((Action)(() => albumArt.Load(albumArtData)));
                            cachedSongName = songName;
                        }

                        client.DownloadStringAsync(new Uri("https://api.spotify.com/v1/me/player"));
                    }
                    catch (TargetInvocationException exception)
                    {
                        HandleWebExceptions(client, exception);
                    }
                    catch (JsonReaderException)
                    {
                        await Task.Delay(15000);
                        client.DownloadStringAsync(new Uri("https://api.spotify.com/v1/me/player"));
                    }
                    catch (Exception exception)
                    {
                        Debug.WriteLine("Unhandled Except: " + exception);
                    }
                };

                client.DownloadStringAsync(new Uri("https://api.spotify.com/v1/me/player"));
            }
        }

        private async void HandleWebExceptions(WebClient webClient, TargetInvocationException exceptionCode)
        {
            if (exceptionCode == null) return;
            if (exceptionCode.InnerException == null) return;

            if (exceptionCode.InnerException.Message.Contains("401"))
            {
                if (!_401Running && !webClient.IsBusy)
                {
                    _401Running = true;
                    _refreshed = false;

                    webView.Invoke((Action)(() => webView.CoreWebView2.Navigate("https://accounts.spotify.com/en/login")));

                    while (!_refreshed) await Task.Delay(25);

                    webClient.DownloadStringAsync(new Uri("https://api.spotify.com/v1/me/player"));

                    _401Running = false;
                }
            }
            else if (exceptionCode.InnerException.Message.Contains("429"))
            {
                await Task.Delay(int.Parse(webClient.ResponseHeaders["Retry-After"]));
                webClient.DownloadStringAsync(new Uri("https://api.spotify.com/v1/me/player"));
            }
            else if (exceptionCode.InnerException.Message.Contains("502")) webClient.DownloadStringAsync(new Uri("https://api.spotify.com/v1/me/player"));
            else if (exceptionCode.InnerException.Message.Contains("503")) webClient.DownloadStringAsync(new Uri("https://api.spotify.com/v1/me/player"));
            else Debug.WriteLine(exceptionCode.InnerException);
        }

        private async void SpotifyWidget_Load(object sender, EventArgs e)
        {
            #if DEBUG 
                RefreshSpotify.Visible = true;
                throwfouroone.Visible = true;
            #endif
            ApplyTheme(darkMode: Properties.Settings.Default.spotifyDarkMode);
            Region = Region.FromHrgn(MiscFunctions.CreateRoundRectRgn(0, 0, Width, Height, 25, 25));
            MiscFunctions.CreateMovableForm(this, this);
            Opacity = .8;

            await webView.EnsureCoreWebView2Async();

            webView.CoreWebView2.NavigationCompleted += async (____, coreWEbViewNavigationArgs) =>
            {
                string webPage = await webView.CoreWebView2.ExecuteScriptAsync("document.documentElement.outerHTML;");

                if (webPage.ToLower().Contains("web-player-link"))
                {
                    Size = new Size(365, 70);
                    webView.CoreWebView2.Navigate("https://open.spotify.com/");
                }
                else if (webPage.Contains("Email address or username")) Size = new Size(365, 630);

                Region = Region.FromHrgn(MiscFunctions.CreateRoundRectRgn(0, 0, Width, Height, 25, 25));
                _refreshed = true;
            };
            webView.CoreWebView2.WebResourceRequested += (__, resourceArgs) =>
            {
                if (!resourceArgs.Request.Headers.Contains("authorization")) return;

                Size = new Size(365, 70);
                Region = Region.FromHrgn(MiscFunctions.CreateRoundRectRgn(0, 0, Width, Height, 25, 25));
                _spotifyAuth = resourceArgs.Request.Headers.GetHeader("Authorization");
            };
            
            webView.CoreWebView2.AddWebResourceRequestedFilter("*spotify.com*", CoreWebView2WebResourceContext.All);
            webView.CoreWebView2.Navigate("https://accounts.spotify.com/en/login");

            while (_spotifyAuth == "") await Task.Delay(50);

            await Task.Run(SpotifyTimestampHandler);
        }

        private void RefreshSpotify_Click(object sender, EventArgs e)
        {
           // webView.Invoke((Action)(() => webView.CoreWebView2.Navigate("https://accounts.spotify.com/en/login")));
        }

        private void scrollTextTimer_Tick(object sender, EventArgs e)
        {
            if (TextRenderer.MeasureText(songName, songNameLabel.Font).Width > songNameLabel.Width)
            {
                songLabelIndex++;

                songNameLabel.Invoke((Action)(() => {
                    try
                    {
                        songNameLabel.Text = songName.Substring(songLabelIndex) + songName.Substring(0, songLabelIndex);
                    }
                    catch { songLabelIndex = 0; }
                }));
            }
            else
            {
                songLabelIndex = 0;
            }
        }
    }
}
