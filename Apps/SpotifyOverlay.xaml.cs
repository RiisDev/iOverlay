﻿using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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
        HttpResponseMessage pageData =
            await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get,
                "https://api.spotify.com/v1/me/player"));

        if (!pageData.IsSuccessStatusCode)
        {
            HandleWebExceptions(pageData.StatusCode);
            return;
        }

        string pageContent = await pageData.Content.ReadAsStringAsync();

        if (!pageContent.Contains('[')) goto Restart;

        JsonDocument returnedData = JsonDocument.Parse(pageContent);
        
        // Get base properties
        JsonElement rootElement = returnedData.RootElement;
        JsonElement? trackData = rootElement.GetPropertyNullable("item");
        JsonElement? progressElement = rootElement.GetPropertyNullable("progress_ms");
        
        // Get track properties
        JsonElement? songDurationElement = trackData?.GetPropertyNullable("duration_ms");
        JsonElement? songNameElement = trackData?.GetPropertyNullable("name");
        JsonElement? artistNameElement = trackData?.GetPropertyNullable("artists")?[0].GetProperty("name");
        JsonElement? albumArtElement = trackData?.GetPropertyNullable("album")?.GetPropertyNullable("images")?[0].GetPropertyNullable("url");

        // Set track variables
        double songTimeStamp = progressElement?.GetDouble() ?? 1.0; 
        double songDuration = songDurationElement?.GetDouble() ?? 1.0;

        int percent = (int)Math.Floor(songTimeStamp / songDuration * 100);

        string songName = songNameElement?.GetString() + " ";
        string artistName = artistNameElement?.GetString() ?? "Failed to fetch artist";
        string albumArt = albumArtElement?.GetString() ?? "pack://application:,,,/Assets/Images/SpotifyIcon.png"; 

        // Perform updates
        SongProgress.Dispatcher.Invoke(() => { SongProgress.Value = percent; });

        if (songName == _cachedSongName) goto Restart;

        SafeSet(SongName, songName);
        SafeSet(ArtistName, artistName);
        AlbumArt.Dispatcher.Invoke(() => { AlbumArt.ImageSource = new BitmapImage(new Uri(albumArt)); });
        SecondAlbumArt.Dispatcher.Invoke(() => { SecondAlbumArt.ImageSource = new BitmapImage(new Uri(albumArt)); });

        _cachedSongName = songName;

        Restart:
        StartSongWatch();
    }

    private async void HandleWebExceptions(HttpStatusCode statusCode)
    {

        switch (statusCode)
        {
            case HttpStatusCode.Unauthorized:
                await Handle401Exception();
                return;
            case HttpStatusCode.TooManyRequests:
                await Handle429Exception();
                return;
            case HttpStatusCode.InternalServerError:
            case HttpStatusCode.NotImplemented:
            case HttpStatusCode.BadGateway:
            case HttpStatusCode.BadRequest:
                await Task.Run(StartSongWatch);
                return;
            default:
                throw new ArgumentOutOfRangeException(nameof(statusCode), statusCode, null);
        }
    }

    private async Task Handle401Exception()
    {
        if (!_401Running)
        {
            _401Running = true;
            _refreshed = false;

            webView.Dispatcher.Invoke(() => webView.CoreWebView2.Navigate("https://accounts.spotify.com/en/login"));

            while (!_refreshed) await Task.Delay(25);

            await Task.Run(StartSongWatch);
            
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
            await Task.Run(StartSongWatch);
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