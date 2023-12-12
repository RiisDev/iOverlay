using System.Diagnostics;
using System.IO;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using iOverlay.Logic.WidgetLogic;

namespace iOverlay.ValorantApi.Clients
{
    public class ValorantClient
    {
        public record Version(
            [property: JsonPropertyName("RiotClientVersion")] string RiotClientVersion,
            [property: JsonPropertyName("Branch")] string Branch,
            [property: JsonPropertyName("BuildVersion")] string BuildVersion,
            [property: JsonPropertyName("Changelist")] string Changelist,
            [property: JsonPropertyName("EngineVersion")] string EngineVersion
        );

        public Version ValorantClientVersion { get; init; } = null!;
        private readonly ValorantLogic _valorantLogic = new();

        private static string ExtractValue(Match match)
        {
            return match.Groups[1].Value.Replace("\r", "").Replace("\n", "");
        }

        public ValorantClient()
        {
            string logFilePath = _valorantLogic.Log;

            if (!File.Exists(logFilePath)) return;

            using FileStream logStream = new(_valorantLogic.Log, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using StreamReader logReader = new(logStream);
            string fileText = logReader.ReadToEnd();

            Match ciServerVersionMatch = Regex.Match(fileText, "CI server version: (.+)");
            Match branchMatch = Regex.Match(fileText, "Branch: (.+)");
            Match changelistMatch = Regex.Match(fileText, @"Changelist: (\d+)");
            Match buildVersionMatch = Regex.Match(fileText, @"Build version: (\d+)");

            if (!ciServerVersionMatch.Success || !branchMatch.Success || !changelistMatch.Success || !buildVersionMatch.Success) return;

            string ciServerVersion = ExtractValue(ciServerVersionMatch);
            string branch = ExtractValue(branchMatch);
            string changelist = ExtractValue(changelistMatch);
            string buildVersion = ExtractValue(buildVersionMatch);
            string engineVersion = "";

            if (File.Exists(@"C:\Riot Games\VALORANT\live\ShooterGame\Binaries\Win64\VALORANT-Win64-Shipping.exe"))
            {
                FileVersionInfo fileInfo = FileVersionInfo.GetVersionInfo(@"C:\Riot Games\VALORANT\live\ShooterGame\Binaries\Win64\VALORANT-Win64-Shipping.exe");
                engineVersion = $"{fileInfo.FileMajorPart}.{fileInfo.FileMinorPart}.{fileInfo.FileBuildPart}.{fileInfo.FilePrivatePart}";
            }
            

            ValorantClientVersion = new Version(ciServerVersion, branch, buildVersion, changelist, engineVersion);
        }
    }
}
