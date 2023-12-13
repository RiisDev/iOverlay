using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using iOverlay.Logic;
using iOverlay.Logic.DataTypes;
using Microsoft.Web.WebView2.Core;
// ReSharper disable SwitchStatementHandlesSomeKnownEnumValuesWithDefault
// ReSharper disable FunctionNeverReturns

namespace iOverlay.Apps;

public partial class SpotifyOverlay
{
    private async void Border_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) => await Task.Run(StartSongWatch);
    public SpotifyOverlay() => InitializeComponent();

    private readonly HttpClient _httpClient = new();

    private static readonly Uri PlayerUri = new("https://api.spotify.com/v1/me/player");

    private string _cachedSongName = "";

    private int _songLabelIndex;

    private bool _401Running;
    private bool _refreshed;

    private async Task StartSongWatch()
    {
        HttpResponseMessage pageData = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, PlayerUri));

        if (!pageData.IsSuccessStatusCode)
        {
            HandleWebExceptions(pageData.StatusCode);
            return;
        }

        string pageContent = await pageData.Content.ReadAsStringAsync();

        if (!pageContent.Contains('[')) goto Restart;

        SpotifyPlayer? player = JsonSerializer.Deserialize<SpotifyPlayer>(pageContent);

        if (player is null) goto Restart;

        double songTimeStamp = player.ProgressMs ?? 1.0; 
        double songDuration = player.Track.DurationMs ?? 1.0;

        int percent = (int)Math.Floor(songTimeStamp / songDuration * 100);

        string songName = player.Track.Name + " ";
        string artistName = player.Track.Artists[0].Name;
        string albumArt = player.Track.Album.Images[0].Url; 

        // Perform updates
        SongProgress.Dispatcher.Invoke(() => { SongProgress.Value = percent; });

        if (songName == _cachedSongName) goto Restart;

        SongName.SafeSet(songName);
        ArtistName.SafeSet(artistName);
        AlbumArt.Dispatcher.Invoke(() => { AlbumArt.ImageSource = new BitmapImage(new Uri(albumArt)); });
        SecondAlbumArt.Dispatcher.Invoke(() => { SecondAlbumArt.ImageSource = new BitmapImage(new Uri(albumArt)); });

        _cachedSongName = songName;

        Restart:
        await Task.Run(StartSongWatch);
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

            WebView.Dispatcher.Invoke(() => WebView.CoreWebView2.Navigate(PlayerUri.ToString()));

            while (!_refreshed) await Task.Delay(25);

            await Task.Run(StartSongWatch);
            
            _401Running = false;
        }
    }

    private async Task Handle429Exception()
    {
        HttpResponseMessage response = await _httpClient.GetAsync(PlayerUri);

        if (response.Headers.TryGetValues("Retry-After", out IEnumerable<string>? retryAfterHeaders) && int.TryParse(retryAfterHeaders.FirstOrDefault(), out int retryAfterDelay))
        {
            await Task.Delay(retryAfterDelay * 1000); // Delay in milliseconds
            await Task.Run(StartSongWatch);
        }
    }


    private void SetupWebViewHandler()
    {
        WebView.CoreWebView2.NavigationCompleted += async (_, _) =>
        {
            string webPage = await WebView.CoreWebView2.ExecuteScriptAsync("document.documentElement.outerHTML;");

            UpdateWindowSize(webPage);

            _refreshed = true;
        };

        WebView.CoreWebView2.WebResourceRequested += (_, resourceArgs) =>
        {
            if (!resourceArgs.Request.Headers.Contains("authorization")) return;

            UpdateWindowSize();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", resourceArgs.Request.Headers.GetHeader("Authorization")["Bearer ".Length..].Trim());
        };

        WebView.CoreWebView2.AddWebResourceRequestedFilter("*spotify.com*", CoreWebView2WebResourceContext.All);
        WebView.CoreWebView2.Navigate("https://accounts.spotify.com/en/login");
    }

    private void UpdateWindowSize(string webPageActual = "")
    {
        string webPage = WebUtility.UrlDecode(webPageActual);
        if (webPage.Contains("web-player-link", StringComparison.CurrentCultureIgnoreCase))
        {
            SetWindowSize(365, 69);
            WebView.CoreWebView2.Navigate("https://open.spotify.com/");
        }
        else if (webPage.Contains("login-username")) SetWindowSize(365, 630);
        else SetWindowSize(363, 69);
    }

    private void SetWindowSize(int width, int height)
    {
        Width = width;
        Height = height;

        WebView.Height = height <= 69 ? 0 : 550;
        WebView.Width = height <= 69 ? 0 : width;
    }

    private double? MeasureTextWidth(FrameworkElement label)
    {
        double pixelsPerDip = VisualTreeHelper.GetDpi(this).PixelsPerDip;

        return label.ActualWidth / pixelsPerDip;
    }

    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        await Task.Run(() =>
        {
            SongName.Dispatcher.InvokeAsync(async () =>
            {
                while (true)
                {
                    if (MeasureTextWidth(SongName) > 280)
                    {
                        _songLabelIndex++;

                        try
                        {
                            if (_songLabelIndex >= 0 && _songLabelIndex <= _cachedSongName.Length) SongName.Content = _cachedSongName[_songLabelIndex..] + _cachedSongName[.._songLabelIndex];
                            else _songLabelIndex = 1;
                        }
                        catch
                        {
                            _songLabelIndex = 0;
                        }
                    }
                    else _songLabelIndex = 0;

                    await Task.Delay(300);
                }
            });
        });


        this.ApplyDraggable();
        await WebView.EnsureCoreWebView2Async();

        SetupWebViewHandler();

        while (string.IsNullOrEmpty(_httpClient.DefaultRequestHeaders.Authorization?.Parameter)) await Task.Delay(50);

        await Task.Run(StartSongWatch);
    }

}