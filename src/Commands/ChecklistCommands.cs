using TrelloCli.Models;
using TrelloCli.Services;
using TrelloCli.Utils;

namespace TrelloCli.Commands;

public class ChecklistCommands
{
    private readonly TrelloApiService _api;

    public ChecklistCommands(TrelloApiService api)
    {
        _api = api;
    }

    public async Task GetChecklistsAsync(string cardId)
    {
        if (string.IsNullOrEmpty(cardId))
        {
            OutputFormatter.Print(ApiResponse<object>.Fail("Card ID required", "MISSING_PARAM"));
            return;
        }

        var result = await _api.GetChecklistsAsync(cardId);
        OutputFormatter.Print(result);
    }

    public async Task CreateChecklistAsync(string cardId, string name)
    {
        if (string.IsNullOrEmpty(cardId))
        {
            OutputFormatter.Print(ApiResponse<object>.Fail("Card ID required", "MISSING_PARAM"));
            return;
        }

        if (string.IsNullOrEmpty(name))
        {
            OutputFormatter.Print(ApiResponse<object>.Fail("Checklist name required", "MISSING_PARAM"));
            return;
        }

        var result = await _api.CreateChecklistAsync(cardId, name);
        OutputFormatter.Print(result);
    }

    public async Task DeleteChecklistAsync(string checklistId)
    {
        if (string.IsNullOrEmpty(checklistId))
        {
            OutputFormatter.Print(ApiResponse<object>.Fail("Checklist ID required", "MISSING_PARAM"));
            return;
        }

        var result = await _api.DeleteChecklistAsync(checklistId);
        OutputFormatter.Print(result);
    }

    public async Task AddChecklistItemAsync(string checklistId, string name)
    {
        if (string.IsNullOrEmpty(checklistId))
        {
            OutputFormatter.Print(ApiResponse<object>.Fail("Checklist ID required", "MISSING_PARAM"));
            return;
        }

        if (string.IsNullOrEmpty(name))
        {
            OutputFormatter.Print(ApiResponse<object>.Fail("Item name required", "MISSING_PARAM"));
            return;
        }

        var result = await _api.AddChecklistItemAsync(checklistId, name);
        OutputFormatter.Print(result);
    }

    public async Task UpdateChecklistItemAsync(string cardId, string checkItemId, string state)
    {
        if (string.IsNullOrEmpty(cardId))
        {
            OutputFormatter.Print(ApiResponse<object>.Fail("Card ID required", "MISSING_PARAM"));
            return;
        }

        if (string.IsNullOrEmpty(checkItemId))
        {
            OutputFormatter.Print(ApiResponse<object>.Fail("Checklist item ID required", "MISSING_PARAM"));
            return;
        }

        if (string.IsNullOrEmpty(state))
        {
            OutputFormatter.Print(ApiResponse<object>.Fail("State required (complete or incomplete)", "MISSING_PARAM"));
            return;
        }

        var normalizedState = state.ToLowerInvariant();
        if (normalizedState != "complete" && normalizedState != "incomplete")
        {
            OutputFormatter.Print(ApiResponse<object>.Fail("State must be 'complete' or 'incomplete'", "INVALID_PARAM"));
            return;
        }

        var result = await _api.UpdateChecklistItemAsync(cardId, checkItemId, normalizedState);
        OutputFormatter.Print(result);
    }

    public async Task DeleteChecklistItemAsync(string checklistId, string checkItemId)
    {
        if (string.IsNullOrEmpty(checklistId))
        {
            OutputFormatter.Print(ApiResponse<object>.Fail("Checklist ID required", "MISSING_PARAM"));
            return;
        }

        if (string.IsNullOrEmpty(checkItemId))
        {
            OutputFormatter.Print(ApiResponse<object>.Fail("Checklist item ID required", "MISSING_PARAM"));
            return;
        }

        var result = await _api.DeleteChecklistItemAsync(checklistId, checkItemId);
        OutputFormatter.Print(result);
    }
}
