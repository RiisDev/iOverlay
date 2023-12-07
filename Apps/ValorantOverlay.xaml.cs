using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using iOverlay.Logic;
using iOverlay.Logic.WidgetLogic;
#pragma warning disable IDE0028

namespace iOverlay.Apps;

public partial class ValorantOverlay
{
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

    private readonly ValorantLogic _logic = new();

    private int _lastEndedCount;
    private long _lastFileSize;
    private int _oldRankRating;
    private bool _firstInvoke = true;
    private string _firstInvokeString = null!;

    private static readonly object LogLock = new();

    public ValorantOverlay() => InitializeComponent();


    //Match Ended
    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        this.ApplyDraggable();
        BackSplash.ImageSource = new BitmapImage(new Uri(GetRandom(_backgroundAssets)));

        if (App.ValorantSettings == null || string.IsNullOrEmpty(App.ValorantSettings.RiotName))
        {
            MessageBox.Show("Riot Id missing!");
            Close();
            return;
        }

        (bool, string) isValidUser = await _logic.IsValidUser(App.ValorantSettings.RiotName, true);

        if (!isValidUser.Item1)
        {
            MessageBox.Show(isValidUser.Item2);
            Close();
            return;
        }

        _firstInvokeString = isValidUser.Item2;

        string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        string logsPath = Path.Combine(userProfile, "AppData", "Local", "Valorant", "Saved", "Logs");
        string logPath = Path.Combine(logsPath, "ShooterGame.log");

        new Task(async() =>
        {
            while (true)
            {
                await Task.Delay(500);
                long currentFileSize = new FileInfo(logPath).Length;

                if (currentFileSize == _lastFileSize) continue;

                _lastFileSize = currentFileSize;

                string fileText = SafeReadFile(logPath);
                int matchEndedCount = fileText.Split("Match Ended").Length;

                if (_lastEndedCount == matchEndedCount) continue;

                await Task.Delay(3000);

                RunRankCheck();
                _lastEndedCount = matchEndedCount;
            }
        }).Start();

    }

    // Perhaps game loaded and ready: [2023.12.07-08.32.34:994][820]LogTravelManager: Beginning travel to 192.207.0.1:7071
    private void RunRankCheck()
    {
        Application.Current.Dispatcher.Invoke(async() =>
        {
            // Get current rank and rating
            (ValorantRank, int, int)? data = await _logic.GetUserRankStats(App.ValorantSettings?.RiotName);

            ValorantRank valorantRank = data?.Item1 ?? ValorantRank.Default();
            int currentRankRating = data?.Item2 ?? 0;
            int rankRatingChanges = data?.Item3 ?? 0;

            // Check if user is valid
            (_, string trackerData) = _firstInvoke ? (false, _firstInvokeString) : await _logic.IsValidUser(App.ValorantSettings?.RiotName, true);

            // Wait until validation is complete
            while (string.IsNullOrEmpty(trackerData)) await Task.Delay(15);

            // Skip if rating hasn't changed
            if (_oldRankRating == currentRankRating) return;

            // Get headshot percentage and win percentage
            string? headshotPercentage = await _logic.GetHeadshotPercentage(trackerData);
            string? winPercentage = await _logic.GetWinPercentage(trackerData);

            // Update UI elements
            PlayerRank.Content = valorantRank.Rank;
            PlayerRank.Foreground = RankIcons.RankColors[(string)PlayerRank.Content!];

            SessionRrGain.AnimateSessionRankRating(_firstInvoke ? 0 : rankRatingChanges);
            PlayerRankRating.AnimateRankRating(currentRankRating);
            RankRatingProgress.AnimateProgress(currentRankRating);

            PlayerWinPercent.Content = winPercentage;
            PlayerHeadshotPercent.Content = headshotPercentage;

            RankIcon.Source = new BitmapImage(new Uri(valorantRank.RankIcon!));

            // Update old rating
            _oldRankRating = currentRankRating;

            // Set _firstInvoke to false after first invocation
            _firstInvoke = false;
        });
    }


    // Other methods

    public static T GetRandom<T>(IEnumerable<T> list)
    {
        IEnumerable<T> enumerable = list.ToList();
        return enumerable.ElementAt(new Random(DateTime.Now.Millisecond).Next(enumerable.Count()));
    }

    private static string SafeReadFile(string filePath)
    {
        lock (LogLock)
        {
            try
            {
                using FileStream fs = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                using StreamReader reader = new(fs);
                return reader.ReadToEnd();
            }
            catch { return string.Empty; }
        }
    }
}