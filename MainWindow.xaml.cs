using System.Diagnostics;
using System.Windows;
using iOverlay.Apps;
using iOverlay.Logic.SaveLoad;
using Microsoft.Web.WebView2.Core;
using Wpf.Ui.Common;
using MessageBox = Wpf.Ui.Controls.MessageBox;

namespace iOverlay;

public partial class MainWindow
{
    private readonly IniFileParser _parser = new();

    public MainWindow() => InitializeComponent();

    private void SettingsPage_Click(object sender, RoutedEventArgs e)
    {
        SettingsWindow.Visibility = Visibility.Visible;
    }

    private void SaveSettings_Click(object sender, RoutedEventArgs e)
    {
        App.ValorantSettings = new SaveLogic.ValorantSettings(ValorantName.Text, ValorantKdWinPercent.IsChecked!.Value);

        _parser.SetValue("Valorant", "RiotId", ValorantName.Text);
        _parser.SetValue("Valorant", "ShowStats", ValorantKdWinPercent.IsChecked!.Value.ToString());
        
        _parser.Save("settings.config");

        SettingsWindow.Visibility = Visibility.Hidden;
    }

    private void UiWindow_Loaded(object sender, RoutedEventArgs e)
    {
        if (Environment.OSVersion.Version.Build > 22000)
        {
            try
            {
                CoreWebView2Environment.GetAvailableBrowserVersionString();
            }
            catch
            {
                ShowInvalidMessageBox("WebView2 SDK is not installed!");
                Environment.Exit(-1);
            }
        }
        else
        {
            ShowInvalidMessageBox("Invalid OS Detected, please be running Windows 11!");
            Environment.Exit(-1);
        }

        _parser.Load("settings.config");
        App.ValorantSettings = new SaveLogic.ValorantSettings(
            _parser.GetValue("Valorant", "RiotId"),
            _parser.GetBoolValue("Valorant", "ShowStats"));

        ValorantDarkMode.IsChecked = _parser.GetBoolValue("Valorant", "DarkMode");
        ValorantKdWinPercent.IsChecked = _parser.GetBoolValue("Valorant", "ShowStats");
        ValorantName.Text = _parser.GetValue("Valorant", "RiotId");

        SpotifyDarkMode.IsChecked = _parser.GetBoolValue("Spotify", "DarkMode");
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

    private void ValorantOverlay_Click(object sender, RoutedEventArgs e)
    {
        new ValorantOverlay().Show();
    }

    private void SpotifyOverlay_Click(object sender, RoutedEventArgs e)
    {
        new SpotifyOverlay().Show();
    }
}