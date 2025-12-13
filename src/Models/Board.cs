using System.Text.Json.Serialization;

namespace TrelloCli.Models;

public class Board
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("desc")]
    public string? Desc { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("closed")]
    public bool Closed { get; set; }
}
