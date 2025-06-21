namespace DiscordIntegration.Databases;

using Newtonsoft.Json;
using DiscordIntegration.Services;

public class Roles
{
    public Dictionary<byte, Dictionary<ulong, Role>> List { get; set; } = new();
    public static Roles Default => new()
    {
        List = []
    };

    public static Roles GetRoles()
    {
        string _path = Path.Combine(Environment.CurrentDirectory, @"configs", "roles.json");
        if (File.Exists(_path))
        {
            return JsonConvert.DeserializeObject<Roles>(File.ReadAllText(_path))!;
        };

        if (!Directory.Exists(Path.Combine(Environment.CurrentDirectory, @"configs")))
        {
            Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, @"configs"));
        };

        File.WriteAllText(_path, JsonConvert.SerializeObject(Default, Formatting.Indented));
        Log.Info("Roles database created!");
        return Default;
    }

    public void SaveRoles() 
    {
        string _path = Path.Combine(Environment.CurrentDirectory, @"configs", "roles.json");
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

public struct Role
{
    public string IngameId { get; set; }
    public byte Priority { get; set; }
}