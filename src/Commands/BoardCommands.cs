using TrelloCli.Models;
using TrelloCli.Services;
using TrelloCli.Utils;

namespace TrelloCli.Commands;

public class BoardCommands
{
    private readonly TrelloApiService _api;

    public BoardCommands(TrelloApiService api)
    {
        _api = api;
    }

    public async Task GetBoardsAsync()
    {
        var result = await _api.GetBoardsAsync();
        OutputFormatter.Print(result);
    }

    public async Task GetBoardAsync(string boardId)
    {
        if (string.IsNullOrEmpty(boardId))
        {
            OutputFormatter.Print(ApiResponse<object>.Fail("Board ID required", "MISSING_PARAM"));
            return;
        }

        var result = await _api.GetBoardAsync(boardId);
        OutputFormatter.Print(result);
    }
}
