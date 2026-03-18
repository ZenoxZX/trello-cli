using TrelloCli.Models;
using TrelloCli.Services;
using TrelloCli.Utils;

namespace TrelloCli.Commands;

public class ListCommands
{
    private readonly TrelloApiService _api;

    public ListCommands(TrelloApiService api)
    {
        _api = api;
    }

    public async Task GetListsAsync(string boardId)
    {
        if (string.IsNullOrEmpty(boardId))
        {
            OutputFormatter.Print(ApiResponse<object>.Fail("Board ID required", "MISSING_PARAM"));
            return;
        }

        var result = await _api.GetListsAsync(boardId);
        OutputFormatter.Print(result);
    }

    public async Task CreateListAsync(string boardId, string name)
    {
        if (string.IsNullOrEmpty(boardId))
        {
            OutputFormatter.Print(ApiResponse<object>.Fail("Board ID required", "MISSING_PARAM"));
            return;
        }

        if (string.IsNullOrEmpty(name))
        {
            OutputFormatter.Print(ApiResponse<object>.Fail("List name required", "MISSING_PARAM"));
            return;
        }

        var result = await _api.CreateListAsync(boardId, name);
        OutputFormatter.Print(result);
    }

    public async Task MoveListAsync(string listId, string pos)
    {
        if (string.IsNullOrEmpty(listId))
        {
            OutputFormatter.Print(ApiResponse<object>.Fail("List ID required", "MISSING_PARAM"));
            return;
        }

        if (string.IsNullOrEmpty(pos))
        {
            OutputFormatter.Print(ApiResponse<object>.Fail("Position required (top, bottom, or a number)", "MISSING_PARAM"));
            return;
        }

        var result = await _api.MoveListAsync(listId, pos);
        OutputFormatter.Print(result);
    }

    public async Task BulkMoveListsAsync(string[] pairs)
    {
        if (pairs.Length == 0)
        {
            OutputFormatter.Print(ApiResponse<object>.Fail("At least one list-id:pos pair required", "MISSING_PARAM"));
            return;
        }

        var results = new List<object>();
        foreach (var pair in pairs)
        {
            var parts = pair.Split(':', 2);
            if (parts.Length != 2 || string.IsNullOrEmpty(parts[0]) || string.IsNullOrEmpty(parts[1]))
            {
                OutputFormatter.Print(ApiResponse<object>.Fail($"Invalid format '{pair}' — expected list-id:pos", "INVALID_PARAM"));
                return;
            }

            var result = await _api.MoveListAsync(parts[0], parts[1]);
            results.Add(new { listId = parts[0], pos = parts[1], ok = result.Ok, data = result.Data, error = result.Error });

            if (!result.Ok)
            {
                OutputFormatter.Print(ApiResponse<List<object>>.Fail($"Failed on '{pair}': {result.Error}", result.Code ?? "ERROR"));
                return;
            }
        }

        OutputFormatter.Print(ApiResponse<List<object>>.Success(results));
    }
}
