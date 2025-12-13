using System.Text.Json.Serialization;

namespace TrelloCli.Models;

public class TrelloList
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("idBoard")]
    public string BoardId { get; set; } = string.Empty;

    [JsonPropertyName("closed")]
    public bool Closed { get; set; }

    [JsonPropertyName("pos")]
    public double Pos { get; set; }
}
