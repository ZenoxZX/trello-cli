using System.Text.Json.Serialization;

namespace TrelloCli.Models;

public class ApiResponse<T>
{
    [JsonPropertyName("ok")]
    public bool Ok { get; set; }

    [JsonPropertyName("data")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public T? Data { get; set; }

    [JsonPropertyName("error")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Error { get; set; }

    [JsonPropertyName("code")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Code { get; set; }

    public static ApiResponse<T> Success(T data) => new() { Ok = true, Data = data };

    public static ApiResponse<T> Fail(string error, string code = "ERROR") => new()
    {
        Ok = false,
        Error = error,
        Code = code
    };
}
