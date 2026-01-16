using System.Text.Json.Serialization;

namespace TrelloCli.Models;

public class Checklist
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("idCard")]
    public string IdCard { get; set; } = string.Empty;

    [JsonPropertyName("idBoard")]
    public string IdBoard { get; set; } = string.Empty;

    [JsonPropertyName("pos")]
    public double Pos { get; set; }

    [JsonPropertyName("checkItems")]
    public List<ChecklistItem> CheckItems { get; set; } = new();
}

public class ChecklistItem
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("state")]
    public string State { get; set; } = string.Empty;

    [JsonPropertyName("pos")]
    public double Pos { get; set; }

    [JsonPropertyName("idChecklist")]
    public string IdChecklist { get; set; } = string.Empty;
}
