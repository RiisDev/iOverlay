using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using iOverlay.Logic;
using iOverlay.Logic.WidgetLogic;

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

    private int _oldRankRating;
    private bool _firstInvoke = true;
    private string _firstInvokeString = null!;

    private readonly DispatcherTimer _rankChecker = new()
    {
        Interval = TimeSpan.FromMinutes(1)
    };

    public ValorantOverlay()
    {
        InitializeComponent();
        _rankChecker.Tick += RankChecker_Tick;
    }

    public static T GetRandom<T>(IEnumerable<T> list)
    {
        IEnumerable<T> enumerable = list.ToList();
        return enumerable.ElementAt(new Random(DateTime.Now.Millisecond).Next(enumerable.Count()));
    }

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

        _rankChecker.Start();
        RankChecker_Tick(null, null);

    }

    private async void RankChecker_Tick(object? sender, EventArgs? e)
    {
        // Get current rank and rating
        (ValorantRank currentRank, int rating) = await _logic.GetCurrentRank(App.ValorantSettings?.RiotName);

        // Check if user is valid
        (_, string trackerData) = _firstInvoke ? (false, _firstInvokeString) : await _logic.IsValidUser(App.ValorantSettings?.RiotName, true);

        // Wait until validation is complete
        while (string.IsNullOrEmpty(trackerData)) await Task.Delay(15);

        // Skip if rating hasn't changed
        if (_oldRankRating == rating) return;

        // Get headshot percentage and win percentage
        string? headshotPercentage = await _logic.GetHeadshotPercentage(trackerData);
        string? winPercentage = await _logic.GetWinPercentage(trackerData);

        // Update UI elements
        PlayerRank.Content = currentRank.Rank;
        PlayerRank.Foreground = RankIcons.RankColors[(string)PlayerRank.Content!];

        SessionRrGain.AnimateSessionRankRating(_firstInvoke ? 0 : (int)RankRatingProgress.Progress - rating);
        PlayerRankRating.AnimateRankRating(rating);
        RankRatingProgress.AnimateProgress(rating);

        PlayerWinPercent.Content = winPercentage;
        PlayerHeadshotPercent.Content = headshotPercentage;

        RankIcon.Source = new BitmapImage(new Uri(currentRank.RankIcon!));

        // Update old rating
        _oldRankRating = rating;

        // Set _firstInvoke to false after first invocation
        _firstInvoke = false;
    }

}