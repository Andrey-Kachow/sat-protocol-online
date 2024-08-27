using System.Linq;

class Utils
{
    public static string ArrayToPrettyString(object[] array)
    {
        return string.Join(", ", array.Select(element => element?.ToString() ?? "null"));
    }
}