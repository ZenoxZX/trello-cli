using System.Text.Json;
using TrelloCli.Models;

namespace TrelloCli.Services;

public class TrelloApiService
{
    private readonly HttpClient _http;
    private readonly ConfigService _config;
    private const string BaseUrl = "https://api.trello.com/1";

    public TrelloApiService(ConfigService config)
    {
        _config = config;
        _http = new HttpClient();
    }

    private string BuildUrl(string endpoint, string? extraParams = null)
    {
        var sep = endpoint.Contains('?') ? "&" : "?";
        var url = $"{BaseUrl}{endpoint}{sep}{_config.GetAuthQuery()}";
        if (!string.IsNullOrEmpty(extraParams))
            url += $"&{extraParams}";
        return url;
    }

    // Board operations
    public async Task<ApiResponse<List<Board>>> GetBoardsAsync()
    {
        try
        {
            var url = BuildUrl("/members/me/boards", "filter=open");
            var response = await _http.GetStringAsync(url);
            var boards = JsonSerializer.Deserialize<List<Board>>(response) ?? new();
            return ApiResponse<List<Board>>.Success(boards);
        }
        catch (HttpRequestException ex)
        {
            return ApiResponse<List<Board>>.Fail(ex.Message, "HTTP_ERROR");
        }
        catch (Exception ex)
        {
            return ApiResponse<List<Board>>.Fail(ex.Message, "ERROR");
        }
    }

    public async Task<ApiResponse<Board>> GetBoardAsync(string boardId)
    {
        try
        {
            var url = BuildUrl($"/boards/{boardId}");
            var response = await _http.GetStringAsync(url);
            var board = JsonSerializer.Deserialize<Board>(response);
            return board != null
                ? ApiResponse<Board>.Success(board)
                : ApiResponse<Board>.Fail("Board not found", "NOT_FOUND");
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return ApiResponse<Board>.Fail("Board not found", "NOT_FOUND");
        }
        catch (HttpRequestException ex)
        {
            return ApiResponse<Board>.Fail(ex.Message, "HTTP_ERROR");
        }
        catch (Exception ex)
        {
            return ApiResponse<Board>.Fail(ex.Message, "ERROR");
        }
    }

    // List operations
    public async Task<ApiResponse<List<TrelloList>>> GetListsAsync(string boardId)
    {
        try
        {
            var url = BuildUrl($"/boards/{boardId}/lists", "filter=open");
            var response = await _http.GetStringAsync(url);
            var lists = JsonSerializer.Deserialize<List<TrelloList>>(response) ?? new();
            return ApiResponse<List<TrelloList>>.Success(lists);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return ApiResponse<List<TrelloList>>.Fail("Board not found", "NOT_FOUND");
        }
        catch (HttpRequestException ex)
        {
            return ApiResponse<List<TrelloList>>.Fail(ex.Message, "HTTP_ERROR");
        }
        catch (Exception ex)
        {
            return ApiResponse<List<TrelloList>>.Fail(ex.Message, "ERROR");
        }
    }

    public async Task<ApiResponse<TrelloList>> CreateListAsync(string boardId, string name)
    {
        try
        {
            var url = BuildUrl("/lists", $"name={Uri.EscapeDataString(name)}&idBoard={boardId}");
            var response = await _http.PostAsync(url, null);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var list = JsonSerializer.Deserialize<TrelloList>(content);
            return list != null
                ? ApiResponse<TrelloList>.Success(list)
                : ApiResponse<TrelloList>.Fail("Failed to create list", "CREATE_FAILED");
        }
        catch (HttpRequestException ex)
        {
            return ApiResponse<TrelloList>.Fail(ex.Message, "HTTP_ERROR");
        }
        catch (Exception ex)
        {
            return ApiResponse<TrelloList>.Fail(ex.Message, "ERROR");
        }
    }

    // Card operations
    public async Task<ApiResponse<List<Card>>> GetCardsInListAsync(string listId)
    {
        try
        {
            var url = BuildUrl($"/lists/{listId}/cards");
            var response = await _http.GetStringAsync(url);
            var cards = JsonSerializer.Deserialize<List<Card>>(response) ?? new();
            return ApiResponse<List<Card>>.Success(cards);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return ApiResponse<List<Card>>.Fail("List not found", "NOT_FOUND");
        }
        catch (HttpRequestException ex)
        {
            return ApiResponse<List<Card>>.Fail(ex.Message, "HTTP_ERROR");
        }
        catch (Exception ex)
        {
            return ApiResponse<List<Card>>.Fail(ex.Message, "ERROR");
        }
    }

    public async Task<ApiResponse<List<Card>>> GetCardsInBoardAsync(string boardId)
    {
        try
        {
            var url = BuildUrl($"/boards/{boardId}/cards", "filter=open");
            var response = await _http.GetStringAsync(url);
            var cards = JsonSerializer.Deserialize<List<Card>>(response) ?? new();
            return ApiResponse<List<Card>>.Success(cards);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return ApiResponse<List<Card>>.Fail("Board not found", "NOT_FOUND");
        }
        catch (HttpRequestException ex)
        {
            return ApiResponse<List<Card>>.Fail(ex.Message, "HTTP_ERROR");
        }
        catch (Exception ex)
        {
            return ApiResponse<List<Card>>.Fail(ex.Message, "ERROR");
        }
    }

    public async Task<ApiResponse<Card>> GetCardAsync(string cardId)
    {
        try
        {
            var url = BuildUrl($"/cards/{cardId}");
            var response = await _http.GetStringAsync(url);
            var card = JsonSerializer.Deserialize<Card>(response);
            return card != null
                ? ApiResponse<Card>.Success(card)
                : ApiResponse<Card>.Fail("Card not found", "NOT_FOUND");
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return ApiResponse<Card>.Fail("Card not found", "NOT_FOUND");
        }
        catch (HttpRequestException ex)
        {
            return ApiResponse<Card>.Fail(ex.Message, "HTTP_ERROR");
        }
        catch (Exception ex)
        {
            return ApiResponse<Card>.Fail(ex.Message, "ERROR");
        }
    }

    public async Task<ApiResponse<Card>> CreateCardAsync(string listId, string name, string? desc = null, string? due = null)
    {
        try
        {
            var url = BuildUrl("/cards");

            var formData = new Dictionary<string, string>
            {
                ["idList"] = listId,
                ["name"] = name
            };
            if (!string.IsNullOrEmpty(desc))
                formData["desc"] = desc;
            if (!string.IsNullOrEmpty(due))
                formData["due"] = due;

            var content = new FormUrlEncodedContent(formData);
            var response = await _http.PostAsync(url, content);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            var card = JsonSerializer.Deserialize<Card>(responseContent);
            return card != null
                ? ApiResponse<Card>.Success(card)
                : ApiResponse<Card>.Fail("Failed to create card", "CREATE_FAILED");
        }
        catch (HttpRequestException ex)
        {
            return ApiResponse<Card>.Fail(ex.Message, "HTTP_ERROR");
        }
        catch (Exception ex)
        {
            return ApiResponse<Card>.Fail(ex.Message, "ERROR");
        }
    }

    public async Task<ApiResponse<Card>> UpdateCardAsync(string cardId, string? name = null, string? desc = null,
        string? due = null, string? listId = null, string? labels = null, string? members = null)
    {
        try
        {
            var formData = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(name))
                formData["name"] = name;
            if (desc != null)
                formData["desc"] = desc;
            if (due != null)
                formData["due"] = due;
            if (!string.IsNullOrEmpty(listId))
                formData["idList"] = listId;
            if (labels != null)
                formData["idLabels"] = labels;
            if (members != null)
                formData["idMembers"] = members;

            if (formData.Count == 0)
                return ApiResponse<Card>.Fail("No update parameters provided", "NO_PARAMS");

            var url = BuildUrl($"/cards/{cardId}");
            var content = new FormUrlEncodedContent(formData);
            var response = await _http.PutAsync(url, content);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            var card = JsonSerializer.Deserialize<Card>(responseContent);
            return card != null
                ? ApiResponse<Card>.Success(card)
                : ApiResponse<Card>.Fail("Failed to update card", "UPDATE_FAILED");
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return ApiResponse<Card>.Fail("Card not found", "NOT_FOUND");
        }
        catch (HttpRequestException ex)
        {
            return ApiResponse<Card>.Fail(ex.Message, "HTTP_ERROR");
        }
        catch (Exception ex)
        {
            return ApiResponse<Card>.Fail(ex.Message, "ERROR");
        }
    }

    public async Task<ApiResponse<Card>> MoveCardAsync(string cardId, string listId)
    {
        return await UpdateCardAsync(cardId, listId: listId);
    }

    public async Task<ApiResponse<bool>> DeleteCardAsync(string cardId)
    {
        try
        {
            var url = BuildUrl($"/cards/{cardId}");
            var response = await _http.DeleteAsync(url);
            response.EnsureSuccessStatusCode();
            return ApiResponse<bool>.Success(true);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return ApiResponse<bool>.Fail("Card not found", "NOT_FOUND");
        }
        catch (HttpRequestException ex)
        {
            return ApiResponse<bool>.Fail(ex.Message, "HTTP_ERROR");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.Fail(ex.Message, "ERROR");
        }
    }

    // Auth check
    public async Task<ApiResponse<object>> CheckAuthAsync()
    {
        try
        {
            var url = BuildUrl("/members/me", "fields=id,username,fullName");
            var response = await _http.GetStringAsync(url);
            var data = JsonSerializer.Deserialize<JsonElement>(response);
            return ApiResponse<object>.Success(new
            {
                id = data.GetProperty("id").GetString(),
                username = data.GetProperty("username").GetString(),
                fullName = data.GetProperty("fullName").GetString()
            });
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            return ApiResponse<object>.Fail("Invalid API key or token", "UNAUTHORIZED");
        }
        catch (HttpRequestException ex)
        {
            return ApiResponse<object>.Fail(ex.Message, "HTTP_ERROR");
        }
        catch (Exception ex)
        {
            return ApiResponse<object>.Fail(ex.Message, "ERROR");
        }
    }
}
