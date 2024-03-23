using System.Diagnostics;
using System.Text.Json;
using System.Windows;
using System.Windows.Media.Imaging;
using iOverlay.Logic;
using iOverlay.Logic.WidgetLogic;
using RadiantConnect;
using RadiantConnect.Methods;
using RadiantConnect.Network.PVPEndpoints.DataTypes;
namespace iOverlay.Apps;

public partial class ValorantOverlay
{

    // Fields
    private Initiator _initiator = null!;
    private readonly Random _backgroundRandom = new(DateTime.Now.Millisecond);

    private string? _seasonId;
    private string? _lastMatchId;

    private bool _firstRun = true;

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
        Debug.WriteLine("Init");
        Content? content = await _initiator.Endpoints.PvpEndpoints.FetchContentAsync();
        _seasonId = content?.Seasons.FirstOrDefault(s => s.IsActive!.Value && s.Type == "act")?.ID;

        RunRankCheck();

        _initiator.GameEvents.Match.OnMatchEnded += async _ =>
        {
            Debug.WriteLine("RUN");
            await Task.Delay(20000);
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
            try
            {
                double totalHeadshots = 0;
                double totalHits = 0;

                List<PlayerStat>? playerStats = matchDetails?.RoundResults.SelectMany(roundResult => roundResult.PlayerStats).Where(playerResults => playerResults.Subject == _initiator.Client.UserId).ToList();

                if (matchInternalDetails.RankedRatingEarned > 0)
                    totalWins++;

                for (int playerStatIndex = 0; playerStatIndex < playerStats?.Count; playerStatIndex++)
                {
                    foreach (Damage damageHits in playerStats[playerStatIndex].DamageInternal)
                    {
                        totalHeadshots += damageHits.Headshots ?? 0;
                        totalHits += (damageHits.Bodyshots ?? 0) + (damageHits.Legshots ?? 0) + (damageHits.Headshots ?? 0);
                    }
                }

                headshotPercentages.Add(Math.Round((totalHeadshots / totalHits) * 100));
            }catch{}
        }
        
        double headshotAverage = headshotPercentages.Count == 0 ? 0.0 : headshotPercentages.Average();

        return (totalWins, headshotAverage);
    }

    private async void RunRankCheck()
    {
        CompetitiveUpdate? competitiveUpdate = await _initiator.Endpoints.PvpEndpoints.FetchCompetitveUpdatesAsync(_initiator.Client.UserId);

        bool playedThisSeason = competitiveUpdate?.Matches.FirstOrDefault(x => x.SeasonID == _seasonId) != null;
        if (!playedThisSeason) return;
        
        Debug.WriteLine("RAN CHECK");

        string currentRankName = ValorantTables.TierToRank[competitiveUpdate?.Matches[0].TierAfterUpdate!.Value ?? 0];

        long rankRating = competitiveUpdate?.Matches[0].RankedRatingEarned ?? 0;
        rankRating += competitiveUpdate?.Matches[0].RankedRatingPerformanceBonus ?? 0;

        bool increment = competitiveUpdate?.Matches[0].RankedRatingAfterUpdate > competitiveUpdate?.Matches[0].RankedRatingBeforeUpdate;

        (double totalWins, double averageHeadshotPercent) = GetRoundStats(await ParseMatches(competitiveUpdate?.Matches));

        UpdateUiElements(
            new ValorantRank(currentRankName, InternalValorantLogic.RankIcon[currentRankName]),
            rankRating,
            $"{Math.Round(averageHeadshotPercent)}%",
            $"{Math.Round(totalWins / competitiveUpdate?.Matches.Count ?? 1, 1) * 100}%",
            increment);
    }

    private void UpdateUiElements(ValorantRank rank, long currentRankRating, string? headshotPercentage, string? winPercentage, bool increment)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            PlayerRank.Content = rank.Rank;
            PlayerRank.Foreground = InternalValorantLogic.RankColors[(string)PlayerRank.Content!];

            if (!_firstRun) SessionRrGain.AnimateSessionRankRating(currentRankRating, increment);
            _firstRun = false;

            PlayerRankRating.AnimateRankRating(currentRankRating);
            RankRatingProgress.AnimateProgress(currentRankRating);

            PlayerWinPercent.Content = winPercentage;
            PlayerHeadshotPercent.Content = headshotPercentage;

            RankIcon.Source = new BitmapImage(new Uri(rank.RankIcon!));
        });
    }

}