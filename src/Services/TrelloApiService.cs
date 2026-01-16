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

    // Comment operations
    public async Task<ApiResponse<List<Comment>>> GetCommentsAsync(string cardId)
    {
        try
        {
            var url = BuildUrl($"/cards/{cardId}/actions", "filter=commentCard");
            var response = await _http.GetStringAsync(url);
            var comments = JsonSerializer.Deserialize<List<Comment>>(response) ?? new();
            return ApiResponse<List<Comment>>.Success(comments);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return ApiResponse<List<Comment>>.Fail("Card not found", "NOT_FOUND");
        }
        catch (HttpRequestException ex)
        {
            return ApiResponse<List<Comment>>.Fail(ex.Message, "HTTP_ERROR");
        }
        catch (Exception ex)
        {
            return ApiResponse<List<Comment>>.Fail(ex.Message, "ERROR");
        }
    }

    public async Task<ApiResponse<Comment>> AddCommentAsync(string cardId, string text)
    {
        try
        {
            var url = BuildUrl($"/cards/{cardId}/actions/comments", $"text={Uri.EscapeDataString(text)}");
            var response = await _http.PostAsync(url, null);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var comment = JsonSerializer.Deserialize<Comment>(content);
            return comment != null
                ? ApiResponse<Comment>.Success(comment)
                : ApiResponse<Comment>.Fail("Failed to add comment", "CREATE_FAILED");
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return ApiResponse<Comment>.Fail("Card not found", "NOT_FOUND");
        }
        catch (HttpRequestException ex)
        {
            return ApiResponse<Comment>.Fail(ex.Message, "HTTP_ERROR");
        }
        catch (Exception ex)
        {
            return ApiResponse<Comment>.Fail(ex.Message, "ERROR");
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

    // Attachment operations
    public async Task<ApiResponse<List<Attachment>>> GetAttachmentsAsync(string cardId)
    {
        try
        {
            var url = BuildUrl($"/cards/{cardId}/attachments");
            var response = await _http.GetStringAsync(url);
            var attachments = JsonSerializer.Deserialize<List<Attachment>>(response) ?? new();
            return ApiResponse<List<Attachment>>.Success(attachments);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return ApiResponse<List<Attachment>>.Fail("Card not found", "NOT_FOUND");
        }
        catch (HttpRequestException ex)
        {
            return ApiResponse<List<Attachment>>.Fail(ex.Message, "HTTP_ERROR");
        }
        catch (Exception ex)
        {
            return ApiResponse<List<Attachment>>.Fail(ex.Message, "ERROR");
        }
    }

    public async Task<ApiResponse<Attachment>> GetAttachmentAsync(string cardId, string attachmentId)
    {
        try
        {
            var url = BuildUrl($"/cards/{cardId}/attachments/{attachmentId}");
            var response = await _http.GetStringAsync(url);
            var attachment = JsonSerializer.Deserialize<Attachment>(response);
            return attachment != null
                ? ApiResponse<Attachment>.Success(attachment)
                : ApiResponse<Attachment>.Fail("Attachment not found", "NOT_FOUND");
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return ApiResponse<Attachment>.Fail("Attachment not found", "NOT_FOUND");
        }
        catch (HttpRequestException ex)
        {
            return ApiResponse<Attachment>.Fail(ex.Message, "HTTP_ERROR");
        }
        catch (Exception ex)
        {
            return ApiResponse<Attachment>.Fail(ex.Message, "ERROR");
        }
    }

    public async Task<ApiResponse<Attachment>> UploadAttachmentAsync(string cardId, string filePath, string? name = null)
    {
        try
        {
            if (!File.Exists(filePath))
                return ApiResponse<Attachment>.Fail($"File not found: {filePath}", "FILE_NOT_FOUND");

            var url = BuildUrl($"/cards/{cardId}/attachments");

            using var content = new MultipartFormDataContent();
            using var fileStream = File.OpenRead(filePath);
            var streamContent = new StreamContent(fileStream);

            var fileName = name ?? Path.GetFileName(filePath);
            streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(
                GetMimeType(filePath));

            content.Add(streamContent, "file", fileName);

            if (!string.IsNullOrEmpty(name))
                content.Add(new StringContent(name), "name");

            var response = await _http.PostAsync(url, content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var attachment = JsonSerializer.Deserialize<Attachment>(responseContent);

            return attachment != null
                ? ApiResponse<Attachment>.Success(attachment)
                : ApiResponse<Attachment>.Fail("Failed to upload attachment", "UPLOAD_FAILED");
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return ApiResponse<Attachment>.Fail("Card not found", "NOT_FOUND");
        }
        catch (HttpRequestException ex)
        {
            return ApiResponse<Attachment>.Fail(ex.Message, "HTTP_ERROR");
        }
        catch (Exception ex)
        {
            return ApiResponse<Attachment>.Fail(ex.Message, "ERROR");
        }
    }

    public async Task<ApiResponse<Attachment>> AttachUrlAsync(string cardId, string attachUrl, string? name = null)
    {
        try
        {
            var url = BuildUrl($"/cards/{cardId}/attachments");

            var formData = new Dictionary<string, string>
            {
                ["url"] = attachUrl
            };
            if (!string.IsNullOrEmpty(name))
                formData["name"] = name;

            var content = new FormUrlEncodedContent(formData);
            var response = await _http.PostAsync(url, content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var attachment = JsonSerializer.Deserialize<Attachment>(responseContent);

            return attachment != null
                ? ApiResponse<Attachment>.Success(attachment)
                : ApiResponse<Attachment>.Fail("Failed to attach URL", "ATTACH_FAILED");
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return ApiResponse<Attachment>.Fail("Card not found", "NOT_FOUND");
        }
        catch (HttpRequestException ex)
        {
            return ApiResponse<Attachment>.Fail(ex.Message, "HTTP_ERROR");
        }
        catch (Exception ex)
        {
            return ApiResponse<Attachment>.Fail(ex.Message, "ERROR");
        }
    }

    public async Task<ApiResponse<bool>> DeleteAttachmentAsync(string cardId, string attachmentId)
    {
        try
        {
            var url = BuildUrl($"/cards/{cardId}/attachments/{attachmentId}");
            var response = await _http.DeleteAsync(url);
            response.EnsureSuccessStatusCode();
            return ApiResponse<bool>.Success(true);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return ApiResponse<bool>.Fail("Attachment not found", "NOT_FOUND");
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

    // Checklist operations
    public async Task<ApiResponse<List<Checklist>>> GetChecklistsAsync(string cardId)
    {
        try
        {
            var url = BuildUrl($"/cards/{cardId}/checklists");
            var response = await _http.GetStringAsync(url);
            var checklists = JsonSerializer.Deserialize<List<Checklist>>(response) ?? new();
            return ApiResponse<List<Checklist>>.Success(checklists);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return ApiResponse<List<Checklist>>.Fail("Card not found", "NOT_FOUND");
        }
        catch (HttpRequestException ex)
        {
            return ApiResponse<List<Checklist>>.Fail(ex.Message, "HTTP_ERROR");
        }
        catch (Exception ex)
        {
            return ApiResponse<List<Checklist>>.Fail(ex.Message, "ERROR");
        }
    }

    public async Task<ApiResponse<Checklist>> CreateChecklistAsync(string cardId, string name)
    {
        try
        {
            var url = BuildUrl("/checklists", $"idCard={cardId}&name={Uri.EscapeDataString(name)}");
            var response = await _http.PostAsync(url, null);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var checklist = JsonSerializer.Deserialize<Checklist>(content);
            return checklist != null
                ? ApiResponse<Checklist>.Success(checklist)
                : ApiResponse<Checklist>.Fail("Failed to create checklist", "CREATE_FAILED");
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return ApiResponse<Checklist>.Fail("Card not found", "NOT_FOUND");
        }
        catch (HttpRequestException ex)
        {
            return ApiResponse<Checklist>.Fail(ex.Message, "HTTP_ERROR");
        }
        catch (Exception ex)
        {
            return ApiResponse<Checklist>.Fail(ex.Message, "ERROR");
        }
    }

    public async Task<ApiResponse<bool>> DeleteChecklistAsync(string checklistId)
    {
        try
        {
            var url = BuildUrl($"/checklists/{checklistId}");
            var response = await _http.DeleteAsync(url);
            response.EnsureSuccessStatusCode();
            return ApiResponse<bool>.Success(true);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return ApiResponse<bool>.Fail("Checklist not found", "NOT_FOUND");
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

    public async Task<ApiResponse<ChecklistItem>> AddChecklistItemAsync(string checklistId, string name)
    {
        try
        {
            var url = BuildUrl($"/checklists/{checklistId}/checkItems", $"name={Uri.EscapeDataString(name)}");
            var response = await _http.PostAsync(url, null);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var item = JsonSerializer.Deserialize<ChecklistItem>(content);
            return item != null
                ? ApiResponse<ChecklistItem>.Success(item)
                : ApiResponse<ChecklistItem>.Fail("Failed to add checklist item", "CREATE_FAILED");
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return ApiResponse<ChecklistItem>.Fail("Checklist not found", "NOT_FOUND");
        }
        catch (HttpRequestException ex)
        {
            return ApiResponse<ChecklistItem>.Fail(ex.Message, "HTTP_ERROR");
        }
        catch (Exception ex)
        {
            return ApiResponse<ChecklistItem>.Fail(ex.Message, "ERROR");
        }
    }

    public async Task<ApiResponse<ChecklistItem>> UpdateChecklistItemAsync(string cardId, string checkItemId, string state)
    {
        try
        {
            var url = BuildUrl($"/cards/{cardId}/checkItem/{checkItemId}");
            var formData = new Dictionary<string, string>
            {
                ["state"] = state
            };
            var content = new FormUrlEncodedContent(formData);
            var response = await _http.PutAsync(url, content);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            var item = JsonSerializer.Deserialize<ChecklistItem>(responseContent);
            return item != null
                ? ApiResponse<ChecklistItem>.Success(item)
                : ApiResponse<ChecklistItem>.Fail("Failed to update checklist item", "UPDATE_FAILED");
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return ApiResponse<ChecklistItem>.Fail("Card or checklist item not found", "NOT_FOUND");
        }
        catch (HttpRequestException ex)
        {
            return ApiResponse<ChecklistItem>.Fail(ex.Message, "HTTP_ERROR");
        }
        catch (Exception ex)
        {
            return ApiResponse<ChecklistItem>.Fail(ex.Message, "ERROR");
        }
    }

    public async Task<ApiResponse<bool>> DeleteChecklistItemAsync(string checklistId, string checkItemId)
    {
        try
        {
            var url = BuildUrl($"/checklists/{checklistId}/checkItems/{checkItemId}");
            var response = await _http.DeleteAsync(url);
            response.EnsureSuccessStatusCode();
            return ApiResponse<bool>.Success(true);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return ApiResponse<bool>.Fail("Checklist or item not found", "NOT_FOUND");
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

    private static string GetMimeType(string filePath)
    {
        var ext = Path.GetExtension(filePath).ToLowerInvariant();
        return ext switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            ".pdf" => "application/pdf",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".xls" => "application/vnd.ms-excel",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".ppt" => "application/vnd.ms-powerpoint",
            ".pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
            ".txt" => "text/plain",
            ".csv" => "text/csv",
            ".zip" => "application/zip",
            ".json" => "application/json",
            ".xml" => "application/xml",
            ".html" or ".htm" => "text/html",
            ".css" => "text/css",
            ".js" => "application/javascript",
            ".mp3" => "audio/mpeg",
            ".mp4" => "video/mp4",
            ".mov" => "video/quicktime",
            _ => "application/octet-stream"
        };
    }
}
