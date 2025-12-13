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
}
