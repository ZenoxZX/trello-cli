using System.Text.Json;

namespace TrelloCli.Services;

public class ConfigService
{
    private static readonly string ConfigDir = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
        ".trello-cli"
    );
    private static readonly string ConfigFile = Path.Combine(ConfigDir, "config.json");

    public string? ApiKey { get; private set; }
    public string? Token { get; private set; }
    public bool IsConfigured => !string.IsNullOrEmpty(ApiKey) && !string.IsNullOrEmpty(Token);

    public ConfigService()
    {
        // Priority: Environment variables > Config file
        ApiKey = Environment.GetEnvironmentVariable("TRELLO_API_KEY");
        Token = Environment.GetEnvironmentVariable("TRELLO_TOKEN");

        // If not in env, try config file
        if (string.IsNullOrEmpty(ApiKey) || string.IsNullOrEmpty(Token))
        {
            LoadFromFile();
        }
    }

    private void LoadFromFile()
    {
        if (!File.Exists(ConfigFile)) return;

        try
        {
            var json = File.ReadAllText(ConfigFile);
            var config = JsonSerializer.Deserialize<ConfigData>(json);
            if (config != null)
            {
                ApiKey ??= config.ApiKey;
                Token ??= config.Token;
            }
        }
        catch
        {
            // Ignore file read errors
        }
    }

    public static (bool success, string? error) SaveAuth(string apiKey, string token)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(apiKey))
                return (false, "API Key cannot be empty");
            if (string.IsNullOrWhiteSpace(token))
                return (false, "Token cannot be empty");

            Directory.CreateDirectory(ConfigDir);

            var config = new ConfigData { ApiKey = apiKey, Token = token };
            var json = JsonSerializer.Serialize(config);
            File.WriteAllText(ConfigFile, json);

            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    public static (bool success, string? error) ClearAuth()
    {
        try
        {
            if (File.Exists(ConfigFile))
                File.Delete(ConfigFile);
            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    public string GetAuthQuery()
    {
        return $"key={ApiKey}&token={Token}";
    }

    public (bool valid, string? error) Validate()
    {
        if (string.IsNullOrEmpty(ApiKey))
            return (false, "API Key not set. Use: trello-cli --set-auth <api-key> <token>");

        if (string.IsNullOrEmpty(Token))
            return (false, "Token not set. Use: trello-cli --set-auth <api-key> <token>");

        return (true, null);
    }

    private class ConfigData
    {
        public string? ApiKey { get; set; }
        public string? Token { get; set; }
    }
}
