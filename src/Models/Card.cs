using System.Text.Json.Serialization;

namespace TrelloCli.Models;

public class Card
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("desc")]
    public string? Desc { get; set; }

    [JsonPropertyName("idList")]
    public string ListId { get; set; } = string.Empty;

    [JsonPropertyName("idBoard")]
    public string BoardId { get; set; } = string.Empty;

    [JsonPropertyName("due")]
    public string? Due { get; set; }

    [JsonPropertyName("closed")]
    public bool Closed { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("pos")]
    public double Pos { get; set; }

    [JsonPropertyName("idLabels")]
    public List<string> LabelIds { get; set; } = new();

    [JsonPropertyName("idMembers")]
    public List<string> MemberIds { get; set; } = new();
}

public class Label
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("color")]
    public string? Color { get; set; }
}
