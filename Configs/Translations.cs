namespace DiscordIntegration.Configs;

using Newtonsoft.Json;
using DiscordIntegration.Services;

public class Translations
{
    public Dictionary<string, string> List = new Dictionary<string, string>();

    public static Translations Default => new()
    {
        List = new Dictionary<string, string>() 
        {
            {
                "ServerStatusConnected",
                "\U0001f7e2 | {name} - {users}/25 players"
            },
            {
                "ServerStatusEmpty",
                "⚫ | {name} - empty"
            },
            {
                "ServerStatusDisconnected",
                "🔴 | {name} - disconnected"
            },
            {
                "UserListServerName",
                "{name} - 👥 {users}/25"
            },
            {
                "UserListEmpty",
                "Empty"
            }
        }
    };

    public static Translations GetTranslations()
    {
        string _path = Path.Combine(Environment.CurrentDirectory, @"configs", "translations.json");
        if (File.Exists(_path))
        {
            return JsonConvert.DeserializeObject<Translations>(File.ReadAllText(_path))!;
        };

        if (!Directory.Exists(Path.Combine(Environment.CurrentDirectory, @"configs")))
        {
            Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, @"configs"));
        };

        File.WriteAllText(_path, JsonConvert.SerializeObject(Default, Formatting.Indented));
        Log.Info("Translation files created!");
        return Default;
    }
}