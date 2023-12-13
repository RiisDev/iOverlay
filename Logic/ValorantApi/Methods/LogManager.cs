using System.Diagnostics.CodeAnalysis;

namespace iOverlay.Logic.ValorantApi.Methods
{
    public record ClientData(ClientData.RegionCode Region, string UserId, string PdUrl, string GlzUrl, string sharedUrl)
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
    }

    public class LogManager
    {
        public ClientData ClientData;
        internal string CurrentLogText;

        public LogManager(string logText)
        {
            CurrentLogText = logText;
            string userId = MiscLogic.ExtractValue(CurrentLogText, "Logged in user changed: (.+)", 1);
            string pdUrl = MiscLogic.ExtractValue(CurrentLogText, @"https://pd\.[^\s]+\.net/", 0);
            string glzUrl = MiscLogic.ExtractValue(CurrentLogText, @"https://glz[^\s]+\.net/", 0);
            string regionData = MiscLogic.ExtractValue(CurrentLogText, @"https://pd\.([^\.]+)\.a\.pvp\.net/", 1);
            string sharedUrl = $"https://shared.{regionData}.a.pvp.net";
            _ = Enum.TryParse(regionData, out ClientData.RegionCode region);
            ClientData = new ClientData(region, userId, pdUrl, glzUrl, sharedUrl);
        }
    }
}
