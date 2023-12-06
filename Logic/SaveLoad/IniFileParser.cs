using System.IO;
using System.Text;

namespace iOverlay.Logic.SaveLoad;

public class IniFileParser
{
    private readonly Dictionary<string, Dictionary<string, string>> _sections = new(StringComparer.OrdinalIgnoreCase);

    public void Load(string filePath)
    {
        _sections.Clear();
        string? currentSection = null;

        if (!File.Exists(filePath))
        {
            SetValue("Valorant", "RiotId", "");
            SetValue("Valorant", "ShowStats", "true");

            SetValue("Spotify", "", "");

            Save(filePath);
        }

        foreach (string line in File.ReadLines(filePath))
        {
            string trimmedLine = line.Trim();

            if (trimmedLine.Length == 0)
                continue;

            if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]"))
            {
                if (trimmedLine.Length > 1)
                    currentSection = trimmedLine[1..^1];
                if (currentSection != null)
                    _sections[currentSection] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            }
            else if (currentSection != null)
            {
                int separatorIndex = trimmedLine.IndexOf('=');
                if (separatorIndex == -1) continue;
                string key = trimmedLine[..separatorIndex].Trim();
                string value = trimmedLine[(separatorIndex + 1)..].Trim();
                _sections[currentSection][key] = value;
            }
        }
    }

    public string? GetValue(string? section, string key, string? defaultValue = null)
    {
        if (section != null && _sections.TryGetValue(section, out Dictionary<string, string>? sectionData) &&
            sectionData.TryGetValue(key, out string? value)) return value;
        return defaultValue;
    }

    public bool GetBoolValue(string? section, string key, string? defaultValue = null)
    {
        if (section != null && _sections.TryGetValue(section, out Dictionary<string, string>? sectionData) &&
            sectionData.TryGetValue(key, out string? value)) return value is "true" or "True";
        return false;
    }

    public void SetValue(string section, string key, string value)
    {
        if (!_sections.TryGetValue(section, out Dictionary<string, string>? sectionData))
        {
            sectionData = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            _sections[section] = sectionData;
        }

        sectionData[key] = value;
    }

    public void Save(string filePath)
    {
        using StreamWriter writer = new(filePath, false, Encoding.UTF8);

        foreach (KeyValuePair<string, Dictionary<string, string>> section in _sections)
        {
            writer.WriteLine($"[{section.Key}]");
            foreach (KeyValuePair<string, string> entry in section.Value)
                writer.WriteLine($"{entry.Key} = {entry.Value}");
            writer.WriteLine();
        }
    }
}