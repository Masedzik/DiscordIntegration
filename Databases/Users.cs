namespace DiscordIntegration.Databases;

using Newtonsoft.Json;
using DiscordIntegration.Services;

public class Users
{
    public Dictionary<ulong, string> List = new();
    public static Users Default => new()
    {
        List = []
    };

    public static Users GetUsers()
    {
        string _path = Path.Combine(Environment.CurrentDirectory, @"configs", "users.json");
        if (File.Exists(_path))
        {
            return JsonConvert.DeserializeObject<Users>(File.ReadAllText(_path))!;
        };

        if (!Directory.Exists(Path.Combine(Environment.CurrentDirectory, @"configs")))
        {
            Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, @"configs"));
        };

        File.WriteAllText(_path, JsonConvert.SerializeObject(Default, Formatting.Indented));
        Log.Info("Users database created!");
        return Default;
    }

    public void SaveUsers()
    {
        string _path = Path.Combine(Environment.CurrentDirectory, @"configs", "users.json");
        if (!Directory.Exists(Path.Combine(Environment.CurrentDirectory, @"configs")))
        {
            Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, @"configs"));
        };

        using (var sw = new StreamWriter(_path))
        {
            sw.Write(JsonConvert.SerializeObject(this, new JsonSerializerSettings() { Formatting = Formatting.Indented }));
        };
    }
}