using System.Diagnostics;
using System.IO;
using System.Text.Json.Serialization;
using iOverlay.Logic.WidgetLogic;

namespace iOverlay.Logic.ValorantApi.Clients
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
        
        public ValorantClient()
        {
            string logFilePath = _valorantLogic.Log;

            if (!File.Exists(logFilePath)) return;

            using FileStream logStream = new(_valorantLogic.Log, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using StreamReader logReader = new(logStream);
            string fileText = logReader.ReadToEnd();

            string ciServerVersion = MiscLogic.ExtractValue(fileText, "CI server version: (.+)", 1);
            string branch = MiscLogic.ExtractValue(fileText, "Branch: (.+)", 1);
            string changelist = MiscLogic.ExtractValue(fileText, @"Changelist: (\d+)", 1);
            string buildVersion = MiscLogic.ExtractValue(fileText, @"Build version: (\d+)", 1);

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
