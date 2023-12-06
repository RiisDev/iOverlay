using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using iOverlay.Logic;
using Microsoft.Web.WebView2.Core;

namespace iOverlay.Apps;

public partial class SpotifyOverlay
{
    public SpotifyOverlay() => InitializeComponent();

    private readonly HttpClient _httpClient = new();
    private bool _401Running;
    private string _cachedSongName = "";
    private bool _refreshed;
    private int _songLabelIndex;

    private static void SafeSet(object controlObject, dynamic value)
    {
        switch (controlObject)
        {
            case Label label:
                label.Dispatcher.Invoke(() => { label.Content = value; });
                break;
        }
    }

    private async void StartSongWatch()
    {
        try
        {
            HttpResponseMessage pageData =
                await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get,
                    "https://api.spotify.com/v1/me/player"));

            JsonDocument returnedData = JsonDocument.Parse(pageData.Content.ReadAsStringAsync().Result);

            double songTimeStamp;

            try
            {
                songTimeStamp = returnedData.RootElement.GetProperty("progress_ms").GetDouble();
            }
            catch
            {
                songTimeStamp = 0.0;
            }


            JsonElement itemData = returnedData.RootElement.GetProperty("item");

            double songDuration = itemData.GetProperty("duration_ms").GetDouble();
            string songName = itemData.GetProperty("name").GetString() + " ";
            string? artistName = itemData.GetProperty("artists")[0].GetProperty("name").GetString();
            string? albumArtData = itemData.GetProperty("album").GetProperty("images")[0].GetProperty("url")
                .GetString();

            int percent = (int)Math.Floor(songTimeStamp / songDuration * 100);

            SongProgress.Dispatcher.Invoke(() => { SongProgress.Value = percent; });

            if (songName == _cachedSongName) goto Restart;

            SafeSet(SongName, songName);
            if (artistName != null) SafeSet(ArtistName, artistName);
            if (albumArtData != null)
            {
                AlbumArt.Dispatcher.Invoke(() => { AlbumArt.ImageSource = new BitmapImage(new Uri(albumArtData)); });
                SecondAlbumArt.Dispatcher.Invoke(() =>
                {
                    SecondAlbumArt.ImageSource = new BitmapImage(new Uri(albumArtData));
                });
            }

            _cachedSongName = songName;

            Restart:
            StartSongWatch();
        }
        catch (TargetInvocationException? exception)
        {
            HandleWebExceptions(exception);
        }
        catch (JsonException)
        {
            await Task.Delay(15000);
            StartSongWatch();
        }
        catch (Exception exception)
        {
            Debug.WriteLine("Unhandled Except: " + exception);
        }
    }

    private async void HandleWebExceptions(TargetInvocationException? exceptionCode)
    {
        if (exceptionCode?.InnerException == null) return;

        string innerExceptionMessage = exceptionCode.InnerException.Message;

        if (innerExceptionMessage.Contains("401"))
            await Handle401Exception();
        else if (innerExceptionMessage.Contains("429"))
            await Handle429Exception();
        else if (innerExceptionMessage.Contains("502") || innerExceptionMessage.Contains("503"))
            await _httpClient.GetStringAsync("https://api.spotify.com/v1/me/player");
        else
            Debug.WriteLine(exceptionCode.InnerException);
    }

    private async Task Handle401Exception()
    {
        if (!_401Running)
        {
            _401Running = true;
            _refreshed = false;

            webView.Dispatcher.Invoke(() => webView.CoreWebView2.Navigate("https://accounts.spotify.com/en/login"));

            while (!_refreshed) await Task.Delay(25);

            await _httpClient.GetStringAsync("https://api.spotify.com/v1/me/player");

            _401Running = false;
        }
    }

    private async Task Handle429Exception()
    {
        HttpResponseMessage response = await _httpClient.GetAsync("https://api.spotify.com/v1/me/player");

        if (response.Headers.TryGetValues("Retry-After", out IEnumerable<string>? retryAfterHeaders) &&
            int.TryParse(retryAfterHeaders.FirstOrDefault(), out int retryAfterDelay))
        {
            await Task.Delay(retryAfterDelay * 1000); // Delay in milliseconds
            await _httpClient.GetStringAsync("https://api.spotify.com/v1/me/player");
        }
    }


    private void SetupWebViewHandler()
    {
        webView.CoreWebView2.NavigationCompleted += async (_, _) =>
        {
            string webPage = await webView.CoreWebView2.ExecuteScriptAsync("document.documentElement.outerHTML;");

            UpdateWindowSize(webPage);

            _refreshed = true;
        };

        webView.CoreWebView2.WebResourceRequested += (_, resourceArgs) =>
        {
            if (!resourceArgs.Request.Headers.Contains("authorization")) return;

            UpdateWindowSize();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
                resourceArgs.Request.Headers.GetHeader("Authorization").Replace("Bearer ", "").Replace("\n", "")
                    .Replace("\r", ""));
        };

        webView.CoreWebView2.AddWebResourceRequestedFilter("*spotify.com*", CoreWebView2WebResourceContext.All);
        webView.CoreWebView2.Navigate("https://accounts.spotify.com/en/login");
    }

    private void UpdateWindowSize(string webPageActual = "")
    {
        string webPage = WebUtility.UrlDecode(webPageActual);
        if (webPage.Contains("web-player-link", StringComparison.CurrentCultureIgnoreCase))
        {
            SetWindowSize(365, 69);
            webView.CoreWebView2.Navigate("https://open.spotify.com/");
        }
        else if (webPage.Contains("login-username")) SetWindowSize(365, 630);
        else SetWindowSize(363, 69);
    }

    private void SetWindowSize(int width, int height)
    {
        Width = width;
        Height = height;

        webView.Height = height <= 69 ? 0 : 550;
        webView.Width = height <= 69 ? 0 : width;
    }

    private double? MeasureTextWidth(FrameworkElement label)
    {
        double pixelsPerDip = VisualTreeHelper.GetDpi(this).PixelsPerDip;

        return label.ActualWidth / pixelsPerDip;
    }

    [SuppressMessage("ReSharper", "FunctionNeverReturns")]
    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        new Task(() =>
        {
            SongName.Dispatcher.Invoke(async () =>
            {
                while (true)
                {
                    if (MeasureTextWidth(SongName) > 280)
                    {
                        _songLabelIndex++;

                        try
                        {
                            if (_songLabelIndex >= 0 && _songLabelIndex <= _cachedSongName.Length)
                                SongName.Content = _cachedSongName[_songLabelIndex..] +
                                                   _cachedSongName[.._songLabelIndex];
                            else _songLabelIndex = 1;
                        }
                        catch
                        {
                            _songLabelIndex = 0;
                        }
                    }
                    else
                    {
                        _songLabelIndex = 0;
                    }

                    await Task.Delay(300);
                }
            });
        }).Start();


        this.ApplyDraggable();
        await webView.EnsureCoreWebView2Async();

        SetupWebViewHandler();

        while (string.IsNullOrEmpty(_httpClient.DefaultRequestHeaders.Authorization?.Parameter)) await Task.Delay(50);

        await Task.Run(StartSongWatch);
    }

    private async void Border_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        await Task.Run(StartSongWatch);
    }
}