using System.IO;
using System.Text.Json;

namespace Campus_Connect_Startup.Services;

public sealed class AppConfig
{
    public FirebaseConfig Firebase { get; set; } = new();

    public static AppConfig Load()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
        if (!File.Exists(path))
        {
            return new AppConfig();
        }

        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<AppConfig>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new AppConfig();
    }
}

public sealed class FirebaseConfig
{
    public string ApiKey { get; set; } = string.Empty;
}
