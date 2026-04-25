using TrelloCli.Models;
using TrelloCli.Services;
using TrelloCli.Utils;

namespace TrelloCli.Commands;

public class LabelCommands
{
    private readonly TrelloApiService _api;

    public LabelCommands(TrelloApiService api)
    {
        _api = api;
    }

    public async Task GetLabelsAsync(string boardId)
    {
        if (string.IsNullOrEmpty(boardId))
        {
            OutputFormatter.Print(ApiResponse<object>.Fail("Board ID required", "MISSING_PARAM"));
            return;
        }

        var result = await _api.GetLabelsAsync(boardId);
        OutputFormatter.Print(result);
    }

    public async Task CreateLabelAsync(string boardId, string name, string? color)
    {
        if (string.IsNullOrEmpty(boardId))
        {
            OutputFormatter.Print(ApiResponse<object>.Fail("Board ID required", "MISSING_PARAM"));
            return;
        }

        if (string.IsNullOrEmpty(name))
        {
            OutputFormatter.Print(ApiResponse<object>.Fail("Label name required", "MISSING_PARAM"));
            return;
        }

        var result = await _api.CreateLabelAsync(boardId, name, color);
        OutputFormatter.Print(result);
    }

    public async Task UpdateLabelAsync(string labelId, string? name, string? color)
    {
        if (string.IsNullOrEmpty(labelId))
        {
            OutputFormatter.Print(ApiResponse<object>.Fail("Label ID required", "MISSING_PARAM"));
            return;
        }

        var result = await _api.UpdateLabelAsync(labelId, name, color);
        OutputFormatter.Print(result);
    }

    public async Task DeleteLabelAsync(string labelId)
    {
        if (string.IsNullOrEmpty(labelId))
        {
            OutputFormatter.Print(ApiResponse<object>.Fail("Label ID required", "MISSING_PARAM"));
            return;
        }

        var result = await _api.DeleteLabelAsync(labelId);
        OutputFormatter.Print(result);
    }
}
