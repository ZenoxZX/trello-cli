using System.Net;
using System.Reflection;
using System.Text.Json;
using TrelloCli.Commands;
using TrelloCli.Services;
using TrelloCli.Utils;

Environment.SetEnvironmentVariable("TRELLO_API_KEY", "test-key");
Environment.SetEnvironmentVariable("TRELLO_TOKEN", "test-token");
var handler = new CaptureHandler();
var api = new TrelloApiService(new ConfigService());
typeof(TrelloApiService).GetField("_http", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(api, new HttpClient(handler));
var commands = new CardCommands(api);
var count = 0;
foreach (var pos in new string?[] { "top", "bottom", "123.5", null })
{
    foreach (var operation in new[] { "create", "move", "update" })
    {
        handler.Calls = 0;
        var output = new StringWriter();
        var original = Console.Out;
        Console.SetOut(output);
        try
        {
            if (operation == "create") await commands.CreateCardAsync("list", "Task", pos: pos);
            if (operation == "move") await commands.MoveCardAsync("card", "list", pos: pos);
            if (operation == "update") await commands.UpdateCardAsync("card", "Task", null, null, null, null, pos: pos);
        }
        finally { Console.SetOut(original); }
        Check(JsonDocument.Parse(output.ToString()).RootElement.GetProperty("ok").GetBoolean(), "command failed");
        Check(handler.Calls == 1, "must use exactly one request");
        Check(handler.Method == (operation == "create" ? HttpMethod.Post : HttpMethod.Put), "wrong method");
        Check(handler.Path == (operation == "create" ? "/1/cards" : "/1/cards/card"), "wrong endpoint");
        var fields = handler.Body.Split('&').Select(x => x.Split('=', 2)).ToDictionary(x => x[0], x => Uri.UnescapeDataString(x[1]));
        Check(pos == null ? !fields.ContainsKey("pos") : fields.GetValueOrDefault("pos") == pos, "position not sent correctly");
        if (operation != "update") Check(fields.GetValueOrDefault("idList") == "list", "list missing");
        count++;
    }
}
handler.Calls = 0;
await commands.UpdateCardAsync("card", null, null, null, null, null, pos: "top");
Check(handler.Calls == 1, "position-only update must send request");
foreach (var value in new[] { "top", "bottom", "123.5", "0.25", "140737488470016" })
    Check(CardPosition.TryRead(["--update-card", "card", "--pos", value], out var position, out _) && position == value, "valid position rejected");
foreach (var value in new[] { "", "middle", "0", "-1", "NaN", "Infinity", "1,5", "--name", "1e999", " " })
    Check(!CardPosition.TryRead(["--update-card", "card", "--pos", value], out _, out _), "invalid position accepted");
Check(!CardPosition.TryRead(["--move-card", "card", "list", "--pos"], out _, out _), "missing position accepted");
Check(!CardPosition.TryRead(["--update-card", "card", "--pos", "top", "--pos", "bottom"], out _, out _), "duplicate position accepted");
Check(CardPosition.TryRead(["--create-card", "list", "Task"], out var absent, out _) && absent == null, "optional position required");
foreach (var arguments in new[] {
    new[] { "--create-card", "list", "--pos" },
    new[] { "--update-card", "card", "--desc", "--pos" },
    new[] { "--create-card", "list", "Task", "--desc", "--pos" }
})
    Check(CardPosition.TryRead(arguments, out var literal, out _) && literal == null, "literal option text was parsed as position");
Check(CardPosition.TryRead(["--create-card", "list", "--pos", "--desc", "--pos", "--pos", "top"], out var mixed, out _) && mixed == "top", "position after literal text was lost");
Console.WriteLine($"PASS: {count + 1} HTTP request checks, 22 position parsing checks");

static void Check(bool ok, string message) { if (!ok) throw new Exception(message); }
sealed class CaptureHandler : HttpMessageHandler
{
    public int Calls;
    public string Body = "";
    public string Path = "";
    public HttpMethod? Method;
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Calls++;
        Body = await request.Content!.ReadAsStringAsync(cancellationToken);
        Path = request.RequestUri!.AbsolutePath;
        Method = request.Method;
        return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{\"id\":\"card\",\"name\":\"Task\",\"pos\":123.5}") };
    }
}
