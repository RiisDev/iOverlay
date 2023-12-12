using System.Text.Json;

namespace iOverlay.Logic;

public static class MiscLogic
{
   
    public static JsonElement? GetPropertyNullable(this JsonElement jsonElement, string propertyName)
    {
        try
        {
            if (jsonElement.TryGetProperty(propertyName, out JsonElement returnedElement)) return returnedElement;

            return null;
        }
        catch { return null; }
    }
    
    public static T GetRandomListItem<T>(this Random random, IEnumerable<T> list)
    {
        IEnumerable<T> enumerable = list.ToList();
        return enumerable.ElementAt(random.Next(enumerable.Count()));
    }
    
}