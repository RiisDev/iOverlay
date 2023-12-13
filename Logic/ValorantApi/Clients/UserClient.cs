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
                MatchData match = matches[i];
                string? matchDetails = await Net.GetAsync(LogStats.ClientData.PdUrl, $"/match-details/v1/matches/{match.MatchID}");
                if (string.IsNullOrEmpty(matchDetails)) continue;

                matchDetail.Add(match.MatchID, JsonSerializer.Deserialize<MatchDetails>(matchDetails));
            }

            return matchDetail;
        }

        public async Task<(ValorantRank, double, double, double, int, int)> GetRankingStatsAsync(int matchCount = 15, string queueType = "competitive")
        {
            string? data = await Net.GetAsync(LogStats.ClientData.PdUrl, $"/mmr/v1/players/{LogStats.ClientData.UserId}/competitiveupdates?startIndex=0&endIndex={matchCount}&queue={queueType}");

            if (string.IsNullOrEmpty(data)) return (ValorantRank.Default(), 0.0, 0.0, 0.0, 0, 0);

            MatchDataContainer? matchContainer = JsonSerializer.Deserialize<MatchDataContainer>(data);

            string rankName = ((RankIndex.Ranks)matchContainer?.Matches?[0].TierAfterUpdate!).ToString().Replace("_", " ");
            if (rankName.Contains("Unranked"))
                rankName = rankName[..7];

            int? rankRatingChanged = matchContainer.Matches?[0].RankedRatingEarned;
            int? rankRating = matchContainer.Matches?[0].RankedRatingAfterUpdate;
            ValorantRank rank = new(rankName, RankIcons.RankIcon[rankName]);

            matchContainer.MatchDetails = await ParseMatches(matchContainer.Matches);

            double totalHeadshots = 1;
            double totalHits = 1;
            double totalWins = 1;
            double totalLosses = 1;
            double totalDeaths = 1;
            double totalKills = 1;

            foreach (MatchDetails? matchDetails in matchContainer.MatchDetails?.Values!)
            {
                IEnumerable<PlayerStat>? playerStats = matchDetails?.RoundResults.SelectMany(roundResult => roundResult.PlayerStats).Where(playerResults => playerResults.Subject == LogStats.ClientData.UserId);
                MatchData matchInfo = matchContainer.Matches!.First(x => x.MatchID == matchDetails?.MatchInfo.MatchId);

                totalLosses += matchInfo.RankedRatingEarned < 0 ? 1 : 0;
                totalWins += matchInfo.RankedRatingEarned > 0 ? 1 : 0;
                totalKills = matchDetails!.Players.Where(roundResult => roundResult.Subject == LogStats.ClientData.UserId).Sum(roundResult => roundResult.Stats.Kills ?? 0);
                totalDeaths = matchDetails.Players.Where(roundResult => roundResult.Subject == LogStats.ClientData.UserId).Sum(roundResult => roundResult.Stats.Deaths ?? 0);


                foreach (DamageData damageHits in playerStats?.SelectMany(playerResults => playerResults.Damage)!)
                {
                    totalHeadshots += damageHits.Headshots ?? 0;
                    totalHits += (damageHits.Bodyshots ?? 0) + (damageHits.Legshots ?? 0);
                }
            }

            return (rank, totalKills / totalDeaths, totalWins / totalLosses, totalHeadshots / totalHits, rankRating ?? 0, rankRatingChanged ?? 0);
        }

        public async Task GetStats()
        {
            (ValorantRank? rank, double killDeathRatio, double winLossRatio, double headshotRatio, int rankRating, int lastRatingChange) = await GetRankingStatsAsync();

            Rank = rank;
            KillDeath = Math.Round(killDeathRatio, 2).ToString();
            WinRate = $"{Math.Round(winLossRatio, 1) * 100}%";
            Headshot = $"{Math.Round(headshotRatio, 1) * 100}%";
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
