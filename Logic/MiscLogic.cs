using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace iOverlay.Logic;

public static class MiscLogic
{
    public static T GetRandomListItem<T>(this Random random, IEnumerable<T> list)
    {
        IEnumerable<T> enumerable = list.ToList();
        return enumerable.ElementAt(random.Next(enumerable.Count()));
    }

    public static string ExtractValue(string haystack, [StringSyntax(StringSyntaxAttribute.Regex)] string pattern, int groupId)
    {
        Match match = Regex.Match(haystack, pattern);
        return match is not { Success: true } ? "" : match.Groups[groupId].Value.Replace("\r", "").Replace("\n", "");
    }

}