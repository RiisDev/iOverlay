using System.Diagnostics;
using System.Text.Json;
using iOverlay.Logic.ValorantApi.DataTypes;
using iOverlay.Logic.ValorantApi.Methods;
using iOverlay.Logic.WidgetLogic;

namespace iOverlay.Logic.ValorantApi.Clients
{
    public class UserClient
    {
        public LogManager LogStats;
        internal NetHandler Net;
        private readonly ValorantLogic _valorantLogic = new();

        public ValorantClient ValorantClient { get; set; }

        public ValorantRank Rank { get; set; }

        public string SeasonId { get; set; }

        public string? KillDeath { get; set; }
        public string? WinRate { get; set; }
        public string? Headshot { get; set; }
        public int RankRating { get; set; }
        public int LastRatingChange { get; set; }

        internal async Task<Dictionary<string, MatchDetails?>?> ParseMatches(IReadOnlyList<MatchData>? matches)
        {
            Dictionary<string, MatchDetails?> matchDetail = new();

            for (int i = 0; i < matches?.Count; i++)
            {
                try
                {
                    MatchData match = matches[i];
                    string? matchDetails = await Net.GetAsync(LogStats.ClientData.PdUrl, $"/match-details/v1/matches/{match.MatchID}");
                    if (string.IsNullOrEmpty(matchDetails)) continue;
                    if (matchDetails.Contains("MATCH_NOT_FOUND")) continue;

                    matchDetail.Add(match.MatchID, JsonSerializer.Deserialize<MatchDetails>(matchDetails));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }

            return matchDetail;
        }

        internal (double, double, double, double) GetRoundStats(MatchDataContainer matchContainer)
        {
            double totalWins = 0;
            double totalKills = 0;
            double totalDeaths = 0;

            List<double> headshotPercentages = new();

            foreach (MatchDetails? matchDetails in matchContainer.MatchDetails?.Values!)
            {
                double totalHeadshots = 0;
                double totalHits = 0;

                IEnumerable<PlayerStat>? playerStats = matchDetails?.RoundResults.SelectMany(roundResult => roundResult.PlayerStats).Where(playerResults => playerResults.Subject == LogStats.ClientData.UserId);
                MatchData matchInfo = matchContainer.Matches!.First(x => x.MatchID == matchDetails?.MatchInfo.MatchId);

                if (matchInfo.RankedRatingEarned > 0)
                    totalWins++;

                totalKills = matchDetails!.Players.Where(roundResult => roundResult.Subject == LogStats.ClientData.UserId).Sum(roundResult => roundResult.Stats.Kills ?? 0);
                totalDeaths = matchDetails.Players.Where(roundResult => roundResult.Subject == LogStats.ClientData.UserId).Sum(roundResult => roundResult.Stats.Deaths ?? 0);


                foreach (DamageData damageHits in playerStats?.SelectMany(playerResults => playerResults.Damage)!)
                {
                    totalHeadshots += damageHits.Headshots ?? 0;
                    totalHits += (damageHits.Bodyshots ?? 0) + (damageHits.Legshots ?? 0) + (damageHits.Headshots ?? 0);
                }

                headshotPercentages.Add(Math.Round((totalHeadshots / totalHits) * 100));
            }
            
            double headshotAverage = headshotPercentages.Count == 0 ? 0.0 : headshotPercentages.Average();

            return (totalWins, totalKills, totalDeaths, headshotAverage);
        }

        public async Task<(ValorantRank, double, double, double, int, int)> GetRankingStatsAsync(int matchCount = 15, string queueType = "competitive")
        {
            string? data = await Net.GetAsync(LogStats.ClientData.PdUrl, $"/mmr/v1/players/{LogStats.ClientData.UserId}/competitiveupdates?startIndex=0&endIndex={matchCount}&queue={queueType}");

            if (string.IsNullOrEmpty(data)) return (ValorantRank.Default(), 0.0, 0.0, 0.0, 0, 0);

            MatchDataContainer? matchContainer = JsonSerializer.Deserialize<MatchDataContainer>(data);

            bool playedThisSeason = matchContainer?.Matches?.FirstOrDefault(x => x.SeasonID == SeasonId) != null;

            if (!playedThisSeason) return (ValorantRank.Default(), 0.0, 0.0, 0.0, 0, 0);

            string rankName = ((RankIndex.Ranks)matchContainer?.Matches?[0].TierAfterUpdate!).ToString().Replace("_", " ");

            if (rankName.Contains("Unranked")) rankName = rankName[..7];

            int? rankRatingChanged = matchContainer.Matches?[0].RankedRatingEarned;
            int? rankRating = matchContainer.Matches?[0].RankedRatingAfterUpdate;

            matchContainer.MatchDetails = await ParseMatches(matchContainer.Matches);

            (double totalWins, double totalKills, double totalDeaths, double averageHeadshotPercent) = GetRoundStats(matchContainer);

            return (
                new ValorantRank(rankName, RankIcons.RankIcon[rankName]),
                totalKills / totalDeaths,
                totalWins / matchContainer.Matches?.Count ?? 1,
                averageHeadshotPercent, rankRating ?? 0,
                rankRatingChanged ?? 0
            );
        }

        public async Task SetSeasonId()
        {
            string? data = await Net.GetAsync(LogStats.ClientData.sharedUrl, "/content-service/v3/content");

            if (string.IsNullOrEmpty(data)) return;

            Content? seasonContent = JsonSerializer.Deserialize<Content>(data);

            Season? season = seasonContent?.Seasons.FirstOrDefault(s => s.IsActive && s.Name.Contains("ACT"));

            SeasonId = season?.SeasonId ?? "";
        }

        public async Task GetStats()
        {
            if (string.IsNullOrEmpty(SeasonId)) await SetSeasonId();
            (ValorantRank? rank, double killDeathRatio, double winLossRatio, double headshotRatio, int rankRating, int lastRatingChange) = await GetRankingStatsAsync();

            Rank = rank;
            KillDeath = Math.Round(killDeathRatio, 2).ToString();
            WinRate = $"{Math.Round(winLossRatio, 1) * 100}%";
            Headshot = $"{Math.Round(headshotRatio)}%";
            RankRating = rankRating;
            LastRatingChange = lastRatingChange;
        }

        public UserClient(ValorantClient config)
        {
            ValorantClient = config;
            LogStats = new LogManager(_valorantLogic.GetLogText());
            Rank = ValorantRank.Default();
            Net = new NetHandler(this);
        }
    }
}
