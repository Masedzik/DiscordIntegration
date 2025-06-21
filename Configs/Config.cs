namespace DiscordIntegration.Configs;

using Newtonsoft.Json;
using DiscordIntegration.Services;

public class Config
{
    public Dictionary<byte, bool> Enabled = new Dictionary<byte, bool>();
    public Dictionary<byte, string> BotTokens { get; set; } = new();
    public Dictionary<byte, ulong> DiscordServerIds { get; set; } = new();

    public Dictionary<byte, DiscordChannels> Channels { get; set; } = new();
    public Dictionary<byte, ListSettings> PlayerListSettings { get; set; } = new();

    public Dictionary<byte, ScpServer> Servers { get; set; } = new();

    public List<string> Patterns { get; set; } = new();
    public DatabaseConnection Database { get; set; } = new DatabaseConnection();

    public static Config Default => new()
    {
        Enabled = new Dictionary<byte, bool>
        {
            {
                1, true
            }
        },

        BotTokens = new Dictionary<byte, string>
        {
            {
                1,
                "bot-token-here"
            }
        },

        DiscordServerIds = new Dictionary<byte, ulong>
        {
            {
                1,
                0
            }
        },

        Channels = new Dictionary<byte, DiscordChannels> 
        {
            {
                1,
                new DiscordChannels()
                {
                    PlayerList = 0,
                    ServerLog = 0
                }
            }
        },

        PlayerListSettings = new Dictionary<byte, ListSettings>
        {
            {
                1,
                new ListSettings()
                {
                    Timeout = 30000,
                    Color = "#ff0000"
                }
            }
        },

        Servers = new Dictionary<byte, ScpServer> 
        {
            {
                1,
                new ScpServer()
                {
                    Server = "localhost:9000",
                    Name = "Server #1",
                    Password = "Password",
                }
            }
        },

        Patterns = new List<string>()
        {
            "^[0-9]{17}@steam$",
            "^{17,18}$",
            "^*@northwood$"
        },

        Database = new DatabaseConnection()
        {
            Enabled = false,
            Server = "ip",
            Username = "username",
            Password = "password",
            Database = "database"
        }

    };

    public static Config GetConfig()
    {
        string _path = Path.Combine(Environment.CurrentDirectory, @"configs", "config.json");
        if (File.Exists(_path))
        {
            return JsonConvert.DeserializeObject<Config>(File.ReadAllText(_path))!;
        };

        if (!Directory.Exists(Path.Combine(Environment.CurrentDirectory, @"configs")))
        {
            Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, @"configs"));
        };

        File.WriteAllText(_path, JsonConvert.SerializeObject(Default, Formatting.Indented));
        Log.Info("Config files created!");
        return Default;
    }
    public struct DiscordChannels 
    {
        public ulong PlayerList;
        public ulong ServerLog;
    }

    public struct ListSettings 
    {
        public short Timeout;
        public string Color;
    }

    public struct ScpServer 
    {
        public string Server;
        public string Name;
        public string Password;
    }

    public struct DatabaseConnection
    {
        public bool Enabled;
        public short Timeout;
        public string Server;
        public string Database;
        public string Username;
        public string Password;
    }
}