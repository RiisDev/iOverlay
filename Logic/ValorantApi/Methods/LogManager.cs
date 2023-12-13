using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
#pragma warning disable IDE0060

namespace iOverlay.Logic.ValorantApi.Methods
{
    public record ClientData(ClientData.RegionCode Region, string UserId, string PdUrl, string GlzUrl)
    {
        [SuppressMessage("ReSharper", "UnusedMember.Global")]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public enum RegionCode
        {
            na,
            latam,
            br,
            eu,
            ap,
            kr,
        }

        public RegionCode Region { get; } = Region;
        public string UserId { get; } = UserId;
        public string PdUrl { get; } = PdUrl;
        public string GlzUrl { get; } = GlzUrl;
    }

    public class LogManager
    {
        public ClientData ClientData;
        internal string CurrentLogText;

        public LogManager(string logText)
        {
            CurrentLogText = logText;
            string userId = GetUserId();
            string pdUrl = GetPdUrl();
            string glzUrl = GetGlzUrl();
            string regionData = GetRegion(pdUrl);
            _ = Enum.TryParse(regionData, out ClientData.RegionCode region);
            ClientData = new ClientData(region, userId, pdUrl, glzUrl);
        }

        private static string ExtractValue(Match match, int groupId)
        {
            return match.Groups[groupId].Value.Replace("\r", "").Replace("\n", "");
        }

        private string GetUserId()
        {
            Match userIdMatch = Regex.Match(CurrentLogText, "Logged in user changed: (.+)");
            return userIdMatch is not { Success: true } ? "" : ExtractValue(userIdMatch, 1);
        }

        private string GetPdUrl()
        {
            Match userIdMatch = Regex.Match(CurrentLogText, @"https://pd\.[^\s]+\.net/");
            return userIdMatch is not { Success: true } ? "" : ExtractValue(userIdMatch, 0);
        }

        private string GetGlzUrl()
        {
            Match userIdMatch = Regex.Match(CurrentLogText, @"https://glz[^\s]+\.net/");
            return userIdMatch is not { Success: true } ? "" : ExtractValue(userIdMatch, 0);
        }

        private string GetRegion(string pdUrl)
        {
            Match userIdMatch = Regex.Match(pdUrl, @"https://pd\.([^\.]+)\.a\.pvp\.net/");
            return userIdMatch is not { Success: true } ? "" : ExtractValue(userIdMatch, 1);
        }
    }
}
