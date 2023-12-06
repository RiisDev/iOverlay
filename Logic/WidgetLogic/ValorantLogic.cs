#pragma warning disable CA2211
#pragma warning disable CA1822

using System.Net.Http;
using System.Text.Json;
using System.Windows;
using System.Windows.Media;

namespace iOverlay.Logic.WidgetLogic;

public class RankIcons
{
    public static Dictionary<string, string> RankIcon = new()
    {
        { "Unranked", "pack://application:,,,/Assets/Images/Unranked.png" },
        { "Iron 1", "pack://application:,,,/Assets/Images/Iron 1.png" },
        { "Iron 2", "pack://application:,,,/Assets/Images/Iron 2.png" },
        { "Iron 3", "pack://application:,,,/Assets/Images/Iron 3.png" },
        { "Bronze 1", "pack://application:,,,/Assets/Images/Bronze 1.png" },
        { "Bronze 2", "pack://application:,,,/Assets/Images/Bronze 2.png" },
        { "Bronze 3", "pack://application:,,,/Assets/Images/Bronze 3.png" },
        { "Silver 1", "pack://application:,,,/Assets/Images/Silver 1.png" },
        { "Silver 2", "pack://application:,,,/Assets/Images/Silver 2.png" },
        { "Silver 3", "pack://application:,,,/Assets/Images/Silver 3.png" },
        { "Gold 1", "pack://application:,,,/Assets/Images/Gold 1.png" },
        { "Gold 2", "pack://application:,,,/Assets/Images/Gold 2.png" },
        { "Gold 3", "pack://application:,,,/Assets/Images/Gold 3.png" },
        { "Platinum 1", "pack://application:,,,/Assets/Images/Platinum 1.png" },
        { "Platinum 2", "pack://application:,,,/Assets/Images/Platinum 2.png" },
        { "Platinum 3", "pack://application:,,,/Assets/Images/Platinum 3.png" },
        { "Diamond 1", "pack://application:,,,/Assets/Images/Diamond 1.png" },
        { "Diamond 2", "pack://application:,,,/Assets/Images/Diamond 2.png" },
        { "Diamond 3", "pack://application:,,,/Assets/Images/Diamond 3.png" },
        { "Ascendant 1", "pack://application:,,,/Assets/Images/Ascendant 1.png" },
        { "Ascendant 2", "pack://application:,,,/Assets/Images/Ascendant 2.png" },
        { "Ascendant 3", "pack://application:,,,/Assets/Images/Ascendant 3.png" },
        { "Immortal 1", "pack://application:,,,/Assets/Images/Immortal 1.png" },
        { "Immortal 2", "pack://application:,,,/Assets/Images/Immortal 2.png" },
        { "Immortal 3", "pack://application:,,,/Assets/Images/Immortal 3.png" },
        { "Radiant", "pack://application:,,,/Assets/Images/Radiant.png" }
    };

    public static Dictionary<string, Brush> RankColors = new()
    {
        { "Unranked", new SolidColorBrush(Color.FromRgb(137, 140, 141)) },
        { "Iron 1", new SolidColorBrush(Color.FromRgb(195, 193, 196)) },
        { "Iron 2", new SolidColorBrush(Color.FromRgb(195, 193, 196)) },
        { "Iron 3", new SolidColorBrush(Color.FromRgb(195, 193, 196)) },
        { "Bronze 1", new SolidColorBrush(Color.FromRgb(151, 107, 25)) },
        { "Bronze 2", new SolidColorBrush(Color.FromRgb(151, 107, 25)) },
        { "Bronze 3", new SolidColorBrush(Color.FromRgb(151, 107, 25)) },
        { "Silver 1", new SolidColorBrush(Color.FromRgb(203, 209, 206)) },
        { "Silver 2", new SolidColorBrush(Color.FromRgb(203, 209, 206)) },
        { "Silver 3", new SolidColorBrush(Color.FromRgb(203, 209, 206)) },
        { "Gold 1", new SolidColorBrush(Color.FromRgb(214, 138, 33)) },
        { "Gold 2", new SolidColorBrush(Color.FromRgb(214, 138, 33)) },
        { "Gold 3", new SolidColorBrush(Color.FromRgb(214, 138, 33)) },
        { "Platinum 1", new SolidColorBrush(Color.FromRgb(54, 151, 167)) },
        { "Platinum 2", new SolidColorBrush(Color.FromRgb(54, 151, 167)) },
        { "Platinum 3", new SolidColorBrush(Color.FromRgb(54, 151, 167)) },
        { "Diamond 1", new SolidColorBrush(Color.FromRgb(166, 111, 237)) },
        { "Diamond 2", new SolidColorBrush(Color.FromRgb(166, 111, 237)) },
        { "Diamond 3", new SolidColorBrush(Color.FromRgb(166, 111, 237)) },
        { "Ascendant 1", new SolidColorBrush(Color.FromRgb(33, 168, 96)) },
        { "Ascendant 2", new SolidColorBrush(Color.FromRgb(33, 168, 96)) },
        { "Ascendant 3", new SolidColorBrush(Color.FromRgb(33, 168, 96)) },
        { "Immortal 1", new SolidColorBrush(Color.FromRgb(173, 54, 113)) },
        { "Immortal 2", new SolidColorBrush(Color.FromRgb(173, 54, 113)) },
        { "Immortal 3", new SolidColorBrush(Color.FromRgb(173, 54, 113)) },
        { "Radiant", new SolidColorBrush(Color.FromRgb(255, 255, 186)) }
    };
}

public record ValorantRank(string? Rank, string? RankIcon)
{
    public string? Rank { get; set; } = Rank;
    public string? RankIcon { get; set; } = RankIcon;

    public static ValorantRank Default()
    {
        return new ValorantRank("Unranked", RankIcons.RankIcon["Unranked"]);
    }
}

public class ValorantLogic
{
    internal HttpClient HttpClient = new();

    public string? InternalRiotParse(string? riotParse, bool tracker = false)
    {
        return riotParse?.Replace(" ", "%20").Replace("#", tracker ? "%23" : "/");
    }

    public async Task<(ValorantRank, int)> GetCurrentRank(string? riotId)
    {
        Retry:
        HttpResponseMessage response =
            await HttpClient.GetAsync(
                $"https://api.kyroskoh.xyz/valorant/v1/mmr/NA/{InternalRiotParse(riotId)}?show=combo&display=0");
        string netResponse = await response.Content.ReadAsStringAsync();

        MessageBox.Show(netResponse);

        if (netResponse.Contains("429"))
        {
            await Task.Delay(5000);
            goto Retry;
        }
        if (!netResponse.Contains("rr", StringComparison.CurrentCultureIgnoreCase)) goto Retry;


        string rank = netResponse[..(netResponse.IndexOf('-', StringComparison.Ordinal) - 1)];
        int rankRating =
            int.Parse(netResponse[(netResponse.IndexOf('-', StringComparison.Ordinal) + 2)..netResponse.IndexOf('R')]);

        return !rank.Contains("failed")
            ? (new ValorantRank(rank, RankIcons.RankIcon[rank]), rankRating)
            : (ValorantRank.Default(), 0);
    }

    public async Task<(bool, string)> IsValidUser(string? riotId, bool checkTracker)
    {
        if (string.IsNullOrEmpty(riotId)) return (false, "empty_riot");
        if (string.IsNullOrWhiteSpace(riotId)) return (false, "empty_riot");

        if (!checkTracker)
        {
            HttpResponseMessage response = await HttpClient.GetAsync(
                $"https://api.kyroskoh.xyz/valorant/v1/mmr/NA/{InternalRiotParse(riotId)}?show=rankonly&display=0");
            string rank = await response.Content.ReadAsStringAsync();
            return (!rank.Contains("failed"), "failed_kyroskoh");
        }

        string trackerData =
            await MiscLogic.GetTrackerJson(
                $"https://api.tracker.gg/api/v2/valorant/standard/profile/riot/{InternalRiotParse(riotId, true)}?forceCollect=true&source=web") ??
            "errors";

        return (!trackerData.Contains("errors"), trackerData);
    }

    public static Task<string?> GetDisplayValue(string statValue, string cachedData)
    {
        JsonDocument userData = JsonDocument.Parse(cachedData);
        JsonElement data = userData.RootElement.GetProperty("data");
        JsonElement segments = data.GetProperty("segments");
        JsonElement stats = segments[0].GetProperty("stats");
        JsonElement stat = stats.GetProperty(statValue);

        return Task.FromResult(stat.TryGetProperty("displayValue", out JsonElement displayValue)
            ? displayValue.GetString()
            : null);
    }

    public async Task<string?> GetHeadshotPercentage(string cachedData)
    {
        string? hsPercentage = await GetDisplayValue("headshotsPercentage", cachedData);
        return cachedData.Contains("errors") ? "N/A" : hsPercentage;
    }

    public async Task<string?> GetWinPercentage(string cachedData)
    {
        string? roundsWinPct = await GetDisplayValue("matchesWinPct", cachedData);
        return cachedData.Contains("errors") ? "N/A" : roundsWinPct;
    }
}