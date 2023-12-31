using System.Diagnostics;
using System.Windows;
using System.Windows.Media.Imaging;
using iOverlay.Logic;
using iOverlay.Logic.WidgetLogic;
using RadiantConnect;
using RadiantConnect.Network.PVPEndpoints.DataTypes;
namespace iOverlay.Apps;

public partial class ValorantOverlay
{
    // Fields
    private Initiator _initiator = null!;
    private readonly Random _backgroundRandom = new(DateTime.Now.Millisecond);

    private string? _seasonId;
    private string? _userId;
    private bool _firstRun;

    private readonly List<string> _backgroundAssets =
    [
        "pack://application:,,,/Assets/Images/Lotus.png",
        "pack://application:,,,/Assets/Images/Icebox.png",
        "pack://application:,,,/Assets/Images/Fracture.png",
        "pack://application:,,,/Assets/Images/Haven.png",
        "pack://application:,,,/Assets/Images/Bind.png",
        "pack://application:,,,/Assets/Images/Breeze.png",
        "pack://application:,,,/Assets/Images/Split.png",
        "pack://application:,,,/Assets/Images/Ascent.png",
        "pack://application:,,,/Assets/Images/Sunset.png"
    ];

    public ValorantOverlay()
    {
        this.ApplyDraggable();
        InitializeComponent();
        BackSplash.ImageSource = new BitmapImage(new Uri(_backgroundRandom.GetRandomListItem(_backgroundAssets)));

    }

    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        _initiator = new Initiator();
        _userId = _initiator.ExternalSystem.ClientData.UserId;
        Content? content = await _initiator.Endpoints.PvpEndpoints.FetchContentAsync();
        _seasonId = content?.Seasons.FirstOrDefault(s => s.IsActive!.Value && s.Name.Contains("ACT"))?.ID;

        RunRankCheck();

        _initiator.GameEvents.Match.OnMatchEnded += async _ =>
        {
            await Task.Delay(3000);
            RunRankCheck();
        };
    }

    internal async Task<Dictionary<Match, MatchInfo?>> ParseMatches(IReadOnlyList<Match>? matches)
    {
        Dictionary<Match, MatchInfo?> matchDetail = new();

        for (int i = 0; i < matches?.Count; i++)
        {
            try
            {
                Match match = matches[i];
                matchDetail.Add(match, await _initiator.Endpoints.PvpEndpoints.FetchMatchInfoAsync(match.MatchID) ?? null);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        return matchDetail;
    }

    internal (double, double) GetRoundStats(Dictionary<Match, MatchInfo?> matchInfo)
    {
        double totalWins = 0;

        List<double> headshotPercentages = new();

        foreach ((Match? matchInternalDetails, MatchInfo? matchDetails) in matchInfo)
        {
            double totalHeadshots = 0;
            double totalHits = 0;

            IEnumerable<PlayerStat>? playerStats = matchDetails?.RoundResults.SelectMany(roundResult => roundResult.PlayerStats).Where(playerResults => playerResults.Subject == _userId);
            
            if (matchInternalDetails.RankedRatingEarned > 0)
                totalWins++;


            foreach (Damage damageHits in playerStats?.SelectMany(playerResults => playerResults.DamageInternal)!)
            {
                totalHeadshots += damageHits.Headshots ?? 0;
                totalHits += (damageHits.Bodyshots ?? 0) + (damageHits.Legshots ?? 0) + (damageHits.Headshots ?? 0);
            }

            headshotPercentages.Add(Math.Round((totalHeadshots / totalHits) * 100));
        }
        
        double headshotAverage = headshotPercentages.Count == 0 ? 0.0 : headshotPercentages.Average();

        return (totalWins, headshotAverage);
    }

    private async void RunRankCheck()
    {
        if (_userId is null) return;

        CompetitiveUpdate? competitiveUpdate = await _initiator.Endpoints.PvpEndpoints.FetchCompetitveUpdatesAsync(_userId);

        bool playedThisSeason = competitiveUpdate?.Matches.FirstOrDefault(x => x.SeasonID == _seasonId) != null;
        if (!playedThisSeason) return;

        string currentRankName = competitiveUpdate?.Matches[0].TierAfterUpdate?.ToString().Replace("_", "") ?? "Unranked";
        if (currentRankName.Contains("Unranked")) currentRankName = currentRankName[..7];

        long rankRating = competitiveUpdate?.Matches[0].RankedRatingAfterUpdate ?? 0;

        (double totalWins, double averageHeadshotPercent) = GetRoundStats(await ParseMatches(competitiveUpdate?.Matches));

        UpdateUiElements(
            new ValorantRank(currentRankName, InternalValorantLogic.RankIcon[currentRankName]),
            rankRating,
            $"{Math.Round(averageHeadshotPercent)}%",
            $"{Math.Round(totalWins / competitiveUpdate?.Matches.Count ?? 1, 1) * 100}%"
            );
    }

    private void UpdateUiElements(ValorantRank rank, long currentRankRating, string? headshotPercentage, string? winPercentage)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            PlayerRank.Content = rank.Rank;
            PlayerRank.Foreground = InternalValorantLogic.RankColors[(string)PlayerRank.Content!];

            if (_firstRun) SessionRrGain.AnimateSessionRankRating(currentRankRating);

            PlayerRankRating.AnimateRankRating(currentRankRating);
            RankRatingProgress.AnimateProgress(currentRankRating);

            PlayerWinPercent.Content = winPercentage;
            PlayerHeadshotPercent.Content = headshotPercentage;

            RankIcon.Source = new BitmapImage(new Uri(rank.RankIcon!));
            _firstRun = false;
        });
    }

}