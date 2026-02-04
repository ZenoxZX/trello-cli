using TrelloCli.Models;
using TrelloCli.Services;
using TrelloCli.Utils;

namespace TrelloCli.Commands;

public class CardCommands
{
    private readonly TrelloApiService _api;

    public CardCommands(TrelloApiService api)
    {
        _api = api;
    }

    public async Task GetCardsAsync(string listId)
    {
        if (string.IsNullOrEmpty(listId))
        {
            OutputFormatter.Print(ApiResponse<object>.Fail("List ID required", "MISSING_PARAM"));
            return;
        }

        var result = await _api.GetCardsInListAsync(listId);
        OutputFormatter.Print(result);
    }

    public async Task GetAllCardsAsync(string boardId)
    {
        if (string.IsNullOrEmpty(boardId))
        {
            OutputFormatter.Print(ApiResponse<object>.Fail("Board ID required", "MISSING_PARAM"));
            return;
        }

        var result = await _api.GetCardsInBoardAsync(boardId);
        OutputFormatter.Print(result);
    }

    public async Task GetCardAsync(string cardId)
    {
        if (string.IsNullOrEmpty(cardId))
        {
            OutputFormatter.Print(ApiResponse<object>.Fail("Card ID required", "MISSING_PARAM"));
            return;
        }

        var result = await _api.GetCardAsync(cardId);
        OutputFormatter.Print(result);
    }

    public async Task CreateCardAsync(string listId, string name, string? desc = null, string? due = null)
    {
        if (string.IsNullOrEmpty(listId))
        {
            OutputFormatter.Print(ApiResponse<object>.Fail("List ID required", "MISSING_PARAM"));
            return;
        }

        if (string.IsNullOrEmpty(name))
        {
            OutputFormatter.Print(ApiResponse<object>.Fail("Card name required", "MISSING_PARAM"));
            return;
        }

        var result = await _api.CreateCardAsync(listId, name, desc, due);
        OutputFormatter.Print(result);
    }

    public async Task UpdateCardAsync(string cardId, string? name, string? desc, string? due, string? labels, string? members, bool? closed = null)
    {
        if (string.IsNullOrEmpty(cardId))
        {
            OutputFormatter.Print(ApiResponse<object>.Fail("Card ID required", "MISSING_PARAM"));
            return;
        }

        var result = await _api.UpdateCardAsync(cardId, name, desc, due, null, labels, members, closed);
        OutputFormatter.Print(result);
    }

    public async Task ArchiveCardAsync(string cardId)
    {
        if (string.IsNullOrEmpty(cardId))
        {
            OutputFormatter.Print(ApiResponse<object>.Fail("Card ID required", "MISSING_PARAM"));
            return;
        }

        var result = await _api.UpdateCardAsync(cardId, closed: true);
        OutputFormatter.Print(result);
    }

    public async Task UnarchiveCardAsync(string cardId)
    {
        if (string.IsNullOrEmpty(cardId))
        {
            OutputFormatter.Print(ApiResponse<object>.Fail("Card ID required", "MISSING_PARAM"));
            return;
        }

        var result = await _api.UpdateCardAsync(cardId, closed: false);
        OutputFormatter.Print(result);
    }

    public async Task MoveCardAsync(string cardId, string listId)
    {
        if (string.IsNullOrEmpty(cardId))
        {
            OutputFormatter.Print(ApiResponse<object>.Fail("Card ID required", "MISSING_PARAM"));
            return;
        }

        if (string.IsNullOrEmpty(listId))
        {
            OutputFormatter.Print(ApiResponse<object>.Fail("List ID required", "MISSING_PARAM"));
            return;
        }

        var result = await _api.MoveCardAsync(cardId, listId);
        OutputFormatter.Print(result);
    }

    public async Task DeleteCardAsync(string cardId)
    {
        if (string.IsNullOrEmpty(cardId))
        {
            OutputFormatter.Print(ApiResponse<object>.Fail("Card ID required", "MISSING_PARAM"));
            return;
        }

        var result = await _api.DeleteCardAsync(cardId);
        OutputFormatter.Print(result);
    }

    public async Task GetCommentsAsync(string cardId)
    {
        if (string.IsNullOrEmpty(cardId))
        {
            OutputFormatter.Print(ApiResponse<object>.Fail("Card ID required", "MISSING_PARAM"));
            return;
        }

        var result = await _api.GetCommentsAsync(cardId);
        OutputFormatter.Print(result);
    }

    public async Task AddCommentAsync(string cardId, string text)
    {
        if (string.IsNullOrEmpty(cardId))
        {
            OutputFormatter.Print(ApiResponse<object>.Fail("Card ID required", "MISSING_PARAM"));
            return;
        }

        if (string.IsNullOrEmpty(text))
        {
            OutputFormatter.Print(ApiResponse<object>.Fail("Comment text required", "MISSING_PARAM"));
            return;
        }

        var result = await _api.AddCommentAsync(cardId, text);
        OutputFormatter.Print(result);
    }
}
