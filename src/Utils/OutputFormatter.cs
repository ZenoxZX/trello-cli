using System.Text.Json;
using System.Text.Json.Serialization;

namespace TrelloCli.Utils;

public static class OutputFormatter
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = false,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public static void Print<T>(T obj)
    {
        Console.WriteLine(JsonSerializer.Serialize(obj, Options));
    }

    public static string ToJson<T>(T obj)
    {
        return JsonSerializer.Serialize(obj, Options);
    }
}
