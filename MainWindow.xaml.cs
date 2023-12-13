#if DEBUG
#else
using iOverlay.Logic.DataTypes;
using System.Net.Http;
using System.Text.Json;
#endif
using System.Diagnostics;
using System.Windows;
using iOverlay.Apps;
using Microsoft.Web.WebView2.Core;
using Wpf.Ui.Common;
using MessageBox = Wpf.Ui.Controls.MessageBox;

namespace iOverlay;

public partial class MainWindow
{
    internal const string OverlayInternalVersion = "6.1.0";

    private void UiWindow_Loaded(object sender, RoutedEventArgs e)
    {
        try {
            CoreWebView2Environment.GetAvailableBrowserVersionString();
        } catch {
            ShowInvalidMessageBox("WebView2 SDK is not installed!");
            Environment.Exit(-1);
        }

#if DEBUG
#else

        Task.Run(() =>
        {
            using HttpClient client = new();

            client.DefaultRequestHeaders.Add("User-Agent", "request");
            client.DefaultRequestHeaders.Accept.ParseAdd("application/vnd.github.v3+json");

            string gitReturn = client.GetAsync("https://api.github.com/repos/IrisV3rm/iOverlay/releases/latest").Result.Content.ReadAsStringAsync().Result;
            GitRelease? gitRelease = JsonSerializer.Deserialize<GitRelease>(gitReturn);

            if (OverlayInternalVersion == gitRelease?.TagName) return;

            MessageBoxResult update = System.Windows.MessageBox.Show("There is an update, would you like to download now?", "iOverlay", MessageBoxButton.YesNo, MessageBoxImage.Information);
            if (update == MessageBoxResult.Yes) Process.Start("https://api.github.com/repos/IrisV3rm/iOverlay/releases/latest");
        });
#endif
    }

    private void ShowInvalidMessageBox(string message)
    {
        MessageBox invalidMessageBox = new()
        {
            Content = message,
            HorizontalContentAlignment = HorizontalAlignment.Center,
            VerticalContentAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            MicaEnabled = false,
            ButtonLeftName = "Help",
            ButtonRightName = "Cancel",
            ButtonLeftAppearance = ControlAppearance.Primary,
            ButtonRightAppearance = ControlAppearance.Danger,
            Title = Title
        };

        invalidMessageBox.ButtonLeftClick += (_, _) => Process.Start("https://irisapp.ca/");
        invalidMessageBox.ButtonRightClick += (_, _) => Environment.Exit(-1);
        invalidMessageBox.Closing += (_, _) => Environment.Exit(-1);

        invalidMessageBox.ShowDialog();
    }

    public MainWindow() => InitializeComponent();
    private void ValorantOverlay_Click(object sender, RoutedEventArgs e) => new ValorantOverlay().Show();
    private void SpotifyOverlay_Click(object sender, RoutedEventArgs e) => new SpotifyOverlay().Show();
    private void SettingsPage_Click(object sender, RoutedEventArgs e) { SettingsWindow.Visibility = Visibility.Visible; }
    private void SaveSettings_Click(object sender, RoutedEventArgs e) { SettingsWindow.Visibility = Visibility.Hidden; }
}