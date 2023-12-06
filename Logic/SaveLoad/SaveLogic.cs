namespace iOverlay.Logic.SaveLoad;

public class SaveLogic
{
    public record ValorantSettings(string? RiotName, bool ShowStats)
    {
        public string? RiotName { get; set; } = RiotName;
        public bool ShowStats { get; set; } = ShowStats;
    }

    public record SpotifySettings(bool DarkMode)
    {
    }
}