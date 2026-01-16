using TrelloCli.Commands;
using TrelloCli.Models;
using TrelloCli.Services;
using TrelloCli.Utils;

const string Version = "1.0.0";

var config = new ConfigService();

// Check for help/version first
if (args.Length == 0 || args[0] == "--help" || args[0] == "-h")
{
    ShowHelp();
    return;
}

if (args[0] == "--version" || args[0] == "-v")
{
    Console.WriteLine($"trello-cli v{Version}");
    return;
}

// Handle auth config commands (no validation needed)
if (args[0] == "--set-auth")
{
    var apiKey = args.Length > 1 ? args[1] : string.Empty;
    var token = args.Length > 2 ? args[2] : string.Empty;

    if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(token))
    {
        OutputFormatter.Print(ApiResponse<object>.Fail("Usage: trello-cli --set-auth <api-key> <token>", "MISSING_PARAM"));
        return;
    }

    var (success, error) = ConfigService.SaveAuth(apiKey, token);
    if (success)
        OutputFormatter.Print(ApiResponse<object>.Success(new { message = "Auth saved to ~/.trello-cli/config.json" }));
    else
        OutputFormatter.Print(ApiResponse<object>.Fail(error!, "SAVE_ERROR"));
    return;
}

if (args[0] == "--clear-auth")
{
    var (success, error) = ConfigService.ClearAuth();
    if (success)
        OutputFormatter.Print(ApiResponse<object>.Success(new { message = "Auth cleared" }));
    else
        OutputFormatter.Print(ApiResponse<object>.Fail(error!, "CLEAR_ERROR"));
    return;
}

// Validate auth for all other commands except check-auth
if (args[0] != "--check-auth")
{
    var (valid, error) = config.Validate();
    if (!valid)
    {
        OutputFormatter.Print(ApiResponse<object>.Fail(error!, "AUTH_ERROR"));
        return;
    }
}

var api = new TrelloApiService(config);
var boardCmd = new BoardCommands(api);
var listCmd = new ListCommands(api);
var cardCmd = new CardCommands(api);
var attachCmd = new AttachmentCommands(api);
var checklistCmd = new ChecklistCommands(api);

try
{
    await ExecuteCommand(args);
}
catch (Exception ex)
{
    OutputFormatter.Print(ApiResponse<object>.Fail(ex.Message, "ERROR"));
}

async Task ExecuteCommand(string[] args)
{
    var command = args[0];

    switch (command)
    {
        // Auth
        case "--check-auth":
            var (valid, error) = config.Validate();
            if (!valid)
            {
                OutputFormatter.Print(ApiResponse<object>.Fail(error!, "AUTH_ERROR"));
                return;
            }
            var authResult = await api.CheckAuthAsync();
            OutputFormatter.Print(authResult);
            break;

        // Board commands
        case "--get-boards":
            await boardCmd.GetBoardsAsync();
            break;

        case "--get-board":
            await boardCmd.GetBoardAsync(GetArg(args, 1));
            break;

        // List commands
        case "--get-lists":
            await listCmd.GetListsAsync(GetArg(args, 1));
            break;

        case "--create-list":
            await listCmd.CreateListAsync(GetArg(args, 1), GetArg(args, 2));
            break;

        // Card commands
        case "--get-cards":
            await cardCmd.GetCardsAsync(GetArg(args, 1));
            break;

        case "--get-all-cards":
            await cardCmd.GetAllCardsAsync(GetArg(args, 1));
            break;

        case "--get-card":
            await cardCmd.GetCardAsync(GetArg(args, 1));
            break;

        case "--create-card":
            await cardCmd.CreateCardAsync(
                GetArg(args, 1),
                GetArg(args, 2),
                GetNamedArg(args, "--desc"),
                GetNamedArg(args, "--due")
            );
            break;

        case "--update-card":
            await cardCmd.UpdateCardAsync(
                GetArg(args, 1),
                GetNamedArg(args, "--name"),
                GetNamedArg(args, "--desc"),
                GetNamedArg(args, "--due"),
                GetNamedArg(args, "--labels"),
                GetNamedArg(args, "--members")
            );
            break;

        case "--move-card":
            await cardCmd.MoveCardAsync(GetArg(args, 1), GetArg(args, 2));
            break;

        case "--delete-card":
            await cardCmd.DeleteCardAsync(GetArg(args, 1));
            break;

        case "--get-comments":
            await cardCmd.GetCommentsAsync(GetArg(args, 1));
            break;

        case "--add-comment":
            await cardCmd.AddCommentAsync(GetArg(args, 1), GetArg(args, 2));
            break;

        // Attachment commands
        case "--list-attachments":
            await attachCmd.GetAttachmentsAsync(GetArg(args, 1));
            break;

        case "--upload-attachment":
            await attachCmd.UploadAttachmentAsync(
                GetArg(args, 1),
                GetArg(args, 2),
                GetNamedArg(args, "--name")
            );
            break;

        case "--attach-url":
            await attachCmd.AttachUrlAsync(
                GetArg(args, 1),
                GetArg(args, 2),
                GetNamedArg(args, "--name")
            );
            break;

        case "--delete-attachment":
            await attachCmd.DeleteAttachmentAsync(GetArg(args, 1), GetArg(args, 2));
            break;

        // Checklist commands
        case "--get-checklists":
            await checklistCmd.GetChecklistsAsync(GetArg(args, 1));
            break;

        case "--create-checklist":
            await checklistCmd.CreateChecklistAsync(GetArg(args, 1), GetArg(args, 2));
            break;

        case "--delete-checklist":
            await checklistCmd.DeleteChecklistAsync(GetArg(args, 1));
            break;

        case "--add-checklist-item":
            await checklistCmd.AddChecklistItemAsync(GetArg(args, 1), GetArg(args, 2));
            break;

        case "--update-checklist-item":
            await checklistCmd.UpdateChecklistItemAsync(
                GetArg(args, 1),
                GetArg(args, 2),
                GetArg(args, 3)
            );
            break;

        case "--delete-checklist-item":
            await checklistCmd.DeleteChecklistItemAsync(GetArg(args, 1), GetArg(args, 2));
            break;

        default:
            OutputFormatter.Print(ApiResponse<object>.Fail($"Unknown command: {command}", "UNKNOWN_COMMAND"));
            break;
    }
}

string GetArg(string[] args, int index)
{
    return args.Length > index ? args[index] : string.Empty;
}

string? GetNamedArg(string[] args, string name)
{
    for (int i = 0; i < args.Length - 1; i++)
    {
        if (args[i] == name)
            return args[i + 1];
    }
    return null;
}

void ShowHelp()
{
    Console.WriteLine($@"trello-cli v{Version}
CLI tool for Trello with AI-friendly JSON output

USAGE:
  trello-cli <command> [arguments] [options]

AUTHENTICATION:
  Option 1 - CLI (recommended):
    trello-cli --set-auth <api-key> <token>

  Option 2 - Environment variables:
    TRELLO_API_KEY  - Your Trello API key
    TRELLO_TOKEN    - Your Trello token

  Get credentials: https://trello.com/app-key

COMMANDS:
  --help, -h                          Show this help
  --version, -v                       Show version
  --set-auth <api-key> <token>        Save credentials to ~/.trello-cli/config.json
  --clear-auth                        Remove saved credentials
  --check-auth                        Verify API credentials

  Board:
    --get-boards            List all boards
    --get-board <id>        Get specific board

  List:
    --get-lists <board-id>              Get lists in a board
    --create-list <board-id> <name>     Create new list

  Card:
    --get-cards <list-id>               Get cards in a list
    --get-all-cards <board-id>          Get all cards in a board
    --get-card <card-id>                Get specific card
    --create-card <list-id> <name>      Create card
      [--desc <description>]
      [--due <date>]
    --update-card <card-id>             Update card
      [--name <name>]
      [--desc <description>]
      [--due <date>]
      [--labels <ids>]
      [--members <ids>]
    --move-card <card-id> <list-id>     Move card to list
    --delete-card <card-id>             Delete card
    --get-comments <card-id>            Get comments on a card
    --add-comment <card-id> <text>      Add comment to a card

  Attachment:
    --list-attachments <card-id>                 List attachments on a card
    --upload-attachment <card-id> <file-path>    Upload file as attachment
      [--name <name>]                            Custom attachment name
    --attach-url <card-id> <url>                 Attach URL to card
      [--name <name>]                            Custom attachment name
    --delete-attachment <card-id> <attach-id>    Delete attachment

  Note: Downloading attachments is not supported. Trello's download API
  requires browser authentication. Use --attach-url to link attachments.

  Checklist:
    --get-checklists <card-id>                          Get checklists on a card
    --create-checklist <card-id> <name>                 Create checklist on a card
    --delete-checklist <checklist-id>                   Delete a checklist
    --add-checklist-item <checklist-id> <name>          Add item to checklist
    --update-checklist-item <card-id> <item-id> <state> Mark item complete/incomplete
    --delete-checklist-item <checklist-id> <item-id>    Delete item from checklist

OUTPUT:
  All responses are JSON: {{""ok"":true,""data"":...}} or {{""ok"":false,""error"":""...""}}

EXAMPLES:
  trello-cli --get-boards
  trello-cli --get-board abc123
  trello-cli --create-card xyz789 ""My Task"" --desc ""Details""
  trello-cli --move-card card123 list456
");
}
