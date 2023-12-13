using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using iOverlay.Logic;
using iOverlay.Logic.ValorantApi.Clients;
using iOverlay.Logic.WidgetLogic;

namespace iOverlay.Apps;

public partial class ValorantOverlay
{
    // Fields
    private UserClient _userClientConfig = null!;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly ValorantLogic _valorantLogic = new ();
    private readonly Random _backgroundRandom = new(DateTime.Now.Millisecond);

    private bool _firstRun;

    private int _lastEndedCount;
    private int _oldRankRating;
    private long _lastFileSize;
    
    private readonly List<string> _backgroundAssets = new()
    {
        "pack://application:,,,/Assets/Images/Lotus.png",
        "pack://application:,,,/Assets/Images/Icebox.png",
        "pack://application:,,,/Assets/Images/Fracture.png",
        "pack://application:,,,/Assets/Images/Haven.png",
        "pack://application:,,,/Assets/Images/Bind.png",
        "pack://application:,,,/Assets/Images/Breeze.png",
        "pack://application:,,,/Assets/Images/Split.png",
        "pack://application:,,,/Assets/Images/Ascent.png",
        "pack://application:,,,/Assets/Images/Sunset.png"
    };

    public ValorantOverlay()
    {
        this.ApplyDraggable();
        InitializeComponent();
        BackSplash.ImageSource = new BitmapImage(new Uri(_backgroundRandom.GetRandomListItem(_backgroundAssets)));

        _cancellationTokenSource = new CancellationTokenSource();
    }

    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        Retry:
        if (Process.GetProcessesByName("VALORANT-Win64-Shipping").Length <= 0)
        {
           MessageBoxResult result = MessageBox.Show("Please launch valorant then click Ok!", "Waiting...", MessageBoxButton.OKCancel, MessageBoxImage.Information);
           if (result == MessageBoxResult.Cancel)
           {
               Close();
               return;
           }
           goto Retry;
        }
        
        _userClientConfig = new UserClient(new ValorantClient());
        await MonitorLogFileAsync();
    }

    private async Task MonitorLogFileAsync()
    {
        while (!_cancellationTokenSource.Token.IsCancellationRequested)
        {
            await Task.Delay(500, _cancellationTokenSource.Token);

            long currentFileSize = new FileInfo(_valorantLogic.Log).Length;

            if (currentFileSize == _lastFileSize) continue;

            _lastFileSize = currentFileSize;

            string fileText = _valorantLogic.GetLogText();
            Debug.WriteLine("FILE CHANGED");
            int matchEndedCount = fileText.Split("Match Ended").Length;
            Debug.WriteLine($"{matchEndedCount} | {_lastEndedCount}");
            if (_lastEndedCount == matchEndedCount) continue;

            Debug.WriteLine("RUNNING RANK CHECK");

            int tempRankRating = _oldRankRating;

            RunRankCheck();

            if (tempRankRating == _oldRankRating)
            {
                await Task.Delay(2000);
                continue;
            }

            _lastEndedCount = matchEndedCount;
        }
    }

    // Perhaps game loaded and ready: [2023.12.07-08.32.34:994][820]LogTravelManager: Beginning travel to 192.207.0.1:7071
    private void RunRankCheck()
    {
        Application.Current.Dispatcher.Invoke(async () =>
        {
            await _userClientConfig.GetStats();

            if (_userClientConfig.RankRating == _oldRankRating) return;

            UpdateUiElements(_userClientConfig.Rank, _userClientConfig.RankRating, _userClientConfig.Headshot, _userClientConfig.WinRate);

            _oldRankRating = _userClientConfig.RankRating;
        });
    }

    private void UpdateUiElements(ValorantRank rank, int currentRankRating, string? headshotPercentage, string? winPercentage)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            PlayerRank.Content = rank.Rank;
            PlayerRank.Foreground = RankIcons.RankColors[(string)PlayerRank.Content!];

            if (_firstRun) SessionRrGain.AnimateSessionRankRating(currentRankRating);

            PlayerRankRating.AnimateRankRating(currentRankRating);
            RankRatingProgress.AnimateProgress(currentRankRating);

            PlayerWinPercent.Content = winPercentage;
            PlayerHeadshotPercent.Content = headshotPercentage;

            RankIcon.Source = new BitmapImage(new Uri(rank.RankIcon!));
            _firstRun = false;
        });
    }

    public void Dispose()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
    }
}